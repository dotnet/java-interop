using System;
using System.Reflection;
using System.Collections.Generic;

using Java.Interop;

using NUnit.Framework;

namespace Java.InteropTests {

	[TestFixture]
	public class JniRuntimeJniTypeManagerTests : JavaVMFixture {

		[Test]
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

#if NET
		[Test]
		public void GetInvokerType_GenericType_Throws ()
		{
			using (var vm  = new MyTypeManager ()) {
				Assert.Throws<NotSupportedException> (() => vm.GetInvokerType (typeof (IGenericJavaInterface<string>)));
			}
		}
#endif  // NET

		class MyTypeManager : JniRuntime.JniTypeManager {
		}

		[JniTypeSignature ("example/GenericInterface", GenerateJavaPeer=false, InvokerType=typeof (IGenericJavaInterfaceInvoker<>))]
		interface IGenericJavaInterface<T> : IJavaPeerable {
		}

		[JniTypeSignature ("example/GenericInterface", GenerateJavaPeer=false)]
		class IGenericJavaInterfaceInvoker<T> : JavaObject, IGenericJavaInterface<T> {

			public IGenericJavaInterfaceInvoker (ref JniObjectReference reference, JniObjectReferenceOptions options)
				: base (ref reference, options)
			{
			}
		}
	}
}
