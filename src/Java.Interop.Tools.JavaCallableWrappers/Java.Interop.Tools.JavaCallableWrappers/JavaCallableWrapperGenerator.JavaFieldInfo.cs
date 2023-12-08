using System;

using Mono.Cecil;
using Java.Interop.Tools.TypeNameMappings;

using MethodAttributes = Mono.Cecil.MethodAttributes;
using static Java.Interop.Tools.TypeNameMappings.JavaNativeTypeManager;

namespace Java.Interop.Tools.JavaCallableWrappers
{
	public partial class JavaCallableWrapperGenerator {
		internal class JavaFieldInfo {
			public JavaFieldInfo (MethodDefinition method, string fieldName, IMetadataResolver resolver)
			{
				this.FieldName = fieldName;
				InitializerName = method.Name;
				TypeName = JavaNativeTypeManager.ReturnTypeFromSignature (GetJniSignature (method, resolver))?.Type
					?? throw new ArgumentException ($"Could not get JNI signature for method `{method.Name}`", nameof (method));
				IsStatic = method.IsStatic;
				Access = method.Attributes & MethodAttributes.MemberAccessMask;
				Annotations = GetAnnotationsString ("\t", method.CustomAttributes, resolver);
			}

			public MethodAttributes Access { get; private set; }
			public bool IsStatic { get; private set; }
			public string TypeName { get; private set; }
			public string FieldName { get; private set; }
			public string InitializerName { get; private set; }
			public string Annotations { get; private set; }

			public string GetJavaAccess ()
			{
				return JavaCallableWrapperGenerator.GetJavaAccess (Access);
			}
		}
	}
}
