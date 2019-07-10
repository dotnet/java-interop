using System;

namespace MonoDroid.Generation
{
	public abstract class MethodBase : ApiVersionsSupport.IApiAvailability
	{
		protected MethodBase (GenBase declaringType)
		{
			DeclaringType = declaringType;
		}

		public string Annotation { get; internal set; }
		public int ApiAvailableSince { get; set; }
		public string AssemblyName { get; set; }
		public GenBase DeclaringType { get; }
		public string Deprecated { get; set; }
		public GenericParameterDefinitionList GenericArguments { get; set; }
		public bool IsAcw { get; set; }
		public bool IsValid { get; private set; }
		public string Name { get; set; }
		public ParameterList Parameters { get; } = new ParameterList ();
		public string Visibility { get; set; }


		internal string IDSignature => Parameters.Count > 0 ? "_" + Parameters.JniSignature.Replace ("/", "_").Replace ("`", "_").Replace (";", "_").Replace ("$", "_").Replace ("[", "array") : String.Empty;

		public virtual bool IsGeneric => Parameters.HasGeneric;

		public virtual bool Matches (MethodBase other)
		{
			if (Name != other.Name)
				return false;

			if (Parameters.Count != other.Parameters.Count)
				return false;

			for (int i = 0; i < Parameters.Count; i++) {
				if (Parameters [i].RawNativeType != other.Parameters [i].RawNativeType)
					return false;
			}

			return true;
		}

		protected virtual bool OnValidate (CodeGenerationOptions opt, GenericParameterDefinitionList type_params, CodeGeneratorContext context)
		{
			var tpl = GenericParameterDefinitionList.Merge (type_params, GenericArguments);
			if (!Parameters.Validate (opt, tpl, context))
				return false;
			if (Parameters.Count > 14) {
				Report.Warning (0, Report.WarningMethodBase + 0, "More than 16 parameters were found, which goes beyond the maximum number of parameters. ({0})", context.ContextString);
				return false;
			}
			return true;
		}

		public bool Validate (CodeGenerationOptions opt, GenericParameterDefinitionList type_params, CodeGeneratorContext context)
		{
			context.ContextMethod = this;

			try {
				return IsValid = OnValidate (opt, type_params, context);
			} finally {
				context.ContextMethod = null;
			}
		}
	}
}
