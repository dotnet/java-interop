//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#nullable restore
using System;
using System.Collections.Generic;
using Java.Interop;

namespace Xamarin.Test {

	// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test']/class[@name='SomeObject']"
	[global::Java.Interop.JniTypeSignature ("xamarin/test/SomeObject", GenerateJavaPeer=false)]
	public partial class SomeObject : global::Java.Lang.Object {

		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='SomeObject']/field[@name='myStrings']"
		public global::Java.Util.IList MyStrings {
			get {
				const string __id = "myStrings.Ljava/util/List;";

				var __v = _members.InstanceFields.GetObjectValue (__id, this);
				return global::Java.Interop.JniEnvironment.Runtime.ValueManager.GetValue<global::Java.Util.IList >(ref __v, JniObjectReferenceOptions.Copy);
			}
			set {
				const string __id = "myStrings.Ljava/util/List;";

				try {
					_members.InstanceFields.SetValue (__id, this, value?.PeerReference ?? default);
				} finally {
					GC.KeepAlive (value);
				}
			}
		}


		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='SomeObject']/field[@name='myInts']"
		public global::Java.Util.IList MyInts {
			get {
				const string __id = "myInts.Ljava/util/List;";

				var __v = _members.InstanceFields.GetObjectValue (__id, this);
				return global::Java.Interop.JniEnvironment.Runtime.ValueManager.GetValue<global::Java.Util.IList >(ref __v, JniObjectReferenceOptions.Copy);
			}
			set {
				const string __id = "myInts.Ljava/util/List;";

				try {
					_members.InstanceFields.SetValue (__id, this, value?.PeerReference ?? default);
				} finally {
					GC.KeepAlive (value);
				}
			}
		}


		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='SomeObject']/field[@name='mybools']"
		public global::Java.Util.IList Mybools {
			get {
				const string __id = "mybools.Ljava/util/List;";

				var __v = _members.InstanceFields.GetObjectValue (__id, this);
				return global::Java.Interop.JniEnvironment.Runtime.ValueManager.GetValue<global::Java.Util.IList >(ref __v, JniObjectReferenceOptions.Copy);
			}
			set {
				const string __id = "mybools.Ljava/util/List;";

				try {
					_members.InstanceFields.SetValue (__id, this, value?.PeerReference ?? default);
				} finally {
					GC.KeepAlive (value);
				}
			}
		}


		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='SomeObject']/field[@name='myObjects']"
		public global::Java.Util.IList MyObjects {
			get {
				const string __id = "myObjects.Ljava/util/List;";

				var __v = _members.InstanceFields.GetObjectValue (__id, this);
				return global::Java.Interop.JniEnvironment.Runtime.ValueManager.GetValue<global::Java.Util.IList >(ref __v, JniObjectReferenceOptions.Copy);
			}
			set {
				const string __id = "myObjects.Ljava/util/List;";

				try {
					_members.InstanceFields.SetValue (__id, this, value?.PeerReference ?? default);
				} finally {
					GC.KeepAlive (value);
				}
			}
		}


		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='SomeObject']/field[@name='myfloats']"
		public global::Java.Util.IList Myfloats {
			get {
				const string __id = "myfloats.Ljava/util/List;";

				var __v = _members.InstanceFields.GetObjectValue (__id, this);
				return global::Java.Interop.JniEnvironment.Runtime.ValueManager.GetValue<global::Java.Util.IList >(ref __v, JniObjectReferenceOptions.Copy);
			}
			set {
				const string __id = "myfloats.Ljava/util/List;";

				try {
					_members.InstanceFields.SetValue (__id, this, value?.PeerReference ?? default);
				} finally {
					GC.KeepAlive (value);
				}
			}
		}


		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='SomeObject']/field[@name='mydoubles']"
		public global::Java.Util.IList Mydoubles {
			get {
				const string __id = "mydoubles.Ljava/util/List;";

				var __v = _members.InstanceFields.GetObjectValue (__id, this);
				return global::Java.Interop.JniEnvironment.Runtime.ValueManager.GetValue<global::Java.Util.IList >(ref __v, JniObjectReferenceOptions.Copy);
			}
			set {
				const string __id = "mydoubles.Ljava/util/List;";

				try {
					_members.InstanceFields.SetValue (__id, this, value?.PeerReference ?? default);
				} finally {
					GC.KeepAlive (value);
				}
			}
		}


		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='SomeObject']/field[@name='mylongs']"
		public global::Java.Util.IList Mylongs {
			get {
				const string __id = "mylongs.Ljava/util/List;";

				var __v = _members.InstanceFields.GetObjectValue (__id, this);
				return global::Java.Interop.JniEnvironment.Runtime.ValueManager.GetValue<global::Java.Util.IList >(ref __v, JniObjectReferenceOptions.Copy);
			}
			set {
				const string __id = "mylongs.Ljava/util/List;";

				try {
					_members.InstanceFields.SetValue (__id, this, value?.PeerReference ?? default);
				} finally {
					GC.KeepAlive (value);
				}
			}
		}

		static readonly JniPeerMembers _members = new JniPeerMembers ("xamarin/test/SomeObject", typeof (SomeObject));

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return _members; }
		}

		protected SomeObject (ref JniObjectReference reference, JniObjectReferenceOptions options) : base (ref reference, options)
		{
		}

	}
}
