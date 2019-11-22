using System;
using System.Collections.Generic;
using System.Linq;

using Java.Interop;

using NUnit.Framework;

namespace Java.InteropTests
{
	public abstract class JavaPrimitiveArrayContract<TArray, TElement> : JavaArrayContract<TElement>
		where TArray : JavaPrimitiveArray<TElement>
	{
		protected override TElement CreateValueA ()
		{
			return FromInt32 ((int) 'A');
		}

		protected override TElement CreateValueB ()
		{
			return FromInt32 ((int) 'B');
		}

		protected abstract ICollection<TElement> CreateCollection (IList<TElement> values);

		protected abstract ICollection<TElement> CreateCollection (int length);

		protected TElement FromInt32 (int value)
		{
			return (TElement) Convert.ChangeType (value, typeof(TElement));
		}

		[Test]
		public void Constructor_Exceptions ()
		{
			Assert.Throws<ArgumentNullException>(() => CreateCollection ((IEnumerable<TElement>) null));
			Assert.Throws<ArgumentNullException>(() => CreateCollection ((IList<TElement>) null));
			Assert.Throws<ArgumentException>(() => CreateCollection (-1));
		}

		[Test]
		public void GetElements ()
		{
			var a = (TArray) CreateCollection (new[]{FromInt32 ('A')});
			JniArrayElements e;
			using (e = a.GetElements ()) {
				if (e == null) // OOM?
					return;
				Assert.IsTrue (e.Elements != IntPtr.Zero);
				// Multi-dispose is supported.
				e.Dispose ();
			}
			Assert.Throws<ObjectDisposedException> (() => e.Release (JniReleaseArrayElementsMode.Abort));
			Assert.Throws<ObjectDisposedException> (() => {
					#pragma warning disable 0219
					var _ = e.Elements;
					#pragma warning restore 0219
			});
			a.Dispose ();
		}

		// TODO: http://developer.android.com/training/articles/perf-jni.html#arrays
		//       "Also, if the Get call fails, you must ensure that your code doesn't
		//        try to Release a NULL pointer later."
		//  This implies that JNIEnv::Get<Type>ArrayElements() can return NULL; how/when?
		//  Theory: this happens if the array is empty (kinda like what C# `fixed` does?).
		//  Try to test this.
		//  (Alas, on OpenJDK JNIEnv::Get<Type>ArrayElements() returns a non-NULL pointer
		//   when the array is empty, so we'll need to run this on Android.)
		[Test]
		public void GetElements_EmptyArray ()
		{
			var a = (TArray) CreateCollection (new TElement[0]);
			JniArrayElements e;
			using (e = a.GetElements ()) {
				if (e == null)
					return;
				Assert.IsTrue (e.Elements != IntPtr.Zero);
				// Multi-dispose is supported.
				e.Dispose ();
			}
			Assert.Throws<ObjectDisposedException> (() => e.Release (JniReleaseArrayElementsMode.Abort));
			Assert.Throws<ObjectDisposedException> (() => {
				#pragma warning disable 0219
				var _ = e.Elements;
				#pragma warning restore 0219
			});
			a.Dispose ();
		}
	}
}

