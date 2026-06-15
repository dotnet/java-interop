using System.Diagnostics.CodeAnalysis;

using Java.Interop;

namespace Java.Interop.Samples.NativeAotFromAndroid;

// See the comment in Hello-NativeAOTFromJNI/NativeAotTypeManager.cs: this derives from the
// reflection-based JniRuntime.ReflectionJniTypeManager (the pre-dotnet/java-interop#1441 base
// behavior) so built-in runtime types such as JavaProxyObject get their native members registered
// automatically. ReflectionJniTypeManager is annotated [RequiresDynamicCode]/[RequiresUnreferencedCode];
// the reflection paths this sample exercises do not require runtime code generation, so the
// constructor suppresses the resulting trimming/AOT warnings via [UnconditionalSuppressMessage]
// (a #pragma would not survive the ILLink/ILC publish passes).
partial class NativeAotTypeManager : JniRuntime.ReflectionJniTypeManager {

	Dictionary<string, Type> typeMappings = new () {
		["android/app/Activity"]                = typeof (Android.App.Activity),
		["android/content/Context"]             = typeof (Android.Content.Context),
		["android/content/ContextWrapper"]      = typeof (Android.Content.ContextWrapper),
		["android/os/BaseBundle"]               = typeof (Android.OS.BaseBundle),
		["android/os/Bundle"]                   = typeof (Android.OS.Bundle),
		["android/view/ContextThemeWrapper"]    = typeof (Android.View.ContextThemeWrapper),
		["my/MainActivity"]                     = typeof (MainActivity),
	};

	[UnconditionalSuppressMessage ("Trimming", "IL2026", Justification = "Reflection-based registration used by this NativeAOT sample does not require unreferenced code; see class comment.")]
	[UnconditionalSuppressMessage ("AOT", "IL3050", Justification = "Reflection-based registration used by this NativeAOT sample does not require runtime code generation; see class comment.")]
	public NativeAotTypeManager ()
	{
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
		if (typeMappings == null)
			yield break;
		foreach (var e in typeMappings) {
			if (e.Value == type)
				yield return e.Key;
		}
	}
}
