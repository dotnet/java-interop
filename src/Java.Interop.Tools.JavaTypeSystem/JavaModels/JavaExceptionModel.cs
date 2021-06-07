using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public class JavaExceptionModel
	{
		public string Name { get; }
		public string Type { get; }

		public JavaExceptionModel (string name, string type)
		{
			Name = name;
			Type = type;
		}
	}
}
