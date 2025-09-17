#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Runtime.InteropServices;

using Java.Interop;

namespace Java.Interop {

	public struct JniNativeMethodRegistration {

		public  string      Name;
		public  string      Signature;
		public  IntPtr      Marshaler;

		private static HashSet<Delegate> s_keepAlive = new ();

		public JniNativeMethodRegistration (string name, string signature, Delegate marshaler)
		{
			Name        = name      ?? throw new ArgumentNullException (nameof (name));
			Signature   = signature ?? throw new ArgumentNullException (nameof (signature));

			if (marshaler == null)
			{
				throw new ArgumentNullException (nameof (marshaler));
			}

#if DEBUG
			if (marshaler.GetType ().GenericTypeArguments.Length != 0) {
				var method  = marshaler.Method;
				Debug.WriteLine ($"JniNativeMethodRegistration given a generic delegate type `{marshaler.GetType()}`.  CoreCLR doesn't like this.");
				Debug.WriteLine ($"  Java: {name}{signature}");
				Debug.WriteLine ($"  Marshaler Type={marshaler.GetType ().FullName} Method={method.DeclaringType?.FullName}.{method.Name}");
			}
#endif  // DEBUG

			s_keepAlive.Add (marshaler);
			Marshaler = GetFunctionPointerForDelegate (marshaler);

			[UnconditionalSuppressMessage ("AOT", "IL3050", Justification = "Dynamic method registration does not work with Native AOT.")]
			static IntPtr GetFunctionPointerForDelegate (Delegate marshaler)
				=> Marshal.GetFunctionPointerForDelegate (marshaler);
		}

		public JniNativeMethodRegistration (string name, string signature, IntPtr marshaler)
		{
			Name        = name        ?? throw new ArgumentNullException (nameof (name));
			Signature   = signature   ?? throw new ArgumentNullException(nameof(signature));

			if (marshaler == IntPtr.Zero)
			{
				throw new ArgumentNullException (nameof (marshaler));
			}

			Marshaler = marshaler;
		}

	}
}
