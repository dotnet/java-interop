using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Java.Interop.Tools.Cecil;
using Java.Interop.Tools.Diagnostics;
using Java.Interop.Tools.TypeNameMappings;
using Mono.Cecil;

using JavaFieldInfo = Java.Interop.Tools.JavaCallableWrappers.JavaCallableWrapperGenerator.JavaFieldInfo;
using Signature = Java.Interop.Tools.JavaCallableWrappers.JavaCallableWrapperGenerator.Signature;

namespace Java.Interop.Tools.JavaCallableWrappers;

class JavaCallableWrapperWriter
{
	readonly string name;
	readonly string package;
	readonly TypeDefinition type;

	readonly List<JavaCallableWrapperGenerator>? children;
	readonly List<Signature> ctors;
	readonly List<JavaFieldInfo> exported_fields;
	readonly List<Signature> methods;

	readonly IMetadataResolver cache;
	readonly JavaCallableWrapperGenerator generator;

	public JavaCallableWrapperWriter (string name, string package, TypeDefinition type, bool hasDynamicallyRegisteredMethods, List<JavaCallableWrapperGenerator>? children, IMetadataResolver cache, string? applicationJavaClass, JavaPeerStyle codeGenerationTarget, bool generateOnCreateOverrides, string? monoRuntimeInitialization, List<JavaFieldInfo> exported_fields, List<Signature> methods, List<Signature> ctors, JavaCallableWrapperGenerator generator)
	{
		this.name = name;
		this.package = package;
		this.type = type;
		HasDynamicallyRegisteredMethods = hasDynamicallyRegisteredMethods;
		this.children = children;
		this.cache = cache;
		ApplicationJavaClass = applicationJavaClass;
		CodeGenerationTarget = codeGenerationTarget;
		GenerateOnCreateOverrides = generateOnCreateOverrides;
		MonoRuntimeInitialization = monoRuntimeInitialization;
		this.exported_fields = exported_fields;
		this.methods = methods;
		this.ctors = ctors;
		this.generator = generator;
	}

	string? ApplicationJavaClass { get; }

	JavaPeerStyle CodeGenerationTarget { get; }

	bool GenerateOnCreateOverrides { get; }

	bool HasDynamicallyRegisteredMethods { get; }

	string? MonoRuntimeInitialization { get; }

	// example of java target to generate for a type
	//
	// package mono.droid;
	//
	// import android.app.Activity;
	// import android.os.Bundle;
	//
	// public class MonoActivity extends android.app.Activity
	// {
	// 	  static final String __md_methods;
	// 	  static {
	// 	    __md_methods =
	// 	      "n_OnCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
	// 	      "";
	// 	    mono.android.Runtime.register ("Mono.Droid.MonoActivity, AssemblyName", MonoActivity.class, __md_methods);
	// 	  }
	//
	//    public void onCreate(android.os.Bundle savedInstanceState)
	//    {
	//      n_onCreate (savedInstanceState);
	//    }
	//
	//    private native void n_onCreate (android.os.Bundle bundle);
	// }

	public void Generate (TextWriter writer)
	{
		if (!string.IsNullOrEmpty (package)) {
			writer.WriteLine ("package " + package + ";");
			writer.WriteLine ();
		}

		GenerateHeader (writer);

		bool needCtor = false;
		if (HasDynamicallyRegisteredMethods) {
			needCtor = true;
			writer.WriteLine ("/** @hide */");
			writer.WriteLine ("\tpublic static final String __md_methods;");
		}

		if (children != null) {
			for (int i = 0; i < children.Count; i++) {
				if (!children[i].HasDynamicallyRegisteredMethods) {
					continue;
				}
				needCtor = true;
				writer.Write ("\tstatic final String __md_");
				writer.Write (i + 1);
				writer.WriteLine ("_methods;");
			}
		}

		if (needCtor) {
			writer.WriteLine ("\tstatic {");

			if (HasDynamicallyRegisteredMethods) {
				GenerateRegisterType (writer, generator, "__md_methods");
			}

			if (children != null) {
				for (int i = 0; i < children.Count; ++i) {
					GenerateRegisterType (writer, children [i], $"__md_{i + 1}_methods");
				}
			}
			writer.WriteLine ("\t}");
		}

		GenerateBody (writer);

		if (children != null)
			foreach (JavaCallableWrapperGenerator child in children) {
				var child_writer = child.CreateWriter ();
				child_writer.GenerateHeader (writer);
				child_writer.GenerateBody (writer);
				child_writer.GenerateFooter (writer);
			}

		GenerateFooter (writer);
	}

	public void Generate (string outputPath)
	{
		using (StreamWriter sw = OpenStream (outputPath)) {
			Generate (sw);
		}
	}

	void GenerateHeader (TextWriter sw)
	{
		sw.WriteLine ();

		// class annotations.
		JavaCallableWrapperGenerator.WriteAnnotations ("", sw, type.CustomAttributes, cache);

		sw.WriteLine ("public " + (type.IsAbstract ? "abstract " : "") + "class " + name);

		string extendsType = GetJavaTypeName (type.BaseType, cache);
		if (extendsType == "android.app.Application" && ApplicationJavaClass != null && !string.IsNullOrEmpty (ApplicationJavaClass))
			extendsType = ApplicationJavaClass;
		sw.WriteLine ("\textends " + extendsType);
		sw.WriteLine ("\timplements");
		sw.Write ("\t\t");
		switch (CodeGenerationTarget) {
			case JavaPeerStyle.JavaInterop1:
				sw.Write ("net.dot.jni.GCUserPeerable");
				break;
			default:
				sw.Write ("mono.android.IGCUserPeer");
				break;
		}
		foreach (var ifaceInfo in type.Interfaces) {
			var iface = cache.Resolve(ifaceInfo.InterfaceType);
			if (!JavaCallableWrapperGenerator.GetTypeRegistrationAttributes (iface).Any ())
				continue;
			sw.WriteLine (",");
			sw.Write ("\t\t");
			sw.Write (GetJavaTypeName (iface, cache));
		}
		sw.WriteLine ();
		sw.WriteLine ("{");
	}

	void GenerateBody (TextWriter sw)
	{
		foreach (Signature ctor in ctors) {
			if (string.IsNullOrEmpty (ctor.Params) && JavaNativeTypeManager.IsApplication (type, cache))
				continue;
			GenerateConstructor (ctor, sw);
		}

		GenerateApplicationConstructor (sw);

		foreach (JavaFieldInfo field in exported_fields)
			GenerateExportedField (field, sw);

		foreach (Signature method in methods)
			GenerateMethod (method, sw);

		if (GenerateOnCreateOverrides && JavaNativeTypeManager.IsApplication (type, cache) && !methods.Any (m => m.Name == "onCreate"))
			WriteApplicationOnCreate (sw, w => {
					w.Write ("\t\tmono.android.Runtime.register (\"");
					w.Write (type.GetPartialAssemblyQualifiedName (cache));
					w.Write ("\", ");
					w.Write (name);
					w.WriteLine (".class, __md_methods);");
					w.WriteLine ("\t\tsuper.onCreate ();");
			});
		if (GenerateOnCreateOverrides && JavaNativeTypeManager.IsInstrumentation (type, cache) && !methods.Any (m => m.Name == "onCreate"))
			WriteInstrumentationOnCreate (sw, w => {
					w.Write ("\t\tmono.android.Runtime.register (\"");
					w.Write (type.GetPartialAssemblyQualifiedName (cache));
					w.Write ("\", ");
					w.Write (name);
					w.WriteLine (".class, __md_methods);");
					w.WriteLine ("\t\tsuper.onCreate (arguments);");
			});

		string addRef       = "monodroidAddReference";
		string clearRefs    = "monodroidClearReferences";
		if (CodeGenerationTarget == JavaPeerStyle.JavaInterop1) {
			addRef      = "jiAddManagedReference";
			clearRefs   = "jiClearManagedReferences";
		}

		sw.WriteLine ();
		sw.WriteLine ("\tprivate java.util.ArrayList refList;");
		sw.WriteLine ($"\tpublic void {addRef} (java.lang.Object obj)");
		sw.WriteLine ("\t{");
		sw.WriteLine ("\t\tif (refList == null)");
		sw.WriteLine ("\t\t\trefList = new java.util.ArrayList ();");
		sw.WriteLine ("\t\trefList.add (obj);");
		sw.WriteLine ("\t}");
		sw.WriteLine ();
		sw.WriteLine ($"\tpublic void {clearRefs} ()");
		sw.WriteLine ("\t{");
		sw.WriteLine ("\t\tif (refList != null)");
		sw.WriteLine ("\t\t\trefList.clear ();");
		sw.WriteLine ("\t}");
	}

	void GenerateRegisterType (TextWriter sw, JavaCallableWrapperGenerator self, string field)
	{
		if (!self.HasDynamicallyRegisteredMethods) {
			return;
		}

		sw.Write ("\t\t");
		sw.Write (field);
		sw.WriteLine (" = ");
		string managedTypeName = self.type.GetPartialAssemblyQualifiedName (cache);
		string javaTypeName = $"{package}.{name}";

		foreach (Signature method in self.methods) {
			if (method.IsDynamicallyRegistered) {
				sw.Write ("\t\t\t\"", method.Method);
				sw.Write (method.Method);
				sw.WriteLine ("\\n\" +");
			}
		}
		sw.WriteLine ("\t\t\t\"\";");
		if (CannotRegisterInStaticConstructor (self.type))
			return;
		sw.Write ("\t\t");
		switch (CodeGenerationTarget) {
			case JavaPeerStyle.JavaInterop1:
				sw.Write ("net.dot.jni.ManagedPeer.registerNativeMembers (");
				sw.Write (self.name);
				sw.Write (".class, ");
				sw.Write (field);
				sw.WriteLine (");");
				break;
			default:
				sw.Write ("mono.android.Runtime.register (\"");
				sw.Write (managedTypeName);
				sw.Write ("\", ");
				sw.Write (self.name);
				sw.Write (".class, ");
				sw.Write (field);
				sw.WriteLine (");");
				break;
		}
	}

	void GenerateFooter (TextWriter sw)
	{
		sw.WriteLine ("}");
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

	bool CannotRegisterInStaticConstructor (TypeDefinition type)
	{
		return JavaNativeTypeManager.IsApplication (type, cache) || JavaNativeTypeManager.IsInstrumentation (type, cache);
	}

	void GenerateConstructor (Signature ctor, TextWriter sw)
	{
		// TODO:  we only generate constructors so that Android types w/ no
		//        default constructor can be subclasses by our generated code.
		//
		//        This does NOT currently allow creating managed types from Java.
		sw.WriteLine ();
		if (ctor.Annotations != null)
			sw.WriteLine (ctor.Annotations);
		sw.Write ("\tpublic ");
		sw.Write (name);
		sw.Write (" (");
		sw.Write (ctor.Params);
		sw.Write (')');
		sw.WriteLine (ctor.ThrowsDeclaration);
		sw.WriteLine ("\t{");
		sw.Write ("\t\tsuper (");
		sw.Write (ctor.SuperCall);
		sw.WriteLine (");");
#if MONODROID_TIMING
		sw.WriteLine ("\t\tandroid.util.Log.i(\"MonoDroid-Timing\", \"{0}..ctor({1}): time: \"+java.lang.System.currentTimeMillis());", name, ctor.Params);
#endif
		if (!CannotRegisterInStaticConstructor (type)) {
			sw.Write ("\t\tif (getClass () == ");
			sw.Write (name);
			sw.WriteLine (".class) {");
			sw.Write ("\t\t\t");
			switch (CodeGenerationTarget) {
				case JavaPeerStyle.JavaInterop1:
					sw.Write ("net.dot.jni.ManagedPeer.construct (this, \"");
					sw.Write (ctor.JniSignature);
					sw.Write ("\", new java.lang.Object[] { ");
					sw.Write (ctor.ActivateCall);
					sw.WriteLine (" });");
					break;
				default:
					sw.Write ("mono.android.TypeManager.Activate (\"");
					sw.Write (type.GetPartialAssemblyQualifiedName (cache));
					sw.Write ("\", \"");
					sw.Write (ctor.ManagedParameters);
					sw.Write ("\", this, new java.lang.Object[] { ");
					sw.Write (ctor.ActivateCall);
					sw.WriteLine (" });");
					break;
			}
			sw.WriteLine ("\t\t}");
		}
		sw.WriteLine ("\t}");
	}

	void GenerateApplicationConstructor (TextWriter sw)
	{
		if (!JavaNativeTypeManager.IsApplication (type, cache)) {
			return;
		}

		sw.WriteLine ();
		sw.Write ("\tpublic ");
		sw.Write (name);
		sw.WriteLine (" ()");
		sw.WriteLine ("\t{");
		sw.WriteLine ("\t\tmono.MonoPackageManager.setContext (this);");
		sw.WriteLine ("\t}");
	}

	void GenerateExportedField (JavaFieldInfo field, TextWriter sw)
	{
		sw.WriteLine ();
		if (field.Annotations != null)
			sw.WriteLine (field.Annotations);
		sw.Write ("\t");
		sw.Write (field.GetJavaAccess ());
		sw.Write (' ');
		if (field.IsStatic)
			sw.Write ("static ");
		sw.Write (field.TypeName);
		sw.Write (' ');
		sw.Write (field.FieldName);
		sw.Write (" = ");
		sw.Write (field.InitializerName);
		sw.WriteLine (" ();");
	}

	void GenerateMethod (Signature method, TextWriter sw)
	{
		sw.WriteLine ();
		if (method.Annotations != null)
			sw.WriteLine (method.Annotations);
		sw.Write ("\t");
		sw.Write (method.IsExport ? method.JavaAccess : "public");
		sw.Write (' ');
		if (method.IsStatic)
			sw.Write ("static ");
		sw.Write (method.Retval);
		sw.Write (' ');
		sw.Write (method.JavaName);
		sw.Write (" (");
		sw.Write (method.Params);
		sw.Write (')');
		sw.WriteLine (method.ThrowsDeclaration);
		sw.WriteLine ("\t{");
#if MONODROID_TIMING
		sw.WriteLine ("\t\tandroid.util.Log.i(\"MonoDroid-Timing\", \"{0}.{1}: time: \"+java.lang.System.currentTimeMillis());", name, method.Name);
#endif
		sw.Write ("\t\t");
		sw.Write (method.Retval == "void" ? String.Empty : "return ");
		sw.Write ("n_");
		sw.Write (method.Name);
		sw.Write (" (");
		sw.Write (method.ActivateCall);
		sw.WriteLine (");");

		sw.WriteLine ("\t}");
		sw.WriteLine ();
		sw.Write ("\tprivate ");
		if (method.IsStatic)
			sw.Write ("static ");
		sw.Write ("native ");
		sw.Write (method.Retval);
		sw.Write (" n_");
		sw.Write (method.Name);
		sw.Write (" (");
		sw.Write (method.Params);
		sw.WriteLine (");");
	}

	void WriteApplicationOnCreate (TextWriter sw, Action<TextWriter> extra)
	{
		sw.WriteLine ();
		sw.WriteLine ("\tpublic void onCreate ()");
		sw.WriteLine ("\t{");
		extra (sw);
		sw.WriteLine ("\t}");
	}

	void WriteInstrumentationOnCreate (TextWriter sw, Action<TextWriter> extra)
	{
		sw.WriteLine ();
		sw.WriteLine ("\tpublic void onCreate (android.os.Bundle arguments)");
		sw.WriteLine ("\t{");

#if MONODROID_TIMING
		sw.WriteLine ("\t\tandroid.util.Log.i(\"MonoDroid-Timing\", \"{0}.onCreate(Bundle): time: \"+java.lang.System.currentTimeMillis());", name);
		sw.WriteLine ();
#endif

		sw.WriteLine ("\t\tandroid.content.Context context = getContext ();");
		sw.WriteLine ();

		if (!string.IsNullOrEmpty (MonoRuntimeInitialization)) {
			sw.WriteLine (MonoRuntimeInitialization);
			sw.WriteLine ();
		}

		extra (sw);
		sw.WriteLine ("\t}");
	}

	StreamWriter OpenStream (string outputPath)
	{
		string destination = GetDestinationPath (outputPath);
		Directory.CreateDirectory (Path.GetDirectoryName (destination));
		return new StreamWriter (new FileStream (destination, FileMode.Create, FileAccess.Write));
	}

	/// <summary>
	/// Returns a destination file path based on the package name of this Java type
	/// </summary>
	public string GetDestinationPath (string outputPath)
	{
		var dir = package.Replace ('.', Path.DirectorySeparatorChar);
		return Path.Combine (outputPath, dir, name + ".java");
	}
}
