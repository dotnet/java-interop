using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public class JavaGenericTypeParameterReference : IJavaTypeReference
	{
		public string Name { get; }

		public JavaTypeModel RootType => throw new NotImplementedException ();

		public JavaGenericTypeParameterReference (string name)
		{
			Name = name;
		}
	}
}
