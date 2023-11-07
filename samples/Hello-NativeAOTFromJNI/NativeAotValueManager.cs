using System.Reflection;

using Java.Interop;

namespace Hello_NativeAOTFromJNI;

class NativeAotValueManager : ManagedValueManager {

	// Can't use default version, because:
	//   System.ArgumentException: Instance property 'DeclaringType' is not defined for type 'System.Reflection.MemberInfo' (Parameter 'propertyName')
	//      at System.Linq.Expressions.Expression.Property(Expression, String) + 0xaa
	//      at Java.Interop.MarshalMemberBuilder.CreateConstructActivationPeerExpression(ConstructorInfo) + 0x209
	//      at Java.Interop.JniRuntime.JniMarshalMemberBuilder.CreateConstructActivationPeerFunc(ConstructorInfo) + 0x13
	// Do it "manually"
	public override void ActivatePeer (IJavaPeerable? self, JniObjectReference reference, ConstructorInfo cinfo, object?[]? argumentValues)
	{
		var declType = cinfo.DeclaringType ?? throw new NotSupportedException ("Do not know the type to create!");

#pragma warning disable IL2072
		self = (IJavaPeerable) System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject (declType);
#pragma warning restore IL2072
		self.SetPeerReference (reference);

		cinfo.Invoke (self, argumentValues);
	}
}
