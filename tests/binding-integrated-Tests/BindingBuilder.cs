using NUnit.Framework;
using System;
using System.Diagnostics;
using Java.Interop;
using System.IO;
using System.Linq;
using Xamarin.Android.Tools.Bytecode;
using Xamarin.Android.Tools.ApiXmlAdjuster;

namespace BindingIntegrationTests
{
	public class BindingBuilder
	{
		[Flags]
		public enum Steps
		{
			Javac = 1,
			Jar = 2,
			ClassParse = 4,
			ApiXmlAdjuster = 8,
			Generator = 16,
			Csc = 32,
			All = Javac | Jar | ClassParse | ApiXmlAdjuster | Generator | Csc
		}

		public const string JavaSourcesSubDir = "java-sources";
		public const string ClassesSubDir = "classes";
		public const string ClassParseSubDir = "class-parse-xml";

		public Steps ProcessSteps { get; set; } = Steps.All;

		// entire work (intermediate output) directory
		public string IntermediateOutputPathRelative { get; set; } = "intermediate-output";

		// Used to resolve javac and rt.jar
		public string JdkPath { get; set; }

		static string ProbeJavaHome ()
		{
			var env = Environment.GetEnvironmentVariable ("JAVA_HOME");
			if (!string.IsNullOrEmpty (env))
				return env;
			return "/usr/lib/jvm/java-8-openjdk-amd64/";
		}

		public static BindingBuilder CreateBestBetDefault (BindingProject project)
		{
			return new BindingBuilder (project) { JdkPath = ProbeJavaHome () };
		}

		public BindingBuilder (BindingProject project)
		{
			this.project = project;
		}

		readonly BindingProject project;

		public string IntermediateOutputPathAbsolute => Path.Combine (Path.GetDirectoryName (new Uri (GetType ().Assembly.CodeBase).LocalPath), IntermediateOutputPathRelative, project.Id);

		public void Clean ()
		{
			if (Directory.Exists (IntermediateOutputPathAbsolute))
				Directory.Delete (IntermediateOutputPathAbsolute, true);
		}

		public void Build ()
		{
			Javac ();
			Jar ();
			ClassParse ();
			AdjustApiXml ();
			GenerateBindingSources ();
			CompileBindings ();
		}

		void EnsureDirectory (string dir)
		{
			var parent = Path.GetDirectoryName (dir);
			if (parent != Path.GetPathRoot (parent))
				EnsureDirectory (parent);
			if (!Directory.Exists (dir))
				Directory.CreateDirectory (dir);
		}

		void Javac ()
		{
			if ((ProcessSteps & Steps.Javac) == 0)
				return;

			if (JdkPath == null)
				throw new InvalidOperationException ("JdkPath is not set.");

			var objDir = IntermediateOutputPathAbsolute;
			EnsureDirectory (objDir);

			string sourcesSaved = Path.Combine (objDir, JavaSourcesSubDir);
			EnsureDirectory (sourcesSaved);
			foreach (var item in project.JavaSourceStrings)
				File.WriteAllText (Path.Combine (sourcesSaved, item.FileName), item.Content);
			var sourceFiles = project.JavaSourceFiles.Concat (project.JavaSourceStrings.Select (i => Path.Combine (sourcesSaved, i.FileName)));

			if (project.CompiledClassesDirectory == null)
				project.CompiledClassesDirectory = Path.Combine (objDir, ClassesSubDir);
			EnsureDirectory (project.CompiledClassesDirectory);

			var psi = new ProcessStartInfo () {
				UseShellExecute = false,
				FileName = JdkPath != null ? Path.Combine (JdkPath, "bin", "javac") : "javac",
				Arguments = $"{project.JavacOptions} -d \"{project.CompiledClassesDirectory}\" {string.Join (" ", sourceFiles.Select (s => '"' + s + '"'))}",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
			};
			if (project.CustomRuntimeJar != null)
				psi.Arguments += $" -bootclasspath {project.CustomRuntimeJar} -classpath {project.CustomRuntimeJar}";

			project.JavacExecutionOutput = $"Execute javac as: {psi.FileName} {psi.Arguments}\n";

			var proc = new Process () { StartInfo = psi };
			proc.OutputDataReceived += (sender, e) => project.JavacExecutionOutput += e.Data;
			proc.ErrorDataReceived += (sender, e) => project.JavacExecutionOutput += e.Data;
			proc.Start ();
			proc.BeginOutputReadLine ();
			proc.BeginErrorReadLine ();
			proc.WaitForExit ();
			if (proc.ExitCode != 0)
				throw new Exception ("Javac failed: " + project.JavacExecutionOutput);
		}

		void Jar ()
		{
			if ((ProcessSteps & Steps.Jar) == 0)
				return;

			if (JdkPath == null)
				throw new InvalidOperationException ("JdkPath is not set.");

			var objDir = IntermediateOutputPathAbsolute;
			if (project.CompiledClassesDirectory == null)
				project.CompiledClassesDirectory = Path.Combine (objDir, ClassesSubDir);
			if (project.CompiledJarFile == null)
				project.CompiledJarFile = Path.Combine (project.CompiledClassesDirectory, project.Id + ".jar");

			var psi = new ProcessStartInfo () {
				UseShellExecute = false,
				FileName = JdkPath != null ? Path.Combine (JdkPath, "bin", "jar") : "jar",
				Arguments = $"cvf \"{project.CompiledJarFile}\" -C \"{project.CompiledClassesDirectory}\" .",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
			};

			project.JarExecutionOutput = $"Execute jar as: {psi.FileName} {psi.Arguments}\n";

			var proc = new Process () { StartInfo = psi };
			proc.OutputDataReceived += (sender, e) => project.JarExecutionOutput += e.Data;
			proc.ErrorDataReceived += (sender, e) => project.JarExecutionOutput += e.Data;
			proc.Start ();
			proc.BeginOutputReadLine ();
			proc.BeginErrorReadLine ();
			proc.WaitForExit ();
			if (proc.ExitCode != 0)
				throw new Exception ("Jar failed: " + project.JarExecutionOutput);
		}

		void ClassParse ()
		{
			if ((ProcessSteps & Steps.ClassParse) == 0)
				return;

			if (project.CompiledJarFile == null && !project.InputJarFiles.Any ())
				throw new InvalidOperationException ("Input Jar files are not set either at CompiledJarFile or InputJarFiles.");

			var objDir = IntermediateOutputPathAbsolute;
			EnsureDirectory (objDir);
			var cpDir = Path.Combine (objDir, ClassParseSubDir);
			EnsureDirectory (cpDir);
			if (project.GeneratedClassParseXmlFile == null)
				project.GeneratedClassParseXmlFile = Path.Combine (cpDir, project.Id + ".class-parse");

			// FIXME: logging
			var cp = new ClassPath ();
			cp.Load (project.CompiledJarFile);
			foreach (var jar in project.InputJarFiles)
				cp.Load (jar);
			cp.SaveXmlDescription (project.GeneratedClassParseXmlFile);
		}

		void AdjustApiXml ()
		{
			if ((ProcessSteps & Steps.ApiXmlAdjuster) == 0)
				return;

			if (project.GeneratedClassParseXmlFile == null)
				throw new InvalidOperationException ("Input class-parse file is not set.");

			var objDir = IntermediateOutputPathAbsolute;
			var cpDir = Path.Combine (objDir, ClassParseSubDir);
			EnsureDirectory (cpDir);
			if (project.GeneratedApiXmlFile == null)
				project.GeneratedApiXmlFile = Path.Combine (objDir, "api.xml");
			foreach (var cpSource in project.ClassParseXmlStrings)
				File.WriteAllText (Path.Combine (cpDir, cpSource.FileName), cpSource.Content);
			var cpFiles = project.ClassParseXmlFiles.Concat (project.ClassParseXmlStrings.Select (i => Path.Combine (cpDir, i.FileName)));

			var writer = new StringWriter ();
			Xamarin.Android.Tools.ApiXmlAdjuster.Log.DefaultWriter = writer;
			var api = new JavaApi ();
			api.Load (project.GeneratedClassParseXmlFile);
			foreach (var apixml in cpFiles)
				api.Load (apixml);
			api.Resolve ();
			api.CreateGenericInheritanceMapping ();
			api.MarkOverrides ();
			api.FindDefects ();
			api.Save (project.GeneratedApiXmlFile);
			project.ApiXmlAdjusterExecutionOutput = writer.ToString ();
			if (project.ApiXmlAdjusterExecutionOutput != string.Empty)
				throw new Exception ("api-xml-adjuster failed: " + project.ApiXmlAdjusterExecutionOutput);
		}

		void GenerateBindingSources ()
		{
		}

		void CompileBindings ()
		{
		}
	}
}

