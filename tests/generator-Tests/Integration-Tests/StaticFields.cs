using System;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	internal sealed class StaticFields : BaseGeneratorTest
	{
		[Test]
		public void GeneratedOK ()
		{
			RunAllTargets (
					outputRelativePath:     "StaticFields",
					apiDescriptionFile:     "expected.ji/StaticFields/StaticField.xml",
					expectedRelativePath:   "StaticFields");
		}
	}
}

