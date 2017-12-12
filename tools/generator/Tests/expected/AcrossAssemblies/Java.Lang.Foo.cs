using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Java.Lang {

	// Metadata.xml XPath class reference: path="/api/package[@name='java.lang']/class[@name='Foo']"
	[global::Android.Runtime.Register ("java/lang/Foo", DoNotGenerateAcw=true)]
	public partial class Foo : global::Java.Lang.Object {

		internal static new IntPtr java_class_handle;
		internal static new IntPtr class_ref {
			get {
				return JNIEnv.FindClass ("java/lang/Foo", ref java_class_handle);
			}
		}

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return typeof (Foo); }
		}

		protected Foo (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		static Delegate cb_bar;
#pragma warning disable 0169
		static Delegate GetBarHandler ()
		{
			if (cb_bar == null)
				cb_bar = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_Bar);
			return cb_bar;
		}

		static void n_Bar (IntPtr jnienv, IntPtr native__this)
		{
			global::Java.Lang.Foo __this = global::Java.Lang.Object.GetObject<global::Java.Lang.Foo> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.Bar ();
		}
#pragma warning restore 0169

		static IntPtr id_bar;
		// Metadata.xml XPath method reference: path="/api/package[@name='java.lang']/class[@name='Foo']/method[@name='bar' and count(parameter)=0]"
		[Register ("bar", "()V", "GetBarHandler")]
		public virtual unsafe void Bar ()
		{
			if (id_bar == IntPtr.Zero)
				id_bar = JNIEnv.GetMethodID (class_ref, "bar", "()V");
			try {

				if (((object) this).GetType () == ThresholdType)
					JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_bar);
				else
					JNIEnv.CallNonvirtualVoidMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "bar", "()V"));
			} finally {
			}
		}

	}
}
