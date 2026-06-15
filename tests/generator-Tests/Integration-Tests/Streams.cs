using System;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	internal sealed class Streams : BaseGeneratorTest
	{
		private static readonly string [] additionalSupportPaths = new []{ "expected.ji/Streams/SupportFiles" };

		[Test]
		public void GeneratedOK ()
		{
			RunAllTargets (
					outputRelativePath:     "Streams",
					apiDescriptionFile:     "expected.ji/Streams/Streams.xml",
					expectedRelativePath:   "Streams",
					additionalSupportPaths: additionalSupportPaths);
		}
	}
}

