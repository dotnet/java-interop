using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Xamarin.Test {

	partial interface ITheInterface {

		new static JniPeerMembers _members = new JniPeerMembers ("xamarin/test/TheInterface", typeof (ITheInterface));
	}

	// Metadata.xml XPath interface reference: path="/api/package[@name='xamarin.test']/interface[@name='TheInterface']"
	[Register ("xamarin/test/TheInterface", "", "Xamarin.Test.ITheInterfaceInvoker")]
	public partial interface ITheInterface : IJavaObject, IJavaPeerable {

		static Delegate cb_getBar;
#pragma warning disable 0169
		static Delegate GetGetBarHandler ()
		{
			if (cb_getBar == null)
				cb_getBar = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, int>) n_GetBar);
			return cb_getBar;
		}

		static int n_GetBar (IntPtr jnienv, IntPtr native__this)
		{
			global::Xamarin.Test.ITheInterface __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.ITheInterface> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.Bar;
		}
#pragma warning restore 0169

		 virtual unsafe int Bar {
			// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/interface[@name='TheInterface']/method[@name='getBar' and count(parameter)=0]"
			[Register ("getBar", "()I", "GetGetBarHandler")]
			get {
				const string __id = "getBar.()I";
				try {
					var __rm = _members.InstanceMethods.InvokeNonvirtualInt32Method (__id, (IJavaPeerable) this, null);
					return __rm;
				} finally {
				}
			}
		}

		static Delegate cb_getStringProp;
#pragma warning disable 0169
		static Delegate GetGetStringPropHandler ()
		{
			if (cb_getStringProp == null)
				cb_getStringProp = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, IntPtr>) n_GetStringProp);
			return cb_getStringProp;
		}

		static IntPtr n_GetStringProp (IntPtr jnienv, IntPtr native__this)
		{
			global::Xamarin.Test.ITheInterface __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.ITheInterface> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return JNIEnv.NewString (__this.StringProp);
		}
#pragma warning restore 0169

		static Delegate cb_setStringProp_Ljava_lang_String_;
#pragma warning disable 0169
		static Delegate GetSetStringProp_Ljava_lang_String_Handler ()
		{
			if (cb_setStringProp_Ljava_lang_String_ == null)
				cb_setStringProp_Ljava_lang_String_ = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, IntPtr>) n_SetStringProp_Ljava_lang_String_);
			return cb_setStringProp_Ljava_lang_String_;
		}

		static void n_SetStringProp_Ljava_lang_String_ (IntPtr jnienv, IntPtr native__this, IntPtr native_v)
		{
			global::Xamarin.Test.ITheInterface __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.ITheInterface> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			string v = JNIEnv.GetString (native_v, JniHandleOwnership.DoNotTransfer);
			__this.StringProp = v;
		}
#pragma warning restore 0169

		 virtual unsafe string StringProp {
			// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/interface[@name='TheInterface']/method[@name='getStringProp' and count(parameter)=0]"
			[Register ("getStringProp", "()Ljava/lang/String;", "GetGetStringPropHandler")]
			get {
				const string __id = "getStringProp.()Ljava/lang/String;";
				try {
					var __rm = _members.InstanceMethods.InvokeNonvirtualObjectMethod (__id, (IJavaPeerable) this, null);
					return JNIEnv.GetString (__rm.Handle, JniHandleOwnership.TransferLocalRef);
				} finally {
				}
			}
			// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/interface[@name='TheInterface']/method[@name='setStringProp' and count(parameter)=1 and parameter[1][@type='java.lang.String']]"
			[Register ("setStringProp", "(Ljava/lang/String;)V", "GetSetStringProp_Ljava_lang_String_Handler")]
			set {
				const string __id = "setStringProp.(Ljava/lang/String;)V";
				IntPtr native_value = JNIEnv.NewString (value);
				try {
					JniArgumentValue* __args = stackalloc JniArgumentValue [1];
					__args [0] = new JniArgumentValue (native_value);
					_members.InstanceMethods.InvokeNonvirtualVoidMethod (__id, (IJavaPeerable) this, __args);
				} finally {
					JNIEnv.DeleteLocalRef (native_value);
				}
			}
		}

		static Delegate cb_foo;
#pragma warning disable 0169
		static Delegate GetFooHandler ()
		{
			if (cb_foo == null)
				cb_foo = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, int>) n_Foo);
			return cb_foo;
		}

		static int n_Foo (IntPtr jnienv, IntPtr native__this)
		{
			global::Xamarin.Test.ITheInterface __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.ITheInterface> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.Foo ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/interface[@name='TheInterface']/method[@name='foo' and count(parameter)=0]"
		[Register ("foo", "()I", "GetFooHandler")]
		 virtual unsafe int Foo ()
		{
			const string __id = "foo.()I";
			try {
				var __rm = _members.InstanceMethods.InvokeNonvirtualInt32Method (__id, (IJavaPeerable) this, null);
				return __rm;
			} finally {
			}
		}

		static Delegate cb_string1;
#pragma warning disable 0169
		static Delegate GetString1Handler ()
		{
			if (cb_string1 == null)
				cb_string1 = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, IntPtr>) n_String1);
			return cb_string1;
		}

		static IntPtr n_String1 (IntPtr jnienv, IntPtr native__this)
		{
			global::Xamarin.Test.ITheInterface __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.ITheInterface> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return JNIEnv.NewString (__this.String1 ());
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/interface[@name='TheInterface']/method[@name='string1' and count(parameter)=0]"
		[Register ("string1", "()Ljava/lang/String;", "GetString1Handler")]
		 virtual unsafe string String1 ()
		{
			const string __id = "string1.()Ljava/lang/String;";
			try {
				var __rm = _members.InstanceMethods.InvokeNonvirtualObjectMethod (__id, (IJavaPeerable) this, null);
				return JNIEnv.GetString (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
			}
		}

		static Delegate cb_string1_Ljava_lang_String_arrayLjava_lang_String_arrayLjava_lang_String_;
#pragma warning disable 0169
		static Delegate GetString1_Ljava_lang_String_arrayLjava_lang_String_arrayLjava_lang_String_Handler ()
		{
			if (cb_string1_Ljava_lang_String_arrayLjava_lang_String_arrayLjava_lang_String_ == null)
				cb_string1_Ljava_lang_String_arrayLjava_lang_String_arrayLjava_lang_String_ = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr>) n_String1_Ljava_lang_String_arrayLjava_lang_String_arrayLjava_lang_String_);
			return cb_string1_Ljava_lang_String_arrayLjava_lang_String_arrayLjava_lang_String_;
		}

		static IntPtr n_String1_Ljava_lang_String_arrayLjava_lang_String_arrayLjava_lang_String_ (IntPtr jnienv, IntPtr native__this, IntPtr native_p1, IntPtr native_p2, IntPtr native_p3)
		{
			global::Xamarin.Test.ITheInterface __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.ITheInterface> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			string p1 = JNIEnv.GetString (native_p1, JniHandleOwnership.DoNotTransfer);
			string[] p2 = (string[]) JNIEnv.GetArray (native_p2, JniHandleOwnership.DoNotTransfer, typeof (string));
			string[] p3 = (string[]) JNIEnv.GetArray (native_p3, JniHandleOwnership.DoNotTransfer, typeof (string));
			IntPtr __ret = JNIEnv.NewString (__this.String1 (p1, p2, p3));
			if (p2 != null)
				JNIEnv.CopyArray (p2, native_p2);
			if (p3 != null)
				JNIEnv.CopyArray (p3, native_p3);
			return __ret;
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/interface[@name='TheInterface']/method[@name='string1' and count(parameter)=3 and parameter[1][@type='java.lang.String'] and parameter[2][@type='java.lang.String[]'] and parameter[3][@type='java.lang.String...']]"
		[Register ("string1", "(Ljava/lang/String;[Ljava/lang/String;[Ljava/lang/String;)Ljava/lang/String;", "GetString1_Ljava_lang_String_arrayLjava_lang_String_arrayLjava_lang_String_Handler")]
		 virtual unsafe string String1 (string p1, string[] p2, params string[] p3)
		{
			const string __id = "string1.(Ljava/lang/String;[Ljava/lang/String;[Ljava/lang/String;)Ljava/lang/String;";
			IntPtr native_p1 = JNIEnv.NewString (p1);
			IntPtr native_p2 = JNIEnv.NewArray (p2);
			IntPtr native_p3 = JNIEnv.NewArray (p3);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [3];
				__args [0] = new JniArgumentValue (native_p1);
				__args [1] = new JniArgumentValue (native_p2);
				__args [2] = new JniArgumentValue (native_p3);
				var __rm = _members.InstanceMethods.InvokeNonvirtualObjectMethod (__id, (IJavaPeerable) this, __args);
				return JNIEnv.GetString (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
				JNIEnv.DeleteLocalRef (native_p1);
				if (p2 != null) {
					JNIEnv.CopyArray (native_p2, p2);
					JNIEnv.DeleteLocalRef (native_p2);
				}
				if (p3 != null) {
					JNIEnv.CopyArray (native_p3, p3);
					JNIEnv.DeleteLocalRef (native_p3);
				}
			}
		}

	}

	[global::Android.Runtime.Register ("xamarin/test/TheInterface", DoNotGenerateAcw=true)]
	internal class ITheInterfaceInvoker : global::Java.Lang.Object, ITheInterface {

		internal    new     static  readonly    JniPeerMembers  _members    = new JniPeerMembers ("xamarin/test/TheInterface", typeof (ITheInterfaceInvoker));

		static IntPtr java_class_ref {
			get { return _members.JniPeerType.PeerReference.Handle; }
		}

		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return _members; }
		}

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return _members.ManagedPeerType; }
		}

		IntPtr class_ref;

		public static ITheInterface GetObject (IntPtr handle, JniHandleOwnership transfer)
		{
			return global::Java.Lang.Object.GetObject<ITheInterface> (handle, transfer);
		}

		static IntPtr Validate (IntPtr handle)
		{
			if (!JNIEnv.IsInstanceOf (handle, java_class_ref))
				throw new InvalidCastException (string.Format ("Unable to convert instance of type '{0}' to type '{1}'.",
							JNIEnv.GetClassNameFromInstance (handle), "xamarin.test.TheInterface"));
			return handle;
		}

		protected override void Dispose (bool disposing)
		{
			if (this.class_ref != IntPtr.Zero)
				JNIEnv.DeleteGlobalRef (this.class_ref);
			this.class_ref = IntPtr.Zero;
			base.Dispose (disposing);
		}

		public ITheInterfaceInvoker (IntPtr handle, JniHandleOwnership transfer) : base (Validate (handle), transfer)
		{
			IntPtr local_ref = JNIEnv.GetObjectClass (((global::Java.Lang.Object) this).Handle);
			this.class_ref = JNIEnv.NewGlobalRef (local_ref);
			JNIEnv.DeleteLocalRef (local_ref);
		}

	}

}
