using System;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	internal sealed class Adapters : BaseGeneratorTest
	{
		protected override bool TryJavaInterop1 => true;

		private static readonly string [] additionalSupportPaths = new []{ "expected.ji/Adapters/SupportFiles" };

		[Test]
		public void GeneratedOK ()
		{
			RunAllTargets (
					outputRelativePath:     "Adapters",
					apiDescriptionFile:     "expected.ji/Adapters/Adapters.xml",
					expectedRelativePath:   "Adapters",
					additionalSupportPaths: additionalSupportPaths);
		}
	}
}

