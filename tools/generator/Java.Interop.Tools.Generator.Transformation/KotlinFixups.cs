using System;
using System.Collections.Generic;
using System.Linq;
using Java.Interop.Tools.Generator;
using MonoDroid.Generation;
using MonoDroid.Utils;

namespace Java.Interop.Tools.Generator.Transformation
{
	public static class KotlinFixups
	{
		public static void Fixup (List<GenBase> gens)
		{
			foreach (var c in gens.OfType<ClassGen> ())
				FixupClass (c);
			foreach (var i in gens.OfType<InterfaceGen> ())
				FixupInterface (i);
		}

		private static void FixupClass (ClassGen c)
		{
			// Kotlin mangles the name of some methods to make them
			// inaccessible from Java, like `add-impl` and `add-V5j3Lk8`.
			// We need to generate C# compatible names as well as prevent overriding 
			// them as we cannot generate JCW for them.

			var mangled = c.Methods.Where (m => m.IsKotlinNameMangled).ToList ();

			foreach (var method in mangled) {

				// If the method is virtual, mark it as !virtual as it can't be overridden in Java
				if (!method.IsFinal)
					method.IsFinal = true;

				if (method.IsVirtual)
					method.IsVirtual = false;

				FixMethodName (method);
			}

			RemoveCollidingSiblings (c, mangled);
		}

		private static void FixupInterface (InterfaceGen gen)
		{
			var mangled = gen.Methods.Where (m => m.IsKotlinNameMangled).ToList ();

			foreach (var method in mangled)
				FixMethodName (method);

			RemoveCollidingSiblings (gen, mangled);
		}

		// When multiple hash-mangled siblings of the same Kotlin source-name exist
		// (common for Jetpack Compose `@Composable` functions with inline-class
		// parameters), the rename above produces several methods with the same
		// name and identical C#-erased parameter lists. Emitting them all causes
		// CS0111 in the generated code. Until step 2 of dotnet/java-interop#1431
		// projects inline-class params as strongly-typed wrappers, drop the
		// duplicates deterministically (keep the first) and warn so the user can
		// override via Metadata.xml if desired.
		private static void RemoveCollidingSiblings (GenBase gen, List<Method> renamed)
		{
			if (renamed.Count < 2)
				return;

			foreach (var group in renamed.GroupBy (m => m.Name)) {
				var kept = new List<Method> ();

				foreach (var method in group) {
					if (kept.Any (k => k.Matches (method))) {
						Report.LogCodedWarning (0, Report.WarningKotlinNameMangledCollision, method, gen.FullName, method.Name, method.JavaName);
						gen.Methods.Remove (method);
					} else {
						kept.Add (method);
					}
				}
			}
		}

		private static void FixMethodName (Method method)
		{
			// Only run this if it's the default name (ie: not a user's "managedName")
			if (method.Name == StringRocks.MemberToPascalCase (method.JavaName).Replace ('-', '_')) {
				// We want to remove the hyphen and anything afterwards to fix mangled names,
				// but a previous step converted it to an underscore. Remove the final
				// underscore and anything after it.
				var index = method.Name.IndexOf ("_impl", StringComparison.Ordinal);

				if (index >= 0)
					method.Name = method.Name.Substring (0, index);
				else
					method.Name = method.Name.Substring (0, method.Name.Length - 8);
			}
		}
	}
}
