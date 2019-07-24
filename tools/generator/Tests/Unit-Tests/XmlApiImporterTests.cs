using System;
using System.Xml.Linq;
using MonoDroid.Generation;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	public class XmlApiImporterTests
	{
		[Test]
		public void CreateField_StudlyCaseName ()
		{
			var xml = XDocument.Parse ("<field name=\"_DES_EDE_CBC\" />");
			var field = XmlApiImporter.CreateField (xml.Root);

			Assert.AreEqual ("DesEdeCbc", field.Name);
		}

		[Test]
		public void CreateField_EnsureValidName ()
		{
			var xml = XDocument.Parse ("<field name=\"_3DES_EDE_CBC\" />");
			var field = XmlApiImporter.CreateField (xml.Root);

			Assert.AreEqual ("_3desEdeCbc", field.Name);
		}
	}
}
