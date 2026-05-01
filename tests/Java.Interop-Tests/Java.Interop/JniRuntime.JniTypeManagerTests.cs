using System;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

using Java.Interop;

using NUnit.Framework;

namespace Java.InteropTests {

	[TestFixture]
	public class JniRuntimeJniTypeManagerTests : JavaVMFixture {

		[Test]
		[RequiresDynamicCode ("Tests generic invoker type construction.")]
		[RequiresUnreferencedCode ("Tests generic invoker type construction.")]
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

		[Test]
		[RequiresDynamicCode ("Tests generic invoker type construction.")]
		[RequiresUnreferencedCode ("Tests generic invoker type construction.")]
		public void GetInvokerType_Generic ()
		{
			using (var vm  = new MyTypeManager ()) {
				Assert.AreSame (typeof (GenericJavaInterfaceInvoker<int>), vm.GetInvokerType (typeof (IGenericJavaInterface<int>)));
			}
		}

		class MyTypeManager : JniRuntime.JniTypeManager {
		}

		[JniTypeSignature ("net/dot/jni/test/GenericJavaInterface", GenerateJavaPeer=false, InvokerType=typeof (GenericJavaInterfaceInvoker<>))]
		interface IGenericJavaInterface<T> : IJavaPeerable {
		}

		[JniTypeSignature ("net/dot/jni/test/GenericJavaInterface", GenerateJavaPeer=false)]
		class GenericJavaInterfaceInvoker<T> : JavaObject, IGenericJavaInterface<T> {
		}
    }
}
