using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public JavaParameterModel Clone (JavaMethodModel parentMethod)
		{
			return new JavaParameterModel (parentMethod, Name, Type, JniType, IsNotNull) {
				TypeModel = TypeModel,
				IsParameterArray = IsParameterArray
			};
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
				unresolvables.Add (new JavaUnresolvableModel (this, Type));

				return;
			}


			//var type_parameters = parentType.TypeParameters.Concat (parentMethod.TypeParameters).ToList ();

			// This handles a non-generic type that is implementing a generic interface:
			//   class MapIterator implements Iterator<Map.Entry<K, V>>, Map.Entry<K, V> { ... }
			//foreach (var i in parentType.ImplementsModels)
			//	if (i.ReferencedType?.TypeParameters is not null)
			//		foreach (var tp in i.ReferencedType.TypeParameters)
			//			type_parameters.Add (tp);

			//TypeModel = types.Resolve (jtn, parentType, parentMethod);

			//if (TypeModel is null)
			//	throw new Exception ();

			//if (JavaTypeSymbol.IsGeneric) {
			// Java is using this as a type without a "T".  C#
			// can't do that, so we're going to make T into JLO.
			//var known = parentType.TypeParameters.Select (p => p.Name).Concat (parentMethod.TypeParameters.Select (p => p.Name)).Distinct ().ToArray ();
			//TypeModel = TypeModel.SetUnknownGenericTypeArguments (types.Resolve ("java.lang.Object")!, known);
			//}
		}

		public override string ToString ()
		{
			return $"{GenericType} {Name}";
		}
	}
}
