using System;
using System.Reflection;

using Xamarin.Android.Tools.Bytecode;

using NUnit.Framework;

namespace Xamarin.Android.Tools.BytecodeTests {

	[TestFixture]
	internal sealed class ClassFileTests {

		[Test]
		public void Constructor_Exceptions ()
		{
			Assert.Throws<ArgumentNullException> (() => new ClassFile (null));
		}
	}
}

