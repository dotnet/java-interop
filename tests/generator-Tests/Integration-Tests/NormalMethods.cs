using System;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	internal sealed class NormalMethods : BaseGeneratorTest
	{
		protected override bool TryJavaInterop1 => false;

		[Test]
		public void GeneratedOK ()
		{
			RunAllTargets (
					outputRelativePath:     "NormalMethods",
					apiDescriptionFile:     "expected.ji/NormalMethods/NormalMethods.xml",
					expectedRelativePath:   "NormalMethods");
		}
	}
}

