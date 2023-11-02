namespace Example;

using Java.Interop;

[JniTypeSignature ("example/ManagedType")]
class ManagedType : Java.Lang.Object {

	public ManagedType ()
	{
	}

	// TODO: remove this
	public ManagedType (ref JniObjectReference reference, JniObjectReferenceOptions options)
		: base (ref reference, options)
	{
	}

	[JavaCallable ("getString")]
	public Java.Lang.String GetString ()
	{
		return new Java.Lang.String ("Hello from C#, via Java.Interop!");
	}
}
