using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.SourceWriter
{
	public class FieldWriter
	{
		public string Name { get; set; }
		public TypeReferenceWriter Type { get; set; }
		public List<string> Comments { get; } = new List<string> ();
		public List<AttributeWriter> Attributes { get; } = new List<AttributeWriter> ();
		public bool IsPublic { get; set; }
		public bool UseExplicitPrivateKeyword { get; set; }
		public bool IsInternal { get; set; }
		public bool IsConst { get; set; }
		public string Value { get; set; }
		public bool IsStatic { get; set; }
		public bool IsReadonly { get; set; }

		public FieldWriter (string name, TypeReferenceWriter Type = null)
		{
			Name = name;
			this.Type = Type ?? TypeReferenceWriter.Void;
		}

		public virtual void WriteField (CodeWriter writer)
		{
			WriteComments (writer);
			WriteAttributes (writer);
			WriteSignature (writer);
		}

		public virtual void WriteComments (CodeWriter writer)
		{
			foreach (var c in Comments)
				writer.WriteLine (c);
		}

		public virtual void WriteAttributes (CodeWriter writer)
		{
			foreach (var att in Attributes)
				att.WriteAttribute (writer);
		}

		public virtual void WriteSignature (CodeWriter writer)
		{
			if (IsPublic)
				writer.Write ("public ");
			else if (IsInternal)
				writer.Write ("internal ");
			else if (UseExplicitPrivateKeyword)
				writer.Write ("private ");

			if (IsStatic)
				writer.Write ("static ");
			if (IsReadonly)
				writer.Write ("readonly ");
			if (IsConst)
				writer.Write ("const ");

			WriteType (writer);

			if (Value.HasValue ()) {
				writer.Write (Name + " = ");
				writer.Write (Value);
				writer.WriteLine (";");
			} else {
				writer.WriteLine ($"{Name};");
			}
		}

		protected virtual void WriteType (CodeWriter writer)
		{
			Type.WriteTypeReference (writer);
		}
	}
}
