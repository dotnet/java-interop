using System;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	public class CovariantReturnTypes : BaseGeneratorTest
	{
		[Test]
		public void GeneratedOK ()
		{
			AllowWarnings = true;
			RunAllTargets (
					outputRelativePath: "CovariantReturnTypes",
					apiDescriptionFile: "expected/CovariantReturnTypes/CovariantReturnTypes.xml",
					expectedRelativePath: "CovariantReturnTypes");
		}
	}
}

