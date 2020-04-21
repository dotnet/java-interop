using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.SourceWriter
{
	public class InterfaceMethodDeclarationWriter : MethodWriter
	{
		public InterfaceMethodDeclarationWriter (string name, TypeReferenceWriter returnType = null)
			: base (name, returnType)
		{
			IsPublic = false;
		}

		protected override void WriteBody (CodeWriter writer)
		{
			writer.WriteLine (";");
		}
	}
}
