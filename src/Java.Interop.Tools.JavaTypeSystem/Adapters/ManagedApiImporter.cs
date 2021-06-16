using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Java.Interop.Tools.Cecil;
using Java.Interop.Tools.JavaTypeSystem.Models;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace Java.Interop.Tools.JavaTypeSystem
{
	public static class ManagedApiImporter
	{
		public static JavaTypeCollection Parse (AssemblyDefinition assembly, JavaTypeCollection collection)
		{
			foreach (var md in assembly.Modules)
				foreach (var td in md.Types) {
					// Currently we do not support generic types because they conflict.
					// ex: AdapterView`1 and AdapterView both have:
					// [Register ("android/widget/AdapterView")]
					// So we do not import the generic type if we also find a non-generic type.
					var non_generic_type = td.HasGenericParameters
						? md.GetType (td.FullName.Substring (0, td.FullName.IndexOf ('`')))
						: null;

					if (ShouldSkipGeneric (td, non_generic_type, null))
						continue;

					if (ParseType (td, collection) is JavaTypeModel type)
						collection.AddReferenceTypeRecursive (type);
				}

			return collection;
		}

		static bool ShouldSkipGeneric (TypeDefinition? a, TypeDefinition? b, TypeDefinitionCache? cache)
		{
			if (a == null || b == null)
				return false;
			if (!a.ImplementsInterface ("Android.Runtime.IJavaObject", cache) || !b.ImplementsInterface ("Android.Runtime.IJavaObject", cache))
				return false;

			return GetRegisteredJavaTypeName (a) == GetRegisteredJavaTypeName (b);
			//return JavaNativeTypeManager.ToJniName (a) == JavaNativeTypeManager.ToJniName (b);
		}

		public static JavaTypeModel? ParseType (TypeDefinition type, JavaTypeCollection collection)
		{
			if (!type.IsPublic && !type.IsNested)
				return null;

			if (!ShouldImport (type))
				return null;

			var model = type.IsInterface ? (JavaTypeModel?) ParseInterface (type, collection) : ParseClass (type, collection);

			if (model is null)
				return null;

			foreach (var nested in type.NestedTypes)
				if (ParseType (nested, collection) is JavaTypeModel nested_model)
					model.NestedTypes.Add (nested_model);

			return model;
		}

		static bool ShouldImport (TypeDefinition td)
		{
			// We want to exclude "IBlahInvoker" types from this type registration.
			if (td.Name.EndsWith ("Invoker")) {
				string n = td.FullName;
				n = n.Substring (0, n.Length - 7);
				var types = td.DeclaringType != null ? td.DeclaringType.Resolve ().NestedTypes : td.Module.Types;
				if (types.Any (t => t.FullName == n))
					return false;
				//Console.Error.WriteLine ("WARNING: " + td.FullName + " survived");
			}

			if (td.Name.EndsWith ("Implementor")) {
				string n = td.FullName;
				n = n.Substring (0, n.Length - 11);
				var types = td.DeclaringType != null ? td.DeclaringType.Resolve ().NestedTypes : td.Module.Types;
				if (types.Any (t => t.FullName == n))
					return false;
				//Console.Error.WriteLine ("WARNING: " + td.FullName + " survived");
			}

			return true;
		}

		public static JavaClassModel? ParseClass (TypeDefinition type, JavaTypeCollection collection)
		{
			// TODO: type parameters?
			var obs_attr = GetObsoleteAttribute (type.CustomAttributes);
			var reg_attr = GetRegisterAttribute (type.CustomAttributes);

			if (reg_attr is null)
				return null;

			var encoded_fullname = ((string) reg_attr.ConstructorArguments [0].Value).Replace ('/', '.');
			var (package, nested_name) = DecodeRegisterJavaFullName (encoded_fullname);

			var base_jni = GetBaseTypeJni (type);

			var model = new JavaClassModel (
				javaPackage: GetOrCreatePackage (collection, package, type.Namespace),
				javaNestedName: nested_name,
				javaVisibility: type.IsPublic || type.IsNestedPublic ? "public" : "protected internal",
				javaAbstract: type.IsAbstract,
				javaFinal: type.IsSealed,
				javaBaseType: base_jni.Replace ('/', '.').Replace ('$', '.'),
				javaBaseTypeGeneric: base_jni.Replace ('/', '.').Replace ('$', '.'),
				javaDeprecated: obs_attr != null ? "deprecated" : "not-deprecated",
				javaStatic: false,
				jniSignature: FormatJniSignature (package, nested_name),
				baseTypeJni: base_jni.HasValue () ? $"L{base_jni};" : string.Empty
			); ;

			ParseImplementedInterfaces (type, model);

			foreach (var method in type.Methods.Where (m => !m.IsConstructor))
				if (ParseMethod (method, model) is JavaMethodModel m)
					model.Methods.Add (m);

			return model;
		}

		public static JavaInterfaceModel? ParseInterface (TypeDefinition type, JavaTypeCollection collection)
		{
			// TODO: type paramters?
			var obs_attr = GetObsoleteAttribute (type.CustomAttributes);
			var reg_attr = GetRegisterAttribute (type.CustomAttributes);

			if (reg_attr is null)
				return null;

			var encoded_fullname = ((string) reg_attr.ConstructorArguments [0].Value).Replace ('/', '.');
			var (package, nested_name) = DecodeRegisterJavaFullName (encoded_fullname);

			var model = new JavaInterfaceModel (
				javaPackage: GetOrCreatePackage (collection, package, type.Namespace),
				javaNestedName: nested_name,
				javaVisibility: type.IsPublic || type.IsNestedPublic ? "public" : "protected internal",
				javaDeprecated: obs_attr != null ? "deprecated" : "not-deprecated",
				javaStatic: false,
				jniSignature: FormatJniSignature (package, nested_name)
			);

			ParseImplementedInterfaces (type, model);

			foreach (var method in type.Methods)
				if (ParseMethod (method, model) is JavaMethodModel m)
					model.Methods.Add (m);

			return model;
		}

		public static JavaMethodModel? ParseMethod (MethodDefinition method, JavaTypeModel parent)
		{
			//if (parent.FullName.StartsWith ("android.hardware.biometrics.BiometricPrompt"))
			//	Debugger.Break ();
			if (method.IsPrivate || method.IsAssembly)
				return null;

			var obs_attr = GetObsoleteAttribute (method.CustomAttributes);
			var reg_attr = GetRegisterAttribute (method.CustomAttributes);

			if (reg_attr is null)
				return null;

			var deprecated = (obs_attr != null) ? (string) obs_attr.ConstructorArguments [0].Value ?? "deprecated" : "not deprecated";
			var jni_signature = JniSignature.Parse ((string) reg_attr.ConstructorArguments [1].Value!);

			var model = new JavaMethodModel (
				javaName: (string) reg_attr.ConstructorArguments [0].Value,
				javaVisibility: method.Visibility (),
				javaAbstract: method.IsAbstract,
				javaFinal: method.IsFinal,
				javaStatic: method.IsStatic,
				javaReturn: jni_signature.Return.Type,
				javaParentType: parent,
				deprecated: deprecated,
				jniSignature: jni_signature.ToString (),
				isSynthetic: false,
				isBridge: false,
				returnJni: jni_signature.Return.Jni,
				isNative: false,
				isSynchronized: false,
				returnNotNull: false
			);

			for (var i = 0; i < jni_signature.Parameters.Count; i++)
				model.Parameters.Add (ParseParameterModel (model, jni_signature.Parameters [i], method.Parameters [i]));
			//var index = 0;

			//foreach (var p in jni_signature.Parameters) {
			//	var name = method.Parameters [index++].Name;
			//	model.Parameters.Add (new JavaParameterModel (model, name, p.Type, p.Jni, false));
			//}

			return model;
		}

		static JavaParameterModel ParseParameterModel (JavaMethodModel parent, JniTypeName jniParameter, ParameterDefinition managedParameter)
		{
			var raw_type = jniParameter.Type;

			// This covers a special case where we have generated an interface method like:
			// void DoThing (System.Collections.Generics.IList<MyPackage.MyType> p0);
			// However the [Register] JNI signature for this method is missing generic information:
			// "(Ljava/util/List;)V"
			// We need to try to rebuild the generic signature from the [Register] attributes
			// on the type components that make up the signature:
			// java.util.List<mypackage.MyType>
			if (managedParameter.ParameterType is GenericInstanceType)
				if (TypeReferenceToJavaType (managedParameter.ParameterType) is string s)
					raw_type = s;

			return new JavaParameterModel (parent, managedParameter.Name, raw_type, jniParameter.Jni, false);
		}

		static string? TypeReferenceToJavaType (TypeReference type)
		{
			var retval = GetRegisteredJavaName (type);

			if (retval != null && type is GenericInstanceType generic) {
				var parameters = generic.GenericArguments.Select (ga => GetRegisteredJavaName (ga.Resolve ())).ToArray ();

				if (parameters.WhereNotNull ().Any ())
					retval += $"<{string.Join (", ", parameters.WhereNotNull ())}>";
			}

			return retval;
		}

		static string? GetRegisteredJavaName (TypeReference type)
		{
			var td = type.Resolve ();

			return GetRegisteredJavaTypeName (td);
		}

		static void ParseImplementedInterfaces (TypeDefinition type, JavaTypeModel model)
		{
			foreach (var iface_impl in type.Interfaces) {
				var iface = iface_impl.InterfaceType;
				var iface_def = iface.Resolve ();

				if (iface_def is null || iface_def.IsNotPublic)
					continue;

				if (GetRegisterAttribute (iface_def.CustomAttributes) is CustomAttribute reg_attr) {
					var jni = (string) reg_attr.ConstructorArguments [0].Value;
					var name = jni.Replace ('/', '.').Replace ('$', '.');

					model.Implements.Add (new JavaImplementsModel (name, name, $"L{jni};"));
				}
			}
		}

		static string GetBaseTypeJni (TypeDefinition type)
		{
			// Find a Java base type, ignoring generic types, if nothing else it will be Java.Lang.Object
			TypeDefinition? base_type = type;

			while (true) {
				base_type = base_type.BaseType?.Resolve ();

				if (base_type is null)
					break;

				// These are the base types for Java.Lang.Object and Java.Lang.Throwable
				if (base_type.FullName == "System.Object" || base_type.FullName == "System.Exception")
					return string.Empty;

				if (base_type.HasGenericParameters || base_type.IsGenericInstance)
					continue;

				if (GetRegisterAttribute (base_type.CustomAttributes) is CustomAttribute reg_attr)
					return (string) reg_attr.ConstructorArguments [0].Value;
			}

			return "java/lang/Object";
		}

		static CustomAttribute? GetObsoleteAttribute (Collection<CustomAttribute> attributes) =>
			attributes.FirstOrDefault (a => a.AttributeType.FullNameCorrected () == "System.ObsoleteAttribute");

		static CustomAttribute? GetRegisterAttribute (Collection<CustomAttribute> attributes) =>
			attributes.FirstOrDefault (a => a.AttributeType.FullNameCorrected () == "Android.Runtime.RegisterAttribute");

		static string? GetRegisteredJavaTypeName (TypeDefinition type)
		{
			if (GetSpecialCase (type) is string s)
				return s;

			if (GetRegisterAttribute (type.CustomAttributes) is CustomAttribute reg_attr)
				return ((string) reg_attr.ConstructorArguments [0].Value).Replace ('/', '.');

			return null;
		}

		static string? GetSpecialCase (TypeDefinition type)
		{
			switch (type.FullName) {
				case "System.Collections.Generic.IList`1":
					return "java.util.List";
				case "System.Collections.Generic.IDictionary`2":
					return "java.util.Map";
				case "System.Collections.Generic.ICollection`1":
					return "java.util.Collection";
				case "System.String":
					return "java.lang.String";
				default:
					return null;
			}
		}

		static string FullNameCorrected (this TypeReference t) => t.FullName.Replace ('/', '.');

		public static string Visibility (this MethodDefinition m) =>
	m.IsPublic ? "public" : m.IsFamilyOrAssembly ? "protected internal" : m.IsFamily ? "protected" : m.IsAssembly ? "internal" : "private";

		static (string package, string nestedName) DecodeRegisterJavaFullName (string value)
		{
			var idx = value.LastIndexOf ('.');

			var package = idx >= 0 ? value.Substring (0, idx) : string.Empty;
			var nested_name = idx >= 0 ? value.Substring (idx + 1) : value;

			return (package, nested_name);
		}

		static string FormatJniSignature (string package, string nestedName)
		{
			if (package.HasValue ())
				return $"L{package.Replace ('.', '/')}/{nestedName.Replace ('$', '/')};";

			return "L" + nestedName.Replace ('$', '/') + ";";
		}

		static JavaPackage GetOrCreatePackage (JavaTypeCollection collection, string package, string managedName)
		{
			if (collection.Packages.TryGetValue (package, out var pkg))
				return pkg;

			pkg = new JavaPackage (
				name: package,
				jniName: package.Replace ('.', '/'),
				managedName: managedName
			);

			collection.Packages.Add (pkg.Name, pkg);

			return pkg;
		}
	}
}
