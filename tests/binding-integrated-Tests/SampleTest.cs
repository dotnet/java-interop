using System;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using NUnit.Framework;

namespace BindingIntegrationTests
{
	// Whenever we write complicated test foundation, we should ensure that it actually works.
	public class SampleTest
	{
		[Test]
		public void YouNeedJdkPath ()
		{
			Assert.Throws<InvalidOperationException> (() => {
				new BindingBuilder (new BindingProject ()).Build ();
			});
		}

		[Test]
		public void VerifyJavac ()
		{
			// You don't even have to create a set of project files. They can be created on the fly.
			var project = new BindingProject { Id = nameof (VerifyJavac) };
			string fooJavaFileName = "Foo.java";
			string fooJavaContent = "public class Foo {}";
			project.JavaSourceStrings.Add (new SourceFile { FileName = fooJavaFileName, Content = fooJavaContent });

			// Set up builder. Hopefully you don't have to provide JDK path, it will be probed.
			var builder = BindingBuilder.CreateBestBetDefault (project);
			builder.ProcessSteps = BindingBuilder.Steps.Javac;
			builder.Clean ();
			builder.Build ();

			var savedFooJavaFile = Path.Combine (builder.IntermediateOutputPathAbsolute, BindingBuilder.JavaSourcesSubDir, fooJavaFileName);
			Assert.IsTrue (File.Exists (savedFooJavaFile), "Java source not saved");
			Assert.AreEqual (fooJavaContent, File.ReadAllText (savedFooJavaFile), "Saved java content mismatch.");
			var classesDir = Path.Combine (builder.IntermediateOutputPathAbsolute, BindingBuilder.ClassesSubDir);
			Assert.AreEqual (classesDir, project.CompiledClassesDirectory, "classes directory mismatch.");
			var fooClassFile = Path.Combine (classesDir, "Foo.class");
			Assert.IsTrue (File.Exists (fooClassFile), "Compiled Foo.class not found");
		}

		[Test]
		public void VerifyJavacWithRtJar ()
		{
			var project = new BindingProject { Id = nameof (VerifyJavacWithRtJar) };
			string fooJavaFileName = "Foo.java";
			string fooJavaContent = "public class Foo {}";
			project.JavaSourceStrings.Add (new SourceFile { FileName = fooJavaFileName, Content = fooJavaContent });

			var builder = BindingBuilder.CreateBestBetDefault (project);
			project.CustomRuntimeJar = Path.Combine (builder.JdkPath, "jre", "lib", "rt.jar");
			Assert.IsTrue (File.Exists (project.CustomRuntimeJar), "rt.jar exists");
			builder.ProcessSteps = BindingBuilder.Steps.Javac;
			builder.Clean ();
			builder.Build ();
		}

		[Test]
		public void VerifyJar ()
		{
			// You don't even have to create a set of project files. They can be created on the fly.
			var project = new BindingProject { Id = nameof (VerifyJar) };
			project.JavaSourceStrings.Add (new SourceFile { FileName = "Foo.java", Content = "public class Foo {}" });
			project.JavaSourceStrings.Add (new SourceFile { FileName = "Bar.java", Content = "public class Bar {}" });

			// Set up builder. Hopefully you don't have to provide JDK path, it will be probed.
			var builder = BindingBuilder.CreateBestBetDefault (project);
			builder.ProcessSteps = BindingBuilder.Steps.Javac | BindingBuilder.Steps.Jar;
			builder.Clean ();
			builder.Build ();

			var jar = Path.Combine (builder.IntermediateOutputPathAbsolute, BindingBuilder.ClassesSubDir, project.Id + ".jar");
			Assert.AreEqual (jar, project.CompiledJarFile, "jar file path mismatch.");
			Assert.IsTrue (File.Exists (project.CompiledJarFile), "Compiled jar not found");
		}

		[Test]
		public void VerifyClassParse ()
		{
			// You don't even have to create a set of project files. They can be created on the fly.
			var project = new BindingProject { Id = nameof (VerifyClassParse) };
			project.JavaSourceStrings.Add (new SourceFile { FileName = "Foo.java", Content = "package com.xamarin.test; public class Foo {}" });
			project.JavaSourceStrings.Add (new SourceFile { FileName = "Bar.java", Content = "package com.xamarin.test; public class Bar {}" });

			// Set up builder. Hopefully you don't have to provide JDK path, it will be probed.
			var builder = BindingBuilder.CreateBestBetDefault (project);
			builder.ProcessSteps = BindingBuilder.Steps.Javac | BindingBuilder.Steps.Jar | BindingBuilder.Steps.ClassParse;
			builder.Clean ();
			builder.Build ();

			var cpxml = Path.Combine (builder.IntermediateOutputPathAbsolute, BindingBuilder.ClassParseSubDir, project.Id + ".class-parse");
			Assert.AreEqual (cpxml, project.GeneratedClassParseXmlFile, "class-parse output file path mismatch.");
			Assert.IsTrue (File.Exists (project.GeneratedClassParseXmlFile), "class-parse output file not found");
			var doc = XDocument.Load (project.GeneratedClassParseXmlFile);
			Assert.IsNotNull (doc.XPathSelectElement ("//class[@name='Foo']"), "Foo node does not exist");
		}

		[Test]
		public void VerifyApiXmlAdjuster ()
		{
			// You don't even have to create a set of project files. They can be created on the fly.
			var project = new BindingProject { Id = nameof (VerifyApiXmlAdjuster) };
			project.JavaSourceStrings.Add (new SourceFile { FileName = "Object.java", Content = "package java.lang; public class Object {}" });
			project.JavaSourceStrings.Add (new SourceFile { FileName = "Foo.java", Content = "package com.xamarin.test; public class Foo {}" });
			project.JavaSourceStrings.Add (new SourceFile { FileName = "Bar.java", Content = "package com.xamarin.test; public class Bar {}" });

			// Set up builder. Hopefully you don't have to provide JDK path, it will be probed.
			var builder = BindingBuilder.CreateBestBetDefault (project);
			builder.ProcessSteps = BindingBuilder.Steps.Javac | BindingBuilder.Steps.Jar | BindingBuilder.Steps.ClassParse | BindingBuilder.Steps.ApiXmlAdjuster;
			builder.Clean ();
			builder.Build ();

			var apixml = Path.Combine (builder.IntermediateOutputPathAbsolute, "api.xml");
			Assert.AreEqual (apixml, project.GeneratedApiXmlFile, "api.xml file path mismatch.");
			Assert.IsTrue (File.Exists (project.GeneratedApiXmlFile), "api.xml file not found");
		}
	}
}
