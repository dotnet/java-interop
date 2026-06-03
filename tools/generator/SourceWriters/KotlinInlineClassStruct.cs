using System;
using MonoDroid.Generation;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	// Emits a `readonly struct` wrapper for a Kotlin @JvmInline value class.
	// The struct holds the underlying primitive (e.g. long, float) that the
	// JVM passes across JNI, and provides implicit conversions so the wrapper
	// can be used directly in projected method signatures while existing JNI
	// thunks marshal the primitive value unchanged.
	public class KotlinInlineClassStruct : TypeWriter
	{
		readonly ClassGen klass;
		readonly string underlying_csharp_type;

		public KotlinInlineClassStruct (ClassGen klass, CodeGenerationOptions opt)
		{
			this.klass = klass;
			underlying_csharp_type = JniPrimitiveToCSharpType (klass.KotlinInlineClassUnderlyingJniType);

			Name = klass.Name;
			SetVisibility (klass.Visibility);

			klass.JavadocInfo?.AddJavadocs (Comments);
			Comments.Add ($"// Metadata.xml XPath class reference: path=\"{klass.MetadataXPathReference}\"");
			Comments.Add ("// Kotlin @JvmInline value class wrapper.");
		}

		public override void WriteSignature (CodeWriter writer)
		{
			if (IsPublic)
				writer.Write ("public ");
			else if (IsInternal)
				writer.Write ("internal ");

			writer.Write ("readonly partial struct ");
			writer.Write (Name + " ");
			writer.WriteLine (": global::System.IEquatable<" + Name + "> {");
			writer.Indent ();
		}

		public override void WriteMembers (CodeWriter writer)
		{
			var t = underlying_csharp_type;
			var n = Name;

			writer.WriteLine ($"public readonly {t} Value;");
			writer.WriteLine ();
			writer.WriteLine ($"public {n} ({t} value) {{ Value = value; }}");
			writer.WriteLine ();
			writer.WriteLine ($"public static implicit operator {t} ({n} value) => value.Value;");
			writer.WriteLine ($"public static implicit operator {n} ({t} value) => new {n} (value);");
			writer.WriteLine ();
			writer.WriteLine ($"public static bool operator == ({n} left, {n} right) => left.Equals (right);");
			writer.WriteLine ($"public static bool operator != ({n} left, {n} right) => !left.Equals (right);");
			writer.WriteLine ();
			writer.WriteLine ($"public bool Equals ({n} other) => Value.Equals (other.Value);");
			writer.WriteLine ($"public override bool Equals (object? obj) => obj is {n} other && Equals (other);");
			writer.WriteLine ("public override int GetHashCode () => Value.GetHashCode ();");
			writer.WriteLine ("public override string? ToString () => Value.ToString ();");
		}

		static string JniPrimitiveToCSharpType (string jni)
		{
			return jni switch {
				"Z" => "bool",
				"B" => "sbyte",
				"C" => "char",
				"D" => "double",
				"F" => "float",
				"I" => "int",
				"J" => "long",
				"S" => "short",
				_ => "long",
			};
		}
	}
}
