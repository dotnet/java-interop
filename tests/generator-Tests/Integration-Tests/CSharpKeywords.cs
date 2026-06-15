using System;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	internal sealed class CSharpKeywords : BaseGeneratorTest
	{
		[Test]
		public void GeneratedOK ()
		{
			RunAllTargets (
					outputRelativePath:     "CSharpKeywords",
					apiDescriptionFile:     "expected.ji/CSharpKeywords/CSharpKeywords.xml",
					expectedRelativePath:   "CSharpKeywords");
		}
	}
}

