using System;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	internal sealed class Arrays : BaseGeneratorTest
	{
		[Test]
		public void GeneratedOK ()
		{
			RunAllTargets (
					outputRelativePath:     "Arrays",
					apiDescriptionFile:     "expected.ji/Arrays/Arrays.xml",
					expectedRelativePath:   "Arrays");
		}
	}
}

