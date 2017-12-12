using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using NUnit.Framework;
using Xamarin.Android.Binder;
using System.Collections.Generic;

namespace generatortests
{
	public class BaseGeneratorTest
	{
		StringWriter sw = null;

		[SetUp]
		public void SetUp ()
		{
			Options = new CodeGeneratorOptions ();
			Options.ApiLevel = "4";
			Options.GlobalTypeNames = true;
			Options.EnumFieldsMapFile = null;
			Options.EnumMethodsMapFile = null;
			Options.AssemblyQualifiedName = "Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
			Options.OnlyBindPublicTypes = true;
			sw = new StringWriter ();
			AdditionalSourceDirectories = new List<string> ();
			AdditionalSupportDirectories = new List<string> ();
		}

		[TearDown]
		public void TearDown ()
		{
			if (!string.IsNullOrEmpty (SupportAssembly)) {
				File.Delete (SupportAssembly);
			}
		}

		protected CodeGeneratorOptions Options;
		/// <summary>
		/// The resulting "main" assembly compiled with the InMemory option, think of this as the output assembly of a Java binding project
		/// </summary>
		protected Assembly MainAssembly;
		/// <summary>
		/// Additional source code to be compiled into the "main" assembly
		/// </summary>
		protected List<string> AdditionalSourceDirectories;
		/// <summary>
		/// Additional source code to be compiled into the "support" assembly
		/// </summary>
		protected List<string> AdditionalSupportDirectories;
		/// <summary>
		/// To be removed eventually, this allows a test to ignore compiler warnings
		/// </summary>
		protected bool AllowWarnings;
		/// <summary>
		/// If set, compiles everything to a single assembly. This allows us to simulate how `generator` will be used upstream in xamarin-android/src/Mono.Android.csproj
		/// </summary>
		protected bool CompileToSingleAssembly;
		/// <summary>
		/// The full path to the "support" assembly, saved to disk in $TMP so the "main" assembly can reference it
		/// </summary>
		protected string SupportAssembly;

		public void Execute ()
		{
			CodeGenerator.Run (Options);
			var output = sw.ToString ();
			if (output.Contains ("error")) {
				Assert.Fail (output);
			}
			bool    hasErrors;
			string  compilerOutput;
			string  supportFilePath = typeof (BaseGeneratorTest).Assembly.Location;

			//Compile the "support" assembly, this emulates what is in Mono.Android.dll
			if (!CompileToSingleAssembly) {
				var sourceFiles = Directory.EnumerateFiles (Options.ManagedCallableWrapperSourceOutputDirectory, "Java.Lang.*.cs",
					SearchOption.AllDirectories).ToList ();
				var supportFiles = Directory.EnumerateFiles (Path.Combine (Path.GetDirectoryName (supportFilePath), "SupportFiles"),
					"*.cs", SearchOption.AllDirectories);
				sourceFiles.AddRange (supportFiles);

				foreach (var dir in AdditionalSupportDirectories) {
					var additional = Directory.EnumerateFiles (dir, "*.cs", SearchOption.AllDirectories);
					sourceFiles.AddRange (additional);
				}

				//NOTE: due to the tests generating Java.Lang.Object or Java.Lang.String, we will get some warnings
				SupportAssembly = Compiler.CompileToDisk (sourceFiles, out hasErrors, out compilerOutput, allowWarnings: true);
				Assert.AreEqual (false, hasErrors, compilerOutput);
				FileAssert.Exists (SupportAssembly, "Support assembly did not exist!");
			}

			//Compile the "main" assembly, this emulates a user's assembly in a binding project
			{
				var sourceDirectories = new List<string> (AdditionalSourceDirectories);
				var csharpFiles = Directory.EnumerateFiles (Options.ManagedCallableWrapperSourceOutputDirectory, "*.cs", SearchOption.AllDirectories);
				if (CompileToSingleAssembly) {
					sourceDirectories.AddRange (AdditionalSupportDirectories);
				} else {
					csharpFiles = csharpFiles.Where (x => !Path.GetFileName (x).StartsWith ("Java.Lang.", StringComparison.InvariantCultureIgnoreCase));
				}

				var sourceFiles = csharpFiles.ToList ();
				if (CompileToSingleAssembly) {
					var supportFiles = Directory.EnumerateFiles (Path.Combine (Path.GetDirectoryName (supportFilePath), "SupportFiles"),
						"*.cs", SearchOption.AllDirectories);
					sourceFiles.AddRange (supportFiles);
				}
				foreach (var dir in sourceDirectories) {
					var additional = Directory.EnumerateFiles (dir, "*.cs", SearchOption.AllDirectories);
					sourceFiles.AddRange (additional);
				}

				MainAssembly = Compiler.CompileInMemory (sourceFiles, FullPath ("UserAssembly.dll"), SupportAssembly, out hasErrors, out compilerOutput, AllowWarnings);
				Assert.AreEqual (false, hasErrors, compilerOutput);
				Assert.IsNotNull (MainAssembly);
			}
		}

		protected void CompareOutputs (string sourceDir, string destinationDir)
		{
			if (!Path.IsPathRooted (sourceDir))
				sourceDir = FullPath (sourceDir);
			if (!Path.IsPathRooted (destinationDir))
				destinationDir = FullPath (destinationDir);

			var files = Directory.GetFiles (sourceDir);
			foreach (var file in files) {
				var extension   = Path.GetExtension (file);
				if (extension == ".xml" || extension == ".fixed")
					continue;
				var filename = Path.GetFileName (file);
				var dest = Path.Combine (destinationDir, filename);
				if (!File.Exists (dest)) {
					Assert.Fail (string.Format ("Expected {0} but it was not generated.", dest));
				} else if (!FileCompare (file, dest)) {
					var fullSource  = Path.GetFullPath (file);
					var fullDest    = Path.GetFullPath (dest);
					//Error message for diff in powershell vs bash
					string message  = Environment.OSVersion.Platform == PlatformID.Win32NT ?
						$"File contents differ; run: diff (cat {fullSource}) `{Environment.NewLine}\t(cat {fullDest})" :
						$"File contents differ; run: diff -u {fullSource} \\{Environment.NewLine}\t{fullDest}";
					Assert.Fail (message);
				}
			}
		}

		protected void Cleanup (string path)
		{
			if (!Path.IsPathRooted (path))
				path = FullPath (path);
			if (Directory.Exists (path))
				Directory.Delete (path, true);
		}

		protected bool FileCompare (string file1, string file2)
		{
			bool result = false;

			result = File.Exists (file1) && File.Exists (file2);

			if (result) {
				byte[] f1 = ReadAllBytesIgnoringLineEndings (file1);
				byte[] f2 = ReadAllBytesIgnoringLineEndings (file2);

				var hash = MD5.Create ();
				var f1hash = Convert.ToBase64String (hash.ComputeHash (f1));
				var f2hash = Convert.ToBase64String (hash.ComputeHash (f2));
				result = f1hash.Equals (f2hash);
			}

			return result;
		}

		private byte[] ReadAllBytesIgnoringLineEndings (string path)
		{
			using (var memoryStream = new MemoryStream ()) {
 				using (var file = File.OpenRead (path)) {
 					int readByte;
 					while ((readByte = file.ReadByte()) != -1) {
 						byte b = (byte)readByte;
 						if (b != '\r' && b != '\n') {
 							memoryStream.WriteByte (b);
 						}
 					}
 				}
				return memoryStream.ToArray ();
			}
		}

		protected void RunAllTargets (string outputRelativePath, string apiDescriptionFile, string expectedRelativePath, string[] additionalSupportPaths = null, string[] additionalSourcePaths = null)
		{
			Run (CodeGenerationTarget.XamarinAndroid,   Path.Combine ("out", outputRelativePath),       apiDescriptionFile,     Path.Combine ("expected", expectedRelativePath),        additionalSupportPaths, additionalSourcePaths);
			Run (CodeGenerationTarget.JavaInterop1,     Path.Combine ("out.ji", outputRelativePath),    apiDescriptionFile,     Path.Combine ("expected.ji", expectedRelativePath),     additionalSupportPaths, additionalSourcePaths);
		}

		protected string FullPath (string path)
		{
			var dir = Path.GetDirectoryName (GetType ().Assembly.Location);
			return Path.Combine (dir, path.Replace ('/', Path.DirectorySeparatorChar));
		}

		protected void Run (CodeGenerationTarget target, string outputPath, string apiDescriptionFile, string expectedPath, string[] additionalSupportPaths = null, string[] additionalSourcePaths = null)
		{
			Cleanup (outputPath);

			Options.CodeGenerationTarget                        = target;
			Options.ApiDescriptionFile                          = FullPath (apiDescriptionFile);
			Options.ManagedCallableWrapperSourceOutputDirectory = FullPath (outputPath);

			if (additionalSupportPaths != null) {
				AdditionalSourceDirectories.AddRange (additionalSupportPaths.Select (p => FullPath (p)));
			}
			if (additionalSourcePaths != null) {
				AdditionalSourceDirectories.AddRange (additionalSourcePaths.Select (p => FullPath (p)));
			}

			Execute ();

			CompareOutputs (expectedPath, outputPath);
		}
	}
}
