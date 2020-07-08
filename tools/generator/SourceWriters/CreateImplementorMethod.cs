using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoDroid.Generation;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	public class CreateImplementorMethod : MethodWriter
	{
		public CreateImplementorMethod (InterfaceGen @interface, CodeGenerationOptions opt)
		{
			Name = $"__Create{@interface.Name}Implementor";

			ReturnType = new TypeReferenceWriter ($"{opt.GetOutputName (@interface.FullName)}Implementor");

			Body.Add ($"return new {opt.GetOutputName (@interface.FullName)}Implementor ({(@interface.NeedsSender ? "this" : "")});");
		}
	}
}
