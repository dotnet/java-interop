using Java.Interop.Localization;

namespace Xamarin.Android.Tools.JniMarshalMethodGenerator
{
	class Message
	{
		public string Localized { get; private set; }

		private Message (string message) => Localized = message;

		public static Message ErrorUnableToPreloadReference = new Message (Resources.JniMarshalMethodGen_JM4001);
		public static Message ErrorAtLeastOneAssembly = new Message (Resources.JniMarshalMethodGen_JM4002);
		public static Message ErrorUnableToCreateJavaVM = new Message (Resources.JniMarshalMethodGen_JM4003);
		public static Message ErrorUnableToReadProfile = new Message (Resources.JniMarshalMethodGen_JM4004);
		public static Message ErrorPathDoesNotExist = new Message (Resources.JniMarshalMethodGen_JM4005);
		public static Message ErrorUnableToProcessAssembly = new Message (Resources.JniMarshalMethodGen_JM4006);

		public static Message WarningCouldntFindInterface = new Message (Resources.JniMarshalMethodGen_JM8001);
		public static Message WarningUnableToReadWithSymbols = new Message (Resources.JniMarshalMethodGen_JM8002);
		public static Message WarningTypeLoadException = new Message (Resources.JniMarshalMethodGen_JM8003);
		public static Message WarningUnableToFindTypeDefinition = new Message (Resources.JniMarshalMethodGen_JM8004);
		public static Message WarningMarshalMethodsTypeAlreadyExists = new Message (Resources.JniMarshalMethodGen_JM8005);
		public static Message WarningUnableToFindMethodDefinition = new Message (Resources.JniMarshalMethodGen_JM8006);
		public static Message WarningUnableToFindSCWriteLine = new Message (Resources.JniMarshalMethodGen_JM8007);
		public static Message WarningNoTypeWasMovedNothingToWrite = new Message (Resources.JniMarshalMethodGen_JM8008);
		public static Message WarningMethodWasNotImproved = new Message (Resources.JniMarshalMethodGen_JM8009);
	}
}
