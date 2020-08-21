using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Xamarin.Android.Tools.ApiXmlAdjuster
{
	class XmlUtil
	{
		static string GetLocation (XmlReader reader)
		{
			var li = reader as IXmlLineInfo;
			return string.Format ("{0} ({1},{2})", string.IsNullOrEmpty (reader.BaseURI) ? null : new Uri (reader.BaseURI).LocalPath, li.LineNumber, li.LinePosition);
		}
		
		public static Exception UnexpectedElementOrContent (string elementName, XmlReader reader, params string [] expected)
		{
			return new Exception (string.Format (Java.Interop.Localization.Resources.ApiXmlAdjuster_0012,
				GetLocation (reader), elementName ?? "(top level)", reader.NodeType, reader.LocalName, string.Join (", ", expected)));
		}
		
		public static Exception UnexpectedAttribute (XmlReader reader, string elementName, params string [] expected)
		{
			if (reader.NodeType != XmlNodeType.Attribute)
				throw new ArgumentException (string.Format (Java.Interop.Localization.Resources.ApiXmlAdjuster_0009, reader.NodeType));
			return new Exception (string.Format (Java.Interop.Localization.Resources.ApiXmlAdjuster_0013,
				GetLocation (reader), elementName, reader.LocalName, string.Join (", ", expected)));
		}

		internal static string GetRequiredAttribute (XmlReader reader, string name)
		{
			var value = reader.GetAttribute (name);
			if (value == null)
				throw new Exception (string.Format (Java.Interop.Localization.Resources.ApiXmlAdjuster_0010, GetLocation (reader), reader.LocalName, name));
			return value;
		}

		internal static void VerifyEndElement (XmlReader reader, string elementName)
		{
			if (reader.NodeType != XmlNodeType.EndElement || reader.LocalName != elementName)
				throw new Exception (string.Format (Java.Interop.Localization.Resources.ApiXmlAdjuster_0011,
					GetLocation (reader), elementName, reader.NodeType, reader.LocalName));
		}

		internal static void CheckExtraneousAttributes (string elementName, XmlReader reader, params string [] expected)
		{
			if (reader.MoveToFirstAttribute ()) {
				do {
					if (!expected.Contains (reader.LocalName))
						throw XmlUtil.UnexpectedAttribute (reader, elementName, expected);
				} while (reader.MoveToNextAttribute ());
			}
			reader.MoveToElement ();
		}
	}
	
}
