using System;
using System.Linq;
using Java.Interop.Tools.JavaCallableWrappers;
using MonoDroid.Generation.Utilities;

namespace MonoDroid.Generation
{
	public class Method : MethodBase
	{
		bool is_override;

		public Method (GenBase declaringType) : base (declaringType)
		{
		}

		public string ArgsType { get; set; }
		public string CustomAttributes { get; set; }
		public string EventName { get; set; }
		public bool GenerateAsyncWrapper { get; set; }
		public bool GenerateDispatchingSetter { get; set; }
		public bool IsAbstract { get; set; }
		public bool IsCompatVirtualMethod { get; set; }
		public bool IsFinal { get; set; }
		public bool IsInterfaceDefaultMethod { get; set; }
		public Method OverriddenInterfaceMethod { get; set; }
		public bool IsReturnEnumified { get; set; }
		public bool IsStatic { get; set; }
		public bool IsVirtual { get; set; }
		public string JavaName { get; set; }
		public string ManagedOverride { get; set; }
		public string ManagedReturn { get; set; }
		public string PropertyNameOverride { get; set; }
		public string Return { get; set; }
		public bool ReturnNotNull { get; set; }
		public ReturnValue RetVal { get; set; }
		public int SourceApiLevel { get; set; }
		public string ExplicitInterface { get; set; }

		// it used to be private though...
		internal string AdjustedName => IsReturnCharSequence ? Name + "Formatted" : Name;

		public bool Asyncify {
			get {
				if (IsOverride)
					return false;

				return GenerateAsyncWrapper;
			}
		}

		public string AutoDetectEnumifiedOverrideReturn (AncestorDescendantCache cache)
		{
			if (RetVal.FullName != "int")
				return null;

			var classes = cache.GetAncestorsAndDescendants (DeclaringType);
			classes = classes.Concat (classes.SelectMany (x => x.GetAllImplementedInterfaces ()));

			foreach (var t in classes) {
				foreach (var candidate in t.GetAllMethods ().Where (m => m.Name == Name && m.Parameters.Count == Parameters.Count)) {
					if (JniSignature != candidate.JniSignature)
						continue;
					if (candidate.IsReturnEnumified)
						RetVal.SetGeneratedEnumType (candidate.RetVal.FullName);
				}
			}
			return null;
		}

		public bool CanAdd {
			get {
				return Name.Length > 3 && Name.StartsWith ("Add", StringComparison.Ordinal) && Name.EndsWith ("Listener", StringComparison.Ordinal) &&  IsVoid &&
					(Parameters.Count == 1 || (Parameters.Count == 2 && Parameters [1].Type == "Android.OS.Handler")) &&
					!(Parameters [0].IsArray);
			}
		}

		public bool CanGet {
			get {
				return Parameters.Count == 0 &&
					!IsVoid && !RetVal.IsArray &&
					((Name.Length > 4 && Name.StartsWith ("Get", StringComparison.Ordinal) && char.IsUpper (Name [3])) ||
					((Name.Length > 4 && Name.StartsWith ("Has", StringComparison.Ordinal) && char.IsUpper (Name [3]) && RetVal.JavaName == "boolean") ||
					 (Name.Length > 3 && Name.StartsWith ("Is", StringComparison.Ordinal) && char.IsUpper (Name [2]) && RetVal.JavaName == "boolean")));
			}
		}

		public bool CanSet {
			get {
				return Name.Length > 3 && Name.StartsWith ("Set", StringComparison.Ordinal) && Parameters.Count == 1 && IsVoid &&
					!(Parameters [0].IsArray);
			}
		}

		internal string CalculateEventName (Func<string, bool> checkNameDuplicate)
		{
			string event_name = EventName;
			if (event_name == null) {
				var trimSize = Name.EndsWith ("Listener", StringComparison.Ordinal) ? 8 : 0;
				event_name = Name.Substring (0, Name.Length - trimSize).Substring (3);
				if (event_name.StartsWith ("On", StringComparison.Ordinal))
					event_name = event_name.Substring (2);
				if (checkNameDuplicate (event_name))
					event_name += "Event";
			}
			return event_name;
		}

		public bool CanHaveStringOverload => IsReturnCharSequence || Parameters.HasCharSequence;

		public Method Clone (GenBase declaringType)
		{
			var clone = new Method (declaringType);

			// MethodBase
			clone.Annotation = Annotation;
			clone.ApiAvailableSince = ApiAvailableSince;
			clone.AssemblyName = AssemblyName;
			clone.Deprecated = Deprecated;
			clone.DeprecatedSince = DeprecatedSince;
			clone.IsAcw = IsAcw;
			clone.Name = Name;
			clone.Visibility = Visibility;
			clone.LineNumber = LineNumber;
			clone.LinePosition = LinePosition;
			clone.SourceFile = SourceFile;
			clone.JavadocInfo = JavadocInfo;

			if (GenericArguments != null) {
				if (clone.GenericArguments is null)
					clone.GenericArguments = new GenericParameterDefinitionList ();

				foreach (var ga in GenericArguments)
					clone.GenericArguments.Add (ga.Clone ());
			}

			foreach (var p in Parameters)
				clone.Parameters.Add (p.Clone ());

			// Method
			clone.ArgsType = ArgsType;
			clone.CustomAttributes = CustomAttributes;
			clone.EventName = EventName;
			clone.GenerateAsyncWrapper = GenerateAsyncWrapper;
			clone.GenerateDispatchingSetter = GenerateDispatchingSetter;
			clone.IsAbstract = IsAbstract;
			clone.IsFinal = IsFinal;
			clone.IsInterfaceDefaultMethod = IsInterfaceDefaultMethod;
			clone.OverriddenInterfaceMethod = OverriddenInterfaceMethod;
			clone.IsReturnEnumified = IsReturnEnumified;
			clone.IsStatic = IsStatic;
			clone.IsVirtual = IsVirtual;
			clone.JavaName = JavaName;
			clone.ManagedOverride = ManagedOverride;
			clone.ManagedReturn = ManagedReturn;
			clone.PropertyNameOverride = PropertyNameOverride;
			clone.Return = Return;
			clone.ReturnNotNull = ReturnNotNull;
			clone.RetVal = RetVal.Clone (clone);
			clone.SourceApiLevel = SourceApiLevel;
			clone.ExplicitInterface = ExplicitInterface;

			return clone;
		}

		public string ConnectorName => $"Get{Name}{IDSignature}Handler";

		public string EscapedCallbackName => IdentifierValidator.CreateValidIdentifier ($"cb_{JavaName}_{Name}{IDSignatureWithReturnType}", true);

		public string EscapedIdName => IdentifierValidator.CreateValidIdentifier ($"id_{JavaName}{IDSignature}", true);

		internal void FillReturnType ()
		{
			RetVal = new ReturnValue (this, Return, ManagedReturn, IsReturnEnumified, ReturnNotNull);
		}

		internal string GetAdapterName (CodeGenerationOptions opt, string adapter)
		{
			if (string.IsNullOrEmpty (adapter))
				return adapter;
			if (AssemblyName == null)
				return adapter + ", " + opt.AssemblyName;
			return adapter + AssemblyName;
		}

		// Connectors for DIM are defined on the interface, not the implementing type
		public string GetConnectorNameFull (CodeGenerationOptions opt) => ConnectorName + (opt.SupportDefaultInterfaceMethods && IsInterfaceDefaultMethod ? $":{DeclaringType.AssemblyQualifiedName}, " + (AssemblyName ?? opt.AssemblyName) : string.Empty);

		internal string GetDelegateType (CodeGenerationOptions opt) => opt.GetJniMarshalDelegate (this);

		public string GetMetadataXPathReference (GenBase declaringType) =>
			$"{declaringType.MetadataXPathReference}/method[@name='{JavaName}'{Parameters.GetMethodXPathPredicate ()}]";

		public string GetSignature () => $"n_{JavaName}:{JniSignature}:{ConnectorName}";

		public string GetSkipInvokerSignature () => $"{DeclaringType.RawJniName}.{JavaName}{JniSignature}";

		public bool IsEventHandlerWithHandledProperty => RetVal.JavaName == "boolean" && EventName != "";

		public override bool IsGeneric => base.IsGeneric || RetVal.IsGeneric;

		public bool IsListenerConnector => (CanAdd || CanSet) && Parameters [0].IsListener;

		public bool IsOverride {
			get => !IsStatic && is_override;
			set => is_override = value;
		}

		public bool IsPropertyAccessor => CanGet || CanSet;

		public bool IsReturnCharSequence => RetVal.FullName.StartsWith ("Java.Lang.ICharSequence", StringComparison.Ordinal);

		public bool IsSimpleEventHandler => RetVal.IsVoid && (Parameters.Count == 0 || (Parameters.HasSender && Parameters.Count == 1));

		public bool IsVoid => RetVal.JavaName == "void";

		public string JniSignature => "(" + Parameters.JniSignature + ")" + RetVal.JniName;

		public InterfaceGen ListenerType => Parameters [0].ListenerType;

		public override bool Matches (MethodBase other)
		{
			var ret = base.Matches (other);

			if (!ret)
				return ret;

			if (!(other is Method otherMethod))
				return false;

			if (RetVal.RawJavaType != otherMethod.RetVal.RawJavaType)
				return false;

			return true;
		}

		public string PropertyName {
			get {
				if (!IsPropertyAccessor)
					throw new InvalidOperationException ("Not a property: " + Name);
				var pn = PropertyNameOverride;
				if (pn != null)
					return pn;
				var nameBase = Name;
				if (CanAdd || CanSet || Name.StartsWith ("Get", StringComparison.Ordinal))
					nameBase = Name.Substring (3);
				if (IsAbstract && (CanGet && RetVal.IsGeneric || CanSet && Parameters [0].IsGeneric) &&
				    DeclaringType is ClassGen) // Interface methods cannot be RawXxx (because they are not generic so far...)
					return "Raw" + nameBase;
				return nameBase;
			}
		}

		public string ReturnType => RetVal.FullName;

		protected override bool OnValidate (CodeGenerationOptions opt, GenericParameterDefinitionList type_params, CodeGeneratorContext context)
		{
			if (GenericArguments != null)
				GenericArguments.Validate (opt, type_params, context);
			var tpl = GenericParameterDefinitionList.Merge (type_params, GenericArguments);
			if (!RetVal.Validate (opt, tpl, context))
				return false;

			return base.OnValidate (opt, tpl, context);
		}
	}
}
