using System;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	internal sealed class ParameterXPath : BaseGeneratorTest
	{
		protected override bool TryJavaInterop1 => true;

		[Test]
		public void GeneratedOK ()
		{
			RunAllTargets (
					outputRelativePath:     "ParameterXPath",
					apiDescriptionFile:     "expected.ji/ParameterXPath/ParameterXPath.xml",
					expectedRelativePath:   "ParameterXPath");
		}
	}
}

