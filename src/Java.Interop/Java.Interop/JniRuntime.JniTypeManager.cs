#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Threading;

namespace Java.Interop {

	public partial class JniRuntime {

#if NET
		[SuppressMessage ("Design", "CA1034:Nested types should not be visible",
			Justification = "Deliberate choice to 'hide' these types from code completion for `Java.Interop.`; see 045b8af7.")]
		public struct ReplacementMethodInfo : IEquatable<ReplacementMethodInfo>
		{
			public  string? SourceJniType                   {get; set;}
			public  string? SourceJniMethodName             {get; set;}
			public  string? SourceJniMethodSignature        {get; set;}
			public  string? TargetJniType                   {get; set;}
			public  string? TargetJniMethodName             {get; set;}
			public  string? TargetJniMethodSignature        {get; set;}
			public  int?    TargetJniMethodParameterCount   {get; set;}
			public  bool    TargetJniMethodInstanceToStatic {get; set;}

			public override bool Equals (object? obj)
			{
				if (obj is ReplacementMethodInfo o) {
					return Equals (o);
				}
				return false;
			}

			public bool Equals (ReplacementMethodInfo other)
			{
				return string.Equals (SourceJniType, other.SourceJniType) &&
					string.Equals (SourceJniMethodName, other.SourceJniMethodName) &&
					string.Equals (SourceJniMethodSignature, other.SourceJniMethodSignature) &&
					string.Equals (TargetJniType, other.TargetJniType) &&
					string.Equals (TargetJniMethodName, other.TargetJniMethodName) &&
					string.Equals (TargetJniMethodSignature, other.TargetJniMethodSignature) &&
					TargetJniMethodParameterCount == other.TargetJniMethodParameterCount &&
					TargetJniMethodInstanceToStatic == other.TargetJniMethodInstanceToStatic;
			}

			public override int GetHashCode ()
			{
				return (SourceJniType?.GetHashCode () ?? 0) ^
					(SourceJniMethodName?.GetHashCode () ?? 0) ^
					(SourceJniMethodSignature?.GetHashCode () ?? 0) ^
					(TargetJniType?.GetHashCode () ?? 0) ^
					(TargetJniMethodName?.GetHashCode () ?? 0) ^
					(TargetJniMethodSignature?.GetHashCode () ?? 0) ^
					(TargetJniMethodParameterCount?.GetHashCode () ?? 0) ^
					TargetJniMethodInstanceToStatic.GetHashCode ();
			}

			public override string ToString ()
			{
				return $"{nameof (ReplacementMethodInfo)} {{ " +
					$"{nameof (SourceJniType)} = \"{SourceJniType}\"" +
					$", {nameof (SourceJniMethodName)} = \"{SourceJniMethodName}\"" +
					$", {nameof (SourceJniMethodSignature)} = \"{SourceJniMethodSignature}\"" +
					$", {nameof (TargetJniType)} = \"{TargetJniType}\"" +
					$", {nameof (TargetJniMethodName)} = \"{TargetJniMethodName}\"" +
					$", {nameof (TargetJniMethodSignature)} = \"{TargetJniMethodSignature}\"" +
					$", {nameof (TargetJniMethodParameterCount)} = {TargetJniMethodParameterCount?.ToString () ?? "null"}" +
					$", {nameof (TargetJniMethodInstanceToStatic)} = {TargetJniMethodInstanceToStatic}" +
					$"}}";
			}

			public static bool operator==(ReplacementMethodInfo a, ReplacementMethodInfo b) => a.Equals (b);
			public static bool operator!=(ReplacementMethodInfo a, ReplacementMethodInfo b) => !a.Equals (b);
		}
#endif  // NET

		/// <include file="../Documentation/Java.Interop/JniRuntime.JniTypeManager.xml" path="/docs/member[@name='T:JniTypeManager']/*" />
		public abstract partial class JniTypeManager : IDisposable, ISetRuntime {

			internal const DynamicallyAccessedMemberTypes Constructors = DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors;
			internal const DynamicallyAccessedMemberTypes Methods = DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods;
			internal const DynamicallyAccessedMemberTypes MethodsAndPrivateNested = Methods | DynamicallyAccessedMemberTypes.NonPublicNestedTypes;
			internal const DynamicallyAccessedMemberTypes MethodsConstructors = MethodsAndPrivateNested | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors;

			JniRuntime?             runtime;
			bool                    disposed;


			public      JniRuntime  Runtime {
				get => runtime ?? throw new NotSupportedException ();
			}

			public virtual void OnSetRuntime (JniRuntime runtime)
			{
				AssertValid ();
				this.runtime = runtime;
			}

			public void Dispose ()
			{
				Dispose (false);
			}

			protected virtual void Dispose (bool disposing)
			{
				disposed    = true;
			}

			[MethodImpl (MethodImplOptions.AggressiveInlining)]
			private protected void AssertValid ()
			{
				if (!disposed)
					return;
				throw new ObjectDisposedException (nameof (JniTypeManager));
			}

			internal static void AssertSimpleReference (string jniSimpleReference, string argumentName = "jniSimpleReference")
			{
				if (string.IsNullOrEmpty (jniSimpleReference))
					throw new ArgumentNullException (argumentName);
				if (jniSimpleReference.IndexOf ('.') >= 0)
					throw new ArgumentException ("JNI type names do not contain '.', they use '/'. Are you sure you're using a JNI type name?", argumentName);
				switch (jniSimpleReference [0]) {
					case '[':
						throw new ArgumentException ("Arrays cannot be present in simplified type references.", argumentName);
					case 'L':
						if (jniSimpleReference [jniSimpleReference.Length - 1] == ';')
							throw new ArgumentException ("JNI type references are not supported.", argumentName);
						break;
					default:
						break;
				}
			}

			// NOTE: This method needs to be kept in sync with GetTypeSignatures()
			// This version of the method has removed IEnumerable for performance reasons.
			public JniTypeSignature GetTypeSignature (Type type)
			{
				AssertValid ();

				if (type == null)
 					throw new ArgumentNullException (nameof (type));

				return GetTypeSignatureCore (type);
			}

			protected abstract JniTypeSignature GetTypeSignatureCore (Type type);

			// NOTE: This method needs to be kept in sync with GetTypeSignature()
			public IEnumerable<JniTypeSignature> GetTypeSignatures (Type type)
			{
				AssertValid ();

				if (type == null)
					return [];

				return GetTypeSignaturesCore (type);
			}

			protected abstract IEnumerable<JniTypeSignature> GetTypeSignaturesCore (Type type);

			[return: DynamicallyAccessedMembers (MethodsConstructors)]
			public  Type?    GetType (JniTypeSignature typeSignature)
			{
				AssertValid ();

				if (!typeSignature.IsValid || typeSignature.SimpleReference == null)
					return null;

				var type = GetTypeForSimpleReference (typeSignature.SimpleReference);
				if (type == null)
					return null;
				if (typeSignature.ArrayRank == 0)
					return type;
				throw new NotSupportedException ($"DAM-annotated type lookup for array signature `{typeSignature}` is not supported. Use {nameof (GetTypes)} instead.");
			}

			protected abstract string? GetSimpleReference (Type type);
			protected abstract IEnumerable<string> GetSimpleReferences (Type type);
			[return: DynamicallyAccessedMembers (MethodsConstructors)]
			protected abstract Type? GetTypeForSimpleReference (string jniSimpleReference);
			public abstract IEnumerable<Type> GetTypes (JniTypeSignature typeSignature);

			public abstract IEnumerable<ReflectionConstructibleType> GetReflectionConstructibleTypes (JniTypeSignature typeSignature);

			public class ReflectionConstructibleType
			{
				public ReflectionConstructibleType (
						[DynamicallyAccessedMembers (Constructors)]
						Type type)
				{
					Type = type;
				}

				[DynamicallyAccessedMembers (Constructors)]
				public Type Type { get; }
			}

			protected abstract IEnumerable<Type> GetTypesForSimpleReference (string jniSimpleReference);

			/// <include file="../Documentation/Java.Interop/JniRuntime.JniTypeManager.xml" path="/docs/member[@name='M:GetInvokerType']/*" />
			[return: DynamicallyAccessedMembers (Constructors)]
			public Type? GetInvokerType (
					[DynamicallyAccessedMembers (Constructors)]
					Type type)
			{
				if (type.IsAbstract || type.IsInterface) {
					return GetInvokerTypeCore (type);
				}
				return null;
			}
			
			[return: DynamicallyAccessedMembers (Constructors)]
			protected abstract Type? GetInvokerTypeCore ([DynamicallyAccessedMembers (Constructors)] Type type);
#if NET
			protected abstract IReadOnlyList<string>? GetStaticMethodFallbackTypesCore (string jniSimple);

			public string? GetReplacementType (string jniSimpleReference)
			{
				AssertValid ();
				AssertSimpleReference (jniSimpleReference, nameof (jniSimpleReference));

				return GetReplacementTypeCore (jniSimpleReference);
			}

			protected abstract string? GetReplacementTypeCore (string jniSimpleReference);

			public IReadOnlyList<string>? GetStaticMethodFallbackTypes (string jniSimpleReference)
			{
				AssertValid ();
				AssertSimpleReference (jniSimpleReference, nameof (jniSimpleReference));

				return GetStaticMethodFallbackTypesCore (jniSimpleReference);
			}

			public ReplacementMethodInfo? GetReplacementMethodInfo (string jniSimpleReference, string jniMethodName, string jniMethodSignature)
			{
				AssertValid ();
				AssertSimpleReference (jniSimpleReference, nameof (jniSimpleReference));
				if (string.IsNullOrEmpty (jniMethodName)) {
					throw new ArgumentNullException (nameof (jniMethodName));
				}
				if (string.IsNullOrEmpty (jniMethodSignature)) {
					throw new ArgumentNullException (nameof (jniMethodSignature));
				}

				return GetReplacementMethodInfoCore (jniSimpleReference, jniMethodName, jniMethodSignature);
			}

			protected abstract ReplacementMethodInfo? GetReplacementMethodInfoCore (string jniSimpleReference, string jniMethodName, string jniMethodSignature);

			public abstract void RegisterNativeMembers (
					JniType nativeClass,
					[DynamicallyAccessedMembers (MethodsAndPrivateNested)]
					Type type,
					ReadOnlySpan<char> methods);
#endif
#if NET
			[Obsolete ("Use RegisterNativeMembers(JniType, Type, ReadOnlySpan<char>)")]
#endif  // NET
			public abstract void RegisterNativeMembers (
					JniType nativeClass,
					[DynamicallyAccessedMembers (MethodsAndPrivateNested)]
					Type type,
					string? methods);
		}

		[RequiresUnreferencedCode ("The default JniTypeManager implementation does not support trimming. Use a custom JniTypeManager implementation that supports trimming, or ensure that all types used with the default JniTypeManager are preserved.")]
		public partial class DynamicJniTypeManager : JniTypeManager {

			protected override JniTypeSignature GetTypeSignatureCore (Type type)
			{
				type = GetUnderlyingType (type, out int rank);

				JniTypeSignature signature = JniTypeSignature.Empty;
				if (GetBuiltInTypeSignature (type, ref signature))
					return signature.AddArrayRank (rank);
				if (GetBuiltInTypeArraySignature (type, ref signature))
					return signature.AddArrayRank (rank);

				var isGeneric = type.IsGenericType;
				var genericDef = isGeneric ? type.GetGenericTypeDefinition () : type;
				if (isGeneric) {
					if (genericDef == typeof (JavaArray<>) || genericDef == typeof (JavaObjectArray<>)) {
						var r = GetTypeSignature (type.GenericTypeArguments [0]);
						return r.AddArrayRank (rank + 1);
					}

					var genericSimpleRef = GetSimpleReference (genericDef);
					if (genericSimpleRef != null)
						return new JniTypeSignature (genericSimpleRef, rank, false);
				}

				var simpleRef = GetSimpleReference (type);
				if (simpleRef != null)
					return new JniTypeSignature (simpleRef, rank, false);

				return default;
			}

			protected override IEnumerable<JniTypeSignature> GetTypeSignaturesCore (Type type)
			{
				type = GetUnderlyingType (type, out int rank);

				var signature = JniTypeSignature.Empty;
				if (GetBuiltInTypeSignature (type, ref signature))
					yield return signature.AddArrayRank (rank);
				if (GetBuiltInTypeArraySignature (type, ref signature))
					yield return signature.AddArrayRank (rank);

				var isGeneric = type.IsGenericType;
				var genericDef = isGeneric ? type.GetGenericTypeDefinition () : type;
				if (isGeneric) {
					if (genericDef == typeof (JavaArray<>) || genericDef == typeof (JavaObjectArray<>)) {
						var r = GetTypeSignature (type.GenericTypeArguments [0]);
						yield return r.AddArrayRank (rank + 1);
					}

					foreach (var genericSimpleRef in GetSimpleReferences (genericDef)) {
						if (genericSimpleRef == null)
							continue;
						yield return new JniTypeSignature (genericSimpleRef, rank, false);
					}
				}

				foreach (var simpleRef in GetSimpleReferences (type)) {
					if (simpleRef == null)
						continue;
					yield return new JniTypeSignature (simpleRef, rank, false);
				}
			}

			static Type GetUnderlyingType (Type type, out int rank)
			{
				rank = 0;
				var originalType = type;
				while (type.IsArray) {
					if (type.IsArray && type.GetArrayRank () > 1)
						throw new ArgumentException ("Multidimensional array '" + originalType.FullName + "' is not supported.", nameof (type));
					rank++;
					type = type.GetElementType ()!;
				}

				if (type.IsEnum)
					type = Enum.GetUnderlyingType (type);

				return type;
			}

			// `type` will NOT be an array type.
			protected override string? GetSimpleReference (Type type)
			{
				return GetSimpleReferences (type).FirstOrDefault ();
			}

			// `type` will NOT be an array type.
			protected override IEnumerable<string> GetSimpleReferences (Type type)
			{
				AssertValid ();

				if (type == null)
					throw new ArgumentNullException (nameof (type));
				if (type.IsArray)
					throw new ArgumentException ("Array type '" + type.FullName + "' is not supported.", nameof (type));

				var name = type.GetCustomAttribute<JniTypeSignatureAttribute> (inherit: false);
				if (name != null) {
					var altRef = GetReplacementType (name.SimpleReference);
					if (altRef != null) {
						yield return altRef;
					} else {
						yield return name.SimpleReference;
					}
				}

				yield break;
			}

			static  readonly    string[]    EmptyStringArray    = Array.Empty<string> ();
			static  readonly    Type[]      EmptyTypeArray      = Array.Empty<Type> ();

			readonly struct KnownArrayTypesInfo
			{
				public readonly Dictionary<Type, Type> ArrayTypes;
				public readonly Dictionary<Type, Type> JavaObjectArrayTypes;

				public KnownArrayTypesInfo (Dictionary<Type, Type> arrayTypes, Dictionary<Type, Type> javaObjectArrayTypes)
				{
					ArrayTypes            = arrayTypes;
					JavaObjectArrayTypes = javaObjectArrayTypes;
				}
			}

			static readonly Lazy<KnownArrayTypesInfo> KnownArrayTypes = new Lazy<KnownArrayTypesInfo> (InitKnownArrayTypes);

			static KnownArrayTypesInfo InitKnownArrayTypes ()
			{
				var arrayTypes           = new Dictionary<Type, Type> ();
				var javaObjectArrayTypes = new Dictionary<Type, Type> ();

				AddKnownArrayTypes<string> (arrayTypes, javaObjectArrayTypes);

				AddKnownPrimitiveArrayTypes<Boolean, JavaBooleanArray> (arrayTypes, javaObjectArrayTypes);
				AddKnownPrimitiveArrayTypes<SByte, JavaSByteArray> (arrayTypes, javaObjectArrayTypes);
				AddKnownPrimitiveArrayTypes<Char, JavaCharArray> (arrayTypes, javaObjectArrayTypes);
				AddKnownPrimitiveArrayTypes<Int16, JavaInt16Array> (arrayTypes, javaObjectArrayTypes);
				AddKnownPrimitiveArrayTypes<Int32, JavaInt32Array> (arrayTypes, javaObjectArrayTypes);
				AddKnownPrimitiveArrayTypes<Int64, JavaInt64Array> (arrayTypes, javaObjectArrayTypes);
				AddKnownPrimitiveArrayTypes<Single, JavaSingleArray> (arrayTypes, javaObjectArrayTypes);
				AddKnownPrimitiveArrayTypes<Double, JavaDoubleArray> (arrayTypes, javaObjectArrayTypes);

				return new KnownArrayTypesInfo (arrayTypes, javaObjectArrayTypes);
			}

			static void AddKnownPrimitiveArrayTypes<
					[DynamicallyAccessedMembers (Constructors)]
					T,
					[DynamicallyAccessedMembers (Constructors)]
					TArray> (Dictionary<Type, Type> arrayTypes, Dictionary<Type, Type> javaObjectArrayTypes)
			{
				AddKnownArrayTypes<T> (arrayTypes, javaObjectArrayTypes);
				AddKnownArrayTypes<JavaArray<T>> (arrayTypes, javaObjectArrayTypes);
				AddKnownArrayTypes<JavaPrimitiveArray<T>> (arrayTypes, javaObjectArrayTypes);
				AddKnownArrayTypes<TArray> (arrayTypes, javaObjectArrayTypes);
			}

			static void AddKnownArrayTypes<
					[DynamicallyAccessedMembers (Constructors)]
					T> (Dictionary<Type, Type> arrayTypes, Dictionary<Type, Type> javaObjectArrayTypes)
			{
				arrayTypes [typeof (T)]                         = typeof (T[]);
				arrayTypes [typeof (T[])]                       = typeof (T[][]);
				arrayTypes [typeof (T[][])]                     = typeof (T[][][]);
				javaObjectArrayTypes [typeof (T)]               = typeof (JavaObjectArray<T>);
				javaObjectArrayTypes [typeof (JavaObjectArray<T>)] = typeof (JavaObjectArray<JavaObjectArray<T>>);
			}

			static bool TryMakeArrayType (Type type, out Type? arrayType) =>
				KnownArrayTypes.Value.ArrayTypes.TryGetValue (type, out arrayType);

			static bool TryMakeJavaObjectArrayType (Type type, out Type? arrayType) =>
				KnownArrayTypes.Value.JavaObjectArrayTypes.TryGetValue (type, out arrayType);

			static Type GetUnsupportedArrayType (Type type) =>
				throw new NotSupportedException ($"Array type construction for `{type}` is not supported.");

			static Type GetUnsupportedJavaObjectArrayType (Type type) =>
				throw new NotSupportedException ($"Generic Java array wrapper type construction for `{type}` is not supported.");

			[return: DynamicallyAccessedMembers (MethodsConstructors)]
			protected override Type? GetTypeForSimpleReference (string jniSimpleReference)
			{
				AssertValid ();
				AssertSimpleReference (jniSimpleReference);

				return jniSimpleReference switch {
					"java/lang/String"                         => TypeOf<string> (),
					"V"                                        => TypeOfVoid (),
					"Z"                                        => TypeOf<Boolean> (),
					"java/lang/Boolean"                        => TypeOf<Boolean?> (),
					"B"                                        => TypeOf<SByte> (),
					"java/lang/Byte"                           => TypeOf<SByte?> (),
					"C"                                        => TypeOf<Char> (),
					"java/lang/Character"                      => TypeOf<Char?> (),
					"S"                                        => TypeOf<Int16> (),
					"java/lang/Short"                          => TypeOf<Int16?> (),
					"I"                                        => TypeOf<Int32> (),
					"java/lang/Integer"                        => TypeOf<Int32?> (),
					"J"                                        => TypeOf<Int64> (),
					"java/lang/Long"                           => TypeOf<Int64?> (),
					"F"                                        => TypeOf<Single> (),
					"java/lang/Float"                          => TypeOf<Single?> (),
					"D"                                        => TypeOf<Double> (),
					"java/lang/Double"                         => TypeOf<Double?> (),
					_                                          => null,
				};
			}

			[return: DynamicallyAccessedMembers (MethodsConstructors)]
			static Type TypeOf<
					[DynamicallyAccessedMembers (MethodsConstructors)]
					T> () => typeof (T);

			[return: DynamicallyAccessedMembers (MethodsConstructors)]
			static Type TypeOfVoid () => typeof (void);

			public override IEnumerable<Type> GetTypes (JniTypeSignature typeSignature)
			{
				AssertValid ();

				if (typeSignature.SimpleReference == null)
					return EmptyTypeArray;
				return CreateGetTypesEnumerator (typeSignature);
			}

			public override IEnumerable<ReflectionConstructibleType> GetReflectionConstructibleTypes (JniTypeSignature typeSignature)
			{
				foreach (var type in GetTypes (typeSignature)) {
					yield return new ReflectionConstructibleType (type);
				}
			}

			IEnumerable<Type> CreateGetTypesEnumerator (JniTypeSignature typeSignature)
			{
				if (!typeSignature.IsValid)
					yield break;
				foreach (var type in GetTypesForSimpleReference (typeSignature.SimpleReference ?? throw new InvalidOperationException ("Should not be reached"))) {
					if (typeSignature.ArrayRank == 0) {
						yield return type;
						continue;
					}

					if (typeSignature.IsKeyword) {
						foreach (var t in GetPrimitiveArrayTypesForSimpleReference (typeSignature, type)) {
							yield return t;
						}
						continue;
					}

					if (typeSignature.ArrayRank > 0) {
						var rank        = typeSignature.ArrayRank;
						var arrayType   = type;
						while (rank-- > 0) {
							arrayType = TryMakeJavaObjectArrayType (arrayType, out var nextArrayType)
								? nextArrayType ?? throw new InvalidOperationException ("Should not be reached")
								: GetUnsupportedJavaObjectArrayType (arrayType);
						}
						yield return arrayType;
					}

					if (typeSignature.ArrayRank > 0) {
						var rank        = typeSignature.ArrayRank;
						var arrayType   = type;
						while (rank-- > 0) {
							arrayType = TryMakeArrayType (arrayType, out var nextArrayType)
								? nextArrayType ?? throw new InvalidOperationException ("Should not be reached")
								: GetUnsupportedArrayType (arrayType);
						}
						yield return arrayType;
					}
				}
			}

			IEnumerable<Type> GetPrimitiveArrayTypesForSimpleReference (JniTypeSignature typeSignature, Type type)
			{
				int index   = -1;
				for (int i = 0; i < JniPrimitiveArrayTypes.Length; ++i) {
					if (JniPrimitiveArrayTypes [i].PrimitiveType == type) {
						index   = i;
						break;
					}
				}
				if (index == -1) {
					throw new InvalidOperationException ($"Should not be reached; Could not find JniPrimitiveArrayInfo for {type}");
				}
				foreach (var t in JniPrimitiveArrayTypes [index].ArrayTypes) {
					var rank        = typeSignature.ArrayRank-1;
					var arrayType   = t;
					while (rank-- > 0) {
						arrayType = TryMakeJavaObjectArrayType (arrayType, out var nextArrayType)
							? nextArrayType ?? throw new InvalidOperationException ("Should not be reached")
							: GetUnsupportedJavaObjectArrayType (arrayType);
					}
					yield return arrayType;

					rank            = typeSignature.ArrayRank-1;
					arrayType       = t;
					while (rank-- > 0) {
						arrayType = TryMakeArrayType (arrayType, out var nextArrayType)
							? nextArrayType ?? throw new InvalidOperationException ("Should not be reached")
							: GetUnsupportedArrayType (arrayType);
					}
					yield return arrayType;
				}
			}

			protected override IEnumerable<Type> GetTypesForSimpleReference (string jniSimpleReference)
			{
				AssertValid ();
				AssertSimpleReference (jniSimpleReference);

				// Not sure why CS8604 is reported on following line when we check against null ~9 lines above...
				return CreateGetTypesForSimpleReferenceEnumerator (jniSimpleReference!);
			}

			IEnumerable<Type> CreateGetTypesForSimpleReferenceEnumerator (string jniSimpleReference)
			{
				if (JniBuiltinSimpleReferenceToType.Value.TryGetValue (jniSimpleReference, out var ret)) {
					yield return ret;
				}
				if (RuntimeFeature.ManagedPeerNativeRegistration && jniSimpleReference == ManagedPeer.JniTypeName) {
					yield return typeof (ManagedPeer);
				}
				yield break;
			}

			[return: DynamicallyAccessedMembers (Constructors)]
			protected override Type? GetInvokerTypeCore (
					[DynamicallyAccessedMembers (Constructors)]
					Type type)
			{
				var signature   = type.GetCustomAttribute<JniTypeSignatureAttribute> ();
				if (signature == null || signature.InvokerType == null) {
					return null;
				}

				Type[] arguments = type.GetGenericArguments ();
				if (arguments.Length == 0)
					return signature.InvokerType;

#if NET
				throw new NotSupportedException ($"Generic invoker type construction for `{type}` is not supported.");
#else   // NET
				return signature.InvokerType.MakeGenericType (arguments);
#endif  // NET
			}

#if NET

			protected override IReadOnlyList<string>? GetStaticMethodFallbackTypesCore (string jniSimple) => null;

			protected override string? GetReplacementTypeCore (string jniSimpleReference) => null;

			protected override ReplacementMethodInfo? GetReplacementMethodInfoCore (string jniSimpleReference, string jniMethodName, string jniMethodSignature) => null;

			public override void RegisterNativeMembers (
					JniType nativeClass,
					[DynamicallyAccessedMembers (MethodsAndPrivateNested)]
					Type type,
					ReadOnlySpan<char> methods)
			{
				TryRegisterNativeMembers (nativeClass, type, methods);
			}

			protected bool TryRegisterNativeMembers (
					JniType nativeClass,
					[DynamicallyAccessedMembers (MethodsAndPrivateNested)]
					Type type,
					ReadOnlySpan<char> methods)
			{
				AssertValid ();

#pragma warning disable CS1717
				methods = methods;
#pragma warning restore CS1717

				return TryLoadJniMarshalMethods (nativeClass, type, null) || TryRegisterNativeMembers (nativeClass, type, null, null);
			}
#endif  // NET

#if NET
			[Obsolete ("Use RegisterNativeMembers(JniType, Type, ReadOnlySpan<char>)")]
#endif  // NET
			public override void RegisterNativeMembers (
					JniType nativeClass,
					[DynamicallyAccessedMembers (MethodsAndPrivateNested)]
					Type type,
					string? methods)
			{
				TryRegisterNativeMembers (nativeClass, type, methods);
			}

#if NET
			[Obsolete ("Use RegisterNativeMembers(JniType, Type, ReadOnlySpan<char>)")]
#endif  // NET
			protected bool TryRegisterNativeMembers (
					JniType nativeClass,
					[DynamicallyAccessedMembers (MethodsAndPrivateNested)]
					Type type,
					string? methods)
			{
				AssertValid ();

				return TryLoadJniMarshalMethods (nativeClass, type, methods) || TryRegisterNativeMembers (nativeClass, type, methods, null);
			}

			static Type [] registerMethodParameters = new Type [] { typeof (JniNativeMethodRegistrationArguments) };

			// https://github.com/xamarin/xamarin-android/blob/5472eec991cc075e4b0c09cd98a2331fb93aa0f3/src/Microsoft.Android.Sdk.ILLink/PreserveRegistrations.cs#L85
			const string MarshalMethods = "'jni_marshal_methods' is preserved by the PreserveRegistrations trimmer step.";

			[UnconditionalSuppressMessage ("Trimming", "IL2072", Justification = MarshalMethods)]
			[UnconditionalSuppressMessage ("Trimming", "IL2075", Justification = MarshalMethods)]
			bool TryLoadJniMarshalMethods (
					JniType nativeClass,
					[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.NonPublicNestedTypes)]
					Type type,
					string? methods)
			{
				var marshalType = type?.GetNestedType ("__<$>_jni_marshal_methods", BindingFlags.NonPublic);
				if (marshalType == null) {
					return false;
				}

				var registerMethod = marshalType.GetMethod (
						name:           "__RegisterNativeMembers",
						bindingAttr:    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public,
						binder:         null,
						callConvention: default,
						types:          registerMethodParameters,
						modifiers:      null);
				return TryRegisterNativeMembers (nativeClass, marshalType, methods, registerMethod);
			}

			static List<JniNativeMethodRegistration> sharedRegistrations = new List<JniNativeMethodRegistration> ();

			bool TryRegisterNativeMembers (
					JniType nativeClass,
					[DynamicallyAccessedMembers (Methods)]
					Type marshalType,
					string? methods,
					MethodInfo? registerMethod)
			{
				bool lockTaken = false;
				bool rv = false;

				try {
					Monitor.TryEnter (sharedRegistrations, ref lockTaken);
					List<JniNativeMethodRegistration> registrations;
					if (lockTaken) {
						sharedRegistrations.Clear ();
						registrations = sharedRegistrations;
					} else {
						registrations = new List<JniNativeMethodRegistration> ();
					}
					JniNativeMethodRegistrationArguments arguments = new JniNativeMethodRegistrationArguments (registrations, methods);
					if (registerMethod != null) {
						registerMethod.Invoke (null, new object [] { arguments });
						rv = true;
					} else
						rv = FindAndCallRegisterMethod (marshalType, arguments);

					if (registrations.Count > 0)
						nativeClass.RegisterNativeMethods (registrations.ToArray ());
				} finally {
					if (lockTaken) {
						Monitor.Exit (sharedRegistrations);
					}
				}

				return rv;
			}

			bool FindAndCallRegisterMethod (
					[DynamicallyAccessedMembers (Methods)]
					Type marshalType,
					JniNativeMethodRegistrationArguments arguments)
			{
				if (!Runtime.JniAddNativeMethodRegistrationAttributePresent)
					return false;

				bool found = false;

				foreach (var methodInfo in marshalType.GetRuntimeMethods ()) {
					if (methodInfo.GetCustomAttribute (typeof (JniAddNativeMethodRegistrationAttribute)) == null) {
						continue;
					}

					var declaringTypeName = methodInfo.DeclaringType?.FullName ?? "<no-decl-type>";

					if ((methodInfo.Attributes & MethodAttributes.Static) != MethodAttributes.Static) {
						throw new InvalidOperationException ($"The method `{declaringTypeName}.{methodInfo}` marked with [{nameof (JniAddNativeMethodRegistrationAttribute)}] must be static!");
					}

					var register = (Action<JniNativeMethodRegistrationArguments>)methodInfo.CreateDelegate (typeof (Action<JniNativeMethodRegistrationArguments>));
					register (arguments);

					found = true;
				}

				return found;
			}

		}
	}
}
