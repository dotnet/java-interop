using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Java.Interop {

	class ManagedValueManager : JniRuntime.JniValueManager {

		Dictionary<int, List<IJavaPeerable>>?   RegisteredInstances = new Dictionary<int, List<IJavaPeerable>>();

		public override void WaitForGCBridgeProcessing ()
		{
		}

		public override bool CanCollectPeers => false;
		public override bool CanReleasePeers => true;

		public override void CollectPeers ()
		{
			if (RegisteredInstances == null)
				throw new ObjectDisposedException (nameof (ManagedValueManager));

			throw new NotSupportedException ();
		}

		public override void ReleasePeers ()
		{
			if (RegisteredInstances == null)
				throw new ObjectDisposedException (nameof (ManagedValueManager));

			var peers = new List<IJavaPeerable> ();

			lock (RegisteredInstances) {
				RegisteredInstances.Clear ();
			}
		}

		public override bool SupportsPeerableRegistrationScopes => true;

		public override void AddPeer (IJavaPeerable value)
		{
			if (RegisteredInstances == null)
				throw new ObjectDisposedException (nameof (ManagedValueManager));

			var r = value.PeerReference;
			if (!r.IsValid)
				throw new ObjectDisposedException (value.GetType ().FullName);
			var o = PeekPeer (value.PeerReference);
			if (o != null)
				return;

			if (r.Type != JniObjectReferenceType.Global) {
				value.SetPeerReference (r.NewGlobalRef ());
				JniObjectReference.Dispose (ref r, JniObjectReferenceOptions.CopyAndDispose);
			}

#pragma warning disable JI9999
			if (TryAddPeerToRegistrationScope (value)) {
				return;
			}
#pragma warning restore JI9999

			int key = value.JniIdentityHashCode;
			lock (RegisteredInstances) {
				List<IJavaPeerable>? peers;
				if (!RegisteredInstances.TryGetValue (key, out peers)) {
					peers = new List<IJavaPeerable> () {
						value,
					};
					RegisteredInstances.Add (key, peers);
					return;
				}

				for (int i = peers.Count - 1; i >= 0; i--) {
					var p   = peers [i];
					if (!JniEnvironment.Types.IsSameObject (p.PeerReference, value.PeerReference))
						continue;
					if (Replaceable (p)) {
						peers [i] = value;
					} else {
						WarnNotReplacing (key, value, p);
					}
					return;
				}
				peers.Add (value);
			}
		}

		static bool Replaceable (IJavaPeerable peer)
		{
			if (peer == null)
				return true;
			return (peer.JniManagedPeerState & JniManagedPeerStates.Replaceable) == JniManagedPeerStates.Replaceable;
		}

		void WarnNotReplacing (int key, IJavaPeerable ignoreValue, IJavaPeerable keepValue)
		{
			Runtime.ObjectReferenceManager.WriteGlobalReferenceLine (
					"Warning: Not registering PeerReference={0} IdentityHashCode=0x{1} Instance={2} Instance.Type={3} Java.Type={4}; " +
					"keeping previously registered PeerReference={5} Instance={6} Instance.Type={7} Java.Type={8}.",
					ignoreValue.PeerReference.ToString (),
					key.ToString ("x"),
					RuntimeHelpers.GetHashCode (ignoreValue).ToString ("x"),
					ignoreValue.GetType ().FullName,
					JniEnvironment.Types.GetJniTypeNameFromInstance (ignoreValue.PeerReference),
					keepValue.PeerReference.ToString (),
					RuntimeHelpers.GetHashCode (keepValue).ToString ("x"),
					keepValue.GetType ().FullName,
					JniEnvironment.Types.GetJniTypeNameFromInstance (keepValue.PeerReference));
		}

		public override IJavaPeerable? PeekPeer (JniObjectReference reference)
		{
			if (RegisteredInstances == null)
				throw new ObjectDisposedException (nameof (ManagedValueManager));

			if (!reference.IsValid)
				return null;
			
			int key = GetJniIdentityHashCode (reference);

#pragma warning disable JI9999
			var peer = TryPeekPeerFromRegistrationScopes (reference, key);
			if (peer != null) {
				return peer;
			}
#pragma warning restore JI9999

			lock (RegisteredInstances) {
				List<IJavaPeerable>? peers;
				if (!RegisteredInstances.TryGetValue (key, out peers))
					return null;

				for (int i = peers.Count - 1; i >= 0; i--) {
					var p = peers [i];
					if (JniEnvironment.Types.IsSameObject (reference, p.PeerReference))
						return p;
				}
				if (peers.Count == 0)
					RegisteredInstances.Remove (key);
			}
			return null;
		}

		public override void RemovePeer (IJavaPeerable value)
		{
			if (RegisteredInstances == null)
				throw new ObjectDisposedException (nameof (ManagedValueManager));

			if (value == null)
				throw new ArgumentNullException (nameof (value));

#pragma warning disable JI9999
			if (TryRemovePeerFromRegistrationScopes (value)) {
				return;
			}
#pragma warning restore JI9999

			int key = value.JniIdentityHashCode;
			lock (RegisteredInstances) {
				List<IJavaPeerable>? peers;
				if (!RegisteredInstances.TryGetValue (key, out peers))
					return;

				for (int i = peers.Count - 1; i >= 0; i--) {
					var p   = peers [i];
					if (object.ReferenceEquals (value, p)) {
						peers.RemoveAt (i);
					}
				}
				if (peers.Count == 0)
					RegisteredInstances.Remove (key);
			}
		}

		public override void FinalizePeer (IJavaPeerable value)
		{
			var h = value.PeerReference;
			var o = Runtime.ObjectReferenceManager;
			// MUST NOT use SafeHandle.ReferenceType: local refs are tied to a JniEnvironment
			// and the JniEnvironment's corresponding thread; it's a thread-local value.
			// Accessing SafeHandle.ReferenceType won't kill anything (so far...), but
			// instead it always returns JniReferenceType.Invalid.
			if (!h.IsValid || h.Type == JniObjectReferenceType.Local) {
				if (o.LogGlobalReferenceMessages) {
					o.WriteGlobalReferenceLine ("Finalizing PeerReference={0} IdentityHashCode=0x{1} Instance=0x{2} Instance.Type={3}",
							h.ToString (),
							value.JniIdentityHashCode.ToString ("x"),
							RuntimeHelpers.GetHashCode (value).ToString ("x"),
							value.GetType ().ToString ());
				}
				RemovePeer (value);
				value.SetPeerReference (new JniObjectReference ());
				value.Finalized ();
				return;
			}

			RemovePeer (value);
			if (o.LogGlobalReferenceMessages) {
				o.WriteGlobalReferenceLine ("Finalizing PeerReference={0} IdentityHashCode=0x{1} Instance=0x{2} Instance.Type={3}",
						h.ToString (),
						value.JniIdentityHashCode.ToString ("x"),
						RuntimeHelpers.GetHashCode (value).ToString ("x"),
						value.GetType ().ToString ());
			}
			value.SetPeerReference (new JniObjectReference ());
			JniObjectReference.Dispose (ref h);
			value.Finalized ();
		}

		public override void ActivatePeer (IJavaPeerable? self, JniObjectReference reference, ConstructorInfo cinfo, object?[]? argumentValues)
		{
			var runtime = JniEnvironment.Runtime;

			try {
				if (runtime.UseMarshalMemberBuilder) {
					ActivateViaMarshalMemberBuilder (runtime.MarshalMemberBuilder, reference, cinfo, argumentValues);
					return;
				}
				ActivateViaReflection (reference, cinfo, argumentValues);
			} catch (Exception e) {
				var m = string.Format ("Could not activate {{ PeerReference={0} IdentityHashCode=0x{1} Java.Type={2} }} for managed type '{3}'.",
						reference,
						runtime.ValueManager.GetJniIdentityHashCode (reference).ToString ("x"),
						JniEnvironment.Types.GetJniTypeNameFromInstance (reference),
						cinfo.DeclaringType?.FullName);
				Debug.WriteLine (m);

				throw new NotSupportedException (m, e);
			}
		}

		void ActivateViaMarshalMemberBuilder (JniRuntime.JniMarshalMemberBuilder builder, JniObjectReference reference, ConstructorInfo cinfo, object?[]? argumentValues)
		{
			var f = builder.CreateConstructActivationPeerFunc (cinfo);
			f (cinfo, reference, argumentValues);
		}

		void ActivateViaReflection (JniObjectReference reference, ConstructorInfo cinfo, object?[]? argumentValues)
		{
			var declType  = cinfo.DeclaringType ?? throw new NotSupportedException ("Do not know the type to create!");

#pragma warning disable IL2072
			var self      = (IJavaPeerable) System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject (declType);
#pragma warning restore IL2072
			self.SetPeerReference (reference);

			cinfo.Invoke (self, argumentValues);
		}

		public override List<JniSurfacedPeerInfo> GetSurfacedPeers ()
		{
			if (RegisteredInstances == null)
				throw new ObjectDisposedException (nameof (ManagedValueManager));

			lock (RegisteredInstances) {
				var peers = new List<JniSurfacedPeerInfo> (RegisteredInstances.Count);
				foreach (var e in RegisteredInstances) {
					foreach (var p in e.Value) {
						peers.Add (new JniSurfacedPeerInfo (e.Key, CreateWeakReference (p)));
					}
				}
				return peers;
			}
		}
	}
}
