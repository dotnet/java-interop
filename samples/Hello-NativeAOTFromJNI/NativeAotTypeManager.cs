using Java.Interop;

namespace Hello_NativeAOTFromJNI;

class NativeAotTypeManager : JniRuntime.JniTypeManager {

#pragma warning disable IL2026
	Dictionary<string, Type> typeMappings = new () {
		["com/xamarin/java_interop/internal/JavaProxyThrowable"] = Type.GetType ("Java.Interop.JavaProxyThrowable, Java.Interop", throwOnError: true)!,
		["example/ManagedType"] = typeof (Example.ManagedType),
		["System.Int32, System.Runtime"] = typeof (int),
		["System.Int32, System.Private.CoreLib"] = typeof (int),
		["Java.Interop.JavaProxyThrowable, Java.Interop"] = Type.GetType ("Java.Interop.JavaProxyThrowable, Java.Interop", throwOnError: true)!,
		["Example.ManagedType, Hello-NativeAOTFromJNI"] = typeof (Example.ManagedType),
	};
#pragma warning restore IL2026


	public override Type GetTypeFromAssemblyQualifiedName (string assemblyQualifiedTypeName)
	{
		if (typeMappings.TryGetValue (assemblyQualifiedTypeName, out var type))
			return type;
		throw new NotSupportedException ($"Unsupported type: \"{assemblyQualifiedTypeName}\"!");
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
