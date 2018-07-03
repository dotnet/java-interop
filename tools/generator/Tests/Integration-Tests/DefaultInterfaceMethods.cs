using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.Android.Binder;

namespace generatortests
{
	[TestFixture]
	public class DefaultInterfaceMethods : BaseGeneratorTest
	{
		[Test]
		public void GeneratedOK ()
		{
			Options.SupportDefaultInterfaceMethods = true;
			RunAllTargets (
					outputRelativePath: "DefaultInterfaceMethods",
					apiDescriptionFile: "expected.ji/DefaultInterfaceMethods/DefaultInterfaceMethods.xml",
					expectedRelativePath: "DefaultInterfaceMethods",
					additionalSupportPaths: null);
		}

		void RunAllTargets (string outputRelativePath, string apiDescriptionFile, string expectedRelativePath, string [] additionalSupportPaths)
		{
			Run (CodeGenerationTarget.JavaInterop1, Path.Combine ("out.ji", outputRelativePath), apiDescriptionFile, Path.Combine ("expected.ji", expectedRelativePath), additionalSupportPaths);
			var type = BuiltAssembly.GetTypes ().First (t => t.FullName == "Xamarin.Test.TheImplementor");
			var instance = Activator.CreateInstance (type);
			type.GetProperty ("Bar").GetValue (instance);
		}
	}
}

