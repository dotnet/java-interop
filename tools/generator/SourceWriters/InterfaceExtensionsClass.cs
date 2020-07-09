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
		public InterfaceExtensionsClass (InterfaceGen @interface, string declaringTypeName, CodeGenerationOptions opt)
		{
			Name = $"{declaringTypeName}{@interface.Name}Extensions";

			IsPublic = true;
			IsStatic = true;
			IsPartial = true;

			foreach (var method in @interface.Methods.Where (m => !m.IsStatic)) {
				if (method.CanHaveStringOverload)
					Methods.Add (new BoundMethodExtensionStringOverload (method, opt, @interface.FullName) { Priority = GetNextPriority () });

				if (method.Asyncify)
					Methods.Add (new MethodExtensionAsyncWrapper (method, opt, @interface.FullName) { Priority = GetNextPriority () });
			}
		}

		public bool ShouldGenerate => Methods.Any ();
	}
}
