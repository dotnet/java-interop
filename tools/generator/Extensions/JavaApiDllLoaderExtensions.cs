#if GENERATOR
using System;
using System.Collections.Generic;
using System.Linq;
using MonoDroid.Generation;

namespace Xamarin.Android.Tools.ApiXmlAdjuster
{
	public static class JavaApiDllLoaderExtensions
	{
		public static void LoadReferences (this JavaApi api, CodeGenerationOptions opt, IEnumerable<GenBase> gens)
		{
			JavaPackage pkg = null;
			foreach (var gen in gens.Where (_ => _.IsAcw)) {
				if (!api.Packages.TryGetValue (gen.PackageName, out pkg)) {
					pkg = new JavaPackage (api) { Name = gen.PackageName };
					api.Packages.Add (pkg.Name, pkg);
				}

				if (gen is InterfaceGen iface_gen) {
					var iface = new JavaInterface (pkg);
					iface.Load (iface_gen);

					pkg.AddType (iface);
				} else if (gen is ClassGen class_gen) {
					var kls = new JavaClass (pkg);
					kls.Load (opt, class_gen);

					pkg.AddType (kls);
				}
				else
					throw new InvalidOperationException ();
				api.LoadReferences (opt, gen.NestedTypes);
			}
		}

		static void Load (this JavaType type, GenBase gen)
		{
			type.IsReferenceOnly = true;

			type.Name = gen.JavaSimpleName;
			type.ExtendedJniSignature = gen.JniName;
			type.Deprecated = gen.DeprecatedComment;
			type.Visibility = gen.RawVisibility;
			type.Implements = gen.Interfaces.Select (_ => new JavaImplements () {
				Name = _.JavaName,
				ExtendedJniType = _.JniName,
			}).ToArray ();
			if (gen.TypeParameters != null && gen.TypeParameters.Any ()) {
				type.TypeParameters = new JavaTypeParameters (type);
				type.TypeParameters.Load (gen.TypeParameters);
			}
			foreach (var f in gen.Fields.Where (_ => _.IsAcw)) {
				var fld = new JavaField (type);
				fld.Load (f);
				type.Members.Add (fld);
			}
			foreach (var p in gen.Properties) {
				if (p.Getter != null && p.Getter.IsAcw) {
					var getter = new JavaMethod (type);
					getter.Load (p.Getter);
					type.Members.Add (getter);
				}
				if (p.Setter != null && p.Setter.IsAcw) {
					var setter = new JavaMethod (type);
					setter.Load (p.Setter);
					type.Members.Add (setter);
				}
			}
			foreach (var m in gen.Methods.Where (_ => _.IsAcw)) {
				var method = new JavaMethod (type);
				method.Load (m);
				type.Members.Add (method);
			}
		}

		static void Load (this JavaInterface iface, InterfaceGen gen)
		{
			((JavaType) iface).Load (gen);
		}

		static string ExpandTypeParameters (ISymbol [] tps)
		{
			if (tps == null)
				return null;
			return '<' + string.Join (", ", tps.Select (_ => _.JavaName)) + '>';
		}

		static void Load (this JavaClass kls, CodeGenerationOptions opt, ClassGen gen)
		{
			((JavaType) kls).Load (gen);

			kls.Abstract = gen.IsAbstract;
			kls.Final = gen.IsFinal;
			var baseGen = gen.BaseType != null ? opt.SymbolTable.Lookup (gen.BaseType) : null;

			if (baseGen != null) {
				kls.Extends = baseGen.JavaName;
				var gs = baseGen as GenericSymbol;
				kls.ExtendsGeneric = gs != null ? gs.JavaName + ExpandTypeParameters (gs.TypeParams) : baseGen.JavaName;
				kls.ExtendedJniExtends = baseGen.JniName;
			}
			foreach (var c in gen.Ctors) {
				var ctor = new JavaConstructor (kls);
				ctor.Load (c);
				kls.Members.Add (ctor);
			}
		}

		static void Load (this JavaField field, Field gf)
		{
			field.Deprecated = gf.DeprecatedComment;
			field.Final = gf.IsFinal;
			field.Name = gf.JavaName;
			field.Static = gf.IsStatic;
			field.Visibility = gf.Visibility;
			field.Type = gf.TypeName; // FIXME: this is managed name. Should there be Java typename?
			field.TypeGeneric = gf.TypeName; // FIXME: this is NOT generic.
			field.Value = gf.Value;
		}

		static void Load (this JavaMethodBase method, MethodBase gm)
		{
			method.Deprecated = gm.Deprecated;
			method.Visibility = gm.Visibility;
			method.Parameters = gm.Parameters.Select (_ => new JavaParameter (method) {
				Name = _.JavaName,
				Type = _.RawNativeType,
				}).ToArray ();
		}

		static void Load (this JavaMethod method, Method gm)
		{
			((JavaMethodBase) method).Load (gm);

			if (gm.RetVal.RawJavaType == "System.Boolean") throw new Exception (method.Parent.FullName + "." + gm.JavaName);

			method.Final = gm.IsFinal;
			method.Name = gm.JavaName;
			method.Static = gm.IsStatic;
			//method.ExtendedJniSignature = gm.JniSignature;
			if (gm.GenericArguments != null && gm.GenericArguments.Any ()) {
				method.TypeParameters = new JavaTypeParameters (method);
				method.TypeParameters.Load (gm.GenericArguments);
			}
			method.Abstract = gm.IsAbstract;
			method.Return = gm.RetVal.RawJavaType;
			// FIXME: get ExtendedJniReturn?
		}

		static void Load (this JavaConstructor ctor, Ctor gc)
		{
			((JavaMethodBase) ctor).Load (gc);
		}

		static void Load (this JavaTypeParameters tps, GenericParameterDefinitionList gtps)
		{
			foreach (var stp in gtps) {
				var tp = new JavaTypeParameter (tps) {
					Name = stp.Name,
				};
				if (stp.Constraints != null)
					tp.GenericConstraints = new JavaGenericConstraints () {
					BoundsType = stp.Name,
					GenericConstraints = stp.Constraints.Select (_ => new JavaGenericConstraint () { Type = _.JavaName }).ToArray ()};
				tps.TypeParameters.Add (tp);
			}
		}
	}
}

#endif
