using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Java.Interop.Tools.Maven.Models;

namespace Java.Interop.Tools.Maven.Repositories;

public class InMemoryMavenRepository : IMavenRepository
{
	readonly Dictionary<string, byte []> cache = new ();

	public string Name => "InMemory";

	public async Task AddFile (Artifact artifact, string filename, Stream stream)
	{
		var bytes = new byte [stream.Length];

		await stream.ReadAsync (bytes, 0, bytes.Length);

		cache.Add ($"{artifact}/{filename}", bytes);
	}

	public bool TryGetFile (Artifact artifact, string filename, [NotNullWhen (true)] out Stream? stream)
	{
		stream = null;

		if (cache.TryGetValue ($"{artifact}/{filename}", out var bytes)) {
			stream = new MemoryStream (bytes);
			return true;
		}

		return false;
	}
}
