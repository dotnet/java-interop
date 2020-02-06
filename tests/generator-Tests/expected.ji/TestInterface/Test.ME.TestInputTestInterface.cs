using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Test.ME {

	// Metadata.xml XPath class reference: path="/api/package[@name='test.me']/class[@name='TestInputTestInterface']"
	[global::Android.Runtime.Register ("test/me/TestInputTestInterface", DoNotGenerateAcw=true)]
	public partial class TestInputTestInterface : global::Java.Lang.Object, global::Test.ME.IInputTest {


		public static class InterfaceConsts {

			// The following are fields from: test.me.InputTest

			// Metadata.xml XPath field reference: path="/api/package[@name='test.me']/interface[@name='InputTest']/field[@name='TYPE_CLASS_DATETIME']"
			[Register ("TYPE_CLASS_DATETIME")]
			public const int TypeClassDatetime = (int) 4;

			// Metadata.xml XPath field reference: path="/api/package[@name='test.me']/interface[@name='InputTest']/field[@name='TYPE_CLASS_NUMBER']"
			[Register ("TYPE_CLASS_NUMBER")]
			public const int TypeClassNumber = (int) 2;
		}

		static readonly JniPeerMembers _members = new JniPeerMembers ("test/me/TestInputTestInterface", typeof (TestInputTestInterface));
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

		protected TestInputTestInterface (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		// Metadata.xml XPath constructor reference: path="/api/package[@name='test.me']/class[@name='TestInputTestInterface']/constructor[@name='TestInputTestInterface' and count(parameter)=0]"
		[Register (".ctor", "()V", "")]
		public unsafe TestInputTestInterface ()
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

		static Delegate cb_describeContents;
#pragma warning disable 0169
		static Delegate GetDescribeContentsHandler ()
		{
			if (cb_describeContents == null)
				cb_describeContents = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, int>) n_DescribeContents);
			return cb_describeContents;
		}

		static int n_DescribeContents (IntPtr jnienv, IntPtr native__this)
		{
			global::Test.ME.TestInputTestInterface __this = global::Java.Lang.Object.GetObject<global::Test.ME.TestInputTestInterface> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.DescribeContents ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='test.me']/class[@name='TestInputTestInterface']/method[@name='describeContents' and count(parameter)=0]"
		[Register ("describeContents", "()I", "GetDescribeContentsHandler")]
		public virtual unsafe int DescribeContents ()
		{
			const string __id = "describeContents.()I";
			try {
				var __rm = _members.InstanceMethods.InvokeVirtualInt32Method (__id, this, null);
				return __rm;
			} finally {
			}
		}

	}
}
