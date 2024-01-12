using System;
using System.IO;
using Java.Interop.Tools.Maven;
using Java.Interop.Tools.Maven.Models;
using Java.Interop.Tools.Maven.Repositories;

namespace Java.Interop.Tools.Maven_Tests.Extensions;

class MavenPomResolver : IPomResolver
{
	readonly IMavenRepository repository;

	public MavenPomResolver (IMavenRepository repository)
	{
		this.repository = repository;
	}

	static MavenPomResolver ()
	{
		var cache_path = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), "dotnet-android", "MavenCacheDirectory");

		Central = new MavenPomResolver (new CachedMavenRepository (cache_path, MavenRepository.Central));
		Google = new MavenPomResolver (new CachedMavenRepository (cache_path, MavenRepository.Google));
	}

	public Project ResolveRawProject (Artifact artifact)
	{
		if (repository.TryGetFile (artifact, $"{artifact.Id}-{artifact.Version}.pom", out var stream)) {
			using (stream) {
				return Project.Parse (stream) ?? throw new InvalidOperationException ($"Could not deserialize POM for {artifact}");
			}
		}

		throw new InvalidOperationException ($"No POM found for {artifact}");
	}

	public static MavenPomResolver Google { get; }

	public static MavenPomResolver Central { get; }
}
