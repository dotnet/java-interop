using System;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	internal sealed class Android_Graphics_Color : BaseGeneratorTest
	{
		[Test]
		public void GeneratedOK ()
		{
			RunAllTargets (
					outputRelativePath:     "Android.Graphics.Color",
					apiDescriptionFile:     "expected.ji/Android.Graphics.Color/Android.Graphics.Color.xml",
					expectedRelativePath:   "Android.Graphics.Color");
		}
	}
}

