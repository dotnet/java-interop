using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.SourceWriter
{
	public class MethodWriter
	{
		private Visibility visibility;

		public string Name { get; set; }
		public List<MethodParameterWriter> Parameters { get; } = new List<MethodParameterWriter> ();
		public TypeReferenceWriter ReturnType { get; set; }
		public List<string> Comments { get; } = new List<string> ();
		public List<AttributeWriter> Attributes { get; } = new List<AttributeWriter> ();
		public bool IsPublic { get => visibility.HasFlag (Visibility.Public); set => visibility |= Visibility.Public; }
		public bool UseExplicitPrivateKeyword { get; set; }
		public bool IsInternal { get => visibility.HasFlag (Visibility.Internal); set => visibility |= Visibility.Internal; }
		public List<string> Body { get; set; } = new List<string> ();
		public bool IsStatic { get; set; }
		public bool IsProtected { get => visibility.HasFlag (Visibility.Protected); set => visibility |= Visibility.Protected; }
		public bool IsOverride { get; set; }
		public bool IsUnsafe { get; set; }

		public MethodWriter (string name, TypeReferenceWriter returnType = null)
		{
			Name = name;
			ReturnType = returnType ?? TypeReferenceWriter.Void;
		}

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
			if (IsInternal)
				writer.Write ("internal ");
			if (IsProtected)
				writer.Write ("protected ");
			if (visibility == Visibility.Private && UseExplicitPrivateKeyword)
				writer.Write ("private ");

			if (IsUnsafe)
				writer.Write ("unsafe ");

			if (IsOverride)
				writer.Write ("override ");

			if (IsStatic)
				writer.Write ("static ");

			WriteReturnType (writer);

			writer.Write (Name + " ");
			writer.Write ("(");

			WriteParameters (writer);

			writer.Write (")");

			WriteConstructorBaseCall (writer);

			writer.WriteLine ();
			writer.WriteLine ("{");
			writer.Indent ();

			WriteBody (writer);

			writer.Unindent ();

			writer.WriteLine ("}");
		}

		protected virtual void WriteBody (CodeWriter writer)
		{
			foreach (var s in Body)
				writer.WriteLine (s);
		}

		protected virtual void WriteParameters (CodeWriter writer)
		{
			for (var i = 0; i < Parameters.Count; i++) {
				var p = Parameters [i];
				p.WriteParameter (writer);

				if (i < Parameters.Count - 1)
					writer.Write (", ");
			}
		}

		protected virtual void WriteReturnType (CodeWriter writer)
		{
			ReturnType.WriteTypeReference (writer);
		}

		protected virtual void WriteConstructorBaseCall (CodeWriter writer) { }
	}
}
