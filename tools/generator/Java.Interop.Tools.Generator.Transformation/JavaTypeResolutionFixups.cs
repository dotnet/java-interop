using System;
using System.IO;
using System.Linq;
using Java.Interop.Tools.Cecil;
using Java.Interop.Tools.Generator;
using Java.Interop.Tools.JavaTypeSystem;

namespace generator
{
	public static class JavaTypeResolutionFixups
	{
		// This fixup ensures all referenced Java types can be resolved, and
		// removes types and members that rely on unresolvable Java types.
		public static void Fixup (string xmlFile, string outputXmlFile, DirectoryAssemblyResolver resolver, string [] references)
		{
			// Parse api.xml
			var type_collection = JavaXmlApiImporter.Parse (xmlFile);

			// Add in reference types from assemblies
			foreach (var reference in references.Distinct ()) {
				Report.Verbose (0, "Resolving assembly for Java type resolution: '{0}'.", reference);
				var assembly = resolver.Load (reference);

				ManagedApiImporter.Parse (assembly, type_collection);
			}

			// Run the type resolution pass
			var results = type_collection.ResolveCollection ();

			OutputResults (results, xmlFile, outputXmlFile);

			// Output the adjusted xml
			JavaXmlApiExporter.Save (type_collection, outputXmlFile);
		}

		static void OutputResults (CollectionResolutionResults results, string xmlFile, string outputXmlFile)
		{
			if (results.Count == 0)
				return;

			var source = new SourceLineInfo (xmlFile);

			var first_cycle = results.First ();

			// The first cycle is more interesting to the user, as it is mainly external Java types
			// that could not be resolved, which often point to a missing reference jar or NuGet.
			// Thus, we're going to output these as warnings.
			var missing_types = first_cycle.Unresolvables.Select (u => u.MissingType).Distinct ().OrderBy (t => t);

			foreach (var type in missing_types)
				Report.LogCodedWarning (0, Report.WarningJavaTypeNotResolved, source, type);

			// The remaining cycles are generally just user types that are being removed because
			// other user types have been removed. Since the root cause is actually the missing external
			// types above, these aren't as important. We'll write them to a diagnostic file
			// for the user to inspect for details if they want.
			var report_file = Path.Combine (Path.GetDirectoryName (outputXmlFile), "java-resolution-report.log");

			using (var tw = File.CreateText (report_file)) {
				for (var i = 0; i < results.Count; i++)
					WriteCycle (tw, i + 1, results [i]);
			}

			// Let users know about this report
			Report.LogCodedWarning (0, Report.WarningTypesNotBoundDueToMissingJavaTypes, new SourceLineInfo (report_file));
		}

		static void WriteCycle (StreamWriter writer, int index, CollectionResolutionResult result)
		{
			writer.WriteLine ($"==== Cycle {index} ====");
			writer.WriteLine ();

			foreach (var item in result.Unresolvables)
				writer.WriteLine ($"'{item.Unresolvable}' was removed because the Java type '{item.MissingType}' could not be found.");

			writer.WriteLine ();
		}
	}
}
