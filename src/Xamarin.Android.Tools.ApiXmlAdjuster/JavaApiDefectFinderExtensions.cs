using System;
using System.Linq;

namespace Xamarin.Android.Tools.ApiXmlAdjuster
{
	public static class JavaApiDefectFinderExtensions
	{
		public static void FindDefects (this JavaApi api)
		{
			foreach (var type in api.Packages.SelectMany (p => p.Types).Where (t => !t.IsReferenceOnly))
				type.FindDefects ();
		}
		
		static void FindDefects (this JavaType type)
		{
			foreach (var m in type.Members.OfType<JavaMethodBase> ())
				m.FindParametersDefects ();
		}
		
		static void FindParametersDefects (this JavaMethodBase methodBase)
		{
			foreach (var p in methodBase.Parameters) {
				if (p.Name.StartsWith ("p", StringComparison.Ordinal) && int.TryParse (p.Name.Substring (1), out var _)) {
					Log.LogWarning (Java.Interop.Localization.Resources.ApiXmlAdjuster_0001, methodBase.Parent, methodBase);
					break; // reporting once is enough.
				}
			}
		}
	}
}

