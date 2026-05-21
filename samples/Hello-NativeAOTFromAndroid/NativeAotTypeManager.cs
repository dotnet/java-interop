using System.Diagnostics.CodeAnalysis;

using Java.Interop;

namespace Java.Interop.Samples.NativeAotFromAndroid;

partial class NativeAotTypeManager : JniRuntime.JniTypeManager {

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

	protected override IEnumerable<Type> GetTypesForSimpleReference (string jniSimpleReference)
	{
		if (typeMappings.TryGetValue (jniSimpleReference, out var target))
			yield return target;
		foreach (var t in base.GetTypesForSimpleReference (jniSimpleReference))
			yield return t;
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
}
