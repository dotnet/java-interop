using System;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	internal sealed class Constructors : BaseGeneratorTest
	{
		[Test]
		public void GeneratedOK ()
		{
			RunAllTargets (
					outputRelativePath:     "Constructors",
					apiDescriptionFile:     "expected.ji/Constructors/Constructors.xml",
					expectedRelativePath:   "Constructors");
		}
	}
}

