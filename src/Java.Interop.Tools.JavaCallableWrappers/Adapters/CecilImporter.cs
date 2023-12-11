using System;
using System.Text;
using System.Xml.Linq;
using Android.Runtime;
using Java.Interop.Tools.Diagnostics;
using Java.Interop.Tools.JavaCallableWrappers.CallableWrapperMembers;
using Java.Interop.Tools.TypeNameMappings;
using Mono.Cecil;

namespace Java.Interop.Tools.JavaCallableWrappers.Adapters;

class CecilImporter
{
	public static CallableWrapperField CreateField (MethodDefinition method, string fieldName, IMetadataResolver resolver)
	{
		var visibility = JavaCallableWrapperGenerator.GetJavaAccess (method.Attributes & MethodAttributes.MemberAccessMask);
		var type_name = JavaNativeTypeManager.ReturnTypeFromSignature (JavaNativeTypeManager.GetJniSignature (method, resolver))?.Type
			?? throw new ArgumentException ($"Could not get JNI signature for method `{method.Name}`", nameof (method));
		var annotations = JavaCallableWrapperGenerator.GetAnnotationsString ("\t", method.CustomAttributes, resolver);

		return new CallableWrapperField (
			fieldName: fieldName,
			typeName: type_name,
			visibility: visibility,
			isStatic: method.IsStatic,
			initializerName: method.Name,
			annotations: annotations);
	}

	// Temporary conversion function
	public static CallableWrapperField CreateField (JavaCallableWrapperGenerator.JavaFieldInfo field)
	{
		return new CallableWrapperField (
			fieldName: field.FieldName,
			typeName: field.TypeName,
			visibility: field.GetJavaAccess (),
			isStatic: field.IsStatic,
			initializerName: field.InitializerName,
			annotations: field.Annotations);
	}

	public static CallableWrapperMethod CreateMethod (MethodDefinition method, RegisterAttribute register, IMetadataResolver cache, bool shouldBeDynamicallyRegistered = true)
		=> CreateMethod (method, register, null, null, cache, shouldBeDynamicallyRegistered);

	public static CallableWrapperMethod CreateMethod (MethodDefinition methodDefinition, RegisterAttribute register, string? managedParameters, string? outerType, IMetadataResolver cache, bool shouldBeDynamicallyRegistered = true)
	{
		var method = CreateMethod (register.Name, register.Signature, register.Connector, managedParameters, outerType, null);

		method.Annotations = JavaCallableWrapperGenerator.GetAnnotationsString ("\t", methodDefinition.CustomAttributes, cache);
		method.IsDynamicallyRegistered = shouldBeDynamicallyRegistered;

		return method;
	}

	public static CallableWrapperMethod CreateMethod (MethodDefinition methodDefinition, ExportAttribute export, string? managedParameters, IMetadataResolver resolver)
	{
		var method = CreateMethod (methodDefinition.Name, JavaNativeTypeManager.GetJniSignature (methodDefinition, resolver), "__export__", null, null, export.SuperArgumentsString);

		method.IsExport = true;
		method.IsStatic = methodDefinition.IsStatic;
		method.JavaAccess = JavaCallableWrapperGenerator.GetJavaAccess (methodDefinition.Attributes & MethodAttributes.MemberAccessMask);
		method.ThrownTypeNames = export.ThrownNames;
		method.JavaNameOverride = export.Name;
		method.ManagedParameters = managedParameters;
		method.Annotations = JavaCallableWrapperGenerator.GetAnnotationsString ("\t", methodDefinition.CustomAttributes, resolver);

		return method;
	}

	public static CallableWrapperMethod CreateMethod (MethodDefinition methodDefinition, IMetadataResolver resolver)
	{
		var method = CreateMethod (methodDefinition.Name, JavaNativeTypeManager.GetJniSignature (methodDefinition, resolver), "__export__", null, null, null);

		if (methodDefinition.HasParameters)
			Diagnostic.Error (4205, JavaCallableWrapperGenerator.LookupSource (methodDefinition), Localization.Resources.JavaCallableWrappers_XA4205);
		if (methodDefinition.ReturnType.MetadataType == MetadataType.Void)
			Diagnostic.Error (4208, JavaCallableWrapperGenerator.LookupSource (methodDefinition), Localization.Resources.JavaCallableWrappers_XA4208);

		method.IsExport = true;
		method.IsStatic = method.IsStatic;
		method.JavaAccess = JavaCallableWrapperGenerator.GetJavaAccess (methodDefinition.Attributes & MethodAttributes.MemberAccessMask);

		// Annotations are processed within CallableWrapperField, not the initializer method. So we don't generate them here.

		return method;
	}

	public static CallableWrapperMethod CreateMethod (string name, string? signature, string? connector, string? managedParameters, string? outerType, string? superCall)
	{
		signature = signature ?? throw new ArgumentNullException ("`connector` cannot be null.", nameof (connector));
		var method_name = "n_" + name + ":" + signature + ":" + connector;

		var method = new CallableWrapperMethod (name, method_name, signature) {
			ManagedParameters = managedParameters
		};

		var jnisig = signature;
		var closer = jnisig.IndexOf (')');
		var ret = jnisig.Substring (closer + 1);
		method.Retval = JavaNativeTypeManager.Parse (ret)?.Type;

		var jniparms = jnisig.Substring (1, closer - 1);

		if (string.IsNullOrEmpty (jniparms) && string.IsNullOrEmpty (superCall))
			return method;

		var parms = new StringBuilder ();
		var scall = new StringBuilder ();
		var acall = new StringBuilder ();
		var first = true;
		var i = 0;

		foreach (var jti in JavaNativeTypeManager.FromSignature (jniparms)) {
			if (outerType != null) {
				acall.Append (outerType).Append (".this");
				outerType = null;
				continue;
			}

			var parmType = jti.Type;

			if (!first) {
				parms.Append (", ");
				scall.Append (", ");
				acall.Append (", ");
			}

			first = false;
			parms.Append (parmType).Append (" p").Append (i);
			scall.Append ("p").Append (i);
			acall.Append ("p").Append (i);
			++i;
		}

		method.Params = parms.ToString ();
		method.SuperCall = superCall ?? scall.ToString ();
		method.ActivateCall = acall.ToString ();

		return method;
	}

	// Temporary conversion function
	public static CallableWrapperMethod CreateMethod (JavaCallableWrapperGenerator.Signature signature)
	{
		return new CallableWrapperMethod (signature.Name, signature.Method, signature.JniSignature) {
			ManagedParameters = signature.ManagedParameters,
			JavaNameOverride = signature.JavaNameOverride,
			Params = signature.Params,
			Retval = signature.Retval,
			ThrowsDeclaration = signature.ThrowsDeclaration,
			JavaAccess = signature.JavaAccess,
			IsExport = signature.IsExport,
			IsStatic = signature.IsStatic,
			IsDynamicallyRegistered = signature.IsDynamicallyRegistered,
			ThrownTypeNames = signature.ThrownTypeNames,
			Annotations = signature.Annotations,
			SuperCall = signature.SuperCall,
			ActivateCall = signature.ActivateCall,
		};
	}

	// Temporary conversion function
	public static CallableWrapperConstructor CreateConstructor (JavaCallableWrapperGenerator.Signature signature)
	{
		return new CallableWrapperConstructor (signature.Name, signature.Method, signature.JniSignature) {
			ManagedParameters = signature.ManagedParameters,
			JavaNameOverride = signature.JavaNameOverride,
			Params = signature.Params,
			Retval = signature.Retval,
			ThrowsDeclaration = signature.ThrowsDeclaration,
			JavaAccess = signature.JavaAccess,
			IsExport = signature.IsExport,
			IsStatic = signature.IsStatic,
			IsDynamicallyRegistered = signature.IsDynamicallyRegistered,
			ThrownTypeNames = signature.ThrownTypeNames,
			Annotations = signature.Annotations,
			SuperCall = signature.SuperCall,
			ActivateCall = signature.ActivateCall
		};
	}

	public static CallableWrapperApplicationConstructor? CreateApplicationConstructor (string name, TypeDefinition type, IMetadataResolver resolver)
	{
		if (!JavaNativeTypeManager.IsApplication (type, resolver))
			return null;

		return new CallableWrapperApplicationConstructor (name);
	}

	// Temporary conversion function
	public static CallableWrapperType CreateType (JavaCallableWrapperGenerator generator)
	{
		var type = new CallableWrapperType (generator.name, generator.package) {
			Type = generator.type,
			Cache = generator.cache,
			IsAbstract = generator.type.IsAbstract,
			ApplicationJavaClass = generator.ApplicationJavaClass,
			Generator = generator,
			HasDynamicallyRegisteredMethods = generator.HasDynamicallyRegisteredMethods,
			GenerateOnCreateOverrides = generator.GenerateOnCreateOverrides,
			MonoRuntimeInitialization = generator.MonoRuntimeInitialization,
		};

		if (generator.children is not null)
			foreach (var nested in generator.children)
				type.NestedTypes.Add (CreateType (nested));

		return type;
	}
}
