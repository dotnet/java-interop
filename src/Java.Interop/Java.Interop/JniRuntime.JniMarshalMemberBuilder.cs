#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Java.Interop {

	partial class JniRuntime {

		partial class CreationOptions {
			public  bool                       UseMarshalMemberBuilder     {get; set;}      = true;
			public  JniMarshalMemberBuilder?   MarshalMemberBuilder        {get; set;}
		}

		JniMarshalMemberBuilder?               marshalMemberBuilder;
		public  JniMarshalMemberBuilder        MarshalMemberBuilder        {
			get {
				if (marshalMemberBuilder == null)
					throw new NotSupportedException ("JniRuntime.ExportedMemberBuilder is not supported.");
				return marshalMemberBuilder;
			}
		}

		internal    bool                       UseMarshalMemberBuilder  => marshalMemberBuilder != null;

		const string NotUsedInAndroid = "This code path is not used in Android projects.";

		// FIXME: https://github.com/dotnet/java-interop/issues/1192
		[DynamicDependency (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor, "Java.Interop.MarshalMemberBuilder", "Java.Interop.Export")]
		[UnconditionalSuppressMessage ("Trimming", "IL2026", Justification = NotUsedInAndroid)]
		[UnconditionalSuppressMessage ("Trimming", "IL2035", Justification = "Java.Interop.Export.dll is not always present.")]
		[UnconditionalSuppressMessage ("Trimming", "IL3050", Justification = NotUsedInAndroid)]
		partial void SetMarshalMemberBuilder (CreationOptions options)
		{
			if (!options.UseMarshalMemberBuilder) {
				return;
			}

			if (options.MarshalMemberBuilder != null) {
				marshalMemberBuilder    = SetRuntime (options.MarshalMemberBuilder);
				return;
			}

			var t   = Type.GetType ("Java.Interop.MarshalMemberBuilder, Java.Interop.Export", throwOnError: false);
			if (t == null)
				throw new InvalidOperationException ("Could not find Java.Interop.MarshalMemberBuilder from Java.Interop.Export.dll!");
			var b   = (JniMarshalMemberBuilder) Activator.CreateInstance (t)!;
			marshalMemberBuilder    = SetRuntime (b);
		}

		public abstract class JniMarshalMemberBuilder : IDisposable, ISetRuntime
		{
			JniRuntime?             runtime;
			bool                    disposed;

			public JniRuntime  Runtime     {
				get => runtime ?? throw new NotSupportedException ();
			}

			protected JniMarshalMemberBuilder ()
			{
			}

			public virtual void OnSetRuntime (JniRuntime runtime)
			{
				if (disposed)
					throw new ObjectDisposedException (GetType ().Name);

				this.runtime = runtime;
			}

			public void Dispose ()
			{
				Dispose (false);
			}

			protected virtual void Dispose (bool disposing)
			{
				disposed = true;
			}

			public  Delegate                                                CreateMarshalToManagedDelegate (Delegate value)
			{
				if (value == null)
					throw new ArgumentNullException (nameof (value));
				return CreateMarshalToManagedExpression (value.GetMethodInfo ()!).Compile ();
			}

			public  abstract    LambdaExpression                            CreateMarshalToManagedExpression (MethodInfo method);
			public  abstract    IEnumerable<JniNativeMethodRegistration>    GetExportedMemberRegistrations (Type declaringType);

			public  abstract    Expression<Func<ConstructorInfo, JniObjectReference, object?[]?, object>>   CreateConstructActivationPeerExpression (ConstructorInfo constructor);

			public  Func<ConstructorInfo, JniObjectReference, object?[]?, object>                           CreateConstructActivationPeerFunc (ConstructorInfo constructor)
			{
				if (constructor == null)
					throw new ArgumentNullException (nameof (constructor));

				var e   = CreateConstructActivationPeerExpression (constructor);
				return e.Compile ();
			}

			public string GetJniMethodSignature (MethodBase member)
			{
				if (disposed)
					throw new ObjectDisposedException (GetType ().Name);

				if (member == null)
					throw new ArgumentNullException (nameof (member));

				var signature           = new StringBuilder ().Append ("(");
				var memberParameters    = member.GetParameters ();
				foreach (var p in IsDirectMethod (memberParameters) ? memberParameters.Skip (2) : memberParameters) {
					signature.Append (GetTypeSignature (p));
				}
				signature.Append (")");

				var method  = member as MethodInfo;
				if (method != null) {
					signature.Append (GetTypeSignature (method.ReturnParameter));
				} else {
					signature.Append ("V");
				}

				return signature.ToString ();
			}

			string GetTypeSignature (ParameterInfo p)
			{
				var info        = Runtime.TypeManager.GetTypeSignature (p.ParameterType);
				if (info.IsValid)
					return info.QualifiedReference;

				var marshaler   = GetParameterMarshaler (p);
				info            = Runtime.TypeManager.GetTypeSignature (marshaler.MarshalType);
				if (info.IsValid)
					return info.QualifiedReference;

				throw new NotSupportedException ("Don't know how to determine JNI signature for parameter type: " + p.ParameterType.FullName + ".");
			}

			public JniValueMarshaler GetParameterMarshaler (ParameterInfo parameter)
			{
				if (parameter.ParameterType == typeof (IntPtr))
					return IntPtrValueMarshaler.Instance;

				JniValueMarshalerAttribute? attr;
				try {
					attr = parameter.GetCustomAttribute<JniValueMarshalerAttribute> ();
				} catch (System.IndexOutOfRangeException) {
					attr = null;
				}
				if (attr != null) {
					return (JniValueMarshaler) Activator.CreateInstance (attr.MarshalerType)!;
				}
				return Runtime.ValueManager.GetValueMarshaler (parameter.ParameterType);
			}

			// Heuristic: if first two parameters are IntPtr, this is a "direct" wrapper.
			public bool IsDirectMethod (ParameterInfo[] methodParameters)
			{
				return methodParameters?.Length >= 2 &&
					methodParameters [0].ParameterType == typeof (IntPtr) &&
					methodParameters [1].ParameterType == typeof (IntPtr);
			}
		}
	}

	sealed class IntPtrValueMarshaler : JniValueMarshaler<IntPtr> {
		internal    static  IntPtrValueMarshaler Instance = new IntPtrValueMarshaler ();

		[RequiresUnreferencedCode (ExpressionRequiresUnreferencedCode)]
		public override Expression CreateParameterFromManagedExpression (Java.Interop.Expressions.JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize)
		{
			return sourceValue;
		}

		[RequiresDynamicCode (ExpressionRequiresUnreferencedCode)]
		[RequiresUnreferencedCode (ExpressionRequiresUnreferencedCode)]
		public override Expression CreateParameterToManagedExpression (Java.Interop.Expressions.JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize, Type? targetType)
		{
			return sourceValue;
		}

		[RequiresDynamicCode (ExpressionRequiresUnreferencedCode)]
		[RequiresUnreferencedCode (ExpressionRequiresUnreferencedCode)]
		public override Expression CreateReturnValueFromManagedExpression (Java.Interop.Expressions.JniValueMarshalerContext context, ParameterExpression sourceValue)
		{
			return sourceValue;
		}


		public override object? CreateValue (
				ref JniObjectReference reference,
				JniObjectReferenceOptions options,
				[DynamicallyAccessedMembers (Constructors)]
				Type? targetType)
		{
			throw new NotSupportedException ();
		}

		public override IntPtr CreateGenericValue (
				ref JniObjectReference reference,
				JniObjectReferenceOptions options,
				[DynamicallyAccessedMembers (Constructors)]
				Type? targetType)
		{
			throw new NotSupportedException ();
		}

		public override JniValueMarshalerState CreateArgumentState (object? value, ParameterAttributes synchronize)
		{
			throw new NotSupportedException ();
		}

		public override JniValueMarshalerState CreateGenericArgumentState ([MaybeNull]IntPtr value, ParameterAttributes synchronize)
		{
			throw new NotSupportedException ();
		}

		public override JniValueMarshalerState CreateObjectReferenceArgumentState (object? value, ParameterAttributes synchronize)
		{
			throw new NotSupportedException ();
		}

		public override JniValueMarshalerState CreateGenericObjectReferenceArgumentState ([MaybeNull]IntPtr value, ParameterAttributes synchronize)
		{
			throw new NotSupportedException ();
		}

		public override void DestroyArgumentState (object? value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			throw new NotSupportedException ();
		}

		public override void DestroyGenericArgumentState (IntPtr value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			throw new NotSupportedException ();
		}
	}
}

