using System.IO;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	public class AcrossAssemblies : BaseGeneratorTest
	{
		[Test]
		public void GeneratedOK ()
		{
			RunAllTargets (
					outputRelativePath: "AcrossAssemblies",
					apiDescriptionFile: "expected/AcrossAssemblies/AcrossAssemblies.xml",
					expectedRelativePath: "AcrossAssemblies",
					additionalSupportPaths: null);
		}
	}
}
