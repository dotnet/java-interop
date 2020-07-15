using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoDroid.Generation;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	public class BoundInterface : InterfaceWriter
	{
		readonly List<TypeWriter> pre_sibling_classes = new List<TypeWriter> ();
		readonly List<TypeWriter> post_sibling_classes = new List<TypeWriter> ();
		readonly bool dont_generate;

		public BoundInterface (InterfaceGen @interface, CodeGenerationOptions opt, CodeGeneratorContext context)
		{
			Name = @interface.Name;

			AddAlternativesClass (@interface, opt, context);

			// If this interface is just fields and we can't generate any of them
			// then we don't need to write the interface.  We still keep this type
			// because it may have nested types or need an InterfaceMemberAlternativeClass.
			if (@interface.IsConstSugar && @interface.GetGeneratableFields (opt).Count () == 0) {
				dont_generate = true;
				return;
			}

			IsPartial = true;

			UsePriorityOrder = true;

			SetVisibility (@interface.Visibility);

			Comments.Add ($"// Metadata.xml XPath interface reference: path=\"{@interface.MetadataXPathReference}\"");

			if (@interface.IsDeprecated)
				Attributes.Add (new ObsoleteAttr (@interface.DeprecatedComment) { WriteAttributeSuffix = true, WriteEmptyString = true });

			if (!@interface.IsConstSugar) {
				var signature = string.IsNullOrWhiteSpace (@interface.Namespace)
					? @interface.FullName.Replace ('.', '/')
					: @interface.Namespace + "." + @interface.FullName.Substring (@interface.Namespace.Length + 1).Replace ('.', '/');

				Attributes.Add (new RegisterAttr (@interface.RawJniName, string.Empty, signature + "Invoker", additionalProperties: @interface.AdditionalAttributeString ()));
			}

			if (@interface.TypeParameters != null && @interface.TypeParameters.Any ())
				Attributes.Add (new CustomAttr (@interface.TypeParameters.ToGeneratedAttributeString ()));

			AddInheritedInterfaces (@interface, opt);

			AddClassHandle (@interface, opt);
			AddFields (@interface, opt, context);
			AddProperties (@interface, opt);
			AddMethods (@interface, opt);
			AddNestedTypes (@interface, opt, context);

			// If this interface is just constant fields we don't need to add all the invoker bits
			if (@interface.IsConstSugar)
				return;

			if (!@interface.AssemblyQualifiedName.Contains ('/')) {
				if (@interface.Methods.Any (m => m.CanHaveStringOverload) || @interface.Methods.Any (m => m.Asyncify))
					post_sibling_classes.Add (new InterfaceExtensionsClass (@interface, null, opt));
			}

			post_sibling_classes.Add (new InterfaceInvokerClass (@interface, opt, context));

			AddInterfaceEventHandler (@interface, opt, context);
		}

		void AddAlternativesClass (InterfaceGen iface, CodeGenerationOptions opt, CodeGeneratorContext context)
		{
			if (iface.NoAlternatives)
				return;

			var staticMethods = iface.Methods.Where (m => m.IsStatic);

			if (iface.Fields.Any () || staticMethods.Any ())
				pre_sibling_classes.Add (new InterfaceMemberAlternativeClass (iface, opt, context));
		}

		void AddInterfaceEventHandler (InterfaceGen @interface, CodeGenerationOptions opt, CodeGeneratorContext context)
		{
			if (!@interface.IsListener)
				return;

			foreach (var method in @interface.Methods.Where (m => m.EventName != string.Empty)) {
				if (method.RetVal.IsVoid || method.IsEventHandlerWithHandledProperty) {
					if (!method.IsSimpleEventHandler || method.IsEventHandlerWithHandledProperty)
						post_sibling_classes.Add (new InterfaceEventArgsClass (@interface, method, opt, context));
				} else {
					var del = new DelegateWriter {
						Name = @interface.GetEventDelegateName (method),
						Type = new TypeReferenceWriter (opt.GetTypeReferenceName (method.RetVal)),
						IsPublic = true,
						Priority = GetNextPriority ()
					};

					Delegates.Add (del);
				}
			}

			post_sibling_classes.Add (new InterfaceEventHandlerImplClass (@interface, opt, context));
		}

		void AddInheritedInterfaces (InterfaceGen @interface, CodeGenerationOptions opt)
		{
			foreach (var isym in @interface.Interfaces) {
				var igen = (isym is GenericSymbol ? (isym as GenericSymbol).Gen : isym) as InterfaceGen;

				if (igen.IsConstSugar || igen.RawVisibility != "public")
					continue;

				Implements.Add (opt.GetOutputName (isym.FullName));
			}

			if (Implements.Count == 0 && !@interface.IsConstSugar)
				Implements.AddRange (new [] { "IJavaObject", "IJavaPeerable" });
		}

		void AddClassHandle (InterfaceGen @interface, CodeGenerationOptions opt)
		{
			if (opt.SupportDefaultInterfaceMethods && (@interface.HasDefaultMethods || @interface.HasStaticMethods))
				Fields.Add (new PeerMembersField (opt, @interface.RawJniName, @interface.Name, true) { Priority = GetNextPriority () });
		}

		void AddFields (InterfaceGen @interface, CodeGenerationOptions opt, CodeGeneratorContext context)
		{
			// Interface fields are only supported with DIM
			if (!opt.SupportInterfaceConstants)
				return;

			var seen = new HashSet<string> ();
			var fields = @interface.GetGeneratableFields (opt).ToList ();

			SourceWriterExtensions.AddFields (this, @interface, fields, seen, opt, context);
		}

		void AddProperties (InterfaceGen @interface, CodeGenerationOptions opt)
		{
			foreach (var prop in @interface.Properties.Where (p => !p.Getter.IsStatic && !p.Getter.IsInterfaceDefaultMethod))
				Properties.Add (new BoundInterfacePropertyDeclaration (prop, @interface, @interface.AssemblyQualifiedName + "Invoker", opt));

			if (!opt.SupportDefaultInterfaceMethods)
				return;

			var dim_properties = @interface.Properties.Where (p => p.Getter.IsInterfaceDefaultMethod || p.Getter.IsStatic);

			foreach (var prop in dim_properties) {
				if (prop.Getter.IsAbstract) {
					var baseProp = @interface.BaseSymbol != null ? @interface.BaseSymbol.GetPropertyByName (prop.Name, true) : null;
					if (baseProp != null) {
						if (baseProp.Type != prop.Getter.Return) {
							// This may not be required if we can change generic parameter support to return constrained type (not just J.L.Object).
							//writer.WriteLine ("{0}// skipped generating property {1} because its Java method declaration is variant that we cannot represent in C#", indent, property.Name);
							return;
						}
					}

					Properties.Add (new BoundAbstractProperty (@interface, prop, opt) { Priority = GetNextPriority () });

					if (prop.Type.StartsWith ("Java.Lang.ICharSequence"))
						Properties.Add (new BoundPropertyStringVariant (prop, opt) { Priority = GetNextPriority () });
				} else {
					Properties.Add (new BoundProperty (@interface, prop, opt, true, false) { Priority = GetNextPriority () });

					if (prop.Type.StartsWith ("Java.Lang.ICharSequence"))
						Properties.Add (new BoundPropertyStringVariant (prop, opt) { Priority = GetNextPriority () });
				}
			}
		}

		void AddMethods (InterfaceGen @interface, CodeGenerationOptions opt)
		{
			foreach (var m in @interface.Methods.Where (m => !m.IsStatic && !m.IsInterfaceDefaultMethod)) {
				if (m.Name == @interface.Name || @interface.ContainsProperty (m.Name, true))
					m.Name = "Invoke" + m.Name;

				Methods.Add (new BoundInterfaceMethodDeclaration (@interface, m, @interface.AssemblyQualifiedName + "Invoker", opt));
			}

			if (!opt.SupportDefaultInterfaceMethods)
				return;

			foreach (var method in @interface.Methods.Where (m => m.IsInterfaceDefaultMethod || m.IsStatic)) {
				if (!method.IsValid)
					continue;

				Methods.Add (new BoundMethod (@interface, method, this, opt, true) { Priority = GetNextPriority () });

				var name_and_jnisig = method.JavaName + method.JniSignature.Replace ("java/lang/CharSequence", "java/lang/String");
				var gen_string_overload = !method.IsOverride && method.Parameters.HasCharSequence && !@interface.ContainsMethod (name_and_jnisig);

				if (gen_string_overload || method.IsReturnCharSequence)
					Methods.Add (new BoundMethodStringOverload (method, opt) { Priority = GetNextPriority () });

				if (method.Asyncify)
					Methods.Add (new MethodAsyncWrapper (method, opt) { Priority = GetNextPriority () });
			}
		}

		void AddNestedTypes (InterfaceGen iface, CodeGenerationOptions opt, CodeGeneratorContext context)
		{
			// Generate nested types for supported nested types.  This is a new addition in C#8.
			// Prior to this, types nested in an interface had to be generated as sibling types.
			// The "Unnest" property is used to support backwards compatibility with pre-C#8 bindings.
			foreach (var nest in iface.NestedTypes.Where (t => !t.Unnest)) {
				var type = SourceWriterExtensions.BuildManagedTypeModel (nest, opt, context);
				type.Priority = GetNextPriority ();
				NestedTypes.Add (type);
			}
		}

		public override void Write (CodeWriter writer)
		{
			WritePreSiblingClasses (writer);

			if (!dont_generate)
				base.Write (writer);

			WritePostSiblingClasses (writer);
		}

		public void WritePreSiblingClasses (CodeWriter writer)
		{
			foreach (var sibling in pre_sibling_classes)
				sibling.Write (writer);
		}

		public void WritePostSiblingClasses (CodeWriter writer)
		{
			foreach (var sibling in post_sibling_classes)
				sibling.Write (writer);
		}
	}
}
