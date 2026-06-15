using System;

using Java.Interop;

using NUnit.Framework;

namespace Java.InteropTests
{
	[TestFixture]
	public class JniMarshalTests
	{
		private static readonly int [] objA = new []{ 1, 2, 3 };
		private static readonly int [] objB = new []{ 1, 2, 3 };

		[Test]
		public void RecursiveEquals ()
		{
			Assert.IsTrue (JniMarshal.RecursiveEquals (null, null));
			Assert.IsFalse (JniMarshal.RecursiveEquals (null, new object ()));
			Assert.IsFalse (JniMarshal.RecursiveEquals (new object (), null));
			Assert.IsTrue (JniMarshal.RecursiveEquals (1, 1));
			Assert.IsFalse (JniMarshal.RecursiveEquals (1, 2));
			Assert.IsTrue (JniMarshal.RecursiveEquals (objA, objB));
			Assert.IsFalse (JniMarshal.RecursiveEquals (objA, new[]{ 1, 2 }));
			Assert.IsFalse (JniMarshal.RecursiveEquals (new[]{ 1, 2 }, objB));
			Assert.IsFalse (JniMarshal.RecursiveEquals (new[]{ 1, 2 }, null));
			Assert.IsFalse (JniMarshal.RecursiveEquals (null, new[]{ 1, 2 }));
			Assert.IsTrue (JniMarshal.RecursiveEquals (
				new[]{new[]{1,2,3}, new[]{4,5,6}},
				new[]{new[]{1,2,3}, new[]{4,5,6}}
			));
			Assert.IsFalse (JniMarshal.RecursiveEquals (
				new[]{new[]{1,2,3}, new[]{4,5}},
				new[]{new[]{1,2,3}, new[]{4,5,6}}
			));
		}
	}
}

