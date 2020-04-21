using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.SourceWriter
{
	public class PropertyWriter
	{
		private Visibility visibility;

		public string Name { get; set; }
		public List<MethodParameterWriter> Parameters { get; } = new List<MethodParameterWriter> ();
		public TypeReferenceWriter PropertyType { get; set; }
		public List<string> Comments { get; } = new List<string> ();
		public List<AttributeWriter> Attributes { get; } = new List<AttributeWriter> ();
		public bool IsPublic { get => visibility.HasFlag (Visibility.Public); set => visibility |= Visibility.Public; }
		public bool UseExplicitPrivateKeyword { get; set; }
		public bool IsInternal { get => visibility.HasFlag (Visibility.Internal); set => visibility |= Visibility.Internal; }
		public List<string> GetBody { get; } = new List<string> ();
		public List<string> SetBody { get; } = new List<string> ();
		public bool IsStatic { get; set; }
		public bool IsProtected { get => visibility.HasFlag (Visibility.Protected); set => visibility |= Visibility.Protected; }
		public bool IsOverride { get; set; }
		public bool HasGet { get; set; }
		public bool HasSet { get; set; }

		public PropertyWriter (string name, TypeReferenceWriter propertyType = null)
		{
			Name = name;
			PropertyType = propertyType ?? TypeReferenceWriter.Void;
		}

		public virtual void WriteMethod (CodeWriter writer)
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

			if (IsOverride)
				writer.Write ("override ");

			if (IsStatic)
				writer.Write ("static ");

			WriteReturnType (writer);

			writer.Write (Name + " ");

			WriteBody (writer);
		}

		protected virtual void WriteBody (CodeWriter writer)
		{
			if (GetBody.Count == 0 && SetBody.Count == 0) {
				WriteAutomaticPropertyBody (writer);
				return;
			}

			writer.WriteLine ("{");

			writer.Indent ();

			WriteGetter (writer);
			WriteSetter (writer);

			writer.Unindent ();

			writer.WriteLine ("}");
		}

		protected virtual void WriteAutomaticPropertyBody (CodeWriter writer)
		{
			writer.Write ("{ ");

			if (HasGet)
				writer.Write ("get; ");
			if (HasSet)
				writer.Write ("set; ");

			writer.WriteLine ("}");
		}

		protected virtual void WriteGetter (CodeWriter writer)
		{
			if (HasGet) {
				if (GetBody.Count == 1)
					writer.WriteLine ("get { " + GetBody [0] + " }");
				else {
					writer.WriteLine ("get {");
					writer.Indent ();

					foreach (var b in GetBody)
						writer.WriteLine (b);

					writer.Unindent ();
					writer.WriteLine ("}");
				}
			}
		}

		protected virtual void WriteSetter (CodeWriter writer)
		{
			if (HasSet) {
				if (SetBody.Count == 1)
					writer.WriteLine ("set { " + SetBody [0] + " }");
				else {
					writer.WriteLine ("set {");
					writer.Indent ();

					foreach (var b in SetBody)
						writer.WriteLine (b);

					writer.Unindent ();
					writer.WriteLine ("}");
				}
			}
		}

		protected virtual void WriteReturnType (CodeWriter writer)
		{
			PropertyType.WriteTypeReference (writer);
		}

		protected virtual void WriteConstructorBaseCall (CodeWriter writer) { }
	}
}
