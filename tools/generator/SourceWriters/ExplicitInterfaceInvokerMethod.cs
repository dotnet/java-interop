using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoDroid.Generation;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	public class ExplicitInterfaceInvokerMethod : MethodWriter
	{
		readonly Method method;
		readonly CodeGenerationOptions opt;

		public ExplicitInterfaceInvokerMethod (Method method, GenBase iface, CodeGenerationOptions opt)
		{
			this.method = method;
			this.opt = opt;

			Name = method.Name;

			IsUnsafe = true;

			ReturnType = new TypeReferenceWriter (opt.GetTypeReferenceName (method.RetVal));
			ExplicitInterfaceImplementation = opt.GetOutputName (iface.FullName);
		}

		protected override void WriteBody (CodeWriter writer)
		{
			SourceWriterExtensions.WriteMethodBody (writer, method, opt);
		}

		protected override void WriteParameters (CodeWriter writer)
		{
			writer.Write (method.GetSignature (opt));
		}
	}
}
