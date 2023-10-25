#nullable enable

using System;
using System.Threading;

using Java.Interop;

namespace Java.Interop {

	public struct JniNativeMethodRegistration {

		public  string      Name;
		public  string      Signature;
		public  Delegate    Marshaler;

		public JniNativeMethodRegistration (string name, string signature, Delegate marshaler)
		{
			Name        = name      ?? throw new ArgumentNullException (nameof (name));
			Signature   = signature ?? throw new ArgumentNullException (nameof (signature));
			Marshaler   = marshaler ?? throw new ArgumentNullException (nameof (marshaler));
		}
	}

	public struct JniBlittableNativeMethodRegistration : IEquatable<JniBlittableNativeMethodRegistration> {

		IntPtr name, signature, marshaler;

		public JniBlittableNativeMethodRegistration (IntPtr name, IntPtr signature, IntPtr marshaler)
		{
			if (name == IntPtr.Zero)
				throw new ArgumentNullException (nameof (name));
			if (signature == IntPtr.Zero)
				throw new ArgumentNullException (nameof (signature));
			if (marshaler == IntPtr.Zero)
				throw new ArgumentNullException (nameof (marshaler));

			this.name       = name;
			this.signature  = signature;
			this.marshaler  = marshaler;
		}

		public JniBlittableNativeMethodRegistration (ReadOnlySpan<byte> name, ReadOnlySpan<byte> signature, IntPtr marshaler)
		{
			if (name.Length == 0)
				throw new ArgumentException ("must not be empty", nameof (name));
			if (signature.Length == 0)
				throw new ArgumentException ("must not be empty", nameof (signature));
			if (marshaler == IntPtr.Zero)
				throw new ArgumentNullException (nameof (marshaler));

			this.name       = FromReadOnlySpan (name);
			this.signature  = FromReadOnlySpan (signature);
			this.marshaler  = marshaler;
		}

		// Dodgy AF, but as the C# compiler guarantees no allocations for u8 strings,
		// the "address" of `value` will never move, so this is "fine"…
		// *so long as* it's *only* used for "string"u8 values.
		// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-11.0/utf8-string-literals#detailed-design
		// > Since the literals would be allocated as global constants, the lifetime of the
		// > resulting ReadOnlySpan<byte> would not prevent it from being returned or passed around to elsewhere. 
		static unsafe IntPtr FromReadOnlySpan (ReadOnlySpan<byte> value)
		{
			fixed (byte* p = value)
				return (IntPtr) p;
		}

		public override bool Equals (object? obj)
		{
			if (obj is JniBlittableNativeMethodRegistration other) {
				return Equals (other);
			}
			return false;
		}

		public override int GetHashCode () =>
			HashCode.Combine (name, signature, marshaler);

		public bool Equals (JniBlittableNativeMethodRegistration other) =>
			name == other.name &&
			signature == other.signature &&
			marshaler == other.marshaler;

		public static bool operator == (JniBlittableNativeMethodRegistration lhs, JniBlittableNativeMethodRegistration rhs) =>
			lhs.Equals (rhs);
		public static bool operator != (JniBlittableNativeMethodRegistration lhs, JniBlittableNativeMethodRegistration rhs) =>
			!lhs.Equals (rhs);
	}
}
