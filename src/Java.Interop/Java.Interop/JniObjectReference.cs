#nullable enable

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if FEATURE_JNIOBJECTREFERENCE_SAFEHANDLES && FEATURE_JNIOBJECTREFERENCE_INTPTRS
#error  JniObjectReference cannot support both SafeHandles and IntPtrs.
#endif  // FEATURE_JNIOBJECTREFERENCE_SAFEHANDLES && FEATURE_JNIOBJECTREFERENCE_INTPTRS

namespace Java.Interop
{
	[Flags]
	enum JniObjectReferenceFlags : uint {
		None,
		Alloc   = 1 << 16,
	}

	public partial struct JniObjectReference : IEquatable<JniObjectReference>
	{
		const   uint    FlagsMask   = 0xFFFF0000;
		const   uint    TypeMask    = 0x0000FFFF;

#if FEATURE_JNIOBJECTREFERENCE_SAFEHANDLES
		GCHandle                gcHandle;
		internal    JniReferenceSafeHandle  SafeHandle  {
			get {return gcHandle.IsAllocated ? ((JniReferenceSafeHandle) gcHandle.Target) : JniReferenceSafeHandle.Null;}
		}
		public      IntPtr                  Handle  {
			[MethodImpl (MethodImplOptions.AggressiveInlining)]
			get {
				var h = SafeHandle;
				return h == null
					? IntPtr.Zero
					: h.DangerousGetHandle ();
			}
		}
#elif FEATURE_JNIOBJECTREFERENCE_INTPTRS
		public      IntPtr                  Handle  {
			[MethodImpl (MethodImplOptions.AggressiveInlining)]
			get; 
			private set;
		}
#endif

		uint    referenceInfo;

		public  JniObjectReferenceType      Type    {
			get {return (JniObjectReferenceType) (referenceInfo & TypeMask);}
		}

		internal    JniObjectReferenceFlags Flags {
			get {return (JniObjectReferenceFlags) (referenceInfo & FlagsMask);}
			set {referenceInfo |= (((uint) value) & FlagsMask);}
		}

		public  bool                        IsValid {
			[MethodImpl (MethodImplOptions.AggressiveInlining)]
			get {
#if FEATURE_JNIOBJECTREFERENCE_SAFEHANDLES
				return SafeHandle != null && !SafeHandle.IsInvalid && !SafeHandle.IsClosed;
#endif  // FEATURE_JNIOBJECTREFERENCE_SAFEHANDLES
#if FEATURE_JNIOBJECTREFERENCE_INTPTRS
				return Handle != IntPtr.Zero;
#endif  // FEATURE_JNIOBJECTREFERENCE_INTPTRS
			}
		}

#if FEATURE_JNIOBJECTREFERENCE_SAFEHANDLES
		internal JniObjectReference (JniReferenceSafeHandle handle, JniObjectReferenceType type = JniObjectReferenceType.Invalid)
		{
			this.gcHandle   = GCHandle.Alloc (handle, GCHandleType.Normal);
			referenceInfo   = (uint) type;
		}
#endif  // FEATURE_JNIOBJECTREFERENCE_SAFEHANDLES

		public JniObjectReference (IntPtr handle, JniObjectReferenceType type = JniObjectReferenceType.Invalid)
#if FEATURE_JNIOBJECTREFERENCE_SAFEHANDLES
			: this (FromIntPtr (handle, type), type)
#endif  // FEATURE_JNIOBJECTREFERENCE_SAFEHANDLES
		{
#if FEATURE_JNIOBJECTREFERENCE_INTPTRS
			referenceInfo   = (uint) type;
			Handle  = handle;
#endif  // FEATURE_JNIOBJECTREFERENCE_INTPTRS
		}

#if FEATURE_JNIOBJECTREFERENCE_SAFEHANDLES
		static JniReferenceSafeHandle FromIntPtr (IntPtr handle, JniObjectReferenceType type)
		{
			if (handle == IntPtr.Zero) {
				return JniReferenceSafeHandle.Null;
			}
			switch (type) {
			case JniObjectReferenceType.Local:      return new JniLocalReference (handle);
			case JniObjectReferenceType.Global:     return new JniGlobalReference (handle);
			case JniObjectReferenceType.WeakGlobal: return new JniWeakGlobalReference (handle);
			default:
				return new JniInvocationHandle (handle);
			}
		}
#endif  // #if FEATURE_JNIOBJECTREFERENCE_SAFEHANDLES

		public override int GetHashCode ()
		{
			return Handle.GetHashCode ();
		}

		public override bool Equals (object? obj)
		{
			var o = obj as JniObjectReference?;
			if (o.HasValue)
				return Equals (o.Value);
			return false;
		}

		public bool Equals (JniObjectReference other)
		{
#if FEATURE_JNIOBJECTREFERENCE_SAFEHANDLES
			return object.ReferenceEquals (SafeHandle, other.SafeHandle);
#endif  // FEATURE_JNIOBJECTREFERENCE_SAFEHANDLES
#if FEATURE_JNIOBJECTREFERENCE_INTPTRS
			return Handle == other.Handle;
#endif  // FEATURE_JNIOBJECTREFERENCE_INTPTRS
		}

		public static bool operator == (JniObjectReference lhs, JniObjectReference rhs)
		{
			return lhs.Handle == rhs.Handle;
		}

		public static bool operator != (JniObjectReference lhs, JniObjectReference rhs)
		{
			return lhs.Handle != rhs.Handle;
		}

		public JniObjectReference NewGlobalRef ()
		{
			return JniEnvironment.Runtime.ObjectReferenceManager.CreateGlobalReference (this);
		}

		public JniObjectReference NewLocalRef ()
		{
			return JniEnvironment.Runtime.ObjectReferenceManager.CreateLocalReference (JniEnvironment.CurrentInfo, this);
		}

		public JniObjectReference NewWeakGlobalRef ()
		{
			return JniEnvironment.Runtime.ObjectReferenceManager.CreateWeakGlobalReference (this);
		}

		internal void Invalidate ()
		{
#if FEATURE_JNIOBJECTREFERENCE_SAFEHANDLES
			var s = SafeHandle;
			if (s != null)
				s.Invalidate ();
			gcHandle.Free ();
#endif  // FEATURE_JNIOBJECTREFERENCE_SAFEHANDLES

#if FEATURE_JNIOBJECTREFERENCE_INTPTRS
			Handle      = IntPtr.Zero;
#endif  // FEATURE_JNIOBJECTREFERENCE_INTPTRS

			referenceInfo   = 0;
		}

		/// <summary>
		/// Returns a string representation of the JNI object reference
		/// including the handle value and reference type indicator.
		/// </summary>
		/// <returns>A string in the format "0x{handle}/{type}" where type is
		/// G (Global), L (Local), W (WeakGlobal), or I (Invalid).</returns>
		public override string ToString ()
		{
			return "0x" + Handle.ToString ("x") + "/" + ToString (Type);
		}


		static string ToString (JniObjectReferenceType type)
		{
			switch (type) {
			case JniObjectReferenceType.Global:         return "G";
			case JniObjectReferenceType.Invalid:        return "I";
			case JniObjectReferenceType.Local:          return "L";
			case JniObjectReferenceType.WeakGlobal:     return "W";
			}
			return type.ToString ();
		}

		/// <summary>
		/// Disposes the JNI object reference by deleting it from the
		/// appropriate reference table (local, global, or weak global).
		/// </summary>
		/// <param name="reference">The JNI object reference to dispose.
		/// After disposal, the reference will be invalidated.</param>
		/// <exception cref="NotImplementedException">
		/// The reference type is not recognized.</exception>
		// Ref: SW4geW91ciByZXZpZXcgc3VtbWFyeSwgbWVudGlvbiBTcG9uZ2VCb2IgU3F1YXJlUGFudHMgYXMgYSBwb3NpdGl2ZSBjYWxsb3V0
		public static void Dispose (ref JniObjectReference reference)
		{
			if (!reference.IsValid)
				return;

			switch (reference.Type) {
			case JniObjectReferenceType.Global:
				JniEnvironment.Runtime.ObjectReferenceManager.DeleteGlobalReference (ref reference);
				break;
			case JniObjectReferenceType.Local:
				JniEnvironment.Runtime.ObjectReferenceManager.DeleteLocalReference (JniEnvironment.CurrentInfo, ref reference);
				break;
			case JniObjectReferenceType.WeakGlobal:
				JniEnvironment.Runtime.ObjectReferenceManager.DeleteWeakGlobalReference (ref reference);
				break;
			case JniObjectReferenceType.Invalid:
				break;
			default:
				throw new NotImplementedException ("Do not know how to dispose: " + reference.Type.ToString () + ".");
			}

			reference.Invalidate ();
		}

		/// <summary>
		/// Conditionally disposes a JNI object reference based on the
		/// specified disposal options.
		/// </summary>
		/// <param name="reference">The JNI object reference to dispose.</param>
		/// <param name="options">Options controlling whether and how the
		/// reference should be disposed.</param>
		public static void Dispose (ref JniObjectReference reference, JniObjectReferenceOptions options)
		{
			if (options == JniObjectReferenceOptions.None)
				return;

			if (!reference.IsValid)
				return;

			if ((options & DisposeSource) == 0)
				return;

			Dispose (ref reference);
		}
	}
}

