using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoDroid.Generation;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	public class BoundAbstractProperty : PropertyWriter
	{
		readonly MethodCallback getter_callback;
		readonly MethodCallback setter_callback;

		public BoundAbstractProperty (GenBase gen, Property property, CodeGenerationOptions opt)
		{
			Name = property.AdjustedName;
			PropertyType = new TypeReferenceWriter (opt.GetTypeReferenceName (property.Getter.RetVal));

			SetVisibility (property.Getter.RetVal.IsGeneric ? "protected" : property.Getter.Visibility);

			IsAbstract = true;
			HasGet = true;

			var baseProp = gen.BaseSymbol != null ? gen.BaseSymbol.GetPropertyByName (property.Name, true) : null;

			if (baseProp != null) {
				IsOverride = true;
			} else {
				IsShadow = gen.RequiresNew (property);

				getter_callback = new MethodCallback (gen, property.Getter, opt, property.AdjustedName, false);

				if (property.Setter != null)
					setter_callback = new MethodCallback (gen, property.Setter, opt, property.AdjustedName, false);
			}

			if (gen.IsGeneratable)
				GetterComments.Add ($"// Metadata.xml XPath method reference: path=\"{gen.MetadataXPathReference}/method[@name='{property.Getter.JavaName}'{property.Getter.Parameters.GetMethodXPathPredicate ()}]\"");
			if (property.Getter.IsReturnEnumified)
				GetterAttributes.Add (new GeneratedEnumAttr (true));

			GetterAttributes.Add (new RegisterAttr (property.Getter.JavaName, property.Getter.JniSignature, property.Getter.GetConnectorNameFull (opt), additionalProperties: property.Getter.AdditionalAttributeString ()));

			SourceWriterExtensions.AddMethodCustomAttributes (GetterAttributes, property.Getter);

			if (property.Setter != null) {
				HasSet = true;

				if (gen.IsGeneratable)
					SetterComments.Add ($"// Metadata.xml XPath method reference: path=\"{gen.MetadataXPathReference}/method[@name='{property.Setter.JavaName}'{property.Setter.Parameters.GetMethodXPathPredicate ()}]\"");

				SourceWriterExtensions.AddMethodCustomAttributes (SetterAttributes, property.Setter);
				SetterAttributes.Add (new RegisterAttr (property.Setter.JavaName, property.Setter.JniSignature, property.Setter.GetConnectorNameFull (opt), additionalProperties: property.Setter.AdditionalAttributeString ()));
			}
		}

		public override void Write (CodeWriter writer)
		{
			// Need to write our property callbacks first
			getter_callback?.Write (writer);
			setter_callback?.Write (writer);

			base.Write (writer);
		}
	}
}
