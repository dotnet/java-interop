using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

using Java.Interop;

using NUnit.Framework;

namespace Java.Interop.PerformanceTests {


	[TestFixture]
	class ReadOnlyPropertyTiming : Java.InteropTests.JavaVMFixture {
		[Test]
		public void StaticReadOnlyPropertyTiming ()
		{
			const int count = 1000;

			var noCache = Stopwatch.StartNew ();
			for (int i = 0; i < count; ++i) {
				_ = JavaTiming.StaticReadonlyField_NoCache;
			}
			noCache.Stop ();

			var badCache = Stopwatch.StartNew ();
			for (int i = 0; i < count; ++i) {
				_ = JavaTiming.StaticReadonlyField_ThreadUnsafeCache;
			}
			badCache.Stop ();

			var goodCache = Stopwatch.StartNew ();
			for (int i = 0; i < count; ++i) {
				_ = JavaTiming.StaticReadonlyField_ThreadSafeCache;
			}
			goodCache.Stop ();

			var ropCache = Stopwatch.StartNew ();
			for (int i = 0; i < count; ++i) {
				_ = JavaTiming.StaticReadonlyField_Rop;
			}
			ropCache.Stop ();

			Console.WriteLine ("Static ReadOnly Property Lookup + Invoke Timing:");
			Console.WriteLine ("\t                 No caching: {0}, {1} ticks", noCache.Elapsed,      noCache.ElapsedTicks / count);
			Console.WriteLine ("\t        Thread Unsafe Cache: {0}, {1} ticks", badCache.Elapsed,     badCache.ElapsedTicks / count);
			Console.WriteLine ("\t          Thread-Safe Cache: {0}, {1} ticks", goodCache.Elapsed,    goodCache.ElapsedTicks / count);
			Console.WriteLine ("\tReadOnlyProperty<int> Cache: {0}, {1} ticks", ropCache.Elapsed,     ropCache.ElapsedTicks / count);
		}
	}

	struct ReadOnlyProperty<T> {
		int have_value;
		T value;

		[MethodImpl (MethodImplOptions.AggressiveInlining)]
		public unsafe T GetValue (delegate *<T> c)
		{
			if (1 == Interlocked.CompareExchange (ref have_value, 1, 0))
				return value;
			var __v = c ();
			value = __v;
			return __v;
		}
	}
}