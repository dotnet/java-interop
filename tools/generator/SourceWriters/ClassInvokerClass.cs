using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using MonoDroid.Generation;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	public class ClassInvokerClass : ClassWriter
	{
		public ClassInvokerClass (ClassGen klass, CodeGenerationOptions opt)
		{
			Name = $"{klass.Name}Invoker";

			IsInternal = true;
			IsPartial = true;
			UsePriorityOrder = true;

			Inherits = klass.Name;

			foreach (var igen in klass.GetAllDerivedInterfaces ().Where (i => i.IsGeneric))
				Implements.Add (opt.GetOutputName (igen.FullName));

			Attributes.Add (new RegisterAttr (klass.RawJniName, noAcw: true, additionalProperties: klass.AdditionalAttributeString ()) { UseGlobal = true });

			var ctor = new ConstructorWriter {
				Name = Name,
				IsPublic = true,
				BaseCall = "base (handle, transfer)"
			};

			ctor.Parameters.Add (new MethodParameterWriter ("handle", TypeReferenceWriter.IntPtr));
			ctor.Parameters.Add (new MethodParameterWriter ("transfer", new TypeReferenceWriter ("JniHandleOwnership")));

			Constructors.Add (ctor);

			// ClassInvokerHandle
			Fields.Add (new PeerMembersField (opt, klass.RawJniName, $"{klass.Name}Invoker", false));
			Properties.Add (new JniPeerMembersGetter ());
			Properties.Add (new ThresholdTypeGetter ());

			AddMemberInvokers (klass, opt, new HashSet<string> ());
		}

		void AddMemberInvokers (ClassGen klass, CodeGenerationOptions opt, HashSet<string> members)
		{
			AddPropertyInvokers (klass, klass.Properties, members, opt);
			AddMethodInvokers (klass, klass.Methods, members, null, opt);

			foreach (var iface in klass.GetAllDerivedInterfaces ()) {
				AddPropertyInvokers (klass, iface.Properties.Where (p => !klass.ContainsProperty (p.Name, false, false)), members, opt);
				AddMethodInvokers (klass, iface.Methods.Where (m => (opt.SupportDefaultInterfaceMethods || !m.IsInterfaceDefaultMethod) && !klass.ContainsMethod (m, false, false) && !klass.IsCovariantMethod (m) && !klass.ExplicitlyImplementedInterfaceMethods.Contains (m.GetSignature ())), members, iface, opt);
			}

			if (klass.BaseGen != null && klass.BaseGen.FullName != "Java.Lang.Object")
				AddMemberInvokers (klass.BaseGen, opt, members);
		}

		void AddPropertyInvokers (ClassGen klass, IEnumerable<Property> properties, HashSet<string> members, CodeGenerationOptions opt)
		{
			foreach (var prop in properties) {
				if (members.Contains (prop.Name))
					continue;

				members.Add (prop.Name);

				if ((prop.Getter != null && !prop.Getter.IsAbstract) || (prop.Setter != null && !prop.Setter.IsAbstract))
					continue;

				var bound_property = new BoundProperty (klass, prop, opt, false, true);
				Properties.Add (bound_property);

				if (prop.Type.StartsWith ("Java.Lang.ICharSequence") && !bound_property.IsOverride)
					Properties.Add (new BoundPropertyStringVariant (prop, opt));
			}
		}

		void AddMethodInvokers (ClassGen klass, IEnumerable<Method> methods, HashSet<string> members, InterfaceGen gen, CodeGenerationOptions opt)
		{
			foreach (var m in methods) {
				var sig = m.GetSignature ();

				if (members.Contains (sig))
					continue;

				members.Add (sig);

				if (!m.IsAbstract)
					continue;

				if (klass.IsExplicitlyImplementedMethod (sig)) {
					Methods.Add (new ExplicitInterfaceInvokerMethod (gen, m, opt));
				} else {
					m.IsOverride = true;
					Methods.Add (new BoundMethod (klass, m, opt, false));

					if (m.Asyncify)
						Methods.Add (new MethodAsyncWrapper (m, opt));

					m.IsOverride = false;
				}
			}
		}
	}
}
