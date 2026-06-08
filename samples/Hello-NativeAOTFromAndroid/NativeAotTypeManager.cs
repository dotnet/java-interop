using System.Diagnostics.CodeAnalysis;

using Java.Interop;

namespace Java.Interop.Samples.NativeAotFromAndroid;

partial class NativeAotTypeManager : JniRuntime.DynamicJniTypeManager {

	internal const DynamicallyAccessedMemberTypes Methods = DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods;
	internal const DynamicallyAccessedMemberTypes MethodsAndPrivateNested = Methods | DynamicallyAccessedMemberTypes.NonPublicNestedTypes;

	Dictionary<string, Type> typeMappings = new () {
		["android/app/Activity"]                = typeof (Android.App.Activity),
		["android/content/Context"]             = typeof (Android.Content.Context),
		["android/content/ContextWrapper"]      = typeof (Android.Content.ContextWrapper),
		["android/os/BaseBundle"]               = typeof (Android.OS.BaseBundle),
		["android/os/Bundle"]                   = typeof (Android.OS.Bundle),
		["android/view/ContextThemeWrapper"]    = typeof (Android.View.ContextThemeWrapper),
		["my/MainActivity"]                     = typeof (MainActivity),
	};

	public override void RegisterNativeMembers (
			JniType nativeClass,
			[DynamicallyAccessedMembers (MethodsAndPrivateNested)]
			Type type,
			ReadOnlySpan<char> methods)
	{
		Console.WriteLine ($"# jonp: RegisterNativeMembers: nativeClass={nativeClass} type=`{type}`");
		if (!methods.IsEmpty)
			throw new NotSupportedException ($"Could not register native members for type '{type.FullName}'.");
	}

	public override void RegisterNativeMembers (
			JniType nativeClass,
			[DynamicallyAccessedMembers (MethodsAndPrivateNested)]
			Type type,
			string? methods)
	{
		RegisterNativeMembers (nativeClass, type, methods.AsSpan ());
	}

	protected override IEnumerable<Type> GetTypesForSimpleReference (string jniSimpleReference)
	{
		Console.WriteLine ($"# jonp: GetTypesForSimpleReference: jniSimpleReference=`{jniSimpleReference}`");
		if (typeMappings.TryGetValue (jniSimpleReference, out var target)) {
			Console.WriteLine ($"# jonp:   GetTypesForSimpleReference: jniSimpleReference=`{jniSimpleReference}` -> `{target}`");
			yield return target;
		}
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

	protected override JniTypeSignature GetTypeSignatureCore (Type type)
	{
		var simpleReference = GetSimpleReferences (type).FirstOrDefault ();
		return simpleReference == null ? default : new JniTypeSignature (simpleReference, 0, false);
	}

	protected override IEnumerable<JniTypeSignature> GetTypeSignaturesCore (Type type)
	{
		var signature = GetTypeSignatureCore (type);
		if (signature.IsValid)
			yield return signature;
	}
}
