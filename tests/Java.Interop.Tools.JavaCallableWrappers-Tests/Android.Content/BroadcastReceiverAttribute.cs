using System;

namespace Android.Content {

	partial sealed class BroadcastReceiverAttribute : Attribute, Java.Interop.IJniNameProviderAttribute {
		public string Name { get; set; }
	}
}
