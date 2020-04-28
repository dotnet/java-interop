using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Java.IO {

	// Metadata.xml XPath class reference: path="/api/package[@name='java.io']/class[@name='IOException']"
	[global::Android.Runtime.Register ("java/io/IOException", DoNotGenerateAcw=true)]
	public abstract partial class IOException : global::Java.Lang.Throwable {

		internal static new IntPtr java_class_handle;
		internal static new IntPtr class_ref {
			get {
				return JNIEnv.FindClass ("java/io/IOException", ref java_class_handle);
			}
		}

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return typeof (IOException); }
		}

		protected IOException (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		static Delegate cb_printStackTrace;
#pragma warning disable 0169
		static Delegate GetPrintStackTraceHandler ()
		{
			if (cb_printStackTrace == null)
				cb_printStackTrace = JNINativeWrapper.CreateDelegate ((_JniMarshal_PP_V) n_PrintStackTrace);
			return cb_printStackTrace;
		}

		static void n_PrintStackTrace (IntPtr jnienv, IntPtr native__this)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Java.IO.IOException> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.PrintStackTrace ();
		}
#pragma warning restore 0169

		static IntPtr id_printStackTrace;
		// Metadata.xml XPath method reference: path="/api/package[@name='java.io']/class[@name='IOException']/method[@name='printStackTrace' and count(parameter)=0]"
		[Register ("printStackTrace", "()V", "GetPrintStackTraceHandler")]
		public virtual unsafe void PrintStackTrace ()
		{
			if (id_printStackTrace == IntPtr.Zero)
				id_printStackTrace = JNIEnv.GetMethodID (class_ref, "printStackTrace", "()V");
			try {

				if (((object) this).GetType () == ThresholdType)
					JNIEnv.CallVoidMethod (((global::Java.Lang.Throwable) this).Handle, id_printStackTrace);
				else
					JNIEnv.CallNonvirtualVoidMethod (((global::Java.Lang.Throwable) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "printStackTrace", "()V"));
			} finally {
			}
		}

	}

	[global::Android.Runtime.Register ("java/io/IOException", DoNotGenerateAcw=true)]
	internal partial class IOExceptionInvoker : IOException {

		public IOExceptionInvoker (IntPtr handle, JniHandleOwnership transfer) : base (handle, transfer) {}

		protected override global::System.Type ThresholdType {
			get { return typeof (IOExceptionInvoker); }
		}

	}

}
