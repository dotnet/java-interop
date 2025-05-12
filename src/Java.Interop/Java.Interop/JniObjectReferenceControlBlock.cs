using System;
using System.Runtime.InteropServices;

namespace Java.Interop;

internal struct JniObjectReferenceControlBlock {
	public	IntPtr  handle;
	public  int     handle_type;
	public  IntPtr  weak_handle;
	public  int     refs_added;

	public  static  readonly    int Size    = Marshal.SizeOf<JniObjectReferenceControlBlock>();

	public static unsafe JniObjectReferenceControlBlock* Alloc ()
	{
		return (JniObjectReferenceControlBlock*)NativeMemory.AllocZeroed (1, (uint) Size);
	}

	public static unsafe void Free (ref JniObjectReferenceControlBlock* value)
	{
		if (value == null) {
			return;
		}
		NativeMemory.Free (value);
		value   = null;
	}
}
