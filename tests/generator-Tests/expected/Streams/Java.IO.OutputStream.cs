using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Java.IO {

	// Metadata.xml XPath class reference: path="/api/package[@name='java.io']/class[@name='OutputStream']"
	[global::Android.Runtime.Register ("java/io/OutputStream", DoNotGenerateAcw=true)]
	public abstract partial class OutputStream : global::Java.Lang.Object {

		internal static new IntPtr java_class_handle;
		internal static new IntPtr class_ref {
			get {
				return JNIEnv.FindClass ("java/io/OutputStream", ref java_class_handle);
			}
		}

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return typeof (OutputStream); }
		}

		protected OutputStream (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		static IntPtr id_ctor;
		// Metadata.xml XPath constructor reference: path="/api/package[@name='java.io']/class[@name='OutputStream']/constructor[@name='OutputStream' and count(parameter)=0]"
		[Register (".ctor", "()V", "")]
		public unsafe OutputStream ()
			: base (IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
		{
			if (((global::Java.Lang.Object) this).Handle != IntPtr.Zero)
				return;

			try {
				if (((object) this).GetType () != typeof (OutputStream)) {
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

		static Delegate cb_close;
#pragma warning disable 0169
		static Delegate GetCloseHandler ()
		{
			if (cb_close == null)
				cb_close = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_Close);
			return cb_close;
		}

		static void n_Close (IntPtr jnienv, IntPtr native__this)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Java.IO.OutputStream> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.Close ();
		}
#pragma warning restore 0169

		static IntPtr id_close;
		// Metadata.xml XPath method reference: path="/api/package[@name='java.io']/class[@name='OutputStream']/method[@name='close' and count(parameter)=0]"
		[Register ("close", "()V", "GetCloseHandler")]
		public virtual unsafe void Close ()
		{
			if (id_close == IntPtr.Zero)
				id_close = JNIEnv.GetMethodID (class_ref, "close", "()V");
			try {

				if (((object) this).GetType () == ThresholdType)
					JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_close);
				else
					JNIEnv.CallNonvirtualVoidMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "close", "()V"));
			} finally {
			}
		}

		static Delegate cb_flush;
#pragma warning disable 0169
		static Delegate GetFlushHandler ()
		{
			if (cb_flush == null)
				cb_flush = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_Flush);
			return cb_flush;
		}

		static void n_Flush (IntPtr jnienv, IntPtr native__this)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Java.IO.OutputStream> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.Flush ();
		}
#pragma warning restore 0169

		static IntPtr id_flush;
		// Metadata.xml XPath method reference: path="/api/package[@name='java.io']/class[@name='OutputStream']/method[@name='flush' and count(parameter)=0]"
		[Register ("flush", "()V", "GetFlushHandler")]
		public virtual unsafe void Flush ()
		{
			if (id_flush == IntPtr.Zero)
				id_flush = JNIEnv.GetMethodID (class_ref, "flush", "()V");
			try {

				if (((object) this).GetType () == ThresholdType)
					JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_flush);
				else
					JNIEnv.CallNonvirtualVoidMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "flush", "()V"));
			} finally {
			}
		}

		static Delegate cb_write_arrayB;
#pragma warning disable 0169
		static Delegate GetWrite_arrayBHandler ()
		{
			if (cb_write_arrayB == null)
				cb_write_arrayB = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, IntPtr>) n_Write_arrayB);
			return cb_write_arrayB;
		}

		static void n_Write_arrayB (IntPtr jnienv, IntPtr native__this, IntPtr native_buffer)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Java.IO.OutputStream> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var buffer = (byte[]) JNIEnv.GetArray (native_buffer, JniHandleOwnership.DoNotTransfer, typeof (byte));
			__this.Write (buffer);
			if (buffer != null)
				JNIEnv.CopyArray (buffer, native_buffer);
		}
#pragma warning restore 0169

		static IntPtr id_write_arrayB;
		// Metadata.xml XPath method reference: path="/api/package[@name='java.io']/class[@name='OutputStream']/method[@name='write' and count(parameter)=1 and parameter[1][@type='byte[]']]"
		[Register ("write", "([B)V", "GetWrite_arrayBHandler")]
		public virtual unsafe void Write (byte[] buffer)
		{
			if (id_write_arrayB == IntPtr.Zero)
				id_write_arrayB = JNIEnv.GetMethodID (class_ref, "write", "([B)V");
			IntPtr native_buffer = JNIEnv.NewArray (buffer);
			try {
				JValue* __args = stackalloc JValue [1];
				__args [0] = new JValue (native_buffer);

				if (((object) this).GetType () == ThresholdType)
					JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_write_arrayB, __args);
				else
					JNIEnv.CallNonvirtualVoidMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "write", "([B)V"), __args);
			} finally {
				if (buffer != null) {
					JNIEnv.CopyArray (native_buffer, buffer);
					JNIEnv.DeleteLocalRef (native_buffer);
				}
			}
		}

		static Delegate cb_write_arrayBII;
#pragma warning disable 0169
		static Delegate GetWrite_arrayBIIHandler ()
		{
			if (cb_write_arrayBII == null)
				cb_write_arrayBII = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, IntPtr, int, int>) n_Write_arrayBII);
			return cb_write_arrayBII;
		}

		static void n_Write_arrayBII (IntPtr jnienv, IntPtr native__this, IntPtr native_buffer, int offset, int count)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Java.IO.OutputStream> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var buffer = (byte[]) JNIEnv.GetArray (native_buffer, JniHandleOwnership.DoNotTransfer, typeof (byte));
			__this.Write (buffer, offset, count);
			if (buffer != null)
				JNIEnv.CopyArray (buffer, native_buffer);
		}
#pragma warning restore 0169

		static IntPtr id_write_arrayBII;
		// Metadata.xml XPath method reference: path="/api/package[@name='java.io']/class[@name='OutputStream']/method[@name='write' and count(parameter)=3 and parameter[1][@type='byte[]'] and parameter[2][@type='int'] and parameter[3][@type='int']]"
		[Register ("write", "([BII)V", "GetWrite_arrayBIIHandler")]
		public virtual unsafe void Write (byte[] buffer, int offset, int count)
		{
			if (id_write_arrayBII == IntPtr.Zero)
				id_write_arrayBII = JNIEnv.GetMethodID (class_ref, "write", "([BII)V");
			IntPtr native_buffer = JNIEnv.NewArray (buffer);
			try {
				JValue* __args = stackalloc JValue [3];
				__args [0] = new JValue (native_buffer);
				__args [1] = new JValue (offset);
				__args [2] = new JValue (count);

				if (((object) this).GetType () == ThresholdType)
					JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_write_arrayBII, __args);
				else
					JNIEnv.CallNonvirtualVoidMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "write", "([BII)V"), __args);
			} finally {
				if (buffer != null) {
					JNIEnv.CopyArray (native_buffer, buffer);
					JNIEnv.DeleteLocalRef (native_buffer);
				}
			}
		}

		static Delegate cb_write_I;
#pragma warning disable 0169
		static Delegate GetWrite_IHandler ()
		{
			if (cb_write_I == null)
				cb_write_I = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, int>) n_Write_I);
			return cb_write_I;
		}

		static void n_Write_I (IntPtr jnienv, IntPtr native__this, int oneByte)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Java.IO.OutputStream> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.Write (oneByte);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='java.io']/class[@name='OutputStream']/method[@name='write' and count(parameter)=1 and parameter[1][@type='int']]"
		[Register ("write", "(I)V", "GetWrite_IHandler")]
		public abstract void Write (int oneByte);

	}

	[global::Android.Runtime.Register ("java/io/OutputStream", DoNotGenerateAcw=true)]
	internal partial class OutputStreamInvoker : OutputStream {

		public OutputStreamInvoker (IntPtr handle, JniHandleOwnership transfer) : base (handle, transfer) {}

		protected override global::System.Type ThresholdType {
			get { return typeof (OutputStreamInvoker); }
		}

		static IntPtr id_write_I;
		// Metadata.xml XPath method reference: path="/api/package[@name='java.io']/class[@name='OutputStream']/method[@name='write' and count(parameter)=1 and parameter[1][@type='int']]"
		[Register ("write", "(I)V", "GetWrite_IHandler")]
		public override unsafe void Write (int oneByte)
		{
			if (id_write_I == IntPtr.Zero)
				id_write_I = JNIEnv.GetMethodID (class_ref, "write", "(I)V");
			try {
				JValue* __args = stackalloc JValue [1];
				__args [0] = new JValue (oneByte);
				JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_write_I, __args);
			} finally {
			}
		}

	}

}
