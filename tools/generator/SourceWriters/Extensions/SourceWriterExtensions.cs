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
		public static void AddField (TypeWriter tw, GenBase type, Field field, CodeGenerationOptions opt)
		{
			if (field.NeedsProperty)
				tw.Properties.Add (new BoundFieldAsProperty (type, field, opt) { Priority = tw.GetNextPriority () });
			else
				tw.Fields.Add (new BoundField (type, field, opt) { Priority = tw.GetNextPriority () });
		}

		public static bool AddFields (TypeWriter tw, GenBase gen, List<Field> fields, HashSet<string> seen, CodeGenerationOptions opt, CodeGeneratorContext context)
		{
			bool needsProperty = false;

			foreach (var f in fields) {
				if (gen.ContainsName (f.Name)) {
					Report.Warning (0, Report.WarningFieldNameCollision, "Skipping {0}.{1}, due to a duplicate field, method or nested type name. {2} (Java type: {3})", gen.FullName, f.Name, gen.HasNestedType (f.Name) ? "(Nested type)" : gen.ContainsProperty (f.Name, false) ? "(Property)" : "(Method)", gen.JavaName);
					continue;
				}

				if (seen != null && seen.Contains (f.Name)) {
					Report.Warning (0, Report.WarningDuplicateField, "Skipping {0}.{1}, due to a duplicate field. (Field) (Java type: {2})", gen.FullName, f.Name, gen.JavaName);
					continue;
				}

				if (f.Validate (opt, gen.TypeParameters, context)) {
					if (seen != null)
						seen.Add (f.Name);
					needsProperty = needsProperty || f.NeedsProperty;
					AddField (tw, gen, f, opt);
				}
			}

			return needsProperty;
		}

		public static void AddInterfaceListenerEventsAndProperties (TypeWriter tw, InterfaceGen @interface, ClassGen target, CodeGenerationOptions opt)
		{
			var methods = target.Methods.Concat (target.Properties.Where (p => p.Setter != null).Select (p => p.Setter));
			var props = new HashSet<string> ();
			var refs = new HashSet<string> ();
			var eventMethods = methods.Where (m => m.IsListenerConnector && m.EventName != String.Empty && m.ListenerType == @interface).OrderBy (m => m.Parameters.Count).GroupBy (m => m.Name).Select (g => g.First ()).Distinct ();
			foreach (var method in eventMethods) {
				string name = method.CalculateEventName (target.ContainsName);
				if (String.IsNullOrEmpty (name)) {
					Report.Warning (0, Report.WarningInterfaceGen + 1, "empty event name in {0}.{1}.", @interface.FullName, method.Name);
					continue;
				}
				if (opt.GetSafeIdentifier (name) != name) {
					Report.Warning (0, Report.WarningInterfaceGen + 4, "event name for {0}.{1} is invalid. `eventName' or `argsType` can be used to assign a valid member name.", @interface.FullName, method.Name);
					continue;
				}
				var prop = target.Properties.FirstOrDefault (p => p.Setter == method);
				if (prop != null) {
					string setter = "__Set" + prop.Name;
					props.Add (prop.Name);
					refs.Add (setter);
					AddInterfaceListenerEventsAndProperties (tw, @interface, target, name, setter,
						string.Format ("__v => {0} = __v", prop.Name),
						string.Format ("__v => {0} = null", prop.Name), opt);
				} else {
					refs.Add (method.Name);
					string rm = null;
					string remove;
					if (method.Name.StartsWith ("Set"))
						remove = string.Format ("__v => {0} (null)", method.Name);
					else if (method.Name.StartsWith ("Add") &&
						 (rm = "Remove" + method.Name.Substring ("Add".Length)) != null &&
						 methods.Where (m => m.Name == rm).Any ())
						remove = string.Format ("__v => {0} (__v)", rm);
					else
						remove = string.Format ("__v => {{throw new NotSupportedException (\"Cannot unregister from {0}.{1}\");}}",
							@interface.FullName, method.Name);
					AddInterfaceListenerEventsAndProperties (tw, @interface, target, name, method.Name,
						method.Name,
						remove, opt);
				}
			}

			foreach (var r in refs) {
				tw.Fields.Add (new WeakImplementorField (r, opt) { Priority = tw.GetNextPriority () });
				//writer.WriteLine ("{0}WeakReference{2} weak_implementor_{1};", indent, r, opt.NullableOperator);
			}
			//writer.WriteLine ();

			tw.Methods.Add (new CreateImplementorMethod (@interface, opt) { Priority = tw.GetNextPriority () });
			//writer.WriteLine ("{0}{1}Implementor __Create{2}Implementor ()", indent, opt.GetOutputName (@interface.FullName), @interface.Name);
			//writer.WriteLine ("{0}{{", indent);
			//writer.WriteLine ("{0}\treturn new {1}Implementor ({2});", indent, opt.GetOutputName (@interface.FullName),
			//	@interface.NeedsSender ? "this" : "");
			//writer.WriteLine ("{0}}}", indent);
		}

		public static void AddInterfaceListenerEventsAndProperties (TypeWriter tw, InterfaceGen @interface, ClassGen target, string name, string connector_fmt, string add, string remove, CodeGenerationOptions opt)
		{
			if (!@interface.IsValid)
				return;

			foreach (var m in @interface.Methods) {
				string nameSpec = @interface.Methods.Count > 1 ? m.EventName ?? m.AdjustedName : String.Empty;
				string nameUnique = String.IsNullOrEmpty (nameSpec) ? name : nameSpec;
				if (nameUnique.StartsWith ("On"))
					nameUnique = nameUnique.Substring (2);
				if (target.ContainsName (nameUnique))
					nameUnique += "Event";
				AddInterfaceListenerEventOrProperty (tw, @interface, m, target, nameUnique, connector_fmt, add, remove, opt);
			}
		}

		public static void AddInterfaceListenerEventOrProperty (TypeWriter tw, InterfaceGen @interface, Method m, ClassGen target, string name, string connector_fmt, string add, string remove, CodeGenerationOptions opt)
		{
			if (m.EventName == string.Empty)
				return;
			string nameSpec = @interface.Methods.Count > 1 ? m.AdjustedName : String.Empty;
			int idx = @interface.FullName.LastIndexOf (".");
			int start = @interface.Name.StartsWith ("IOn") ? 3 : 1;
			string full_delegate_name = @interface.FullName.Substring (0, idx + 1) + @interface.Name.Substring (start, @interface.Name.Length - start - 8) + nameSpec;
			if (m.IsSimpleEventHandler)
				full_delegate_name = "EventHandler";
			else if (m.RetVal.IsVoid || m.IsEventHandlerWithHandledProperty)
				full_delegate_name = "EventHandler<" + @interface.FullName.Substring (0, idx + 1) + @interface.GetArgsName (m) + ">";
			else
				full_delegate_name += "Handler";
			if (m.RetVal.IsVoid || m.IsEventHandlerWithHandledProperty) {
				if (opt.GetSafeIdentifier (name) != name) {
					Report.Warning (0, Report.WarningInterfaceGen + 5, "event name for {0}.{1} is invalid. `eventName' or `argsType` can be used to assign a valid member name.", @interface.FullName, name);
					return;
				} else {
					var mt = target.Methods.Where (method => string.Compare (method.Name, connector_fmt, StringComparison.OrdinalIgnoreCase) == 0 && method.IsListenerConnector).FirstOrDefault ();
					var hasHandlerArgument = mt != null && mt.IsListenerConnector && mt.Parameters.Count == 2 && mt.Parameters [1].Type == "Android.OS.Handler";

					tw.Events.Add (new InterfaceListenerEvent (@interface, name, nameSpec, full_delegate_name, connector_fmt, add, remove, hasHandlerArgument, opt));
					//WriteInterfaceListenerEvent (@interface, indent, name, nameSpec, m.AdjustedName, full_delegate_name, !m.Parameters.HasSender, connector_fmt, add, remove, hasHandlerArgument);
				}
			} else {
				if (opt.GetSafeIdentifier (name) != name) {
					Report.Warning (0, Report.WarningInterfaceGen + 6, "event property name for {0}.{1} is invalid. `eventName' or `argsType` can be used to assign a valid member name.", @interface.FullName, name);
					return;
				}

				//var cw = new CodeWriter (writer, indent);
				tw.Properties.Add (new InterfaceListenerPropertyImplementor (@interface, name, opt) { Priority = tw.GetNextPriority () });
				//writer.WriteLine ($"{indent}WeakReference{opt.NullableOperator} weak_implementor_{name};");
				//writer.WriteLine (string.Format("{0}{1}Implementor{3} Impl{2} {{", indent, opt.GetOutputName (@interface.FullName), name, opt.NullableOperator));
				//writer.WriteLine ("{0}\tget {{", indent);
				//writer.WriteLine ($"{indent}\t\tif (weak_implementor_{name} == null || !weak_implementor_{name}.IsAlive)");
				//writer.WriteLine ($"{indent}\t\t\treturn null;");
				//writer.WriteLine ($"{indent}\t\treturn weak_implementor_{name}.Target as {opt.GetOutputName (@interface.FullName)}Implementor;");
				//writer.WriteLine ("{0}\t}}", indent);
				//writer.WriteLine ($"{indent}\tset {{ weak_implementor_{name} = new WeakReference (value, true); }}");
				//writer.WriteLine ("{0}}}", indent);
				//writer.WriteLine ();
				tw.Properties.Add (new InterfaceListenerProperty (@interface, name, nameSpec, m.AdjustedName, full_delegate_name, opt) { Priority = tw.GetNextPriority () });
				//WriteInterfaceListenerProperty (@interface, indent, name, nameSpec, m.AdjustedName, connector_fmt, full_delegate_name);
			}
		}

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
