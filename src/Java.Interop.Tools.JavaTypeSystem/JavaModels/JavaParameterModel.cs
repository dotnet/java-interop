using System;
using System.Collections.Generic;
using System.Linq;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public class JavaParameterModel : IJavaResolvable
	{
		public string Name { get; }
		public string Type { get; }
		public string JniType { get; }
		public bool IsNotNull { get; }
		public string GenericType { get; }

		public JavaMethodModel ParentMethod { get; }
		public JavaTypeReference? TypeModel { get; private set; }
		public bool IsParameterArray { get; private set; }
		public string? InstantiatedGenericArgumentName { get; internal set; }

		public JavaParameterModel (JavaMethodModel parent, string javaName, string javaType, string jniType, bool isNotNull)
		{
			ParentMethod = parent;
			Name = javaName;
			Type = javaType;
			JniType = jniType;
			IsNotNull = isNotNull;
			GenericType = javaType;

			if (Type.Contains ('<'))
				Type = Type.Substring (0, Type.IndexOf ('<'));
		}

		public void Resolve (JavaTypeCollection types, List<JavaUnresolvableModel> unresolvables)
		{
			var jtn = JavaTypeName.Parse (GenericType);

			if (jtn.ArrayPart == "...")
				IsParameterArray = true;

			var type_parameters = ParentMethod.GetApplicableTypeParameters ().ToArray ();

			try {
				TypeModel = types.ResolveTypeReference (GenericType, type_parameters);
			} catch (JavaTypeResolutionException) {
				unresolvables.Add (new JavaUnresolvableModel (this, Type, UnresolvableType.ParameterType));

				return;
			}
		}

		public override string ToString ()
		{
			return $"{GenericType} {Name}";
		}
	}
}
