using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MonoDroid.Utils;
using Xamarin.Android.Tools;

namespace MonoDroid.Generation
{
	class XmlApiImporter
	{
		static readonly Regex ApiLevel = new Regex (@"api-(\d+).xml");

		public static Ctor CreateCtor (GenBase declaringType, XElement elem)
		{
			var ctor = new Ctor (declaringType) {
				CustomAttributes = elem.XGetAttribute ("customAttributes"),
				Deprecated = elem.Deprecated (),
				GenericArguments = elem.GenericArguments (),
				Name = elem.XGetAttribute ("name"),
				Visibility = elem.Visibility ()
			};

			var idx = ctor.Name.LastIndexOf ('.');

			if (idx > 0)
				ctor.Name = ctor.Name.Substring (idx + 1);

			// If 'elem' is a constructor for a non-static nested type, then
			// the type of the containing class must be inserted as the first argument
			ctor.IsNonStaticNestedType = idx > 0 && elem.Parent.Attribute ("static").Value == "false";

			if (ctor.IsNonStaticNestedType) {
				string declName = elem.Parent.XGetAttribute ("name");
				string expectedEnclosingName = declName.Substring (0, idx);
				XElement enclosingType = GetPreviousClass (elem.Parent.PreviousNode, expectedEnclosingName);

				if (enclosingType == null) {
					ctor.MissingEnclosingClass = true;
					Report.Warning (0, Report.WarningCtor + 0, "For {0}, could not find enclosing type '{1}'.", ctor.Name, expectedEnclosingName);
				} else
					ctor.Parameters.AddFirst (CreateParameterFromClassElement (enclosingType));
			}

			foreach (var child in elem.Elements ()) {
				if (child.Name == "parameter")
					ctor.Parameters.Add (CreateParameter (child));
			}

			return ctor;
		}

		public static Field CreateField (XElement elem)
		{
			var field = new Field {
				DeprecatedComment = elem.XGetAttribute ("deprecated"),
				IsAcw = true,
				IsDeprecated = elem.XGetAttribute ("deprecated") != "not deprecated",
				IsFinal = elem.XGetAttribute ("final") == "true",
				IsStatic = elem.XGetAttribute ("static") == "true",
				JavaName = elem.XGetAttribute ("name"),
				SetterParameter = CreateParameter (elem),
				TypeName = elem.XGetAttribute ("type"),
				Value = elem.XGetAttribute ("value"), // do not trim
				Visibility = elem.XGetAttribute ("visibility")
			};

			field.SetterParameter.Name = "value";

			if (elem.XGetAttribute ("enumType") != null) {
				field.IsEnumified = true;
				field.TypeName = elem.XGetAttribute ("enumType");
			}

			if (elem.Attribute ("managedName") != null)
				field.Name = elem.XGetAttribute ("managedName");
			else
				field.Name = SymbolTable.StudlyCase (char.IsLower (field.JavaName [0]) || field.JavaName.ToLower ().ToUpper () != field.JavaName ? field.JavaName : field.JavaName.ToLower ());

			return field;
		}

		public static Method CreateMethod (GenBase declaringType, XElement elem)
		{
			var method = new Method (declaringType) {
				ArgsType = elem.Attribute ("argsType")?.Value,
				CustomAttributes = elem.XGetAttribute ("customAttributes"),
				Deprecated = elem.Deprecated (),
				EventName = elem.Attribute ("eventName")?.Value,
				GenerateAsyncWrapper = elem.Attribute ("generateAsyncWrapper") != null,
				GenerateDispatchingSetter = elem.Attribute ("generateDispatchingSetter") != null,
				GenericArguments = elem.GenericArguments (),
				IsAbstract = elem.XGetAttribute ("abstract") == "true",
				IsAcw = true,
				IsFinal = elem.XGetAttribute ("final") == "true",
				IsReturnEnumified = elem.Attribute ("enumReturn") != null,
				IsStatic = elem.XGetAttribute ("static") == "true",
				JavaName = elem.XGetAttribute ("name"),
				ManagedReturn = elem.XGetAttribute ("managedReturn"),
				PropertyNameOverride = elem.XGetAttribute ("propertyName"),
				Return = elem.XGetAttribute ("return"),
				SourceApiLevel = GetApiLevel (elem.XGetAttribute ("merge.SourceFile")),
				Visibility = elem.Visibility ()
			};

			method.IsVirtual = !method.IsStatic && elem.XGetAttribute ("final") == "false";

			if (elem.Attribute ("managedName") != null)
				method.Name = elem.XGetAttribute ("managedName");
			else
				method.Name = StringRocks.MemberToPascalCase (method.JavaName);

			if (method.IsReturnEnumified) {
				method.ManagedReturn = elem.XGetAttribute ("enumReturn");

				// FIXME: this should not require enumReturn. Somewhere in generator uses this property improperly.
				method.Return = method.ManagedReturn;
			}

			if (declaringType is InterfaceGen)
				method.IsInterfaceDefaultMethod = !method.IsAbstract && !method.IsStatic;

			foreach (var child in elem.Elements ()) {
				if (child.Name == "parameter")
					method.Parameters.Add (CreateParameter (child));
			}

			method.FillReturnType ();

			return method;
		}

		public static Parameter CreateParameter (XElement elem)
		{
			string managedName = elem.XGetAttribute ("managedName");
			string name = !string.IsNullOrEmpty (managedName) ? managedName : SymbolTable.MangleName (elem.XGetAttribute ("name"));
			string java_type = elem.XGetAttribute ("type");
			string enum_type = elem.Attribute ("enumType") != null ? elem.XGetAttribute ("enumType") : null;
			string managed_type = elem.Attribute ("managedType") != null ? elem.XGetAttribute ("managedType") : null;
			// FIXME: "enum_type ?? java_type" should be extraneous. Somewhere in generator uses it improperly.
			var result = new Parameter (name, enum_type ?? java_type, enum_type ?? managed_type, enum_type != null, java_type);
			if (elem.Attribute ("sender") != null)
				result.IsSender = true;
			return result;
		}

		public static Parameter CreateParameterFromClassElement (XElement elem)
		{
			string name = "__self";
			string java_type = elem.XGetAttribute ("name");
			string java_package = elem.Parent.XGetAttribute ("name");
			return new Parameter (name, java_package + "." + java_type, null, false);
		}

		static int GetApiLevel (string source)
		{
			if (source == null)
				return 0;

			var m = ApiLevel.Match (source);

			if (!m.Success)
				return 0;

			if (int.TryParse (m.Groups [1].Value, out var api))
				return api;

			return 0;
		}

		static XElement GetPreviousClass (XNode n, string nameValue)
		{
			XElement e = null;

			while (n != null &&
			       ((e = n as XElement) == null ||
				e.Name != "class" ||
				!e.XGetAttribute ("name").StartsWith (nameValue, StringComparison.Ordinal) ||
				// this complicated check (instead of simple name string equivalence match) is required for nested class inside a generic class e.g. android.content.Loader.ForceLoadContentObserver.
				(e.XGetAttribute ("name") != nameValue && e.XGetAttribute ("name").IndexOf ('<') < 0))) {
				n = n.PreviousNode;
			}

			return e;
		}
	}
}
