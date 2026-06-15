using System;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	internal sealed class InterfaceMethodsConflict : BaseGeneratorTest
	{
		[Test]
		public void GeneratedOK ()
		{
			RunAllTargets (
					outputRelativePath:     "InterfaceMethodsConflict",
					apiDescriptionFile:     "expected.ji/InterfaceMethodsConflict/InterfaceMethodsConflict.xml",
					expectedRelativePath:   "InterfaceMethodsConflict");
		}
	}
}

