using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public class JavaBuiltInType : JavaTypeModel
	{
		public JavaBuiltInType (string name) : base (new JavaPackage ("", "", null), name, "public", false, true, "not deprecated", false, "") { }

		public override void Resolve (JavaTypeCollection types, List<JavaUnresolvableModel> unresolvables)
		{
			throw new NotImplementedException ();
		}
	}
}
