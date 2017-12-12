using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Xamarin.Test {

	// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test']/class[@name='MyInterfaceImplementation']"
	[global::Android.Runtime.Register ("xamarin/test/MyInterfaceImplementation", DoNotGenerateAcw=true)]
	public abstract partial class MyInterfaceImplementation : global::Java.Lang.Object, global::Java.Lang.IMyInterface {


		public static class InterfaceConsts {

			// The following are fields from: java.lang.MyInterface

			// Metadata.xml XPath field reference: path="/api/package[@name='java.lang']/interface[@name='MyInterface']/field[@name='MY_FIELD']"
			[Register ("MY_FIELD")]
			public const int MyField = (int) 256;
		}

		internal    new     static  readonly    JniPeerMembers  _members    = new JniPeerMembers ("xamarin/test/MyInterfaceImplementation", typeof (MyInterfaceImplementation));
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

		protected MyInterfaceImplementation (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		// Metadata.xml XPath constructor reference: path="/api/package[@name='xamarin.test']/class[@name='MyInterfaceImplementation']/constructor[@name='MyInterfaceImplementation' and count(parameter)=0]"
		[Register (".ctor", "()V", "")]
		public unsafe MyInterfaceImplementation ()
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

		static Delegate cb_myMethod;
#pragma warning disable 0169
		static Delegate GetMyMethodHandler ()
		{
			if (cb_myMethod == null)
				cb_myMethod = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_MyMethod);
			return cb_myMethod;
		}

		static void n_MyMethod (IntPtr jnienv, IntPtr native__this)
		{
			global::Xamarin.Test.MyInterfaceImplementation __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.MyInterfaceImplementation> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.MyMethod ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='MyInterfaceImplementation']/method[@name='myMethod' and count(parameter)=0]"
		[Register ("myMethod", "()V", "GetMyMethodHandler")]
		public virtual unsafe void MyMethod ()
		{
			const string __id = "myMethod.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

	}

	[global::Android.Runtime.Register ("xamarin/test/MyInterfaceImplementation", DoNotGenerateAcw=true)]
	internal partial class MyInterfaceImplementationInvoker : MyInterfaceImplementation {

		public MyInterfaceImplementationInvoker (IntPtr handle, JniHandleOwnership transfer) : base (handle, transfer) {}

		internal    new     static  readonly    JniPeerMembers  _members    = new JniPeerMembers ("xamarin/test/MyInterfaceImplementation", typeof (MyInterfaceImplementationInvoker));

		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return _members; }
		}

		protected override global::System.Type ThresholdType {
			get { return _members.ManagedPeerType; }
		}

	}

}
