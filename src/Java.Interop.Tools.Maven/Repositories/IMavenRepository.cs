using System.Diagnostics.CodeAnalysis;
using System.IO;
using Java.Interop.Tools.Maven.Models;

namespace Java.Interop.Tools.Maven.Repositories;

public interface IMavenRepository
{
	// This name is used for on-disk caching purposes, and thus must be compatible with file system naming rules.
	string Name { get; }
	bool TryGetFile (Artifact artifact, string filename, [NotNullWhen (true)] out Stream? stream);
}
