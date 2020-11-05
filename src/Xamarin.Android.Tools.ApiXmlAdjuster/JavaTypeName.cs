using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Android.Tools.ApiXmlAdjuster
{
	// An internal Type that represents a parsed Java type name structure.
	// A type name string is passed to Parse() method to create this instance
	// (the only place you can create an instance of this type), then it is
	// used to create JavaTypeReference instance.
	//
	// The structure does not depend on any context type information.
	//
	// It is constructed from a "dotted" name.
	// Unlike JNI-based names, there is no way to determine that a dot is within
	// package name, between package and class, or for nested class.
	// Hence it is impossible to get a package and a local names without
	// context type information. Such analyses should be done at JavaTypeReference.
	//
	// A generic type parameter, or a primitive type, is also represented by this too.
	//
	public class JavaTypeName
	{
		const string extendsLabel = " extends ";
		const string superLabel = " super ";

		static readonly string [] genericConstraintsLabels = { extendsLabel, superLabel };

		JavaTypeName ()
		{
		}

		public static JavaTypeName Parse (string dottedFullName)
		{
			var ret = new JavaTypeName ();

			foreach (var label in genericConstraintsLabels) {
				int gcidx = dottedFullName.IndexOf (label, StringComparison.Ordinal);
				int gcgidx = gcidx < 0 ? -1 : dottedFullName.IndexOf ('<', 0, gcidx);
				int gccidx = gcidx < 0 ? -1 : dottedFullName.IndexOf (',', 0, gcidx);
				if (gcidx > 0 && gcgidx < 0 && gccidx < 0) {
					string args = dottedFullName.Substring (gcidx + label.Length).Trim ();
					ret.BoundsType = label;
					ret.GenericConstraints = ParseCommaSeparatedTypeNames (args).Select (s => Parse (s)).ToArray ();
					dottedFullName = dottedFullName.Substring (0, gcidx).Trim ();
				}
			}

			if (dottedFullName.EndsWith ("...", StringComparison.Ordinal)) {
				ret.ArrayPart = "...";
				dottedFullName = dottedFullName.Substring (0, dottedFullName.Length - 3);
			}
			while (dottedFullName.LastOrDefault () == ']') {
				int aidx = dottedFullName.LastIndexOf ('[');
				ret.ArrayPart += dottedFullName.Substring (aidx);
				dottedFullName = dottedFullName.Substring (0, aidx);
			}

			int idx = dottedFullName.IndexOf ('<');
			int nextIndex = dottedFullName.Length;
			if (idx > 0) {
				int last = GetMatchingGenericCloser (dottedFullName, idx + 1);
				ret.GenericArguments = ParseCommaSeparatedTypeNames (dottedFullName.Substring (idx + 1, last - idx - 1))
					.Select (s => JavaTypeName.Parse (s.Trim ()))
					.ToArray ();
				nextIndex = last + 1;
			}
			// at this state, there is no way to distinguish package name from this name specification.
			ret.DottedName = idx < 0 ? dottedFullName : dottedFullName.Substring (0, idx);

			if (nextIndex < dottedFullName.Length) {
				if (dottedFullName [nextIndex] != '.')
					throw new ArgumentException (nameof (dottedFullName));
				// the generic parent is parsed, but the rest is still there.
				var parent = ret;
				ret = Parse (dottedFullName.Substring (nextIndex + 1));
				ret.GenericParent = parent;
			}

			return ret;
		}

		static int GetMatchingGenericCloser (string str, int start)
		{
			int count = 0;
			for (int i = start; i < str.Length; i++) {
				switch (str [i]) {
				case '<':
					count++;
					break;
				case '>':
					if (count-- == 0)
						return i;
					break;
				}
			}
			return -1;
		}

		static IEnumerable<string> ParseCommaSeparatedTypeNames (string args)
		{
			int comma = args.IndexOf (',');
			if (comma < 0)
				yield return args;
			else {
				int open = args.IndexOf ('<', 0, comma);
				if (open > 0) {
					int openCount = 1;
					int i = open + 1;
					while (i < args.Length) {
						if (args [i] == '<')
							openCount++;
						else if (args [i] == '>')
							openCount--;
						i++;
						if (openCount == 0)
							break;
					}
					yield return args.Substring (0, i);
					if (i < args.Length) {
						comma = args.IndexOf (',', i);
						if (comma > 0)
							foreach (var s in ParseCommaSeparatedTypeNames (args.Substring (comma + 1)))
								yield return s;
					}
				} else {
					yield return args.Substring (0, comma);
					foreach (var s in ParseCommaSeparatedTypeNames (args.Substring (comma + 1).Trim ()))
						yield return s;
				}
			}
		}

		public  JavaTypeName?           GenericParent       { get; set; }
		public  string?                 DottedName          { get; set; }
		public  string?                 BoundsType          { get; set; } // " extends " / " super "
		public  IList<JavaTypeName>?    GenericConstraints  { get; private set; }
		public  IList<JavaTypeName>?    GenericArguments    { get; private set; }
		public  string?                 ArrayPart           { get; set; }

		public string? FullNameNonGeneric {
			get {
				if (GenericParent != null)
					return GenericParent.FullNameNonGeneric + "." + DottedName;
				else
					return DottedName;
			}
		}

	}
}
