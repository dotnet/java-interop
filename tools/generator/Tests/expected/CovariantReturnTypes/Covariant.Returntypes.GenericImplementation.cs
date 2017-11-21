using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Covariant.Returntypes {

	// Metadata.xml XPath class reference: path="/api/package[@name='covariant.returntypes']/class[@name='GenericImplementation']"
	[global::Android.Runtime.Register ("covariant/returntypes/GenericImplementation", DoNotGenerateAcw=true)]
	public partial class GenericImplementation : global::Java.Lang.Object, global::Covariant.Returntypes.IGenericInterface {

		internal static new IntPtr java_class_handle;
		internal static new IntPtr class_ref {
			get {
				return JNIEnv.FindClass ("covariant/returntypes/GenericImplementation", ref java_class_handle);
			}
		}

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return typeof (GenericImplementation); }
		}

		protected GenericImplementation (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		static IntPtr id_ctor;
		// Metadata.xml XPath constructor reference: path="/api/package[@name='covariant.returntypes']/class[@name='GenericImplementation']/constructor[@name='GenericImplementation' and count(parameter)=0]"
		[Register (".ctor", "()V", "")]
		public unsafe GenericImplementation ()
			: base (IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
		{
			if (((global::Java.Lang.Object) this).Handle != IntPtr.Zero)
				return;

			try {
				if (((object) this).GetType () != typeof (GenericImplementation)) {
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
			global::Covariant.Returntypes.GenericImplementation __this = global::Java.Lang.Object.GetObject<global::Covariant.Returntypes.GenericImplementation> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return JNIEnv.ToLocalJniHandle (__this.SomeMethod ());
		}
#pragma warning restore 0169

		static IntPtr id_someMethod;
		// Metadata.xml XPath method reference: path="/api/package[@name='covariant.returntypes']/class[@name='GenericImplementation']/method[@name='someMethod' and count(parameter)=0]"
		[Register ("someMethod", "()Ljava/lang/Object;", "GetSomeMethodHandler")]
		public virtual unsafe global::Java.Lang.Object SomeMethod ()
		{
			if (id_someMethod == IntPtr.Zero)
				id_someMethod = JNIEnv.GetMethodID (class_ref, "someMethod", "()Ljava/lang/Object;");
			try {

				if (((object) this).GetType () == ThresholdType)
					return global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (JNIEnv.CallObjectMethod (((global::Java.Lang.Object) this).Handle, id_someMethod), JniHandleOwnership.TransferLocalRef);
				else
					return global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (JNIEnv.CallNonvirtualObjectMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "someMethod", "()Ljava/lang/Object;")), JniHandleOwnership.TransferLocalRef);
			} finally {
			}
		}

		// This method is explicitly implemented as a member of an instantiated Covariant.Returntypes.IGenericInterface
		global::Java.Lang.Object global::Covariant.Returntypes.IGenericInterface.SomeMethod ()
		{
			return global::Java.Interop.JavaObjectExtensions.JavaCast<Java.Lang.Object>(SomeMethod ());
		}

	}
}
