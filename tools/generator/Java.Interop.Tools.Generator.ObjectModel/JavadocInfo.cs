using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Irony.Parsing;

using Java.Interop.Tools.JavaSource;

namespace MonoDroid.Generation
{
	enum ApiLinkStyle {
		None,
		DeveloperAndroidComReference_2020Nov,
	}

	public sealed class JavadocInfo {

		public  string          Javadoc             { get; set; }

		public  XElement[]      ExtraRemarks        { get; set; }

		public  XmldocStyle     XmldocStyle         { get; set; }

		string  MemberDescription;

		public static JavadocInfo CreateInfo (XElement element, XmldocStyle style)
		{
			if (element == null) {
				return null;
			}

			string javadoc                  = element.Element ("javadoc")?.Value;

			var desc                        = GetMemberDescription (element);
			string declaringJniType         = desc.DeclaringJniType;
			string declaringMemberName      = desc.DeclaringMemberName;
			var declaringMemberJniSignature = desc.DeclaringMemberJniSignature;

			XElement[]  extra               = GetExtra (element, style, declaringJniType, declaringMemberName, declaringMemberJniSignature);

			if (string.IsNullOrEmpty (javadoc) && extra == null)
				return null;

			var info = new JavadocInfo () {
				ExtraRemarks        = extra,
				Javadoc             = javadoc,
				MemberDescription   = declaringMemberName == null
					? declaringJniType
					: $"{declaringJniType}.{declaringMemberName}.{declaringMemberJniSignature}",
				XmldocStyle         = style,
			};
			return info;
		}

		static (string DeclaringJniType, string DeclaringMemberName, string DeclaringMemberJniSignature) GetMemberDescription (XElement element)
		{
			bool isType     = element.Name.LocalName == "class" ||
				element.Name.LocalName == "interface";

			string declaringJniType             = isType
				? (string) element.Attribute ("jni-signature")
				: (string) element.Parent.Attribute ("jni-signature");
			if (declaringJniType.StartsWith ("L", StringComparison.Ordinal) &&
					declaringJniType.EndsWith (";", StringComparison.Ordinal)) {
				declaringJniType = declaringJniType.Substring (1, declaringJniType.Length-2);
			}

			string declaringMemberName          = isType
				? null
				: (string) element.Attribute ("name") ?? declaringJniType.Substring (declaringJniType.LastIndexOf ('/')+1);
			string declaringMemberJniSignature  = isType
				? null
				: (string) element.Attribute ("jni-signature");

			return (declaringJniType, declaringMemberName, declaringMemberJniSignature);
		}

		static XElement[] GetExtra (XElement element, XmldocStyle style, string declaringJniType, string declaringMemberName, string declaringMemberJniSignature)
		{
			if (!style.HasFlag (XmldocStyle.IntelliSenseAndExtraRemarks))
				return null;

			XElement javadocMetadata    = null;
			while (element != null) {
				javadocMetadata = element.Element ("javadoc-metadata");
				if (javadocMetadata != null) {
					break;
				}
				element         = element.Parent;
			}

			List<XElement>  extra   = null;
			if (javadocMetadata != null) {
				var link            = javadocMetadata.Element ("link");
				var urlPrefix       = (string) link.Attribute ("prefix");
				var linkStyle       = (string) link.Attribute ("style");
				var kind            = ParseApiLinkStyle (linkStyle);

				XElement docLink	= null;
				if (!string.IsNullOrEmpty (urlPrefix)) {
					docLink         = CreateDocLinkUrl (kind, urlPrefix, declaringJniType, declaringMemberName, declaringMemberJniSignature);
				}
				extra           = new List<XElement> ();
				extra.Add (docLink);
				extra.AddRange (javadocMetadata.Element ("copyright").Elements ());
			}
			return extra?.ToArray ();
		}

		static ApiLinkStyle ParseApiLinkStyle (string style)
		{
			switch (style) {
				case "developer.android.com/reference@2020-Nov":
					return ApiLinkStyle.DeveloperAndroidComReference_2020Nov;
				default:
					return ApiLinkStyle.None;
			}
		}


		public void AddJavadocs (ICollection<string> comments)
		{
			var nodes = ParseJavadoc ();
			AddComments (comments, nodes);
		}

		public IEnumerable<XNode> ParseJavadoc ()
		{
			if (string.IsNullOrWhiteSpace (Javadoc))
				return Enumerable.Empty<XNode> ();

			Javadoc         = Javadoc.Trim ();

			ParseTree           tree    = null;
			IEnumerable<XNode>  nodes   = null;

			try {
				var parser  = new SourceJavadocToXmldocParser (XmldocStyle) {
					ExtraRemarks    = ExtraRemarks,
				};
				nodes       = parser.TryParse (Javadoc, fileName: null, out tree);
			}
			catch (Exception e) {
				Console.Error.WriteLine ($"## Exception translating remarks: {e.ToString ()}");
			}

			if (tree != null && tree.HasErrors ()) {
				Console.Error.WriteLine ($"## Unable to translate remarks for {MemberDescription}:");
				Console.Error.WriteLine ("```");
				Console.Error.WriteLine (Javadoc);
				Console.Error.WriteLine ("```");
				PrintMessages (tree, Console.Error);
				Console.Error.WriteLine ();
			}

			return nodes;
		}

		public static void AddComments (ICollection<string> comments, IEnumerable<XNode> nodes)
		{
			if (nodes == null)
				return;

			foreach (var node in nodes) {
				AddNode (comments, node);
			}
		}

		static void AddNode (ICollection<string> comments, XNode node)
		{
			if (node == null)
				return;
			var contents = node.ToString ();

			var lines = new StringReader (contents);
			string line;
			while ((line = lines.ReadLine ()) != null) {
				comments.Add ($"/// {line}");
			}
		}

		static void PrintMessages (ParseTree tree, TextWriter writer)
		{
			var lines   = GetLines (tree.SourceText);
			foreach (var m in tree.ParserMessages) {
				writer.WriteLine ($"JavadocImport-{m.Level} {m.Location}: {m.Message}");
				writer.WriteLine (lines [m.Location.Line]);
				writer.Write (new string (' ', m.Location.Column));
				writer.WriteLine ("^");
			}
		}

		static List<string> GetLines (string text)
		{
			var lines = new List<string>();
			var reader = new StringReader (text);
			string line;
			while ((line = reader.ReadLine()) != null) {
				lines.Add (line);
			}
			return lines;
		}

		static Dictionary<ApiLinkStyle, Func<string, string, string, string, XElement>> UrlCreators = new Dictionary<ApiLinkStyle, Func<string, string, string, string, XElement>> {
			[ApiLinkStyle.DeveloperAndroidComReference_2020Nov] = CreateAndroidDocLinkUri,
		};

		static XElement CreateDocLinkUrl (ApiLinkStyle style, string prefix, string declaringJniType, string declaringMemberName, string declaringMemberJniSignature)
		{
			;
			if (style == ApiLinkStyle.None || prefix == null || declaringJniType == null)
				return null;
			if (UrlCreators.TryGetValue (style, out var creator)) {
				return creator (prefix, declaringJniType, declaringMemberName, declaringMemberJniSignature);
			}
			return null;
		}

		static XElement CreateAndroidDocLinkUri (string prefix, string declaringJniType, string declaringMemberName, string declaringMemberJniSignature)
		{
			// URL is:
			//  * {prefix}
			//  * declaring type in JNI format
			//  * when `declaringJniMemberName` != null, `#{declaringJniMemberName}`
			//  * for methods & constructors, a `(`, the arguments in *Java* syntax -- separated by `, ` -- and `)`
			//
			// Example: https://developer.android.com/reference/android/app/Application#registerOnProvideAssistDataListener(android.app.Application.OnProvideAssistDataListener)

			var java    = new StringBuilder (declaringJniType)
				.Replace ("/", ".")
				.Replace ("$", ".");
			var url     = new StringBuilder (prefix);
			if (!prefix.EndsWith ("/")) {
				url.Append ("/");
			}
			url.Append (declaringJniType);

			if (declaringMemberName != null) {
				java.Append (".").Append (declaringMemberName);
				url.Append ("#").Append (declaringMemberName);
				if (declaringMemberJniSignature?.StartsWith ("(", StringComparison.Ordinal) ?? false) {
					java.Append ("(");
					url.Append ("(");
					AppendJavaParameterTypes (java, declaringMemberJniSignature);
					AppendJavaParameterTypes (url, declaringMemberJniSignature);
					java.Append (")");
					url.Append (")");
				}
			}
			var format  = new XElement ("format",
					new XAttribute ("type", "text/html"),
					new XElement ("a",
						new XAttribute ("href", new Uri (url.ToString ()).AbsoluteUri),
						new XAttribute ("title", "Reference documentation"),
						"Java documentation for ",
						new XElement ("tt", java.ToString ()),
						"."));
			return new XElement ("para", format);
		}

		static StringBuilder AppendJavaParameterTypes (StringBuilder builder, string declaringMemberJniSignature)
		{
			if (string.IsNullOrEmpty (declaringMemberJniSignature) || declaringMemberJniSignature [0] != '(')
				return builder;

			int startLen = builder.Length;

			for (int i = 1; i < declaringMemberJniSignature.Length; ++i) {
				if (declaringMemberJniSignature [i] == ')')
					break;
				AppendComma ();
				AppendJavaParameterType (builder, declaringMemberJniSignature, ref i);
			}

			return builder;

			void AppendComma ()
			{
				if (startLen == builder.Length)
					return;
				builder.Append (", ");
			}
		}

		static void AppendJavaParameterType (StringBuilder builder, string declaringMemberJniSignature, ref int i)
		{
			switch (declaringMemberJniSignature [i]) {
				case '[': {
					++i;
					AppendJavaParameterType (builder, declaringMemberJniSignature, ref i);
					builder.Append ("[]");
					break;
				}
				case 'B': {
					builder.Append ("byte");
					break;
				}
				case 'C': {
					builder.Append ("char");
					break;
				}
				case 'D': {
					builder.Append ("double");
					break;
				}
				case 'F': {
					builder.Append ("float");
					break;
				}
				case 'I': {
					builder.Append ("int");
					break;
				}
				case 'J': {
					builder.Append ("long");
					break;
				}
				case 'L': {
					int end = declaringMemberJniSignature.IndexOf (';', i);
					if (end < 0)
						throw new InvalidOperationException ($"INTERNAL ERROR: Invalid JNI signature '{declaringMemberJniSignature}': no ';' to end 'L' at index {i}!");
					var type    = declaringMemberJniSignature.Substring (i+1, end - i - 1)
						.Replace ('/', '.')
						.Replace ('$', '.');
					builder.Append (type);
					i           = end;
					break;
				}
				case 'S': {
					builder.Append ("short");
					break;
				}
				case 'Z': {
					builder.Append ("boolean");
					break;
				}
				default:
					throw new NotSupportedException ($"INTERNAL ERROR: Don't know what to do with '{declaringMemberJniSignature [i]}' in '{declaringMemberJniSignature}'!");
			}
		}
	}
}
