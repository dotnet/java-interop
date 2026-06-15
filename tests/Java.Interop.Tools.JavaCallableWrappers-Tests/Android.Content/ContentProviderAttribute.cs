using System;

namespace Android.Content {

	partial sealed class ContentProviderAttribute : Attribute, Java.Interop.IJniNameProviderAttribute {
		public string Name { get; set; }
	}
}
