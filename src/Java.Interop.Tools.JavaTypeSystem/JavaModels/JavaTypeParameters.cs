using System;
using System.Collections.Generic;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public class JavaTypeParameters : List<JavaTypeParameter>
	{
		public JavaTypeModel? DeclaringType { get; }
		public JavaMethodModel? DeclaringMethod { get; }

		public Dictionary<string, string> PropertyBag { get; } = new Dictionary<string, string> ();

		public JavaTypeParameters (JavaTypeModel declaringType) => DeclaringType = declaringType;
		public JavaTypeParameters (JavaMethodModel declaringMethod) => DeclaringMethod = declaringMethod;
	}
}
