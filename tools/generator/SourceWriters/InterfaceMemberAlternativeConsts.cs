using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoDroid.Generation;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	public class InterfaceMemberAlternativeConstsClass : ClassWriter
	{
		// Historically .NET has not allowed interface implemented fields or constants, so we
		// initially worked around that by moving them to an abstract class, generally
		// IMyInterface -> MyInterfaceConsts
		// This was later expanded to accomodate static interface methods, creating a more appropriately named class
		// IMyInterface -> MyInterface
		// In this case the XXXConsts class is [Obsolete]'d and simply inherits from the newer class
		// in order to maintain backward compatibility.
		// If we're creating a binding that supports DIM, we remove the XXXConsts class as they've been
		// [Obsolete:iserror] for a long time, and we add [Obsolete] to the interface "class".
		public InterfaceMemberAlternativeConstsClass (InterfaceGen @interface, CodeGenerationOptions opt) : base ()
		{
			var should_obsolete = opt.SupportInterfaceConstants && opt.SupportDefaultInterfaceMethods;

			Name = @interface.HasManagedName
				? @interface.Name.Substring (1) + "Consts"
				: @interface.Name.Substring (1);
			Inherits = "Java.Lang.Object";

			Constructors.Add (new ConstructorWriter (Name) { IsInternal = true });

			Attributes.Add (new RegisterAttr (@interface.RawJniName, null, null, true, @interface.AdditionalAttributeString ()));

			if (should_obsolete)
				Attributes.Add (new ObsoleteAttr ($"Use the '{@interface.FullName}' type. This class will be removed in a future release."));
		}
	}
}
