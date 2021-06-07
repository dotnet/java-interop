using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public class JavaMethodModel : JavaMemberModel
	{
		public string Return { get; }
		public string ReturnGeneric { get; }
		public bool IsAbstract { get; }
		public bool IsBridge { get; }
		public string ReturnJni { get; }
		public bool IsSynthetic { get; }
		public bool IsSynchronized { get; }
		public bool IsNative { get; }
		public bool ReturnNotNull { get; }

		public JavaTypeParameters TypeParameters { get; set; } = new JavaTypeParameters ();
		public JavaTypeReference? ReturnTypeModel { get; private set; }
		public JavaMethodModel? BaseMethod { get; set; }
		public List<JavaParameterModel> Parameters { get; } = new List<JavaParameterModel> ();
		public List<JavaExceptionModel> Exceptions { get; } = new List<JavaExceptionModel> ();

		//public BoundMethodModel? ManagedModel { get; set; }

		public JavaMethodModel (string javaName, string javaVisibility, bool javaAbstract, bool javaFinal, bool javaStatic, string javaReturn, JavaTypeModel javaParentType, string deprecated, string jniSignature, bool isSynthetic, bool isBridge, string returnJni, bool isNative, bool isSynchronized, bool returnNotNull)
			: base (javaName, javaStatic, javaFinal, javaVisibility, javaParentType, deprecated, jniSignature)
		{
			IsAbstract = javaAbstract;
			Return = javaReturn;
			ReturnGeneric = javaReturn;
			IsBridge = isBridge;
			IsSynthetic = isSynthetic;
			ReturnJni = returnJni;
			IsNative = isNative;
			IsSynchronized = isSynchronized;
			ReturnNotNull = returnNotNull;

			//if (Return.Contains ('<'))
			//	Return = Return.Substring (0, Return.IndexOf ('<'));
		}

		public JavaMethodModel Clone (string? newVisibility = null, bool? newAbstract = null, JavaTypeModel? newParentType = null)
		{
			var result = new JavaMethodModel (Name, newVisibility ?? Visibility, newAbstract ?? IsAbstract, IsFinal, IsStatic, Return, newParentType ?? ParentType, Deprecated, JniSignature, IsSynthetic, IsBridge, ReturnJni, IsNative, IsSynchronized, ReturnNotNull) {
				ReturnTypeModel = ReturnTypeModel,
				BaseMethod = BaseMethod,
				//ManagedModel = ManagedModel
			};

			foreach (var p in Parameters)
				result.Parameters.Add (p.Clone (result));
			foreach (var tp in TypeParameters)
				result.TypeParameters.Add (new JavaTypeParameter (tp.Name));
			foreach (var p in PropertyBag)
				result.PropertyBag.Add (p.Key, p.Value);

			return result;
		}

		public override void Resolve (JavaTypeCollection types, List<JavaUnresolvableModel> unresolvables)
		{
			if (Name.Contains ('$')) {
				unresolvables.Add (new JavaUnresolvableModel (this, "$"));
				return;
			}

			var type_parameters = GetApplicableTypeParameters ().ToArray ();

			//var type_parameters = ParentType.TypeParameters.Concat (TypeParameters).ToList ();

			// This handles a non-generic type that is implementing a generic interface:
			//   class MapIterator implements Iterator<Map.Entry<K, V>>, Map.Entry<K, V> { ... }
			//foreach (var i in ParentType.ImplementsModels)
			//	if (i.ReferencedType?.TypeParameters is not null)
			//		foreach (var tp in i.ReferencedType.TypeParameters)
			//			type_parameters.Add (tp);


			try {
				ReturnTypeModel = types.ResolveTypeReference (Return, type_parameters);
			} catch (JavaTypeResolutionException) {
				unresolvables.Add (new JavaUnresolvableModel (this, Return));

				return;
			}

			//ReturnTypeModel = types.ResolveTypeReference (Return, type_parameters);
			//ReturnTypeModel = types.Resolve (Return, ParentType, this);

			//if (ReturnTypeModel is null)
			//	throw new Exception ();

			//if (JavaReturnTypeModel.IsGeneric) {
			// Java is using this as a type without a "T".  C#
			// can't do that, so we're going to make T into JLO.
			//ReturnTypeModel = ReturnTypeModel.SetUnknownGenericTypeArguments (types.Resolve ("java.lang.Object")!, GetKnownTypeArguments ());
			//}

			foreach (var p in Parameters.OfType<JavaParameterModel> ())
				p.Resolve (types, unresolvables);
		}

		//private string [] GetKnownTypeArguments ()
		//	=> ParentType.TypeParameters.Select (p => p.Name).Concat (TypeParameters.Select (p => p.Name)).Distinct ().ToArray ();

		// Return method's type parameters, plus type parameters for any parent type(s).
		public IEnumerable<JavaTypeParameter> GetApplicableTypeParameters ()
		{
			foreach (var jtp in TypeParameters)
				yield return jtp;

			if (ParentType != null)
				foreach (var jtp in ParentType.GetApplicableTypeParameters ())
					yield return jtp;
		}

		public void FindBaseMethod (JavaClassModel? type)
		{
			//if (ParentType.Name == "MethodReference")
			//	Debugger.Break ();

			if (type is null)
				return;

			var pt = (JavaClassModel)ParentType;

			var candidate = type.Methods.FirstOrDefault (p => p.Name == Name && IsImplementing (this, p, pt.GenericInheritanceMapping ?? throw new InvalidOperationException ($"missing {nameof (pt.GenericInheritanceMapping)}!")));

			if (candidate != null) {
				BaseMethod = candidate;

				for (var i = 0; i < candidate.Parameters.Count; i++)
					if (candidate.Parameters [i].TypeModel?.ReferencedTypeParameter != null && Parameters [i].TypeModel?.ReferencedTypeParameter == null)
						Parameters [i].InstantiatedGenericArgumentName = candidate.Parameters [i].TypeModel?.ReferencedTypeParameter?.Name;

				return;
			}

			if (type.BaseTypeReference?.ReferencedType is JavaClassModel klass)
				FindBaseMethod (klass);
		}

		// It should detect implementation method for:
		//	- equivalent implementation
		//	- generic instantiation
		//	- TODO: variance
		//	- TODO?: array indicator fixup ("T..." should match "T[]")
		public static bool IsImplementing (JavaMethodModel derived, JavaMethodModel basis, IDictionary<JavaTypeReference, JavaTypeReference> genericInstantiation)
		{
			if (genericInstantiation == null)
				throw new ArgumentNullException ("genericInstantiation");

			if (basis.Name != derived.Name)
				return false;

			if (basis.Parameters.Count != derived.Parameters.Count)
				return false;

			if (basis.Parameters.Zip (derived.Parameters, (bp, dp) => IsParameterAssignableTo (dp, bp, derived, basis, genericInstantiation)).All (v => v))
				return true;
			return false;
		}

		static bool IsParameterAssignableTo (JavaParameterModel dp, JavaParameterModel bp, JavaMethodModel derived, JavaMethodModel basis, IDictionary<JavaTypeReference, JavaTypeReference> genericInstantiation)
		{
			// If type names are equivalent, they simply match... except that the generic type parameter names match.
			// Generic type arguments need more check, so do not examine them just by name.
			//
			// FIXME: It is likely that this check should NOT result in "this method is not an override",
			// but rather like "this method is an override, but it should be still generated in the resulting XML".
			// For example, this results in that java.util.EnumMap#put() is NOT an override of
			// java.util.AbstractMap#put(), it is an override, not just that it is still generated in the XML.
			if (bp.TypeModel?.ReferencedTypeParameter != null && dp.TypeModel?.ReferencedTypeParameter != null &&
			    bp.TypeModel.ReferencedTypeParameter.ToString () != dp.TypeModel.ReferencedTypeParameter.ToString ())
				return false;
			if (bp.GenericType == dp.GenericType)
				return true;

			if (bp.TypeModel?.ArrayPart != bp.TypeModel?.ArrayPart)
				return false;

			// if base is type with generic type parameters and derived is without any generic args, that's OK.
			// java.lang.Class should match java.lang.Class<T>.
			if (bp.TypeModel?.ReferencedType != null && dp.TypeModel?.ReferencedType != null &&
			    bp.TypeModel?.ReferencedType.FullName == dp.TypeModel?.ReferencedType.FullName &&
			    dp.TypeModel?.TypeParameters == null)
				return true;

			// generic instantiation check.
			var baseGTP = bp.TypeModel?.ReferencedTypeParameter;
			if (baseGTP != null) {
				//if (baseGTP.Parent?.ParentMethod != null && IsConformantType (baseGTP, dp.TypeModel))
				//	return true;
				var k = genericInstantiation.Keys.FirstOrDefault (tr => bp.TypeModel?.Equals (tr) ?? false);
				if (k == null)
					// the specified generic type parameter is not part of
					// the mappings e.g. non-instantiated ones.
					return false;
				if (genericInstantiation [k].Equals (dp.TypeModel))
					// the specified generic type parameter exactly matches
					// whatever specified at the derived method.
					return true;
			}

			// FIXME: implement variance check.

			return false;
		}

		static bool IsConformantType (JavaTypeParameter typeParameter, JavaTypeReference? examinedType)
		{
			if (!typeParameter.GenericConstraints.Any ())
				return true;
			// FIXME: implement correct generic constraint conformance check.
			//Log.LogDebug ("NOTICE: generic constraint conformance check is not implemented, so the type might be actually compatible. Type parameter: {0}{1}, examined type: {2}",
			//		typeParameter.Name, typeParameter.Parent?.ParentMethod?.Name ?? typeParameter.Parent?.ParentType?.Name, examinedType);
			return false;
		}

		bool ParametersMatch (List<JavaParameterModel> other)
		{
			if (Parameters.Count != other.Count)
				return false;

			for (var i = 0; i < Parameters.Count; i++) {
				var para = GetParameterType (Parameters [i]);
				var base_para = GetParameterType (other [i]);

				if (para != base_para)
					return false;

				//if (Parameters [i].Type != other [i].Type)
				//	return false;
			}

			return true;
		}

		string GetParameterType (JavaParameterModel parameter)
		{
			var type_parameters = parameter.ParentMethod.GetApplicableTypeParameters ();

			if (type_parameters.FirstOrDefault (tp => tp.Name == parameter.TypeModel?.ReferencedTypeParameter?.Name) is JavaTypeParameter jtp) {
				return jtp.ExtendedClassBound ?? jtp.ExtendedInterfaceBounds ?? parameter.Type;
			}

			return parameter.Type;

		}

		public override string ToString () => "[Method] " + ToStringHelper (Return, Name, TypeParameters);

		// Content of this value is not stable.
		public string ToStringHelper (string? returnType, string? name, JavaTypeParameters? typeParameters)
		{
			return string.Format ("{0}{1}{2}{3}{4}{5}({6})",
				returnType,
				returnType == null ? null : " ",
				name,
				typeParameters?.Any () == true ? "<" : null,
				typeParameters?.Any () == true ? string.Join (", ", typeParameters) : null,
				typeParameters?.Any () == true ? ">" : null,
				string.Join (", ", Parameters));
		}
	}
}
