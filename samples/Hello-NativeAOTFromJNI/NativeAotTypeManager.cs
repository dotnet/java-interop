using System.Diagnostics.CodeAnalysis;

using Java.Interop;

namespace Hello_NativeAOTFromJNI;

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
