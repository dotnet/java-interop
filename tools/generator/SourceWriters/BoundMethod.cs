using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoDroid.Generation;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	public class BoundMethod : MethodWriter
	{
		readonly Method method;
		readonly CodeGenerationOptions opt;

		public BoundMethod (GenBase type, Method method, TypeWriter @class, CodeGenerationOptions opt, bool generateCallbacks) : base ()
		{
			this.method = method;
			this.opt = opt;

			if (generateCallbacks && method.IsVirtual)
				@class.Methods.Add (new MethodCallback (type, method, opt, null, method.IsReturnCharSequence) { Priority = @class.GetNextPriority () });

			Name = method.AdjustedName;

			IsStatic = method.IsStatic;
			IsSealed = method.IsOverride && method.IsFinal;
			IsUnsafe = true;

			SetVisibility (type is InterfaceGen && !IsStatic ? string.Empty : method.Visibility);

			// TODO: Clean up this logic
			var is_explicit = opt.SupportDefaultInterfaceMethods && type is InterfaceGen && method.OverriddenInterfaceMethod != null;
			var virt_ov = is_explicit ? string.Empty : method.IsOverride ? (opt.SupportDefaultInterfaceMethods && method.OverriddenInterfaceMethod != null ? " virtual" : " override") : method.IsVirtual ? " virtual" : string.Empty;

			IsVirtual = virt_ov.Trim () == "virtual";
			IsOverride = virt_ov.Trim () == "override";

			// When using DIM, don't generate "virtual sealed" methods, remove both modifiers instead
			if (opt.SupportDefaultInterfaceMethods && method.OverriddenInterfaceMethod != null && IsVirtual && IsSealed) {
				IsVirtual = false;
				IsSealed = false;
			}

			if ((IsVirtual || !IsOverride) && type.RequiresNew (method.AdjustedName, method))
				IsShadow = true;

			ReturnType = new TypeReferenceWriter (opt.GetTypeReferenceName (method.RetVal));

			if (method.DeclaringType.IsGeneratable)
				Comments.Add ($"// Metadata.xml XPath method reference: path=\"{method.GetMetadataXPathReference (method.DeclaringType)}\"");

			if (method.Deprecated.HasValue ())
				Attributes.Add (new ObsoleteAttr (method.Deprecated.Replace ("\"", "\"\"")));

			if (method.IsReturnEnumified)
				Attributes.Add (new GeneratedEnumAttr (true));

			Attributes.Add (new RegisterAttr (method.JavaName, method.JniSignature, method.IsVirtual ? method.GetConnectorNameFull (opt) : string.Empty, additionalProperties: method.AdditionalAttributeString ()));

			SourceWriterExtensions.AddMethodCustomAttributes (Attributes, method);
		}

		protected override void WriteBody (CodeWriter writer)
		{
			var old_virtual = method.IsVirtual;
			method.IsVirtual = IsVirtual || IsOverride;
			SourceWriterExtensions.WriteMethodBody (writer, method, opt);
			method.IsVirtual = old_virtual;
		}

		protected override void WriteParameters (CodeWriter writer)
		{
			writer.Write (method.GetSignature (opt));
		}
	}
}
