using System;
using System.Runtime.InteropServices;

namespace Java.Interop;

internal struct JniObjectReferenceControlBlock {
	public	IntPtr  handle;
	public  int     handle_type;
	public  IntPtr  weak_handle;
	public  int     refs_added;

	public  static  readonly    int Size    = Marshal.SizeOf<JniObjectReferenceControlBlock>();

	public  static  unsafe  IntPtr Alloc () =>
		(IntPtr) NativeMemory.AllocZeroed (1, (uint) Size);

	public  static  unsafe  void Free (ref IntPtr value)
	{
		if (value == 0) {
			return;
		}
		NativeMemory.Free ((void*) value);
		value   = IntPtr.Zero;
	}
}
