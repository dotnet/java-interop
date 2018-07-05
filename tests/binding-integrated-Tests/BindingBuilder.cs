using NUnit.Framework;
using System;
using System.Diagnostics;
using Java.Interop;
using System.IO;
using System.Linq;

namespace BindingIntegrationTests
{
	public class BindingBuilder
	{
		[Flags]
		public enum Steps
		{
			Javac,
			Jar,
			ClassParse,
			ApiXmlAdjuster,
			Generator,
			Csc,
			All = Javac | Jar | ClassParse | ApiXmlAdjuster | Generator | Csc
		}

		public const string JavaSourcesSubDir = "java-sources";
		public const string ClassesSubDir = "classes";

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

		Action<string> ensureDirectory = dir => { if (!Directory.Exists (dir)) Directory.CreateDirectory (dir); };

		void Javac ()
		{
			if ((ProcessSteps & Steps.Javac) != Steps.Javac)
				return;

			if (JdkPath == null)
				throw new InvalidOperationException ("JdkPath is not set.");

			var objDir = IntermediateOutputPathAbsolute;
			ensureDirectory (objDir);

			string sourcesSaved = Path.Combine (objDir, JavaSourcesSubDir);
			ensureDirectory (sourcesSaved);
			foreach (var item in project.JavaSourceStrings)
				File.WriteAllText (Path.Combine (sourcesSaved, item.FileName), item.Content);
			var sourceFiles = project.JavaSourceFiles.Concat (project.JavaSourceStrings.Select (i => Path.Combine (sourcesSaved, i.FileName)));

			if (project.CompiledClassesDirectory == null)
				project.CompiledClassesDirectory = Path.Combine (objDir, ClassesSubDir);
			ensureDirectory (project.CompiledClassesDirectory);

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
			if ((ProcessSteps & Steps.Jar) != Steps.Jar)
				return;

			if (JdkPath == null)
				throw new InvalidOperationException ("JdkPath is not set.");

			var objDir = IntermediateOutputPathAbsolute;
			if (project.CompiledClassesDirectory == null)
				project.CompiledClassesDirectory = Path.Combine (objDir, ClassesSubDir);
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
		}

		void AdjustApiXml ()
		{
		}

		void GenerateBindingSources ()
		{
		}

		void CompileBindings ()
		{
		}
	}
}

