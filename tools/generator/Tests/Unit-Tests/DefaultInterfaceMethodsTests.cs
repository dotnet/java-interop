using System;
using MonoDroid.Generation;
using NUnit.Framework;
using Xamarin.Android.Binder;

namespace generatortests
{
	[TestFixture]
	class JavaInteropDefaultInterfaceMethodsTests : DefaultInterfaceMethodsTests
	{
		protected override CodeGenerationTarget Target => CodeGenerationTarget.JavaInterop1;
	}

	[TestFixture]
	class XAJavaInteropDefaultInterfaceMethodsTests : DefaultInterfaceMethodsTests
	{
		protected override CodeGenerationTarget Target => CodeGenerationTarget.XAJavaInterop1;
	}

	[TestFixture]
	class XamarinAndroidDefaultInterfaceMethodsTests : DefaultInterfaceMethodsTests
	{
		protected override CodeGenerationTarget Target => CodeGenerationTarget.XamarinAndroid;
	}

	abstract class DefaultInterfaceMethodsTests : CodeGeneratorTestBase
	{
		protected override CodeGenerationOptions CreateOptions ()
		{
			var options = base.CreateOptions ();

			options.SupportDefaultInterfaceMethods = true;

			return options;
		}

		[Test]
		public void WriteInterfaceDefaultMethod ()
		{
			// Create an interface with a default method
			var iface = SupportTypeBuilder.CreateEmptyInterface("java.code.IMyInterface");

			iface.Methods.Add (new TestMethod (iface, "DoSomething").SetDefaultInterfaceMethod ());

			iface.Validate (options, new GenericParameterDefinitionList (), new CodeGeneratorContext ());

			generator.WriteInterfaceDeclaration (iface, string.Empty);

			Assert.AreEqual (GetTargetedExpected (nameof (WriteInterfaceDefaultMethod)), writer.ToString ().NormalizeLineEndings ());
		}

		[Test]
		public void WriteInterfaceRedeclaredDefaultMethod ()
		{
			// Create an interface with a default method
			var iface = SupportTypeBuilder.CreateEmptyInterface ("java.code.IMyInterface");
			iface.Methods.Add (new TestMethod (iface, "DoSomething").SetDefaultInterfaceMethod ());
			options.SymbolTable.AddType (iface);

			// Create a second interface that inherits the first, declaring the method as not default
			var iface2 = SupportTypeBuilder.CreateEmptyInterface ("java.code.IMyInterface2");
			iface2.AddImplementedInterface ("java.code.IMyInterface");
			iface2.Methods.Add (new TestMethod (iface, "DoSomething"));

			iface.Validate (options, new GenericParameterDefinitionList (), new CodeGeneratorContext ());
			iface2.Validate (options, new GenericParameterDefinitionList (), new CodeGeneratorContext ());

			generator.WriteInterfaceDeclaration (iface2, string.Empty);

			// IMyInterface2 should generate the method as abstract, not a default method
			Assert.AreEqual (GetExpected (nameof (WriteInterfaceRedeclaredDefaultMethod)), writer.ToString ().NormalizeLineEndings ());
		}

		[Test]
		public void WriteInterfaceDefaultProperty ()
		{
			// Create an interface with a default method
			var iface = SupportTypeBuilder.CreateEmptyInterface ("java.code.IMyInterface");
			var prop = SupportTypeBuilder.CreateProperty (iface, "Value", "int", options);

			prop.Getter.IsInterfaceDefaultMethod = true;
			prop.Setter.IsInterfaceDefaultMethod = true;

			iface.Properties.Add (prop);

			iface.Validate (options, new GenericParameterDefinitionList (), new CodeGeneratorContext ());

			generator.WriteInterfaceDeclaration (iface, string.Empty);

			Assert.AreEqual (GetTargetedExpected (nameof (WriteInterfaceDefaultProperty)), writer.ToString ().NormalizeLineEndings ());
		}

		[Test]
		public void WriteInterfaceDefaultPropertyGetterOnly ()
		{
			// Create an interface with a default method
			var iface = SupportTypeBuilder.CreateEmptyInterface ("java.code.IMyInterface");
			var prop = SupportTypeBuilder.CreateProperty (iface, "Value", "int", options);

			prop.Getter.IsInterfaceDefaultMethod = true;
			prop.Setter = null;

			iface.Properties.Add (prop);

			iface.Validate (options, new GenericParameterDefinitionList (), new CodeGeneratorContext ());

			generator.WriteInterfaceDeclaration (iface, string.Empty);

			Assert.AreEqual (GetTargetedExpected (nameof (WriteInterfaceDefaultPropertyGetterOnly)), writer.ToString ().NormalizeLineEndings ());
		}

		[Test]
		public void WriteSealedOverriddenDefaultMethod ()
		{
			// Create an interface with a default method
			var iface = SupportTypeBuilder.CreateEmptyInterface ("java.code.IMyInterface");
			iface.Methods.Add (new TestMethod (iface, "DoSomething").SetDefaultInterfaceMethod ());
			options.SymbolTable.AddType (iface);

			// Create a type that inherits the interface, overriding the method as final
			var klass = new TestClass ("java.code.IMyInterface", "java.code.MyClass");
			klass.AddImplementedInterface ("java.code.IMyInterface");
			klass.Methods.Add (new TestMethod (iface, "DoSomething").SetFinal ());

			iface.Validate (options, new GenericParameterDefinitionList (), generator.Context);
			klass.Validate (options, new GenericParameterDefinitionList (), generator.Context);

			klass.FixupMethodOverrides (options);

			generator.Context.ContextTypes.Push (klass);
			generator.WriteClass (klass, string.Empty, new GenerationInfo (string.Empty, string.Empty, "MyAssembly"));
			generator.Context.ContextTypes.Pop ();

			// The method should not be marked as 'virtual sealed'
			Assert.False (writer.ToString ().Contains ("virtual sealed"));
		}
	}
}
