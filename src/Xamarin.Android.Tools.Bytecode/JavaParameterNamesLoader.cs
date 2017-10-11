using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Xamarin.Android.Tools.Bytecode
{
	public class JavaParameterNamesLoader
	{
		void FillSyntheticMethodsFixup (List<Package> fixup, XElement type, HashSet<XElement> alreadyChecked, Dictionary<string,XElement> fullNameMap)
		{
			if (alreadyChecked.Contains (type))
				return;
			alreadyChecked.Add (type);

			var fpkg = fixup.FirstOrDefault (p => p.Name == type.Parent.Attribute ("name").Value);
			if (fpkg == null)
				return;
			var ftype = fpkg.Types.FirstOrDefault (t => t.Name == type.Attribute ("name").Value);
			if (ftype == null)
				return;

			var extends = type.Attribute ("extends")?.Value;
			var bt = extends != null ? fullNameMap [extends] : null;
			if (bt == null || bt.Attribute ("visibility").Value != "")
				return;
			var fbpkg = fixup.FirstOrDefault (p => p.Name == bt.Parent.Attribute ("name").Value);
			if (fbpkg == null)
				return;
			var fbtype = fbpkg.Types.FirstOrDefault (t => t.Name == bt.Attribute ("name").Value);
			if (fbtype == null)
				return;
			
			// FIXME: it is hacky; it should remove the conflicting synthetic methods.
			ftype.Methods = fbtype.Methods.Concat (ftype.Methods).ToList ();
		}

		public void ApplyParameterNameChanges (XElement api, string path)
		{
			var fixup = LoadParameterFixupDescription (path);

			// We have to supply "dummy" fixups for "synthetic" methods that might come
			// from non-public ancestor classes.
			// Unfortunately ancestor types are unknown in the fixup description, so
			// they have to be extracted from XML API metadata. So, do it here.
			var hashset = new HashSet<XElement> ();
			var fullNameMap = api.Elements ("package").SelectMany (p => p.Elements ()).ToDictionary (e => e.Parent.Attribute ("name").Value + "." + e.Attribute ("name").Value);
			foreach (var t in api.Elements ("package").SelectMany (p => p.Elements ()))
			         FillSyntheticMethodsFixup (fixup, t, hashset, fullNameMap);

			var methods = api.XPathSelectElements ("package/*/*");
			foreach (var method in methods) {
				switch (method.Name.LocalName) {
				case "method":
				case "constructor":

					string package = method.Parent.Parent.Attribute ("name").Value;
					string type = method.Parent.Attribute ("name").Value;
					string mname = method.Attribute ("name").Value;
					var parameters = method.Elements ("parameter").ToArray ();
					if (!parameters.Any ()) // we don't care about parameterless methods.
						continue;
					var matchedPackage = fixup.FirstOrDefault (p => p.Name == package);
					if (matchedPackage == null) {
						Log.Warning (0, "Package {0} not found.", package);
						continue;
					}
					var matchedType = matchedPackage.Types.FirstOrDefault (t => t.Name == type);
					if (matchedType == null) {
						Log.Warning (0, "Type {0} not found.", package + '.' + type);
						continue;
					}
					var matchedMethods = matchedType.Methods.Where (m => (m.Name == "#ctor" ? method.Name.LocalName == "constructor" : m.Name == mname) && m.Parameters.Count == parameters.Length);
					if (!matchedMethods.Any ()) {
						Log.Warning (0, "Method {0} with {1} parameters not found.", package + '.' + type + '.' + mname, parameters.Length);
						continue;
					}
					var matched = matchedMethods.FirstOrDefault (m => m.Parameters.Zip (parameters, (f, x) => f.Type == x.Attribute ("type").Value.Replace (", ", ",")).All (b => b));

					if (matched == null) {
						Log.Warning (0, "Method {0}({1}) not found.",
						             package + '.' + type + '.' + mname,
						             string.Join (",", parameters.Select (para => para.Attribute ("type").Value)));
						continue;
					}

					for (int i = 0; i < parameters.Length; i++)
						parameters [i].SetAttributeValue ("name", matched.Parameters [i].Name);

					matched.Applied = true;
					break;
				}
			}
			foreach (var p in fixup)
				foreach (var t in p.Types)
					foreach (var m in t.Methods.Where (m => !m.Applied))
						Log.Warning (0, "Method parameter description for {0}.{1}.{2}({3}) is never applied",
						             p.Name, t.Name, m.Name, string.Join (",", m.Parameters.Select (para => para.Type)));
		}

		class Parameter
		{
			public string Type { get; set; }
			public string Name { get; set; }
		}

		class Method
		{
			public string Name { get; set; }
			public List<Parameter> Parameters { get; set; }
			public bool Applied { get; set; }
		}

		class Type
		{
			public string Name { get; set; }
			public List<Method> Methods { get; set; }
		}

		class Package
		{
			public string Name { get; set; }
			public List<Type> Types { get; set; }
		}

		// from https://github.com/atsushieno/xamarin-android-docimporter-ng/blob/master/Xamarin.Android.Tools.JavaStubImporter/JavaApiParameterNamesXmlExporter.cs#L78
		/*
		 * The Text Format is:
		 * 
		 * package {packagename}
		 * #---------------------------------------
		 *   interface {interfacename}{optional_type_parameters} -or-
		 *   class {classname}{optional_type_parameters}
		 *     {optional_type_parameters}{methodname}({parameters})
		 * 
		 * Anything after # is treated as comment.
		 * 
		 * optional_type_parameters: "" -or- "<A,B,C>" (no constraints allowed)
		 * parameters: type1 p0, type2 p1 (pairs of {type} {name}, joined by ", ")
		 * 
		 * It is with strict indentations. two spaces for types, four spaces for methods.
		 * 
		 * Constructors are named as "#ctor".
		 * 
		 * Commas are used by both parameter types and parameter separators,
		 * but only parameter separators can be followed by a whitespace.
		 * It is useful when writing text parsers for this format.
		 * 
		 * Type names may contain whitespaces in case it is with generic constraints (e.g. "? extends FooBar"),
		 * so when parsing a parameter type-name pair, the only trustworthy whitespace for tokenizing name is the *last* one.
		 * 
		 */
		List<Package> LoadParameterFixupDescription (string path)
		{
			var fixup = new List<Package> ();
			string package = null;
			var types = new List<Type> ();
			string type = null;
			var methods = new List<Method> ();
			foreach (var l in File.ReadAllLines (path)) {
				var line = l.IndexOf ('#') >= 0 ? l.Substring (0, l.IndexOf ('#')) : l;
				if (line.Trim ().Length == 0)
					continue;
				if (line.StartsWith ("package ", StringComparison.Ordinal)) {
					package = line.Substring ("package ".Length);
					types = new List<Type> ();
					fixup.Add (new Package { Name = package, Types = types });
					continue;
				} else if (line.StartsWith ("    ", StringComparison.Ordinal)) {
					int open = line.IndexOf ('(');
					string parameters = line.Substring (open + 1).TrimEnd (')');
					string name = line.Substring (4, open - 4);
					if (name.FirstOrDefault () == '<') // generic method can begin with type parameters.
						name = name.Substring (name.IndexOf (' ') + 1);
					methods.Add (new Method {
						Name = name,
						Parameters = parameters.Replace (", ", "\0").Split ('\0')
								       .Select (s => s.Split (' '))
						                       .Select (a => new Parameter { Type = string.Join (" ", a.Take (a.Length - 1)), Name = a.Last () }).ToList ()
					});
				} else {
					type = line.Substring (line.IndexOf (' ', 2) + 1);
					// To match type name from class-parse, we need to strip off generic arguments here (generics are erased).
					if (type.IndexOf ('<') > 0)
						type = type.Substring (0, type.IndexOf ('<'));
					methods = new List<Method> ();
					types.Add (new Type { Name = type, Methods = methods });
				}
			}
			return fixup;
		}
	}
}
