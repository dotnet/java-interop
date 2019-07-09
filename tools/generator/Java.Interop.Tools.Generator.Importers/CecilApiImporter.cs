using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace MonoDroid.Generation
{
	class CecilApiImporter
	{
		public static Field CreateField (FieldDefinition f)
		{
			var obs_attr = GetObsoleteAttribute (f.CustomAttributes);
			var reg_attr = GetRegisterAttribute (f.CustomAttributes);

			var field = new Field {
				DeprecatedComment = GetObsoleteComment (obs_attr),
				IsAcw = reg_attr != null,
				IsDeprecated = obs_attr != null,
				IsEnumified = GetGeneratedEnumAttribute (f.CustomAttributes) != null,
				IsFinal = f.Constant != null,
				IsStatic = f.IsStatic,
				JavaName = reg_attr != null ? ((string) reg_attr.ConstructorArguments [0].Value).Replace ('/', '.') : f.Name,
				Name = f.Name,
				TypeName = f.FieldType.FullNameCorrected (),
				Value = f.Constant == null ? null : f.FieldType.FullName == "System.String" ? '"' + f.Constant.ToString () + '"' : f.Constant.ToString (),
				Visibility = f.IsPublic ? "public" : f.IsFamilyOrAssembly ? "protected internal" : f.IsFamily ? "protected" : f.IsAssembly ? "internal" : "private"
			};

			field.SetterParameter = CreateParameter (f.FieldType.Resolve ()?.FullName ?? f.FieldType.FullName, null);
			field.SetterParameter.Name = "value";

			return field;
		}

		public static Parameter CreateParameter (ParameterDefinition p, string jnitype, string rawtype)
		{
			// FIXME: safe to use CLR type name? assuming yes as we often use it in metadatamap.
			// FIXME: IsSender?
			var isEnumType = GetGeneratedEnumAttribute (p.CustomAttributes) != null;;
			return new Parameter (SymbolTable.MangleName (p.Name), jnitype ?? p.ParameterType.FullNameCorrected (), null, isEnumType, rawtype);
		}

		public static Parameter CreateParameter (string managedType, string javaType)
		{
			return new Parameter ("__self", javaType ?? managedType, managedType, false);
		}

		static CustomAttribute GetGeneratedEnumAttribute (Collection<CustomAttribute> attributes) =>
			attributes.FirstOrDefault (a => a.AttributeType.FullName == "Android.Runtime.GeneratedEnumAttribute");

		static CustomAttribute GetObsoleteAttribute (Collection<CustomAttribute> attributes) =>
			attributes.FirstOrDefault (a => a.AttributeType.FullNameCorrected () == "System.ObsoleteAttribute");

		static string GetObsoleteComment (CustomAttribute attribute) =>
			attribute?.ConstructorArguments.Any () == true ? (string) attribute.ConstructorArguments [0].Value : null;

		static CustomAttribute GetRegisterAttribute (Collection<CustomAttribute> attributes) =>
			attributes.FirstOrDefault (a => a.AttributeType.FullNameCorrected () == "Android.Runtime.RegisterAttribute");
	}
}
