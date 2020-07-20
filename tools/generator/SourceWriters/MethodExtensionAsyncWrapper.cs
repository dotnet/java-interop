using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoDroid.Generation;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	public class MethodExtensionAsyncWrapper : MethodWriter
	{
		readonly Method method;
		readonly CodeGenerationOptions opt;
		readonly string self_type;

		public MethodExtensionAsyncWrapper (Method method, CodeGenerationOptions opt, string selfType)
		{
			this.method = method;
			this.opt = opt;
			self_type = selfType;

			Name = method.AdjustedName + "Async";
			IsStatic = true;

			SetVisibility (method.Visibility);

			ReturnType = new TypeReferenceWriter ("global::System.Threading.Tasks.Task");

			if (!method.IsVoid)
				ReturnType.Name += "<" + opt.GetTypeReferenceName (method.RetVal) + ">";

			Body.Add ($"return global::System.Threading.Tasks.Task.Run (() => self.{method.AdjustedName} ({method.Parameters.GetCall (opt)}));");
		}

		protected override void WriteParameters (CodeWriter writer)
		{
			writer.Write ($"this {self_type} self{(method.Parameters.Count > 0 ? ", " : "")}");
			writer.Write (method.GetSignature (opt));
		}
	}
}
