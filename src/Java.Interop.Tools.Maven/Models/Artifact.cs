using System;
using System.Diagnostics.CodeAnalysis;

namespace Java.Interop.Tools.Maven.Models;

public class Artifact
{
	public string GroupId { get; }

	public string Id { get; }

	public string Version { get; }

	public Artifact (string groupId, string artifactId, string version)
	{
		Id = artifactId;
		GroupId = groupId;
		Version = version;
	}

	public static Artifact Parse (string value)
	{
		if (TryParse (value, out var artifact))
			return artifact;

		throw new ArgumentException ($"Invalid artifact format: {value}");
	}

	public static bool TryParse (string value, [NotNullWhen (true)]out Artifact? artifact)
	{
		artifact = null;

		var parts = value.Split (':');

		if (parts.Length != 3)
			return false;

		artifact = new Artifact (parts [0], parts [1], parts [2]);

		return true;
	}

	// This is a "well-known" format we use, it should not be changed.
	public override string ToString () => $"{GroupId}:{Id}:{Version}";
}
