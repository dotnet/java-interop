using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public class JavaGenericConstraint
	{
		public string Type { get; set; }

		public JavaGenericConstraint (string type)
			=> Type = type;
	}
}
