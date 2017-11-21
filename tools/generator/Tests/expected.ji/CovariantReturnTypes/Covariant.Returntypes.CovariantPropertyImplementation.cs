using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Covariant.Returntypes {

	// Metadata.xml XPath class reference: path="/api/package[@name='covariant.returntypes']/class[@name='CovariantPropertyImplementation']"
	[global::Android.Runtime.Register ("covariant/returntypes/CovariantPropertyImplementation", DoNotGenerateAcw=true)]
	public partial class CovariantPropertyImplementation : global::Java.Lang.Object, global::Covariant.Returntypes.ICovariantPropertyInterface {

		internal    new     static  readonly    JniPeerMembers  _members    = new JniPeerMembers ("covariant/returntypes/CovariantPropertyImplementation", typeof (CovariantPropertyImplementation));
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

		protected CovariantPropertyImplementation (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		// Metadata.xml XPath constructor reference: path="/api/package[@name='covariant.returntypes']/class[@name='CovariantPropertyImplementation']/constructor[@name='CovariantPropertyImplementation' and count(parameter)=0]"
		[Register (".ctor", "()V", "")]
		public unsafe CovariantPropertyImplementation ()
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
			global::Covariant.Returntypes.CovariantPropertyImplementation __this = global::Java.Lang.Object.GetObject<global::Covariant.Returntypes.CovariantPropertyImplementation> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return JNIEnv.ToLocalJniHandle (__this.Object);
		}
#pragma warning restore 0169

		public virtual unsafe global::Java.Lang.Object Object {
			// Metadata.xml XPath method reference: path="/api/package[@name='covariant.returntypes']/class[@name='CovariantPropertyImplementation']/method[@name='getObject' and count(parameter)=0]"
			[Register ("getObject", "()Ljava/lang/String;", "GetGetObjectHandler")]
			get {
				const string __id = "getObject.()Ljava/lang/String;";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, null);
					return JNIEnv.GetString (__rm.Handle, JniHandleOwnership.TransferLocalRef);
				} finally {
				}
			}
		}

	}
}
