using System.IO;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	public class MonoAndroid : BaseGeneratorTest
	{
		[Test]
		public void GeneratedOK ()
		{
			CompileToSingleAssembly = true;
			RunAllTargets (
					outputRelativePath: "Mono.Android",
					apiDescriptionFile: "expected/Mono.Android/api.xml",
					expectedRelativePath: "Mono.Android",
					additionalSupportPaths: null);
		}
	}
}
