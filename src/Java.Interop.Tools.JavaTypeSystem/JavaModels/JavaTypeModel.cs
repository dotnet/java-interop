using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public abstract class JavaTypeModel : IJavaTypeReference, IJavaResolvable
	{
		/// <summary>
		/// Only the type's name, does not include parent type name for nested type.
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// Includes parent type name(s) if type is nested (period separator). ex: Manifest.permission
		/// </summary>
		public string NestedName { get; set;}
		public string Visibility { get; }
		public bool IsAbstract { get; }
		public bool IsFinal { get; }
		public string Deprecated { get; }
		public bool IsStatic { get; }
		public string ExtendedJniSignature { get; }

		public JavaPackage Package { get; }
		public JavaTypeModel? ParentType { get; internal set; }
		public List<JavaTypeModel> NestedTypes { get; } = new List<JavaTypeModel> ();

		public JavaTypeParameters TypeParameters { get; set; } = new JavaTypeParameters ();
		public List<JavaImplementsModel> Implements { get; } = new List<JavaImplementsModel> ();
		public List<JavaTypeReference> ImplementsModels { get; } = new List<JavaTypeReference> ();
		public List<JavaMethodModel> Methods { get; } = new List<JavaMethodModel> ();
		public List<JavaFieldModel> Fields { get; } = new List<JavaFieldModel> ();

		public Dictionary<string, string> PropertyBag { get; } = new Dictionary<string, string> ();

		public JavaTypeModel RootType => this;
		public bool IsReferenceOnly { get; internal set; }

		//public BoundTypeModel? ManagedModel { get; set; }

		protected JavaTypeModel (JavaPackage javaPackage, string javaNestedName, string javaVisibility, bool javaAbstract, bool javaFinal, string deprecated, bool javaStatic, string jniSignature)
		{
			Package = javaPackage;
			NestedName = javaNestedName.Replace ('$', '.');
			Name = NestedName.LastSubset ('.');
			Visibility = javaVisibility;
			IsAbstract = javaAbstract;
			IsFinal = javaFinal;
			Deprecated = deprecated;
			IsStatic = javaStatic;
			ExtendedJniSignature = jniSignature;
		}

		public string FullName {
			get {
				if (Name == "boolean")
					return "bool";

				if (ParentType != null)
					return $"{ParentType.FullName}.{Name}";

				if (Package.Name.Length > 0)
					return $"{Package.Name}.{NestedName}";

				return Name;
			}
		}

		public string FullNameWithDollarSign {
			get {
				if (Name == "boolean")
					return "bool";

				if (ParentType != null)
					return $"{ParentType.FullName}${Name}";

				if (Package.Name.Length > 0)
					return $"{Package.Name}.{NestedName.Replace ('.', '$')}";

				return Name;
			}
		}

		public virtual void Resolve (JavaTypeCollection types, List<JavaUnresolvableModel> unresolvables)
		{
			var type_parameters = GetApplicableTypeParameters ().ToArray ();

			// Resolve any implemented interfaces
			foreach (var i in Implements) {
				//var implements = types.Resolve (JavaTypeReferenceExtensions.ResolveGenerics ? i.NameGeneric : i.Name, this);
				try {
					var implements = types.ResolveTypeReference (JavaTypeReferenceExtensions.ResolveGenerics ? i.NameGeneric : i.Name, type_parameters);

					if (implements is null)
						throw new Exception ();

					ImplementsModels.Add (implements);
				} catch (JavaTypeResolutionException) {
					unresolvables.Add (new JavaUnresolvableModel (this, i.NameGeneric));

					throw;
				}
			}

			// Resolve members
			foreach (var method in Methods)
				method.Resolve (types, unresolvables);

			foreach (var field in Fields)
				field.Resolve (types, unresolvables);

			// Resolve nested types
			foreach (var child in NestedTypes)
				child.Resolve (types, unresolvables);
		}

		// Return type's type parameters, plus type parameters for any types this is nested in.
		public IEnumerable<JavaTypeParameter> GetApplicableTypeParameters ()
		{
			foreach (var jtp in TypeParameters)
				yield return jtp;

			// TODO
			yield break;
			if (ParentType != null)
				foreach (var jtp in ParentType.GetApplicableTypeParameters ())
					yield return jtp;
		}
	}
}
