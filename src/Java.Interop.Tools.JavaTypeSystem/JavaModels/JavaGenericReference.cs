using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public class JavaGenericReference : IJavaTypeReference
	{
		public IJavaTypeReference Symbol { get; }
		public List<IJavaTypeReference> TypeParameters { get; } = new List<IJavaTypeReference> ();

		public JavaTypeModel RootType => Symbol.RootType;

		public JavaGenericReference (IJavaTypeReference symbol, params IJavaTypeReference [] parameters)
		{
			Symbol = symbol;
			TypeParameters.AddRange (parameters);
		}
	}
}
