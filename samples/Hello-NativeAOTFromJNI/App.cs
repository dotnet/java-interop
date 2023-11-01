using System.Runtime.InteropServices;

using Java.Interop;

namespace Hello_NativeAOTFromJNI;

static class App {

	// symbol name from `$(IntermediateOutputPath)/h-classes/com_microsoft_hello_from_jni_NativeAOTInit.h`
	[UnmanagedCallersOnly (EntryPoint="Java_com_microsoft_hello_1from_1jni_App_sayHello")]
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