using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Android.Tools.ApiXmlAdjuster
{
	public static class JavaApiGenericInheritanceMapperExtensions
	{
		public static void CreateGenericInheritanceMapping (this JavaApi api)
		{
			foreach (var kls in api.Packages.SelectMany (p => p.Types).OfType<JavaClass> ())
				kls.PrepareGenericInheritanceMapping ();
		}
		
		static void PrepareGenericInheritanceMapping (this JavaClass cls)
		{
			if (cls.GenericInheritanceMapping != null)
				return; // already done.
			
			var empty = new Dictionary<JavaTypeReference,JavaTypeReference> ();
			
			var bt = cls.ResolvedExtends == null ? null : cls.ResolvedExtends.ReferencedType as JavaClass;
			if (bt == null)
				cls.GenericInheritanceMapping = new Dictionary<JavaTypeReference,JavaTypeReference> (); // empty
			else {
				// begin processing from the base class.
				bt.PrepareGenericInheritanceMapping ();
				
				if (cls.ResolvedExtends.TypeParameters == null)
					cls.GenericInheritanceMapping = empty;
				else if (cls.ResolvedExtends.ReferencedType.TypeParameters == null) {
					// FIXME: I guess this should not happen. But this still happens.
					Log.LogWarning (Java.Interop.Localization.Resources.ApiXmlAdjuster_0002, cls.ExtendsGeneric, cls.FullName);
					cls.GenericInheritanceMapping = empty;
				} else {
					if (cls.ResolvedExtends.ReferencedType.TypeParameters.TypeParameters.Count != cls.ResolvedExtends.TypeParameters.Count)
						throw new Exception (string.Format (Java.Interop.Localization.Resources.ApiXmlAdjuster_0007, cls.Parent.Name, cls.Name));
					var dic = empty;
					foreach (var kvp in cls.ResolvedExtends.ReferencedType.TypeParameters.TypeParameters.Zip (
						 cls.ResolvedExtends.TypeParameters,
						 (def, use) => new KeyValuePair<JavaTypeParameter, JavaTypeReference> (def, use))
						 .Where (p => p.Value.ReferencedTypeParameter == null || p.Key.Name != p.Value.ReferencedTypeParameter.Name))
						dic.Add (new JavaTypeReference (kvp.Key, null), kvp.Value);
					if (dic.Any ()) {
						cls.GenericInheritanceMapping = dic;
					}
					else
						cls.GenericInheritanceMapping = empty;
				}
			}
		}
	}
}

