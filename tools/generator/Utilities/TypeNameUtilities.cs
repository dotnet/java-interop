using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoDroid.Generation
{
	public static class TypeNameUtilities
	{		
		public static string FilterPrimitiveFullName (string s)
		{
			switch (s) {
			case "System.Boolean":
				return "boolean";
			case "System.Char":
				return "char";
			case "System.Byte":
				return "byte";
			case "System.SByte":
				return "byte";
			case "System.Int16":
				return "short";
			case "System.Int32":
				return "int";
			case "System.Int64":
				return "long";
			case "System.Single":
				return "float";
			case "System.Double":
				return "double";
			case "System.Void":
				return "void";
			case "System.String":
				return "java.lang.String";
			}
			return null;
		}

		public static string GetGenericJavaObjectTypeOverride (string managed_name, string parms)
		{
			switch (managed_name) {
			case "System.Collections.ICollection":
				return "JavaCollection";
			case "System.Collections.IDictionary":
				return "JavaDictionary";
			case "System.Collections.IList":
				return "JavaList";
			case "System.Collections.Generic.ICollection":
				return "JavaCollection" + parms;
			case "System.Collections.Generic.IList":
				return "JavaList" + parms;
			case "System.Collections.Generic.IDictionary":
				return "JavaDictionary" + parms;
			}
			return null;
		}

		public static string GetNativeName (string name)
		{
			if (name.StartsWith ("@"))
				return "native__" + name.Substring (1);
			return "native_" + name;
		}

		public static string MangleName (string name)
		{
			switch (name) {
			case "event":
				return "e";
			case "base":
			case "bool":
			case "byte":
			case "callback":
			case "checked":
			case "decimal":
			case "delegate":
			case "fixed":
			case "foreach":
			case "in":
			case "int":
			case "interface":
			case "internal":
			case "is":
			case "lock":
			case "namespace":
			case "new":
			case "null":
			case "object":
			case "out":
			case "override":
			case "params":
			case "readonly":
			case "ref":
			case "remove":
			case "string":
			case "where":
				return "@" + name;
			default:
				return name;
			}
		}

		public static string StudlyCase (string name)
		{
			StringBuilder builder = new StringBuilder ();
			bool raise = true;
			foreach (char c in name) {
				if (c == '_' || c == '-')
					raise = true;
				else if (raise) {
					builder.Append (Char.ToUpper (c));
					raise = false;
				} else
					builder.Append (c);
			}
			return builder.ToString ();
		}
	}
}
