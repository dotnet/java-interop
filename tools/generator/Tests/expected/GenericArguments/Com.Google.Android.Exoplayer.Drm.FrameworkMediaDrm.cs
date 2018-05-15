using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Com.Google.Android.Exoplayer.Drm {

	// Metadata.xml XPath class reference: path="/api/package[@name='com.google.android.exoplayer.drm']/class[@name='FrameworkMediaDrm']"
	[global::Android.Runtime.Register ("com/google/android/exoplayer/drm/FrameworkMediaDrm", DoNotGenerateAcw=true)]
	public sealed partial class FrameworkMediaDrm : global::Java.Lang.Object, global::Com.Google.Android.Exoplayer.Drm.IExoMediaDrm {

		internal static IntPtr java_class_handle;
		internal static IntPtr class_ref {
			get {
				return JNIEnv.FindClass ("com/google/android/exoplayer/drm/FrameworkMediaDrm", ref java_class_handle);
			}
		}

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return typeof (FrameworkMediaDrm); }
		}

		internal FrameworkMediaDrm (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		static IntPtr id_ctor;
		// Metadata.xml XPath constructor reference: path="/api/package[@name='com.google.android.exoplayer.drm']/class[@name='FrameworkMediaDrm']/constructor[@name='FrameworkMediaDrm' and count(parameter)=0]"
		[Register (".ctor", "()V", "")]
		public unsafe FrameworkMediaDrm ()
			: base (IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
		{
			if (((global::Java.Lang.Object) this).Handle != IntPtr.Zero)
				return;

			try {
				if (((object) this).GetType () != typeof (FrameworkMediaDrm)) {
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

		static IntPtr id_setOnEventListener_Lcom_google_android_exoplayer_drm_ExoMediaDrm_OnEventListener_;
		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.android.exoplayer.drm']/class[@name='FrameworkMediaDrm']/method[@name='setOnEventListener' and count(parameter)=1 and parameter[1][@type='com.google.android.exoplayer.drm.ExoMediaDrm.OnEventListener&lt;com.google.android.exoplayer.drm.FrameworkMediaCrypto&gt;']]"
		[Register ("setOnEventListener", "(Lcom/google/android/exoplayer/drm/ExoMediaDrm$OnEventListener;)V", "")]
		public unsafe void SetOnEventListener (global::Com.Google.Android.Exoplayer.Drm.IExoMediaDrmOnEventListener p0)
		{
			if (id_setOnEventListener_Lcom_google_android_exoplayer_drm_ExoMediaDrm_OnEventListener_ == IntPtr.Zero)
				id_setOnEventListener_Lcom_google_android_exoplayer_drm_ExoMediaDrm_OnEventListener_ = JNIEnv.GetMethodID (class_ref, "setOnEventListener", "(Lcom/google/android/exoplayer/drm/ExoMediaDrm$OnEventListener;)V");
			try {
				JValue* __args = stackalloc JValue [1];
				__args [0] = new JValue (p0);
				JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_setOnEventListener_Lcom_google_android_exoplayer_drm_ExoMediaDrm_OnEventListener_, __args);
			} finally {
			}
		}

		// This method is explicitly implemented as a member of an instantiated Com.Google.Android.Exoplayer.Drm.IExoMediaDrm
		void global::Com.Google.Android.Exoplayer.Drm.IExoMediaDrm.SetOnEventListener (global::Com.Google.Android.Exoplayer.Drm.IExoMediaDrmOnEventListener p0)
		{
			SetOnEventListener (global::Java.Interop.JavaObjectExtensions.JavaCast<global::Com.Google.Android.Exoplayer.Drm.IExoMediaDrmOnEventListener>(p0));
		}

	}
}
