using System.Diagnostics.CodeAnalysis;

using Java.Interop;

namespace Java.Interop.Samples.NativeAotFromAndroid;

// This sample derives from the reflection-based JniRuntime.ReflectionJniTypeManager, which is
// annotated [RequiresDynamicCode]/[RequiresUnreferencedCode], so the constructor below suppresses
// the resulting IL2026/IL3050 trim/AOT warnings.
//
// Suppressing here is intentional and good enough: these NativeAOT projects are *samples*, not
// product code. .NET for Android (what we actually ship) does not pair ReflectionJniTypeManager
// with NativeAOT, so it isn't worth the effort to make these samples fully trim/AOT-clean right now.
// The reflection paths were always trim/AOT-unsafe: before dotnet/java-interop#1441 the equivalent
// suppressions lived (buried) inside JniTypeManager itself, justified "NotUsedInAndroid"; #1441 just
// moved that responsibility to callers via [RequiresDynamicCode]/[RequiresUnreferencedCode].
partial class NativeAotTypeManager : JniRuntime.ReflectionJniTypeManager {

	const DynamicallyAccessedMemberTypes MethodsConstructors =
		DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods |
		DynamicallyAccessedMemberTypes.NonPublicNestedTypes |
		DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors;

	Dictionary<string, Type> typeMappings = new () {
		["android/app/Activity"]                = typeof (Android.App.Activity),
		["android/content/Context"]             = typeof (Android.Content.Context),
		["android/content/ContextWrapper"]      = typeof (Android.Content.ContextWrapper),
		["android/os/BaseBundle"]               = typeof (Android.OS.BaseBundle),
		["android/os/Bundle"]                   = typeof (Android.OS.Bundle),
		["android/view/ContextThemeWrapper"]    = typeof (Android.View.ContextThemeWrapper),
		["my/MainActivity"]                     = typeof (MainActivity),
	};

	[UnconditionalSuppressMessage ("Trimming", "IL2026", Justification = "Reflection-based registration used by this NativeAOT sample does not require unreferenced code.")]
	[UnconditionalSuppressMessage ("AOT", "IL3050", Justification = "Reflection-based registration used by this NativeAOT sample does not require runtime code generation.")]
	public NativeAotTypeManager ()
	{
	}

	// GetType() dispatches through GetTypeForSimpleReference (singular), so the sample's own type
	// map has to be applied here; the base ReflectionJniTypeManager only knows the built-in types.
	[return: DynamicallyAccessedMembers (MethodsConstructors)]
	protected override Type? GetTypeForSimpleReference (string jniSimpleReference)
	{
		if (typeMappings.TryGetValue (jniSimpleReference, out var target))
			return target;
		return base.GetTypeForSimpleReference (jniSimpleReference);
	}

	protected override IEnumerable<Type> GetTypesForSimpleReference (string jniSimpleReference)
	{
		if (typeMappings.TryGetValue (jniSimpleReference, out var target))
			yield return target;
		foreach (var t in base.GetTypesForSimpleReference (jniSimpleReference))
			yield return t;
	}

	protected override IEnumerable<string> GetSimpleReferences (Type type)
	{
		return base.GetSimpleReferences (type)
			.Concat (CreateSimpleReferencesEnumerator (type));
	}

	IEnumerable<string> CreateSimpleReferencesEnumerator (Type type)
	{
		foreach (var e in typeMappings) {
			if (e.Value == type)
				yield return e.Key;
		}
	}
}
