using Mono.Cecil;
using System.IO;
using System.Linq;

namespace Java.Interop.Tools.Cecil
{
	public class AssemblyNotFoundException : FileNotFoundException
	{
		public AssemblyNotFoundException (AssemblyNameReference reference)
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
		}

		public AssemblyNameReference Reference { get; private set; }
	}
}
