#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Java.Interop {

	public static partial class JniEnvironment {

		internal    static  readonly    ThreadLocal<JniEnvironmentInfo>     Info    = new ThreadLocal<JniEnvironmentInfo> (() => new JniEnvironmentInfo (), trackAllValues: true);

		internal    static  JniEnvironmentInfo      CurrentInfo {
			[MethodImpl (MethodImplOptions.AggressiveInlining)]
			get {
				var e = Info.Value!;
				if (!e.IsValid)
					throw new NotSupportedException ("JNI Environment Information has been invalidated on this thread.");
				return e;
			}
		}

		public      static  JniRuntime              Runtime {
			[MethodImpl (MethodImplOptions.AggressiveInlining)]
			get {return CurrentInfo.Runtime;}
		}

		public      static  IntPtr                  EnvironmentPointer {
			[MethodImpl (MethodImplOptions.AggressiveInlining)]
			get {return CurrentInfo.EnvironmentPointer;}
		}

		public      static  JniVersion              JniVersion {
			[MethodImpl (MethodImplOptions.AggressiveInlining)]
			get {return (JniVersion) Versions.GetVersion ();}
		}

		public      static  int                     LocalReferenceCount {
			[MethodImpl (MethodImplOptions.AggressiveInlining)]
			get {return CurrentInfo.LocalReferenceCount;}
		}

		public      static  bool                    WithinNewObjectScope {
			[MethodImpl (MethodImplOptions.AggressiveInlining)]
			get {return CurrentInfo.WithinNewObjectScope;}
			internal set {CurrentInfo.WithinNewObjectScope = value;}
		}

		internal    static  void    SetEnvironmentPointer (IntPtr environmentPointer)
		{
			CurrentInfo.EnvironmentPointer  = environmentPointer;
		}

		internal    static  void    SetEnvironmentPointer (IntPtr environmentPointer, JniRuntime runtime)
		{
			if (!Info.IsValueCreated) {
				Info.Value = new JniEnvironmentInfo (environmentPointer, runtime);
				return;
			}
			CurrentInfo.EnvironmentPointer  = environmentPointer;
		}

		internal    static  void    SetEnvironmentInfo (JniEnvironmentInfo info)
		{
			Info.Value  = info;
		}

		internal    static  Exception?  GetExceptionForLastThrowable ()
		{
			var e   = JniEnvironment.Exceptions.ExceptionOccurred ();
			if (!e.IsValid)
				return null;
			// JniEnvironment.Errors.ExceptionDescribe ();
			JniEnvironment.Exceptions.ExceptionClear ();
			JniEnvironment.LogCreateLocalRef (e);
			return Runtime.GetExceptionForThrowable (ref e, JniObjectReferenceOptions.CopyAndDispose);
		}

		internal    static  Exception?  GetExceptionForLastThrowable (IntPtr thrown)
		{
			if (thrown == IntPtr.Zero)
				return null;
			var e   = new JniObjectReference (thrown, JniObjectReferenceType.Local);
			// JniEnvironment.Errors.ExceptionDescribe ();
			JniEnvironment.Exceptions.ExceptionClear ();
			JniEnvironment.LogCreateLocalRef (e);
			return Runtime.GetExceptionForThrowable (ref e, JniObjectReferenceOptions.CopyAndDispose);
		}

		internal    static  void        LogCreateLocalRef (JniObjectReference value)
		{
			if (!value.IsValid)
				return;
			Runtime.ObjectReferenceManager.CreatedLocalReference (CurrentInfo, value);
		}

#if FEATURE_JNIENVIRONMENT_SAFEHANDLES
		internal    static  void    PushLocalReferenceFrame ()
		{
			Info.Value.LocalReferences.Add (new List<JniLocalReference> ());
		}

		internal    static  void    PopLocalReferenceFrame ()
		{
			var localRefs   = Info.Value.LocalReferences;
			int last        = localRefs.Count - 1;
			var curRefs     = localRefs [last];
			localRefs.RemoveAt (last);

			foreach (var lref in curRefs) {
				// check required due to https://bugzilla.xamarin.com/show_bug.cgi?id=25850
				if (!lref.IsClosed)
					lref.Dispose ();
			}
		}

		internal    static  void    AddLocalReference (JniLocalReference value)
		{
			var localRefs   = Info.Value.LocalReferences;
			var cur         = localRefs [localRefs.Count - 1];
			cur.Add (value);
		}

		internal    static  void    LogCreateLocalRef (JniLocalReference value)
		{
			if (value == null || value.IsInvalid || value.IsClosed)
				return;
			var r = new JniObjectReference (value, JniObjectReferenceType.Local);
			LogCreateLocalRef (r);
		}

		internal static     void    DeleteLocalReference (JniLocalReference value, IntPtr handle)
		{
			var localRefs   = Info.Value.LocalReferences;
			var c           = localRefs.FirstOrDefault (r => r.Contains (value));
			if (c == null) {
				Runtime.ObjectReferenceManager.WriteLocalReferenceLine (
						"Deleting JNI local reference handle 0x{0} from wrong thread {1}! Ignoring...",
						handle.ToString ("x"), Runtime.GetCurrentThreadDescription ());
				Runtime.ObjectReferenceManager.WriteLocalReferenceLine ("{0}",
						System.Activator.CreateInstance (Type.GetType ("System.Diagnostics.StackTrace")));
				return;
			}
			c.Remove (value);
			var lref    = new JniObjectReference (value, JniObjectReferenceType.Local);
			Runtime.ObjectReferenceManager.DeleteLocalReference (Info.Value, ref lref);
			value.SetHandleAsInvalid ();
		}

		internal    static  bool    IsHandleValid (JniLocalReference lref)
		{
			if (lref == null || lref.IsInvalid || lref.IsClosed)
				return false;

			return  Info.Value.LocalReferences.FirstOrDefault (r => r.Contains (lref)) != null;
		}
#endif  // FEATURE_JNIENVIRONMENT_SAFEHANDLES

#if FEATURE_JNIOBJECTREFERENCE_INTPTRS
		internal    static  void    LogCreateLocalRef (IntPtr value)
		{
			if (value == IntPtr.Zero)
				return;
			var r = new JniObjectReference (value, JniObjectReferenceType.Local);
			LogCreateLocalRef (r);
		}
#endif  // FEATURE_JNIOBJECTREFERENCE_INTPTRS

		partial class References {

			internal static unsafe int GetJavaVM (IntPtr jnienv, out IntPtr vm)
			{
#if FEATURE_JNIENVIRONMENT_JI_PINVOKES
				return NativeMethods.java_interop_jnienv_get_java_vm (jnienv, out vm);
#elif FEATURE_JNIENVIRONMENT_JI_FUNCTION_POINTERS
				IntPtr _vm;
				int r       = JniNativeMethods.GetJavaVM (jnienv, &_vm);
				vm          = _vm;
				return r;
#else
				Invoker     = CreateInvoker (environmentPointer);
				var r       = Invoker.GetJavaVM (EnvironmentPointer, out vm);
				return r;
#endif
			}

			internal static void RawDeleteLocalRef (IntPtr env, IntPtr localRef)
			{
#if FEATURE_JNIENVIRONMENT_JI_PINVOKES
				NativeMethods.java_interop_jnienv_delete_local_ref (env, localRef);
#elif FEATURE_JNIENVIRONMENT_JI_FUNCTION_POINTERS
				JniNativeMethods.DeleteLocalRef (env, localRef);
#endif  // FEATURE_JNIENVIRONMENT_JI_FUNCTION_POINTERS
			}
		}

	}

	sealed class JniEnvironmentInfo : IDisposable {

		const   int             NameBufferLength        = 512;

		IntPtr                  environmentPointer;
		char[]?                 nameBuffer;
		bool                    disposed;
		JniRuntime?             runtime;

		List<PeerableCollection?>?          scopes;

		public      int                     LocalReferenceCount     {get; internal set;}
		public      bool                    WithinNewObjectScope    {get; set;}
		public      JniRuntime              Runtime {
			get => runtime ?? throw new NotSupportedException ();
			private set => runtime = value;
		}

		public IntPtr                  EnvironmentPointer {
			get {return environmentPointer;}
			set {
				if (disposed)
					throw new ObjectDisposedException (nameof (JniEnvironmentInfo));
				if (environmentPointer == value)
					return;

				environmentPointer  = value;
				IntPtr  vmh = IntPtr.Zero;
				int     r   = JniEnvironment.References.GetJavaVM (EnvironmentPointer, out vmh);
				if (r < 0)
					throw new InvalidOperationException ("JNIEnv::GetJavaVM() returned: " + r.ToString ());

				var vm = JniRuntime.GetRegisteredRuntime (vmh);
				if (vm == null)
					throw new NotSupportedException (
							string.Format ("No JavaVM registered with handle 0x{0}.",
								vmh.ToString ("x")));
				Runtime = vm;
			}
		}

		public      bool                    IsValid {
			get {return Runtime != null && environmentPointer != IntPtr.Zero;}
		}

		public      List<PeerableCollection?>?
		                                    Scopes => scopes;
		public      PeerableCollection?     CurrentScope =>
			scopes == null ? null : scopes [scopes.Count-1];

		public JniEnvironmentInfo ()
		{
			Runtime             = JniRuntime.CurrentRuntime;
			EnvironmentPointer  = Runtime._AttachCurrentThread ();
		}

		internal    JniEnvironmentInfo (IntPtr environmentPointer, JniRuntime runtime)
		{
			EnvironmentPointer  = environmentPointer;
			Runtime             = runtime;
		}

		internal unsafe JniObjectReference ToJavaName (string jniTypeName)
		{
			int index = jniTypeName.IndexOf ("/", StringComparison.Ordinal);

			if (index == -1)
				return JniEnvironment.Strings.NewString (jniTypeName);

			int length = jniTypeName.Length;
			if (length > NameBufferLength)
				return JniEnvironment.Strings.NewString (jniTypeName.Replace ('/', '.'));

			if (nameBuffer == null)
				nameBuffer = new char [NameBufferLength];

			fixed (char* src = jniTypeName, dst = nameBuffer) {
				char* src_ptr = src;
				char* dst_ptr = dst;
				char* end_ptr = src + length;
				while (src_ptr < end_ptr) {
					*dst_ptr = (*src_ptr == '/') ? '.' : *src_ptr;
					src_ptr++;
					dst_ptr++;
				}
				return JniEnvironment.Strings.NewString (dst, length);
			}
		}

		public void Dispose ()
		{
			if (disposed)
				return;
			runtime                 = null;
			environmentPointer      = IntPtr.Zero;
			nameBuffer              = null;
			LocalReferenceCount     = 0;
			disposed                = true;
		}

#pragma warning disable JI9999
		public PeerableCollection? BeginRegistrationScope (JavaPeerableRegistrationScopeCleanup cleanup)
		{
			if (cleanup != JavaPeerableRegistrationScopeCleanup.RegisterWithManager &&
					!Runtime.ValueManager.SupportsPeerableRegistrationScopes) {
				throw new NotSupportedException ("Peerable registration scopes are not supported by this runtime.");
			}
			scopes ??= new List<PeerableCollection?> ();
			if (cleanup == JavaPeerableRegistrationScopeCleanup.RegisterWithManager) {
				scopes.Add (null);
				return null;
			}
			var scope = new PeerableCollection (cleanup);
			scopes.Add (scope);
			return scope;
		}

		public void EndRegistrationScope (PeerableCollection? scope)
		{
			Debug.Assert (scopes != null);
			if (scopes == null) {
				return;
			}

			for (int i = scopes.Count; i > 0; --i) {
				var s = scopes [i - 1];
				if (s == scope) {
					scopes.RemoveAt (i - 1);
					break;
				}
			}
			if (scopes.Count == 0) {
				scopes = null;
			}
			scope?.DisposeScope ();
		}
#pragma warning restore JI9999

#if FEATURE_JNIENVIRONMENT_SAFEHANDLES
		internal    List<List<JniLocalReference>>   LocalReferences = new List<List<JniLocalReference>> () {
			new List<JniLocalReference> (),
		};
#endif  // FEATURE_JNIENVIRONMENT_SAFEHANDLES

#if !FEATURE_JNIENVIRONMENT_JI_FUNCTION_POINTERS && !FEATURE_JNIENVIRONMENT_JI_PINVOKES
		internal    JniEnvironmentInvoker   Invoker                 {get; private set;}

		static unsafe JniEnvironmentInvoker CreateInvoker (IntPtr handle)
		{
			IntPtr p = Marshal.ReadIntPtr (handle);
			return new JniEnvironmentInvoker ((JniNativeInterfaceStruct*) p);
		}
#endif  // !FEATURE_JNIENVIRONMENT_JI_PINVOKES
	}

#pragma warning disable JI9999
	sealed class PeerableCollection : KeyedCollection<int, IJavaPeerable> {
		public JavaPeerableRegistrationScopeCleanup Cleanup { get; }

		public PeerableCollection (JavaPeerableRegistrationScopeCleanup cleanup)
		{
			Cleanup = cleanup;
		}

		protected override int GetKeyForItem (IJavaPeerable item) => item.JniIdentityHashCode;

		public IJavaPeerable? GetPeerable (JniObjectReference reference, int identityHashCode)
		{
			if (!reference.IsValid) {
				return null;
			}
			if (TryGetValue (identityHashCode, out var p) &&
					JniEnvironment.Types.IsSameObject (reference, p.PeerReference)) {
				return p;
			}
			return null;
		}

		public void DisposeScope ()
		{
			Console.Error.WriteLine ($"# jonp: DisposeScope: {Cleanup}");
			Debug.Assert (Cleanup != JavaPeerableRegistrationScopeCleanup.RegisterWithManager);
			switch (Cleanup) {
				case JavaPeerableRegistrationScopeCleanup.Dispose:
					List<Exception>?    exceptions = null;
					foreach (var p in this) {
						DisposePeer (p, ref exceptions);
					}
					Clear ();
					if (exceptions != null) {
						throw new AggregateException ("Exceptions while disposing peers.", exceptions);
					}
					break;
				case JavaPeerableRegistrationScopeCleanup.Release:
					Clear ();
					break;
				case JavaPeerableRegistrationScopeCleanup.RegisterWithManager:
				default:
					throw new NotSupportedException ($"Unsupported scope cleanup value: {Cleanup}");
			}

			[SuppressMessage ("Design", "CA1031:Do not catch general exception types",
					Justification = "Exceptions are bundled into an AggregateException and rethrown")]
			static void DisposePeer (IJavaPeerable peer, ref List<Exception>? exceptions)
			{
				try {
					Console.Error.WriteLine ($"# jonp: DisposeScope: disposing of: {peer} {peer.PeerReference}");
					peer.Dispose ();
				} catch (Exception e) {
					exceptions ??= new ();
					exceptions.Add (e);
					Trace.WriteLine (e);
				}
			}
		}

		public override string ToString ()
		{
			var c   = (Collection<IJavaPeerable>) this;
			var s   = new StringBuilder ();
			s.Append ("PeerableCollection[").Append (Count).Append ("](");
			for (int i  = 0; i < Count; ++i ) {
				s.AppendLine ();
				var e   = c [i];
				s.Append ($"  [{i}] hash={e.JniIdentityHashCode} ref={e.PeerReference} type={e.GetType ().ToString ()} value=`{e.ToString ()}`");
			}
			s.Append (")");
			return s.ToString ();
		}
	}
#pragma warning restore JI9999
}

