using System.Diagnostics.CodeAnalysis;

using Java.Interop;

using NUnit.Framework;

namespace Java.InteropTests {

	[TestFixture]
	public class JniRuntimeJniTypeManagerTests : JavaVMFixture {

		[Test]
		[RequiresDynamicCode ("This test uses DynamicJniTypeManager, which is reflection-based and not NativeAOT-compatible.")]
		[RequiresUnreferencedCode ("This test uses DynamicJniTypeManager, which is reflection-based and not trimming-compatible.")]
		public void GetInvokerType ()
		{
			using (var vm  = new MyTypeManager ()) {
				// Concrete type; no invoker
				Assert.IsNull (vm.GetInvokerType (typeof (JavaObject)));

				// Not a bound abstract Java type; no invoker
				Assert.IsNull (vm.GetInvokerType (typeof (System.ICloneable)));

				// Bound abstract Java type; has an invoker
				Assert.AreSame (typeof (IJavaInterfaceInvoker), vm.GetInvokerType (typeof (IJavaInterface)));
			}
		}

		[RequiresDynamicCode ("MyTypeManager uses DynamicJniTypeManager, which is reflection-based and not NativeAOT-compatible.")]
		[RequiresUnreferencedCode ("MyTypeManager uses DynamicJniTypeManager, which is reflection-based and not trimming-compatible.")]
		class MyTypeManager : JniRuntime.DynamicJniTypeManager {
			public MyTypeManager ()
			{
			}
		}
	}
}
