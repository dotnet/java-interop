using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Xamarin.Test {

	// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test']/class[@name='CSharpKeywords']"
	[global::Android.Runtime.Register ("xamarin/test/CSharpKeywords", DoNotGenerateAcw=true)]
	public partial class CSharpKeywords : global::Java.Lang.Object {

		internal static new IntPtr java_class_handle;
		internal static new IntPtr class_ref {
			get {
				return JNIEnv.FindClass ("xamarin/test/CSharpKeywords", ref java_class_handle);
			}
		}

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return typeof (CSharpKeywords); }
		}

		protected CSharpKeywords (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		static Delegate cb_usePartial_I;
#pragma warning disable 0169
		static Delegate GetUsePartial_IHandler ()
		{
			if (cb_usePartial_I == null)
				cb_usePartial_I = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, int, IntPtr>) n_UsePartial_I);
			return cb_usePartial_I;
		}

		static IntPtr n_UsePartial_I (IntPtr jnienv, IntPtr native__this, int partial)
		{
			global::Xamarin.Test.CSharpKeywords __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.CSharpKeywords> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return JNIEnv.NewString (__this.UsePartial (partial));
		}
#pragma warning restore 0169

		static IntPtr id_usePartial_I;
		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='CSharpKeywords']/method[@name='usePartial' and count(parameter)=1 and parameter[1][@type='int']]"
		[Register ("usePartial", "(I)Ljava/lang/String;", "GetUsePartial_IHandler")]
		public virtual unsafe string UsePartial (int partial)
		{
			if (id_usePartial_I == IntPtr.Zero)
				id_usePartial_I = JNIEnv.GetMethodID (class_ref, "usePartial", "(I)Ljava/lang/String;");
			try {
				JValue* __args = stackalloc JValue [1];
				__args [0] = new JValue (partial);

				if (((object) this).GetType () == ThresholdType)
					return JNIEnv.GetString (JNIEnv.CallObjectMethod (((global::Java.Lang.Object) this).Handle, id_usePartial_I, __args), JniHandleOwnership.TransferLocalRef);
				else
					return JNIEnv.GetString (JNIEnv.CallNonvirtualObjectMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "usePartial", "(I)Ljava/lang/String;"), __args), JniHandleOwnership.TransferLocalRef);
			} finally {
			}
		}

	}
}
