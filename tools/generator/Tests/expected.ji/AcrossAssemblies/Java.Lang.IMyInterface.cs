using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Java.Lang {

	[Register ("java/lang/MyInterface", DoNotGenerateAcw=true)]
	public abstract class MyInterface : Java.Lang.Object {

		internal MyInterface ()
		{
		}

		// Metadata.xml XPath field reference: path="/api/package[@name='java.lang']/interface[@name='MyInterface']/field[@name='MY_FIELD']"
		[Register ("MY_FIELD")]
		public const int MyField = (int) 256;
	}

	[Register ("java/lang/MyInterface", DoNotGenerateAcw=true)]
	[global::System.Obsolete ("Use the 'MyInterface' type. This type will be removed in a future release.")]
	public abstract class MyInterfaceConsts : MyInterface {

		private MyInterfaceConsts ()
		{
		}
	}

	// Metadata.xml XPath interface reference: path="/api/package[@name='java.lang']/interface[@name='MyInterface']"
	[Register ("java/lang/MyInterface", "", "Java.Lang.IMyInterfaceInvoker")]
	public partial interface IMyInterface : IJavaObject {

		// Metadata.xml XPath method reference: path="/api/package[@name='java.lang']/interface[@name='MyInterface']/method[@name='myMethod' and count(parameter)=0]"
		[Register ("myMethod", "()V", "GetMyMethodHandler:Java.Lang.IMyInterfaceInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")]
		void MyMethod ();

	}

	[global::Android.Runtime.Register ("java/lang/MyInterface", DoNotGenerateAcw=true)]
	internal class IMyInterfaceInvoker : global::Java.Lang.Object, IMyInterface {

		internal    new     static  readonly    JniPeerMembers  _members    = new JniPeerMembers ("java/lang/MyInterface", typeof (IMyInterfaceInvoker));

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

		public static IMyInterface GetObject (IntPtr handle, JniHandleOwnership transfer)
		{
			return global::Java.Lang.Object.GetObject<IMyInterface> (handle, transfer);
		}

		static IntPtr Validate (IntPtr handle)
		{
			if (!JNIEnv.IsInstanceOf (handle, java_class_ref))
				throw new InvalidCastException (string.Format ("Unable to convert instance of type '{0}' to type '{1}'.",
							JNIEnv.GetClassNameFromInstance (handle), "java.lang.MyInterface"));
			return handle;
		}

		protected override void Dispose (bool disposing)
		{
			if (this.class_ref != IntPtr.Zero)
				JNIEnv.DeleteGlobalRef (this.class_ref);
			this.class_ref = IntPtr.Zero;
			base.Dispose (disposing);
		}

		public IMyInterfaceInvoker (IntPtr handle, JniHandleOwnership transfer) : base (Validate (handle), transfer)
		{
			IntPtr local_ref = JNIEnv.GetObjectClass (((global::Java.Lang.Object) this).Handle);
			this.class_ref = JNIEnv.NewGlobalRef (local_ref);
			JNIEnv.DeleteLocalRef (local_ref);
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
			global::Java.Lang.IMyInterface __this = global::Java.Lang.Object.GetObject<global::Java.Lang.IMyInterface> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.MyMethod ();
		}
#pragma warning restore 0169

		IntPtr id_myMethod;
		public unsafe void MyMethod ()
		{
			if (id_myMethod == IntPtr.Zero)
				id_myMethod = JNIEnv.GetMethodID (class_ref, "myMethod", "()V");
			JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_myMethod);
		}

	}

}
