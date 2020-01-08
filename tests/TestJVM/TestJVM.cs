using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using Java.Interop;

namespace Java.InteropTests
{
	public class TestJVM : JreRuntime {

		static JreRuntimeOptions CreateBuilder (string[] jars)
		{
			var dir = Path.GetDirectoryName (typeof (TestJVM).Assembly.Location);
			var builder = new JreRuntimeOptions () {
				JvmLibraryPath              = Environment.GetEnvironmentVariable ("JI_JVM_PATH"),
			};
			if (jars != null) {
				foreach (var jar in jars)
					builder.ClassPath.Add (Path.Combine (dir, jar));
			}
			builder.AddOption ("-Xcheck:jni");
			builder.TypeManager                 = new JreTypeManager ();

			return builder;
		}

		Dictionary<string, Type> typeMappings;

		public TestJVM (string[] jars = null, Dictionary<string, Type> typeMappings = null)
			: base (CreateBuilder (jars))
		{
			this.typeMappings = typeMappings;
		}

		class JreTypeManager : JniTypeManager {

			protected override IEnumerable<Type> GetTypesForSimpleReference (string jniSimpleReference)
			{
				foreach (var t in base.GetTypesForSimpleReference (jniSimpleReference))
					yield return t;
				var mappings = ((TestJVM) Runtime).typeMappings;
				Type target;
				if (mappings != null && mappings.TryGetValue (jniSimpleReference, out target))
					yield return target;
			}

			protected override string GetSimpleReference (Type type)
			{
				var simpleRef = base.GetSimpleReference (type);
				if (simpleRef != null)
					return simpleRef;
				var mappings = ((TestJVM) Runtime).typeMappings;
				if (mappings == null)
					return null;
				foreach (var e in mappings) {
					if (e.Value == type)
						return e.Key;
				}
				return null;
			}

			protected override IEnumerable<string> GetSimpleReferences (Type type)
			{
				return base.GetSimpleReferences (type)
					.Concat (CreateSimpleReferencesEnumerator (type));
			}

			IEnumerable<string> CreateSimpleReferencesEnumerator (Type type)
			{
				var mappings = ((TestJVM) Runtime).typeMappings;
				if (mappings == null)
					yield break;
				foreach (var e in mappings) {
					if (e.Value == type)
						yield return e.Key;
				}
			}
		}
	}
}

