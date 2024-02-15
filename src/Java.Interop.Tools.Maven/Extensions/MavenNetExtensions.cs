using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Java.Interop.Tools.Maven.Models;

namespace Java.Interop.Tools.Maven.Extensions;

static class MavenNetExtensions
{
	public static string GetInheritedProperty (this ResolvedDependency dependency, ResolvedProject project, Func<ResolvedDependency, string?> property)
	{
		// Check our <dependencyManagement> section
		if (CheckDependencyManagementSection (project, dependency, property, out var result))
			return result;

		// Check imported POMs
		foreach (var imported in project.ImportedPomProjects) {
			var value = GetInheritedProperty (dependency, imported, property);

			if (value.HasValue ())
				return value;
		}

		// Check parent POM
		if (project.Parent is not null && !project.Parent.IsSuperPom)
			return GetInheritedProperty (dependency, project.Parent, property);

		return string.Empty;
	}

	static bool CheckImportedPoms (ResolvedDependency dependency, ResolvedProject project, Func<ResolvedDependency, string?> property, [NotNullWhen (true)] out string? result)
	{
		result = null;

		foreach (var imported in project.ImportedPomProjects) {
			var imported_dep = imported.Resolved.DependencyManagement?.Dependencies.FirstOrDefault (x => x.ArtifactId == dependency.ArtifactId && x.GroupId == dependency.GroupId);

			if (imported_dep != null) {
				result = property (new ResolvedDependency (imported, imported_dep, true));

				if (result.HasValue ())
					return true;
			}

			// Recurse, as imported POMs can also import POMs
			if (CheckImportedPoms (dependency, imported, property, out result))
				return true;
		}

		return false;
	}

	static bool CheckDependencyManagementSection (ResolvedProject project, ResolvedDependency dependency, Func<ResolvedDependency, string?> property, [NotNullWhen (true)] out string? result)
	{
		result = null;

		// Check <dependencyManagement>
		var dep_man = project.Resolved.DependencyManagement?.Dependencies.FirstOrDefault (x => x.ArtifactId == dependency.ArtifactId && x.GroupId == dependency.GroupId);

		if (dep_man != null) {
			result = property (new ResolvedDependency (project, dep_man, true)) ?? string.Empty;
			return result.HasValue ();
		}

		return false;
	}

	public static Artifact ToArtifact (this Dependency dependency)
	{
		return new Artifact (dependency.GroupId.OrEmpty (), dependency.ArtifactId.OrEmpty ().OrEmpty (), dependency.Version.OrEmpty ());
	}
}
