using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Xamarin.Java.Interop
{
	static class StringCoda {
		public static string FixupType (this string t)
		{
			return t.Replace ("*", "Ref").Replace ("[]", "Array").Replace (" ", "");
		}
	}

	partial class Generator
	{
		static string jnienv_g_c;
		static string jnienv_g_cs;

		public static int Main (string [] args)
		{
			jnienv_g_c  = "JniEnvironment.g.c";
			jnienv_g_cs = "JniEnvironment.g.cs";
			if (args.Length > 0)
				jnienv_g_cs = args [0];
			if (args.Length > 1)
				jnienv_g_c  = args [1];
			try {
				using (TextWriter w = new StringWriter ()) {
					w.NewLine = "\n";
					GenerateFile (w);
					string content = w.ToString ();
					if (!File.Exists (jnienv_g_cs) || !string.Equals (content, File.ReadAllText (jnienv_g_cs)))
						File.WriteAllText (jnienv_g_cs, content);
				}
				using (TextWriter w = new StringWriter ()) {
					w.NewLine = "\n";
					GenerateNativeLibSource (w);
					string content = w.ToString ();
					if (!File.Exists (jnienv_g_c) || !string.Equals (content, File.ReadAllText (jnienv_g_c)))
						File.WriteAllText (jnienv_g_c, content);
				}
				return 0;
			} catch (Exception ex) {
				Console.WriteLine (ex);
				return 1;
			}
		}

		static string Escape (string value)
		{
			switch (value) {
			case "string":
			case "ref":
				return "@" + value;
			default: return value;
			}
		}

		static void GenerateFile (TextWriter o)
		{
			o.WriteLine ("// Generated file; DO NOT EDIT!");
			o.WriteLine ("//");
			o.WriteLine ("// To make changes, edit monodroid/tools/jnienv-gen-interop and rerun");
			o.WriteLine ();
			o.WriteLine ("using System;");
			o.WriteLine ("using System.Linq;");
			o.WriteLine ("using System.Runtime.InteropServices;");
			o.WriteLine ("using System.Threading;");
			o.WriteLine ();
			o.WriteLine ("namespace Java.Interop {");
			o.WriteLine ();
			GenerateTypes (o);
			o.WriteLine ("}");
		}

		static string Initialize (JniFunction e, string prefix)
		{
			return string.Format ("{2}{0} = ({1}) Marshal.GetDelegateForFunctionPointer (env.{0}, typeof ({1}));", e.Name, e.Delegate, prefix);
		}

		static void GenerateTypes (TextWriter o)
		{
			var visibilities = new Dictionary<string, string> {
				{ "Errors",     "public" },
				{ "Handles",    "public" },
				{ "IO",         "public" },
				{ "Strings",    "public" },
				{ "Types",      "public" },
			};
			o.WriteLine ("\tpartial class JniEnvironment {");
			o.WriteLine ();
			o.WriteLine ("\tconst string JavaInteropLib = \"JavaInterop\";");
			foreach (var t in JNIEnvEntries
					.Select (e => e.DeclaringType ?? "JniEnvironment")
					.Distinct ()
					.OrderBy (t => t)) {
				string visibility;
				if (!visibilities.TryGetValue (t, out visibility))
					visibility = "internal";
				GenerateJniEnv (o, t, visibility);
			}
			o.WriteLine ("\t}");
		}

		static void GenerateJniEnv (TextWriter o, string type, string visibility)
		{
			o.WriteLine ();
			o.WriteLine ("\t{0} static partial class {1} {{", visibility, type);
			foreach (JniFunction entry in JNIEnvEntries) {
				if ((entry.DeclaringType ?? "JniEnvironment") != type)
					continue;
				if (entry.Parameters == null)
					continue;
				if (entry.IsPrivate || entry.CustomWrapper)
					continue;

				o.WriteLine ();
				o.WriteLine ("\t\t[DllImport (JavaInteropLib, CallingConvention=CallingConvention.Cdecl)]");
				o.WriteLine ("\t\tinternal static extern {0} JavaInterop_{1} (JniEnvironmentSafeHandle env{2}{3});",
						entry.GetReturnType (entry.Name),
						entry.Name,
						entry.Parameters.Length != 0 ? ", " : "",
						string.Join (", ", entry.Parameters.Select (p => string.Format ("{0} {1}", p.Type.ManagedType, Escape (p.Name)))));

				o.WriteLine ();
				o.Write ("\t\t{2} static {0} {1} (", entry.GetReturnType(entry.Name), entry.ApiName, entry.Visibility);
				switch (entry.ApiName) {
				case "NewArray":
					var copyArray = GetArrayCopy (entry);
					o.WriteLine ("{0} array)", copyArray.Parameters [3].Type.ManagedType);
					o.WriteLine ("\t\t{");
					o.WriteLine ("\t\t\tif (array == null)");
					o.WriteLine ("\t\t\t\treturn null;");
					o.WriteLine ("\t\t\tvar result = JavaInterop_{0} (JniEnvironment.Current.SafeHandle, array.Length);", entry.Name);
					RaiseException (o, entry);
					LogHandleCreation (o, entry, "result", "\t\t\t");
					o.WriteLine ("\t\t\tCopyArray (array, result);");
					o.WriteLine ("\t\t\treturn result;");
					break;
				case "CopyArray":
					if (entry.Name.StartsWith ("G")) {
						o.WriteLine ("JniReferenceSafeHandle src, {0} dest)", entry.Parameters [3].Type.ManagedType);
						o.WriteLine ("\t\t{");
						o.WriteLine ("\t\t\tif (src == null)");
						o.WriteLine ("\t\t\t\treturn;");
						o.WriteLine ("\t\t\tJavaInterop_{0} (JniEnvironment.Current.SafeHandle, src, 0, dest.Length, dest);", entry.Name);
					} else {
						o.WriteLine ("{0} src, JniReferenceSafeHandle dest)", entry.Parameters [3].Type.ManagedType);
						o.WriteLine ("\t\t{");
						o.WriteLine ("\t\t\tif (src == null)");
						o.WriteLine ("\t\t\t\tthrow new ArgumentNullException (\"src\");");
						o.WriteLine ("\t\t\tJavaInterop_{0} (JniEnvironment.Current.SafeHandle, dest, 0, src.Length, src);", entry.Name);
					}
					RaiseException (o, entry);
					break;
				default:
					bool is_void = entry.ReturnType.ManagedType == "void";
					for (int i = 0; i < entry.Parameters.Length; i++) {
						if (i > 0)
							o.Write (", ");
						if (entry.Parameters [i].IsParamArray)
							o.Write ("params ");
						o.Write ("{0} {1}", entry.Parameters [i].Type.ManagedType, Escape (entry.Parameters [i].Name));
					}
					o.WriteLine (")");
					o.WriteLine ("\t\t{");
					NullCheckParameters (o, entry.Parameters);
					o.Write ("\t\t\t");
					if (!is_void) 
						o.Write ("var tmp = ", entry.ReturnType.GetManagedType (isReturn:true));
					o.Write ("JavaInterop_{0} (JniEnvironment.Current.SafeHandle", entry.Name);
					for (int i = 0; i < entry.Parameters.Length; i++) {
						o.Write (", ");
						if (entry.Parameters [i].Type.ManagedType.StartsWith ("out "))
							o.Write ("out ");
						o.Write (Escape (entry.Parameters [i].Name));
					}
					o.WriteLine (");");
					RaiseException (o, entry);
					if (is_void) {
					} else {
						LogHandleCreation (o, entry, "tmp", "\t\t\t");
						o.WriteLine ("\t\t\treturn tmp;");
					}
					break;
				}

				o.WriteLine ("\t\t}");
			}
			o.WriteLine ("\t}");
		}
		
		static JniFunction GetArrayCopy (JniFunction entry)
		{
			JniFunction r = null;
			int c = 0;
			var es = JNIEnvEntries.Where (e => e.Name.StartsWith ("Get") && e.Name.EndsWith ("ArrayRegion") &&
				e.Parameters [0].Type.Type == entry.ReturnType.Type);

			foreach (var e in es)
			{
				r = e;
				c++;
			}
			if (c > 1) {
				string s = string.Format ("# Couldn't find matching array copy method! Candidates: {0}",
					string.Join (", ", es.Select(e => e.Name)));
				throw new InvalidOperationException (s);
			}
			if (c == 0)
				throw new InvalidOperationException ("Couldn't find matching array copy method for " + entry.Name + ". No candidates found.");
			return r;
		}

		static void LogHandleCreation (TextWriter o, JniFunction entry, string variable, string indent)
		{
			string rt = entry.GetReturnType (entry.Name);
			switch (rt) {
			case "JniLocalReference":
				if (entry.Name == "NewLocalRef" || entry.Name == "ExceptionOccurred")
					break;
				o.Write (indent);
				o.WriteLine ("JniEnvironment.Current.LogCreateLocalRef ({0});",
						variable);
				break;
			}
		}

		static void NullCheckParameters (TextWriter o, ParamInfo[] ps)
		{
			bool haveChecks = false;
			for (int i = 0; i < ps.Length; i++) {
				var p = ps [i];
				if (p.CanBeNull)
					continue;
				if (p.Type.ManagedType == "IntPtr") {
					haveChecks = true;
					o.WriteLine ("\t\t\tif ({0} == IntPtr.Zero)", Escape (p.Name), p.Type.ManagedType);
					o.WriteLine ("\t\t\t\tthrow new ArgumentException (\"'{0}' must not be IntPtr.Zero.\", \"{0}\");", Escape (p.Name));
					continue;
				}
				var t = p.Type.GetManagedType (isReturn:false);
				if (t == "string") {
					haveChecks = true;
					o.WriteLine ("\t\t\tif ({0} == null)", Escape (p.Name), p.Type.ManagedType);
					o.WriteLine ("\t\t\t\tthrow new ArgumentNullException (\"{0}\");", Escape (p.Name));
					continue;
				}
				if (t == "JniReferenceSafeHandle" ||
						(t.StartsWith ("Jni") && t.EndsWith ("ID"))) {
					haveChecks = true;
					o.WriteLine ("\t\t\tif ({0} == null)", Escape (p.Name), p.Type.ManagedType);
					o.WriteLine ("\t\t\t\tthrow new ArgumentNullException (\"{0}\");", Escape (p.Name));
					o.WriteLine ("\t\t\tif ({0}.IsInvalid)", Escape (p.Name), p.Type.ManagedType);
					o.WriteLine ("\t\t\t\tthrow new ArgumentException (\"{0}\");", Escape (p.Name));
					continue;
				}
			}
			if (haveChecks)
				o.WriteLine ();
		}

		static void RaiseException (TextWriter o, JniFunction entry)
		{
			if (!entry.Throws)
				return;

			o.WriteLine ();
			o.WriteLine ("\t\t\tException __e = JniEnvironment.Current.GetExceptionForLastThrowable ();");
			o.WriteLine ("\t\t\tif (__e != null)");
			o.WriteLine ("\t\t\t\tthrow __e;");
			o.WriteLine ();
		}

		static void GenerateNativeLibSource (TextWriter o)
		{
			o.WriteLine ("/*");
			o.WriteLine (" * Generated file; DO NOT EDIT!");
			o.WriteLine (" *");
			o.WriteLine (" * To make changes, edit Java.Interop/tools/jnienv-gen and rerun");
			o.WriteLine (" */");
			o.WriteLine ();
			o.WriteLine ("#include <jni.h>");
			o.WriteLine ();
			o.WriteLine ("typedef jmethodID jstaticmethodID;");
			o.WriteLine ("typedef jfieldID  jstaticfieldID;");
			foreach (JniFunction entry in JNIEnvEntries) {
				if (entry.IsPrivate || entry.CustomWrapper)
					continue;
				o.WriteLine ();
				o.WriteLine (entry.ReturnType.Type);
				o.WriteLine ("JavaInterop_{0} (JNIEnv *env{1}{2})",
						entry.Name,
						entry.Parameters.Length != 0 ? ", " : "",
						string.Join (", ", entry.Parameters.Select (p => string.Format ("{0} {1}", p.Type.Type, p.Name))));
				o.WriteLine ("{");
				o.Write ("\t");
				if (entry.ReturnType.Type != "void")
					o.Write ("return ");
				o.WriteLine ("(*env)->{0} (env{1}{2});",
						entry.Name,
						entry.Parameters.Length != 0 ? ", " : "",
						string.Join (", ", entry.Parameters.Select (p => p.Name)));
				o.WriteLine ("}");
			}
		}
	}

	class JniFunction {

		public string DeclaringType;

		// The java name
		public string Name;

		// The name of the property/method we will generate. Defaults to the (java) name.
		private string api_name;
		public string ApiName
		{
			get { return api_name ?? Name; }
			set { api_name = value; }
		}
		
		// The C prototype that we are binding (for diagnostic purposes)
		public string Prototype;
		
		public TypeInfo ReturnType;
		public ParamInfo [] Parameters;

		// If true, then we initialize the binding on the static ctor, we dont lazy-define it
		public bool Prebind = false;

		// If there is a custom wrapper in JNIEnv (so an automatic one shouldn't be generated)
		public bool CustomWrapper = false;

		// If the JNI function can throw an exception (ExceptionOccurred needs to be invoked)
		public bool Throws;

		// The signature of the C# delegate that we will use for the generated property
		private string @delegate;
		public string Delegate
		{
			get
			{
				StringBuilder d;

				if (@delegate != null)
					return @delegate;

				if (Parameters == null)
					return null;

				if (IsPrivate)
					return null;

				d = new StringBuilder ();
				bool is_void = ReturnType.Type == "void";

				if (is_void) {
					d.Append ("Action<IntPtr");
				} else {
					d.Append ("Func<IntPtr");
				}
				for (int i = 0; i < Parameters.Length; i++) {
					d.Append (",");
					//d.Append (Parameters [i].Type.GetManagedType (Parameters [i].Type.Type));
					d.Append (Parameters [i].Type.ManagedType);
				}
				if (!is_void) {
					d.Append (",");
					if (ReturnType.ManagedType == "void") {
						d.Append (ReturnType.GetManagedType (isReturn:true));
					} else {
						d.Append (ReturnType.ManagedType);
					}
				}
				d.Append (">");

				return d.ToString ();
			}
			set
			{
				@delegate = value;
			}
		}

		private string visibility;
		public string Visibility {
			get {
				if (visibility == null)
					return "public";
				return visibility;
			}
			set {
				visibility = value;
			}
		}

		public bool IsPublic { get { return Visibility == "public"; } }
		public bool IsPrivate { get { return visibility == "private"; } }
		
		public string GetReturnType (string methodName)
		{
			if (ReturnType == null || ReturnType.ManagedType == "void")
				return "void";
			if (methodName == "NewGlobalRef")
				return "JniGlobalReference";
			if (methodName == "NewWeakGlobalRef")
				return "JniWeakGlobalReference";
			return ReturnType.GetManagedType (isReturn:true).FixupType ();
		}
		
		public string GetDelegateTypeName (string methodName)
		{
			StringBuilder name = new StringBuilder ();

			if (ReturnType == null || ReturnType.ManagedType == "void")
				name.Append ("JniAction_");
			else
				name.Append ("JniFunc_");
			name.Append ("JniEnvironmentSafeHandle");
			for (int i = 0; i < Parameters.Length; i++) {				
				if (Parameters [i].Type.ManagedType == "va_list")
					return null;

				name.AppendFormat ("_").Append (Parameters [i].Type.GetManagedType (isReturn:false).FixupType ());
			}
			
			string rt = GetReturnType (methodName);
			if (rt != "void")
				name.Append ("_").Append (rt);
			
			return name.ToString ();
		}
	}

	class TypeInfo
	{
		public string Type;

		private string managed_type;
		public string ManagedType
		{
			get { return managed_type ?? GetManagedType (Type, false); }
			set { managed_type = value; }
		}

		public string GetManagedType (string native_type = null, bool isReturn = false)
		{
			native_type = native_type ?? managed_type ?? Type;
			switch (native_type) {
			case "jvalue*":                 return "JValue[]";
			case "jbyte":                   return "sbyte";
			case "jchar":                   return "char";
			case "jchar*":                  return "IntPtr";
			case "jshort":                  return "short";
			case "jsize":
			case "jint":                    return "int";
			case "jlong":                   return "long";
			case "jfloat":                  return "float";
			case "jdouble":                 return "double";
			case "jboolean":                return "bool";
			case "":                        return "void";
			case "void*":                   return "IntPtr";
			case "jfieldID":                return "JniInstanceFieldID";
			case "jstaticfieldID":          return "JniStaticFieldID";
			case "jmethodID":               return "JniInstanceMethodID";
			case "jstaticmethodID":         return "JniStaticMethodID";
			case "jstring":
			case "jarray":
			case "jobject":
			case "jthrowable":
			case "jclass":                  return isReturn ? "JniLocalReference" : "JniReferenceSafeHandle";
			case "jweak":                   return "JniWeakGlobalReference";
			case "const jchar*":
			case "const char*":             return "string";
			case "JavaVM**":                return "out JavaVMSafeHandle";
			case "const JNINativeMethod*":	return "JniNativeMethodRegistration []";
			case "jobjectRefType":          return "JniReferenceType";
			default:
				if (native_type.EndsWith ("Array"))
					return isReturn ? "JniLocalReference" : "JniReferenceSafeHandle";
				if (native_type.EndsWith ("*"))
					return "IntPtr";
				return native_type;
			}
		}

		public TypeInfo ()
		{
		}

		public TypeInfo (string Type, string ManagedType = null)
		{
			this.Type = Type;
			this.ManagedType = ManagedType;
		}

		public static implicit operator TypeInfo (string type)
		{
			return new TypeInfo (type);
		}
	}

	class ParamInfo
	{
		public TypeInfo Type;
		public string Name;
		public bool IsParamArray;
		public bool CanBeNull;

		public ParamInfo (TypeInfo Type, string Name, bool IsParamArray)
		{
			this.Type = Type;
			this.Name = Name;
			this.IsParamArray = IsParamArray;
		}

		public ParamInfo (TypeInfo Type, string Name = null, Modifier m = 0)
		{
			this.Type = Type;
			this.Name = Name;
			IsParamArray  = (m & Modifier.Params) != 0;
			CanBeNull     = (m & Modifier.CanBeNull) != 0;
		}
	}

	[Flags]
	enum Modifier {
		None      = 0,
		Params    = 1,
		CanBeNull = 2,
	}
}

