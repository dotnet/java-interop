using Java.Interop;
using System.Diagnostics.CodeAnalysis;

namespace Hello_NativeAOTFromJNI;

class NativeAotTypeManager : JniRuntime.JniTypeManager {
	internal const DynamicallyAccessedMemberTypes Constructors = DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors;
	internal const DynamicallyAccessedMemberTypes Methods = DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods;
	internal const DynamicallyAccessedMemberTypes MethodsAndPrivateNested = Methods | DynamicallyAccessedMemberTypes.NonPublicNestedTypes;

#pragma warning disable IL2026
	Dictionary<string, Type> typeMappings = new () {
		[Example.ManagedType.JniTypeName]   = typeof (Example.ManagedType),
		[Java.Lang.Object.JniTypeName]      = typeof (Java.Lang.Object),
		[Java.Lang.String.JniTypeName]      = typeof (Java.Lang.String),
	};
#pragma warning restore IL2026

	protected override string? GetSimpleReference (Type type)
	{
		foreach (var e in typeMappings) {
			if (e.Value == type)
				return e.Key;
		}
		return null;
	}


	protected override IEnumerable<Type> GetTypesForSimpleReference (string jniSimpleReference)
	{
		if (typeMappings.TryGetValue (jniSimpleReference, out var target))
			yield return target;
	}

	protected override Type? GetTypeForSimpleReference (string jniSimpleReference)
	{
		if (typeMappings.TryGetValue (jniSimpleReference, out var target))
			return target;
		return null;
	}

	protected override IEnumerable<string> GetSimpleReferences (Type type)
	{
		return CreateSimpleReferencesEnumerator (type);
	}

	IEnumerable<string> CreateSimpleReferencesEnumerator (Type type)
	{
		if (typeMappings == null)
			yield break;
		foreach (var e in typeMappings) {
			if (e.Value == type)
				yield return e.Key;
		}
	}

	public override IEnumerable<Type> GetTypes (JniTypeSignature typeSignature)
	{
		if (!typeSignature.IsValid || typeSignature.ArrayRank != 0 || typeSignature.SimpleReference == null)
			return [];
		return GetTypesForSimpleReference (typeSignature.SimpleReference);
	}

	public override IEnumerable<ReflectionConstructibleType> GetReflectionConstructibleTypes (JniTypeSignature typeSignature)
	{
		foreach (var type in GetTypes (typeSignature)) {
			yield return new ReflectionConstructibleType (type);
		}
	}

	protected override Type? GetInvokerTypeCore ([DynamicallyAccessedMembers (Constructors)] Type type) => null;

	protected override JniTypeSignature GetTypeSignatureCore (Type type)
	{
		var simpleReference = GetSimpleReference (type);
		return simpleReference == null ? default : new JniTypeSignature (simpleReference, 0, false);
	}

	protected override IEnumerable<JniTypeSignature> GetTypeSignaturesCore (Type type)
	{
		var signature = GetTypeSignatureCore (type);
		if (signature.IsValid)
			yield return signature;
	}

	protected override IReadOnlyList<string>? GetStaticMethodFallbackTypesCore (string jniSimpleReference) => null;

	protected override string? GetReplacementTypeCore (string jniSimpleReference) => null;

	protected override JniRuntime.ReplacementMethodInfo? GetReplacementMethodInfoCore (string jniSimpleReference, string jniMethodName, string jniMethodSignature) => null;

	public override void RegisterNativeMembers (
			JniType nativeClass,
			[DynamicallyAccessedMembers (MethodsAndPrivateNested)]
			Type type,
			ReadOnlySpan<char> methods)
	{
		if (type != typeof (Example.ManagedType)) {
			if (!methods.IsEmpty)
				throw new NotSupportedException ($"Could not register native members for type '{type.FullName}'.");
			return;
		}

		var registrations = new List<JniNativeMethodRegistration> ();
		Example.ManagedType.RegisterNativeMembers (new JniNativeMethodRegistrationArguments (registrations, null));
		if (registrations.Count > 0)
			nativeClass.RegisterNativeMethods (registrations.ToArray ());
	}

	public override void RegisterNativeMembers (
			JniType nativeClass,
			[DynamicallyAccessedMembers (MethodsAndPrivateNested)]
			Type type,
			string? methods)
	{
		RegisterNativeMembers (nativeClass, type, methods.AsSpan ());
	}
}
