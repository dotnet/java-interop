using System;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	public class Streams : BaseGeneratorTest
	{
		[Test]
		public void GeneratedOK ()
		{
			CompileToSingleAssembly = true;
			RunAllTargets (
					outputRelativePath:    "Streams",
					apiDescriptionFile:    "expected/Streams/Streams.xml",
					expectedRelativePath:  "Streams",
					additionalSourcePaths: new[]{ "expected/Streams/SupportFiles" });
		}
	}
}

