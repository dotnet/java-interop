using System;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	internal sealed class AccessModifiers : BaseGeneratorTest
	{
		[Test]
		public void GeneratedOK ()
		{
			RunAllTargets (
					outputRelativePath: "AccessModifiers",
					apiDescriptionFile: "expected.ji/AccessModifiers/AccessModifiers.xml",
					expectedRelativePath: "AccessModifiers",
					additionalSupportPaths: null);
		}
	}
}

