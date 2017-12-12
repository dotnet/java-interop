using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Xamarin.Test {

	// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test']/class[@name='MyInterfaceImplementation']"
	[global::Android.Runtime.Register ("xamarin/test/MyInterfaceImplementation", DoNotGenerateAcw=true)]
	public abstract partial class MyInterfaceImplementation : global::Java.Lang.Object, global::Java.Lang.IMyInterface {


		public static class InterfaceConsts {

			// The following are fields from: java.lang.MyInterface

			// Metadata.xml XPath field reference: path="/api/package[@name='java.lang']/interface[@name='MyInterface']/field[@name='MY_FIELD']"
			[Register ("MY_FIELD")]
			public const int MyField = (int) 256;
		}

		internal static new IntPtr java_class_handle;
		internal static new IntPtr class_ref {
			get {
				return JNIEnv.FindClass ("xamarin/test/MyInterfaceImplementation", ref java_class_handle);
			}
		}

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return typeof (MyInterfaceImplementation); }
		}

		protected MyInterfaceImplementation (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		static IntPtr id_ctor;
		// Metadata.xml XPath constructor reference: path="/api/package[@name='xamarin.test']/class[@name='MyInterfaceImplementation']/constructor[@name='MyInterfaceImplementation' and count(parameter)=0]"
		[Register (".ctor", "()V", "")]
		public unsafe MyInterfaceImplementation ()
			: base (IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
		{
			if (((global::Java.Lang.Object) this).Handle != IntPtr.Zero)
				return;

			try {
				if (((object) this).GetType () != typeof (MyInterfaceImplementation)) {
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

		static Delegate cb_myMethod;
#pragma warning disable 0169
		static Delegate GetMyMethodHandler ()
		{
			if (cb_myMethod == null)
				cb_myMethod = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_MyMethod);
			return cb_myMethod;
		}

		static void n_MyMethod (IntPtr jnienv, IntPtr native__this)
		{
			global::Xamarin.Test.MyInterfaceImplementation __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.MyInterfaceImplementation> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.MyMethod ();
		}
#pragma warning restore 0169

		static IntPtr id_myMethod;
		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='MyInterfaceImplementation']/method[@name='myMethod' and count(parameter)=0]"
		[Register ("myMethod", "()V", "GetMyMethodHandler")]
		public virtual unsafe void MyMethod ()
		{
			if (id_myMethod == IntPtr.Zero)
				id_myMethod = JNIEnv.GetMethodID (class_ref, "myMethod", "()V");
			try {

				if (((object) this).GetType () == ThresholdType)
					JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_myMethod);
				else
					JNIEnv.CallNonvirtualVoidMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "myMethod", "()V"));
			} finally {
			}
		}

	}

	[global::Android.Runtime.Register ("xamarin/test/MyInterfaceImplementation", DoNotGenerateAcw=true)]
	internal partial class MyInterfaceImplementationInvoker : MyInterfaceImplementation {

		public MyInterfaceImplementationInvoker (IntPtr handle, JniHandleOwnership transfer) : base (handle, transfer) {}

		protected override global::System.Type ThresholdType {
			get { return typeof (MyInterfaceImplementationInvoker); }
		}

	}

}
