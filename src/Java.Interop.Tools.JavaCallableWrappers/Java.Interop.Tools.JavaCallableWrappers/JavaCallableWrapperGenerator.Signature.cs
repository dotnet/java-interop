using System;
using System.Text;

using Mono.Cecil;

using Android.Runtime;
using Java.Interop.Tools.Diagnostics;
using Java.Interop.Tools.TypeNameMappings;

using MethodAttributes = Mono.Cecil.MethodAttributes;
using static Java.Interop.Tools.TypeNameMappings.JavaNativeTypeManager;

namespace Java.Interop.Tools.JavaCallableWrappers
{

	public partial class JavaCallableWrapperGenerator {
		internal class Signature {

			public Signature (MethodDefinition method, RegisterAttribute register, IMetadataResolver cache, bool shouldBeDynamicallyRegistered = true) : this (method, register, null, null, cache, shouldBeDynamicallyRegistered) {}

			public Signature (MethodDefinition method, RegisterAttribute register, string? managedParameters, string? outerType, IMetadataResolver cache, bool shouldBeDynamicallyRegistered = true)
				: this (register.Name, register.Signature, register.Connector, managedParameters, outerType, null)
			{
				Annotations = JavaCallableWrapperGenerator.GetAnnotationsString ("\t", method.CustomAttributes, cache);
				IsDynamicallyRegistered = shouldBeDynamicallyRegistered;
			}

			public Signature (MethodDefinition method, ExportAttribute export, string? managedParameters, IMetadataResolver cache)
				: this (method.Name, GetJniSignature (method, cache), "__export__", null, null, export.SuperArgumentsString)
			{
				IsExport = true;
				IsStatic = method.IsStatic;
				JavaAccess = JavaCallableWrapperGenerator.GetJavaAccess (method.Attributes & MethodAttributes.MemberAccessMask);
				ThrownTypeNames = export.ThrownNames;
				JavaNameOverride = export.Name;
				ManagedParameters = managedParameters;
				Annotations = JavaCallableWrapperGenerator.GetAnnotationsString ("\t", method.CustomAttributes, cache);
			}

			public Signature (MethodDefinition method, ExportFieldAttribute exportField, IMetadataResolver cache)
				: this (method.Name, GetJniSignature (method, cache), "__export__", null, null, null)
			{
				if (method.HasParameters)
					Diagnostic.Error (4205, JavaCallableWrapperGenerator.LookupSource (method), Localization.Resources.JavaCallableWrappers_XA4205);
				if (method.ReturnType.MetadataType == MetadataType.Void)
					Diagnostic.Error (4208, JavaCallableWrapperGenerator.LookupSource (method), Localization.Resources.JavaCallableWrappers_XA4208);
				IsExport = true;
				IsStatic = method.IsStatic;
				JavaAccess = JavaCallableWrapperGenerator.GetJavaAccess (method.Attributes & MethodAttributes.MemberAccessMask);

				// annotations are processed within JavaFieldInfo, not the initializer method. So we don't generate them here.
			}

			public Signature (string name, string? signature, string? connector, string? managedParameters, string? outerType, string? superCall)
			{
				ManagedParameters = managedParameters;
				JniSignature      = signature ?? throw new ArgumentNullException ("`connector` cannot be null.", nameof (connector));
				Method    = "n_" + name + ":" + JniSignature + ":" + connector;
				Name      = name;

				var jnisig = JniSignature;
				int closer = jnisig.IndexOf (')');
				string ret = jnisig.Substring (closer + 1);
				retval = JavaNativeTypeManager.Parse (ret)?.Type;
				string jniparms = jnisig.Substring (1, closer - 1);
				if (string.IsNullOrEmpty (jniparms) && string.IsNullOrEmpty (superCall))
					return;
				var parms = new StringBuilder ();
				var scall = new StringBuilder ();
				var acall = new StringBuilder ();
				bool first = true;
				int i = 0;
				foreach (JniTypeName jti in JavaNativeTypeManager.FromSignature (jniparms)) {
					if (outerType != null) {
						acall.Append (outerType).Append (".this");
						outerType = null;
						continue;
					}
					string? parmType = jti.Type;
					if (!first) {
						parms.Append (", ");
						scall.Append (", ");
						acall.Append (", ");
					}
					first = false;
					parms.Append (parmType).Append (" p").Append (i);
					scall.Append ("p").Append (i);
					acall.Append ("p").Append (i);
					++i;
				}
				this.parms = parms.ToString ();
				this.call  = superCall != null ? superCall : scall.ToString ();
				this.ActivateCall = acall.ToString ();
			}

			string? call;
			public string? SuperCall {
				get { return call; }
			}

			public string? ActivateCall {get; private set;}

			public readonly string Name;
			public readonly string? JavaNameOverride;
			public string JavaName {
				get { return JavaNameOverride ?? Name; }
			}

			string? parms;
			public string? Params {
				get { return parms; }
			}

			string? retval;
			public string? Retval {
				get { return retval; }
			}

			public string? ThrowsDeclaration {
				get { return ThrownTypeNames?.Length > 0 ? " throws " + String.Join (", ", ThrownTypeNames) : null; }
			}

			public readonly string? JavaAccess;
			public readonly string? ManagedParameters;
			public readonly string JniSignature;
			public readonly string Method;
			public readonly bool IsExport;
			public readonly bool IsStatic;
			public readonly bool IsDynamicallyRegistered = true;
			public readonly string []? ThrownTypeNames;
			public readonly string? Annotations;
		}
	}
}
