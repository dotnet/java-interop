using System;
using MonoDroid.Generation;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	public class CodeGenerationOptionsTests
	{
		[Test]
		public void GetOutputNameUseGlobal ()
		{
			var opt = new CodeGenerationOptions { UseGlobal = true };

			Assert.AreEqual (string.Empty, opt.GetOutputName (string.Empty));
			Assert.AreEqual ("int", opt.GetOutputName ("int"));
			Assert.AreEqual ("void", opt.GetOutputName ("void"));
			Assert.AreEqual ("void", opt.GetOutputName ("System.Void"));
			Assert.AreEqual ("params int[]", opt.GetOutputName ("params int[]"));
			Assert.AreEqual ("params global::System.Object[]", opt.GetOutputName ("params System.Object[]"));
			Assert.AreEqual ("int[][][]", opt.GetOutputName ("int[][][]"));
			Assert.AreEqual ("global::System.Object[][][]", opt.GetOutputName ("System.Object[][][]"));

			Assert.AreEqual ("global::System.Collections.Generic.List<string[]>",
				opt.GetOutputName ("System.Collections.Generic.List<string[]>"));

			Assert.AreEqual ("global::System.Collections.Generic.Dictionary<string, string>",
				opt.GetOutputName ("System.Collections.Generic.Dictionary<string, string>"));

			Assert.AreEqual ("global::System.Collections.Generic.List<global::System.Collections.Generic.List<string>>",
				opt.GetOutputName ("System.Collections.Generic.List<System.Collections.Generic.List<string>>"));

			Assert.AreEqual ("global::System.Collections.Generic.List<global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.Dictionary<global::System.Object, global::System.Object>>>",
				opt.GetOutputName ("System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<System.Object, System.Object>>>"));

			Assert.AreEqual ("global::System.Collections.Generic.IList<global::Kotlin.Pair>",
				opt.GetOutputName ("System.Collections.Generic.IList<Kotlin.Pair>"));

			Assert.AreEqual ("global::System.Collections.Generic.IDictionary<string, global::System.Collections.Generic.IList<global::Kotlin.Pair>>",
				opt.GetOutputName ("System.Collections.Generic.IDictionary<string, System.Collections.Generic.IList<Kotlin.Pair>>"));

			Assert.AreEqual ("global::System.Collections.Generic.IDictionary<global::System.Collections.Generic.IList<string>, global::Kotlin.Pair>",
				opt.GetOutputName ("System.Collections.Generic.IDictionary<System.Collections.Generic.IList<string>, Kotlin.Pair>"));

			Assert.AreEqual ("global::System.Collections.Generic.IDictionary<global::System.Collections.Generic.IList<string>, global::System.Collections.Generic.IList<global::Kotlin.Pair>>",
				opt.GetOutputName ("System.Collections.Generic.IDictionary<System.Collections.Generic.IList<string>, System.Collections.Generic.IList<Kotlin.Pair>>"));

			Assert.AreEqual ("global::System.Collections.Generic.List<global::System.Collections.Generic.List<string>[]>[]",
				opt.GetOutputName ("System.Collections.Generic.List<System.Collections.Generic.List<string>[]>[]"));

			Assert.AreEqual ("global::System.Collections.Generic.List<global::System.Collections.Generic.List<string>.Enumerator[]>",
				opt.GetOutputName ("System.Collections.Generic.List<System.Collections.Generic.List<string>.Enumerator[]>"));
		}
	}
}
