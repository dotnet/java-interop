using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Java.Interop.Tools.Maven.Extensions;

namespace Java.Interop.Tools.Maven.Models;

public partial class Project
{
	public static Project Parse (Stream stream)
	{
		Project? result = null;

		var serializer = new XmlSerializer (typeof (Project));

		using (var sr = new StreamReader (stream))
			result = (Project) serializer.Deserialize (new XmlTextReader (sr) {
				Namespaces = false,
			});

		return result;
	}

	public static Project ParseXml (string xml)
	{
		Project? result = null;

		var serializer = new XmlSerializer (typeof (Project));

		using (var sr = new StringReader (xml))
			result = (Project) serializer.Deserialize (new XmlTextReader (sr) {
				Namespaces = false,
			});

		return result;
	}

	public bool TryGetParentPomArtifact ([NotNullWhen (true)] out Artifact? parent)
	{
		parent = null;

		if (Parent is not null) {
			parent = new Artifact (Parent.GroupId.OrEmpty (), Parent.ArtifactId.OrEmpty (), Parent.Version.OrEmpty ());
			return true;
		}

		return false;
	}

	public override string ToString () => $"{GroupId}:{ArtifactId}:{Version}";

	public string ToXml ()
	{
		var serializer = new XmlSerializer (typeof (Project));

		using (var sw = new StringWriter ()) {
			serializer.Serialize (sw, this);
			return sw.ToString ();
		}
	}
}
