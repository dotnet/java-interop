using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Xamarin.Test {

	// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test']/class[@name='GenericReturnObject']"
	[global::Android.Runtime.Register ("xamarin/test/GenericReturnObject", DoNotGenerateAcw=true)]
	public partial class GenericReturnObject : global::Java.Lang.Object {

		static readonly JniPeerMembers _members = new JniPeerMembers ("xamarin/test/GenericReturnObject", typeof (GenericReturnObject));
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

		protected GenericReturnObject (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		static Delegate cb_GenericReturn;
#pragma warning disable 0169
		static Delegate GetGenericReturnHandler ()
		{
			if (cb_GenericReturn == null)
				cb_GenericReturn = JNINativeWrapper.CreateDelegate ((_JniMarshal_PP_L) n_GenericReturn);
			return cb_GenericReturn;
		}

		static IntPtr n_GenericReturn (IntPtr jnienv, IntPtr native__this)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.GenericReturnObject> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return JNIEnv.ToLocalJniHandle (__this.GenericReturn ());
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='GenericReturnObject']/method[@name='GenericReturn' and count(parameter)=0]"
		[Register ("GenericReturn", "()Lxamarin/test/AdapterView;", "GetGenericReturnHandler")]
		public virtual unsafe global::Xamarin.Test.AdapterView GenericReturn ()
		{
			const string __id = "GenericReturn.()Lxamarin/test/AdapterView;";
			try {
				var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, null);
				return global::Java.Lang.Object.GetObject<global::Xamarin.Test.AdapterView> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
			}
		}

	}
}
