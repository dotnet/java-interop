using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.Android.Tools.ApiXmlAdjuster;

namespace Xamarin.Android.Tools.ApiXmlAdjusterTests
{
	[TestFixture]
	public class StripNonBindablesTests
	{
		[Test]
		public void StripHyphenClasses ()
		{
			var api = new JavaApi ();
			var package = new JavaPackage (api) { Name = "com.foo.java" };
			var bad_type = new JavaClass (package) { Name = "-Base64" };
			var good_type = new JavaClass (package) { Name = "Base64" };

			api.Packages.Add (package);
			package.Types.Add (bad_type);
			package.Types.Add (good_type);

			api.StripNonBindables ();

			Assert.AreEqual (1, package.Types.Count);
			Assert.AreEqual ("Base64", package.Types [0].Name);
		}

		[Test]
		public void StripHyphenMembers ()
		{
			var api = new JavaApi ();
			var package = new JavaPackage (api) { Name = "com.foo.java" };
			var type = new JavaClass (package) { Name = "Base64" };
			var good_member = new JavaMethod (type) { Name = "DoSomething" };
			var bad_member = new JavaMethod (type) { Name = "-DoSomething" };

			api.Packages.Add (package);
			package.Types.Add (type);
			type.Members.Add (bad_member);
			type.Members.Add (good_member);

			api.StripNonBindables ();

			Assert.AreEqual (1, type.Members.Count);
			Assert.AreEqual ("DoSomething", type.Members [0].Name);
		}

		[Test]
		public void StripDollarSignMembers ()
		{
			var api = new JavaApi ();
			var package = new JavaPackage (api) { Name = "com.foo.java" };
			var type = new JavaClass (package) { Name = "Base64" };
			var good_member = new JavaMethod (type) { Name = "DoSomething" };
			var bad_member = new JavaMethod (type) { Name = "DoSome$thing" };

			api.Packages.Add (package);
			package.Types.Add (type);
			type.Members.Add (bad_member);
			type.Members.Add (good_member);

			api.StripNonBindables ();

			Assert.AreEqual (1, type.Members.Count);
			Assert.AreEqual ("DoSomething", type.Members [0].Name);
		}
	}
}
