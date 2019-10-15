using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Xamarin.Test {

	// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test']/class[@name='FieldMethodNameColisionClass']"
	[global::Android.Runtime.Register ("xamarin/test/FieldMethodNameColisionClass", DoNotGenerateAcw=true)]
	public partial class FieldMethodNameColisionClass : global::Java.Lang.Object {


		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='FieldMethodNameColisionClass']/field[@name='ABC']"
		[Register ("ABC")]
		public const bool Abc = (bool) true;

		static IntPtr item_jfieldId;

		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='FieldMethodNameColisionClass']/field[@name='item']"
		[Register ("item")]
		public int Item {
			get {
				if (item_jfieldId == IntPtr.Zero)
					item_jfieldId = JNIEnv.GetFieldID (class_ref, "item", "I");
				return JNIEnv.GetIntField (((global::Java.Lang.Object) this).Handle, item_jfieldId);
			}
			set {
				if (item_jfieldId == IntPtr.Zero)
					item_jfieldId = JNIEnv.GetFieldID (class_ref, "item", "I");
				try {
					JNIEnv.SetField (((global::Java.Lang.Object) this).Handle, item_jfieldId, value);
				} finally {
				}
			}
		}

		static IntPtr name_jfieldId;

		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='FieldMethodNameColisionClass']/field[@name='name']"
		[Register ("name")]
		public string Name {
			get {
				if (name_jfieldId == IntPtr.Zero)
					name_jfieldId = JNIEnv.GetFieldID (class_ref, "name", "Ljava/lang/String;");
				IntPtr __ret = JNIEnv.GetObjectField (((global::Java.Lang.Object) this).Handle, name_jfieldId);
				return JNIEnv.GetString (__ret, JniHandleOwnership.TransferLocalRef);
			}
			set {
				if (name_jfieldId == IntPtr.Zero)
					name_jfieldId = JNIEnv.GetFieldID (class_ref, "name", "Ljava/lang/String;");
				IntPtr native_value = JNIEnv.NewString (value);
				try {
					JNIEnv.SetField (((global::Java.Lang.Object) this).Handle, name_jfieldId, native_value);
				} finally {
					JNIEnv.DeleteLocalRef (native_value);
				}
			}
		}
		internal static new IntPtr java_class_handle;
		internal static new IntPtr class_ref {
			get {
				return JNIEnv.FindClass ("xamarin/test/FieldMethodNameColisionClass", ref java_class_handle);
			}
		}

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return typeof (FieldMethodNameColisionClass); }
		}

		protected FieldMethodNameColisionClass (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		static Delegate cb_getNumber;
#pragma warning disable 0169
		static Delegate GetGetNumberHandler ()
		{
			if (cb_getNumber == null)
				cb_getNumber = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, int>) n_GetNumber);
			return cb_getNumber;
		}

		static int n_GetNumber (IntPtr jnienv, IntPtr native__this)
		{
			global::Xamarin.Test.FieldMethodNameColisionClass __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.FieldMethodNameColisionClass> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.Number;
		}
#pragma warning restore 0169

		static Delegate cb_setNumber_I;
#pragma warning disable 0169
		static Delegate GetSetNumber_IHandler ()
		{
			if (cb_setNumber_I == null)
				cb_setNumber_I = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, int>) n_SetNumber_I);
			return cb_setNumber_I;
		}

		static void n_SetNumber_I (IntPtr jnienv, IntPtr native__this, int item)
		{
			global::Xamarin.Test.FieldMethodNameColisionClass __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.FieldMethodNameColisionClass> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.Number = item;
		}
#pragma warning restore 0169

		static IntPtr id_getNumber;
		static IntPtr id_setNumber_I;
		public virtual unsafe int Number {
			// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='FieldMethodNameColisionClass']/method[@name='getNumber' and count(parameter)=0]"
			[Register ("getNumber", "()I", "GetGetNumberHandler")]
			get {
				if (id_getNumber == IntPtr.Zero)
					id_getNumber = JNIEnv.GetMethodID (class_ref, "getNumber", "()I");
				try {

					if (((object) this).GetType () == ThresholdType)
						return JNIEnv.CallIntMethod (((global::Java.Lang.Object) this).Handle, id_getNumber);
					else
						return JNIEnv.CallNonvirtualIntMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "getNumber", "()I"));
				} finally {
				}
			}
			// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='FieldMethodNameColisionClass']/method[@name='setNumber' and count(parameter)=1 and parameter[1][@type='int']]"
			[Register ("setNumber", "(I)V", "GetSetNumber_IHandler")]
			set {
				if (id_setNumber_I == IntPtr.Zero)
					id_setNumber_I = JNIEnv.GetMethodID (class_ref, "setNumber", "(I)V");
				try {
					JValue* __args = stackalloc JValue [1];
					__args [0] = new JValue (value);

					if (((object) this).GetType () == ThresholdType)
						JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_setNumber_I, __args);
					else
						JNIEnv.CallNonvirtualVoidMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "setNumber", "(I)V"), __args);
				} finally {
				}
			}
		}

		static Delegate cb_getYourAge;
#pragma warning disable 0169
		static Delegate GetGetYourAgeHandler ()
		{
			if (cb_getYourAge == null)
				cb_getYourAge = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, int>) n_GetYourAge);
			return cb_getYourAge;
		}

		static int n_GetYourAge (IntPtr jnienv, IntPtr native__this)
		{
			global::Xamarin.Test.FieldMethodNameColisionClass __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.FieldMethodNameColisionClass> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.YourAge;
		}
#pragma warning restore 0169

		static IntPtr id_getYourAge;
		public virtual unsafe int YourAge {
			// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='FieldMethodNameColisionClass']/method[@name='getYourAge' and count(parameter)=0]"
			[Register ("getYourAge", "()I", "GetGetYourAgeHandler")]
			get {
				if (id_getYourAge == IntPtr.Zero)
					id_getYourAge = JNIEnv.GetMethodID (class_ref, "getYourAge", "()I");
				try {

					if (((object) this).GetType () == ThresholdType)
						return JNIEnv.CallIntMethod (((global::Java.Lang.Object) this).Handle, id_getYourAge);
					else
						return JNIEnv.CallNonvirtualIntMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "getYourAge", "()I"));
				} finally {
				}
			}
		}

		static Delegate cb_setAge_I;
#pragma warning disable 0169
		static Delegate GetSetAge_IHandler ()
		{
			if (cb_setAge_I == null)
				cb_setAge_I = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, int>) n_SetAge_I);
			return cb_setAge_I;
		}

		static void n_SetAge_I (IntPtr jnienv, IntPtr native__this, int age)
		{
			global::Xamarin.Test.FieldMethodNameColisionClass __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.FieldMethodNameColisionClass> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.SetAge (age);
		}
#pragma warning restore 0169

		static IntPtr id_setAge_I;
		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='FieldMethodNameColisionClass']/method[@name='setAge' and count(parameter)=1 and parameter[1][@type='int']]"
		[Register ("setAge", "(I)V", "GetSetAge_IHandler")]
		public virtual unsafe void SetAge (int age)
		{
			if (id_setAge_I == IntPtr.Zero)
				id_setAge_I = JNIEnv.GetMethodID (class_ref, "setAge", "(I)V");
			try {
				JValue* __args = stackalloc JValue [1];
				__args [0] = new JValue (age);

				if (((object) this).GetType () == ThresholdType)
					JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_setAge_I, __args);
				else
					JNIEnv.CallNonvirtualVoidMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "setAge", "(I)V"), __args);
			} finally {
			}
		}

		static Delegate cb_getAbc;
#pragma warning disable 0169
		static Delegate GetGetAbcHandler ()
		{
			if (cb_getAbc == null)
				cb_getAbc = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, bool>) n_GetAbc);
			return cb_getAbc;
		}

		static bool n_GetAbc (IntPtr jnienv, IntPtr native__this)
		{
			global::Xamarin.Test.FieldMethodNameColisionClass __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.FieldMethodNameColisionClass> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.GetAbc ();
		}
#pragma warning restore 0169

		static IntPtr id_getAbc;
		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='FieldMethodNameColisionClass']/method[@name='getAbc' and count(parameter)=0]"
		[Register ("getAbc", "()Z", "GetGetAbcHandler")]
		public virtual unsafe bool GetAbc ()
		{
			if (id_getAbc == IntPtr.Zero)
				id_getAbc = JNIEnv.GetMethodID (class_ref, "getAbc", "()Z");
			try {

				if (((object) this).GetType () == ThresholdType)
					return JNIEnv.CallBooleanMethod (((global::Java.Lang.Object) this).Handle, id_getAbc);
				else
					return JNIEnv.CallNonvirtualBooleanMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "getAbc", "()Z"));
			} finally {
			}
		}

		static Delegate cb_getItem;
#pragma warning disable 0169
		static Delegate GetGetItemHandler ()
		{
			if (cb_getItem == null)
				cb_getItem = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, int>) n_GetItem);
			return cb_getItem;
		}

		static int n_GetItem (IntPtr jnienv, IntPtr native__this)
		{
			global::Xamarin.Test.FieldMethodNameColisionClass __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.FieldMethodNameColisionClass> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.GetItem ();
		}
#pragma warning restore 0169

		static IntPtr id_getItem;
		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='FieldMethodNameColisionClass']/method[@name='getItem' and count(parameter)=0]"
		[Register ("getItem", "()I", "GetGetItemHandler")]
		public virtual unsafe int GetItem ()
		{
			if (id_getItem == IntPtr.Zero)
				id_getItem = JNIEnv.GetMethodID (class_ref, "getItem", "()I");
			try {

				if (((object) this).GetType () == ThresholdType)
					return JNIEnv.CallIntMethod (((global::Java.Lang.Object) this).Handle, id_getItem);
				else
					return JNIEnv.CallNonvirtualIntMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "getItem", "()I"));
			} finally {
			}
		}

		static Delegate cb_setItem_I;
#pragma warning disable 0169
		static Delegate GetSetItem_IHandler ()
		{
			if (cb_setItem_I == null)
				cb_setItem_I = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, int>) n_SetItem_I);
			return cb_setItem_I;
		}

		static void n_SetItem_I (IntPtr jnienv, IntPtr native__this, int item)
		{
			global::Xamarin.Test.FieldMethodNameColisionClass __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.FieldMethodNameColisionClass> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.SetItem (item);
		}
#pragma warning restore 0169

		static IntPtr id_setItem_I;
		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='FieldMethodNameColisionClass']/method[@name='setItem' and count(parameter)=1 and parameter[1][@type='int']]"
		[Register ("setItem", "(I)V", "GetSetItem_IHandler")]
		public virtual unsafe void SetItem (int item)
		{
			if (id_setItem_I == IntPtr.Zero)
				id_setItem_I = JNIEnv.GetMethodID (class_ref, "setItem", "(I)V");
			try {
				JValue* __args = stackalloc JValue [1];
				__args [0] = new JValue (item);

				if (((object) this).GetType () == ThresholdType)
					JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_setItem_I, __args);
				else
					JNIEnv.CallNonvirtualVoidMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "setItem", "(I)V"), __args);
			} finally {
			}
		}

		static Delegate cb_getName;
#pragma warning disable 0169
		static Delegate GetGetNameHandler ()
		{
			if (cb_getName == null)
				cb_getName = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, int>) n_GetName);
			return cb_getName;
		}

		static int n_GetName (IntPtr jnienv, IntPtr native__this)
		{
			global::Xamarin.Test.FieldMethodNameColisionClass __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.FieldMethodNameColisionClass> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.GetName ();
		}
#pragma warning restore 0169

		static IntPtr id_getName;
		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='FieldMethodNameColisionClass']/method[@name='getName' and count(parameter)=0]"
		[Register ("getName", "()I", "GetGetNameHandler")]
		public virtual unsafe int GetName ()
		{
			if (id_getName == IntPtr.Zero)
				id_getName = JNIEnv.GetMethodID (class_ref, "getName", "()I");
			try {

				if (((object) this).GetType () == ThresholdType)
					return JNIEnv.CallIntMethod (((global::Java.Lang.Object) this).Handle, id_getName);
				else
					return JNIEnv.CallNonvirtualIntMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "getName", "()I"));
			} finally {
			}
		}

	}
}
