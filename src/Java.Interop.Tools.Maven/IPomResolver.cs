using Java.Interop.Tools.Maven.Models;

namespace Java.Interop.Tools.Maven;

public interface IPomResolver
{
	Project ResolveRawProject (Artifact artifact);
}
