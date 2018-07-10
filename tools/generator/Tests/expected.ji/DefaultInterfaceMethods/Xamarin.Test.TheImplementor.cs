using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Xamarin.Test {

	// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test']/class[@name='TheImplementor']"
	[global::Android.Runtime.Register ("xamarin/test/TheImplementor", DoNotGenerateAcw=true)]
	public partial class TheImplementor : global::Java.Lang.Object, global::Xamarin.Test.ITheInterface {

		internal    new     static  readonly    JniPeerMembers  _members    = new JniPeerMembers ("xamarin/test/TheImplementor", typeof (TheImplementor));
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

		protected TheImplementor (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		// Metadata.xml XPath constructor reference: path="/api/package[@name='xamarin.test']/class[@name='TheImplementor']/constructor[@name='TheImplementor' and count(parameter)=0]"
		[Register (".ctor", "()V", "")]
		public unsafe TheImplementor ()
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

	}
}
