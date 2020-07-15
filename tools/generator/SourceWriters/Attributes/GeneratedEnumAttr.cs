using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	public class GeneratedEnumAttr : AttributeWriter
	{
		readonly bool is_return;

		public GeneratedEnumAttr (bool isReturn = false) => is_return = isReturn;

		public override void WriteAttribute (CodeWriter writer)
		{
			writer.Write ($"[{(is_return ? "return:" : string.Empty)}global::Android.Runtime.GeneratedEnum]");
		}
	}
}
