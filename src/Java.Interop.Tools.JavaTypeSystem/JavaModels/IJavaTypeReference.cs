using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public interface IJavaTypeReference
	{
		JavaTypeModel RootType { get; }
	}
}
