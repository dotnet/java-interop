using Java.Interop;
using System.Diagnostics.CodeAnalysis;

namespace Hello_NativeAOTFromJNI;

class NativeAotTypeManager : JniRuntime.JniTypeManager {
	internal const DynamicallyAccessedMemberTypes Methods = DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods;
	internal const DynamicallyAccessedMemberTypes MethodsAndPrivateNested = Methods | DynamicallyAccessedMemberTypes.NonPublicNestedTypes;
	internal const DynamicallyAccessedMemberTypes MethodsConstructors = MethodsAndPrivateNested | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors;

	protected override IEnumerable<Type> GetTypesForSimpleReference (string jniSimpleReference)
	{
		var target = GetTypeForSimpleReference (jniSimpleReference);
		if (target != null)
			yield return target;
	}

	[return: DynamicallyAccessedMembers (MethodsConstructors)]
	protected override Type? GetTypeForSimpleReference (string jniSimpleReference)
	{
		return jniSimpleReference switch {
			Example.ManagedType.JniTypeName => TypeOf<Example.ManagedType> (),
			"java/lang/Object"             => TypeOf<Java.Lang.Object> (),
			"java/lang/String"             => TypeOf<Java.Lang.String> (),
			_                              => null,
		};
	}

	[return: DynamicallyAccessedMembers (MethodsConstructors)]
	static Type TypeOf<
			[DynamicallyAccessedMembers (MethodsConstructors)]
			T> () => typeof (T);

	public override IEnumerable<Type> GetTypes (JniTypeSignature typeSignature)
	{
		if (!typeSignature.IsValid || typeSignature.ArrayRank != 0 || typeSignature.SimpleReference == null)
			return [];
		return GetTypesForSimpleReference (typeSignature.SimpleReference);
	}

	public override IEnumerable<JniRuntime.JniTypeManager.ReflectionConstructibleType> GetReflectionConstructibleTypes (JniTypeSignature typeSignature)
	{
		if (!typeSignature.IsValid || typeSignature.ArrayRank != 0 || typeSignature.SimpleReference == null)
			yield break;
		var target = GetTypeForSimpleReference (typeSignature.SimpleReference);
		if (target != null)
			yield return new JniRuntime.JniTypeManager.ReflectionConstructibleType (target);
	}

	protected override IEnumerable<string> GetSimpleReferences (Type type)
	{
		return CreateSimpleReferencesEnumerator (type);
	}

	IEnumerable<string> CreateSimpleReferencesEnumerator (Type type)
	{
		if (type == typeof (Example.ManagedType))
			yield return Example.ManagedType.JniTypeName;
		else if (type == typeof (Java.Lang.Object))
			yield return "java/lang/Object";
		else if (type == typeof (Java.Lang.String))
			yield return "java/lang/String";
	}

	protected override string? GetSimpleReference (Type type)
	{
		return GetSimpleReferences (type).FirstOrDefault ();
	}

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

	[return: DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
	protected override Type? GetInvokerTypeCore (
			[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
			Type type)
	{
		return null;
	}

	protected override IReadOnlyList<string>? GetStaticMethodFallbackTypesCore (string jniSimpleReference)
	{
		return null;
	}

	protected override string? GetReplacementTypeCore (string jniSimpleReference)
	{
		return null;
	}

	protected override JniRuntime.ReplacementMethodInfo? GetReplacementMethodInfoCore (string jniSourceType, string jniMethodName, string jniMethodSignature)
	{
		return null;
	}

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

	[Obsolete ("Use RegisterNativeMembers(JniType, Type, ReadOnlySpan<char>)")]
	public override void RegisterNativeMembers (
			JniType nativeClass,
			[DynamicallyAccessedMembers (MethodsAndPrivateNested)]
			Type type,
			string? methods)
	{
		RegisterNativeMembers (nativeClass, type, methods.AsSpan ());
	}
}
