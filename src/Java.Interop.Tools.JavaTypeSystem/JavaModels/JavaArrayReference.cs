using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public class JavaArrayReference : IJavaTypeReference
	{
		public IJavaTypeReference ElementType { get; }

		public bool IsParamArray { get; }

		public JavaTypeModel RootType => ElementType.RootType;

		public JavaArrayReference (IJavaTypeReference elementType, bool isParams)
		{
			ElementType = elementType;
			IsParamArray = isParams;
		}
	}
}
