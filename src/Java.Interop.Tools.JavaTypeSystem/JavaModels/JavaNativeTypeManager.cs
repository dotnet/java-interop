#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
//using Android.Runtime;
//using Java.Interop.Tools.JavaCallableWrappers;

using Mono.Cecil;
using Java.Interop.Tools.Cecil;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public
	enum ExportParameterKind
	{
		Unspecified,
		InputStream,
		OutputStream,
		XmlPullParser,
		XmlResourceParser
	}

	public
	enum PackageNamingPolicy {
		[Obsolete ("No longer supported. Use PackageNamingPolicy.LowercaseCrc64 instead.", error: true)]
		LowercaseHash = 0,
		Lowercase = 1,
		LowercaseWithAssemblyName = 2,
		[Obsolete ("No longer supported. Use PackageNamingPolicy.LowercaseCrc64 instead.", error: true)]
		LowercaseMD5 = LowercaseHash,
		LowercaseCrc64 = 3,
	}

	public class JniSignature
	{
		public JniTypeName Return { get; }
		public List<JniTypeName> Parameters { get; } = new List<JniTypeName> ();

		public JniSignature (JniTypeName returnType, params JniTypeName[] parameterTypes)
		{
			Return = returnType;
			Parameters.AddRange (parameterTypes);
		}

		public static JniSignature Parse (string signature)
		{
			var idx = signature.LastIndexOf (')') + 1;
			var jni = new JniSignature (JniTypeName.Parse (signature.Substring (idx)));

			// Strip out return type
			if (signature.StartsWith ("(")) {
				var e = signature.IndexOf (")");
				signature = signature.Substring (1, e >= 0 ? e - 1 : signature.Length - 1);
			}

			// Parse parameters
			var i = 0;

			while (i < signature.Length) {
				var t = JniTypeName.Parse (signature.Substring (i));

				jni.Parameters.Add (t);
				i += t.Jni.Length;
			}

			return jni;
		}

		public override string ToString ()
		{
			return $"({string.Join ("", Parameters)}){Return}";
		}
	}

	public class JniTypeName
	{
		public string Type { get; }
		public string Jni { get; }
		public bool IsKeyword { get; }

		public JniTypeName (string jni, string type, bool isKeyword)
		{
			Jni = jni;
			Type = type;
			IsKeyword = isKeyword;
		}

		// This returns the first type found in the signature and ignores any extra signature data
		public static JniTypeName Parse (string signature)
		{
			var index = 0;

			switch (signature [index]) {
				case '[': {
						++index;

						if (index >= signature.Length)
							throw new InvalidOperationException ("Missing array type after '[' at index " + index + " in: " + signature);

						var r = Parse (signature.Substring (index));

						return new JniTypeName (signature.Substring (0, index) + r.Jni, r.Type + "[]", r.IsKeyword);
					}
				case 'B':
					return new JniTypeName ("B", "byte", true);
				case 'C':
					return new JniTypeName ("C", "char", true);
				case 'D':
					return new JniTypeName ("D", "double", true);
				case 'F':
					return new JniTypeName ("F", "float", true);
				case 'I':
					return new JniTypeName ("I", "int", true);
				case 'J':
					return new JniTypeName ("J", "long", true);
				case 'L': {
						var e = signature.IndexOf (";", index);

						if (e <= 0)
							throw new InvalidOperationException ("Missing reference type after 'L' at index " + index + "in: " + signature);

						//var s = index;
						//index = e + 1;

						return new JniTypeName (
							signature.Substring (0, e + 1),
							signature.Substring (index + 1, e - 1).Replace ("/", ".").Replace ("$", "."),
							false
						);
					}
				case 'S':
					return new JniTypeName ("S", "short", true);
				case 'V':
					return new JniTypeName ("V", "void", true);
				case 'Z':
					return new JniTypeName ("Z", "boolean", true);
				default:
					throw new InvalidOperationException ("Unknown JNI Type '" + signature [index] + "' within: " + signature);
			}
		}

		// This throws an exception if there is extra data in the signature
		public static JniTypeName ParseExact (string signature)
		{
			var jni = Parse (signature);

			if (jni.Jni.Length != signature.Length)
				throw new InvalidOperationException ("Extra JNI signature");

			return jni;
		}

		public override string ToString () => Jni;
	}

	public
	static class JavaNativeTypeManager {
		const string CRC_PREFIX = "crc64";

		public static PackageNamingPolicy PackageNamingPolicy { get; set; } = PackageNamingPolicy.LowercaseCrc64;

		public static string? ApplicationJavaClass { get; set; }

		//public static JniTypeName Parse (string jniType)
		//{
		//	int _ = 0;
		//	return ExtractType (jniType, ref _);
		//}

		public static IEnumerable<JniTypeName> GetParametersFromSignature (string signature)
		{
			// Strip out return type
			if (signature.StartsWith ("(")) {
				var e = signature.IndexOf (")");
				signature = signature.Substring (1, e >= 0 ? e-1 : signature.Length-1);
			}

			var i = 0;

			while (i < signature.Length) {
				var t = JniTypeName.Parse (signature.Substring (i));

				yield return t;

				i += t.Jni.Length;
			}

			//JniTypeName t;
			//while ((t = ExtractType (signature, ref i)) != null)
			//	yield return t;
		}

		public static JniTypeName GetReturnFromSignature (string signature)
		{
			int idx = signature.LastIndexOf (')') + 1;
			return JniTypeName.Parse (signature.Substring (idx));
		}

		//public static string ReturnJniFromSignature (string signature)
		//{
		//	int idx = signature.LastIndexOf (')') + 1;
		//	return signature.Substring (0, idx);
		//}

		// as per: http://java.sun.com/j2se/1.5.0/docs/guide/jni/spec/types.html
		//[return: NotNullIfNotNull ("signature")]
		//static JniTypeName? ExtractType (string? signature, ref int index)
		//{
		//	if (signature is null || index >= signature.Length)
		//		return null;
		//	var i = index++;
		//	switch (signature [i]) {
		//		case '[': {
		//			++i;
		//			if (i >= signature.Length)
		//				throw new InvalidOperationException ("Missing array type after '[' at index " + i + " in: " + signature);
		//			var r = ExtractType (signature, ref index);
		//			return new JniTypeName { Type = r.Type + "[]", IsKeyword = r.IsKeyword };
		//		}
		//		case 'B':
		//			return new JniTypeName { Type = "byte", IsKeyword = true };
		//		case 'C':
		//			return new JniTypeName { Type = "char", IsKeyword = true };
		//		case 'D':
		//			return new JniTypeName { Type = "double", IsKeyword = true };
		//		case 'F':
		//			return new JniTypeName { Type = "float", IsKeyword = true };
		//		case 'I':
		//			return new JniTypeName { Type = "int", IsKeyword = true };
		//		case 'J':
		//			return new JniTypeName { Type = "long", IsKeyword = true };
		//		case 'L': {
		//			var e = signature.IndexOf (";", index);
		//			if (e <= 0)
		//				throw new InvalidOperationException ("Missing reference type after 'L' at index " + i + "in: " + signature);
		//			var s = index;
		//			index = e + 1;
		//			return new JniTypeName {
		//				Type      = signature.Substring (s, e - s).Replace ("/", ".").Replace ("$", "."),
		//				IsKeyword = false,
		//			};
		//		}
		//		case 'S':
		//			return new JniTypeName { Type = "short", IsKeyword = true };
		//		case 'V':
		//			return new JniTypeName { Type = "void", IsKeyword = true };
		//		case 'Z':
		//			return new JniTypeName { Type = "boolean", IsKeyword = true };
		//		default:
		//			throw new InvalidOperationException ("Unknown JNI Type '" + signature [i] + "' within: " + signature);
		//	}
		//}

		public static string ToCliType (string jniType)
		{
			if (string.IsNullOrEmpty (jniType))
				return jniType;
			string[] parts = jniType.Split ('/');
			for (int i = 0; i < parts.Length; ++i) {
				parts [i] = ToCliTypePart (parts [i]);
			}
			return string.Join (".", parts);
		}

		static string ToCliTypePart (string part)
		{
			if (part.IndexOf ('$') < 0)
				return ToPascalCase (part, 2);
			string[] parts = part.Split ('$');
			for (int i = 0; i < parts.Length; ++i) {
				parts [i] = ToPascalCase (parts [i], 1);
			}
			return string.Join ("/", parts);
		}

		static string ToPascalCase (string value, int minLength)
		{
			return value.Length <= minLength
				? value.ToUpperInvariant ()
				: char.ToUpperInvariant (value [0]) + value.Substring (1);
		}

		// Keep in sync with ToJniName(TypeDefinition)
		//public static string ToJniName (Type type)
		//{
		//	return ToJniName (type, ExportParameterKind.Unspecified) ??
		//		"java/lang/Object";
		//}

		//static string? ToJniName (Type type, ExportParameterKind exportKind)
		//{
		//	if (type == null)
		//		throw new ArgumentNullException ("type");

		//	if (type.IsValueType)
		//		return GetPrimitiveClass (type);

		//	if (type == typeof (string))
		//		return "java/lang/String";


		//	if (!type.GetInterfaces ().Any (t => t.FullName == "Android.Runtime.IJavaObject"))
		//		return GetSpecialExportJniType (type.FullName!, exportKind);

		//	return ToJniName (type, t => t.DeclaringType!, t => t.Name, GetPackageName, t => {
		//		return ToJniNameFromAttributes (t);
		//	}, _ => false);
		//}

		public static string ToJniName (string jniType, int rank)
		{
			if (rank == 0)
				return jniType;

			if (jniType.Length > 1)
				jniType = "L" + jniType + ";";
			return new string ('[', rank) + jniType;
		}

		static bool IsPackageNamePreservedForAssembly (string assemblyName)
		{
			return assemblyName == "Mono.Android";
		}

		//public static string GetPackageName (Type type)
		//{
		//	string assemblyName = GetAssemblyName (type.Assembly);
		//	if (IsPackageNamePreservedForAssembly (assemblyName))
		//		return type.Namespace!.ToLowerInvariant ();
		//	switch (PackageNamingPolicy) {
		//	case PackageNamingPolicy.Lowercase:
		//		return type.Namespace!.ToLowerInvariant ();
		//	case PackageNamingPolicy.LowercaseWithAssemblyName:
		//		return "assembly_" + (assemblyName.Replace ('.', '_') + "." + type.Namespace).ToLowerInvariant ();
		//	case PackageNamingPolicy.LowercaseCrc64:
		//		return CRC_PREFIX + ToCrc64 (type.Namespace + ":" + assemblyName);
		//	default:
		//			throw new NotSupportedException ($"PackageNamingPolicy.{PackageNamingPolicy} is no longer supported.");
		//	}
		//}

		/// <summary>
		/// A more performant equivalent of `Assembly.GetName().Name`
		/// </summary>
		static string GetAssemblyName (Assembly assembly)
		{
			var name = assembly.FullName!;
			int index = name.IndexOf (',');
			if (index != -1) {
				return name.Substring (0, index);
			}
			return name;
		}

		public static int GetArrayInfo (Type type, out Type elementType)
		{
			elementType = type;
			int rank = 0;
			while (type.IsArray) {
				rank++;
				elementType = type = type.GetElementType ()!;
			}
			return rank;
		}

		static string? GetPrimitiveClass (Type type)
		{
			if (type.IsEnum)
				return GetPrimitiveClass (Enum.GetUnderlyingType (type));
			if (type == typeof (byte))
				return "B";
			if (type == typeof (char))
				return "C";
			if (type == typeof (double))
				return "D";
			if (type == typeof (float))
				return "F";
			if (type == typeof (int))
				return "I";
			if (type == typeof (uint))
				return "I";
			if (type == typeof (long))
				return "J";
			if (type == typeof (ulong))
				return "J";
			if (type == typeof (short))
				return "S";
			if (type == typeof (ushort))
				return "S";
			if (type == typeof (bool))
				return "Z";
			return null;
		}

		static string? GetSpecialExportJniType (string typeName, ExportParameterKind exportKind)
		{
			switch (exportKind) {
			case ExportParameterKind.InputStream:
				if (typeName != "System.IO.Stream")
					throw new ArgumentException ("ExportParameterKind.InputStream is valid only for System.IO.Stream parameter type");
				return "java/io/InputStream";
			case ExportParameterKind.OutputStream:
				if (typeName != "System.IO.Stream")
					throw new ArgumentException ("ExportParameterKind.OutputStream is valid only for System.IO.Stream parameter type");
				return "java/io/OutputStream";
			case ExportParameterKind.XmlPullParser:
				if (typeName != "System.Xml.XmlReader")
					throw new ArgumentException ("ExportParameterKind.XmlPullParser is valid only for System.Xml.XmlReader parameter type");
				return "org/xmlpull/v1/XmlPullParser";
			case ExportParameterKind.XmlResourceParser:
				if (typeName != "System.Xml.XmlReader")
					throw new ArgumentException ("ExportParameterKind.XmlResourceParser is valid only for System.Xml.XmlReader parameter type");
				return "android/content/res/XmlResourceParser";
			}
			// FIXME: this *must* error out here, instead of returning null.
			// Either Droidinator must be fixed to not reach here, or a global flag that skips this error check must be added.
			return null;
		}

		// Keep in sync with ToJniNameFromAttributes(TypeDefinition)
		//public static string? ToJniNameFromAttributes (Type type)
		//{
		//	var aa = (IJniNameProviderAttribute []) type.GetCustomAttributes (typeof (IJniNameProviderAttribute), inherit: false);
		//	return aa.Length > 0 && !string.IsNullOrEmpty (aa [0].Name) ? aa [0].Name.Replace ('.', '/') : null;
		//}

		/*
		 * Semantics: return `null` on "failure", DO NOT throw an exception.
		 *
		 * Why? tools/msbuild/Generator/JavaTypeInfo.cs!AddConstructors() attempts
		 * to generate (non-[Export]) constructors, and to determine whether or
		 * not the constructor CAN be declared it calls
		 * JniType.GetJniSignature(MethodDefinition). If GetJniSignature() returns
		 * null, it can't be exported, and the method is skipped.
		 *
		 * Callers of GetJniSignature() MUST check for `null` and behave
		 * appropriately.
		 */
		static string? GetJniSignature<T,P> (IEnumerable<P> parameters, Func<P,T> getParameterType, Func<P,ExportParameterKind> getExportKind, T returnType, ExportParameterKind returnExportKind, Func<T,ExportParameterKind,string?> getJniTypeName, bool isConstructor)
		{
			StringBuilder sb = new StringBuilder ().Append ("(");
			foreach (P p in parameters) {
				var jniType = getJniTypeName (getParameterType (p), getExportKind (p));
				if (jniType == null)
					return null;
				sb.Append (jniType);
			}
			sb.Append (')');
			if (isConstructor)
				sb.Append ("V");
			else {
				var jniType = getJniTypeName (returnType, returnExportKind);
				if (jniType == null)
					return null;
				sb.Append (jniType);
			}
			return sb.ToString ();
		}

		static string? GetJniTypeName<TR,TD> (TR typeRef, ExportParameterKind exportKind, Func<TR,TD> resolve, Func<TR,KeyValuePair<int,TR>> getArrayInfo, Func<TD,string> getFullName, Func<TD,ExportParameterKind,string?> toJniName)
		{
			TD ptype = resolve (typeRef);
			var p = getArrayInfo (typeRef);
			int rank = p.Key;
			TR etype = p.Value;
			ptype = resolve (etype);
			if (ptype == null) {
				// Likely caused by generic parameters, which we probably can't bind anyway.
				return null;
			}
			if (getFullName (ptype) == "System.Void")
				return "V";
			if (getFullName (ptype) == "System.IntPtr")
				// Probably a (IntPtr, JniHandleOwnership) parameter; skip
				return null;

			var pJniName = toJniName (ptype, exportKind);
			if (pJniName == null) {
				return null;
			}
			return rank == 0 && pJniName.Length > 1 ? "L" + pJniName + ";" : ToJniName (pJniName, rank);
		}

		//static ExportParameterKind GetExportKind (System.Reflection.ICustomAttributeProvider p)
		//{
		//	foreach (ExportParameterAttribute a in p.GetCustomAttributes (typeof (ExportParameterAttribute), false))
		//		return a.Kind;
		//	return ExportParameterKind.Unspecified;
		//}

		//public static string? GetJniSignature (MethodInfo method)
		//{
		//	return GetJniSignature<Type,ParameterInfo> (method.GetParameters (),
		//		p => p.ParameterType,
		//		p => GetExportKind (p),
		//		method.ReturnType,
		//		GetExportKind (method.ReturnParameter),
		//		(t, k) => GetJniTypeName (t, k),
		//		method.IsConstructor);
		//}

		//public static string? GetJniTypeName (Type typeRef)
		//{
		//	return GetJniTypeName (typeRef, ExportParameterKind.Unspecified);
		//}

		//internal static string? GetJniTypeName (Type typeRef, ExportParameterKind exportKind)
		//{
		//	return GetJniTypeName<Type,Type> (typeRef, exportKind, t => t, t => {
		//		Type etype;
		//		int rank = GetArrayInfo (t, out etype);
		//		return new KeyValuePair<int,Type> (rank, etype);
		//	}, t => t.FullName!, (t, k) => ToJniNameWhichShouldReplaceExistingToJniName (t, k));
		//}

		//static string? ToJniNameWhichShouldReplaceExistingToJniName (Type type, ExportParameterKind exportKind)
		//{
		//	// we need some method that exactly does the same as ToJniName(TypeDefinition)
		//	var ret = ToJniNameFromAttributes (type);
		//	return ret ?? ToJniName (type, exportKind);
		//}


		//internal static ExportParameterKind GetExportKind (Mono.Cecil.ICustomAttributeProvider p)
		//{
		//	foreach (CustomAttribute a in p.GetCustomAttributes (typeof (ExportParameterAttribute)))
		//		return ToExportParameterAttribute (a).Kind;
		//	return ExportParameterKind.Unspecified;
		//}

		//internal static ExportParameterAttribute ToExportParameterAttribute (CustomAttribute attr)
		//{
		//	return new ExportParameterAttribute ((ExportParameterKind)attr.ConstructorArguments [0].Value);
		//}

		//[Obsolete ("Use the TypeDefinitionCache overload for better performance.")]
		//public static bool IsApplication (TypeDefinition type) =>
		//	IsApplication (type, cache: null);

		//public static bool IsApplication (TypeDefinition type, TypeDefinitionCache? cache)
		//{
		//	return type.GetBaseTypes (cache).Any (b => b.FullName == "Android.App.Application");
		//}

		//[Obsolete ("Use the TypeDefinitionCache overload for better performance.")]
		//public static bool IsInstrumentation (TypeDefinition type) =>
		//	IsInstrumentation (type, cache: null);

		//public static bool IsInstrumentation (TypeDefinition type, TypeDefinitionCache? cache)
		//{
		//	return type.GetBaseTypes (cache).Any (b => b.FullName == "Android.App.Instrumentation");
		//}

		// moved from JavaTypeInfo
		//[Obsolete ("Use the TypeDefinitionCache overload for better performance.")]
		//public static string? GetJniSignature (MethodDefinition method) =>
		//	GetJniSignature (method, cache: null);

		//public static string? GetJniSignature (MethodDefinition method, TypeDefinitionCache? cache)
		//{
		//	return GetJniSignature<TypeReference,ParameterDefinition> (
		//		method.Parameters,
		//		p => p.ParameterType,
		//		p => GetExportKind (p),
		//		method.ReturnType,
		//		GetExportKind (method.MethodReturnType),
		//		(t, k) => GetJniTypeName (t, k, cache),
		//		method.IsConstructor);
		//}

		//// moved from JavaTypeInfo
		//[Obsolete ("Use the TypeDefinitionCache overload for better performance.")]
		//public static string? GetJniTypeName (TypeReference typeRef) =>
		//	GetJniTypeName (typeRef, cache: null);

		//public static string? GetJniTypeName (TypeReference typeRef, TypeDefinitionCache? cache)
		//{
		//	return GetJniTypeName (typeRef, ExportParameterKind.Unspecified, cache);
		//}

		//internal static string? GetJniTypeName (TypeReference typeRef, ExportParameterKind exportKind, TypeDefinitionCache? cache)
		//{
		//	return GetJniTypeName<TypeReference, TypeDefinition> (typeRef, exportKind, t => t.Resolve (), t => {
		//		TypeReference etype;
		//		int rank = GetArrayInfo (typeRef, out etype);
		//		return new KeyValuePair<int,TypeReference> (rank,etype);
		//		}, t => t.FullName, (t, k) => ToJniName (t, k, cache));
		//}

		//[Obsolete ("Use the TypeDefinitionCache overload for better performance.")]
		//public static string ToCompatJniName (TypeDefinition type) =>
		//	ToCompatJniName (type, cache: null);

		//public static string ToCompatJniName (TypeDefinition type, TypeDefinitionCache? cache)
		//{
		//	return ToJniName (type, t => t.DeclaringType, t => t.Name, ToCompatPackageName, ToJniNameFromAttributes, t => IsNonStaticInnerClass (t as TypeDefinition, cache));
		//}

		static string ToCompatPackageName (TypeDefinition type)
		{
			return type.Namespace;
		}

		//// Keep in sync with ToJniNameFromAttributes(Type) and ToJniName(Type)
		//public static string ToJniName (TypeDefinition type) =>
		//	ToJniName (type, cache: null);

		//public static string ToJniName (TypeDefinition type, TypeDefinitionCache? cache)
		//{
		//	return ToJniName (type, ExportParameterKind.Unspecified, cache) ??
		//		"java/lang/Object";
		//}

		//static string? ToJniName (TypeDefinition type, ExportParameterKind exportKind, TypeDefinitionCache? cache)
		//{
		//	if (type == null)
		//		throw new ArgumentNullException ("type");

		//	if (type.IsValueType)
		//		return GetPrimitiveClass (type);

		//	if (type.FullName == "System.String")
		//		return "java/lang/String";

		//	if (!type.ImplementsInterface ("Android.Runtime.IJavaObject", cache)) {
		//		return GetSpecialExportJniType (type.FullName, exportKind);
		//	}

		//	return ToJniName (type, t => t.DeclaringType, t => t.Name, t => GetPackageName (t, cache), ToJniNameFromAttributes, t => IsNonStaticInnerClass (t as TypeDefinition, cache));
		//}

		//static string? ToJniNameFromAttributes (TypeDefinition type)
		//{
		//	#region CustomAttribute alternate name support
		//	var attrs = type.CustomAttributes.Where (a => a.AttributeType.Resolve ().Interfaces.Any (it => it.InterfaceType.FullName == typeof (IJniNameProviderAttribute).FullName));
		//	return attrs.Select (attr => {
		//		var ap = attr.Properties.FirstOrDefault (p => p.Name == "Name");
		//		string? name = null;
		//		if (ap.Name == null) {
		//			var ca = attr.ConstructorArguments.FirstOrDefault ();
		//			if (ca.Type == null || ca.Type.FullName != "System.String")
		//				return null;
		//			name = (string) ca.Value;
		//		} else
		//			name = (string) ap.Argument.Value;
		//		if (!string.IsNullOrEmpty (name))
		//			return name.Replace ('.', '/');
		//		else
		//			return null;
		//		})
		//		.FirstOrDefault (s => s != null);
		//	#endregion
		//}

		public static int GetArrayInfo (Mono.Cecil.TypeReference type, out Mono.Cecil.TypeReference elementType)
		{
			elementType = type;
			int rank = 0;
			while (type.IsArray) {
				rank++;
				elementType = type = type.GetElementType ();
			}
			return rank;
		}

		static string? GetPrimitiveClass (Mono.Cecil.TypeDefinition type)
		{
			if (type.IsEnum)
				return GetPrimitiveClass (type.Fields.First (f => f.IsSpecialName).FieldType.Resolve ());
			if (type.FullName == "System.Byte")
				return "B";
			if (type.FullName == "System.Char")
				return "C";
			if (type.FullName == "System.Double")
				return "D";
			if (type.FullName == "System.Single")
				return "F";
			if (type.FullName == "System.Int32")
				return "I";
			if (type.FullName == "System.Int64")
				return "J";
			if (type.FullName == "System.Int16")
				return "S";
			if (type.FullName == "System.Boolean")
				return "Z";
			return null;
		}

		//[Obsolete ("Use the TypeDefinitionCache overload for better performance.")]
		//public static string GetPackageName (TypeDefinition type) =>
		//	GetPackageName (type, cache: null);

		//public static string GetPackageName (TypeDefinition type, TypeDefinitionCache? cache)
		//{
		//	if (IsPackageNamePreservedForAssembly (type.GetPartialAssemblyName (cache)))
		//		return type.Namespace.ToLowerInvariant ();
		//	switch (PackageNamingPolicy) {
		//	case PackageNamingPolicy.Lowercase:
		//		return type.Namespace.ToLowerInvariant ();
		//	case PackageNamingPolicy.LowercaseWithAssemblyName:
		//		return "assembly_" + (type.GetPartialAssemblyName (cache).Replace ('.', '_') + "." + type.Namespace).ToLowerInvariant ();
		//	case PackageNamingPolicy.LowercaseCrc64:
		//		return CRC_PREFIX + ToCrc64 (type.Namespace + ":" + type.GetPartialAssemblyName (cache));
		//	default:
		//			throw new NotSupportedException ($"PackageNamingPolicy.{PackageNamingPolicy} is no longer supported.");
		//	}
		//}

		static string ToJniName<T> (T type, Func<T, T> decl, Func<T, string> name, Func<T, string> ns, Func<T, string?> overrideName, Func<T,bool> shouldUpdateName)
			where T : class
		{
			var nameParts   = new List<string> ();
			var typeName    = (string?) null;
			var nsType      = type;

			for (var declType = type ; declType != null; declType = decl (declType)) {
				nsType      = declType;
				typeName    = overrideName (declType);
				if (typeName != null) {
					break;
				}
				var n   = name (declType).Replace ('`', '_');
				if (shouldUpdateName (declType)) {
					n = "$" + name (decl (declType)) + "_" + n;
				}
				nameParts.Add (n);
			}

			if (nameParts.Count == 0 && typeName != null)
				return typeName;

			nameParts.Reverse ();

			var nestedSuffix    = string.Join ("_", nameParts.ToArray ()).Replace ("_$", "$");
			if (typeName != null)
				return (typeName + "_" + nestedSuffix).Replace ("_$", "$");;

			// Results in namespace/parts/OuterType_InnerType
			// We do this to simplify monodroid type generation
			typeName = nestedSuffix;
			var _ns = ToLowerCase (ns (nsType));
			return string.IsNullOrEmpty (_ns)
				? typeName
				: _ns.Replace ('.', '/') + "/" + typeName;
		}

		//internal static bool IsNonStaticInnerClass (TypeDefinition? type, TypeDefinitionCache? cache)
		//{
		//	if (type == null)
		//		return false;
		//	if (!type.IsNested)
		//		return false;

		//	if (!type.DeclaringType.IsSubclassOf ("Java.Lang.Object", cache))
		//		return false;

		//	return GetBaseConstructors (type, cache)
		//		.Any (ctor => ctor.Parameters.Any (p => p.Name == "__self"));
		//}

		//static IEnumerable<MethodDefinition> GetBaseConstructors (TypeDefinition type, TypeDefinitionCache? cache)
		//{
		//	var baseType = type.GetBaseTypes (cache).FirstOrDefault (t => t.GetCustomAttributes (typeof (RegisterAttribute)).Any ());
		//	if (baseType != null)
		//		return baseType.Methods.Where (m => m.IsConstructor && !m.IsStatic);
		//	return Enumerable.Empty<MethodDefinition> ();
		//}

		//static string ToCrc64 (string value)
		//{
		//	var data = Encoding.UTF8.GetBytes (value);
		//	var hash = Crc64Helper.Compute (data);
		//	var buf  = new StringBuilder (hash.Length * 2);
		//	foreach (var b in hash)
		//		buf.AppendFormat ("{0:x2}", b);
		//	return buf.ToString ();
		//}

		static string ToLowerCase (string value)
		{
			if (string.IsNullOrEmpty (value))
				return value;
			string[] parts = value.Split ('.');
			for (int i = 0; i < parts.Length; ++i) {
				parts [i] = parts [i].ToLowerInvariant ();
			}
			return string.Join (".", parts);
		}
	}
}


