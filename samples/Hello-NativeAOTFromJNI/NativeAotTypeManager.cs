using System.Diagnostics.CodeAnalysis;

using Java.Interop;

namespace Hello_NativeAOTFromJNI;

// This NativeAOT sample derives from the reflection-based JniRuntime.ReflectionJniTypeManager,
// which is the behavior the base JniRuntime.JniTypeManager provided before dotnet/java-interop#1441.
// Deriving from it means built-in runtime types such as JavaProxyObject/JavaProxyThrowable get
// their native members registered automatically (via reflection over their
// [JniAddNativeMethodRegistration] methods), so the sample does not need to register them by hand.
// ReflectionJniTypeManager is annotated [RequiresDynamicCode]/[RequiresUnreferencedCode]; the
// reflection paths actually exercised by this sample do not require runtime code generation, so the
// constructor suppresses the resulting trimming/AOT warnings (a #pragma would not survive the
// ILLink/ILC publish passes, so an [UnconditionalSuppressMessage] is required).
class NativeAotTypeManager : JniRuntime.ReflectionJniTypeManager {

	Dictionary<string, Type> typeMappings = new () {
		[Example.ManagedType.JniTypeName]   = typeof (Example.ManagedType),
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
