using System;
using System.Collections.Generic;

using Mono.Cecil;

namespace Java.Interop.Tools.Cecil {

	public static class TypeDefinitionRocks {

		[Obsolete ("Use the TypeDefinitionCache overload for better performance.", error: true)]
		public static TypeDefinition? GetBaseType (this TypeDefinition type) => throw new NotSupportedException ();

		public static TypeDefinition? GetBaseType (this TypeDefinition type, TypeDefinitionCache cache) =>
			GetBaseType (type, (IMetadataResolver) cache);

		public static TypeDefinition? GetBaseType (this TypeDefinition type, IMetadataResolver resolver)
		{
			var bt = type.BaseType;
			if (bt == null)
				return null;
			return resolver.Resolve (bt);
		}

		[Obsolete ("Use the TypeDefinitionCache overload for better performance.", error: true)]
		public static IEnumerable<TypeDefinition> GetTypeAndBaseTypes (this TypeDefinition type) => throw new NotSupportedException ();

		public static IEnumerable<TypeDefinition> GetTypeAndBaseTypes (this TypeDefinition type, TypeDefinitionCache cache) =>
			GetTypeAndBaseTypes (type, (IMetadataResolver) cache);

		public static IEnumerable<TypeDefinition> GetTypeAndBaseTypes (this TypeDefinition type, IMetadataResolver resolver)
		{
			TypeDefinition? t = type;

			while (t != null) {
				yield return t;
				t = t.GetBaseType (resolver);
			}
		}

		[Obsolete ("Use the TypeDefinitionCache overload for better performance.", error: true)]
		public static IEnumerable<TypeDefinition> GetBaseTypes (this TypeDefinition type) => throw new NotSupportedException();

		public static IEnumerable<TypeDefinition> GetBaseTypes (this TypeDefinition type, TypeDefinitionCache cache) =>
			GetBaseTypes (type, (IMetadataResolver) cache);

		public static IEnumerable<TypeDefinition> GetBaseTypes (this TypeDefinition type, IMetadataResolver resolver)
		{
			TypeDefinition? t = type;

			while ((t = t.GetBaseType (resolver)) != null) {
				yield return t;
			}
		}

		[Obsolete ("Use the TypeDefinitionCache overload for better performance.", error: true)]
		public static bool IsAssignableFrom (this TypeReference type, TypeReference c) => throw new NotSupportedException ();

		public static bool IsAssignableFrom (this TypeReference type, TypeReference c, TypeDefinitionCache cache) =>
			IsAssignableFrom (type, c, (IMetadataResolver) cache);

		public static bool IsAssignableFrom (this TypeReference type, TypeReference c, IMetadataResolver resolver)
		{
			if (type.FullName == c.FullName)
				return true;
			var d = resolver.Resolve (c);
			if (d == null)
				return false;

			TypeDefinition? t = d;

			while (t is not null) {
				if (type.FullName == t.FullName)
					return true;
				foreach (var ifaceImpl in t.Interfaces) {
					var i   = ifaceImpl.InterfaceType;
					if (IsAssignableFrom (type, i, resolver))
						return true;
				}

				t = t.GetBaseType (resolver);
			}
			return false;
		}

		[Obsolete ("Use the TypeDefinitionCache overload for better performance.", error: true)]
		public static bool IsSubclassOf (this TypeDefinition type, string typeName) => throw new NotSupportedException ();

		public static bool IsSubclassOf (this TypeDefinition type, string typeName, TypeDefinitionCache cache) =>
			IsSubclassOf (type, typeName, (IMetadataResolver) cache);
		public static bool IsSubclassOf (this TypeDefinition type, string typeName, IMetadataResolver resolver)
		{
			TypeDefinition? t = type;

			while (t is not null) {
				if (t.FullName == typeName) {
					return true;
				}

				t = t.GetBaseType (resolver);
			}
			return false;
		}

		public static bool IsSubclassOfAny (this TypeDefinition type, IList<string> typeNames, IMetadataResolver resolver, out string? subclassType)
		{
			subclassType = null;

			TypeDefinition? t = type;

			while (t is not null) {
				if (typeNames.Contains (t.FullName)) {
					subclassType = t.FullName;
					return true;
				}

				t = t.GetBaseType (resolver);
			}

			return false;
		}

		public static bool HasJavaPeer (this TypeDefinition type, IMetadataResolver resolver)
		{
			if (type.IsInterface)
				return type.ImplementsInterface ("Java.Interop.IJavaPeerable", resolver);

			TypeDefinition? t = type;

			while (t is not null) {
				switch (t.FullName) {
					case "Java.Lang.Object":
					case "Java.Lang.Throwable":
					case "Java.Interop.JavaObject":
					case "Java.Interop.JavaException":
						return true;
					default:
						break;
				}

				t = t.GetBaseType (resolver);
			}
			return false;
		}

		[Obsolete ("Use the TypeDefinitionCache overload for better performance.", error: true)]
		public static bool ImplementsInterface (this TypeDefinition type, string interfaceName) => throw new NotSupportedException ();

		public static bool ImplementsInterface (this TypeDefinition type, string interfaceName, TypeDefinitionCache cache) =>
			ImplementsInterface (type, interfaceName, (IMetadataResolver) cache);

		public static bool ImplementsInterface (this TypeDefinition type, string interfaceName, IMetadataResolver resolver)
		{
			TypeDefinition? t = type;

			while (t is not null) {
				foreach (var i in t.Interfaces) {
					if (i.InterfaceType.FullName == interfaceName) {
						return true;
					}
				}

				t = t.GetBaseType (resolver);
			}
			return false;
		}

		public static bool ImplementsAnyInterface (this TypeDefinition type, IList<string> interfaceNames, IMetadataResolver resolver, out string? implementedInterface)
		{
			implementedInterface = null;
			TypeDefinition? t = type;

			while (t is not null) {
				foreach (var i in t.Interfaces) {
					if (interfaceNames.Contains (i.InterfaceType.FullName)) {
						implementedInterface = i.InterfaceType.FullName;
						return true;
					}
				}

				t = t.GetBaseType (resolver);
			}

			return false;
		}

		[Obsolete ("Use the TypeDefinitionCache overload for better performance.", error: true)]
		public static string GetPartialAssemblyName (this TypeReference type) => throw new NotSupportedException ();

		public static string GetPartialAssemblyName (this TypeReference type, TypeDefinitionCache cache) =>
			GetPartialAssemblyName (type, (IMetadataResolver) cache);

		public static string GetPartialAssemblyName (this TypeReference type, IMetadataResolver resolver)
		{
			TypeDefinition? def = resolver.Resolve (type);
			return (def ?? type).Module.Assembly.Name.Name;
		}

		[Obsolete ("Use the TypeDefinitionCache overload for better performance.", error: true)]
		public static string GetPartialAssemblyQualifiedName (this TypeReference type) => throw new NotSupportedException ();

		public static string GetPartialAssemblyQualifiedName (this TypeReference type, TypeDefinitionCache cache) =>
			GetPartialAssemblyQualifiedName (type, (IMetadataResolver) cache);

		public static string GetPartialAssemblyQualifiedName (this TypeReference type, IMetadataResolver resolver)
		{
			return string.Format ("{0}, {1}",
					// Cecil likes to use '/' as the nested type separator, while
					// Reflection uses '+' as the nested type separator. Use Reflection.
			                CecilTypeNameToReflectionTypeName (type.FullName),
					type.GetPartialAssemblyName (resolver));
		}

		[Obsolete ("Use the TypeDefinitionCache overload for better performance.", error: true)]
		public static string GetAssemblyQualifiedName (this TypeReference type) => throw new NotSupportedException ();

		public static string GetAssemblyQualifiedName (this TypeReference type, TypeDefinitionCache cache) =>
			GetAssemblyQualifiedName (type, (IMetadataResolver) cache);

		public static string GetAssemblyQualifiedName (this TypeReference type, IMetadataResolver resolver)
		{
			TypeDefinition? def = resolver.Resolve (type);
			return string.Format ("{0}, {1}",
					// Cecil likes to use '/' as the nested type separator, while
					// Reflection uses '+' as the nested type separator. Use Reflection.
			                CecilTypeNameToReflectionTypeName (type.FullName),
					(def ?? type).Module.Assembly.Name.FullName);
		}

		public static TypeDefinition? GetNestedType (this TypeDefinition type, string name)
		{
			if (type == null)
				return null;

			foreach (TypeDefinition t in type.NestedTypes)
				if (t.Name == name || t.FullName == name)
					return t;

			return null;
		}

		// Note: this is not recursive, so it will not find nested types.
		public static TypeDefinition? FindType (this ModuleDefinition module, string name)
		{
			if (module == null)
				return null;

			foreach (TypeDefinition t in module.Types)
				if (t.Name == name || t.FullName == name)
					return t;

			return null;
		}

		public static string? CecilTypeNameToReflectionTypeName (string? typeName) => typeName?.Replace ('/', '+');
	}
}
