using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	public class ObsoleteAttr : AttributeWriter
	{
		public string Message { get; set; }
		public bool IsError { get; set; }
		public bool NoAtSign { get; set; }	// TODO: Temporary to match unit tests
		public ObsoleteAttr (string message = null, bool isError = false)
		{
			Message = message;
			IsError = isError;
		}

		public override void WriteAttribute (CodeWriter writer)
		{
			if (!Message.HasValue () && !IsError) {
				writer.Write ($"[Obsolete]");
				return;
			}

			writer.Write ($"[Obsolete ({(NoAtSign ? "" : "@")}\"{Message}\"");

			if (IsError)
				writer.Write (", error: true");

			writer.WriteLine (")]");
		}
	}
}
