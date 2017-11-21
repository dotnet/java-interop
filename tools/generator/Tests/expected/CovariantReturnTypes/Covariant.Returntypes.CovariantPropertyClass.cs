using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Covariant.Returntypes {

	// Metadata.xml XPath class reference: path="/api/package[@name='covariant.returntypes']/class[@name='CovariantPropertyClass']"
	[global::Android.Runtime.Register ("covariant/returntypes/CovariantPropertyClass", DoNotGenerateAcw=true)]
	public partial class CovariantPropertyClass : global::Java.Lang.Object {

		internal static new IntPtr java_class_handle;
		internal static new IntPtr class_ref {
			get {
				return JNIEnv.FindClass ("covariant/returntypes/CovariantPropertyClass", ref java_class_handle);
			}
		}

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return typeof (CovariantPropertyClass); }
		}

		protected CovariantPropertyClass (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		static Delegate cb_getObject;
#pragma warning disable 0169
		static Delegate GetGetObjectHandler ()
		{
			if (cb_getObject == null)
				cb_getObject = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, IntPtr>) n_GetObject);
			return cb_getObject;
		}

		static IntPtr n_GetObject (IntPtr jnienv, IntPtr native__this)
		{
			global::Covariant.Returntypes.CovariantPropertyClass __this = global::Java.Lang.Object.GetObject<global::Covariant.Returntypes.CovariantPropertyClass> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return JNIEnv.ToLocalJniHandle (__this.Object);
		}
#pragma warning restore 0169

		static IntPtr id_getObject;
		public virtual unsafe global::Java.Lang.Object Object {
			// Metadata.xml XPath method reference: path="/api/package[@name='covariant.returntypes']/class[@name='CovariantPropertyClass']/method[@name='getObject' and count(parameter)=0]"
			[Register ("getObject", "()Ljava/lang/Object;", "GetGetObjectHandler")]
			get {
				if (id_getObject == IntPtr.Zero)
					id_getObject = JNIEnv.GetMethodID (class_ref, "getObject", "()Ljava/lang/Object;");
				try {

					if (((object) this).GetType () == ThresholdType)
						return global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (JNIEnv.CallObjectMethod (((global::Java.Lang.Object) this).Handle, id_getObject), JniHandleOwnership.TransferLocalRef);
					else
						return global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (JNIEnv.CallNonvirtualObjectMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "getObject", "()Ljava/lang/Object;")), JniHandleOwnership.TransferLocalRef);
				} finally {
				}
			}
		}

	}
}
