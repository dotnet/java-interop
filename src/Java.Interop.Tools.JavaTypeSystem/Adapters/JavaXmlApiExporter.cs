using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Java.Interop.Tools.JavaTypeSystem.Models;

namespace Java.Interop.Tools.JavaTypeSystem
{
	public static class JavaXmlApiExporter
	{
		public static void Save (JavaTypeCollection api, string xmlFile)
		{
			using (var writer = XmlWriter.Create (xmlFile, new XmlWriterSettings {
				Encoding = new UTF8Encoding (false, true),
				Indent = true,
				OmitXmlDeclaration = true,
			}))
				Save (api, writer);
		}

		public static void Save (JavaTypeCollection api, XmlWriter writer)
		{
			writer.WriteStartElement ("api");
			//if (api.Platform != null)
			//	writer.WriteAttributeString ("platform", api.Platform);



			foreach (var pkg in api.Packages.Values) {
				if (!pkg.Types.Any ()) //t => !t.IsReferenceOnly))
					continue;
				writer.WriteStartElement ("package");
				writer.WriteAttributeString ("name", pkg.Name);

				if (pkg.PropertyBag.TryGetValue ("merge.SourceFile", out var source))
					writer.WriteAttributeString ("merge.SourceFile", source);

				if (!string.IsNullOrEmpty (pkg.JniName)) {
					writer.WriteAttributeString ("jni-name", pkg.JniName);
				}
				foreach (var type in pkg.Types) {
					//if (type.IsReferenceOnly)
					//	continue; // skip reference only types

					SaveType (type, writer);
				}
				WriteFullEndElement (writer);
			}
			WriteFullEndElement (writer);
		}

		static void SaveType (JavaTypeModel type, XmlWriter writer)
		{
			if (type is JavaClassModel cls)
				SaveType (type, writer, "class", XmlConvert.ToString (cls.IsAbstract), cls.BaseType, cls.BaseTypeGeneric, cls.BaseTypeJni);
			else
				SaveType (type, writer, "interface", "true", null, null, null);

			foreach (var nested in type.NestedTypes)
				SaveType (nested, writer);
		}

		static void SaveType (JavaTypeModel cls, XmlWriter writer, string elementName, string abs, string? ext, string? extgen, string? jniExt)
		{
			writer.WriteStartElement (elementName);
			if (abs != null)
				writer.WriteAttributeString ("abstract", abs);
			writer.WriteAttributeString ("deprecated", cls.Deprecated);
			if (ext.HasValue ())
				writer.WriteAttributeString ("extends", ext);
			if (ext.HasValue ())
				writer.WriteAttributeString ("extends-generic-aware", extgen);
			if (jniExt.HasValue ())
				writer.WriteAttributeString ("jni-extends", jniExt);
			writer.WriteAttributeString ("final", XmlConvert.ToString (cls.IsFinal));
			writer.WriteAttributeString ("name", cls.NestedName);
			writer.WriteAttributeString ("static", XmlConvert.ToString (cls.IsStatic));
			writer.WriteAttributeString ("visibility", cls.Visibility);
			if (!string.IsNullOrEmpty (cls.ExtendedJniSignature)) {
				writer.WriteAttributeString ("jni-signature", cls.ExtendedJniSignature);
			}

			if (cls.PropertyBag.TryGetValue ("merge.SourceFile", out var source))
				writer.WriteAttributeString ("merge.SourceFile", source);
			if (cls.PropertyBag.TryGetValue ("deprecated-since", out var dep))
				writer.WriteAttributeString ("deprecated-since", dep);

			SaveTypeParameters (cls.TypeParameters, writer);
			foreach (var imp in cls.Implements.OrderBy (i => i.Name, StringComparer.Ordinal)) {
				writer.WriteStartElement ("implements");
				writer.WriteAttributeString ("name", imp.Name);
				writer.WriteAttributeString ("name-generic-aware", imp.NameGeneric);
				if (!string.IsNullOrEmpty (imp.JniType)) {
					writer.WriteAttributeString ("jni-type", imp.JniType);
				}
				if (imp.PropertyBag.TryGetValue ("merge.SourceFile", out var imp_source))
					writer.WriteAttributeString ("merge.SourceFile", imp_source);
				//writer.WriteString ("\n      ");
				WriteFullEndElement (writer);
			}

			//if (cls.TypeParameters != null)
			//	cls.TypeParameters.Save (writer, "      ");

			if (cls is JavaClassModel klass)
				foreach (var m in klass.Constructors) //.OrderBy (m => m.Name, StringComparer.OrdinalIgnoreCase).ThenBy (m => string.Join (", ", m.Parameters.Select (p => p.Type))).ThenBy (m => m.IsSynthetic))
					SaveConstructor (m, writer);
			foreach (var m in cls.Methods) //.OrderBy (m => m.Name, StringComparer.Ordinal).ThenBy (m => string.Join (", ", m.Parameters.Select (p => p.Type))).ThenBy (m => m.ExtendedSynthetic))
				SaveMethod (m, writer);
			foreach (var m in cls.Fields) //.OrderBy (m => m.Name, StringComparer.OrdinalIgnoreCase))
				SaveField (m, writer);

			WriteFullEndElement (writer);
		}

		static void SaveTypeParameters (JavaTypeParameters parameters, XmlWriter writer)
		{
			if (parameters.Count == 0)
				return;

			writer.WriteStartElement ("typeParameters");

			if (parameters.PropertyBag.TryGetValue ("merge.SourceFile", out var source))
				writer.WriteAttributeString ("merge.SourceFile", source);

			foreach (var tp in parameters) {
				writer.WriteStartElement ("typeParameter");
				writer.WriteAttributeString ("name", tp.Name);
				if (!string.IsNullOrEmpty (tp.ExtendedClassBound)) {
					writer.WriteAttributeString ("classBound", tp.ExtendedClassBound);
				}
				if (!string.IsNullOrEmpty (tp.ExtendedJniClassBound)) {
					writer.WriteAttributeString ("jni-classBound", tp.ExtendedJniClassBound);
				}
				if (!string.IsNullOrEmpty (tp.ExtendedInterfaceBounds)) {
					writer.WriteAttributeString ("interfaceBounds", tp.ExtendedInterfaceBounds);
				}
				if (!string.IsNullOrEmpty (tp.ExtendedJniInterfaceBounds)) {
					writer.WriteAttributeString ("jni-interfaceBounds", tp.ExtendedJniInterfaceBounds);
				}

				if (tp.GenericConstraints.Count > 0) {
					// If there is only one generic constraint that specifies java.lang.Object,
					// that is not really a constraint, so skip that.
					// jar2xml does not emit that either.
					if (tp.GenericConstraints.Count == 1 && tp.GenericConstraints[0].Type == "java.lang.Object") {
						WriteFullEndElement (writer);
						continue;
					}

					//var gcs = tp.GenericConstraints.GenericConstraints;
					//var gctr = gcs.Count == 1 ? gcs [0].ResolvedType : null;
					//if (gctr?.ReferencedType?.FullName != "java.lang.Object") {
						writer.WriteStartElement ("genericConstraints");
						foreach (var g in tp.GenericConstraints) {
							writer.WriteStartElement ("genericConstraint");
							writer.WriteAttributeString ("type", g.Type);
							//writer.WriteString ("\n" + indent + "      ");
							WriteFullEndElement (writer);
						}
						WriteFullEndElement (writer);
					//}
				}
				//else
				//	writer.WriteString ("\n" + indent + "  ");
				WriteFullEndElement (writer);
			}
			//writer.WriteString ("\n" + indent);
			WriteFullEndElement (writer);
		}


		static void SaveConstructor (JavaConstructorModel ctor, XmlWriter writer)
			=> SaveMember (ctor, writer, "constructor", null, null, null, null, null, /* ctor.Type ?? */ ctor.ParentType.FullName, null, null, null, /* ctor.TypeParameters, */ ctor.Parameters, /* ctor.Exceptions, */ ctor.IsBridge, null, ctor.IsSynthetic, null);

		static void SaveField (JavaFieldModel field, XmlWriter writer)
		{
			var value = field.Value;

			if (value != null && (field.Type == "double" || field.Type == "float"))
				value = value.Replace ("E+", "E");

			SaveMember (field, writer, "field", null, null, null, null,
				XmlConvert.ToString (field.IsTransient),
				field.Type,
				field.TypeGeneric,
				value,
				XmlConvert.ToString (field.IsVolatile),
				//null,
				null,
				//null,
				null,
				null,
				null,
				field.IsNotNull);

		}

		static void SaveMethod (JavaMethodModel method, XmlWriter writer)
		{

			bool check (JavaMethodModel _) => _.BaseMethod?.ParentType?.Visibility == "public" &&
			     !method.IsStatic &&
					      method.Parameters.All (p => p.InstantiatedGenericArgumentName == null);

			//// skip synthetic methods, that's what jar2xml does.
			//// However, jar2xml is based on Java reflection and it generates synthetic methods
			//// that actually needs to be generated in the output XML (they are not marked as
			//// "synthetic" either by asm or java reflection), when:
			//// - the synthetic method is actually from non-public ancestor class
			////   (e.g. FileBackupHelperBase.writeNewStateDescription())
			//// For such case, it does not skip generation.
			if (method.IsSynthetic && (method.BaseMethod == null || check (method)))
				return;

			//// Here we skip most of the overriding methods of a virtual method, unless
			//// - the method visibility or final-ity has changed: protected Object#clone() is often
			////   overriden as public. In that case, we need a "new" method.
			//// - the method is covariant. In that case we need another overload.
			//// - they differ in "abstract" or "final" method attribute.
			//// - the derived method is static.
			//// - the base method is in the NON-public class.
			//// - none of the arguments are type parameters.
			//// - finally, it is the synthetic method already checked above.
			if (method.BaseMethod != null &&
			    !method.BaseMethod.IsAbstract &&
			    method.BaseMethod.Visibility == method.Visibility &&
			    method.BaseMethod.IsAbstract == method.IsAbstract &&
			    method.BaseMethod.IsFinal == method.IsFinal &&
			    method.BaseMethod.TypeParameters.Count == method.TypeParameters.Count &&
			    !method.IsSynthetic &&
			    check (method))
				return;

			SaveMember (method, writer, "method",
				XmlConvert.ToString (method.IsAbstract),
				XmlConvert.ToString (method.IsNative),
				GetVisibleReturnTypeString (method), //method.Return,
				XmlConvert.ToString (method.IsSynchronized),
				null,
				null,
				null,
				null,
				null,
				//method.TypeParameters,
				method.Parameters,
				//method.Exceptions,
				method.IsBridge,
				method.ReturnJni,
				method.IsSynthetic,
				method.ReturnNotNull);
		}

		static string GetVisibleReturnTypeString (JavaMethodModel method)
		{
			if (GetVisibleNonSpecialType (method, method.ReturnTypeModel) is JavaTypeReference jtr)
				return jtr.ToString ();

			return method.Return;
		}

		public static string? GetVisibleParamterTypeName (this JavaParameterModel parameter)
		{
			if (GetVisibleNonSpecialType (parameter.ParentMethod, parameter.TypeModel) is JavaTypeReference jtr)
				return jtr.ToString ();

			return parameter.GenericType;
		}

		private static JavaTypeReference? GetVisibleNonSpecialType (JavaMethodModel method, JavaTypeReference? r)
		{
			if (r == null || r.SpecialName != null || r.ReferencedTypeParameter != null || r.ArrayPart != null)
				return null;

			var requiredVisibility = method?.Visibility == "public" && method.ParentType?.Visibility == "public" ? "public" : method?.Visibility;

			for (var t = r; t != null; t = (t.ReferencedType as JavaClassModel)?.BaseTypeReference) {
				if (t.ReferencedType == null)
					break;
				if (IsAcceptableVisibility (required: requiredVisibility, actual: t.ReferencedType.Visibility))
					return t;
			}

			return null;
		}

		static bool IsAcceptableVisibility (string? required, string? actual)
		{
			if (required == "public")
				return actual == "public";
			else
				return true;
		}

		static void SaveMember (JavaMemberModel m, XmlWriter writer, string elementName,
					string? abs, string? native, string? ret, string? sync,
					string? transient, string? type, string? typeGeneric,
					string? value, string? volat,
					//JavaTypeParameters? typeParameters,
					IEnumerable<JavaParameterModel>? parameters,
					//IEnumerable<JavaException>? exceptions,
					bool? extBridge, string? jniReturn, bool? extSynthetic, bool? notNull)
		{
			// If any of the parameters contain reference to non-public type, it cannot be generated.
			// TODO
			//if (parameters != null && parameters.Any (p => p.ResolvedType?.ReferencedType != null && string.IsNullOrEmpty (p.ResolvedType.ReferencedType.Visibility)))
			//	return;

			if (parameters != null && parameters.Any (p => p.TypeModel != null && p.TypeModel.ReferencedType?.Visibility.HasValue () == false))
				return;

			writer.WriteStartElement (elementName);
			if (abs != null)
				writer.WriteAttributeString ("abstract", abs);
			writer.WriteAttributeString ("deprecated", m.Deprecated);
			writer.WriteAttributeString ("final", XmlConvert.ToString (m.IsFinal));
			writer.WriteAttributeString ("name", m.Name);
			writer.WriteAttributeString ("jni-signature", m.JniSignature);
			if (notNull.GetValueOrDefault () && m is JavaFieldModel)
				writer.WriteAttributeString (m is JavaFieldModel ? "not-null" : "return-not-null", "true");
			if (extBridge.HasValue)
				writer.WriteAttributeString ("bridge", extBridge.Value ? "true" : "false");
			if (native != null)
				writer.WriteAttributeString ("native", native);
			if (ret != null)
				writer.WriteAttributeString ("return", ret);
			if (jniReturn != null)
				writer.WriteAttributeString ("jni-return", jniReturn);
			writer.WriteAttributeString ("static", XmlConvert.ToString (m.IsStatic));
			if (sync != null)
				writer.WriteAttributeString ("synchronized", sync);
			if (transient != null)
				writer.WriteAttributeString ("transient", transient);
			if (type != null)
				writer.WriteAttributeString ("type", type);
			if (typeGeneric != null)
				writer.WriteAttributeString ("type-generic-aware", typeGeneric);
			if (value != null)
				writer.WriteAttributeString ("value", value);
			if (extSynthetic.HasValue)
				writer.WriteAttributeString ("synthetic", extSynthetic.Value ? "true" : "false");
			writer.WriteAttributeString ("visibility", m.Visibility);
			if (volat != null)
				writer.WriteAttributeString ("volatile", volat);

			if (m.PropertyBag.TryGetValue ("merge.SourceFile", out var source))
				writer.WriteAttributeString ("merge.SourceFile", source);
			if (m.PropertyBag.TryGetValue ("deprecated-since", out var dep))
				writer.WriteAttributeString ("deprecated-since", dep);
			if (notNull.GetValueOrDefault () && !(m is JavaFieldModel))
				writer.WriteAttributeString (m is JavaFieldModel ? "not-null" : "return-not-null", "true");

			if (m is JavaMethodModel m2)
				SaveTypeParameters (m2.TypeParameters, writer);
			//if (typeParameters != null)
			//	typeParameters.Save (writer, "      ");

			if (parameters != null) {
				foreach (var p in parameters) {
					writer.WriteStartElement ("parameter");
					writer.WriteAttributeString ("name", p.Name);
					writer.WriteAttributeString ("type", GetVisibleParamterTypeName (p));
					if (!string.IsNullOrEmpty (p.JniType)) {
						writer.WriteAttributeString ("jni-type", p.JniType);
					}
					if (p.IsNotNull == true) {
						writer.WriteAttributeString ("not-null", "true");
					}
					//writer.WriteString ("\n        ");
					WriteFullEndElement (writer);
					//WriteFullEndElement (writer);
				}
			}

			if (m is JavaMethodModel method) {
				foreach (var e in method.Exceptions.OrderBy (e => e.Name.LastSubset ('/'), StringComparer.Ordinal)) {
					writer.WriteStartElement ("exception");
					writer.WriteAttributeString ("name", e.Name.LastSubset ('/'));
					writer.WriteAttributeString ("type", e.Type);
					//if (!string.IsNullOrEmpty (e.TypeGenericAware)) {
					//	writer.WriteAttributeString ("type-generic-aware", e.TypeGenericAware);
					//}
					//writer.WriteString ("\n        ");
					WriteFullEndElement (writer);
				}
			}

			//writer.WriteString ("\n      ");

			//if (m is JavaMethodModel)
			WriteFullEndElement (writer);
			//else
			//	writer.WriteFullEndElement ();
		}

		static void WriteFullEndElement (XmlWriter writer) => writer.WriteEndElement ();
	}
}
