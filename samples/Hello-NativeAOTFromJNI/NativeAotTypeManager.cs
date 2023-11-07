using Java.Interop;

namespace Hello_NativeAOTFromJNI;

class NativeAotTypeManager : JniRuntime.JniTypeManager {

#pragma warning disable IL2026
	Dictionary<string, Type> typeMappings = new () {
		["com/xamarin/java_interop/internal/JavaProxyThrowable"] = Type.GetType ("Java.Interop.JavaProxyThrowable, Java.Interop", throwOnError: true)!,
		["example/ManagedType"] = typeof (Example.ManagedType),
	};
#pragma warning restore IL2026

	public override void RegisterNativeMembers (JniType nativeClass, ReadOnlySpan<char> assemblyQualifiedTypeName, ReadOnlySpan<char> methods)
	{
		Console.WriteLine ($"C# RegisterNativeMembers({nativeClass}, \"{assemblyQualifiedTypeName}\", \"{methods}\")");
		if (typeMappings.TryGetValue (nativeClass.Name, out var type)) {
			base.TryRegisterNativeMembers (nativeClass, type, methods);
		}
		else {
			throw new NotSupportedException ($"Unsupported registration type: \"{nativeClass.Name}\" <=> \"{assemblyQualifiedTypeName}\"!");
		}
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
