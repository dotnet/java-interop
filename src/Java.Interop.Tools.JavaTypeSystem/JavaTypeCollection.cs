using System;
using System.Collections.Generic;
using System.Linq;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public class JavaTypeCollection
	{
		readonly Dictionary<string, JavaPackage> packages = new Dictionary<string, JavaPackage> ();
		readonly Dictionary<string, JavaTypeModel> types = new Dictionary<string, JavaTypeModel> ();
		readonly Dictionary<string, JavaTypeModel> types_flattened = new Dictionary<string, JavaTypeModel> ();
		readonly Dictionary<string, JavaTypeModel> reference_types = new Dictionary<string, JavaTypeModel> ();
		readonly Dictionary<string, JavaTypeModel> reference_types_flattened = new Dictionary<string, JavaTypeModel> ();
		readonly Dictionary<string, JavaTypeModel> built_in_types = new Dictionary<string, JavaTypeModel> ();

		// Expose ReadOnly versions so internal type management cannot be bypassed
		public IReadOnlyDictionary<string, JavaPackage> Packages => packages;
		public IReadOnlyDictionary<string, JavaTypeModel> Types => types;
		public IReadOnlyDictionary<string, JavaTypeModel> ReferenceTypes => reference_types;

		public IReadOnlyDictionary<string, JavaTypeModel> TypesFlattened => types_flattened;
		public IReadOnlyDictionary<string, JavaTypeModel> ReferenceTypesFlattened => reference_types_flattened;

		public string? ApiSource { get; set; }
		public string? Platform { get; set; }

		public JavaTypeCollection ()
		{
			built_in_types.Add ("void", new JavaBuiltInType ("void"));
			built_in_types.Add ("boolean", new JavaBuiltInType ("boolean"));
			built_in_types.Add ("int", new JavaBuiltInType ("int"));
			built_in_types.Add ("byte", new JavaBuiltInType ("byte"));
			built_in_types.Add ("double", new JavaBuiltInType ("double"));
			built_in_types.Add ("float", new JavaBuiltInType ("float"));
			built_in_types.Add ("long", new JavaBuiltInType ("long"));
			built_in_types.Add ("short", new JavaBuiltInType ("short"));
			built_in_types.Add ("char", new JavaBuiltInType ("char"));
		}

		/// <summary>
		/// Adds a new package with the specified name.  Note if package already exists, existing package will be returned.
		/// </summary>
		public JavaPackage AddPackage (string name, string jniName, string? managedName = null)
		{
			if (packages.TryGetValue (name, out var pkg))
				return pkg;

			var new_pkg = new JavaPackage (name, jniName, managedName);

			packages.Add (new_pkg.Name, new_pkg);

			return new_pkg;
		}

		/// <summary>
		/// Adds a type to the collection.  Note parent classes must be added before nested classes.
		/// </summary>
		/// <returns>True if type was added to collection. False if type could not be added because its parent type was missing.</returns>
		public bool AddType (JavaTypeModel type) => AddType (type, types, types_flattened);

		/// <summary>
		/// Adds a reference type to the collection.  Note parent classes must be added before nested classes.
		/// </summary>
		/// <returns>True if type was added to collection. False if type could not be added because its parent type was missing.</returns>
		public bool AddReferenceType (JavaTypeModel type)
		{
			type.IsReferenceOnly = true;

			return AddType (type, reference_types, reference_types_flattened);
		}

		bool AddType (JavaTypeModel type, Dictionary<string, JavaTypeModel> typeDictionary, Dictionary<string, JavaTypeModel> flattenedDictionary)
		{
			var nested_name = type.NestedName;

			// Not a nested type
			if (!nested_name.Contains ('.')) {
				typeDictionary [type.FullName] = type;
				flattenedDictionary [type.FullName] = type;

				return true;
			}

			var full_name = type.FullName.ChompLast ('.');

			// Nested type, find parent model to put it in
			if (flattenedDictionary.TryGetValue (full_name, out var parent)) {
				if (!parent.NestedTypes.Contains (type))
					parent.NestedTypes.Add (type);

				type.ParentType = parent;
				flattenedDictionary [type.FullName] = type;

				return true;
			}

			// Could not find parent type to nest child type in
			return false;
		}

		// This is a little trickier than we may initially think, because nested classes
		// will also need to be removed from TypesFlattened (recursively). Note this only
		// removes the type from this collection, it does not remove a nested type from
		// its parent type model. Returns true if type(s) were removed. 
		public bool Remove (JavaTypeModel type)
		{
			var removed = false;

			// Remove all nested types
			foreach (var nested in type.NestedTypes)
				removed |= Remove (nested);

			// Remove ourselves
			removed |= types_flattened.Remove (type.FullName);
			removed |= types.Remove (type.FullName);

			return removed;
		}

		/// <summary>
		/// Ensures all types needed by the binding types can be found. Removes members or types
		/// that need types that cannot be found.
		/// </summary>
		public CollectionResolutionResults ResolveCollection (TypeResolutionOptions? options = null)
		{
			options ??= TypeResolutionOptions.Default;

			var results = new CollectionResolutionResults ();

			while (true) {
				var unresolvables = new List<JavaUnresolvableModel> ();
				var types_removed = false;

				foreach (var t in Types)
					try {
						t.Value.Resolve (this, unresolvables);
					} catch (JavaTypeResolutionException) {
					}

				foreach (var u in unresolvables) {
					if (u.Unresolvable is JavaTypeModel type) {
						types_removed |= RemoveType (type);
					} else if (u.Unresolvable is JavaConstructorModel ctor) {
						// Remove from parent type (must pattern check for ctor before method)
						((JavaClassModel) ctor.ParentType).Constructors.Remove (ctor);
					} else if (u.Unresolvable is JavaMethodModel method) {
						// Remove from parent type
						types_removed |= RemoveMethod (method, options);
					} else if (u.Unresolvable is JavaFieldModel field) {
						// Remove from parent type
						field.ParentType.Fields.Remove (field);
					} else if (u.Unresolvable is JavaParameterModel parameter) {
						// Remove method from parent type
						types_removed |= RemoveMethod (parameter.ParentMethod, options);
					} else {
						// *Shouldn't* be possible
						throw new Exception ($"Encountered unknown IJavaResolvable: '{u.Unresolvable.GetType ().Name}'");
					}
				}

				results.Add (new CollectionResolutionResult (unresolvables));

				// We may have removed a type that other types/members reference, so we have
				// to keep doing this until we do not remove any types.
				if (!types_removed)
					break;
			}

			// Once we have resolved all base classes we can resolve class members
			foreach (var klass in TypesFlattened.Values.OfType<JavaClassModel> ())
				klass.ResolveBaseMembers ();

			return results;
		}

		public JavaTypeReference ResolveTypeReference (string name, params JavaTypeParameter [] contextTypeParameters)
			=> ResolveTypeReference (JavaTypeName.Parse (name), contextTypeParameters);

		public JavaTypeReference ResolveTypeReference (JavaTypeName tn, params JavaTypeParameter [] contextTypeParameters)
		{
			var tp = contextTypeParameters.FirstOrDefault (xp => xp.Name == tn.DottedName);

			if (tp != null)
				return new JavaTypeReference (tp, tn.ArrayPart);

			if (tn.DottedName == JavaTypeReference.GenericWildcard.SpecialName)
				return new JavaTypeReference (tn.BoundsType, tn.GenericConstraints?.Select (gc => ResolveTypeReference (gc, contextTypeParameters)), tn.ArrayPart);

			var primitive = JavaTypeReference.GetSpecialType (tn.DottedName);

			if (primitive != null)
				return tn.ArrayPart == null && tn.GenericConstraints == null ? primitive : new JavaTypeReference (primitive, tn.ArrayPart, tn.BoundsType, tn.GenericConstraints?.Select (gc => ResolveTypeReference (gc, contextTypeParameters)));

			var type = FindType (tn.FullNameNonGeneric);

			if (type is null)
				throw new JavaTypeResolutionException (tn.FullNameNonGeneric);

			return new JavaTypeReference (type,
				tn.GenericArguments != null ? tn.GenericArguments.Select (_ => ResolveTypeReference (_, contextTypeParameters)).ToArray () : null,
				tn.ArrayPart);
		}

		// Returns true if a type was removed.
		bool RemoveMethod (JavaMethodModel method, TypeResolutionOptions options)
		{
			// We cannot remove a non-static, non-default method on an interface without breaking the contract.
			// If we need to do that we have to remove the entire interface instead.
			if (method.ParentType is JavaInterfaceModel && !method.IsStatic && method.IsAbstract && options.RemoveInterfacesWithUnresolvableMembers)
				return RemoveType (method.ParentType);

			if (method is JavaConstructorModel ctor && method.ParentType is JavaClassModel klass)
				klass.Constructors.Remove (ctor);
			else
				method.ParentType.Methods.Remove (method);

			return false;
		}

		bool RemoveType (JavaTypeModel type)
		{
			var removed = false;

			// Remove from parent type
			if (type.ParentType != null)
				removed |= type.ParentType.NestedTypes.Remove (type);

			// Remove from collection
			removed |= Remove (type);

			// Remove from parent package
			type.Package.Types.Remove (type);

			return removed;
		}

		JavaTypeModel? FindType (string type)
		{
			// Prefer built-in types
			if (built_in_types.TryGetValue (type, out var builtin))
				return builtin;

			// Then binding types
			if (TypesFlattened.TryGetValue (type, out var value))
				return value;

			// Finally reference types
			if (ReferenceTypesFlattened.TryGetValue (type, out var ref_type))
				return ref_type;

			return null;
		}
	}
}
