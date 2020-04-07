using System;
using System.Collections.Generic;
using Android.Runtime;

// Metadata.xml XPath class reference: path="/api/package[@name='']/class[@name='ClassWithoutNamespace']"
[global::Android.Runtime.Register ("ClassWithoutNamespace", DoNotGenerateAcw=true)]
public abstract partial class ClassWithoutNamespace : global::Java.Lang.Object, IInterfaceWithoutNamespace {

	internal static new IntPtr java_class_handle;
	internal static new IntPtr class_ref {
		get {
			return JNIEnv.FindClass ("ClassWithoutNamespace", ref java_class_handle);
		}
	}

	protected override IntPtr ThresholdClass {
		get { return class_ref; }
	}

	protected override global::System.Type ThresholdType {
		get { return typeof (ClassWithoutNamespace); }
	}

	protected ClassWithoutNamespace (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

	static IntPtr id_ctor;
	// Metadata.xml XPath constructor reference: path="/api/package[@name='']/class[@name='ClassWithoutNamespace']/constructor[@name='ClassWithoutNamespace' and count(parameter)=0]"
	[Register (".ctor", "()V", "")]
	public unsafe ClassWithoutNamespace ()
		: base (IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
	{
		if (((global::Java.Lang.Object) this).Handle != IntPtr.Zero)
			return;

		try {
			if (((object) this).GetType () != typeof (ClassWithoutNamespace)) {
				SetHandle (
						global::Android.Runtime.JNIEnv.StartCreateInstance (((object) this).GetType (), "()V"),
						JniHandleOwnership.TransferLocalRef);
				global::Android.Runtime.JNIEnv.FinishCreateInstance (((global::Java.Lang.Object) this).Handle, "()V");
				return;
			}

			if (id_ctor == IntPtr.Zero)
				id_ctor = JNIEnv.GetMethodID (class_ref, "<init>", "()V");
			SetHandle (
					global::Android.Runtime.JNIEnv.StartCreateInstance (class_ref, id_ctor),
					JniHandleOwnership.TransferLocalRef);
			JNIEnv.FinishCreateInstance (((global::Java.Lang.Object) this).Handle, class_ref, id_ctor);
		} finally {
		}
	}

	static Delegate cb_Foo;
#pragma warning disable 0169
	static Delegate GetFooHandler ()
	{
		if (cb_Foo == null)
			cb_Foo = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_Foo);
		return cb_Foo;
	}

	static void n_Foo (IntPtr jnienv, IntPtr native__this)
	{
		var __this = global::Java.Lang.Object.GetObject<ClassWithoutNamespace> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
		__this.Foo ();
	}
#pragma warning restore 0169

	// Metadata.xml XPath method reference: path="/api/package[@name='']/interface[@name='InterfaceWithoutNamespace']/method[@name='Foo' and count(parameter)=0]"
	[Register ("Foo", "()V", "GetFooHandler")]
	public abstract void Foo ();

}

[global::Android.Runtime.Register ("ClassWithoutNamespace", DoNotGenerateAcw=true)]
internal partial class ClassWithoutNamespaceInvoker : ClassWithoutNamespace {

	public ClassWithoutNamespaceInvoker (IntPtr handle, JniHandleOwnership transfer) : base (handle, transfer) {}

	protected override global::System.Type ThresholdType {
		get { return typeof (ClassWithoutNamespaceInvoker); }
	}

	static IntPtr id_Foo;
	// Metadata.xml XPath method reference: path="/api/package[@name='']/interface[@name='InterfaceWithoutNamespace']/method[@name='Foo' and count(parameter)=0]"
	[Register ("Foo", "()V", "GetFooHandler")]
	public override unsafe void Foo ()
	{
		if (id_Foo == IntPtr.Zero)
			id_Foo = JNIEnv.GetMethodID (class_ref, "Foo", "()V");
		try {
			JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_Foo);
		} finally {
		}
	}

}

