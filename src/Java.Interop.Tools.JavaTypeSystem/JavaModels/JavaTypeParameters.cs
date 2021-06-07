using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public class JavaTypeParameters : List<JavaTypeParameter>
	{
		public Dictionary<string, string> PropertyBag { get; } = new Dictionary<string, string> ();
	}
}
