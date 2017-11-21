using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Covariant.Returntypes {

	// Metadata.xml XPath class reference: path="/api/package[@name='covariant.returntypes']/class[@name='CovariantInterfaceImplementation']"
	[global::Android.Runtime.Register ("covariant/returntypes/CovariantInterfaceImplementation", DoNotGenerateAcw=true)]
	public partial class CovariantInterfaceImplementation : global::Java.Lang.Object, global::Covariant.Returntypes.ICovariantInterface {

		internal    new     static  readonly    JniPeerMembers  _members    = new JniPeerMembers ("covariant/returntypes/CovariantInterfaceImplementation", typeof (CovariantInterfaceImplementation));
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

		protected CovariantInterfaceImplementation (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		// Metadata.xml XPath constructor reference: path="/api/package[@name='covariant.returntypes']/class[@name='CovariantInterfaceImplementation']/constructor[@name='CovariantInterfaceImplementation' and count(parameter)=0]"
		[Register (".ctor", "()V", "")]
		public unsafe CovariantInterfaceImplementation ()
			: base (IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
		{
			const string __id = "()V";

			if (((global::Java.Lang.Object) this).Handle != IntPtr.Zero)
				return;

			try {
				var __r = _members.InstanceMethods.StartCreateInstance (__id, ((object) this).GetType (), null);
				SetHandle (__r.Handle, JniHandleOwnership.TransferLocalRef);
				_members.InstanceMethods.FinishCreateInstance (__id, this, null);
			} finally {
			}
		}

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
			global::Covariant.Returntypes.CovariantInterfaceImplementation __this = global::Java.Lang.Object.GetObject<global::Covariant.Returntypes.CovariantInterfaceImplementation> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return JNIEnv.ToLocalJniHandle (__this.SomeMethod ());
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='covariant.returntypes']/class[@name='CovariantInterfaceImplementation']/method[@name='someMethod' and count(parameter)=0]"
		[Register ("someMethod", "()Ljava/lang/String;", "GetSomeMethodHandler")]
		public virtual unsafe global::Java.Lang.Object SomeMethod ()
		{
			const string __id = "someMethod.()Ljava/lang/String;";
			try {
				var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, null);
				return JNIEnv.GetString (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
			}
		}

	}
}
