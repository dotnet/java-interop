using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public class JavaClassModel : JavaTypeModel
	{
		public string BaseType { get; }
		public string BaseTypeGeneric { get; }
		public string BaseTypeJni { get; }

		public JavaTypeReference? BaseTypeReference { get; private set; }
		public List<JavaConstructorModel> Constructors { get; } = new List<JavaConstructorModel> ();

		public JavaClassModel (JavaPackage javaPackage, string javaNestedName, string javaVisibility, bool javaAbstract, bool javaFinal, string javaBaseType, string javaBaseTypeGeneric, string javaDeprecated, bool javaStatic, string jniSignature, string baseTypeJni) :
			base (javaPackage, javaNestedName, javaVisibility, javaAbstract, javaFinal, javaDeprecated, javaStatic, jniSignature)
		{
			BaseType = javaBaseType;
			BaseTypeGeneric = javaBaseTypeGeneric;
			BaseTypeJni = baseTypeJni;
		}

		public override void Resolve (JavaTypeCollection types, List<JavaUnresolvableModel> unresolvables)
		{
			var type_parameters = GetApplicableTypeParameters ().ToArray ();

			// Resolve base class
			if (FullName != "java.lang.Object") {
				//var symbol = types.Resolve (JavaTypeReferenceExtensions.ResolveGenerics ? BaseTypeGeneric : BaseType, this);

				try {
					BaseTypeReference = types.ResolveTypeReference (JavaTypeReferenceExtensions.ResolveGenerics ? BaseTypeGeneric : BaseType, type_parameters);
				} catch (JavaTypeResolutionException) {
					unresolvables.Add (new JavaUnresolvableModel (this, BaseTypeGeneric));

					throw;
				}

				// We don't resolve reference types by default, we only resolve the ones that are
				// actually used as base classes, which we trigger here.
				try {
					if (BaseTypeReference.ReferencedType is JavaClassModel klass && klass.FullName != "java.lang.Object" && klass.BaseTypeReference is null && klass.IsReferenceOnly)
						klass.Resolve (types, unresolvables);
				} catch (JavaTypeResolutionException) {
					// Ignore
				}
				//if (BaseTypeReference is null)
				//	throw new Exception ();
			}

			// Resolve constructors
			foreach (var ctor in Constructors)
				ctor.Resolve (types, unresolvables);

			base.Resolve (types, unresolvables);
		}

		public virtual void ResolveBaseMembers ()
		{
			var klass = BaseTypeReference?.ReferencedType as JavaClassModel;

			foreach (var method in Methods)
				method.FindBaseMethod (klass);

			foreach (var nested in NestedTypes.OfType<JavaClassModel> ())
				nested.ResolveBaseMembers ();
		}

		public IDictionary<JavaTypeReference, JavaTypeReference>? GenericInheritanceMapping { get; set; }

		public void PrepareGenericInheritanceMapping ()
		{
			if (GenericInheritanceMapping != null)
				return; // already done.

			var empty = new Dictionary<JavaTypeReference, JavaTypeReference> ();

			var bt = BaseTypeReference == null ? null : BaseTypeReference.ReferencedType as JavaClassModel;
			if (bt == null)
				GenericInheritanceMapping = new Dictionary<JavaTypeReference, JavaTypeReference> (); // empty
			else {
				// begin processing from the base class.
				bt.PrepareGenericInheritanceMapping ();

				if (BaseTypeReference?.TypeParameters == null)
					GenericInheritanceMapping = empty;
				else if (BaseTypeReference?.ReferencedType is null || BaseTypeReference?.ReferencedType?.TypeParameters.Count == 0) {
					// FIXME: I guess this should not happen. But this still happens.
					//Log.LogWarning ("Warning: '{0}' is referenced as base type of '{1}' and expected to have generic type parameters, but it does not.", cls.ExtendsGeneric, cls.FullName);
					GenericInheritanceMapping = empty;
				} else {
					if (BaseTypeReference.ReferencedType.TypeParameters.Count != BaseTypeReference.TypeParameters.Count)
						throw new Exception (string.Format ("On {0}.{1}, referenced generic arguments count do not match the base type parameters definition",
							ParentType?.Name, Name));
					var dic = empty;
					foreach (var kvp in BaseTypeReference.ReferencedType.TypeParameters.Zip (
						 BaseTypeReference.TypeParameters,
						 (def, use) => new KeyValuePair<JavaTypeParameter, JavaTypeReference> (def, use))
						 .Where (p => p.Value.ReferencedTypeParameter == null || p.Key.Name != p.Value.ReferencedTypeParameter.Name))
						dic.Add (new JavaTypeReference (kvp.Key, null), kvp.Value);
					if (dic.Any ()) {
						GenericInheritanceMapping = dic;
					} else
						GenericInheritanceMapping = empty;
				}
			}
		}

		public override string ToString () => $"[Class] {FullName}";
	}
}
