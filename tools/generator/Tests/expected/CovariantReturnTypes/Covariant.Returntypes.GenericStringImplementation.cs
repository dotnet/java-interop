using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Covariant.Returntypes {

	// Metadata.xml XPath class reference: path="/api/package[@name='covariant.returntypes']/class[@name='GenericStringImplementation']"
	[global::Android.Runtime.Register ("covariant/returntypes/GenericStringImplementation", DoNotGenerateAcw=true)]
	public partial class GenericStringImplementation : global::Java.Lang.Object, global::Covariant.Returntypes.IGenericInterface {

		internal static new IntPtr java_class_handle;
		internal static new IntPtr class_ref {
			get {
				return JNIEnv.FindClass ("covariant/returntypes/GenericStringImplementation", ref java_class_handle);
			}
		}

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return typeof (GenericStringImplementation); }
		}

		protected GenericStringImplementation (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		static IntPtr id_ctor;
		// Metadata.xml XPath constructor reference: path="/api/package[@name='covariant.returntypes']/class[@name='GenericStringImplementation']/constructor[@name='GenericStringImplementation' and count(parameter)=0]"
		[Register (".ctor", "()V", "")]
		public unsafe GenericStringImplementation ()
			: base (IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
		{
			if (((global::Java.Lang.Object) this).Handle != IntPtr.Zero)
				return;

			try {
				if (((object) this).GetType () != typeof (GenericStringImplementation)) {
					SetHandle (
							global::Android.Runtime.JNIEnv.StartCreateInstance (((object) this).GetType (), "()V"),
							JniHandleOwnership.TransferLocalRef);
					global::Android.Runtime.JNIEnv.FinishCreateInstance (((global::Java.Lang.Object) this).Handle, "()V");
					return;
				}

				if (id_ctor == IntPtr.Zero)
					id_ctor = JNIEnv.GetMethodID (class_ref, "<init>", "()V");
				SetHandle (
						global::Android.Runtime.JNIEnv.StartCreateInstance (class_ref, id_ctor),
						JniHandleOwnership.TransferLocalRef);
				JNIEnv.FinishCreateInstance (((global::Java.Lang.Object) this).Handle, class_ref, id_ctor);
			} finally {
			}
		}

		static Delegate cb_someMethod;
#pragma warning disable 0169
		static Delegate GetSomeMethodHandler ()
		{
			if (cb_someMethod == null)
				cb_someMethod = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, IntPtr>) n_SomeMethod);
			return cb_someMethod;
		}

		static IntPtr n_SomeMethod (IntPtr jnienv, IntPtr native__this)
		{
			global::Covariant.Returntypes.GenericStringImplementation __this = global::Java.Lang.Object.GetObject<global::Covariant.Returntypes.GenericStringImplementation> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return JNIEnv.NewString (__this.SomeMethod ());
		}
#pragma warning restore 0169

		static IntPtr id_someMethod;
		// Metadata.xml XPath method reference: path="/api/package[@name='covariant.returntypes']/class[@name='GenericStringImplementation']/method[@name='someMethod' and count(parameter)=0]"
		[Register ("someMethod", "()Ljava/lang/String;", "GetSomeMethodHandler")]
		public virtual unsafe string SomeMethod ()
		{
			if (id_someMethod == IntPtr.Zero)
				id_someMethod = JNIEnv.GetMethodID (class_ref, "someMethod", "()Ljava/lang/String;");
			try {

				if (((object) this).GetType () == ThresholdType)
					return JNIEnv.GetString (JNIEnv.CallObjectMethod (((global::Java.Lang.Object) this).Handle, id_someMethod), JniHandleOwnership.TransferLocalRef);
				else
					return JNIEnv.GetString (JNIEnv.CallNonvirtualObjectMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "someMethod", "()Ljava/lang/String;")), JniHandleOwnership.TransferLocalRef);
			} finally {
			}
		}

		// This method is explicitly implemented as a member of an instantiated Covariant.Returntypes.IGenericInterface
		global::Java.Lang.Object global::Covariant.Returntypes.IGenericInterface.SomeMethod ()
		{
			return SomeMethod ().ToString ();
		}

	}
}
