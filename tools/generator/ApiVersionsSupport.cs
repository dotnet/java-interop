using System;
using System.Collections.Generic;
using Xamarin.AndroidTools.AnnotationSupport;
using System.Linq;

namespace MonoDroid.Generation
{
	static class IApiAvailabilityExtensions
	{
		public static string AdditionalAttributeString (this ApiVersionsSupport.IApiAvailability a)
		{
			return a.ApiAvailableSince == 0 ? null : ", ApiSince = " + a.ApiAvailableSince;
		}
	}

	public static class ApiVersionsSupport
	{
		public interface IApiAvailability
		{
			int ApiAvailableSince { get; set; }
		}

		public static void AssignApiLevels (IList<GenBase> gens, string apiVersionsXml, string currentApiLevelString)
		{
			int dummy;
			int currentApiLevel = int.TryParse (currentApiLevelString, out dummy) ? dummy : int.MaxValue;

			var versions = new ApiVersionsProvider ();
			versions.Parse (apiVersionsXml);
			foreach (var type in versions.Versions.Values) {
				var matchedGens = gens.Where (g => g.JavaName == type.Name);
				if (!matchedGens.Any ())
					continue;
				foreach (var gen in matchedGens)
					gen.ApiAvailableSince = type.Since;
				foreach (var field in type.Fields) {
					var genf = matchedGens.SelectMany (g => g.Fields).FirstOrDefault (f => f.JavaName == field.Name);
					// it might be moved to the corresponding class. 
					if (genf != null)
						genf.ApiAvailableSince = field.Since > 0 ? field.Since : type.Since;
				}
				Func<Method,ApiVersionsProvider.Definition,bool> methodMatch =
					(m, method) => m.JavaName == method.MethodName && m.JniSignature == method.Name.Substring (method.MethodName.Length);
				Func<Ctor,ApiVersionsProvider.Definition,bool> ctorMatch =
					(m, method) => m.JniSignature == method.Name.Substring (method.MethodName.Length);
				foreach (var method in type.Methods) {
					var genm = method.MethodName == "<init>" ?
						(MethodBase) matchedGens.OfType<ClassGen> ().SelectMany (g => g.Ctors).FirstOrDefault (m => ctorMatch (m, method)) :
						matchedGens.SelectMany (g => GetAllMethods (g)).FirstOrDefault (m => methodMatch (m, method));
					if (genm != null)
						genm.ApiAvailableSince = method.Since > 0 ? method.Since : type.Since;
				}
			}
		}

		static IEnumerable<Method> GetAllMethods (GenBase g)
		{
			foreach (var m in g.Properties) {
				if (m.Getter != null)
					yield return m.Getter;
				if (m.Setter != null)
					yield return m.Setter;
			}
			foreach (var m in g.Methods)
				yield return m;
		}
	}
}

