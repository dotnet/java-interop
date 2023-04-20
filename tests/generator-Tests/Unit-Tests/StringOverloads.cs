using System;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	public class StringOverloads : BaseGeneratorTest
	{
		protected override bool TryJavaInterop1 => false;

		[Test]
		public void GeneratedOK ()
		{
			RunAllTargets (
					outputRelativePath: "StringOverloads",
					apiDescriptionFile: "expected/StringOverloads/StringOverloads.xml",
					expectedRelativePath: "StringOverloads");
		}
	}
}

