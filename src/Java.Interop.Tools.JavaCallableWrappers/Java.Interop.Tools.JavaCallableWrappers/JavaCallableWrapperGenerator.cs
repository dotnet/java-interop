using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Android.Runtime;

using Java.Interop.Tools.Cecil;
using Java.Interop.Tools.Diagnostics;
using Java.Interop.Tools.TypeNameMappings;

using MethodAttributes = Mono.Cecil.MethodAttributes;
using static Java.Interop.Tools.TypeNameMappings.JavaNativeTypeManager;
using Java.Interop.Tools.JavaCallableWrappers.CallableWrapperMembers;
using Java.Interop.Tools.JavaCallableWrappers.Adapters;

namespace Java.Interop.Tools.JavaCallableWrappers {

	public partial class JavaCallableWrapperGenerator {

		Action<string, object[]> log;
		internal string name;
		internal string package;
		internal TypeDefinition type;
		internal List<JavaFieldInfo> exported_fields = new List<JavaFieldInfo> ();
		internal List<Signature> methods = new List<Signature> ();
		internal List<Signature> ctors   = new List<Signature> ();
		internal List<JavaCallableWrapperGenerator>? children;

		internal readonly IMetadataResolver cache;
		readonly JavaCallableMethodClassifier? methodClassifier;

		[Obsolete ("Use the TypeDefinitionCache overload for better performance.", error: true)]
		public JavaCallableWrapperGenerator (TypeDefinition type, Action<string, object []> log) => throw new NotSupportedException ();

		public JavaCallableWrapperGenerator (TypeDefinition type, Action<string, object[]> log, TypeDefinitionCache cache)
			: this (type, log, (IMetadataResolver) cache, methodClassifier: null)
		{ }

		public JavaCallableWrapperGenerator (TypeDefinition type, Action<string, object[]> log, TypeDefinitionCache cache, JavaCallableMethodClassifier? methodClassifier)
			: this (type, log, (IMetadataResolver) cache, methodClassifier)
		{
		}

		public JavaCallableWrapperGenerator (TypeDefinition type, Action<string, object[]> log, IMetadataResolver resolver)
			: this (type, log, resolver, methodClassifier: null)
		{ }

		public JavaCallableWrapperGenerator (TypeDefinition type, Action<string, object[]> log, IMetadataResolver resolver, JavaCallableMethodClassifier? methodClassifier)
			: this (type, null, log, resolver, methodClassifier)
		{
			AddNestedTypes (type);
		}

		public  string?         ApplicationJavaClass            { get; set; }
		public  JavaPeerStyle   CodeGenerationTarget            { get; set; }

		public bool GenerateOnCreateOverrides { get; set; }

		public bool HasExport { get; private set; }

		// If there are no methods, we need to generate "empty" registration because of backward compatibility
		public bool HasDynamicallyRegisteredMethods => methods.Count == 0 || methods.Any ((Signature sig) => sig.IsDynamicallyRegistered);

		/// <summary>
		/// The Java source code to be included in Instrumentation.onCreate
		///
		/// Originally came from MonoRuntimeProvider.java delimited by:
		/// // Mono Runtime Initialization {{{
		/// // }}}
		/// </summary>
		public string? MonoRuntimeInitialization { get; set; }

		public string Name {
			get { return name; }
		}

		void AddNestedTypes (TypeDefinition type)
		{
			if (!type.HasNestedTypes) {
				return;
			}
			children = children ?? new List<JavaCallableWrapperGenerator> ();
			foreach (TypeDefinition nt in type.NestedTypes) {
				if (!nt.HasJavaPeer (cache))
					continue;
				if (!JavaNativeTypeManager.IsNonStaticInnerClass (nt, cache))
					continue;
				children.Add (new JavaCallableWrapperGenerator (nt, JavaNativeTypeManager.ToJniName (type, cache), log, cache));
				AddNestedTypes (nt);
			}
			HasExport |= children.Any (t => t.HasExport);
		}

		JavaCallableWrapperGenerator (TypeDefinition type, string? outerType, Action<string, object[]> log, IMetadataResolver resolver, JavaCallableMethodClassifier? methodClassifier = null)
		{
			this.methodClassifier = methodClassifier;
			this.type = type;
			this.log = log;
			this.cache = resolver ?? new TypeDefinitionCache ();

			if (type.IsEnum || type.IsInterface || type.IsValueType)
				Diagnostic.Error (4200, LookupSource (type), Localization.Resources.JavaCallableWrappers_XA4200, type.FullName);

			string jniName = JavaNativeTypeManager.ToJniName (type, this.cache);
			if (jniName == null) {
				Diagnostic.Error (4201, LookupSource (type), Localization.Resources.JavaCallableWrappers_XA4201, type.FullName);
				throw new InvalidOperationException ("--nrt:jniName-- Should not be reached");
			}
			if (outerType != null && !string.IsNullOrEmpty (outerType)) {
				string p;
				jniName = jniName.Substring (outerType.Length + 1);
				ExtractJavaNames (outerType, out p, out outerType);
			}
			ExtractJavaNames (jniName, out package, out name);
			if (string.IsNullOrEmpty (package) &&
					(type.IsSubclassOf ("Android.App.Activity", cache) ||
					 type.IsSubclassOf ("Android.App.Application", cache) ||
					 type.IsSubclassOf ("Android.App.Service", cache) ||
					 type.IsSubclassOf ("Android.Content.BroadcastReceiver", cache) ||
					 type.IsSubclassOf ("Android.Content.ContentProvider", cache)))
				Diagnostic.Error (4203, LookupSource (type), Localization.Resources.JavaCallableWrappers_XA4203, jniName);

			foreach (MethodDefinition minfo in type.Methods.Where (m => !m.IsConstructor)) {
				var baseRegisteredMethod = GetBaseRegisteredMethod (minfo);
				if (baseRegisteredMethod != null)
					AddMethod (baseRegisteredMethod, minfo);
				else if (minfo.AnyCustomAttributes ("Java.Interop.JavaCallableAttribute")) {
					AddMethod (null, minfo);
					HasExport = true;
				} else if (minfo.AnyCustomAttributes ("Java.Interop.JavaCallableConstructorAttribute")) {
					AddMethod (null, minfo);
					HasExport = true;
				} else if (minfo.AnyCustomAttributes (typeof(ExportFieldAttribute))) {
					AddMethod (null, minfo);
					HasExport = true;
				} else if (minfo.AnyCustomAttributes (typeof (ExportAttribute))) {
					AddMethod (null, minfo);
					HasExport = true;
				}
			}

			foreach (InterfaceImplementation ifaceInfo in type.Interfaces) {
				var typeReference = ifaceInfo.InterfaceType;
				var typeDefinition = cache.Resolve (typeReference);
				if (typeDefinition == null) {
					Diagnostic.Error (4204,
						LookupSource (type),
						Localization.Resources.JavaCallableWrappers_XA4204,
						typeReference.FullName);
					continue;
				}
				if (!GetTypeRegistrationAttributes (typeDefinition).Any ())
					continue;
				foreach (MethodDefinition imethod in typeDefinition.Methods) {
					if (imethod.IsStatic)
						continue;
					AddMethod (imethod, imethod);
				}
			}

			var ctorTypes = new List<TypeDefinition> () {
				type,
			};
			foreach (var bt in type.GetBaseTypes (cache)) {
				ctorTypes.Add (bt);
				RegisterAttribute rattr = GetMethodRegistrationAttributes (bt).FirstOrDefault ();
				if (rattr != null && rattr.DoNotGenerateAcw)
					break;
			}
			ctorTypes.Reverse ();

			var curCtors = new List<MethodDefinition> ();

			foreach (MethodDefinition minfo in type.Methods) {
				if (minfo.IsConstructor && minfo.AnyCustomAttributes (typeof (ExportAttribute))) {
					if (minfo.IsStatic) {
						// Diagnostic.Warning (log, "ExportAttribute does not work on static constructor");
					}
					else {
						AddConstructor (minfo, ctorTypes [0], outerType, null, curCtors, false, true);
						HasExport = true;
					}
				}
			}

			AddConstructors (ctorTypes [0], outerType, null, curCtors, true);

			for (int i = 1; i < ctorTypes.Count; ++i) {
				var baseCtors = curCtors;
				curCtors      = new List<MethodDefinition> ();
				AddConstructors (ctorTypes [i], outerType, baseCtors, curCtors, false);
			}
		}

		static void ExtractJavaNames (string jniName, out string package, out string type)
		{
			int i = jniName.LastIndexOf ('/');
			if (i < 0) {
				type    = jniName;
				package = string.Empty;
			}
			else {
				type    = jniName.Substring (i+1);
				package = jniName.Substring (0, i).Replace ('/', '.');
			}
		}

		internal static SequencePoint? LookupSource (MethodDefinition method)
		{
			if (!method.HasBody)
				return null;

			foreach (var ins in method.Body.Instructions) {
				var seqPoint = method.DebugInformation.GetSequencePoint (ins);
				if (seqPoint != null)
					return seqPoint;
			}

			return null;
		}

		static SequencePoint? LookupSource (TypeDefinition type)
		{
			SequencePoint? candidate = null;
			foreach (var method in type.Methods) {
				if (!method.HasBody)
					continue;

				foreach (var ins in method.Body.Instructions) {
					var seq = method.DebugInformation.GetSequencePoint (ins);
					if (seq == null)
						continue;

					if (Regex.IsMatch (seq.Document.Url, ".+\\.(g|designer)\\..+"))
						break;
					if (candidate == null || seq.StartLine < candidate.StartLine)
						candidate = seq;
					break;
				}
			}

			return candidate;
		}

		void AddConstructors (TypeDefinition type, string? outerType, List<MethodDefinition>? baseCtors, List<MethodDefinition> curCtors, bool onlyRegisteredOrExportedCtors)
		{
			foreach (MethodDefinition ctor in type.Methods)
				if (ctor.IsConstructor && !ctor.IsStatic && !ctor.AnyCustomAttributes (typeof (ExportAttribute)))
					AddConstructor (ctor, type, outerType, baseCtors, curCtors, onlyRegisteredOrExportedCtors, false);
		}

		void AddConstructor (MethodDefinition ctor, TypeDefinition type, string? outerType, List<MethodDefinition>? baseCtors, List<MethodDefinition> curCtors, bool onlyRegisteredOrExportedCtors, bool skipParameterCheck)
		{
				string managedParameters = GetManagedParameters (ctor, outerType);
				if (!skipParameterCheck && (managedParameters == null || ctors.Any (c => c.ManagedParameters == managedParameters))) {
					return;
				}

				ExportAttribute eattr = GetExportAttributes (ctor).FirstOrDefault ();
				if (eattr != null) {
					if (!string.IsNullOrEmpty (eattr.Name)) {
						// Diagnostic.Warning (log, "Use of ExportAttribute.Name property is invalid on constructors");
					}
					ctors.Add (new Signature (ctor, eattr, managedParameters, cache));
					curCtors.Add (ctor);
					return;
				}

				RegisterAttribute rattr = GetMethodRegistrationAttributes (ctor).FirstOrDefault ();
				if (rattr != null) {
					if (ctors.Any (c => c.JniSignature == rattr.Signature))
						return;
					ctors.Add (new Signature (ctor, rattr, managedParameters, outerType, cache));
					curCtors.Add (ctor);
					return;
				}

				if (onlyRegisteredOrExportedCtors)
					return;

				string? jniSignature = GetJniSignature (ctor, cache);

				if (jniSignature == null)
					return;

				if (ctors.Any (c => c.JniSignature == jniSignature))
					return;

				if (baseCtors == null) {
					throw new InvalidOperationException ("`baseCtors` should not be null!");
				}

				if (baseCtors.Any (m => m.Parameters.AreParametersCompatibleWith (ctor.Parameters, cache))) {
					ctors.Add (new Signature (".ctor", jniSignature, "", managedParameters, outerType, null));
					curCtors.Add (ctor);
					return;
				}
				if (baseCtors.Any (m => !m.HasParameters)) {
					ctors.Add (new Signature (".ctor", jniSignature, "", managedParameters, outerType, ""));
					curCtors.Add (ctor);
					return;
				}
		}

		MethodDefinition? GetBaseRegisteredMethod (MethodDefinition method)
		{
			MethodDefinition bmethod;
			while ((bmethod = method.GetBaseDefinition (cache)) != method) {
				method = bmethod;

				if (method.AnyCustomAttributes (typeof (RegisterAttribute))) {
					return method;
				}
			}
			return null;
		}

		internal static RegisterAttribute? ToRegisterAttribute (CustomAttribute attr)
		{
			// attr.Resolve ();
			RegisterAttribute? r = null;
			if (attr.ConstructorArguments.Count == 1)
				r = new RegisterAttribute ((string) attr.ConstructorArguments [0].Value, attr);
			else if (attr.ConstructorArguments.Count == 3)
				r = new RegisterAttribute (
						(string) attr.ConstructorArguments [0].Value,
						(string) attr.ConstructorArguments [1].Value,
						(string) attr.ConstructorArguments [2].Value,
						attr);
			if (r != null) {
				var v = attr.Properties.FirstOrDefault (p => p.Name == "DoNotGenerateAcw");
				r.DoNotGenerateAcw = v.Name == null ? false : (bool) v.Argument.Value;
			}
			return r;
		}

		internal static RegisterAttribute? RegisterFromJniTypeSignatureAttribute (CustomAttribute attr)
		{
			// attr.Resolve ();
			RegisterAttribute? r = null;
			if (attr.ConstructorArguments.Count == 1)
				r = new RegisterAttribute ((string) attr.ConstructorArguments [0].Value, attr);
			if (r != null) {
				var v = attr.Properties.FirstOrDefault (p => p.Name == "GenerateJavaPeer");
				if (v.Name == null) {
					r.DoNotGenerateAcw = false;
				} else if (v.Name == "GenerateJavaPeer") {
					r.DoNotGenerateAcw = ! (bool) v.Argument.Value;
				}
				var isKeyProp   = attr.Properties.FirstOrDefault (p => p.Name == "IsKeyword");
				var isKeyword	= isKeyProp.Name != null && ((bool) isKeyProp.Argument.Value) == true;
				var arrRankProp = attr.Properties.FirstOrDefault (p => p.Name == "ArrayRank");
				if (arrRankProp.Name != null && arrRankProp.Argument.Value is int rank) {
					r.Name = new string ('[', rank) + (isKeyword ? r.Name : "L" + r.Name + ";");
				}
			}
			return r;
		}

		internal static RegisterAttribute? RegisterFromJniConstructorSignatureAttribute (CustomAttribute attr)
		{
			// attr.Resolve ();
			RegisterAttribute? r = null;
			if (attr.ConstructorArguments.Count == 1)
				r = new RegisterAttribute (
					name:             ".ctor",
					signature:        (string) attr.ConstructorArguments [0].Value,
					connector:        "",
					originAttribute:  attr);
			return r;
		}

		internal static RegisterAttribute? RegisterFromJniMethodSignatureAttribute (CustomAttribute attr)
		{
			// attr.Resolve ();
			RegisterAttribute? r = null;
			if (attr.ConstructorArguments.Count == 2)
				r = new RegisterAttribute ((string) attr.ConstructorArguments [0].Value,
					(string) attr.ConstructorArguments [1].Value,
				        "",
				        attr);
			return r;
		}

		ExportAttribute ToExportAttribute (CustomAttribute attr, IMemberDefinition declaringMember)
		{
			var name = attr.ConstructorArguments.Count > 0 ? (string) attr.ConstructorArguments [0].Value : declaringMember.Name;
			if (attr.Properties.Count == 0)
				return new ExportAttribute (name);
			var typeArgs = (CustomAttributeArgument []) attr.Properties.FirstOrDefault (p => p.Name == "Throws").Argument.Value;
			var thrown = typeArgs != null && typeArgs.Any ()
				? (from caa in typeArgs select JavaNativeTypeManager.Parse (GetJniTypeName ((TypeReference)caa.Value, cache))?.Type)
					.Where (v => v != null)
					.ToArray ()
				: null;
			var superArgs = (string) attr.Properties.FirstOrDefault (p => p.Name == "SuperArgumentsString").Argument.Value;
			return new ExportAttribute (name) {ThrownNames = thrown, SuperArgumentsString = superArgs};
		}

		ExportAttribute ToExportAttributeFromJavaCallableAttribute (CustomAttribute attr, IMemberDefinition declaringMember)
		{
			var name = attr.ConstructorArguments.Count > 0
				? (string) attr.ConstructorArguments [0].Value
				: declaringMember.Name;
			return new ExportAttribute (name);
		}

		ExportAttribute ToExportAttributeFromJavaCallableConstructorAttribute (CustomAttribute attr, IMemberDefinition declaringMember)
		{
			var superArgs = (string) attr.Properties
				.FirstOrDefault (p => p.Name == "SuperConstructorExpression")
				.Argument
				.Value;
			return new ExportAttribute (".ctor") {
				SuperArgumentsString = superArgs,
			};
		}

		internal static ExportFieldAttribute ToExportFieldAttribute (CustomAttribute attr)
		{
			return new ExportFieldAttribute ((string) attr.ConstructorArguments [0].Value);
		}

		internal static IEnumerable<RegisterAttribute> GetTypeRegistrationAttributes (Mono.Cecil.ICustomAttributeProvider p)
		{
			foreach (var a in GetAttributes<RegisterAttribute> (p, a => ToRegisterAttribute (a))) {
				yield return a;
			}
			foreach (var c in p.GetCustomAttributes ("Java.Interop.JniTypeSignatureAttribute")) {
				var r = RegisterFromJniTypeSignatureAttribute (c);
				if (r == null) {
					continue;
				}
				yield return r;
			}
		}

		static IEnumerable<RegisterAttribute> GetMethodRegistrationAttributes (Mono.Cecil.ICustomAttributeProvider p)
		{
			foreach (var a in GetAttributes<RegisterAttribute> (p, a => ToRegisterAttribute (a))) {
				yield return a;
			}
			foreach (var c in p.GetCustomAttributes ("Java.Interop.JniConstructorSignatureAttribute")) {
				var r = RegisterFromJniConstructorSignatureAttribute (c);
				if (r == null) {
					continue;
				}
				yield return r;
			}
			foreach (var c in p.GetCustomAttributes ("Java.Interop.JniMethodSignatureAttribute")) {
				var r = RegisterFromJniMethodSignatureAttribute (c);
				if (r == null) {
					continue;
				}
				yield return r;
			}
		}

		IEnumerable<ExportAttribute> GetExportAttributes (IMemberDefinition p)
		{
			return GetAttributes<ExportAttribute> (p, a => ToExportAttribute (a, p))
				.Concat (GetAttributes<ExportAttribute> (p, "Java.Interop.JavaCallableAttribute",
					a => ToExportAttributeFromJavaCallableAttribute (a, p)))
				.Concat (GetAttributes<ExportAttribute> (p, "Java.Interop.JavaCallableConstructorAttribute",
					a => ToExportAttributeFromJavaCallableConstructorAttribute (a, p)));
		}

		static IEnumerable<ExportFieldAttribute> GetExportFieldAttributes (Mono.Cecil.ICustomAttributeProvider p)
		{
			return GetAttributes<ExportFieldAttribute> (p, a => ToExportFieldAttribute (a));
		}

		static IEnumerable<TAttribute> GetAttributes<TAttribute> (Mono.Cecil.ICustomAttributeProvider p, Func<CustomAttribute, TAttribute?> selector)
			where TAttribute : class
		{
			return GetAttributes (p, typeof (TAttribute).FullName, selector);
		}

		static IEnumerable<TAttribute> GetAttributes<TAttribute> (Mono.Cecil.ICustomAttributeProvider p, string attributeName, Func<CustomAttribute, TAttribute?> selector)
			where TAttribute : class
		{
			return p.GetCustomAttributes (attributeName)
				.Select (selector)
				.Where (v => v != null)
				.Select (v => v!);
		}

		void AddMethod (MethodDefinition? registeredMethod, MethodDefinition implementedMethod)
		{
			if (registeredMethod != null)
				foreach (RegisterAttribute attr in GetMethodRegistrationAttributes (registeredMethod)) {
					// Check for Kotlin-mangled methods that cannot be overridden
					if (attr.Name.Contains ("-impl") || (attr.Name.Length > 7 && attr.Name[attr.Name.Length - 8] == '-'))
						Diagnostic.Error (4217, LookupSource (implementedMethod), Localization.Resources.JavaCallableWrappers_XA4217, attr.Name);

					bool shouldBeDynamicallyRegistered = methodClassifier?.ShouldBeDynamicallyRegistered (type, registeredMethod, implementedMethod, attr.OriginAttribute) ?? true;
					var msig = new Signature (implementedMethod, attr, cache, shouldBeDynamicallyRegistered);
					if (!registeredMethod.IsConstructor && !methods.Any (m => m.Name == msig.Name && m.Params == msig.Params))
						methods.Add (msig);
				}
			foreach (ExportAttribute attr in GetExportAttributes (implementedMethod)) {
				if (type.HasGenericParameters)
					Diagnostic.Error (4206, LookupSource (implementedMethod), Localization.Resources.JavaCallableWrappers_XA4206);

				var msig = new Signature (implementedMethod, attr, managedParameters: null, cache: cache);
				if (!string.IsNullOrEmpty (attr.SuperArgumentsString)) {
					// Diagnostic.Warning (log, "Use of ExportAttribute.SuperArgumentsString property is invalid on methods");
				}
				if (!implementedMethod.IsConstructor && !methods.Any (m => m.Name == msig.Name && m.Params == msig.Params))
					methods.Add (msig);
			}
			foreach (ExportFieldAttribute attr in GetExportFieldAttributes (implementedMethod)) {
				if (type.HasGenericParameters)
					Diagnostic.Error (4207, LookupSource (implementedMethod), Localization.Resources.JavaCallableWrappers_XA4207);

				var msig = new Signature (implementedMethod, attr, cache);
				if (!implementedMethod.IsConstructor && !methods.Any (m => m.Name == msig.Name && m.Params == msig.Params)) {
					methods.Add (msig);
					exported_fields.Add (new JavaFieldInfo (implementedMethod, attr.Name, cache));
				}
			}
		}

		string GetManagedParameters (MethodDefinition ctor, string? outerType)
		{
			StringBuilder sb = new StringBuilder ();
			foreach (ParameterDefinition pdef in ctor.Parameters) {
				if (sb.Length > 0)
					sb.Append (':');
				if (outerType != null && sb.Length == 0)
					sb.Append (type.DeclaringType.GetPartialAssemblyQualifiedName (cache));
				else
					sb.Append (pdef.ParameterType.GetPartialAssemblyQualifiedName (cache));
			}
			return sb.ToString ();
		}

		public void Generate (string outputPath)
		{
			var options = new CallableWrapperWriterOptions { CodeGenerationTarget = CodeGenerationTarget };
			var jcw_writer = CreateWriter ();
			jcw_writer.Generate (outputPath, options);
		}

		public void Generate (TextWriter writer)
		{
			var options = new CallableWrapperWriterOptions { CodeGenerationTarget = CodeGenerationTarget };
			var jcw_writer = CreateWriter ();
			jcw_writer.Generate (writer, options, false);
		}

		internal CallableWrapperType CreateWriter ()
		{
			return CecilImporter.CreateType (this);
		}

		internal static string GetAnnotationsString (string indent, IEnumerable<CustomAttribute> atts, IMetadataResolver resolver)
		{
			var sw = new StringWriter ();
			WriteAnnotations (indent, sw, atts, resolver);
			return sw.ToString ();
		}

		internal static void WriteAnnotations (string indent, TextWriter sw, IEnumerable<CustomAttribute> atts, IMetadataResolver resolver)
		{
			foreach (var ca in atts) {
				var catype = resolver.Resolve (ca.AttributeType);
				var tca = catype.CustomAttributes.FirstOrDefault (a => a.AttributeType.FullName == "Android.Runtime.AnnotationAttribute");
				if (tca != null) {
					sw.Write (indent);
					sw.Write ('@');
					sw.Write (tca.ConstructorArguments [0].Value);
					if (ca.Properties.Count > 0) {
						sw.WriteLine ("(");
						bool wrote = false;
						foreach (var p in ca.Properties) {
							if (wrote)
								sw.WriteLine (',');
							var pd = catype.Properties.FirstOrDefault (pp => pp.Name == p.Name);
							var reg = pd != null ? pd.CustomAttributes.FirstOrDefault (pdca => pdca.AttributeType.FullName == "Android.Runtime.RegisterAttribute") : null;
							sw.Write (reg != null ? reg.ConstructorArguments [0].Value : p.Name);
							sw.Write (" = ");
							sw.Write (ManagedValueToJavaSource (p.Argument.Value));
							wrote = true;
						}
						sw.Write (")");
					}
					sw.WriteLine ();
				}
			}
		}

		internal static string GetJavaAccess (MethodAttributes access)
		{
			switch (access) {
				case MethodAttributes.Public:
					return "public";
				case MethodAttributes.FamORAssem:
					return "protected";
				case MethodAttributes.Family:
					return "protected";
				default:
					return "private";
			}
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
}
