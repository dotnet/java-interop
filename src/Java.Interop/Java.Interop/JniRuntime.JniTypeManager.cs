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
	}
}
