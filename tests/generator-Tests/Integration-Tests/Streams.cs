﻿using System;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	public class Streams : BaseGeneratorTest
	{
		[Test]
		public void GeneratedOK ()
		{
			RunAllTargets (
					outputRelativePath:     "Streams",
					apiDescriptionFile:     "expected/Streams/Streams.xml",
					expectedRelativePath:   "Streams",
					additionalSupportPaths: new[]{ "expected/Streams/SupportFiles" });
		}
	}
}

