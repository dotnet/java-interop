using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	public class RegisterAttr : AttributeWriter
	{
		public string Name { get; set; }
		public string Signature { get; set; }
		public string Connector { get; set; }
		public bool DoNotGenerateAcw { get; set; }
		public string AdditionalProperties { get; set; }

		public RegisterAttr (string name, string signature, string connector, bool noAcw = false, string additionalProperties = null)
		{
			Name = name;
			Signature = signature;
			Connector = connector;
			DoNotGenerateAcw = noAcw;
			AdditionalProperties = additionalProperties;
		}

		public override void WriteAttribute (CodeWriter writer)
		{
			var sb = new StringBuilder ();

			sb.Append ($"[Register (\"{Name}\"");

			// TODO: We shouldn't write these when they aren't needed, but to be compatible
			// with existing unit tests we're always writing them currently
			//if (Signature.HasValue () || Connector.HasValue ())
				sb.Append ($", \"{Signature}\", \"{Connector}\"");

			if (DoNotGenerateAcw)
				sb.Append (", DoNotGenerateAcw=true");

			if (AdditionalProperties.HasValue ())
				sb.Append (AdditionalProperties);

			sb.Append (")]");

			writer.WriteLine (sb.ToString ());
		}
	}
}
