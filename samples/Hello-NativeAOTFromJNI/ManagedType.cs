namespace Example;

using Java.Interop;

[JniTypeSignature ("example/ManagedType")]
class ManagedType : Java.Lang.Object {

	public ManagedType ()
	{
	}

	[JavaCallable ("getString")]
	public Java.Lang.String GetString ()
	{
		return new Java.Lang.String ("Hello from C#, via Java.Interop!");
	}
}
