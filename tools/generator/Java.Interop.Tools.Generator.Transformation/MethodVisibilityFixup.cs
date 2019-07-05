using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoDroid.Generation
{
	// Java allows a public member to override a protected member, but C# does not.
	// This Fixup makes the override member protected as well.
	static class MethodVisibilityFixup
	{
		public static void Fixup (GenBase gen)
		{
			foreach (var method in gen.GetAllMethods ()) {
				var base_method = method.GetBaseMethodDeclaration ();

				if (base_method == null)
					continue;

				if (method.Visibility == "public" && (base_method.Visibility == "protected" || base_method.Visibility == "protected internal")) {
					Report.Warning (0, Report.WarningInconsistentAccessbility, $"Setting {gen.FullName}.{method.Name} visibility to {base_method.Visibility} to match base method");
					method.Visibility = base_method.Visibility;
				}
			}
		}
	}
}
