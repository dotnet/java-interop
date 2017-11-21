using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Covariant.Returntypes {

	// Metadata.xml XPath interface reference: path="/api/package[@name='covariant.returntypes']/interface[@name='GenericInterface']"
	[Register ("covariant/returntypes/GenericInterface", "", "Covariant.Returntypes.IGenericInterfaceInvoker")]
	[global::Java.Interop.JavaTypeParameters (new string [] {"T"})]
	public partial interface IGenericInterface : IJavaObject {

		// Metadata.xml XPath method reference: path="/api/package[@name='covariant.returntypes']/interface[@name='GenericInterface']/method[@name='someMethod' and count(parameter)=0]"
		[Register ("someMethod", "()Ljava/lang/Object;", "GetSomeMethodHandler:Covariant.Returntypes.IGenericInterfaceInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")]
		global::Java.Lang.Object SomeMethod ();

	}

	[global::Android.Runtime.Register ("covariant/returntypes/GenericInterface", DoNotGenerateAcw=true)]
	internal class IGenericInterfaceInvoker : global::Java.Lang.Object, IGenericInterface {

		static IntPtr java_class_ref = JNIEnv.FindClass ("covariant/returntypes/GenericInterface");

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return typeof (IGenericInterfaceInvoker); }
		}

		new IntPtr class_ref;

		public static IGenericInterface GetObject (IntPtr handle, JniHandleOwnership transfer)
		{
			return global::Java.Lang.Object.GetObject<IGenericInterface> (handle, transfer);
		}

		static IntPtr Validate (IntPtr handle)
		{
			if (!JNIEnv.IsInstanceOf (handle, java_class_ref))
				throw new InvalidCastException (string.Format ("Unable to convert instance of type '{0}' to type '{1}'.",
							JNIEnv.GetClassNameFromInstance (handle), "covariant.returntypes.GenericInterface"));
			return handle;
		}

		protected override void Dispose (bool disposing)
		{
			if (this.class_ref != IntPtr.Zero)
				JNIEnv.DeleteGlobalRef (this.class_ref);
			this.class_ref = IntPtr.Zero;
			base.Dispose (disposing);
		}

		public IGenericInterfaceInvoker (IntPtr handle, JniHandleOwnership transfer) : base (Validate (handle), transfer)
		{
			IntPtr local_ref = JNIEnv.GetObjectClass (((global::Java.Lang.Object) this).Handle);
			this.class_ref = JNIEnv.NewGlobalRef (local_ref);
			JNIEnv.DeleteLocalRef (local_ref);
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
			global::Covariant.Returntypes.IGenericInterface __this = global::Java.Lang.Object.GetObject<global::Covariant.Returntypes.IGenericInterface> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return JNIEnv.ToLocalJniHandle (__this.SomeMethod ());
		}
#pragma warning restore 0169

		IntPtr id_someMethod;
		public unsafe global::Java.Lang.Object SomeMethod ()
		{
			if (id_someMethod == IntPtr.Zero)
				id_someMethod = JNIEnv.GetMethodID (class_ref, "someMethod", "()Ljava/lang/Object;");
			return (Java.Lang.Object) global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (JNIEnv.CallObjectMethod (((global::Java.Lang.Object) this).Handle, id_someMethod), JniHandleOwnership.TransferLocalRef);
		}

	}

}
