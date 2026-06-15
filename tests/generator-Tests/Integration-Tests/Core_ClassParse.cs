using System;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	internal sealed class Core_ClassParse : BaseGeneratorTest
	{
		[Test]
		public void GeneratedOK ()
		{
			AllowWarnings = true;

			RunAllTargets (
					outputRelativePath: "Core_ClassParse",
					apiDescriptionFile: "expected.ji/Core_ClassParse/api.xml",
					expectedRelativePath: "Core_ClassParse",
					metadataFile: FullPath ("expected.ji/Core_ClassParse/metadata.xml"));
		}
	}
}

