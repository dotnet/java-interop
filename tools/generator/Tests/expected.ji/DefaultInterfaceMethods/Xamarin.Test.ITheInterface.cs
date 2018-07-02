using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Xamarin.Test {

	partial interface ITheInterface {

		new static JniPeerMembers _members = new JniPeerMembers ("xamarin/test/TheInterface", typeof (ITheInterface));
	}

	// Metadata.xml XPath interface reference: path="/api/package[@name='xamarin.test']/interface[@name='TheInterface']"
	[Register ("xamarin/test/TheInterface", "", "Xamarin.Test.ITheInterfaceInvoker")]
	public partial interface ITheInterface : IJavaObject {

		static Delegate cb_getBar;
#pragma warning disable 0169
		static Delegate GetGetBarHandler ()
		{
			if (cb_getBar == null)
				cb_getBar = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, int>) n_GetBar);
			return cb_getBar;
		}

		static int n_GetBar (IntPtr jnienv, IntPtr native__this)
		{
			global::Xamarin.Test.ITheInterface __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.ITheInterface> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.Bar;
		}
#pragma warning restore 0169

		 virtual unsafe int Bar {
			// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/interface[@name='TheInterface']/method[@name='getBar' and count(parameter)=0]"
			[Register ("getBar", "()I", "GetGetBarHandler")]
			get {
				const string __id = "getBar.()I";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualInt32Method (__id, (IJavaPeerable) this, null);
					return __rm;
				} finally {
				}
			}
		}

		static Delegate cb_foo;
#pragma warning disable 0169
		static Delegate GetFooHandler ()
		{
			if (cb_foo == null)
				cb_foo = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, int>) n_Foo);
			return cb_foo;
		}

		static int n_Foo (IntPtr jnienv, IntPtr native__this)
		{
			global::Xamarin.Test.ITheInterface __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.ITheInterface> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.Foo ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/interface[@name='TheInterface']/method[@name='foo' and count(parameter)=0]"
		[Register ("foo", "()I", "GetFooHandler")]
		 virtual unsafe int Foo ()
		{
			const string __id = "foo.()I";
			try {
				var __rm = _members.InstanceMethods.InvokeVirtualInt32Method (__id, (IJavaPeerable) this, null);
				return __rm;
			} finally {
			}
		}

	}

	[global::Android.Runtime.Register ("xamarin/test/TheInterface", DoNotGenerateAcw=true)]
	internal class ITheInterfaceInvoker : global::Java.Lang.Object, ITheInterface {

		internal    new     static  readonly    JniPeerMembers  _members    = new JniPeerMembers ("xamarin/test/TheInterface", typeof (ITheInterfaceInvoker));

		static IntPtr java_class_ref {
			get { return _members.JniPeerType.PeerReference.Handle; }
		}

		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return _members; }
		}

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return _members.ManagedPeerType; }
		}

		IntPtr class_ref;

		public static ITheInterface GetObject (IntPtr handle, JniHandleOwnership transfer)
		{
			return global::Java.Lang.Object.GetObject<ITheInterface> (handle, transfer);
		}

		static IntPtr Validate (IntPtr handle)
		{
			if (!JNIEnv.IsInstanceOf (handle, java_class_ref))
				throw new InvalidCastException (string.Format ("Unable to convert instance of type '{0}' to type '{1}'.",
							JNIEnv.GetClassNameFromInstance (handle), "xamarin.test.TheInterface"));
			return handle;
		}

		protected override void Dispose (bool disposing)
		{
			if (this.class_ref != IntPtr.Zero)
				JNIEnv.DeleteGlobalRef (this.class_ref);
			this.class_ref = IntPtr.Zero;
			base.Dispose (disposing);
		}

		public ITheInterfaceInvoker (IntPtr handle, JniHandleOwnership transfer) : base (Validate (handle), transfer)
		{
			IntPtr local_ref = JNIEnv.GetObjectClass (((global::Java.Lang.Object) this).Handle);
			this.class_ref = JNIEnv.NewGlobalRef (local_ref);
			JNIEnv.DeleteLocalRef (local_ref);
		}

	}

}
