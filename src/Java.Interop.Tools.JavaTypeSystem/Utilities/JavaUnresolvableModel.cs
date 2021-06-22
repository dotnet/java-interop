using System;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public class JavaUnresolvableModel
	{
		public IJavaResolvable Unresolvable { get; }
		public string MissingType { get; }

		public JavaUnresolvableModel (IJavaResolvable unresolvable, string missingType)
		{
			Unresolvable = unresolvable;
			MissingType = missingType;
		}
	}
}
