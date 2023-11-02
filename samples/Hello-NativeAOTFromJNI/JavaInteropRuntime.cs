using System.Runtime.InteropServices;

using Java.Interop;

namespace Hello_NativeAOTFromJNI;

static class JavaInteropRuntime
{
	static JniRuntime? runtime;

	[UnmanagedCallersOnly (EntryPoint="JNI_OnLoad")]
	static int JNI_OnLoad (IntPtr vm, IntPtr reserved)
	{
		return (int) JniVersion.v1_6;
	}

	[UnmanagedCallersOnly (EntryPoint="JNI_OnUnload")]
	static void JNI_Onload (IntPtr vm, IntPtr reserved)
	{
		runtime?.Dispose ();
	}

	[UnmanagedCallersOnly (EntryPoint="Java_com_microsoft_java_1interop_JavaInteropRuntime_init")]
	static void init (IntPtr jnienv, IntPtr klass)
	{
		Console.WriteLine ($"C# init()");
		try {
			var options = new JreRuntimeOptions {
				EnvironmentPointer  = jnienv,
				TypeManager         = new NativeAotTypeManager (),
			};
			runtime = options.CreateJreVM ();
		}
		catch (Exception e) {
			Console.Error.WriteLine ($"JavaInteropRuntime.init: error: {e}");
		}
	}
}
