using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Test.ME {

	[Register ("test/me/InputTest", DoNotGenerateAcw=true)]
	public abstract class InputTest : Java.Lang.Object {

		internal InputTest ()
		{
		}

		// Metadata.xml XPath field reference: path="/api/package[@name='test.me']/interface[@name='InputTest']/field[@name='TYPE_CLASS_DATETIME']"
		[Register ("TYPE_CLASS_DATETIME")]
		public const int TypeClassDatetime = (int) 4;

		// Metadata.xml XPath field reference: path="/api/package[@name='test.me']/interface[@name='InputTest']/field[@name='TYPE_CLASS_NUMBER']"
		[Register ("TYPE_CLASS_NUMBER")]
		public const int TypeClassNumber = (int) 2;
	}

	[Register ("test/me/InputTest", DoNotGenerateAcw=true)]
	[global::System.Obsolete ("Use the 'InputTest' type. This type will be removed in a future release.", error: true)]
	public abstract class InputTestConsts : InputTest {

		private InputTestConsts ()
		{
		}
	}

	// Metadata.xml XPath interface reference: path="/api/package[@name='test.me']/interface[@name='InputTest']"
	public partial interface IInputTest : IJavaObject {

	}

}
