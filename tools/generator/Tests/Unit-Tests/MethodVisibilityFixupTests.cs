using System;
using MonoDroid.Generation;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	public class MethodVisibilityFixupTests
	{
		CodeGenerationOptions opt;
		GenericParameterDefinitionList list;
		CodeGeneratorContext context;

		[SetUp]
		public void SetUp ()
		{
			opt = new CodeGenerationOptions ();
			list = new GenericParameterDefinitionList ();
			context = new CodeGeneratorContext ();
		}

		[Test]
		public void FixupBaseOverride ()
		{
			var base_class = new TestClass ("java.lang.Object", "com.mypackage.Foo");
			var this_class = new TestClass ("com.mypackage.Foo", "com.mypackage.MyClass");

			base_class.Methods.Add (SupportTypeBuilder.CreateMethod (base_class, "DoSomething", opt));
			base_class.Methods [0].Visibility = "protected";
			opt.SymbolTable.AddType (base_class);

			this_class.Methods.Add (SupportTypeBuilder.CreateMethod (this_class, "DoSomething", opt));
			this_class.Methods [0].Visibility = "public";
			this_class.Methods [0].IsOverride = true;
			opt.SymbolTable.AddType (this_class);

			base_class.Validate (opt, list, context);
			this_class.Validate (opt, list, context);

			MethodVisibilityFixup.Fixup (this_class);

			Assert.AreEqual ("protected", this_class.Methods [0].Visibility);
		}

		[Test]
		public void FixupRecursiveBaseOverride ()
		{
			// This throws a middle class into the inheritance tree that does
			// not override the method to ensure we look recursively
			var base_class = new TestClass ("java.lang.Object", "com.mypackage.Foo");
			var middle_class = new TestClass ("com.mypackage.Foo", "com.mypackage.Middle");
			var this_class = new TestClass ("com.mypackage.Middle", "com.mypackage.MyClass");

			base_class.Methods.Add (SupportTypeBuilder.CreateMethod (base_class, "DoSomething", opt));
			base_class.Methods [0].Visibility = "protected";
			opt.SymbolTable.AddType (base_class);

			opt.SymbolTable.AddType (middle_class);

			this_class.Methods.Add (SupportTypeBuilder.CreateMethod (this_class, "DoSomething", opt));
			this_class.Methods [0].Visibility = "public";
			this_class.Methods [0].IsOverride = true;
			opt.SymbolTable.AddType (this_class);

			base_class.Validate (opt, list, context);
			this_class.Validate (opt, list, context);

			MethodVisibilityFixup.Fixup (this_class);

			Assert.AreEqual ("protected", this_class.Methods [0].Visibility);
		}

		[Test]
		public void FixupNestedBaseOverride ()
		{
			// This tests:
			// - MyClass: public override void DoSomething ()
			//   |- MiddleClass: public override void DoSomething ()
			//      |- BaseClass: protected virtual void DoSomething ()
			var base_class = new TestClass ("java.lang.Object", "com.mypackage.Foo");
			var middle_class = new TestClass ("com.mypackage.Foo", "com.mypackage.Middle");
			var this_class = new TestClass ("com.mypackage.Middle", "com.mypackage.MyClass");

			base_class.Methods.Add (SupportTypeBuilder.CreateMethod (base_class, "DoSomething", opt));
			base_class.Methods [0].Visibility = "protected";
			opt.SymbolTable.AddType (base_class);

			middle_class.Methods.Add (SupportTypeBuilder.CreateMethod (middle_class, "DoSomething", opt));
			middle_class.Methods [0].Visibility = "public";
			middle_class.Methods [0].IsOverride = true;
			opt.SymbolTable.AddType (middle_class);

			this_class.Methods.Add (SupportTypeBuilder.CreateMethod (this_class, "DoSomething", opt));
			this_class.Methods [0].Visibility = "public";
			this_class.Methods [0].IsOverride = true;
			opt.SymbolTable.AddType (this_class);

			base_class.Validate (opt, list, context);
			middle_class.Validate (opt, list, context);
			this_class.Validate (opt, list, context);

			MethodVisibilityFixup.Fixup (this_class);

			Assert.AreEqual ("protected", this_class.Methods [0].Visibility);
		}

		[Test]
		public void FixupBaseProtectedInternalOverride ()
		{
			var base_class = new TestClass ("java.lang.Object", "com.mypackage.Foo");
			var this_class = new TestClass ("com.mypackage.Foo", "com.mypackage.MyClass");

			base_class.Methods.Add (SupportTypeBuilder.CreateMethod (base_class, "DoSomething", opt));
			base_class.Methods [0].Visibility = "protected internal";
			opt.SymbolTable.AddType (base_class);

			this_class.Methods.Add (SupportTypeBuilder.CreateMethod (this_class, "DoSomething", opt));
			this_class.Methods [0].Visibility = "public";
			this_class.Methods [0].IsOverride = true;
			opt.SymbolTable.AddType (this_class);

			base_class.Validate (opt, list, context);
			this_class.Validate (opt, list, context);

			MethodVisibilityFixup.Fixup (this_class);

			Assert.AreEqual ("protected internal", this_class.Methods [0].Visibility);
		}

		[Test]
		public void IgnoreValidPublic ()
		{
			var base_class = new TestClass ("java.lang.Object", "com.mypackage.Foo");
			var this_class = new TestClass ("com.mypackage.Foo", "com.mypackage.MyClass");

			base_class.Methods.Add (SupportTypeBuilder.CreateMethod (base_class, "DoSomething", opt));
			base_class.Methods [0].Visibility = "public";
			opt.SymbolTable.AddType (base_class);

			this_class.Methods.Add (SupportTypeBuilder.CreateMethod (this_class, "DoSomething", opt));
			this_class.Methods [0].Visibility = "public";
			this_class.Methods [0].IsOverride = true;
			opt.SymbolTable.AddType (this_class);

			base_class.Validate (opt, list, context);
			this_class.Validate (opt, list, context);

			MethodVisibilityFixup.Fixup (this_class);

			Assert.AreEqual ("public", this_class.Methods [0].Visibility);
		}

		[Test]
		public void FixupPropertyBaseOverride ()
		{
			var base_class = new TestClass ("java.lang.Object", "com.mypackage.Foo");
			var this_class = new TestClass ("com.mypackage.Foo", "com.mypackage.MyClass");

			base_class.Properties.Add (SupportTypeBuilder.CreateProperty (base_class, "Count", "int", opt));
			base_class.Properties [0].Getter.Visibility = "protected";
			opt.SymbolTable.AddType (base_class);

			this_class.Properties.Add (SupportTypeBuilder.CreateProperty (this_class, "Count", "int", opt));
			this_class.Properties [0].Getter.Visibility = "public";
			this_class.Properties [0].Getter.IsOverride = true;
			opt.SymbolTable.AddType (this_class);

			base_class.Validate (opt, list, context);
			this_class.Validate (opt, list, context);

			MethodVisibilityFixup.Fixup (this_class);

			Assert.AreEqual ("protected", this_class.Properties [0].Getter.Visibility);
		}
	}
}
