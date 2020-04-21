using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.SourceWriter
{
	public class MethodParameterWriter
	{
		public TypeReferenceWriter Type { get; set; }
		public List<AttributeWriter> Attributes { get; } = new List<AttributeWriter> ();
		public string Name { get; set; }

		public MethodParameterWriter (string name, TypeReferenceWriter type)
		{
			Name = name;
			Type = type;
		}

		public virtual void WriteParameter (CodeWriter writer)
		{
			WriteAttributes (writer);

			Type.WriteTypeReference (writer);
			writer.Write (Name);
		}

		public virtual void WriteAttributes (CodeWriter writer)
		{
			foreach (var att in Attributes)
				att.WriteAttribute (writer);
		}
	}
}
