using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Java.Interop.Tools.JavaTypeSystem
{
	public static class XmlExtensions
	{
		public static string XGetAttribute (this XElement element, string name)
		{
			return element.Attribute (name)?.Value.Trim () ?? string.Empty;
		}
	}
}
