namespace Example;

using System;
using Java.Interop;

[JniTypeSignature (JniTypeName)]
class ManagedType : Java.Lang.Object {
	internal const string JniTypeName = "example/ManagedType";

	public ManagedType (int value)
	{
		this.value = value;
	}

	int value;

	public Java.Lang.String GetString ()
	{
		return new Java.Lang.String ($"Hello from C#, via Java.Interop! Value={value}");
	}

	delegate IntPtr _JniMarshal_PP_L (IntPtr jnienv, IntPtr n_self);

	static Delegate GetGetStringHandler ()
	{
		return new _JniMarshal_PP_L (n_GetString);
	}

	static IntPtr n_GetString (IntPtr jnienv, IntPtr n_self)
	{
		var r_self = new JniObjectReference (n_self);
		var self = JniEnvironment.Runtime.ValueManager.GetValue<ManagedType> (ref r_self, JniObjectReferenceOptions.CopyAndDoNotRegister);
		try {
			var result = self!.GetString ();
			var r = result.PeerReference.NewLocalRef ();
			return JniEnvironment.References.NewReturnToJniRef (r);
		} finally {
			self?.DisposeUnlessReferenced ();
		}
	}

	[JniAddNativeMethodRegistration]
	static void RegisterNativeMembers (JniNativeMethodRegistrationArguments args)
	{
		args.AddRegistrations (new [] {
			new JniNativeMethodRegistration ("n_GetString", "()Ljava/lang/String;", new _JniMarshal_PP_L (n_GetString)),
		});
	}
}
