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
		public ClassInvokerClass (ClassGen @class, CodeGenerationOptions opt)
		{
			Name = $"{@class.Name}Invoker";

			IsInternal = true;
			IsPartial = true;
			UsePriorityOrder = true;

			Inherits = @class.Name;

			foreach (var igen in @class.GetAllDerivedInterfaces ().Where (i => i.IsGeneric))
				Implements.Add (opt.GetOutputName (igen.FullName));

			Attributes.Add (new RegisterAttr (@class.RawJniName, noAcw: true, additionalProperties: @class.AdditionalAttributeString ()) { UseGlobal = true });

			var ctor = new ConstructorWriter (Name) {
				IsPublic = true,
				BaseCall = "base (handle, transfer)",
				Priority = GetNextPriority ()
			};

			ctor.Parameters.Add (new MethodParameterWriter ("handle", TypeReferenceWriter.IntPtr));
			ctor.Parameters.Add (new MethodParameterWriter ("transfer", new TypeReferenceWriter ("JniHandleOwnership")));

			Constructors.Add (ctor);

			// ClassInvokerHandle
			Fields.Add (new PeerMembersField (opt, @class.RawJniName, $"{@class.Name}Invoker", false) { Priority = GetNextPriority () });
			Properties.Add (new JniPeerMembersGetter { Priority = GetNextPriority () });
			Properties.Add (new ThresholdTypeGetter { Priority = GetNextPriority () });

			AddMemberInvokers (@class, opt, new HashSet<string> ());
		}

		void AddMemberInvokers (ClassGen @class, CodeGenerationOptions opt, HashSet<string> members)
		{
			AddPropertyInvokers (@class, @class.Properties, members, opt);
			AddMethodInvokers (@class, @class.Methods, members, null, opt);

			foreach (var iface in @class.GetAllDerivedInterfaces ()) {
				AddPropertyInvokers (@class, iface.Properties.Where (p => !@class.ContainsProperty (p.Name, false, false)), members, opt);
				AddMethodInvokers (@class, iface.Methods.Where (m => (opt.SupportDefaultInterfaceMethods || !m.IsInterfaceDefaultMethod) && !@class.ContainsMethod (m, false, false) && !@class.IsCovariantMethod (m) && !@class.ExplicitlyImplementedInterfaceMethods.Contains (m.GetSignature ())), members, iface, opt);
			}

			if (@class.BaseGen != null && @class.BaseGen.FullName != "Java.Lang.Object")
				AddMemberInvokers (@class.BaseGen, opt, members);
		}

		void AddPropertyInvokers (ClassGen @class, IEnumerable<Property> properties, HashSet<string> members, CodeGenerationOptions opt)
		{
			foreach (var prop in properties) {
				if (members.Contains (prop.Name))
					continue;

				members.Add (prop.Name);

				if ((prop.Getter != null && !prop.Getter.IsAbstract) ||
						(prop.Setter != null && !prop.Setter.IsAbstract))
					continue;

				Properties.Add (new BoundProperty (@class, prop, opt, false, true) { Priority = GetNextPriority () });

				if (prop.Type.StartsWith ("Java.Lang.ICharSequence"))
					Properties.Add (new BoundPropertyStringVariant (prop, opt) { Priority = GetNextPriority () });
			}
		}

		void AddMethodInvokers (ClassGen @class, IEnumerable<Method> methods, HashSet<string> members, InterfaceGen gen, CodeGenerationOptions opt)
		{
			foreach (var m in methods) {
				var sig = m.GetSignature ();

				if (members.Contains (sig))
					continue;

				members.Add (sig);
				if (!m.IsAbstract)

					continue;
				if (@class.IsExplicitlyImplementedMethod (sig)) {
					// sw.WriteLine ("// This invoker explicitly implements this method");
					Methods.Add (new ExplicitInterfaceInvokerMethod (m, gen, opt) { Priority = GetNextPriority () });
					//WriteMethodExplicitInterfaceInvoker (m, indent, gen);
				} else {
					// sw.WriteLine ("// This invoker overrides {0} method", gen.FullName);
					m.IsOverride = true;
					Methods.Add (new BoundMethod (@class, m, this, opt, false) { Priority = GetNextPriority () });

					if (m.Asyncify)
						Methods.Add (new MethodAsyncWrapper (m, opt) { Priority = GetNextPriority () });
					//WriteMethod (m, indent, @class, false);
					m.IsOverride = false;
				}
			}

		}
	}
}
