using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Xamarin.Test.Invalidnames {

	// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test.invalidnames']/class[@name='in']"
	[global::Android.Runtime.Register ("xamarin/test/invalidnames/in", DoNotGenerateAcw=true)]
	public partial class In : global::Java.Lang.Object {

		static readonly JniPeerMembers _members = new JniPeerMembers ("xamarin/test/invalidnames/in", typeof (In));
		internal static new IntPtr class_ref {
			get {
				return _members.JniPeerType.PeerReference.Handle;
			}
		}

		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return _members; }
		}

		protected override IntPtr ThresholdClass {
			get { return _members.JniPeerType.PeerReference.Handle; }
		}

		protected override global::System.Type ThresholdType {
			get { return _members.ManagedPeerType; }
		}

		protected In (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

	}
}