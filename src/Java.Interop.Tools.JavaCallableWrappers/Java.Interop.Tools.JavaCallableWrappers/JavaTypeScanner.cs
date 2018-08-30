using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Mono.Cecil;

using Java.Interop.Tools.Cecil;
using Java.Interop.Tools.TypeNameMappings;

namespace Java.Interop.Tools.JavaCallableWrappers
{
	public class JavaTypeScanner
	{
		public  Action<TraceLevel, string>      Logger                      { get; private set; }
		public  bool                            ErrorOnCustomJavaObject     { get; set; }

		public JavaTypeScanner (Action<TraceLevel, string> logger)
		{
			if (logger == null)
				throw new ArgumentNullException (nameof (logger));
			Logger      = logger;
		}

		public List<TypeDefinition> GetJavaTypes (IEnumerable<string> assemblies, IAssemblyResolver resolver)
		{
			var javaTypes = new List<TypeDefinition> ();

			foreach (var assembly in assemblies) {
				var assm = resolver.GetAssembly (assembly);
				foreach (ModuleDefinition md in assm.Modules) {
					foreach (TypeDefinition td in md.Types) {
						AddJavaTypes (javaTypes.Add, td);
					}
				}
			}

			return javaTypes;
		}

		public List<TypeDefinition> GetJavaTypesInParallel (IEnumerable<string> assemblies, IAssemblyResolver resolver)
		{
			var javaTypes = new BlockingCollection<TypeDefinition> ();

			Parallel.ForEach (assemblies, assembly => {
				var assm = resolver.GetAssembly (assembly);
				foreach (ModuleDefinition md in assm.Modules) {
					foreach (TypeDefinition td in md.Types) {
						AddJavaTypes (javaTypes.Add, td);
					}
				}
			});

			return javaTypes.ToList ();
		}

		void AddJavaTypes (Action<TypeDefinition> addMethod, TypeDefinition type)
		{
			if (type.IsSubclassOf ("Java.Lang.Object") || type.IsSubclassOf ("Java.Lang.Throwable")) {

				// For subclasses of e.g. Android.App.Activity.
				addMethod (type);
			} else if (type.IsClass && !type.IsSubclassOf ("System.Exception") && type.ImplementsInterface ("Android.Runtime.IJavaObject")) {
				var level   = ErrorOnCustomJavaObject ? TraceLevel.Error : TraceLevel.Warning;
				var prefix  = ErrorOnCustomJavaObject ? "error" : "warning";
				Logger (
						level,
						$"{prefix} XA4212: Type `{type.FullName}` implements `Android.Runtime.IJavaObject` but does not inherit `Java.Lang.Object` or `Java.Lang.Throwable`. This is not supported.");
				return;
			}

			if (!type.HasNestedTypes)
				return;

			foreach (TypeDefinition nested in type.NestedTypes)
				AddJavaTypes (addMethod, nested);
		}

		public static bool ShouldSkipJavaCallableWrapperGeneration (TypeDefinition type)
		{
			if (JavaNativeTypeManager.IsNonStaticInnerClass (type))
				return true;

			foreach (var r in type.GetCustomAttributes (typeof (global::Android.Runtime.RegisterAttribute))) {

				if (JavaCallableWrapperGenerator.ToRegisterAttribute (r).DoNotGenerateAcw) {
					return true;
				}
			}

			return false;
		}
		// Returns all types for which we need to generate Java delegate types.
		public static List<TypeDefinition> GetJavaTypes (IEnumerable<string> assemblies, IAssemblyResolver resolver, Action<string, object []> log)
		{
			Action<TraceLevel, string> l = (level, value) => log ("{0}", new string [] { value });
			return new JavaTypeScanner (l).GetJavaTypes (assemblies, resolver);
		}
	}
}
