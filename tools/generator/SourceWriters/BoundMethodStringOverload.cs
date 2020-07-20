using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoDroid.Generation;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	public class BoundMethodStringOverload : MethodWriter
	{
		readonly Method method;
		readonly CodeGenerationOptions opt;

		public BoundMethodStringOverload (Method method, CodeGenerationOptions opt) : base ()
		{
			this.method = method;
			this.opt = opt;

			Name = method.Name;
			IsStatic = method.IsStatic;

			SetVisibility (method.Visibility);
			ReturnType = new TypeReferenceWriter (opt.GetTypeReferenceName (method.RetVal).Replace ("Java.Lang.ICharSequence", "string").Replace ("global::string", "string"));

			if (method.Deprecated != null)
				Attributes.Add (new ObsoleteAttr (method.Deprecated.Replace ("\"", "\"\"").Trim ()));
		}

		protected override void WriteBody (CodeWriter writer)
		{
			SourceWriterExtensions.WriteMethodStringOverloadBody (writer, method, opt, false);
		}

		protected override void WriteParameters (CodeWriter writer)
		{
			writer.Write (method.GetSignature (opt).Replace ("Java.Lang.ICharSequence", "string").Replace ("global::string", "string"));
		}
	}
}
