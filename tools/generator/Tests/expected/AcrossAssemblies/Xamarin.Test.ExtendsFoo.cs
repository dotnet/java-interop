using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Xamarin.Test {

	// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test']/class[@name='ExtendsFoo']"
	[global::Android.Runtime.Register ("xamarin/test/ExtendsFoo", DoNotGenerateAcw=true)]
	public partial class ExtendsFoo : global::Java.Lang.Foo {

		internal static new IntPtr java_class_handle;
		internal static new IntPtr class_ref {
			get {
				return JNIEnv.FindClass ("xamarin/test/ExtendsFoo", ref java_class_handle);
			}
		}

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return typeof (ExtendsFoo); }
		}

		protected ExtendsFoo (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		static Delegate cb_foo;
#pragma warning disable 0169
		static Delegate GetFooHandler ()
		{
			if (cb_foo == null)
				cb_foo = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_Foo);
			return cb_foo;
		}

		static void n_Foo (IntPtr jnienv, IntPtr native__this)
		{
			global::Xamarin.Test.ExtendsFoo __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.ExtendsFoo> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.Foo ();
		}
#pragma warning restore 0169

		static IntPtr id_foo;
		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='ExtendsFoo']/method[@name='foo' and count(parameter)=0]"
		[Register ("foo", "()V", "GetFooHandler")]
		public virtual unsafe void Foo ()
		{
			if (id_foo == IntPtr.Zero)
				id_foo = JNIEnv.GetMethodID (class_ref, "foo", "()V");
			try {

				if (((object) this).GetType () == ThresholdType)
					JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_foo);
				else
					JNIEnv.CallNonvirtualVoidMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "foo", "()V"));
			} finally {
			}
		}

	}
}
