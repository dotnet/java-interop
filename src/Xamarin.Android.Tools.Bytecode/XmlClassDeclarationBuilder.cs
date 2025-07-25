using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace Xamarin.Android.Tools.Bytecode {

	public class XmlClassDeclarationBuilder {

		ClassFile       classFile;
		ClassSignature? signature;

		bool IsInterface {
			get {return (classFile.AccessFlags & ClassAccessFlags.Interface) != 0;}
		}

		public XmlClassDeclarationBuilder (ClassFile classFile)
		{
			if (classFile == null)
				throw new ArgumentNullException ("classFile");

			this.classFile  = classFile;
			signature       = classFile.GetSignature ();
		}

		public XElement ToXElement ()
		{
			return new XElement (GetElementName (),
					new XAttribute ("abstract",                 (classFile.AccessFlags & ClassAccessFlags.Abstract) != 0),
					new XAttribute ("deprecated",               GetDeprecatedValue (classFile.Attributes)),
					GetEnclosingMethod (),
					GetExtends (),
					GetExtendsGenericAware (),
					new XAttribute ("final",                    (classFile.AccessFlags & ClassAccessFlags.Final) != 0),
					new XAttribute ("name",                     GetThisClassName ()),
					new XAttribute ("jni-signature",            classFile.FullJniName),
					GetSourceFile (),
					new XAttribute ("static",                   classFile.IsStatic),
					new XAttribute ("visibility",               GetVisibility (classFile.Visibility)),
					GetAnnotatedVisibility (classFile.Visibility, classFile.Attributes),
					GetTypeParmeters (signature == null ? null : signature.TypeParameters),
					GetImplementedInterfaces (),
					GetConstructors (),
					GetMethods (),
					GetFields ()
			);
		}

		string GetElementName ()
		{
			if (IsInterface)
				return "interface";
			return "class";
		}

		static string GetDeprecatedValue (AttributeCollection attributes)
		{
			if (attributes.Get<DeprecatedAttribute> () == null)
				return "not deprecated";
			return "deprecated";
		}

		IEnumerable<XAttribute> GetEnclosingMethod ()
		{
			string? declaringClass, declaringMethod, declaringDescriptor;
			if (!classFile.TryGetEnclosingMethodInfo (out declaringClass, out declaringMethod, out declaringDescriptor)) {
				yield break;
			}
			if (declaringClass != null)
				yield return new XAttribute ("enclosing-method-jni-type",    "L" + declaringClass + ";");
			if (declaringMethod != null)
				yield return new XAttribute ("enclosing-method-name",        declaringMethod);
			if (declaringDescriptor != null)
				yield return new XAttribute ("enclosing-method-signature",   declaringDescriptor);
		}

		XAttribute[]? GetExtends ()
		{
			if (IsInterface)
				return null;
			if (classFile.SuperClass == null)
				return null;
			return new []{
				new XAttribute ("jni-extends",  "L" + classFile.SuperClass.Name.Value + ";"),
				new XAttribute ("extends",      BinaryNameToJavaClassName (classFile.SuperClass.Name.Value)),
			};
		}

		XAttribute? GetExtendsGenericAware ()
		{
			if (IsInterface)
				return null;
			if (classFile.SuperClass == null)
				return null;
			var superSig    = classFile.SuperClass.Name.Value;
			return new XAttribute ("extends-generic-aware",
				signature != null
					? SignatureToGenericJavaTypeName (signature.SuperclassSignature)
					: BinaryNameToJavaClassName (superSig));
		}

		static string GetVisibility (ClassAccessFlags accessFlags)
		{
			if ((accessFlags & ClassAccessFlags.Public) != 0)
				return "public";
			if ((accessFlags & ClassAccessFlags.Protected) != 0)
				return "protected";
			if ((accessFlags & ClassAccessFlags.Private) != 0)
				return "private";
			if (accessFlags.HasFlag (ClassAccessFlags.Internal))
				return "public";    // TODO: `kotlin-internal` at some point?  See also GetAnnotatedVisibility()
			return "";
		}

		string GetThisClassName ()
		{
			return GetName (classFile.ThisClass.Name.Value);
		}

		static string GetName (string value)
		{
			int s = value.LastIndexOf ('/');
			if (s >= 0)
				value = value.Substring (s + 1);
			return value.Replace ('$', '.');
		}

		static string BinaryNameToJavaClassName (string? value)
		{
			if (value == null || string.IsNullOrEmpty (value))
				return string.Empty;
			return value.Replace ('/', '.').Replace ('$', '.');
		}

		string SignatureToJavaTypeName (string? value)
		{
			if (value == null || string.IsNullOrEmpty (value))
				return string.Empty;
			int index   = 0;
			var array   = GetArraySuffix (value, ref index);
			var builtin = GetBuiltinName (value, ref index);
			if (builtin != null)
				return builtin + array;
			if (value [index] == 'L') {
				index++;
				var type    = new StringBuilder ();
				int depth   = 0;
				int e       = index;
				while (e < value.Length) {
					var c = value [e++];
					if (depth == 0 && c == ';')
						break;

					if (c == '<') {
						depth++;
					} else if (c == '>') {
						depth--;
					} else if (depth > 0) {
						;
					} else if (c == '/' || c == '$') {
						type.Append ('.');
					} else {
						type.Append (c);
					}
				}
				return type.Append (array).ToString ();
			}
			if (value [index] == 'T') {
				index++;
				var tp  = Signature.ExtractIdentifier (value, ref index);
				if (signature != null)
					return SignatureToJavaTypeName (signature.TypeParameters [tp].ClassBound) + array;
			}
			return value;
		}

		static string SignatureToGenericJavaTypeName (string? value)
		{
			if (value == null || string.IsNullOrEmpty (value))
				return string.Empty;
			int index   = 0;
			var type    = new StringBuilder ();
			return AppendGenericTypeNameFromSignature (type, value, ref index)
				.ToString ();
		}

		static StringBuilder AppendGenericTypeNameFromSignature (StringBuilder typeBuilder, string value, ref int index)
		{
			var array   = GetArraySuffix (value, ref index);
			var builtin = GetBuiltinName (value, ref index);
			if (builtin != null) {
				return typeBuilder.Append (builtin).Append (array);
			}
			switch (value [index]) {
			case 'L':
				index++;
				int depth   = 0;
				while (index < value.Length) {
					var c   = value [index++];
					if (depth == 0 && c == ';')
						break;

					if (c == '<') {
						depth++;
						typeBuilder.Append ("<");
						AppendGenericTypeNameFromSignature (typeBuilder, value, ref index);
					} else if (c == '>') {
						typeBuilder.Append (">");
						depth--;
					} else if (depth > 0) {
						index--;
						typeBuilder.Append (", ");
						AppendGenericTypeNameFromSignature (typeBuilder, value, ref index);
					} else if (c == '/' || c == '$') {
						typeBuilder.Append ('.');
					} else {
						typeBuilder.Append (c);
					}
				}
				return typeBuilder.Append (array);
			case 'T':
				index++;
				typeBuilder.Append (Signature.ExtractIdentifier (value, ref index));
				index++;    // consume ';'
				return typeBuilder.Append(array);
			case '*':
				index++;
				return typeBuilder.Append ("?");
			case '+':
				index++;
				typeBuilder.Append ("? extends ");
				return AppendGenericTypeNameFromSignature (typeBuilder, value, ref index);
			case '-':
				index++;
				typeBuilder.Append ("? super ");
				return AppendGenericTypeNameFromSignature (typeBuilder, value, ref index);
			}
			typeBuilder.Append ("/* should not be reached */").Append (value.Substring (index));
			index = value.Length;
			return typeBuilder;
		}

		static string? GetArraySuffix (string value, ref int index)
		{
			int o   = index;
			while (value [index] == '[') {
				index++;
			}
			if (o == index)
				return null;
			return string.Join ("", Enumerable.Repeat ("[]", index - o));
		}

		static string? GetBuiltinName (string value, ref int index)
		{
			switch (value [index]) {
			case 'B':   index++;    return "byte";
			case 'C':   index++;    return "char";
			case 'D':   index++;    return "double";
			case 'F':   index++;    return "float";
			case 'I':   index++;    return "int";
			case 'J':   index++;    return "long";
			case 'S':   index++;    return "short";
			case 'V':   index++;    return "void";
			case 'Z':   index++;    return "boolean";
			}
			return null;
		}

		XAttribute? GetSourceFile ()
		{
			var sourceFile  = classFile.SourceFileName;
			if (sourceFile == null)
				return null;
			return new XAttribute ("source-file-name", sourceFile);
		}

		XElement? GetTypeParmeters (TypeParameterInfoCollection? typeParameters)
		{
			if (typeParameters == null || typeParameters.Count == 0)
				return null;
			return new XElement ("typeParameters",
					typeParameters.Select (tp =>
						new XElement ("typeParameter",
							new XAttribute ("name",             tp.Identifier),
							new XAttribute ("jni-classBound",   tp.ClassBound ?? ""),
							new XAttribute ("classBound",       SignatureToGenericJavaTypeName (tp.ClassBound)),
							new XAttribute ("interfaceBounds",  string.Join (":", tp.InterfaceBounds.Select (_ => SignatureToGenericJavaTypeName (_)))),
							new XAttribute ("jni-interfaceBounds",  string.Join (":", tp.InterfaceBounds)))));
		}

		IEnumerable<XElement> GetImplementedInterfaces ()
		{
			if (signature != null) {
				if (signature.SuperinterfaceSignatures.Count != classFile.Interfaces.Count) {
					Console.Error.WriteLine ("class-parse: warning: class' Signature's superinterfaces count doesn't match Interfaces count!");
				}
				int max = Math.Min (signature.SuperinterfaceSignatures.Count, classFile.Interfaces.Count);
				for (int i = 0; i < max; ++i) {
					yield return new XElement ("implements",
						new XAttribute ("name",                 BinaryNameToJavaClassName (classFile.Interfaces [i].Name.Value)),
						new XAttribute ("name-generic-aware",   SignatureToGenericJavaTypeName (signature.SuperinterfaceSignatures [i])),
						new XAttribute ("jni-type",             signature.SuperinterfaceSignatures [i]));
				}
				yield break;
			}
			foreach (var c in classFile.Interfaces) {
				var n = BinaryNameToJavaClassName (c.Name.Value);
				yield return new XElement ("implements",
						new XAttribute ("name",                 n),
						new XAttribute ("name-generic-aware",   n),
						new XAttribute ("jni-type",             "L" + c.Name.Value + ";"));
			}
		}

		IEnumerable<XElement> GetConstructors ()
		{
			return classFile.Methods.Where (m => m.Name == "<init>" 
					&& (GetMethodVisibility(m.AccessFlags) == "public" || GetMethodVisibility(m.AccessFlags) == "protected" || GetMethodVisibility (m.AccessFlags) == "kotlin-internal"))
				.OrderBy (m => m.Name + m.Descriptor, StringComparer.OrdinalIgnoreCase)
				.Select (c => GetMethod ("constructor", GetThisClassName (), c, null));
		}

		XElement GetMethod (string element, string name, MethodInfo method, string? returns = null)
		{
			var abstr   = element == "method"
				? new XAttribute ("abstract", (method.AccessFlags & MethodAccessFlags.Abstract) != 0)
				: null;
			var ret     = returns != null
				? new XAttribute ("return",     SignatureToGenericJavaTypeName (returns))
				: null;
			if (!string.IsNullOrWhiteSpace (method.KotlinReturnType))
				ret?.SetValue (method.KotlinReturnType);
			var jniRet  = returns != null
				? new XAttribute ("jni-return", returns)
				: null;
			var msig    = method.GetSignature ();
			return new XElement (element,
				abstr,
				new XAttribute ("deprecated",   GetDeprecatedValue (method.Attributes)),
				new XAttribute ("final",        (method.AccessFlags & MethodAccessFlags.Final) != 0),
				new XAttribute ("name",         name),
				GetNative (method),
				ret,
				jniRet,
				new XAttribute ("static",       (method.AccessFlags & MethodAccessFlags.Static) != 0),
				GetSynchronized (method),
				new XAttribute ("visibility",   GetVisibility (method.AccessFlags)),
				GetAnnotatedVisibility (method.Attributes),
				new XAttribute ("bridge",       (method.AccessFlags & MethodAccessFlags.Bridge) != 0),
				new XAttribute ("synthetic",    (method.AccessFlags & MethodAccessFlags.Synthetic) != 0),
				new XAttribute ("jni-signature",    method.Descriptor),
				GetNotNull (method),
				GetTypeParmeters (msig == null ? null : msig.TypeParameters),
				GetMethodParameters (method),
				GetExceptions (method));
		}

		static XAttribute? GetNative (MethodInfo method)
		{
			if (method.IsConstructor)
				return null;
			return new XAttribute ("native",    (method.AccessFlags & MethodAccessFlags.Native) != 0);
		}

		static XAttribute? GetSynchronized (MethodInfo method)
		{
			if (method.IsConstructor)
				return null;
			return new XAttribute ("synchronized",  (method.AccessFlags & MethodAccessFlags.Synchronized) != 0);
		}

		static string GetVisibility (MethodAccessFlags accessFlags)
		{
			if (accessFlags.HasFlag (MethodAccessFlags.Internal))
				return "kotlin-internal";
			if ((accessFlags & MethodAccessFlags.Public) != 0)
				return "public";
			if ((accessFlags & MethodAccessFlags.Protected) != 0)
				return "protected";
			if ((accessFlags & MethodAccessFlags.Private) != 0)
				return "private";
			return "";
		}

		IEnumerable<XElement> GetMethodParameters (MethodInfo method)
		{
			var annotations = method.Attributes?.OfType<RuntimeInvisibleParameterAnnotationsAttribute> ().FirstOrDefault ()?.Annotations;
			var varargs     = (method.AccessFlags & MethodAccessFlags.Varargs) != 0;
			var parameters  = method.GetParameters ();
			for (int i = 0; i < parameters.Length; ++i) {
				var p           = parameters [i];
				var type        = p.Type.BinaryName;
				var genericType = p.Type.TypeSignature;
				var varargArray = (i == (parameters.Length - 1)) && varargs;
				if (varargArray) {
					Debug.Assert (p.Type.BinaryName.StartsWith ("[", StringComparison.Ordinal),
							"Varargs parameters MUST be arrays!");
					Debug.Assert (p.Type.TypeSignature != null && p.Type.TypeSignature.StartsWith ("[", StringComparison.Ordinal),
							"Varargs parameters MUST be arrays!");
					type        = type.Substring (1);
					genericType = genericType?.Substring (1);
				}
				genericType = SignatureToGenericJavaTypeName (genericType);
				if (!string.IsNullOrWhiteSpace (p.KotlinType))
					genericType = p.KotlinType;
				if (varargArray) {
					type        += "...";
					genericType += "...";
				}
				yield return new XElement ("parameter",
						new XAttribute ("name", p.Name),
						new XAttribute ("type",     genericType),
						new XAttribute ("jni-type", p.Type.TypeSignature ?? p.Type.BinaryName),
						GetNotNull (annotations, i));
			}
		}

		IEnumerable<XElement> GetExceptions (MethodInfo method)
		{
			foreach (var t in method.GetThrows ()) {
				yield return new XElement ("exception",
						new XAttribute ("name", t.BinaryName),
						new XAttribute ("type", BinaryNameToJavaClassName (t.BinaryName)),
						new XAttribute ("type-generic-aware",   t.TypeSignature != null
							? SignatureToGenericJavaTypeName (t.TypeSignature)
							: BinaryNameToJavaClassName (t.BinaryName)));
			}
		}

		static XAttribute? GetAnnotatedVisibility (ClassAccessFlags flags, AttributeCollection attributes)
		{
			var attr = GetAnnotatedVisibility (attributes);
			if (flags.HasFlag (ClassAccessFlags.Internal)) {
				if (attr == null) {
					attr = new XAttribute ("annotated-visibility", "module-info");
				} else {
					attr.Value += " module-info";
				}
			}
			return attr;
		}

		static XAttribute? GetAnnotatedVisibility (AttributeCollection attributes)
		{
			var annotations = attributes?.OfType<RuntimeInvisibleAnnotationsAttribute> ().FirstOrDefault ()?.Annotations;

			if (annotations?.FirstOrDefault (a => a.Type == "Landroidx/annotation/RestrictTo;") is Annotation annotation) {
				var annotation_element_values = (annotation.Values.FirstOrDefault ().Value as AnnotationElementArray)?.Values?.OfType<AnnotationElementEnum> ();

				if (annotation_element_values is null || !annotation_element_values.Any ())
					return null;

				var value_string = string.Join (" ", annotation_element_values.Select (v => v.ConstantName).Where (p => p != null));

				if (string.IsNullOrWhiteSpace (value_string))
					return null;

				return new XAttribute ("annotated-visibility", value_string);
			}

			return null;
		}

		static XAttribute? GetNotNull (MethodInfo method)
		{
			var annotations = method.Attributes?.OfType<RuntimeInvisibleAnnotationsAttribute> ().FirstOrDefault ()?.Annotations;

			if (annotations?.Any (a => IsNotNullAnnotation (a)) == true)
				return new XAttribute ("return-not-null", "true");

			return null;
		}

		static XAttribute? GetNotNull (IList<ParameterAnnotation>? annotations, int parameterIndex)
		{
			var ann = annotations?.FirstOrDefault (a => a.ParameterIndex == parameterIndex)?.Annotations;

			if (ann?.Any (a => IsNotNullAnnotation (a)) == true)
				return new XAttribute ("not-null", "true");

			return null;
		}

		static XAttribute? GetNotNull (FieldInfo field)
		{
			var annotations = field.Attributes?.OfType<RuntimeInvisibleAnnotationsAttribute> ().FirstOrDefault ()?.Annotations;

			if (annotations?.Any (a => IsNotNullAnnotation (a)) == true)
				return new XAttribute ("not-null", "true");

			return null;
		}

		static bool IsNotNullAnnotation (Annotation annotation)
		{
			// Android ones plus the list from here:
			// https://stackoverflow.com/questions/4963300/which-notnull-java-annotation-should-i-use
			// https://github.com/JetBrains/kotlin/blob/03360c0108797b2a98b6608e2bddfacd5f4e87ce/core/compiler.common.jvm/src/org/jetbrains/kotlin/load/java/JvmAnnotationNames.kt#L64-L91
			switch (annotation.Type) {
				case "Landroid/annotation/NonNull;":
				case "Landroid/support/annotation/NonNull;":
				case "Landroidx/annotation/NonNull;":
				case "Landroidx/annotation/RecentlyNonNull;":
				case "Lcom/android/annotations/NonNull;":
				case "Ledu/umd/cs/findbugs/annotations/NonNull;":
				case "Ljakarta/annotation/Nonnull;":
				case "Ljavax/annotation/Nonnull;":
				case "Ljavax/validation/constraints/NotNull;":
				case "Llombok/NonNull;":
				case "Lorg/checkerframework/checker/nullness/compatqual/NonNullDecl;":
				case "Lorg/checkerframework/checker/nullness/qual/NonNull;":
				case "Lorg/eclipse/jdt/annotation/NonNull;":
				case "Lorg/jetbrains/annotations/NotNull;":
				case "Lorg/jspecify/annotations/NonNull;":
					return true;
			}

			return false;
		}

		IEnumerable<XElement> GetFields ()
		{
			foreach (var field in classFile.Fields.OrderBy (n => n.Name, StringComparer.OrdinalIgnoreCase)) {
				var visibility = GetVisibility (field.AccessFlags);
				if (visibility == "private" || visibility == "")
					continue;
				var type = new XAttribute ("type", SignatureToJavaTypeName (field.Descriptor));
				if (!string.IsNullOrWhiteSpace (field.KotlinType))
					type.SetValue (field.KotlinType);
				yield return new XElement ("field",
						new XAttribute ("deprecated",           GetDeprecatedValue (field.Attributes)),
						new XAttribute ("final",                (field.AccessFlags & FieldAccessFlags.Final) != 0),
						new XAttribute ("name",                 field.Name),
						new XAttribute ("static",               (field.AccessFlags & FieldAccessFlags.Static) != 0),
						new XAttribute ("synthetic",            (field.AccessFlags & FieldAccessFlags.Synthetic) != 0),
						new XAttribute ("transient",            (field.AccessFlags & FieldAccessFlags.Transient) != 0),
						type,
						new XAttribute ("type-generic-aware",   GetGenericType (field)),
						new XAttribute ("jni-signature",        field.Descriptor),
						GetNotNull (field),
						GetValue (field),
						new XAttribute ("visibility",           visibility),
						GetAnnotatedVisibility (field.Attributes),
						new XAttribute ("volatile",             (field.AccessFlags & FieldAccessFlags.Volatile) != 0));
			}
		}

		string GetGenericType (FieldInfo field)
		{
			var signature = field.GetSignature ();
			if (signature == null)
				return SignatureToJavaTypeName (field.Descriptor);
			return SignatureToGenericJavaTypeName (signature);
		}

		static XAttribute? GetValue (FieldInfo field)
		{
			var constantValue = (ConstantValueAttribute?) field.Attributes.FirstOrDefault (a => a.Name == "ConstantValue");
			if (constantValue == null)
				return null;
			var value       = "";
			var constant    = constantValue.Constant;
			switch (constant.Type) {
			case ConstantPoolItemType.Double:
				var doubleItem = (ConstantPoolDoubleItem)constant;
				if (Double.IsNaN (doubleItem.Value))
					value = "(0.0 / 0.0)";
				else if (Double.IsNegativeInfinity (doubleItem.Value))
					value = "(-1.0 / 0.0)";
				else if (Double.IsPositiveInfinity (doubleItem.Value))
					value = "(1.0 / 0.0)";
				else
					value = doubleItem.Value.ToString ("G17", CultureInfo.InvariantCulture);
				break;
			case ConstantPoolItemType.Float:
				var floatItem = (ConstantPoolFloatItem) constant;
				if (Double.IsNaN (floatItem.Value))
					value = "(0.0f / 0.0f)";
				else if (Double.IsNegativeInfinity (floatItem.Value))
					value = "(-1.0f / 0.0f)";
				else if (Double.IsPositiveInfinity (floatItem.Value))
					value = "(1.0f / 0.0f)";
				else
					value = floatItem.Value.ToString ("G9", CultureInfo.InvariantCulture);
				break;
			case ConstantPoolItemType.Long:     value = ((ConstantPoolLongItem) constant).Value.ToString ();    break;
			case ConstantPoolItemType.Integer:
				if (field.Descriptor == "Z")
					value = ((ConstantPoolIntegerItem) constant).Value == 1 ? bool.TrueString.ToLower () : bool.FalseString.ToLower ();
				else
					value = ((ConstantPoolIntegerItem) constant).Value.ToString (); 
				break;
			case ConstantPoolItemType.String:
				value = '"' + EscapeLiteral (((ConstantPoolStringItem) constant).StringData.Value) + '"';
				break;
			default:
				throw new InvalidOperationException ("Unable to get value for: " + constant);
			}
			return new XAttribute ("value", value);
		}

		static string EscapeLiteral (string value)
		{
			bool fixup = false;
			for (int i = 0; i < value.Length; ++i) {
				var c = value [i];
				if (c < 0x20 || c > 0xff || c == '\\' || c == '"') {
					fixup = true;
					break;
				}
			}
			if (fixup) {
				var sb = new StringBuilder ();
				for (int i = 0; i < value.Length; ++i) {
					var c = value [i];
					if (c == '\\') {
						sb.Append (@"\\");
						continue;
					}
					if (c == '"') {
						sb.Append ("\\\"");
						continue;
					}
					if (c < 0x20 || c > 0xff) {
						sb.Append ("\\u").AppendFormat ("{0:x4}", (int)c);
						continue;
					}
					sb.Append (c);
				}
				value = sb.ToString ();
			}
			return value;
		}

		static string GetVisibility (FieldAccessFlags accessFlags)
		{
			if (accessFlags.HasFlag (FieldAccessFlags.Internal))
				return "kotlin-internal";
			if ((accessFlags & FieldAccessFlags.Public) != 0)
				return "public";
			if ((accessFlags & FieldAccessFlags.Protected) != 0)
				return "protected";
			if ((accessFlags & FieldAccessFlags.Private) != 0)
				return "private";
			return "";
		}

		static string GetMethodVisibility (MethodAccessFlags accessFlags)
		{
			if (accessFlags.HasFlag (MethodAccessFlags.Internal))
				return "kotlin-internal";
			if ((accessFlags & MethodAccessFlags.Public) != 0)
				return "public";
			if ((accessFlags & MethodAccessFlags.Protected) != 0)
				return "protected";
			if ((accessFlags & MethodAccessFlags.Private) != 0)
				return "private";
			return "";
		}

		static string GetClassVisibility (ClassAccessFlags accessFlags)
		{
			if ((accessFlags & ClassAccessFlags.Public) != 0)
				return "public";
			if ((accessFlags & ClassAccessFlags.Protected) != 0)
				return "protected";
			if ((accessFlags & ClassAccessFlags.Private) != 0)
				return "private";
			return "";
		}

		IEnumerable<XElement> GetMethods ()
		{
			return classFile.Methods.Where (m => !m.Name.StartsWith ("<", StringComparison.OrdinalIgnoreCase)
						&& (GetMethodVisibility(m.AccessFlags) == "public" || GetMethodVisibility(m.AccessFlags) == "protected" || GetMethodVisibility (m.AccessFlags) == "kotlin-internal"))
				.OrderBy (m => m.Name + m.Descriptor, StringComparer.OrdinalIgnoreCase)
				.Select (m => GetMethod ("method", m.Name, m,
					returns: m.ReturnType.TypeSignature));
		}
	}
}

