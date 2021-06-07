using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public static class JavaTypeReferenceExtensions
	{
		public static bool ResolveGenerics = true;

		// Given a type like List<Dictionary<K, T>> when the type or method only have type argument T,
		// replace K with the specified typeParameter.
		public static IJavaTypeReference SetUnknownGenericTypeArguments (this IJavaTypeReference symbol, IJavaTypeReference typeParameter, params string [] knownArguments)
		{
			if (symbol is JavaGenericTypeParameterReference)
				return symbol;
			//if (symbol.FullName == symbol.GenericFullName)
			//	return symbol;

			//var parent = symbol.ParentType?.SetUnknownGenericTypeArguments (typeParameter, knownArguments);

			if (symbol is JavaArrayReference a) {
				var element_type = a.ElementType.SetUnknownGenericTypeArguments (typeParameter, knownArguments);
				//element_type.ParentType = parent;

				return new JavaArrayReference (element_type, a.IsParamArray);
			}

			if (symbol is JavaGenericReference gs) {
				var arguments = new List<IJavaTypeReference> ();

				foreach (var tp in gs.TypeParameters) {
					if (tp is JavaGenericTypeParameterReference gtps && !knownArguments.Contains (gtps.Name))
						arguments.Add (typeParameter);
					else
						arguments.Add (tp.SetUnknownGenericTypeArguments (typeParameter, knownArguments));
				}

				var base_symbol = gs.Symbol.SetUnknownGenericTypeArguments (typeParameter, knownArguments);
				//base_symbol.ParentType = parent;

				return new JavaGenericReference (base_symbol, arguments.ToArray ());
			}

			if (symbol is JavaTypeModel tm) {
				//if (!tm.IsGeneric)
				//	return new WrappedSymbol (symbol, parent);

				var arguments = new List<IJavaTypeReference> ();

				foreach (var _ in tm.TypeParameters)
					arguments.Add (typeParameter);

				//tm.ParentType = parent;
				return new JavaGenericReference (tm, arguments.ToArray ());
			}

			throw new Exception ();
		}
	}
}
