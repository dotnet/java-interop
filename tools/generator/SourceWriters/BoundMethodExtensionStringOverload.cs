using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoDroid.Generation;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	public class BoundMethodExtensionStringOverload : MethodWriter
	{
		readonly Method method;
		readonly CodeGenerationOptions opt;
		readonly string self_type;

		public BoundMethodExtensionStringOverload (Method method, CodeGenerationOptions opt, string selfType)
		{
			this.method = method;
			this.opt = opt;
			self_type = selfType;

			Name = method.Name;
			IsStatic = true;

			SetVisibility (method.Visibility);
			ReturnType = new TypeReferenceWriter (opt.GetTypeReferenceName (method.RetVal).Replace ("Java.Lang.ICharSequence", "string").Replace ("global::string", "string"));

			if (method.Deprecated != null)
				Attributes.Add (new ObsoleteAttr (method.Deprecated.Replace ("\"", "\"\"").Trim ()));
		}

		protected override void WriteBody (CodeWriter writer)
		{
			SourceWriterExtensions.WriteMethodStringOverloadBody (writer, method, opt, true);
		}

		protected override void WriteParameters (CodeWriter writer)
		{
			writer.Write ($"this {self_type} self{(method.Parameters.Count > 0 ? ", " : "")}");
			writer.Write (method.GetSignature (opt).Replace ("Java.Lang.ICharSequence", "string").Replace ("global::string", "string"));
		}
	}
}
