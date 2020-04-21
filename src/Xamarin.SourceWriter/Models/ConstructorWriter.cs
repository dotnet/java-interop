using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.SourceWriter
{
	public class ConstructorWriter : MethodWriter
	{
		public string BaseCall { get; set; }
		
		public ConstructorWriter (string name) : base (name)
		{
			Name = name;
		}

		protected override void WriteReturnType (CodeWriter writer)
		{
		}

		protected override void WriteConstructorBaseCall (CodeWriter writer)
		{
			if (BaseCall.HasValue ())
				writer.Write ($" : {BaseCall}");
		}
	}
}
