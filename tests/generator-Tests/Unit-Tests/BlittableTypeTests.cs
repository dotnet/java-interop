using System;
using MonoDroid.Generation;
using NUnit.Framework;
using Xamarin.Android.Binder;

namespace generatortests;

[TestFixture]
class BlittableTypeTests : CodeGeneratorTestBase
{
	protected override CodeGenerationTarget Target => CodeGenerationTarget.XAJavaInterop1;

	[Test]
	public void MethodWithBoolReturnType ()
	{
		var klass = new TestClass ("Object", "java.code.MyClass");
		var method = SupportTypeBuilder.CreateMethod (klass, "IsEmpty", options, "boolean");

		klass.Methods.Add (method);

		var actual = GetGeneratedTypeOutput (klass);

		// Return type should be byte
		Assert.That (actual, Contains.Substring ("static byte n_IsEmpty"));

		// Return statement should convert to 0 or 1
		Assert.That (actual, Contains.Substring ("return __this.IsEmpty () ? 1 : 0"));

		// Ensure the marshal delegate is byte
		Assert.That (actual, Contains.Substring ("new _JniMarshal_PP_B"));
		Assert.That (actual, Does.Not.Contains ("new _JniMarshal_PP_Z"));
	}

	[Test]
	public void MethodWithBoolParameter ()
	{
		var klass = new TestClass ("Object", "java.code.MyClass");
		var method = SupportTypeBuilder.CreateMethod (klass, "SetEmpty", options, "void", parameters: new Parameter ("value", "boolean", "bool", false));

		klass.Methods.Add (method);

		var actual = GetGeneratedTypeOutput (klass);

		// Method parameter should be byte
		Assert.That (actual, Contains.Substring ("static void n_SetEmpty_Z (IntPtr jnienv, IntPtr native__this, byte native_value)"));

		// Method should convert from 0 or 1
		Assert.That (actual, Contains.Substring ("var value = native_value != 0;"));

		// Ensure the marshal delegate is byte
		Assert.That (actual, Contains.Substring ("new _JniMarshal_PPB_V"));
		Assert.That (actual, Does.Not.Contains ("new _JniMarshal_PPZ_V"));
	}

	[Test]
	public void BoolProperty ()
	{
		var klass = SupportTypeBuilder.CreateClassWithProperty ("MyClass", "com.example.myClass", "IsEmpty", "boolean", options);
		var actual = GetGeneratedTypeOutput (klass);

		// Getter return type should be byte
		Assert.That (actual, Contains.Substring ("static byte n_get_IsEmpty"));

		// Getter return statement should convert to 0 or 1
		Assert.That (actual, Contains.Substring ("return __this.IsEmpty ? 1 : 0"));

		// Setter parameter should be byte
		Assert.That (actual, Contains.Substring ("static void n_set_IsEmpty_Z (IntPtr jnienv, IntPtr native__this, byte native_value)"));

		// Setter should convert from 0 or 1
		Assert.That (actual, Contains.Substring ("var value = native_value != 0;"));

		// Ensure the marshal delegate is byte
		Assert.That (actual, Contains.Substring ("new _JniMarshal_PP_B"));
		Assert.That (actual, Does.Not.Contains ("new _JniMarshal_PP_Z"));
	}
}
