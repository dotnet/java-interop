using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Java.Interop.Tools.JavaTypeSystem.Models;

namespace Java.Interop.Tools.JavaTypeSystem
{
	public class JavaXmlApiImporter
	{
		public static JavaTypeCollection Parse (string filename)
		{
			var collection = new JavaTypeCollection ();

			return Parse (filename, collection);
		}

		public static JavaTypeCollection Parse (string filename, JavaTypeCollection collection)
		{
			var doc = XDocument.Load (filename, LoadOptions.SetBaseUri | LoadOptions.SetLineInfo);
			var root = doc.Root;

			if (root is null)
				throw new Exception ();

			var packages = new List<JavaPackage> ();

			foreach (var elem in root.Elements ()) {
				switch (elem.Name.LocalName) {
					case "package":
						packages.Add (ParsePackage (elem));
						break;
				}
			}

			foreach (var pkg in packages)
				collection.Packages.Add (pkg.Name, pkg);

			// First add all non-nested types
			foreach (var type in packages.SelectMany (p => p.Types).Where (t => !t.NestedName.Contains ('.')))
				collection.Add (type);

			// Add all nested types
			// This needs to be done ordered from least nested to most nested, in order for nesting to work.
			// That is, 'android.foo.blah' needs to be added before 'android.foo.blah.bar'.
			foreach (var type in packages.SelectMany (p => p.Types).Where (t => t.NestedName.Contains ('.')).OrderBy (t => t.FullName.Count (c => c == '.')).ToArray ()) {
				collection.Add (type);

				// Remove nested types from Package
				type.Package.Types.Remove (type);
			}

			// Remove any package-private classes
			//foreach (var klass in collection.Types.Values.OfType<JavaClassModel> ())
			//	RemovePackagePrivateClasses (collection, klass);

			return collection;
		}

		static void RemovePackagePrivateClasses (JavaTypeCollection types, JavaClassModel klass)
		{
			if (!klass.Visibility.HasValue ()) {
				RemoveTypeAndChildren (types, klass);
				return;
			}

			foreach (var child in klass.NestedTypes.OfType<JavaClassModel> ())
				RemovePackagePrivateClasses (types, child);
		}

		static void RemoveTypeAndChildren (JavaTypeCollection types, JavaTypeModel type)
		{
			foreach (var child in type.NestedTypes)
				RemoveTypeAndChildren (types, child);

			if (types.Types.ContainsKey (type.FullName))
				types.Types.Remove (type.FullName);

			if (types.TypesFlattened.ContainsKey (type.FullName))
				types.TypesFlattened.Remove (type.FullName);
		}

		public static JavaPackage ParsePackage (XElement package)
		{
			var pkg = new JavaPackage (
				name: package.XGetAttribute ("name"),
				jniName: package.XGetAttribute ("jni-name"),
				managedName: package.Attribute ("managedName")?.Value
			);

			if (package.XGetAttribute ("merge.SourceFile") is string source && source.HasValue ())
				pkg.PropertyBag.Add ("merge.SourceFile", source);

			foreach (var elem in package.Elements ()) {
				switch (elem.Name.LocalName) {
					case "class":
						if (package.XGetAttribute ("obfuscated") == "true")
							continue;

						pkg.Types.Add (ParseClass (pkg, elem));
						break;
					case "interface":
						if (package.XGetAttribute ("obfuscated") == "true")
							continue;

						pkg.Types.Add (ParseInterface (pkg, elem));
						break;
				}
			}

			return pkg;
		}

		public static JavaClassModel ParseClass (JavaPackage package, XElement element)
		{
			var model = new JavaClassModel (
				javaPackage: package,
				javaNestedName: element.XGetAttribute ("name"),
				javaVisibility: element.XGetAttribute ("visibility"),
				javaAbstract: element.XGetAttribute ("abstract") == "true",
				javaFinal: element.XGetAttribute ("final") == "true",
				javaBaseType: element.XGetAttribute ("extends"),
				javaBaseTypeGeneric: element.XGetAttribute ("extends-generic-aware"),
				javaDeprecated: element.XGetAttribute ("deprecated"),
				javaStatic: element.XGetAttribute ("static") == "true",
				jniSignature: element.XGetAttribute ("jni-signature"),
				baseTypeJni: element.XGetAttribute ("jni-extends")
			);

			if (element.XGetAttribute ("merge.SourceFile") is string source && source.HasValue ())
				model.PropertyBag.Add ("merge.SourceFile", source);
			if (element.XGetAttribute ("deprecated-since") is string dep && dep.HasValue ())
				model.PropertyBag.Add ("deprecated-since", dep);

			if (element.Element ("typeParameters") is XElement tp)
				model.TypeParameters = ParseTypeParameters (tp);

			foreach (var child in element.Elements ()) {
				switch (child.Name.LocalName) {
					case "constructor":
						//if (child.XGetAttribute ("synthetic") != "true")
							model.Constructors.Add (ParseConstructor (model, child));
						break;
					case "field":
						model.Fields.Add (ParseField (model, child));
						break;
					case "implements":
						model.Implements.Add (ParseImplements (child));
						break;
					case "method":
						//if (child.XGetAttribute ("synthetic") != "true")
							model.Methods.Add (ParseMethod (model, child));
						break;
				}
			}

			return model;
		}

		public static JavaInterfaceModel ParseInterface (JavaPackage package, XElement element)
		{
			var nested_name = element.XGetAttribute ("name");
			var visibility = element.XGetAttribute ("visibility");
			var deprecated = element.XGetAttribute ("deprecated");
			var is_static = element.XGetAttribute ("static") == "true";
			var jni_signature = element.XGetAttribute ("jni-signature");

			var model = new JavaInterfaceModel (package, nested_name, visibility, deprecated, is_static, jni_signature);

			if (element.XGetAttribute ("merge.SourceFile") is string source && source.HasValue ())
				model.PropertyBag.Add ("merge.SourceFile", source);
			if (element.XGetAttribute ("deprecated-since") is string dep && dep.HasValue ())
				model.PropertyBag.Add ("deprecated-since", dep);

			if (element.Element ("typeParameters") is XElement tp)
				model.TypeParameters = ParseTypeParameters (tp);

			foreach (var child in element.Elements ()) {
				switch (child.Name.LocalName) {
					case "field":
						model.Fields.Add (ParseField (model, child));
						break;
					case "implements":
						model.Implements.Add (ParseImplements (child));
						break;
					case "method":
						if (child.XGetAttribute ("synthetic") != "true")
							model.Methods.Add (ParseMethod (model, child));
						break;
				}
			}

			return model;
		}

		public static JavaMethodModel ParseMethod (JavaTypeModel type, XElement element)
		{
			var method = new JavaMethodModel (
				javaName: element.XGetAttribute ("name"),
				javaVisibility: element.XGetAttribute ("visibility"),
				javaAbstract: element.XGetAttribute ("abstract") == "true",
				javaFinal: element.XGetAttribute ("final") == "true",
				javaStatic: element.XGetAttribute ("static") == "true",
				javaReturn: element.XGetAttribute ("return"),
				javaParentType: type,
				deprecated: element.XGetAttribute ("deprecated"),
				jniSignature: element.XGetAttribute ("jni-signature"),
				isSynthetic: element.XGetAttribute ("synthetic") == "true",
				isBridge: element.XGetAttribute ("bridge") == "true",
				returnJni: element.XGetAttribute ("jni-return"),
				isNative: element.XGetAttribute ("native") == "true",
				isSynchronized: element.XGetAttribute ("synchronized") == "true",
				returnNotNull: element.XGetAttribute ("return-not-null") == "true"
			);

			if (element.Element ("typeParameters") is XElement tp)
				method.TypeParameters = ParseTypeParameters (tp);

			foreach (var child in element.Elements ("parameter"))
				method.Parameters.Add (ParseParameter (method, child));
			foreach (var child in element.Elements ("exception"))
				method.Exceptions.Add (ParseException (child));

			if (element.XGetAttribute ("merge.SourceFile") is string source && source.HasValue ())
				method.PropertyBag.Add ("merge.SourceFile", source);
			if (element.XGetAttribute ("deprecated-since") is string dep && dep.HasValue ())
				method.PropertyBag.Add ("deprecated-since", dep);

			return method;
		}

		public static JavaConstructorModel ParseConstructor (JavaTypeModel type, XElement element)
		{
			var method = new JavaConstructorModel (
				javaName: element.XGetAttribute ("name"),
				javaVisibility: element.XGetAttribute ("visibility"),
				javaAbstract: element.XGetAttribute ("abstract") == "true",
				javaFinal: element.XGetAttribute ("final") == "true",
				javaStatic: element.XGetAttribute ("static") == "true",
				javaParentType: type,
				deprecated: element.XGetAttribute ("deprecated"),
				jniSignature: element.XGetAttribute ("jni-signature"),
				isSynthetic: element.XGetAttribute ("synthetic") == "true",
				isBridge: element.XGetAttribute ("bridge") == "true"
			);

			if (element.Element ("typeParameters") is XElement tp)
				method.TypeParameters = ParseTypeParameters (tp);
			foreach (var child in element.Elements ("exception"))
				method.Exceptions.Add (ParseException (child));

			foreach (var child in element.Elements ("parameter"))
				method.Parameters.Add (ParseParameter (method, child));

			if (element.XGetAttribute ("merge.SourceFile") is string source && source.HasValue ())
				method.PropertyBag.Add ("merge.SourceFile", source);
			if (element.XGetAttribute ("deprecated-since") is string dep && dep.HasValue ())
				method.PropertyBag.Add ("deprecated-since", dep);

			return method;
		}

		public static JavaFieldModel ParseField (JavaTypeModel type, XElement element)
		{
			var field = new JavaFieldModel (
				name: element.XGetAttribute ("name"),
				visibility: element.XGetAttribute ("visibility"),
				type: element.XGetAttribute ("type"),
				typeGeneric: element.XGetAttribute ("type-generic-aware"),
				isStatic: element.XGetAttribute ("static") == "true",
				value: element.Attribute ("value")?.Value,
				parent: type,
				isFinal: element.XGetAttribute ("final") == "true",
				deprecated: element.XGetAttribute ("deprecated"),
				jniSignature: element.XGetAttribute ("jni-signature"),
				isTransient: element.XGetAttribute ("transient") == "true",
				isVolatile: element.XGetAttribute ("volatile") == "true",
				isNotNull: element.XGetAttribute ("not-null") == "true"
			);

			if (element.XGetAttribute ("merge.SourceFile") is string source && source.HasValue ())
				field.PropertyBag.Add ("merge.SourceFile", source);
			if (element.XGetAttribute ("deprecated-since") is string dep && dep.HasValue ())
				field.PropertyBag.Add ("deprecated-since", dep);

			return field;
		}

		public static JavaImplementsModel ParseImplements (XElement element)
		{
			var model = new JavaImplementsModel (
				name: element.XGetAttribute ("name"),
				nameGeneric: element.XGetAttribute ("name-generic-aware"),
				jniType: element.XGetAttribute ("jni-type")
			);

			if (element.XGetAttribute ("merge.SourceFile") is string source && source.HasValue ())
				model.PropertyBag.Add ("merge.SourceFile", source);

			return model;
		}

		public static JavaExceptionModel ParseException (XElement element)
		{
			return new JavaExceptionModel (
				name: element.XGetAttribute ("name"),
				type: element.XGetAttribute ("type")
			);
		}

		public static JavaParameterModel ParseParameter (JavaMethodModel method, XElement element)
		{
			var parameter = new JavaParameterModel (
				parent: method,
				javaName: element.XGetAttribute ("name"),
				javaType: element.XGetAttribute ("type"),
				jniType: element.XGetAttribute ("jni-type"),
				isNotNull: element.XGetAttribute ("not-null") == "true"
			);

			return parameter;
		}

		public static JavaTypeParameters ParseTypeParameters (XElement element)
		{
			var parameters = new JavaTypeParameters ();

			foreach (var elem in element.Elements ()) {
				if (elem.Name.LocalName == "typeParameter")
					parameters.Add (ParseTypeParameter (elem));
			}

			if (element.XGetAttribute ("merge.SourceFile") is string source && source.HasValue ())
				parameters.PropertyBag.Add ("merge.SourceFile", source);

			return parameters;
		}

		public static JavaTypeParameter ParseTypeParameter (XElement element)
		{
			var parameter = new JavaTypeParameter (element.XGetAttribute ("name")) {
				ExtendedJniClassBound = element.XGetAttribute ("jni-classBound"),
				ExtendedClassBound = element.XGetAttribute ("classBound"),
				ExtendedInterfaceBounds = element.XGetAttribute ("interfaceBounds"),
				ExtendedJniInterfaceBounds = element.XGetAttribute ("jni-interfaceBounds")
			};

			if (element.Element ("genericConstraints") is XElement gc) {
				parameter.GenericConstraints.AddRange (ParseGenericConstraints (gc));
				return parameter;
			}

			// Now we have to deal with the format difference...
			// Some versions of class-parse stopped generating <genericConstraints> but started
			// generating "classBound" and "interfaceBounds" attributes instead.
			// They don't make sense and blocking this effort, but we have to deal with that...
			if (!string.IsNullOrEmpty (parameter.ExtendedClassBound) || !string.IsNullOrEmpty (parameter.ExtendedInterfaceBounds)) {
				if (!string.IsNullOrEmpty (parameter.ExtendedClassBound))
					parameter.GenericConstraints.Add (new JavaGenericConstraint (parameter.ExtendedClassBound));
				if (!string.IsNullOrEmpty (parameter.ExtendedInterfaceBounds))
					foreach (var ic in parameter.ExtendedInterfaceBounds.Split (':'))
						parameter.GenericConstraints.Add (new JavaGenericConstraint (ic));
			}

			return parameter;
		}

		public static List<JavaGenericConstraint> ParseGenericConstraints (XElement element)
		{
			var list = new List<JavaGenericConstraint> ();

			foreach (var elem in element.Elements ()) {
				if (elem.Name.LocalName == "genericConstraint")
					list.Add (ParseGenericConstraint (elem));
			}

			return list;
		}

		public static JavaGenericConstraint ParseGenericConstraint (XElement element)
		{
			return new JavaGenericConstraint (
				element.XGetAttribute ("type")
			);
		}
	}
}
