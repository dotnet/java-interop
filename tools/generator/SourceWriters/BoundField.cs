using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoDroid.Generation;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	public class BoundField : FieldWriter
	{
		// // Metadata.xml XPath field reference: path="/api/package[@name='android.os']/class[@name='Vibrator']/field[@name='VIBRATION_EFFECT_SUPPORT_UNKNOWN']"
		// [Register ("VIBRATION_EFFECT_SUPPORT_UNKNOWN", ApiSince = 30)]
		// [Obsolete ("This constant will be removed in the future version. Use Android.OS.VibrationEffectSupport enum directly instead of this field.", error: true)]
		// public const Android.OS.VibrationEffectSupport VibrationEffectSupportUnknown = (Android.OS.VibrationEffectSupport) 0;
		public BoundField (GenBase type, Field field, CodeGenerationOptions opt)
		{
			Name = field.Name;
			Type = new TypeReferenceWriter (opt.GetOutputName (field.Symbol.FullName));

			Comments.Add ($"// Metadata.xml XPath field reference: path=\"{type.MetadataXPathReference}/field[@name='{field.JavaName}']\"");

			Attributes.Add (new RegisterAttr (field.JavaName, additionalProperties: field.AdditionalAttributeString ()));

			if (field.IsEnumified)
				Attributes.Add (new GeneratedEnumAttr ());
			if (field.IsDeprecated)
				Attributes.Add (new ObsoleteAttr (field.DeprecatedComment, field.IsDeprecatedError) { NoAtSign = true, WriteEmptyString = true });
			if (field.Annotation.HasValue ())
				Attributes.Add (new CustomAttr (field.Annotation));

			SetVisibility (field.Visibility);
			IsConst = true;

			// the Value complication is due to constant enum from negative integer value (C# compiler requires explicit parenthesis).
			Value = $"({opt.GetOutputName (field.Symbol.FullName)}) {(field.Value.Contains ('-') && field.Symbol.FullName.Contains ('.') ? '(' + field.Value + ')' : field.Value)}";
		}		
	}
}
