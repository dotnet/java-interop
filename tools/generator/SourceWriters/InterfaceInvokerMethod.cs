using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoDroid.Generation;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	public class InterfaceInvokerMethod : MethodWriter
	{
		readonly MethodCallback method_callback;
		readonly Method method;
		readonly CodeGenerationOptions opt;
		readonly CodeGeneratorContext context;

		public InterfaceInvokerMethod (InterfaceGen iface, Method method, CodeGenerationOptions opt, CodeGeneratorContext context)
		{
			this.method = method;
			this.opt = opt;
			this.context = context;

			Name = method.AdjustedName;
			ReturnType = new TypeReferenceWriter (opt.GetTypeReferenceName (method.RetVal));

			IsPublic = true;
			IsUnsafe = true;
			IsStatic = method.IsStatic;

			method_callback = new MethodCallback (iface, method, opt, null, method.IsReturnCharSequence);
		}

		public override void Write (CodeWriter writer)
		{
			method_callback?.Write (writer);

			writer.WriteLine ($"IntPtr {method.EscapedIdName};");

			base.Write (writer);
		}

		protected override void WriteParameters (CodeWriter writer)
		{
			writer.Write (method.GetSignature (opt));
		}

		protected override void WriteBody (CodeWriter writer)
		{
			SourceWriterExtensions.WriteMethodInvokerBody (writer, method, opt, context);
		}
	}
}
