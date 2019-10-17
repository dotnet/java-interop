using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Android.Tools.Bytecode
{
	static class KotlinFixups
	{
		public static void Fixup (IList<ClassFile> classes)
		{
			foreach (var c in classes) {
				// See if this is a Kotlin class
				var attr = c.Attributes.OfType<RuntimeVisibleAnnotationsAttribute> ().FirstOrDefault ();
				var kotlin = attr?.Annotations.SingleOrDefault (a => a.Type == "Lkotlin/Metadata;");

				if (kotlin is null)
					continue;

				try {
					var km = KotlinMetadata.FromAnnotation (kotlin);

					if (km.Kind == KotlinMetadataKind.Class) {
						var class_metadata = km.AsClassMetadata ();

						FixupClassVisibility (c, class_metadata);

						if (class_metadata.Flags.HasFlag (KotlinClassFlags.EnumClass))
							continue;

						foreach (var con in class_metadata.Constructors)
							FixupConstructor (FindJavaConstructor (class_metadata, con, c), con);

						foreach (var met in class_metadata.Functions)
							FixupFunction (FindJavaMethod (class_metadata, met, c), met, class_metadata);

						foreach (var prop in class_metadata.Properties) {
							var getter = FindJavaPropertyGetter (class_metadata, prop, c);
							var setter = FindJavaPropertySetter (class_metadata, prop, c);

							FixupProperty (getter, setter, prop);
						}
					} else {
						// We don't have explicit support for other types of Kotlin constructs yet,
						// so they are unlikely to work. Mark them as private so they don't get bound.
						c.AccessFlags = ClassAccessFlags.Private;
					}
				} catch (Exception ex) {
					Log.Warning (0, $"class-parse: warning: Unable to parse Kotlin metadata on '{c.ThisClass.Name}': {ex}");
				}
			}
		}

		static void FixupClassVisibility (ClassFile klass, KotlinClass metadata)
		{
			// Hide class if it isn't Public/Protected
			if (klass.AccessFlags.IsPubliclyVisible () && !metadata.Flags.IsPubliclyVisible ()) {
				Log.Debug ($"Kotlin: Hiding internal class {klass.ThisClass.Name.Value}");
				klass.AccessFlags = ClassAccessFlags.Private;
				return;
			}

			// We don't have explicit support for these types of Kotlin constructs yet,
			// so they are unlikely to work. Mark them as private so they don't get bound.
			if (metadata.Flags.HasFlag (KotlinClassFlags.AnnotationClass) || metadata.Flags.HasFlag (KotlinClassFlags.CompanionObject) || metadata.Flags.HasFlag (KotlinClassFlags.Object)) {
				Log.Debug ($"Kotlin: Hiding unsupported class {klass.ThisClass.Name.Value} ({metadata.Flags})");
				klass.AccessFlags = ClassAccessFlags.Private;
			}

			foreach (var method in klass.Methods)
				if (method.Name.Contains ("-impl")) {
					Log.Debug ($"Kotlin: Hiding implementation method {method.DeclaringType?.ThisClass.Name.Value} - {method.Name}");
					method.AccessFlags = MethodAccessFlags.Private;
				} else if (method.Name.Contains ("-deprecated")) {
					Log.Debug ($"Kotlin: Hiding deprecated method {method.DeclaringType?.ThisClass.Name.Value} - {method.Name}");
					method.AccessFlags = MethodAccessFlags.Private;
				} else if (method.AccessFlags.HasFlag (MethodAccessFlags.Static) && method.GetParameters ().FirstOrDefault ()?.Name == "$this") {
					Log.Debug ($"Kotlin: Hiding extension method {method.DeclaringType?.ThisClass.Name.Value} - {method.Name}");
					method.AccessFlags = MethodAccessFlags.Private;
				}
		}

		static void FixupConstructor (MethodInfo method, KotlinConstructor metadata)
		{
			if (method is null)
				return;

			// Hide constructor if it isn't Public/Protected
			if (method.IsPubliclyVisible && !metadata.Flags.IsPubliclyVisible ()) {
				Log.Debug ($"Kotlin: Hiding internal constructor {method.DeclaringType?.ThisClass.Name.Value} - {metadata.GetSignature ()}");
				method.AccessFlags = MethodAccessFlags.Private;
			}

		}

		static void FixupFunction (MethodInfo method, KotlinFunction metadata, KotlinClass kotlinClass)
		{
			if (method is null)
				return;

			// Hide function if it isn't Public/Protected
			if (method.IsPubliclyVisible && !metadata.Flags.IsPubliclyVisible ()) {
				Log.Debug ($"Kotlin: Hiding internal method {method.DeclaringType?.ThisClass.Name.Value} - {metadata.GetSignature ()}");
				method.AccessFlags = MethodAccessFlags.Private;
				return;
			}

			var java_parameters = method.GetFilteredParameters ();

			for (var i = 0; i < java_parameters.Length; i++) {
				var java_p = java_parameters [i];
				var kotlin_p = metadata.ValueParameters [i];

				if (TypesMatch (java_p.Type, kotlin_p.Type, kotlinClass) && java_p.IsUnnamedParameter () && !kotlin_p.IsUnnamedParameter ()) {
					Log.Debug ($"Kotlin: Renaming parameter {method.DeclaringType?.ThisClass.Name.Value} - {method.Name} - {java_p.Name} -> {kotlin_p.Name}");
					java_p.Name = kotlin_p.Name;
				}
			}
		}

		static void FixupProperty (MethodInfo getter, MethodInfo setter, KotlinProperty metadata)
		{
			if (getter is null && setter is null)
				return;

			// Hide property if it isn't Public/Protected
			if (!metadata.Flags.IsPubliclyVisible ()) {

				if (getter?.IsPubliclyVisible == true) {
					Log.Debug ($"Kotlin: Hiding internal getter method {getter.DeclaringType?.ThisClass.Name.Value} - {getter.Name}");
					getter.AccessFlags = MethodAccessFlags.Private;
				}

				if (setter?.IsPubliclyVisible == true) {
					Log.Debug ($"Kotlin: Hiding internal setter method {setter.DeclaringType?.ThisClass.Name.Value} - {setter.Name}");
					setter.AccessFlags = MethodAccessFlags.Private;
				}

				return;
			}

			if (setter != null) {
				var setter_parameter = setter.GetParameters () [0];

				if (setter_parameter.IsUnnamedParameter () && !metadata.SetterValueParameter.IsUnnamedParameter ()) {
					Log.Debug ($"Kotlin: Renaming setter parameter {setter.DeclaringType?.ThisClass.Name.Value} - {setter.Name} - {setter_parameter.Name} -> {metadata.SetterValueParameter.Name}");
					setter_parameter.Name = metadata.SetterValueParameter.Name;
				}
			}
		}

		static MethodInfo FindJavaConstructor (KotlinClass kotlinClass, KotlinConstructor constructor, ClassFile klass)
		{
			var all_constructors = klass.Methods.Where (method => method.Name == "<init>" || method.Name == "<clinit>");
			var possible_constructors = all_constructors.Where (method => method.GetFilteredParameters ().Length == constructor.ValueParameters.Count);

			foreach (var method in possible_constructors) {
				if (ParametersMatch (kotlinClass, method, constructor.ValueParameters))
					return method;
			}

			return null;
		}

		static MethodInfo FindJavaMethod (KotlinClass kotlinClass, KotlinFunction function, ClassFile klass)
		{
			var possible_methods = klass.Methods.Where (method => method.GetMethodNameWithoutSuffix () == function.Name &&
									      method.GetFilteredParameters ().Length == function.ValueParameters.Count);

			foreach (var method in possible_methods) {
				if (!TypesMatch (method.ReturnType, function.ReturnType, kotlinClass))
					continue;

				if (!ParametersMatch (kotlinClass, method, function.ValueParameters))
					continue;

				return method;
			}

			return null;
		}

		static MethodInfo FindJavaPropertyGetter (KotlinClass kotlinClass, KotlinProperty property, ClassFile klass)
		{
			var possible_methods = klass.Methods.Where (method => (string.Compare (method.GetMethodNameWithoutSuffix (), $"get{property.Name}", true) == 0 ||
									       string.Compare (method.GetMethodNameWithoutSuffix (), property.Name, true) == 0) &&
									      method.GetParameters ().Length == 0 &&
									      TypesMatch (method.ReturnType, property.ReturnType, kotlinClass));

			return possible_methods.FirstOrDefault ();
		}

		static MethodInfo FindJavaPropertySetter (KotlinClass kotlinClass, KotlinProperty property, ClassFile klass)
		{
			var possible_methods = klass.Methods.Where (method => string.Compare (method.GetMethodNameWithoutSuffix (), $"set{property.Name}", true) == 0 &&
									      property.SetterValueParameter != null &&
									      method.GetParameters ().Length == 1 &&
									      TypesMatch (method.GetParameters () [0].Type, property.SetterValueParameter.Type, kotlinClass));

			return possible_methods.FirstOrDefault ();
		}

		static bool ParametersMatch (KotlinClass kotlinClass, MethodInfo method, List<KotlinValueParameter> kotlinParameters)
		{
			var java_parameters = method.GetFilteredParameters ();

			if (java_parameters.Length == 0 && kotlinParameters.Count == 0)
				return true;

			for (var i = 0; i < java_parameters.Length; i++) {
				var java_p = java_parameters [i];
				var kotlin_p = kotlinParameters [i];

				if (!TypesMatch (java_p.Type, kotlin_p.Type, kotlinClass))
					return false;
			}

			return true;
		}

		static bool TypesMatch (TypeInfo javaType, KotlinType kotlinType, KotlinClass kotlinClass)
		{
			// Generic type
			if (!string.IsNullOrWhiteSpace (kotlinType.TypeParameterName) && $"T{kotlinType.TypeParameterName};" == javaType.TypeSignature)
				return true;

			if (javaType.BinaryName == KotlinUtilities.ConvertKotlinTypeSignature (kotlinType, kotlinClass))
				return true;

			// Could be a generic type erasure
			if (javaType.BinaryName == "Ljava/lang/Object;")
				return true;

			// Sometimes Kotlin keeps its native types rather than converting them to Java native types
			// ie: "Lkotlin/UShort;" instead of "S"
			if (javaType.BinaryName.StartsWith ("L", StringComparison.Ordinal) && javaType.BinaryName.EndsWith (";", StringComparison.Ordinal)) {
				if (KotlinUtilities.ConvertKotlinClassToJava (javaType.BinaryName.Substring (1, javaType.BinaryName.Length - 2)) == KotlinUtilities.ConvertKotlinTypeSignature (kotlinType, kotlinClass))
					return true;
			}

			// Same for some arrays
			if (javaType.BinaryName.StartsWith ("[L", StringComparison.Ordinal) && javaType.BinaryName.EndsWith (";", StringComparison.Ordinal)) {
				if ("[" + KotlinUtilities.ConvertKotlinClassToJava (javaType.BinaryName.Substring (2, javaType.BinaryName.Length - 3)) == KotlinUtilities.ConvertKotlinTypeSignature (kotlinType, kotlinClass))
					return true;
			}

			return false;
		}
	}
}
