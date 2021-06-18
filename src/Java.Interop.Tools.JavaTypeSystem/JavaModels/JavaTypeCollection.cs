using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public class JavaTypeCollection
	{
		public Dictionary<string, JavaPackage> Packages { get; } = new Dictionary<string, JavaPackage> ();
		public Dictionary<string, JavaTypeModel> Types { get; } = new Dictionary<string, JavaTypeModel> ();
		public Dictionary<string, JavaTypeModel> TypesFlattened { get; } = new Dictionary<string, JavaTypeModel> ();
		public readonly Dictionary<string, JavaTypeModel> ReferenceTypes = new Dictionary<string, JavaTypeModel> ();
		public readonly Dictionary<string, JavaTypeModel> ReferenceTypesFlattened = new Dictionary<string, JavaTypeModel> ();
		readonly Dictionary<string, JavaTypeModel> built_in_types = new Dictionary<string, JavaTypeModel> ();

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

		public void Add (JavaPackage package)
		{
			Packages.Add (package.Name, package);

			foreach (var type in package.Types)
				Add (type);
		}

		public void Add (JavaTypeModel item)
		{
			var nested_name = item.NestedName;

			// Not a nested type
			if (!nested_name.Contains ('.')) {
				Types [item.FullName] = item;
				TypesFlattened [item.FullName] = item;

				return;
			}

			var full_name = item.FullName.ChompLast ('.');

			// Nested type, find parent model to put it in
			if (TypesFlattened.TryGetValue (full_name, out var parent)) {
				parent.NestedTypes.Add (item);
				item.ParentType = parent;
				TypesFlattened [item.FullName] = item;

				return;
			}

			// TODO: Probably want to log this
			//throw new Exception ();
		}

		public void AddReferenceType (JavaTypeModel item)
		{
			item.IsReferenceOnly = true;

			var nested_name = item.NestedName;

			// Not a nested type
			if (!nested_name.Contains ('.')) {
				ReferenceTypes [item.FullName] = item;
				ReferenceTypesFlattened [item.FullName] = item;

				return;
			}

			var full_name = item.FullName;

			// Nested type, find parent model to put it in
			while ((full_name = full_name.ChompLast ('.')).Length > 0) {
				if (ReferenceTypesFlattened.TryGetValue (full_name, out var parent)) {
					parent.NestedTypes.Add (item);
					item.ParentType = parent;
					ReferenceTypesFlattened [item.FullName] = item;

					return;
				}
			}

			throw new Exception ();
		}

		public void AddReferenceTypeRecursive (JavaTypeModel item)
		{
			item.IsReferenceOnly = true;

			// Only add non-nested types to ReferenceTypes
			if (item.ParentType is null)
				ReferenceTypes [item.FullName] = item;

			// Add all types to Flattened
			ReferenceTypesFlattened [item.FullName.Replace ('$', '.')] = item;

			foreach (var type in item.NestedTypes)
				AddReferenceTypeRecursive (type);
		}

		public void AddRange (IEnumerable<JavaTypeModel> types)
		{
			foreach (var type in types)
				Add (type);
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
			removed |= TypesFlattened.Remove (type.FullName);
			removed |= Types.Remove (type.FullName);

			return removed;
		}

		public JavaTypeModel? FindType (string? type)
		{
			if (!type.HasValue ())
				return null;

			if (built_in_types.TryGetValue (type, out var builtin))
				return builtin;

			if (TypesFlattened.TryGetValue (type, out var value))
				return value;

			if (ReferenceTypesFlattened.TryGetValue (type, out var ref_type))
				return ref_type;

			return null;
		}

		// declaringType is the optional class that contains this reference,
		// used for resolving generic type parameters.
		//public IJavaTypeReference? Resolve (string? javaName, JavaTypeModel? declaringType = null, JavaMethodModel? declaringMember = null)
		//{
		//	if (!javaName.HasValue ())
		//		return null;

		//	return Resolve (JavaTypeName.Parse (javaName), declaringType, declaringMember);
		//}

		//public IJavaTypeReference? Resolve (JavaTypeName type, JavaTypeModel? declaringType = null, JavaMethodModel? declaringMember = null)
		//{
		//	// Check for a generic type parameter like "T"
		//	if (declaringType?.TypeParameters.Any (p => p.Name == type.DottedName) == true || declaringMember?.TypeParameters.Any (p => p.Name == type.DottedName) == true) {
		//		var gtps = new JavaGenericTypeParameterReference (type.DottedName!);

		//		return type.ArrayPart == "..." ? new JavaArrayReference (gtps, true) : gtps;
		//	}

		//	// Handle the "?" generic type parameter as JLO
		//	if (type.DottedName == "?")
		//		return FindType ("java.lang.Object");

		//	// Resolve an array
		//	if (type.ArrayPart.HasValue ()) {
		//		var element_type = Resolve (type.DottedName, declaringType, declaringMember);
		//		var is_params = type.ArrayPart.Contains ("...");

		//		if (element_type is null)
		//			throw new Exception ();

		//		return new JavaArrayReference (element_type, is_params);
		//	}

		//	// Resolve a generic type
		//	if (type.GenericArguments?.Any () == true) {
		//		var generic_symbol = Resolve (type.DottedName, declaringType, declaringMember);

		//		if (generic_symbol is null)
		//			throw new Exception ();

		//		var arguments = new List<IJavaTypeReference> ();

		//		foreach (var a in type.GenericArguments) {
		//			var arg_symbol = Resolve (a, declaringType, declaringMember);

		//			if (arg_symbol is null)
		//				throw new Exception ();

		//			arguments.Add (arg_symbol);
		//		}

		//		var return_symbol = new JavaGenericReference (generic_symbol, arguments.ToArray ());

		//		return return_symbol;
		//	}

		//	// Hopefully just a simple type
		//	var symbol = FindType (type.DottedName);

		//	if (symbol is null)
		//		throw new Exception ();

		//	return symbol;
		//}

		public void ResolveCollection (TypeResolutionOptions? options = null)
		{
			options ??= TypeResolutionOptions.Default;

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

				// We may have removed a type that other types/members reference, so we have
				// to keep doing this until we do not remove any types.
				if (!types_removed)
					break;
			}

			// Fixing this here is the least disruptive way to add these abstract members
			//JavaInterfacesMustBeImplementedInAbstractTypesFixup.Fixup (this);
			//foreach (var klass in TypesFlattened.Values.OfType<JavaClassModel> ())
			//	klass.PrepareGenericInheritanceMapping ();

			foreach (var klass in TypesFlattened.Values.OfType<JavaClassModel> ()) {
				//if (klass.Name == "BaseDexClassLoader")
				//	Debugger.Break ();

				klass.ResolveBaseMembers ();
			}

			Console.WriteLine ();
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

		public JavaTypeReference ResolveTypeReference (string name, params JavaTypeParameter [] contextTypeParameters)
			=> ResolveTypeReference (JavaTypeName.Parse (name), contextTypeParameters);

		public JavaTypeReference ResolveTypeReference (JavaTypeName tn, params JavaTypeParameter [] contextTypeParameters)
		{
			var tp = contextTypeParameters
				.FirstOrDefault (xp => xp.Name == tn.DottedName);

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
	}
}
