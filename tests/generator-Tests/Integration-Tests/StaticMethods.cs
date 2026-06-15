using System;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	internal sealed class StaticMethods : BaseGeneratorTest
	{
		[Test]
		public void GeneratedOK ()
		{
			RunAllTargets (
					outputRelativePath:     "StaticMethods",
					apiDescriptionFile:     "expected.ji/StaticMethods/StaticMethod.xml",
					expectedRelativePath:   "StaticMethods");
		}
	}
}

