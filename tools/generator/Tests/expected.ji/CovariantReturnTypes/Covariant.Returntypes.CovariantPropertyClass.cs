using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Covariant.Returntypes {

	// Metadata.xml XPath class reference: path="/api/package[@name='covariant.returntypes']/class[@name='CovariantPropertyClass']"
	[global::Android.Runtime.Register ("covariant/returntypes/CovariantPropertyClass", DoNotGenerateAcw=true)]
	public partial class CovariantPropertyClass : global::Java.Lang.Object {

		internal    new     static  readonly    JniPeerMembers  _members    = new JniPeerMembers ("covariant/returntypes/CovariantPropertyClass", typeof (CovariantPropertyClass));
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

		public virtual unsafe global::Java.Lang.Object Object {
			// Metadata.xml XPath method reference: path="/api/package[@name='covariant.returntypes']/class[@name='CovariantPropertyClass']/method[@name='getObject' and count(parameter)=0]"
			[Register ("getObject", "()Ljava/lang/Object;", "GetGetObjectHandler")]
			get {
				const string __id = "getObject.()Ljava/lang/Object;";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, null);
					return global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
				} finally {
				}
			}
		}

	}
}
