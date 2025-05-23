#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

using Java.Interop;

using NUnit.Framework;

namespace Java.InteropTests {

	// Android doesn't support `[NonParallelizable]`, but runs tests sequentially by default.
#if !__ANDROID__
	// Modifies JniRuntime.valueManager instance field; can't be done in parallel
	[NonParallelizable]
#endif  // !__ANDROID__
	public abstract class JniRuntimeJniValueManagerContract : JavaVMFixture {

		[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
		protected abstract Type ValueManagerType {
			get;
		}

		protected virtual JniRuntime.JniValueManager CreateValueManager ()
		{
			var manager = Activator.CreateInstance (ValueManagerType) as JniRuntime.JniValueManager;
			return manager ?? throw new InvalidOperationException ($"Could not create instance of `{ValueManagerType}`!");
		}

#pragma warning disable CS8618
		JniRuntime.JniValueManager  systemManager;
		JniRuntime.JniValueManager  valueManager;
#pragma warning restore CS8618

		[SetUp]
		public void CreateVM ()
		{
			systemManager   = JniRuntime.CurrentRuntime.valueManager!;
			valueManager    = CreateValueManager ();
			valueManager.OnSetRuntime (JniRuntime.CurrentRuntime);
			JniRuntime.CurrentRuntime.valueManager  = valueManager;
		}

		[TearDown]
		public void DestroyVM ()
		{
			JniRuntime.CurrentRuntime.valueManager  = systemManager;
			systemManager   = null!;
			valueManager?.Dispose ();
			valueManager    = null!;
		}

		[Test]
		public void AddPeer ()
		{
		}

		int GetSurfacedPeersCount ()
		{
			return valueManager.GetSurfacedPeers ().Count;
		}

		[Test]
		public void AddPeer_NoDuplicates ()
		{
			int startPeerCount  = GetSurfacedPeersCount ();
			using (var v = new MyDisposableObject ()) {
				// MyDisposableObject ctor implicitly calls AddPeer();
				Assert.AreEqual (startPeerCount + 1, GetSurfacedPeersCount (), DumpPeers ());
				valueManager.AddPeer (v);
				Assert.AreEqual (startPeerCount + 1, GetSurfacedPeersCount (), DumpPeers ());
			}
		}

		[Test]
		public void ConstructPeer_ImplicitViaBindingConstructor_PeerIsInSurfacedPeers ()
		{
			int startPeerCount  = GetSurfacedPeersCount ();

			var g               = new GetThis ();
			var surfaced        = valueManager.GetSurfacedPeers ();
			Assert.AreEqual (startPeerCount + 1, surfaced.Count);

			var found           = false;
			foreach (var pr in surfaced) {
				if (!pr.SurfacedPeer.TryGetTarget (out var p))
					continue;
				if (object.ReferenceEquals (g, p)) {
					found = true;
				}
			}
			Assert.IsTrue (found);

			var localRef        = g.PeerReference.NewLocalRef ();
			g.Dispose ();
			Assert.AreEqual (startPeerCount, GetSurfacedPeersCount ());
			Assert.IsNull (valueManager.PeekPeer (localRef));
			JniObjectReference.Dispose (ref localRef);
		}

		[Test]
		public void ConstructPeer_ImplicitViaBindingMethod_PeerIsInSurfacedPeers ()
		{
			int startPeerCount  = GetSurfacedPeersCount ();

			var g               = new GetThis ();
			var surfaced        = valueManager.GetSurfacedPeers ();
			Assert.AreEqual (startPeerCount + 1, surfaced.Count);

			var found           = false;
			foreach (var pr in surfaced) {
				if (!pr.SurfacedPeer.TryGetTarget (out var p))
					continue;
				if (object.ReferenceEquals (g, p)) {
					found = true;
				}
			}
			Assert.IsTrue (found);

			var localRef        = g.PeerReference.NewLocalRef ();
			g.Dispose ();
			Assert.AreEqual (startPeerCount, GetSurfacedPeersCount ());
			Assert.IsNull (valueManager.PeekPeer (localRef));
			JniObjectReference.Dispose (ref localRef);
		}


		[Test]
		public void CollectPeers ()
		{
			// TODO
		}

		[Test]
		public void CreatePeer_InvalidHandleReturnsNull ()
		{
			var r = new JniObjectReference ();
			var o = valueManager.CreatePeer (ref r, JniObjectReferenceOptions.Copy, null);
			Assert.IsNull (o);
		}

		[Test]
		public unsafe void CreatePeer_UsesFallbackType ()
		{
			using var t = new JniType (AnotherJavaInterfaceImpl.JniTypeName);

			var ctor    = t.GetConstructor ("()V");
			var lref    = t.NewObject (ctor, null);

			using var p = valueManager.CreatePeer (ref lref, JniObjectReferenceOptions.CopyAndDispose, typeof (IJavaInterface));

			Assert.IsFalse (lref.IsValid);  // .CopyAndDispose disposes

			Assert.IsNotNull (p);
			Assert.AreSame (typeof (IJavaInterfaceInvoker), p!.GetType ());
		}

		[Test]
		public void CreatePeer_CreatesNewValueUsingActivationConstructor ()
		{
			using var v1    = new AnotherJavaInterfaceImpl ();
			var lref        = v1.PeerReference.NewLocalRef ();
			try {
				using var v2    = valueManager.CreatePeer (ref lref, JniObjectReferenceOptions.CopyAndDispose, typeof (AnotherJavaInterfaceImpl));
				Assert.AreNotSame (v1, v2, "CreatePeer() should create new values");
			}
			finally {
				JniObjectReference.Dispose (ref lref);
			}
		}

		[Test]
		public void CreatePeer_ThrowsIfNoActivationConstructorPresent ()
		{
			using var v1    = new GetThis ();
			var lref        = v1.PeerReference.NewLocalRef ();
			var ex = Assert.Throws<NotSupportedException> (
					() => valueManager.CreatePeer (ref lref, JniObjectReferenceOptions.CopyAndDispose, typeof (GetThis)),
					$"`GetThis` has no activation constructor, so attempting to use it should throw NotSupportedException.");
			Assert.IsTrue (lref.IsValid, "lref should still be valid");
			JniObjectReference.Dispose (ref lref);
		}

		[Test]
		public void CreatePeer_ReplaceableDoesNotReplace ()
		{
			var v       = new AnotherJavaInterfaceImpl ();
			var lref    = v.PeerReference.NewLocalRef ();
			v.Dispose ();

			try {
				Assert.IsNull (valueManager.PeekPeer (lref), "v.Dispose() should have unregistered the peer.");
				var peer1   = valueManager.CreatePeer (ref lref, JniObjectReferenceOptions.Copy, typeof (AnotherJavaInterfaceImpl));
				Assert.IsTrue (
						peer1!.JniManagedPeerState.HasFlag (JniManagedPeerStates.Replaceable),
						$"Expected peer1.JniManagedPeerState to have .Replaceable, but was {peer1.JniManagedPeerState}.");
				Assert.AreSame (peer1, valueManager.PeekPeer (lref),
						$"Expected peer1==PeekValue(peer1.PeerReference); it's the only one that should exist!");

				var peer2   = valueManager.CreatePeer (ref lref, JniObjectReferenceOptions.Copy, typeof (AnotherJavaInterfaceImpl));
				Assert.IsTrue (
						peer2!.JniManagedPeerState.HasFlag (JniManagedPeerStates.Replaceable),
						$"Expected peer2.JniManagedPeerState to have .Replaceable, but was {peer2.JniManagedPeerState}.");
				Assert.AreNotSame (peer1, peer2, "Expected peer1 and peer2 to be different instances.");

				var peeked  = valueManager.PeekPeer (lref);
				Assert.AreSame (peer1, peeked,
						"Expected peer1 and peeked to be the same instance; " +
						$"peeked={RuntimeHelpers.GetHashCode (peeked).ToString ("x")}, " +
						$"peer1={RuntimeHelpers.GetHashCode (peer1).ToString ("x")}, " +
						$"peer2={RuntimeHelpers.GetHashCode (peer2).ToString ("x")}");
			} finally {
				JniObjectReference.Dispose (ref lref);
			}
		}

		[Test]
		public void CreateValue ()
		{
			using (var o = new JavaObject ()) {
				var r = o.PeerReference;
				var x = (IJavaPeerable) valueManager.CreateValue (ref r, JniObjectReferenceOptions.Copy)!;
				Assert.AreNotSame (o, x);
				x.Dispose ();

				x = valueManager.CreateValue<IJavaPeerable> (ref r, JniObjectReferenceOptions.Copy);
				Assert.AreNotSame (o, x);
				x!.Dispose ();
			}
		}

		[Test]
		public void GetValue_ReturnsAlias ()
		{
			var local   = new JavaObject ();
			local.UnregisterFromRuntime ();
			Assert.IsNull (valueManager.PeekValue (local.PeerReference));
			// GetObject must always return a value (unless handle is null, etc.).
			// However, since we called local.UnregisterFromRuntime(),
			// JniRuntime.PeekObject() is null (asserted above), but GetObject() must
			// **still** return _something_.
			// In this case, it returns an _alias_.
			// TODO: "most derived type" alias generation. (Not relevant here, but...)
			var p       = local.PeerReference;
			var alias   = JniRuntime.CurrentRuntime.ValueManager.GetValue<IJavaPeerable> (ref p, JniObjectReferenceOptions.Copy);
			Assert.AreNotSame (local, alias);
			alias!.Dispose ();
			local.Dispose ();
		}

		[Test]
		public void GetValue_ReturnsNullWithNullHandle ()
		{
			var r = new JniObjectReference ();
			var o = valueManager.GetValue (ref r, JniObjectReferenceOptions.Copy);
			Assert.IsNull (o);
		}

		[Test]
		public void GetValue_ReturnsNullWithInvalidSafeHandle ()
		{
			var invalid = new JniObjectReference ();
			Assert.IsNull (valueManager.GetValue (ref invalid, JniObjectReferenceOptions.CopyAndDispose));
		}

		[Test]
		public unsafe void GetValue_FindBestMatchType ()
		{
#if !NO_MARSHAL_MEMBER_BUILDER_SUPPORT
			using (var t = new JniType (TestType.JniTypeName)) {
				var c = t.GetConstructor ("()V");
				var o = t.NewObject (c, null);
				using (var w = valueManager.GetValue<IJavaPeerable> (ref o, JniObjectReferenceOptions.CopyAndDispose)) {
					Assert.AreEqual (typeof (TestType), w!.GetType ());
					Assert.IsTrue (((TestType) w).ExecutedActivationConstructor);
				}
			}
#endif  // !NO_MARSHAL_MEMBER_BUILDER_SUPPORT
		}

		[Test]
		public void PeekPeer ()
		{
			Assert.IsNull (valueManager.PeekPeer (new JniObjectReference ()));

			using (var v = new MyDisposableObject ()) {
				Assert.IsNotNull (valueManager.PeekPeer (v.PeerReference));
				Assert.AreSame (v, valueManager.PeekPeer (v.PeerReference));
			}
		}

		[Test]
		public void PeekValue ()
		{
			JniObjectReference lref;
			using (var o = new JavaObject ()) {
				lref = o.PeerReference.NewLocalRef ();
				Assert.AreSame (o, valueManager.PeekValue (lref));
			}
			// At this point, the Java-side object is kept alive by `lref`,
			// but the wrapper instance has been disposed, and thus should
			// be unregistered, and thus unfindable.
			Assert.IsNull (valueManager.PeekValue (lref));
			JniObjectReference.Dispose (ref lref);
		}

		[Test]
		public void PeekValue_BoxedObjects ()
		{
			var marshaler   = valueManager.GetValueMarshaler<object> ();
			var ad          = AppDomain.CurrentDomain;

			var proxy       = marshaler.CreateGenericArgumentState (ad);
			Assert.AreSame (ad, valueManager.PeekValue (proxy.ReferenceValue));
			marshaler.DestroyGenericArgumentState (ad, ref proxy);

			var ex  = new InvalidOperationException ("boo!");
			proxy   = marshaler.CreateGenericArgumentState (ex);
			Assert.AreSame (ex, valueManager.PeekValue (proxy.ReferenceValue));
			marshaler.DestroyGenericArgumentState (ex, ref proxy);
		}

		void AllNestedRegistrationScopeTests ()
		{
			AddPeer ();
			AddPeer_NoDuplicates ();
			ConstructPeer_ImplicitViaBindingConstructor_PeerIsInSurfacedPeers ();
			CreateValue ();
			GetValue_FindBestMatchType ();
			GetValue_ReturnsAlias ();
			GetValue_ReturnsNullWithInvalidSafeHandle ();
			GetValue_ReturnsNullWithNullHandle ();
			PeekPeer ();
			PeekValue ();
			PeekValue_BoxedObjects ();
		}

		string DumpPeers ()
		{
			return DumpPeers (valueManager.GetSurfacedPeers ());
		}

		static string DumpPeers (IEnumerable<JniSurfacedPeerInfo> peers)
		{
			return string.Join ("," + Environment.NewLine, peers);
		}


		// also test:
		// Singleton scenario
		// Types w/o "activation" constructors -- need to support checking parent scopes
		// nesting of scopes
		// Adding an instance already added in a previous scope?
	}

	public abstract class JniRuntimeJniValueManagerContract<
			[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
			T
		> : JniRuntimeJniValueManagerContract
	{
		[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
		protected override Type ValueManagerType => typeof (T);
	}

#if !__ANDROID__
#if !NETCOREAPP
	[TestFixture]
	public class JniRuntimeJniValueManagerContract_Mono : JniRuntimeJniValueManagerContract {
		static Type MonoRuntimeValueManagerType = Type.GetType ("Java.Interop.MonoRuntimeValueManager, Java.Runtime.Environment", throwOnError:true)!;

		protected override Type ValueManagerType => MonoRuntimeValueManagerType;
	}
#endif	// !NETCOREAPP

	[TestFixture]
	public class JniRuntimeJniValueManagerContract_NoGCIntegration : JniRuntimeJniValueManagerContract {
		[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
		static Type ManagedValueManagerType = Type.GetType ("Java.Interop.ManagedValueManager, Java.Runtime.Environment", throwOnError:true)!;

		[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
		protected override Type ValueManagerType => ManagedValueManagerType;
	}
#endif  // !__ANDROID__

	// Note: Java side implements JavaInterface, while managed binding DOES NOT.
	// This is so that `CreatePeer(…, typeof(IJavaInterface))` tests don't use an existing AnotherJavaInterfaceImpl instance.
	//
	// This is mostly identical to MyJavaInterfaceImpl; the important difference is that
	// it contains an activation constructor, while MyJavaInterfaceImpl does not.
	// MyJavaInterfaceImpl can't have one, as that's what provokes the NotSupportedException in the JavaAs() tests.
	//
	// We want one here so that in "bad" `CreatePeer()` implementations, we'll find this peer and construct it
	// before verifying that it satisfies the targetType requirement.
	[JniTypeSignature (JniTypeName, GenerateJavaPeer=false)]
	public class AnotherJavaInterfaceImpl : JavaObject {
		internal            const       string          JniTypeName    = "net/dot/jni/test/AnotherJavaInterfaceImpl";

		internal    static  readonly    JniPeerMembers  _members    = new JniPeerMembers (JniTypeName, typeof (AnotherJavaInterfaceImpl));

		public override JniPeerMembers JniPeerMembers {
			get {return _members;}
		}

		AnotherJavaInterfaceImpl (ref JniObjectReference reference, JniObjectReferenceOptions options)
			: base (ref reference, options)
		{
		}

		public unsafe AnotherJavaInterfaceImpl ()
			: base (ref *InvalidJniObjectReference, JniObjectReferenceOptions.None)
		{
			const   string  id  = "()V";
			var peer = _members.InstanceMethods.StartCreateInstance (id, GetType (), null);
			Construct (ref peer, JniObjectReferenceOptions.CopyAndDispose);
			_members.InstanceMethods.FinishCreateInstance (id, this, null);
		}
	}
}
