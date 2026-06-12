using System.Diagnostics;
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
	static void JNI_OnUnload (IntPtr vm, IntPtr reserved)
	{
		runtime?.Dispose ();
		runtime = null;
	}

	[UnmanagedCallersOnly (EntryPoint="Java_net_dot_jni_hello_JavaInteropRuntime_init")]
	static void init (IntPtr jnienv, IntPtr klass)
	{
		if (runtime != null)
			return;

		try {
			runtime = new ExistingJniRuntime (jnienv);
		}
		catch (Exception e) {
			Console.Error.WriteLine ($"JavaInteropRuntime.init: error: {e}");
		}
	}

	sealed class ExistingJniRuntime : JniRuntime {

		public ExistingJniRuntime (IntPtr jnienv)
			: base (new CreationOptions {
				EnvironmentPointer      = jnienv,
				ObjectReferenceManager  = new ObjectReferenceManager (),
				ValueManager            = new ValueManager (),
			})
		{
		}

		public override string? GetCurrentManagedThreadName ()
		{
			return Thread.CurrentThread.Name;
		}

		public override string GetCurrentManagedThreadStackTrace (int skipFrames, bool fNeedFileInfo)
		{
			return new StackTrace (skipFrames, fNeedFileInfo).ToString ();
		}
	}

	sealed class ObjectReferenceManager : JniRuntime.JniObjectReferenceManager {
		public override int GlobalReferenceCount => 0;

		public override int WeakGlobalReferenceCount => 0;
	}

	sealed class ValueManager : JniRuntime.JniValueManager {
		public override void WaitForGCBridgeProcessing ()
		{
		}

		public override void CollectPeers ()
		{
		}

		public override void AddPeer (IJavaPeerable value)
		{
		}

		public override void RemovePeer (IJavaPeerable value)
		{
		}

		public override void FinalizePeer (IJavaPeerable value)
		{
		}

		public override List<JniSurfacedPeerInfo> GetSurfacedPeers ()
		{
			return new List<JniSurfacedPeerInfo> ();
		}

		public override IJavaPeerable? PeekPeer (JniObjectReference reference)
		{
			return null;
		}
	}
}
