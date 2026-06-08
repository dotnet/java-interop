using Java.Interop;
using System.Diagnostics.CodeAnalysis;

namespace Hello_NativeAOTFromJNI;

class NativeAotTypeManager : JniRuntime.DynamicJniTypeManager {
	internal const DynamicallyAccessedMemberTypes Methods = DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods;
	internal const DynamicallyAccessedMemberTypes MethodsAndPrivateNested = Methods | DynamicallyAccessedMemberTypes.NonPublicNestedTypes;

#pragma warning disable IL2026
	Dictionary<string, Type> typeMappings = new () {
		[Example.ManagedType.JniTypeName]   = typeof (Example.ManagedType),
		[Java.Lang.Object.JniTypeName]      = typeof (Java.Lang.Object),
		[Java.Lang.String.JniTypeName]      = typeof (Java.Lang.String),
	};
#pragma warning restore IL2026

	protected override IEnumerable<Type> GetTypesForSimpleReference (string jniSimpleReference)
	{
		if (typeMappings.TryGetValue (jniSimpleReference, out var target))
			yield return target;
		foreach (var type in base.GetTypesForSimpleReference (jniSimpleReference))
			yield return type;
	}

	protected override Type? GetTypeForSimpleReference (string jniSimpleReference)
	{
		if (typeMappings.TryGetValue (jniSimpleReference, out var target))
			return target;
		return base.GetTypeForSimpleReference (jniSimpleReference);
	}

	protected override IEnumerable<string> GetSimpleReferences (Type type)
	{
		return base.GetSimpleReferences (type)
			.Concat (CreateSimpleReferencesEnumerator (type));
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
