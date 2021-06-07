using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public class JavaTypeResolutionException : Exception
	{
		public JavaTypeResolutionException (string message) : base (message)
		{
		}

		public enum ResolutionType
		{
			Unknown,
			MethodParameter,
			MethodReturn
		}
	}
}
