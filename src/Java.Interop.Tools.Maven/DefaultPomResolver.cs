using System;
using System.Collections.Generic;
using Java.Interop.Tools.Maven.Models;

namespace Java.Interop.Tools.Maven;

public class DefaultPomResolver : IPomResolver
{
	readonly Dictionary<string, Project> poms = new ();

	public void Register (Project project)
	{
		poms.Add (project.ToString (), project);
	}

	public virtual Project ResolveRawProject (Artifact artifact)
	{
		if (poms.TryGetValue (artifact.ToString (), out var project))
			return project;

		throw new InvalidOperationException ($"No POM registered for {artifact}");
	}
}
