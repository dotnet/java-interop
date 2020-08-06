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
	}
}
