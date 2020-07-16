using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoDroid.Generation;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	public class InterfaceExtensionsClass : ClassWriter
	{
		public InterfaceExtensionsClass (InterfaceGen iface, string declaringTypeName, CodeGenerationOptions opt)
		{
			Name = $"{declaringTypeName}{iface.Name}Extensions";

			IsPublic = true;
			IsStatic = true;
			IsPartial = true;

			foreach (var method in iface.Methods.Where (m => !m.IsStatic)) {
				if (method.CanHaveStringOverload)
					Methods.Add (new BoundMethodExtensionStringOverload (method, opt, iface.FullName));

				if (method.Asyncify)
					Methods.Add (new MethodExtensionAsyncWrapper (method, opt, iface.FullName));
			}
		}

		public bool ShouldGenerate => Methods.Any ();
	}
}
