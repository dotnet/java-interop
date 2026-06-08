using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Java.Interop;
using NUnit.Framework;

namespace Java.InteropTests {

	public abstract partial class JavaVMFixture {

		[RequiresDynamicCode ("JavaVMFixture uses DynamicJniTypeManager, which is reflection-based and not NativeAOT-compatible.")]
		[RequiresUnreferencedCode ("JavaVMFixture uses DynamicJniTypeManager, which is reflection-based and not trimming-compatible.")]
		static partial void CreateJavaVM ();

		// VM supports specifying a class to JNIEnv::CallNonvirtualVoidMethod()
		// that  isn't where the jmethodID came from.
		public  static  bool    CallNonvirtualVoidMethodSupportsDeclaringClassMismatch;

		public  static  readonly    bool    HaveSafeHandles = typeof (JniObjectReference).GetField ("gcHandle", BindingFlags.NonPublic | BindingFlags.Instance) != null;

		protected JavaVMFixture ()
		{
		}


		[OneTimeSetUp]
		[RequiresDynamicCode ("JavaVMFixture uses DynamicJniTypeManager, which is reflection-based and not NativeAOT-compatible.")]
		[RequiresUnreferencedCode ("JavaVMFixture uses DynamicJniTypeManager, which is reflection-based and not trimming-compatible.")]
		public void EnsureJavaVM ()
		{
			if (VM == null) {
				CreateJavaVM ();
			}
		}
	}
}
