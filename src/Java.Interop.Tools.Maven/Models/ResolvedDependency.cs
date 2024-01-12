using Java.Interop.Tools.Maven.Extensions;

namespace Java.Interop.Tools.Maven.Models;

public class ResolvedDependency
{
	public ResolvedProject Project { get; }
	public string ArtifactId { get; }
	public string? Classifier { get; }
	public string GroupId { get; }
	public string? Optional { get; }
	public string Scope { get; }
	public string? Type { get; }
	public string Version { get; }

	public ResolvedDependency (ResolvedProject project, Dependency dependency)
		: this (project, dependency, false)
	{ }

	internal ResolvedDependency (ResolvedProject project, Dependency dependency, bool shallow)
	{
		Project = project;

		// First fill these in with values from the dependency
		ArtifactId = dependency.ArtifactId.OrEmpty ();
		GroupId = dependency.GroupId.OrEmpty ();
		Classifier = dependency.Classifier;
		Optional = dependency.Optional.OrEmpty ();
		Scope = dependency.Scope.OrEmpty ();
		Type = dependency.Type;
		Version = dependency.Version.OrEmpty ();

		// If we're not shallow, fill in any still missing properties with parent values
		if (!shallow) {
			if (!Classifier.HasValue ())
				Classifier = this.GetInheritedProperty (project, d => d.Classifier);

			if (!Optional.HasValue ())
				Optional = this.GetInheritedProperty (project, d => d.Optional);

			if (!Scope.HasValue ())
				Scope = this.GetInheritedProperty (project, d => d.Scope);

			if (!Type.HasValue ())
				Type = this.GetInheritedProperty (project, d => d.Type);

			if (!Version.HasValue ())
				Version = this.GetInheritedProperty (project, d => d.Version);
		}

		// Default scope to "compile" if not specified
		if (!Scope.HasValue ())
			Scope = "compile";

		// Default optional to "false" if not specified
		if (!Optional.HasValue ())
			Optional = "false";
	}

	public string ToArtifactString (bool includeVersion = true)
	{
		return includeVersion
			? $"{GroupId}:{ArtifactId}:{Version}"
			: $"{GroupId}:{ArtifactId}";
	}

	public override string ToString () => $"{GroupId}:{ArtifactId}:{Version} - {Scope}";
}
