using System.Runtime.InteropServices;

using Java.Interop;

namespace Hello_NativeAOTFromJNI;

static class JNIEnvInit
{
	// static JniRuntime? runtime;

	[UnmanagedCallersOnly (EntryPoint="JNI_OnLoad")]
	static int JNI_OnLoad (IntPtr vm, IntPtr reserved)
	{
		try {
			// runtime = new JniRuntime (null);
			// return runtime.JniVersion;
			return (int) JniVersion.v1_2;
		}
		catch (Exception e) {
			Console.Error.WriteLine ($"JNI_OnLoad: {e}");
			return 0;
		}
	}

	[UnmanagedCallersOnly (EntryPoint="JNI_OnUnload")]
	static void JNI_Onload (IntPtr vm, IntPtr reserved)
	{
		// runtime?.Dispose ();
	}

	// symbol name from `$(IntermediateOutputPath)/h-classes/com_microsoft_hello_from_jni_NativeAOTInit.h`
	[UnmanagedCallersOnly (EntryPoint="Java_com_microsoft_hello_1from_1jni_NativeAOTInit_sayHello")]
	static void sayHello (IntPtr jnienv, IntPtr klass)
	{
		Console.WriteLine ($"Hello from .NET NativeAOT!");
	}
}
