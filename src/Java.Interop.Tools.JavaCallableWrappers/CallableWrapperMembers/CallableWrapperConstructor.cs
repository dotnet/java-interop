using System.IO;

namespace Java.Interop.Tools.JavaCallableWrappers.CallableWrapperMembers;

public class CallableWrapperConstructor : CallableWrapperMethod
{
	public bool CannotRegisterInStaticConstructor { get; set; }
	public string? PartialAssemblyQualifiedName { get; set; }

	public CallableWrapperConstructor (string name, string method, string jniSignature) : base (name, method, jniSignature)
	{
	}

	public override void Generate (TextWriter sw, CallableWrapperWriterOptions options)
	{
		// TODO:  we only generate constructors so that Android types w/ no
		//        default constructor can be subclasses by our generated code.
		//
		//        This does NOT currently allow creating managed types from Java.
		sw.WriteLine ();

		foreach (var annotation in Annotations)
			annotation.Generate (sw, "", options);

		sw.Write ("\tpublic ");
		sw.Write (Name);

		sw.Write (" (");
		sw.Write (Params);
		sw.Write (')');

		sw.WriteLine (ThrowsDeclaration);

		sw.WriteLine ("\t{");
		sw.Write ("\t\tsuper (");
		sw.Write (SuperCall);
		sw.WriteLine (");");

#if MONODROID_TIMING
		sw.WriteLine ("\t\tandroid.util.Log.i(\"MonoDroid-Timing\", \"{0}..ctor({1}): time: \"+java.lang.System.currentTimeMillis());", Name, Params);
#endif

		if (!CannotRegisterInStaticConstructor) {

			sw.Write ("\t\tif (getClass () == ");
			sw.Write (Name);
			sw.WriteLine (".class) {");

			sw.Write ("\t\t\t");

			switch (options.CodeGenerationTarget) {
				case JavaPeerStyle.JavaInterop1:
					sw.Write ("net.dot.jni.ManagedPeer.construct (this, \"");
					sw.Write (JniSignature);
					sw.Write ("\", new java.lang.Object[] { ");
					sw.Write (ActivateCall);
					sw.WriteLine (" });");
					break;
				default:
					sw.Write ("mono.android.TypeManager.Activate (\"");
					sw.Write (PartialAssemblyQualifiedName);
					sw.Write ("\", \"");
					sw.Write (ManagedParameters);
					sw.Write ("\", this, new java.lang.Object[] { ");
					sw.Write (ActivateCall);
					sw.WriteLine (" });");
					break;
			}

			sw.WriteLine ("\t\t}");
		}

		sw.WriteLine ("\t}");
	}
}
