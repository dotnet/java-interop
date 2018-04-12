using Mono.Cecil;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Java.Interop.Tools.Cecil
{
	public class AssemblyNotFoundException : FileNotFoundException
	{
		public AssemblyNameReference Reference { get; private set; }

		public ReadOnlyCollection<AssemblyNameReference> ResolutionPath { get; private set; }

		public AssemblyNotFoundException (AssemblyNameReference reference, IList<AssemblyNameReference> resolutionPath)
			: base (string.Format ("Could not load assembly '{0}, Version={1}, Culture={2}, PublicKeyToken={3}'. Perhaps it doesn't exist in the Mono for Android profile?",
				reference.Name,
				reference.Version,
				string.IsNullOrEmpty (reference.Culture) ? "neutral" : reference.Culture,
				reference.PublicKeyToken == null
				? "null"
				: string.Join ("", reference.PublicKeyToken.Select (b => b.ToString ("x2")))),
			reference.Name)
		{
			Reference = reference;
			ResolutionPath = new ReadOnlyCollection<AssemblyNameReference> (resolutionPath);
		}

		public string GetDetailedMessage ()
		{
			var m = new StringBuilder ();
			m.Append ("Could not load assembly '").Append (Reference).Append ("'.");
			if (ResolutionPath.Count > 0) {
				m.Append (" Found via: ").Append (ResolutionPath [0]);
				for (int i = 1; i < ResolutionPath.Count; ++i) {
					m.Append (" -> ").Append (ResolutionPath [i]);
				}
			}
			return m.ToString ();
		}

		public override string ToString ()
		{
			return GetDetailedMessage () + " " + base.ToString ();
		}
	}
}
