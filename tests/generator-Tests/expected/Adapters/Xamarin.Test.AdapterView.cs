using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Xamarin.Test {

	// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test']/class[@name='AdapterView']"
	[global::Android.Runtime.Register ("xamarin/test/AdapterView", DoNotGenerateAcw=true)]
	[global::Java.Interop.JavaTypeParameters (new string [] {"T extends xamarin.test.Adapter"})]
	public abstract partial class AdapterView : global::Java.Lang.Object {

		internal static new IntPtr java_class_handle;
		internal static new IntPtr class_ref {
			get {
				return JNIEnv.FindClass ("xamarin/test/AdapterView", ref java_class_handle);
			}
		}

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return typeof (AdapterView); }
		}

		protected AdapterView (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		static Delegate cb_getAdapter;
#pragma warning disable 0169
		static Delegate GetGetAdapterHandler ()
		{
			if (cb_getAdapter == null)
				cb_getAdapter = JNINativeWrapper.CreateDelegate ((_JniMarshal_PP_L) n_GetAdapter);
			return cb_getAdapter;
		}

		static IntPtr n_GetAdapter (IntPtr jnienv, IntPtr native__this)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.AdapterView> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return JNIEnv.ToLocalJniHandle (__this.RawAdapter);
		}
#pragma warning restore 0169

		static Delegate cb_setAdapter_Lxamarin_test_Adapter_;
#pragma warning disable 0169
		static Delegate GetSetAdapter_Lxamarin_test_Adapter_Handler ()
		{
			if (cb_setAdapter_Lxamarin_test_Adapter_ == null)
				cb_setAdapter_Lxamarin_test_Adapter_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_V) n_SetAdapter_Lxamarin_test_Adapter_);
			return cb_setAdapter_Lxamarin_test_Adapter_;
		}

		static void n_SetAdapter_Lxamarin_test_Adapter_ (IntPtr jnienv, IntPtr native__this, IntPtr native_adapter)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.AdapterView> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var adapter = global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (native_adapter, JniHandleOwnership.DoNotTransfer);
			__this.RawAdapter = adapter;
		}
#pragma warning restore 0169

		protected abstract global::Java.Lang.Object RawAdapter {
			// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='AdapterView']/method[@name='getAdapter' and count(parameter)=0]"
			[Register ("getAdapter", "()Lxamarin/test/Adapter;", "GetGetAdapterHandler")] get;
			// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='AdapterView']/method[@name='setAdapter' and count(parameter)=1 and parameter[1][@type='T']]"
			[Register ("setAdapter", "(Lxamarin/test/Adapter;)V", "GetSetAdapter_Lxamarin_test_Adapter_Handler")] set;
		}

	}

	[global::Android.Runtime.Register ("xamarin/test/AdapterView", DoNotGenerateAcw=true)]
	internal partial class AdapterViewInvoker : AdapterView {

		public AdapterViewInvoker (IntPtr handle, JniHandleOwnership transfer) : base (handle, transfer) {}

		protected override global::System.Type ThresholdType {
			get { return typeof (AdapterViewInvoker); }
		}

		static IntPtr id_getAdapter;
		static IntPtr id_setAdapter_Lxamarin_test_Adapter_;
		protected override unsafe global::Java.Lang.Object RawAdapter {
			// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='AdapterView']/method[@name='getAdapter' and count(parameter)=0]"
			[Register ("getAdapter", "()Lxamarin/test/Adapter;", "GetGetAdapterHandler")]
			get {
				if (id_getAdapter == IntPtr.Zero)
					id_getAdapter = JNIEnv.GetMethodID (class_ref, "getAdapter", "()Lxamarin/test/Adapter;");
				try {
					return (Java.Lang.Object) global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (JNIEnv.CallObjectMethod (((global::Java.Lang.Object) this).Handle, id_getAdapter), JniHandleOwnership.TransferLocalRef);
				} finally {
				}
			}
			// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='AdapterView']/method[@name='setAdapter' and count(parameter)=1 and parameter[1][@type='T']]"
			[Register ("setAdapter", "(Lxamarin/test/Adapter;)V", "GetSetAdapter_Lxamarin_test_Adapter_Handler")]
			set {
				if (id_setAdapter_Lxamarin_test_Adapter_ == IntPtr.Zero)
					id_setAdapter_Lxamarin_test_Adapter_ = JNIEnv.GetMethodID (class_ref, "setAdapter", "(Lxamarin/test/Adapter;)V");
				IntPtr native_value = JNIEnv.ToLocalJniHandle (value);
				try {
					JValue* __args = stackalloc JValue [1];
					__args [0] = new JValue (native_value);
					JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_setAdapter_Lxamarin_test_Adapter_, __args);
				} finally {
					JNIEnv.DeleteLocalRef (native_value);
				}
			}
		}

	}

}
