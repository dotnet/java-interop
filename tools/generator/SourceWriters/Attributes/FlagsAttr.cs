using System;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	internal class FlagsAttr : AttributeWriter
	{
		public override void WriteAttribute (CodeWriter writer)
		{
			writer.WriteLine ("[System.Flags]");
		}
	}
}
