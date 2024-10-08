#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace Java.Interop {

	[JniTypeSignature (JniTypeName, GenerateJavaPeer=false)]
	/* static */ sealed class ManagedPeer : JavaObject {

		internal const string JniTypeName = "net/dot/jni/ManagedPeer";
		internal const DynamicallyAccessedMemberTypes ConstructorsMethodsNestedTypes = Constructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods | DynamicallyAccessedMemberTypes.NonPublicNestedTypes;


		static  readonly    JniPeerMembers  _members        = new JniPeerMembers (JniTypeName, typeof (ManagedPeer));

		static ManagedPeer ()
		{
			_members.JniPeerType.RegisterNativeMethods (
					new JniNativeMethodRegistration (
						"construct",
						ConstructSignature,
						new ConstructMarshalMethod (Construct)),
					new JniNativeMethodRegistration (
						"registerNativeMembers",
						RegisterNativeMembersSignature,
						new RegisterMarshalMethod (RegisterNativeMembers))
			);
		}

		ManagedPeer ()
		{
		}

		internal static void Init ()
		{
			// Present so that JniRuntime has _something_ to reference to
			// prompt invocation of the static constructor & registration
		}

		public override JniPeerMembers JniPeerMembers {
			get {return _members;}
		}

		const string ConstructSignature = "(Ljava/lang/Object;Ljava/lang/String;[Ljava/lang/Object;)V";

		// TODO: Keep in sync with the code generated by ExportedMemberBuilder
		[UnmanagedFunctionPointer (CallingConvention.Winapi)]
		delegate void ConstructMarshalMethod (IntPtr jnienv,
				IntPtr klass,
				IntPtr n_self,
				IntPtr n_constructorSignature,
				IntPtr n_constructorArguments);
		static void Construct (
				IntPtr jnienv,
				IntPtr klass,
				IntPtr n_self,
				IntPtr n_constructorSignature,
				IntPtr n_constructorArguments)
		{
			var envp = new JniTransition (jnienv);
			try {
				var runtime = JniEnvironment.Runtime;
				var r_self  = new JniObjectReference (n_self);
				var self    = runtime.ValueManager.PeekPeer (r_self);
				if (self != null) {
					var state   = self.JniManagedPeerState;
					if ((state & JniManagedPeerStates.Activatable) != JniManagedPeerStates.Activatable &&
							(state & JniManagedPeerStates.Replaceable) != JniManagedPeerStates.Replaceable) {
						return;
					}
				}

				if (JniEnvironment.WithinNewObjectScope) {
					if (runtime.ObjectReferenceManager.LogGlobalReferenceMessages) {
						runtime.ObjectReferenceManager.WriteGlobalReferenceLine (
								"Warning: Skipping managed constructor invocation for PeerReference={0} IdentityHashCode=0x{1} Java.Type={2}. " +
								"Please use JniPeerMembers.InstanceMethods.StartCreateInstance() + JniPeerMembers.InstanceMethods.FinishCreateInstance() instead of " +
								"JniEnvironment.Object.NewObject().",
								r_self,
								runtime.ValueManager.GetJniIdentityHashCode (r_self).ToString ("x"),
								JniEnvironment.Types.GetJniTypeNameFromInstance (r_self));
					}
					return;
				}

				var typeSig = new JniTypeSignature (JniEnvironment.Types.GetJniTypeNameFromInstance (r_self));
				var type    = GetTypeFromSignature (runtime.TypeManager, typeSig);

				if (type.IsGenericTypeDefinition) {
					throw new NotSupportedException (
							"Constructing instances of generic types from Java is not supported, as the type parameters cannot be determined.",
							CreateJniLocationException ());
				}

				var ctorSig = JniEnvironment.Strings.ToString (n_constructorSignature) ?? "()V";
				var cinfo   = GetConstructor (type, typeSig.SimpleReference!, ctorSig) ??
					throw CreateMissingConstructorException (type, ctorSig);
				var pvalues = GetValues (runtime, new JniObjectReference (n_constructorArguments), cinfo);

				if (self != null) {
					cinfo.Invoke (self, pvalues);
					return;
				}

				JniEnvironment.Runtime.ValueManager.ActivatePeer (self, new JniObjectReference (n_self), cinfo, pvalues);
			}
			catch (Exception e) when (JniEnvironment.Runtime.ExceptionShouldTransitionToJni (e)) {
				envp.SetPendingException (e);
			}
			finally {
				envp.Dispose ();
			}
		}

		static Exception CreateMissingConstructorException (Type type, string signature)
		{
			var message = new StringBuilder ();
			message.Append ("Unable to find constructor for type `");
			message.Append (type.FullName);
			message.Append ("` with JNI signature `");
			message.Append (signature);
			message.Append ("`. Please provide the missing constructor.");

			return new NotSupportedException (message.ToString (), CreateJniLocationException ());
		}

		static Exception CreateJniLocationException ()
		{
			using (var e = new JavaException ()) {
				return new JniLocationException (e.ToString ());
			}
		}

		static Dictionary<string, ConstructorInfo?> ConstructorCache    = new Dictionary<string, ConstructorInfo?> ();

		static ConstructorInfo? GetConstructor (
				[DynamicallyAccessedMembers (Constructors)]
				Type type,
				string jniTypeName,
				string signature)
		{
			var ctorCacheKey    = jniTypeName + "." + signature;
			lock (ConstructorCache) {
				if (ConstructorCache.TryGetValue (ctorCacheKey, out var ctor)) {
					return ctor;
				}
			}

			var candidateParameterTypes = GetConstructorCandidateParameterTypes (signature);
			if (candidateParameterTypes.Length == 0) {
				return CacheConstructor (ctorCacheKey, type.GetConstructor (Array.Empty<Type> ()));
			}

			var constructors    = new List<ConstructorInfo>(type.GetConstructors (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

			// Filter out wrong parameter count
			for (int c = constructors.Count; c > 0; --c) {
				if (constructors [c-1].GetParameters ().Length != candidateParameterTypes.Length) {
					constructors.RemoveAt (c-1);
				}
			}

			// Filter out mismatched types
			for (int c = constructors.Count; c > 0; --c) {
				var parameters  = constructors [c-1].GetParameters ();
				for (int i = 0; i < parameters.Length; ++i) {
					if (!candidateParameterTypes [i].Contains (parameters [i].ParameterType)) {
						constructors.RemoveAt (c-1);
						break;
					}
				}
			}

			if (constructors.Count == 0)
				return CacheConstructor (ctorCacheKey, null);

			if (constructors.Count != 1) {
				var message = new StringBuilder ($"Found {constructors.Count} constructors matching JNI signature {signature}:")
					.Append (Environment.NewLine);
				foreach (var c in constructors) {
					message.Append ("  ").Append (c).Append (Environment.NewLine);
				}
				throw new NotSupportedException (message.ToString (), CreateJniLocationException ());
			}

			return CacheConstructor (ctorCacheKey, constructors [0]);
		}

		static ConstructorInfo? CacheConstructor (string cacheKey, ConstructorInfo? ctor)
		{
			lock (ConstructorCache) {
				if (ConstructorCache.TryGetValue (cacheKey, out var existing)) {
					return existing;
				}
				ConstructorCache.Add (cacheKey, ctor);
			}
			return ctor;
		}

		static List<Type>[] GetConstructorCandidateParameterTypes (string signature)
		{
			var parameterCount  = JniMemberSignature.GetParameterCountFromMethodSignature (signature);
			if (parameterCount == 0) {
				return Array.Empty<List<Type>> ();
			}
			var typeManager             = JniEnvironment.Runtime.TypeManager;
			var candidateParameterTypes = new List<Type>[parameterCount];
			int i                       = 0;
			foreach (var jniType in JniMemberSignature.GetParameterTypesFromMethodSignature (signature)) {
				var possibleTypes       = new List<Type> (typeManager.GetTypes (jniType));
				if (possibleTypes.Count == 0) {
					throw new NotSupportedException (
							$"Could not find System.Type corresponding to Java type `{jniType}` within constructor signature `{signature}`.",
							CreateJniLocationException ());
				}
				candidateParameterTypes [i++]   = possibleTypes;
			}
			return candidateParameterTypes;
		}

		static object?[]? GetValues (JniRuntime runtime, JniObjectReference values, ConstructorInfo cinfo)
		{
			// https://github.com/xamarin/xamarin-android/blob/5472eec991cc075e4b0c09cd98a2331fb93aa0f3/src/Microsoft.Android.Sdk.ILLink/MarkJavaObjects.cs#L51-L132
			[UnconditionalSuppressMessage ("Trimming", "IL2072", Justification = "Constructors are preserved by the MarkJavaObjects trimmer step.")]
			static object? ValueManagerGetValue (JniRuntime runtime, ref JniObjectReference value, ParameterInfo parameter) =>
				runtime.ValueManager.GetValue (ref value, JniObjectReferenceOptions.CopyAndDispose, parameter.ParameterType);

			if (!values.IsValid)
				return null;

			var parameters  = cinfo.GetParameters ();

			int len = JniEnvironment.Arrays.GetArrayLength (values);
			Debug.Assert (len == parameters.Length,
					$"Unexpected number of parameter types! Expected {parameters.Length}, got {len}");
			var pvalues = new object? [len];
			for (int i = 0; i < len; ++i) {
				var n_value = JniEnvironment.Arrays.GetObjectArrayElement (values, i);
				var value   = ValueManagerGetValue (runtime, ref n_value, parameters [i]);
				pvalues [i] = value;
			}

			return pvalues;
		}

		const   string  RegisterNativeMembersSignature  = "(Ljava/lang/Class;Ljava/lang/String;)V";

		[UnmanagedFunctionPointer (CallingConvention.Winapi)]
		delegate void RegisterMarshalMethod (IntPtr jnienv,
				IntPtr klass,
				IntPtr n_nativeClass,
				IntPtr n_methods);
		static unsafe void RegisterNativeMembers (
				IntPtr jnienv,
				IntPtr klass,
				IntPtr n_nativeClass,
				IntPtr n_methods)
		{
			var envp = new JniTransition (jnienv);
			try {
				var r_nativeClass   = new JniObjectReference (n_nativeClass);
#pragma warning disable CA2000
				var nativeClass     = new JniType (ref r_nativeClass, JniObjectReferenceOptions.Copy);
#pragma warning restore CA2000

				var methodsRef              = new JniObjectReference (n_methods);

				var typeSig                 = new JniTypeSignature (nativeClass.Name);
				var type                    = GetTypeFromSignature (JniEnvironment.Runtime.TypeManager, typeSig);

#if NET
				int methodsLength           = JniEnvironment.Strings.GetStringLength (methodsRef);
				var methodsChars            = JniEnvironment.Strings.GetStringChars (methodsRef, null);
				var methods                 = new ReadOnlySpan<char>(methodsChars, methodsLength);
				try {
					JniEnvironment.Runtime.TypeManager.RegisterNativeMembers (nativeClass, type, methods);
				}
				finally {
					JniEnvironment.Strings.ReleaseStringChars (methodsRef, methodsChars);
				}
#else   // NET
				var methods                 = JniEnvironment.Strings.ToString (methodsRef);
				JniEnvironment.Runtime.TypeManager.RegisterNativeMembers (nativeClass, type, methods);
#endif  // NET

			}
			catch (Exception e) when (JniEnvironment.Runtime.ExceptionShouldTransitionToJni (e)) {
				Debug.WriteLine ($"Exception when trying to register native methods with JNI: {e}");
				envp.SetPendingException (e);
			}
			finally {
				envp.Dispose ();
			}
		}

		[return: DynamicallyAccessedMembers (ConstructorsMethodsNestedTypes)]
		static Type GetTypeFromSignature (JniRuntime.JniTypeManager typeManager, JniTypeSignature typeSignature, string? context = null)
		{
			return typeManager.GetType (typeSignature) ??
				throw new NotSupportedException ($"Could not find System.Type corresponding to Java type {typeSignature} {(context == null ? "" : "within `" + context + "`")}.");
		}
	}

	sealed class JniLocationException : Exception {

		string stackTrace;

		public JniLocationException (string stackTrace)
		{
			this.stackTrace = stackTrace;
		}

		public override string StackTrace {
			get {
				return stackTrace;
			}
		}
	}
}

