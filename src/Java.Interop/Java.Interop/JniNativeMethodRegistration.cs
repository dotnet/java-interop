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
		public  IntPtr      MarshalerPtr;
		public Delegate Marshaler {
			[Obsolete ("Use MarshalerPtr instead.")]
			get {
				return s_reverseMapping.TryGetValue (MarshalerPtr, out var mappedDelegate)
					? mappedDelegate
					: throw new InvalidOperationException ($"Cannot convert MarshalerPtr {MarshalerPtr} for {Name}{Signature} to Delegate.");
			}

			set {
#if DEBUG
				if (value.GetType ().GenericTypeArguments.Length != 0) {
					var method  = value.Method;
					Debug.WriteLine ($"JniNativeMethodRegistration given a generic delegate type `{value.GetType ()}`.  CoreCLR doesn't like this.");
					if (Name is string name && Signature is string signature) {
						Debug.WriteLine ($"  Java: {name}{signature}");
					}
					Debug.WriteLine ($"  Marshaler Type={value.GetType ().FullName} Method={method.DeclaringType?.FullName}.{method.Name}");
				}
#endif  // DEBUG

				MarshalerPtr = GetFunctionPointerForDelegate (value);
				s_reverseMapping.Add (MarshalerPtr, value);

				[UnconditionalSuppressMessage ("AOT", "IL3050", Justification = "Dynamic method registration does not work with Native AOT.")]
				static IntPtr GetFunctionPointerForDelegate (Delegate marshaler)
					=> Marshal.GetFunctionPointerForDelegate (marshaler);
			}
		}

		private static readonly Dictionary<IntPtr, Delegate> s_reverseMapping = new ();

		public JniNativeMethodRegistration (string name, string signature, Delegate marshaler)
		{
			Name = name ?? throw new ArgumentNullException (nameof (name));
			Signature = signature ?? throw new ArgumentNullException (nameof (signature));

			if (marshaler == null) {
				throw new ArgumentNullException (nameof (marshaler));
			}

			Marshaler = marshaler;
		}

		public JniNativeMethodRegistration (string name, string signature, IntPtr marshaler)
		{
			Name        = name        ?? throw new ArgumentNullException (nameof (name));
			Signature   = signature   ?? throw new ArgumentNullException(nameof(signature));

			if (marshaler == IntPtr.Zero)
			{
				throw new ArgumentNullException (nameof (marshaler));
			}

			MarshalerPtr = marshaler;
		}

	}
}
