using System.Diagnostics.CodeAnalysis;

using Java.Interop;

namespace Hello_NativeAOTFromJNI;

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
class NativeAotTypeManager : JniRuntime.ReflectionJniTypeManager {

	const DynamicallyAccessedMemberTypes MethodsConstructors =
		DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods |
		DynamicallyAccessedMemberTypes.NonPublicNestedTypes |
		DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors;

	[UnconditionalSuppressMessage ("Trimming", "IL2026", Justification = "Reflection-based registration used by this NativeAOT sample does not require unreferenced code.")]
	[UnconditionalSuppressMessage ("AOT", "IL3050", Justification = "Reflection-based registration used by this NativeAOT sample does not require runtime code generation.")]
	public NativeAotTypeManager ()
	{
	}

	// The base ReflectionJniTypeManager resolves built-in types (primitives, java/lang/String,
	// JavaProxyObject, ...) and handles registration and the reverse Type->JNI mapping (via the
	// [JniTypeSignature] attribute) for us. We only need to teach it about this sample's own
	// managed types.
	[return: DynamicallyAccessedMembers (MethodsConstructors)]
	protected override Type? GetTypeForSimpleReference (string jniSimpleReference)
	{
		if (jniSimpleReference == Example.ManagedType.JniTypeName)
			return typeof (Example.ManagedType);
		return base.GetTypeForSimpleReference (jniSimpleReference);
	}

	protected override IEnumerable<Type> GetTypesForSimpleReference (string jniSimpleReference)
	{
		if (jniSimpleReference == Example.ManagedType.JniTypeName)
			yield return typeof (Example.ManagedType);
		foreach (var t in base.GetTypesForSimpleReference (jniSimpleReference))
			yield return t;
	}
}
