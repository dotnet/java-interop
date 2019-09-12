using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Android.Tools.ApiXmlAdjuster
{
	public static class JavaApiNonBindableStripper
	{
		public static void StripNonBindables (this JavaApi api)
		{
			// Remove invalid types:
			// - Types that start with a hyphen
			var inv_types = api.Packages.SelectMany (p => p.Types).Where (t => t.Name?.StartsWith ("-") == true).ToList ();

			foreach (var type in inv_types)
				type.Parent.Types.Remove (type);

			// Remove invalid members:
			// - Members that contain a $
			// - Members that start with a hyphen
			var inv_members = api.Packages.SelectMany (p => p.Types).SelectMany (t => t.Members)
						      .Where (m => m.Name?.Contains ('$') == true || m.Name?.StartsWith ("-") == true).ToList ();

			foreach (var member in inv_members)
				member.Parent.Members.Remove (member);
		}
	}
}
