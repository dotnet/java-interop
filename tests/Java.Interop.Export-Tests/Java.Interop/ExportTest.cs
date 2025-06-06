using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;

using Java.Interop;
using Java.Interop.Expressions;

// For use by `jnimarshalmethod-gen.exe` & `make run-test-jnimarshal`
delegate bool _JniMarshal_PPZBCSIJFDLLLLLDFJ_Z (
		IntPtr              jnienv,
		IntPtr              klass,
		bool                a,
		sbyte               b,
		char                c,
		short               d,
		int                 e,
		long                f,
		float               g,
		double              h,
		IntPtr              i,  // java.lang.Object
		IntPtr              j,  // java.lang.String
		IntPtr              k,  // java.util.ArrayList<String>
		IntPtr              l,  // java.lang.String
		IntPtr              m,  // java.lang.Object
		double              n,
		float               o,
		long                p
);

namespace Java.InteropTests
{
	[JniTypeSignature (JniTypeName, GenerateJavaPeer=false)]
	public class ExportTest : JavaObject
	{
		internal const string JniTypeName = "net/dot/jni/test/ExportType";

		[JniAddNativeMethodRegistrationAttribute]
		static void RegisterNativeMembers (JniNativeMethodRegistrationArguments args)
		{
			args.AddRegistrations (JniEnvironment.Runtime.MarshalMemberBuilder.GetExportedMemberRegistrations (typeof (ExportTest)));
		}

		public ExportTest (ref JniObjectReference reference, JniObjectReferenceOptions transfer)
			: base (ref reference, transfer)
		{
		}

		public bool HelloCalled;

		[JavaCallable ("action", Signature="()V")]
		public void InstanceAction ()
		{
			HelloCalled = true;
		}

		public static bool StaticHelloCalled;

		[JavaCallable ("staticAction", Signature="()V")]
		public static void StaticAction ()
		{
			StaticHelloCalled = true;
		}

		public static bool StaticActionInt32StringCalled;

		[JavaCallable ("staticActionInt32String", Signature = "(ILjava/lang/String;)V")]
		public static void StaticActionInt32String (int i, string v)
		{
			StaticActionInt32StringCalled = i == 1 && v == "2";
		}

		[JavaCallable ("staticFuncMyLegacyColorMyColor_MyColor")]
		public static MyColor StaticFuncMyLegacyColorMyColor_MyColor ([JniValueMarshaler (typeof (MyLegacyColorValueMarshaler))] MyLegacyColor color1, MyColor color2)
		{
			return new MyColor (color1.Value + color2.Value);
		}

		[JavaCallable ("funcInt64", Signature = "()J")]
		public long FuncInt64 ()
		{
			return 42;
		}

		[JavaCallable ("funcIJavaObject", Signature = "()Ljava/lang/Object;")]
		public JavaObject FuncIJavaObject ()
		{
			return this;
		}

		[JavaCallable ("actionIJavaObject", Signature="(Ljava/lang/Object;)V")]
		public void InstanceActionIJavaObject (JavaObject test)
		{
		}

		[JavaCallable ("staticActionIJavaObject", Signature="(Ljava/lang/Object;)V")]
		public static void StaticActionIJavaObject (JavaObject test)
		{
		}

		[JavaCallable ("staticActionInt", Signature="(I)V")]
		public static void StaticActionInt (int i)
		{
		}

		[JavaCallable ("staticActionFloat", Signature="(F)V")]
		public static void StaticActionFloat (float f)
		{
		}

		// TODO: [JavaCallable ("staticActionNullableFloat", Signature="(Ljava/lang/Float;)V")]
		public static void StaticActionNullableFloat (float? f)
		{
		}

		[JavaCallable ("staticFuncThisMethodTakesLotsOfParameters", Signature="(ZBCSIJFDLjava/lang/Object;Ljava/lang/String;Ljava/util/ArrayList;Ljava/lang/String;Ljava/lang/Object;DFJ)Z")]
		public static bool StaticFuncThisMethodTakesLotsOfParameters (
				bool                a,
				sbyte               b,
				char                c,
				short               d,
				int                 e,
				long                f,
				float               g,
				double              h,
				IntPtr              i,  // java.lang.Object
				IntPtr              j,  // java.lang.String
				IntPtr              k,  // java.util.ArrayList<String>
				IntPtr              l,  // java.lang.String
				IntPtr              m,  // java.lang.Object
				double              n,
				float               o,
				long                p)
		{
			if (a != false)
				return false;
			if (b != (byte) 0xb)
				return false;
			if (c != 'c')
				return false;
			if (d != (short) 0xd)
				return false;
			if (e != 0xe)
				return false;
			if (f != 0xf)
				return false;
			if (g != 1.0f)
				return false;
			if (h != 2.0)
				return false;
			if (i == IntPtr.Zero)
				return false;
			if (j == IntPtr.Zero)
				return false;
			if (k == IntPtr.Zero)
				return false;
			if (l == IntPtr.Zero)
				return false;
			if (m == IntPtr.Zero)
				return false;
			if (n != 3.0)
				return false;
			if (o != 4.0f)
				return false;
			if (p != 0x70)
				return false;
			return true;
		}
	}

	[JniValueMarshaler (typeof (MyColorValueMarshaler))]
	public struct MyColor {

		public readonly int Value;

		public MyColor (int value)
		{
			Value = value;
		}
	}

	// Note: no [JniValueMarshaler] type; we use a parameter custom attribute instead.
	public struct MyLegacyColor {

		public readonly int Value;

		public MyLegacyColor (int value)
		{
			Value = value;
		}
	}

	public interface MyInterface : IJavaPeerable
	{
		void MyMethod ();
	}

	public class MyColorValueMarshaler : JniValueMarshaler<MyColor> {

		public override Type MarshalType {
			get {return typeof (int);}
		}

		public override MyColor CreateGenericValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type targetType)
		{
			throw new NotImplementedException ();
		}

		public override JniValueMarshalerState CreateGenericObjectReferenceArgumentState (MyColor value, ParameterAttributes synchronize)
		{
			throw new NotImplementedException ();
		}

		public override void DestroyGenericArgumentState (MyColor value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			throw new NotImplementedException ();
		}

		public override Expression CreateParameterToManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize, Type targetType)
		{
			var c = typeof (MyColor).GetConstructor (new[]{typeof (int)});
			var v = Expression.Variable (typeof (MyColor), sourceValue.Name + "_val");
			context.LocalVariables.Add (v);
			context.CreationStatements.Add (Expression.Assign (v, Expression.New (c, sourceValue)));
			return v;
		}

		public override Expression CreateParameterFromManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize)
		{
			var r = Expression.Variable (MarshalType, sourceValue.Name + "_p");
			context.LocalVariables.Add (r);
			context.CreationStatements.Add (Expression.Assign (r, Expression.Field (sourceValue, "Value")));
			return r;
		}

		public override Expression CreateReturnValueFromManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue)
		{
			return CreateParameterFromManagedExpression (context, sourceValue, 0);
		}
	}

	public class MyLegacyColorValueMarshaler : JniValueMarshaler<MyLegacyColor> {

		public override Type MarshalType {
			get {return typeof (int);}
		}

		public override MyLegacyColor CreateGenericValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type targetType)
		{
			throw new NotImplementedException ();
		}

		public override JniValueMarshalerState CreateGenericObjectReferenceArgumentState (MyLegacyColor value, ParameterAttributes synchronize)
		{
			throw new NotImplementedException ();
		}

		public override void DestroyGenericArgumentState (MyLegacyColor value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			throw new NotImplementedException ();
		}

		public override Expression CreateParameterToManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize, Type targetType)
		{
			var c = typeof (MyLegacyColor).GetConstructor (new[]{typeof (int)});
			var v = Expression.Variable (typeof (MyLegacyColor), sourceValue.Name + "_val");
			context.LocalVariables.Add (v);
			context.CreationStatements.Add (Expression.Assign (v, Expression.New (c, sourceValue)));
			return v;
		}

		public override Expression CreateParameterFromManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize)
		{
			var r = Expression.Variable (MarshalType, sourceValue.Name + "_p");
			context.LocalVariables.Add (r);
			context.CreationStatements.Add (Expression.Assign (r, Expression.Field (sourceValue, "Value")));
			return r;
		}

		public override Expression CreateReturnValueFromManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue)
		{
			return CreateParameterFromManagedExpression (context, sourceValue, 0);
		}
	}
}

