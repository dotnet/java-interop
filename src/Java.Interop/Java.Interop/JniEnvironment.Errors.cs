#nullable enable

using System;

namespace Java.Interop {
	partial class JniEnvironment {

		partial class Exceptions {

			/// <summary>
			/// Throws a pending Java exception from a JNI object reference.
			/// </summary>
			/// <param name="toThrow">A valid JNI reference to the throwable object.</param>
			/// <exception cref="ArgumentException">
			/// <paramref name="toThrow"/> is not a valid JNI reference.
			/// </exception>
			public static void Throw (JniObjectReference toThrow)
			{
				if (!toThrow.IsValid)
					throw new ArgumentException ("JNI reference is not valid.", nameof (toThrow));

				int r = _Throw (toThrow);
				if (r != 0)
					throw new InvalidOperationException (string.Format ("Could not raise an exception; JNIEnv::Throw() returned {0}.", r));
			}


			/// <summary>
			/// Throws a new Java exception of the specified class with a message.
			/// </summary>
			/// <param name="klass">A valid JNI reference to the exception class.</param>
			/// <param name="message">The exception message.</param>
			/// <exception cref="ArgumentException">
			/// <paramref name="klass"/> is not a valid JNI reference.
			/// </exception>
			/// <exception cref="ArgumentNullException">
			/// <paramref name="message"/> is <c>null</c>.
			/// </exception>
			public static void ThrowNew (JniObjectReference klass, string message)
			{
				if (!klass.IsValid)
					throw new ArgumentException ("JNI reference is not valid.", nameof (klass));
				if (message == null)
					throw new ArgumentNullException (nameof (message));

				int r = _ThrowNew (klass, message);
				if (r != 0)
					throw new InvalidOperationException (string.Format ("Could not raise an exception; JNIEnv::ThrowNew() returned {0}.", r));
			}

			/// <summary>
			/// Throws a managed exception as a Java exception.
			/// </summary>
			/// <param name="e">The managed exception to marshal to Java.</param>
			/// <exception cref="ArgumentNullException">
			/// <paramref name="e"/> is <c>null</c>.
			/// </exception>
			public static void Throw (Exception e)
			{
				if (e == null)
					throw new ArgumentNullException (nameof (e));
				var je = e as JavaException;
				if (je == null) {
					je  = new JavaProxyThrowable (e);
				}
				Throw (je.PeerReference);
			}
		}
	}
}

