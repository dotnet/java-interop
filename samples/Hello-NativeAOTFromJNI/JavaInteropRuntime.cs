using System.Runtime.InteropServices;

using Java.Interop;

namespace Hello_NativeAOTFromJNI;

static class JavaInteropRuntime
{
	static JniRuntime? runtime;

	[UnmanagedCallersOnly (EntryPoint="JNI_OnLoad")]
	static int JNI_OnLoad (IntPtr vm, IntPtr reserved)
	{
		try {
			var options = new JreRuntimeOptions {
				InvocationPointer = vm,
			};
			runtime = options.CreateJreVM ();
			return (int) runtime.JniVersion;
		}
		catch (Exception e) {
			Console.Error.WriteLine ($"JNI_OnLoad: error: {e}");
			return 0;
		}
	}

	[UnmanagedCallersOnly (EntryPoint="JNI_OnUnload")]
	static void JNI_Onload (IntPtr vm, IntPtr reserved)
	{
		runtime?.Dispose ();
	}

	[UnmanagedCallersOnly (EntryPoint="Java_com_microsoft_java_1interop_JavaInteropRuntime_init")]
	static void init ()
	{
		Console.Error.WriteLine ($"C# init()");
	}
}
