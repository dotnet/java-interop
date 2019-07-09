using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Xamarin.Android.Tools;

namespace MonoDroid.Generation
{
	class XmlApiImporter
	{
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
	}
}
