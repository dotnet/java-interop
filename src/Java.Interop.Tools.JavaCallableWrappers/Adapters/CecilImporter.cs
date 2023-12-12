using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Runtime;
using Java.Interop.Tools.Cecil;
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

		return new CallableWrapperField (fieldName, type_name, visibility, method.Name) {
			IsStatic = method.IsStatic,
			Annotations = annotations
		};
	}

	// Temporary conversion function
	public static CallableWrapperField CreateField (JavaCallableWrapperGenerator.JavaFieldInfo field)
	{
		return new CallableWrapperField (field.FieldName, field.TypeName, field.GetJavaAccess (), field.InitializerName) {
			IsStatic = field.IsStatic,
			Annotations = field.Annotations
		};
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
		var partial_assembly_qualified_name = generator.type.GetPartialAssemblyQualifiedName (generator.cache);

		var type = new CallableWrapperType (generator.name, generator.package, partial_assembly_qualified_name) {
			IsAbstract = generator.type.IsAbstract,
			ApplicationJavaClass = generator.ApplicationJavaClass,
			HasDynamicallyRegisteredMethods = generator.HasDynamicallyRegisteredMethods,
			GenerateOnCreateOverrides = generator.GenerateOnCreateOverrides,
			MonoRuntimeInitialization = generator.MonoRuntimeInitialization,
			IsApplication = JavaNativeTypeManager.IsApplication (generator.type, generator.cache),
			IsInstrumentation = JavaNativeTypeManager.IsInstrumentation (generator.type, generator.cache),
		};

		type.Annotations.AddRange (CreateTypeAnnotations (generator.type, generator.cache));

		// Extends
		var extendsType = GetJavaTypeName (generator.type.BaseType, generator.cache);

		if (extendsType == "android.app.Application" && generator.ApplicationJavaClass != null && !string.IsNullOrEmpty (generator.ApplicationJavaClass))
			extendsType = generator.ApplicationJavaClass;

		type.ExtendsType = extendsType;

		// Implemented interfaces
		foreach (var ifaceInfo in generator.type.Interfaces) {
			var iface = generator.cache.Resolve (ifaceInfo.InterfaceType);

			if (!JavaCallableWrapperGenerator.GetTypeRegistrationAttributes (iface).Any ())
				continue;

			type.ImplementedInterfaces.Add (GetJavaTypeName (iface, generator.cache));
		}

		// Type constructors
		foreach (var ctor in generator.ctors) {
			if (string.IsNullOrEmpty (ctor.Params) && type.IsApplication)
				continue;

			var ct = CreateConstructor (ctor);

			ct.Name = type.Name;
			ct.CannotRegisterInStaticConstructor = type.CannotRegisterInStaticConstructor;
			ct.PartialAssemblyQualifiedName = type.PartialAssemblyQualifiedName ;

			type.Constructors.Add (ct);
		}

		// Application constructor
		if (CreateApplicationConstructor (type.Name, generator.type, generator.cache) is CallableWrapperApplicationConstructor app_ctor)
			type.ApplicationConstructor = app_ctor;

		// Exported fields
		foreach (var field in generator.exported_fields)
			type.Fields.Add (CreateField (field));

		// Methods
		foreach (var method in generator.methods)
			type.Methods.Add (CreateMethod (method));

		// Nested types
		if (generator.children is not null)
			foreach (var nested in generator.children)
				type.NestedTypes.Add (CreateType (nested));

		return type;
	}

	static string GetJavaTypeName (TypeReference r, IMetadataResolver cache)
	{
		TypeDefinition d = cache.Resolve (r);
		string? jniName = JavaNativeTypeManager.ToJniName (d, cache);
		if (jniName == null) {
			Diagnostic.Error (4201, Localization.Resources.JavaCallableWrappers_XA4201, r.FullName);
			throw new InvalidOperationException ("--nrt:jniName-- Should not be reached");
		}
		return jniName.Replace ('/', '.').Replace ('$', '.');
	}

	public static IEnumerable<CallableWrapperTypeAnnotation> CreateTypeAnnotations (TypeDefinition type, IMetadataResolver resolver)
	{
		foreach (var ca in type.CustomAttributes) {
			var annotation = CreateTypeAnnotation (ca, resolver);

			if (annotation is not null)
				yield return annotation;
		}
	}

	public static CallableWrapperTypeAnnotation? CreateTypeAnnotation (CustomAttribute ca, IMetadataResolver resolver)
	{
		var catype = resolver.Resolve (ca.AttributeType);
		var tca = catype.CustomAttributes.FirstOrDefault (a => a.AttributeType.FullName == "Android.Runtime.AnnotationAttribute");

		if (tca is null)
			return null;

		var name_object = tca.ConstructorArguments [0].Value;

		// Should never be hit
		if (name_object is not string name)
			throw new ArgumentException ($"Expected a string for the first argument of the {nameof (RegisterAttribute)} constructor.", nameof (ca));

		var annotation = new CallableWrapperTypeAnnotation (name);

		foreach (var p in ca.Properties) {
			var pd = catype.Properties.FirstOrDefault (pp => pp.Name == p.Name);
			var reg = pd != null ? pd.CustomAttributes.FirstOrDefault (pdca => pdca.AttributeType.FullName == "Android.Runtime.RegisterAttribute") : null;

			var key = reg != null ? (string) reg.ConstructorArguments [0].Value : p.Name;
			var value = ManagedValueToJavaSource (p.Argument.Value);

			annotation.Properties.Add (new System.Collections.Generic.KeyValuePair<string, string> (key, value));
		}

		return annotation;
	}

	// FIXME: this is hacky. Is there any existing code for value to source conversion?
	static string ManagedValueToJavaSource (object value)
	{
		if (value is string)
			return "\"" + value.ToString ().Replace ("\"", "\"\"") + '"';
		else if (value.GetType ().FullName == "Java.Lang.Class")
			return value.ToString () + ".class";
		else if (value is bool)
			return ((bool) value) ? "true" : "false";
		else
			return value.ToString ();
	}
}
