#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Java.Interop {

	[AttributeUsage (Targets, AllowMultiple=false)]
	public class JniValueMarshalerAttribute : Attribute {
		const DynamicallyAccessedMemberTypes ParameterlessConstructors = DynamicallyAccessedMemberTypes.PublicParameterlessConstructor;

		const   AttributeTargets    Targets =
			AttributeTargets.Class | AttributeTargets.Enum |
			AttributeTargets.Interface | AttributeTargets.Struct |
			AttributeTargets.Parameter | AttributeTargets.ReturnValue;

		public JniValueMarshalerAttribute (
				[DynamicallyAccessedMembers (ParameterlessConstructors)]
				Type marshalerType)
		{
			if (marshalerType == null)
				throw new ArgumentNullException (nameof (marshalerType));
			if (!typeof (JniValueMarshaler).IsAssignableFrom (marshalerType))
				throw new ArgumentException (
						string.Format ("`{0}` must inherit from JniValueMarshaler!", marshalerType.FullName),
						nameof (marshalerType));

			MarshalerType   = marshalerType;
		}

		[DynamicallyAccessedMembers (ParameterlessConstructors)]
		public  Type    MarshalerType   {
			get;
		}
	}
}

