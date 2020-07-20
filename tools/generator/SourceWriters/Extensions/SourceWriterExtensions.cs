using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoDroid.Generation;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	public static class SourceWriterExtensions
	{
		public static void AddMethodCustomAttributes (List<AttributeWriter> attributes, Method method)
		{
			if (method.GenericArguments != null && method.GenericArguments.Any ())
				attributes.Add (new CustomAttr (method.GenericArguments.ToGeneratedAttributeString ()));
			if (method.CustomAttributes != null)
				attributes.Add (new CustomAttr (method.CustomAttributes));
			if (method.Annotation != null)
				attributes.Add (new CustomAttr (method.Annotation));
		}

		public static string GetInvokeType (string type)
		{
			switch (type) {
				case "Bool": return "Boolean";
				case "Byte": return "SByte";
				case "Int": return "Int32";
				case "Short": return "Int16";
				case "Long": return "Int64";
				case "Float": return "Single";
				case "UInt": return "Int32";
				case "UShort": return "Int16";
				case "ULong": return "Int64";
				case "UByte": return "SByte";
				default: return type;
			}
		}

		public static void WriteMethodBody (CodeWriter writer, Method method, CodeGenerationOptions opt)
		{
			writer.WriteLine ($"const string __id = \"{method.JavaName}.{method.JniSignature}\";");

			foreach (string prep in method.Parameters.GetCallPrep (opt))
				writer.WriteLine (prep);

			writer.WriteLine ("try {");

			WriteParameterListCallArgs (writer, method.Parameters, opt, false);

			var invokeType = JavaInteropCodeGenerator.GetInvokeType (method.RetVal.CallMethodPrefix);

			if (!method.IsVoid) {
				writer.Write ("var __rm = ");
			}

			if (method.IsStatic) {
				writer.WriteLine ("_members.StaticMethods.Invoke{0}Method (__id{1});",
						invokeType,
						method.Parameters.GetCallArgs (opt, invoker: false));
			} else if (method.IsFinal) {
				writer.WriteLine ("_members.InstanceMethods.InvokeNonvirtual{0}Method (__id, this{1});",
						invokeType,
						method.Parameters.GetCallArgs (opt, invoker: false));
			} else if ((method.IsVirtual && !method.IsAbstract) || method.IsInterfaceDefaultMethod) {
				writer.WriteLine ("_members.InstanceMethods.InvokeVirtual{0}Method (__id, this{1});",
						invokeType,
						method.Parameters.GetCallArgs (opt, invoker: false));
			} else {
				writer.WriteLine ("_members.InstanceMethods.InvokeAbstract{0}Method (__id, this{1});",
						invokeType,
						method.Parameters.GetCallArgs (opt, invoker: false));
			}

			if (!method.IsVoid) {
				var r = invokeType == "Object" ? "__rm.Handle" : "__rm";
				writer.WriteLine ($"return {method.RetVal.ReturnCast}{method.RetVal.FromNative (opt, r, true) + opt.GetNullForgiveness (method.RetVal)};");
			}

			writer.WriteLine ("} finally {");

			foreach (string cleanup in method.Parameters.GetCallCleanup (opt))
				writer.WriteLine (cleanup);

			writer.WriteLine ("}");
		}

		public static void WriteParameterListCallArgs (CodeWriter writer, ParameterList parameters, CodeGenerationOptions opt, bool invoker)
		{
			if (parameters.Count == 0)
				return;

			var JValue = invoker ? "JValue" : "JniArgumentValue";

			writer.WriteLine ($"{JValue}* __args = stackalloc {JValue} [{parameters.Count}];");

			for (var i = 0; i < parameters.Count; ++i) {
				var p = parameters [i];
				writer.WriteLine ($"__args [{i}] = new {JValue} ({p.GetCall (opt)});");
			}
		}

		public static void WriteMethodStringOverloadBody (CodeWriter writer, Method method, CodeGenerationOptions opt, bool haveSelf)
		{
			var call = new StringBuilder ();

			foreach (var p in method.Parameters) {
				var pname = p.Name;
				if (p.Type == "Java.Lang.ICharSequence") {
					pname = p.GetName ("jls_");
					writer.WriteLine ($"var {pname} = {p.Name} == null ? null : new global::Java.Lang.String ({p.Name});");
				} else if (p.Type == "Java.Lang.ICharSequence[]" || p.Type == "params Java.Lang.ICharSequence[]") {
					pname = p.GetName ("jlca_");
					writer.WriteLine ($"var {pname} = CharSequence.ArrayFromStringArray({p.Name});");
				}

				if (call.Length > 0)
					call.Append (", ");

				call.Append (pname + (p.Type == "Java.Lang.ICharSequence" ? opt.GetNullForgiveness (p) : string.Empty));
			}

			writer.WriteLine ($"{(method.RetVal.IsVoid ? string.Empty : opt.GetTypeReferenceName (method.RetVal) + " __result = ")}{(haveSelf ? "self." : "")}{method.AdjustedName} ({call.ToString ()});");

			switch (method.RetVal.FullName) {
				case "void":
					break;
				case "Java.Lang.ICharSequence[]":
					writer.WriteLine ("var __rsval = CharSequence.ArrayToStringArray (__result);");
					break;
				case "Java.Lang.ICharSequence":
					writer.WriteLine ("var __rsval = __result?.ToString ();");
					break;
				default:
					writer.WriteLine ("var __rsval = __result;");
					break;
			}

			foreach (var p in method.Parameters) {
				if (p.Type == "Java.Lang.ICharSequence")
					writer.WriteLine ($"{p.GetName ("jls_")}?.Dispose ();");
				else if (p.Type == "Java.Lang.ICharSequence[]")
					writer.WriteLine ($"if ({p.GetName ("jlca_")} != null) foreach (var s in {p.GetName ("jlca_")}) s?.Dispose ();");
			}

			if (!method.RetVal.IsVoid)
				writer.WriteLine ($"return __rsval{opt.GetNullForgiveness (method.RetVal)};");
		}
	}
}
