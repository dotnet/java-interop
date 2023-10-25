using System.Runtime.InteropServices;

using Java.Interop;

namespace Hello_NativeAOTFromJNI;

static class JNIEnvInit
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

	// symbol name from `$(IntermediateOutputPath)/h-classes/com_microsoft_hello_from_jni_NativeAOTInit.h`
	[UnmanagedCallersOnly (EntryPoint="Java_com_microsoft_hello_1from_1jni_NativeAOTInit_sayHello")]
	static IntPtr sayHello (IntPtr jnienv, IntPtr klass)
	{
		var s = $"Hello from .NET NativeAOT!";
		Console.WriteLine (s);
		var h = JniEnvironment.Strings.NewString (s);
		var r = JniEnvironment.References.NewReturnToJniRef (h);
		JniObjectReference.Dispose (ref h);
		return r;
	}
}
