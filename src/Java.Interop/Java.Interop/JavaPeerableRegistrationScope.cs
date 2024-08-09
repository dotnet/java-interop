using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Java.Interop {
	[Experimental ("JI9999")]
	public enum JavaPeerableRegistrationScopeCleanup {
		RegisterWithManager,
		Dispose,
		Release,
	}

	[Experimental ("JI9999")]
	public ref struct JavaPeerableRegistrationScope {
		PeerableCollection? scope;
		bool disposed;

		public JavaPeerableRegistrationScope (JavaPeerableRegistrationScopeCleanup cleanup)
		{
			scope       = JniEnvironment.CurrentInfo.BeginRegistrationScope (cleanup);
			disposed    = false;
		}

		public void Dispose ()
		{
			if (disposed) {
				return;
			}
			disposed    = true;
			JniEnvironment.CurrentInfo.EndRegistrationScope (scope);
			scope       = null;
		}
	}
}
