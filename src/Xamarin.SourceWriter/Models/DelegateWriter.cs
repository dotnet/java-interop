using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.SourceWriter
{
	public class DelegateWriter : ISourceWriter
	{
		private Visibility visibility;

		public string Name { get; set; }
		public TypeReferenceWriter Type { get; set; }
		public List<string> Comments { get; } = new List<string> ();
		public List<AttributeWriter> Attributes { get; } = new List<AttributeWriter> ();
		public bool IsPublic { get => visibility == Visibility.Public; set => visibility = value ? Visibility.Public : Visibility.Default; }
		public bool UseExplicitPrivateKeyword { get; set; }
		public bool IsInternal { get => visibility == Visibility.Internal; set => visibility = value ? Visibility.Internal : Visibility.Default; }
		public bool IsConst { get; set; }
		public string Value { get; set; }
		public bool IsStatic { get; set; }
		public bool IsReadonly { get; set; }
		public bool IsPrivate { get => visibility == Visibility.Private; set => visibility = value ? Visibility.Private : Visibility.Default; }
		public bool IsProtected { get => visibility == Visibility.Protected; set => visibility = value ? Visibility.Protected : Visibility.Default; }
		public int Priority { get; set; }
		public bool IsShadow { get; set; }
		public string Signature { get; set; }

		public void SetVisibility (string visibility)
		{
			switch (visibility?.ToLowerInvariant ()) {
				case "public":
					IsPublic = true;
					break;
				case "internal":
					IsInternal = true;
					break;
				case "protected":
					IsProtected = true;
					break;
				case "private":
					IsPrivate = true;
					break;
			}
		}

		public virtual void Write (CodeWriter writer)
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
			else if (IsPrivate)
				writer.Write ("private ");

			if (IsStatic)
				writer.Write ("static ");
			if (IsReadonly)
				writer.Write ("readonly ");
			if (IsConst)
				writer.Write ("const ");

			if (IsShadow)
				writer.Write ("new ");

			WriteType (writer);

			writer.Write (Name + " ");
			writer.Write ($"({Signature})");
		}

		protected virtual void WriteType (CodeWriter writer)
		{
			Type.WriteTypeReference (writer);
		}
	}
}
