using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Java.Interop.Tools.Cecil;
using System.Xml.Linq;
using Java.Interop.Tools.Diagnostics;
using Java.Interop.Tools.TypeNameMappings;
using Mono.Cecil;
using static Java.Interop.Tools.JavaCallableWrappers.JavaCallableWrapperGenerator;
using Java.Interop.Tools.JavaCallableWrappers.Adapters;

namespace Java.Interop.Tools.JavaCallableWrappers.CallableWrapperMembers;

class CallableWrapperType
{
	public string Name { get; set; }
	public string Package { get; set; }
	public bool IsAbstract { get; set; }
	public string? ApplicationJavaClass { get; set; }
	public bool HasDynamicallyRegisteredMethods { get; set; }
	public bool GenerateOnCreateOverrides { get; set; }
	public string? MonoRuntimeInitialization { get; set; }

	public List<CallableWrapperConstructor> Constructors { get; } = new List<CallableWrapperConstructor> ();
	public List<CallableWrapperField> Fields { get; } = new List<CallableWrapperField> ();
	public List<CallableWrapperMethod> Methods { get; } = new List<CallableWrapperMethod> ();
	public List<CallableWrapperType> NestedTypes { get; } = new List<CallableWrapperType> ();

	// TODO: Remove Cecil
	public TypeDefinition Type { get; set; } = null!;
	public IMetadataResolver Cache { get; set; } = null!;
	public JavaCallableWrapperGenerator Generator { get; set; } = null!;

	public CallableWrapperType (string name, string package)
	{
		Name = name;
		Package = package;
	}

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

	public void Generate (TextWriter writer, CallableWrapperWriterOptions options, bool isNested)
	{
		if (!isNested && !string.IsNullOrEmpty (Package)) {
			writer.WriteLine ("package " + Package + ";");
			writer.WriteLine ();
		}

		GenerateHeader (writer, options);

		if (!isNested)
			GenerateInfrastructure (writer, options);

		GenerateBody (writer, options);

		foreach (var nested in NestedTypes)
			nested.Generate (writer, options, true);

		GenerateFooter (writer, options);
	}	

	public void GenerateHeader (TextWriter sw, CallableWrapperWriterOptions options)
	{
		sw.WriteLine ();

		// class annotations.
		JavaCallableWrapperGenerator.WriteAnnotations ("", sw, Type.CustomAttributes, Cache);

		sw.WriteLine ("public " + (IsAbstract ? "abstract " : "") + "class " + Name);

		var extendsType = GetJavaTypeName (Type.BaseType, Cache);

		if (extendsType == "android.app.Application" && ApplicationJavaClass != null && !string.IsNullOrEmpty (ApplicationJavaClass))
			extendsType = ApplicationJavaClass;

		sw.WriteLine ("\textends " + extendsType);
		sw.WriteLine ("\timplements");
		sw.Write ("\t\t");
		switch (options.CodeGenerationTarget) {
			case JavaPeerStyle.JavaInterop1:
				sw.Write ("net.dot.jni.GCUserPeerable");
				break;
			default:
				sw.Write ("mono.android.IGCUserPeer");
				break;
		}
		foreach (var ifaceInfo in Type.Interfaces) {
			var iface = Cache.Resolve (ifaceInfo.InterfaceType);
			if (!JavaCallableWrapperGenerator.GetTypeRegistrationAttributes (iface).Any ())
				continue;
			sw.WriteLine (",");
			sw.Write ("\t\t");
			sw.Write (GetJavaTypeName (iface, Cache));
		}
		sw.WriteLine ();
		sw.WriteLine ("{");
	}

	public void GenerateInfrastructure (TextWriter writer, CallableWrapperWriterOptions options)
	{
		var needCtor = false;

		if (HasDynamicallyRegisteredMethods) {
			needCtor = true;
			writer.WriteLine ("/** @hide */");
			writer.WriteLine ("\tpublic static final String __md_methods;");
		}

		for (int i = 0; i < NestedTypes.Count; i++) {
			if (!NestedTypes [i].HasDynamicallyRegisteredMethods) {
				continue;
			}
			needCtor = true;
			writer.Write ("\tstatic final String __md_");
			writer.Write (i + 1);
			writer.WriteLine ("_methods;");
		}

		if (needCtor) {
			writer.WriteLine ("\tstatic {");

			if (HasDynamicallyRegisteredMethods) {
				GenerateRegisterType (writer, Generator, "__md_methods", options);
			}

			for (int i = 0; i < NestedTypes.Count; ++i) {
				GenerateRegisterType (writer, NestedTypes [i].Generator, $"__md_{i + 1}_methods", options);
			}

			writer.WriteLine ("\t}");
		}
	}

	public void GenerateBody (TextWriter sw, CallableWrapperWriterOptions options)
	{
		foreach (Signature ctor in Generator.ctors) {
			if (string.IsNullOrEmpty (ctor.Params) && JavaNativeTypeManager.IsApplication (Type, Cache))
				continue;

			var ct = CecilImporter.CreateConstructor (ctor);

			ct.Name = Name;
			ct.CannotRegisterInStaticConstructor = CannotRegisterInStaticConstructor (Type);
			ct.PartialAssemblyQualifiedName = Type.GetPartialAssemblyQualifiedName (Cache);

			ct.Generate (sw, options);
		}

		if (CecilImporter.CreateApplicationConstructor (Name, Type, Cache) is CallableWrapperApplicationConstructor app_ctor)
			app_ctor.Generate (sw, options);

		foreach (JavaFieldInfo field in Generator.exported_fields)
			CecilImporter.CreateField (field).Generate (sw);

		foreach (Signature method in Generator.methods)
			CecilImporter.CreateMethod (method).Generate (sw, options);

		if (GenerateOnCreateOverrides && JavaNativeTypeManager.IsApplication (Type, Cache) && !Generator.methods.Any (m => m.Name == "onCreate"))
			WriteApplicationOnCreate (sw, w => {
				w.Write ("\t\tmono.android.Runtime.register (\"");
				w.Write (Type.GetPartialAssemblyQualifiedName (Cache));
				w.Write ("\", ");
				w.Write (Name);
				w.WriteLine (".class, __md_methods);");
				w.WriteLine ("\t\tsuper.onCreate ();");
			});
		if (GenerateOnCreateOverrides && JavaNativeTypeManager.IsInstrumentation (Type, Cache) && !Generator.methods.Any (m => m.Name == "onCreate"))
			WriteInstrumentationOnCreate (sw, w => {
				w.Write ("\t\tmono.android.Runtime.register (\"");
				w.Write (Type.GetPartialAssemblyQualifiedName (Cache));
				w.Write ("\", ");
				w.Write (Name);
				w.WriteLine (".class, __md_methods);");
				w.WriteLine ("\t\tsuper.onCreate (arguments);");
			});

		string addRef = "monodroidAddReference";
		string clearRefs = "monodroidClearReferences";
		if (options.CodeGenerationTarget == JavaPeerStyle.JavaInterop1) {
			addRef = "jiAddManagedReference";
			clearRefs = "jiClearManagedReferences";
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

	public void GenerateFooter (TextWriter sw, CallableWrapperWriterOptions options)
	{
		sw.WriteLine ("}");
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

	void GenerateRegisterType (TextWriter sw, JavaCallableWrapperGenerator self, string field, CallableWrapperWriterOptions options)
	{
		if (!self.HasDynamicallyRegisteredMethods) {
			return;
		}

		sw.Write ("\t\t");
		sw.Write (field);
		sw.WriteLine (" = ");
		string managedTypeName = self.type.GetPartialAssemblyQualifiedName (Cache);
		string javaTypeName = $"{Package}.{Name}";

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
		switch (options.CodeGenerationTarget) {
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

	bool CannotRegisterInStaticConstructor (TypeDefinition type)
	{
		return JavaNativeTypeManager.IsApplication (type, Cache) || JavaNativeTypeManager.IsInstrumentation (type, Cache);
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

	/// <summary>
	/// Returns a destination file path based on the package name of this Java type
	/// </summary>
	public string GetDestinationPath (string outputPath)
	{
		var dir = Package.Replace ('.', Path.DirectorySeparatorChar);
		return Path.Combine (outputPath, dir, Name + ".java");
	}

	public void Generate (string outputPath, CallableWrapperWriterOptions options)
	{
		using (StreamWriter sw = OpenStream (outputPath)) {
			Generate (sw, options, false);
		}
	}

	StreamWriter OpenStream (string outputPath)
	{
		string destination = GetDestinationPath (outputPath);
		Directory.CreateDirectory (Path.GetDirectoryName (destination));
		return new StreamWriter (new FileStream (destination, FileMode.Create, FileAccess.Write));
	}
}
