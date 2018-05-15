using System;
using System.Reflection;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace generatortests
{
	public static class Compiler
	{
		const string RoslynEnvironmentVariable = "ROSLYN_COMPILER_LOCATION";
		private static string unitTestFrameworkAssemblyPath = typeof(Assert).Assembly.Location;

		public static string BinDirectory { get; } = Path.GetDirectoryName (typeof (BaseGeneratorTest).Assembly.Location);

		static CodeDomProvider GetCodeDomProvider ()
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
				//NOTE: there is an issue where Roslyn's csc.exe isn't copied to output for non-ASP.NET projects
				// Comments on this here: https://stackoverflow.com/a/40311406/132442
				// They added an environment variable as a workaround: https://github.com/aspnet/RoslynCodeDomProvider/pull/12
				if (string.IsNullOrEmpty (Environment.GetEnvironmentVariable (RoslynEnvironmentVariable, EnvironmentVariableTarget.Process))) {
					string roslynPath = Path.GetFullPath (Path.Combine (unitTestFrameworkAssemblyPath, "..", "..", "..", "packages", "Microsoft.Net.Compilers.2.1.0", "tools"));
					Environment.SetEnvironmentVariable (RoslynEnvironmentVariable, roslynPath, EnvironmentVariableTarget.Process);
				}

				return new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider ();
			} else {
				return new Microsoft.CSharp.CSharpCodeProvider ();
			}
		}

		public static string CompileToDisk (IEnumerable<string> sourceFiles,
			out bool hasErrors, out string output, bool allowWarnings)
		{
			var path = Path.Combine (Path.GetTempPath (), Path.GetRandomFileName ());
			var parameters = CreateParameters (path, false);
			using (var codeProvider = GetCodeDomProvider ()) {
				var results = codeProvider.CompileAssemblyFromFile (parameters, sourceFiles.ToArray ());

				hasErrors = false;

				foreach (CompilerError message in results.Errors) {
					hasErrors |= !message.IsWarning || !allowWarnings;
				}
				output = string.Join (Environment.NewLine, results.Output.Cast<string> ());

				return results.PathToAssembly;
			}
		}

		public static Assembly CompileInMemory (IEnumerable<string> sourceFiles,
			string assemblyFileName, string supportAssemblyPath,
			out bool hasErrors, out string output, bool allowWarnings)
		{
			var parameters = CreateParameters (assemblyFileName, true);
			parameters.ReferencedAssemblies.Add (supportAssemblyPath);

			using (var codeProvider = GetCodeDomProvider ()) {
				CompilerResults results = codeProvider.CompileAssemblyFromFile (parameters, sourceFiles.ToArray ());

				hasErrors = false;

				foreach (CompilerError message in results.Errors) {
					hasErrors |= !message.IsWarning || !allowWarnings;
				}
				output = string.Join (Environment.NewLine, results.Output.Cast<string> ());

				return results.CompiledAssembly;
			}
		}

		static CompilerParameters CreateParameters (string assemblyFileName, bool inMemory)
		{
			var parameters = new CompilerParameters ();
			parameters.GenerateExecutable = false;
			parameters.GenerateInMemory = inMemory;
			parameters.CompilerOptions = "/unsafe";
			parameters.OutputAssembly = assemblyFileName;
			parameters.ReferencedAssemblies.Add (unitTestFrameworkAssemblyPath);
			parameters.ReferencedAssemblies.Add (typeof (Enumerable).Assembly.Location);
			parameters.ReferencedAssemblies.Add (Path.Combine (BinDirectory, "Java.Interop.dll"));
			parameters.ReferencedAssemblies.Add (GetSystemAssembly ("System.Runtime.dll"));
			parameters.ReferencedAssemblies.Add (GetSystemAssembly ("System.Xml.dll"));
#if DEBUG
			parameters.IncludeDebugInformation = true;
#else
			parameters.IncludeDebugInformation = false;
#endif
			return parameters;
		}

		public static string GetSystemAssembly (string assembly)
		{
			string path;

			var env = Environment.GetEnvironmentVariable ("FACADES_PATH");
			if (!string.IsNullOrEmpty (env)) {
				path = Path.Combine (env, assembly);
				if (File.Exists (path))
					return path;

				path = Path.Combine (env, "..", assembly);
				if (File.Exists (path))
					return path;
			}

			var dir = Path.GetDirectoryName (typeof (object).Assembly.Location);
			path = Path.Combine (dir, assembly);
			if (File.Exists (path))
				return path;

			return Path.Combine (dir, "Facades", assembly);
		}
	}
}

