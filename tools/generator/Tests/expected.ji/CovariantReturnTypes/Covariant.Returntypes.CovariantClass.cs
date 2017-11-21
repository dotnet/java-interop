using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Covariant.Returntypes {

	// Metadata.xml XPath class reference: path="/api/package[@name='covariant.returntypes']/class[@name='CovariantClass']"
	[global::Android.Runtime.Register ("covariant/returntypes/CovariantClass", DoNotGenerateAcw=true)]
	public partial class CovariantClass : global::Java.Lang.Object {

		internal    new     static  readonly    JniPeerMembers  _members    = new JniPeerMembers ("covariant/returntypes/CovariantClass", typeof (CovariantClass));
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

		protected CovariantClass (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

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
			global::Covariant.Returntypes.CovariantClass __this = global::Java.Lang.Object.GetObject<global::Covariant.Returntypes.CovariantClass> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return JNIEnv.ToLocalJniHandle (__this.SomeMethod ());
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='covariant.returntypes']/class[@name='CovariantClass']/method[@name='someMethod' and count(parameter)=0]"
		[Register ("someMethod", "()Ljava/lang/Object;", "GetSomeMethodHandler")]
		public virtual unsafe global::Java.Lang.Object SomeMethod ()
		{
			const string __id = "someMethod.()Ljava/lang/Object;";
			try {
				var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, null);
				return global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
			}
		}

	}
}
