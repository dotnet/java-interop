using System.Xml.Linq;
using Java.Interop.Tools.Maven.Models;

namespace Java.Interop.Tools.Maven_Tests.Extensions;

static class TestDataExtensions
{
	public static Project CreateProject (Artifact artifact, Project? parent = null)
	{
		var p = new Project {
			GroupId = artifact.GroupId,
			ArtifactId = artifact.Id,
			Version = artifact.Version,
		};

		if (parent is not null)
			p.Parent = new Parent {
				GroupId = parent.GroupId,
				ArtifactId = parent.ArtifactId,
				Version = parent.Version,
			};

		return p;
	}

	public static void AddProperty (this Project project, string key, string value)
	{
		var xml = new XElement (key, value);

		project.Properties ??= new ModelProperties ();
		project.Properties.Any.Add (xml);
	}
}
