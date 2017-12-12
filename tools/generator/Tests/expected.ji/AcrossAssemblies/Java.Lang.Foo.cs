using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Java.Lang {

	// Metadata.xml XPath class reference: path="/api/package[@name='java.lang']/class[@name='Foo']"
	[global::Android.Runtime.Register ("java/lang/Foo", DoNotGenerateAcw=true)]
	public partial class Foo : global::Java.Lang.Object {

		internal    new     static  readonly    JniPeerMembers  _members    = new JniPeerMembers ("java/lang/Foo", typeof (Foo));
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

		protected Foo (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		static Delegate cb_bar;
#pragma warning disable 0169
		static Delegate GetBarHandler ()
		{
			if (cb_bar == null)
				cb_bar = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_Bar);
			return cb_bar;
		}

		static void n_Bar (IntPtr jnienv, IntPtr native__this)
		{
			global::Java.Lang.Foo __this = global::Java.Lang.Object.GetObject<global::Java.Lang.Foo> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.Bar ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='java.lang']/class[@name='Foo']/method[@name='bar' and count(parameter)=0]"
		[Register ("bar", "()V", "GetBarHandler")]
		public virtual unsafe void Bar ()
		{
			const string __id = "bar.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

	}
}
