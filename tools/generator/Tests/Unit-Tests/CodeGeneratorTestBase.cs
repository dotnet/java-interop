using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MonoDroid.Generation;
using NUnit.Framework;

namespace generatortests
{
	abstract class CodeGeneratorTestBase
	{
		protected CodeGenerator generator;
		protected StringBuilder builder;
		protected StringWriter writer;
		protected CodeGenerationOptions options;

		[SetUp]
		public void SetUp ()
		{
			builder = new StringBuilder ();
			writer = new StringWriter (builder);
			options = CreateOptions ();

			generator = options.CreateCodeGenerator (writer);
		}

		[TearDown]
		public void TearDown ()
		{
			writer.Dispose ();
		}

		protected virtual CodeGenerationOptions CreateOptions ()
		{
			return new CodeGenerationOptions {
				CodeGenerationTarget = Target,
			};
		}

		protected abstract Xamarin.Android.Binder.CodeGenerationTarget Target { get; }

		// Get the test results from "Common" for tests with the same results regardless of Target
		protected string GetExpected (string testName)
		{
			var root = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);

			return File.ReadAllText (Path.Combine (root, "Unit-Tests", "CodeGeneratorExpectedResults", "Common", $"{testName}.txt")).NormalizeLineEndings ();
		}

		// Get the test results from "JavaInterop1" or "XamarinAndroid" for tests with the different results per Target
		protected string GetTargetedExpected (string testName)
		{
			var target = Target.ToString ();
			var root = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);

			return File.ReadAllText (Path.Combine (root, "Unit-Tests", "CodeGeneratorExpectedResults", target, $"{testName}.txt")).NormalizeLineEndings ();
		}

		protected List<GenBase> ParseApiDefinition (string xml)
		{
			var doc = XDocument.Parse (xml);
			var gens = new Parser (options).Parse (doc, Enumerable.Empty<string> (), "29", 7);

			foreach (var gen in gens)
				options.SymbolTable.AddType (gen);

			foreach (var gen in gens)
				gen.Validate (options, new GenericParameterDefinitionList (), generator.Context);

			foreach (var gen in gens)
				gen.FillProperties ();

			foreach (var gen in gens)
				gen.FixupMethodOverrides (options);

			return gens;
		}
	}
}
