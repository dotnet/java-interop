using System;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	internal sealed class StaticProperties : BaseGeneratorTest
	{
		[Test]
		public void GeneratedOK ()
		{
			RunAllTargets (
					outputRelativePath:     "StaticProperties",
					apiDescriptionFile:     "expected.ji/StaticProperties/StaticProperties.xml",
					expectedRelativePath:   "StaticProperties");
		}
	}
}

