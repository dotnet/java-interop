using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Xamarin.Test {

	// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test']/class[@name='A']"
	[global::Android.Runtime.Register ("xamarin/test/A", DoNotGenerateAcw=true)]
	public partial class A : global::Java.Lang.Object {

		// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test']/class[@name='A.B']"
		[global::Android.Runtime.Register ("xamarin/test/A$B", DoNotGenerateAcw=true)]
		[global::Java.Interop.JavaTypeParameters (new string [] {"T extends xamarin.test.A.B"})]
		public partial class B : global::Java.Lang.Object {

			internal static new readonly JniPeerMembers _members = new JniPeerMembers ("xamarin/test/A$B", typeof (B));
			internal static new IntPtr class_ref {
				get {
					return _members.JniPeerType.PeerReference.Handle;
				}
			}

			public override global::Java.Interop.JniPeerMembers JniPeerMembers {
				get { return _members; }
			}

			protected override IntPtr ThresholdClass {
				get { return _members.JniPeerType.PeerReference.Handle; }
			}

			protected override global::System.Type ThresholdType {
				get { return _members.ManagedPeerType; }
			}

			protected B (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

			static Delegate cb_setCustomDimension_I;
#pragma warning disable 0169
			static Delegate GetSetCustomDimension_IHandler ()
			{
				if (cb_setCustomDimension_I == null)
					cb_setCustomDimension_I = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, int, IntPtr>) n_SetCustomDimension_I);
				return cb_setCustomDimension_I;
			}

			static IntPtr n_SetCustomDimension_I (IntPtr jnienv, IntPtr native__this, int index)
			{
				global::Xamarin.Test.A.B __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.A.B> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
				return JNIEnv.ToLocalJniHandle (__this.SetCustomDimension (index));
			}
#pragma warning restore 0169

			// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='A.B']/method[@name='setCustomDimension' and count(parameter)=1 and parameter[1][@type='int']]"
			[Register ("setCustomDimension", "(I)Lxamarin/test/A$B;", "GetSetCustomDimension_IHandler")]
			public virtual unsafe global::Java.Lang.Object SetCustomDimension (int index)
			{
				const string __id = "setCustomDimension.(I)Lxamarin/test/A$B;";
				try {
					JniArgumentValue* __args = stackalloc JniArgumentValue [1];
					__args [0] = new JniArgumentValue (index);
					var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, __args);
					return (Java.Lang.Object) global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
				} finally {
				}
			}

		}

		internal static new readonly JniPeerMembers _members = new JniPeerMembers ("xamarin/test/A", typeof (A));
		internal static new IntPtr class_ref {
			get {
				return _members.JniPeerType.PeerReference.Handle;
			}
		}

		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return _members; }
		}

		protected override IntPtr ThresholdClass {
			get { return _members.JniPeerType.PeerReference.Handle; }
		}

		protected override global::System.Type ThresholdType {
			get { return _members.ManagedPeerType; }
		}

		protected A (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		static Delegate cb_getHandle;
#pragma warning disable 0169
		static Delegate GetGetHandleHandler ()
		{
			if (cb_getHandle == null)
				cb_getHandle = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, int>) n_GetHandle);
			return cb_getHandle;
		}

		static int n_GetHandle (IntPtr jnienv, IntPtr native__this)
		{
			global::Xamarin.Test.A __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.A> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.GetHandle ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='A']/method[@name='getHandle' and count(parameter)=0]"
		[Register ("getHandle", "()I", "GetGetHandleHandler")]
		public virtual unsafe int GetHandle ()
		{
			const string __id = "getHandle.()I";
			try {
				var __rm = _members.InstanceMethods.InvokeVirtualInt32Method (__id, this, null);
				return __rm;
			} finally {
			}
		}

	}
}
