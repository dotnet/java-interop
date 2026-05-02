#if NET

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;


namespace Java.Interop {

	public class JreTypeManager : JniRuntime.JniTypeManager {

		const DynamicallyAccessedMemberTypes RequiredConstructors = DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors;

		IDictionary<string, Type>? typeMappings;

		public JreTypeManager ()
			: this (null)
		{
		}

		public JreTypeManager (IDictionary<string, Type>? typeMappings)
		{
			this.typeMappings = typeMappings;
		}

		[return: DynamicallyAccessedMembers (RequiredConstructors)]
		public override Type? GetType (JniTypeSignature typeSignature)
		{
			var type = base.GetType (typeSignature);
			if (type != null) {
				return type;
			}

			if (!typeSignature.IsValid || typeSignature.SimpleReference == null || typeSignature.ArrayRank != 0 || typeMappings == null) {
				return null;
			}

			return typeMappings.TryGetValue (typeSignature.SimpleReference, out var target)
				? target
				: null;
		}

		[return: DynamicallyAccessedMembers (RequiredConstructors)]
		public override Type? GetTypeAssignableTo (
				JniTypeSignature typeSignature,
				[DynamicallyAccessedMembers (RequiredConstructors)]
				Type targetType)
		{
			var type = base.GetTypeAssignableTo (typeSignature, targetType);
			if (type != null) {
				return type;
			}

			if (!typeSignature.IsValid || typeSignature.SimpleReference == null || typeSignature.ArrayRank != 0 || typeMappings == null) {
				return null;
			}

			if (typeMappings.TryGetValue (typeSignature.SimpleReference, out var target) && targetType.IsAssignableFrom (target)) {
				return target;
			}
			return null;
		}

		[return: DynamicallyAccessedMembers (JniRuntime.JniTypeManager.MethodsConstructors)]
		public override Type? GetTypeForNativeRegistration (JniTypeSignature typeSignature)
		{
			return GetType (typeSignature);
		}

		protected override IEnumerable<Type> GetTypesForSimpleReference (string jniSimpleReference)
		{
			foreach (var t in base.GetTypesForSimpleReference (jniSimpleReference))
				yield return t;
			if (typeMappings == null)
				yield break;
			if (typeMappings.TryGetValue (jniSimpleReference, out var target))
				yield return target;
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

		public override void RegisterNativeMembers (
				JniType nativeClass,
				[DynamicallyAccessedMembers (MethodsAndPrivateNested)]
				Type type,
				ReadOnlySpan<char> methods)
		{
			if (base.TryRegisterNativeMembers (nativeClass, type, methods)) {
				return;
			}

			if (methods.IsEmpty) {
				return;
			}

			throw new NotSupportedException (
				$"Could not register native members for type '{type.FullName}'. " +
				"Ensure that the type has the appropriate [JniAddNativeMethodRegistration] attribute and static registration method.");
		}
	}
}

#endif  // NET
