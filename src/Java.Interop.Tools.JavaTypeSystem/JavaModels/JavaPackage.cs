using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public class JavaPackage
	{
		public string Name { get; }
		public string JniName { get; }

		// Note 'null' is significant: it means 'not-set'.
		// Empty string means "set to a blank namespace".
		public string? ManagedName { get; set; }

		public List<JavaTypeModel> Types { get; } = new List<JavaTypeModel> ();
		public Dictionary<string, string> PropertyBag { get; } = new Dictionary<string, string> ();

		public JavaPackage (string name, string jniName, string? managedName)
		{
			Name = name;
			JniName = jniName;
			ManagedName = managedName;
		}
	}
}
