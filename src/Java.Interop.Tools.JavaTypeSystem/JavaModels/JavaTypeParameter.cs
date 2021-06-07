using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public class JavaTypeParameter : IJavaResolvable
	{
		public string Name { get; set; }

		public string? ExtendedJniClassBound { get; set; }
		public string? ExtendedClassBound { get; set; }
		public string? ExtendedInterfaceBounds { get; set; }
		public string? ExtendedJniInterfaceBounds { get; set; }

		public List<JavaGenericConstraint> GenericConstraints { get; } = new List<JavaGenericConstraint> ();

		public JavaTypeParameter (string name)
		{
			Name = name;
		}

		public void Resolve (JavaTypeCollection types, List<JavaUnresolvableModel> unresolvables)
		{
			// TODO: Resolve generic constraints
			//var type_parameters = GetApplicableTypeParameters ().ToArray ();
		}

		public override string ToString () => Name ?? "";
	}
}
