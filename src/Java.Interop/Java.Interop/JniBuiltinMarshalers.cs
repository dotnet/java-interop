﻿#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

using Java.Interop.Expressions;

namespace Java.Interop {

	partial class JniRuntime {

		static readonly Lazy<KeyValuePair<Type, JniTypeSignature>[]> JniBuiltinTypeNameMappings = new Lazy<KeyValuePair<Type, JniTypeSignature>[]> (InitJniBuiltinTypeNameMappings);

		static KeyValuePair<Type, JniTypeSignature>[] InitJniBuiltinTypeNameMappings ()
		{
			return new []{
				new KeyValuePair<Type, JniTypeSignature>(typeof (string),    new JniTypeSignature ("java/lang/String")),

				new KeyValuePair<Type, JniTypeSignature>(typeof (void),      new JniTypeSignature ("V", arrayRank: 0, keyword: true)),
				new KeyValuePair<Type, JniTypeSignature>(typeof (void),      new JniTypeSignature ("java/lang/Void")),

				new KeyValuePair<Type, JniTypeSignature>(typeof (Boolean),     new JniTypeSignature ("Z", 0, keyword: true)),
				new KeyValuePair<Type, JniTypeSignature>(typeof (Boolean?),    new JniTypeSignature ("java/lang/Boolean")),
				new KeyValuePair<Type, JniTypeSignature>(typeof (SByte),     new JniTypeSignature ("B", 0, keyword: true)),
				new KeyValuePair<Type, JniTypeSignature>(typeof (SByte?),    new JniTypeSignature ("java/lang/Byte")),
				new KeyValuePair<Type, JniTypeSignature>(typeof (Char),     new JniTypeSignature ("C", 0, keyword: true)),
				new KeyValuePair<Type, JniTypeSignature>(typeof (Char?),    new JniTypeSignature ("java/lang/Character")),
				new KeyValuePair<Type, JniTypeSignature>(typeof (Int16),     new JniTypeSignature ("S", 0, keyword: true)),
				new KeyValuePair<Type, JniTypeSignature>(typeof (Int16?),    new JniTypeSignature ("java/lang/Short")),
				new KeyValuePair<Type, JniTypeSignature>(typeof (Int32),     new JniTypeSignature ("I", 0, keyword: true)),
				new KeyValuePair<Type, JniTypeSignature>(typeof (Int32?),    new JniTypeSignature ("java/lang/Integer")),
				new KeyValuePair<Type, JniTypeSignature>(typeof (Int64),     new JniTypeSignature ("J", 0, keyword: true)),
				new KeyValuePair<Type, JniTypeSignature>(typeof (Int64?),    new JniTypeSignature ("java/lang/Long")),
				new KeyValuePair<Type, JniTypeSignature>(typeof (Single),     new JniTypeSignature ("F", 0, keyword: true)),
				new KeyValuePair<Type, JniTypeSignature>(typeof (Single?),    new JniTypeSignature ("java/lang/Float")),
				new KeyValuePair<Type, JniTypeSignature>(typeof (Double),     new JniTypeSignature ("D", 0, keyword: true)),
				new KeyValuePair<Type, JniTypeSignature>(typeof (Double?),    new JniTypeSignature ("java/lang/Double")),
			};
		}

		static readonly Lazy<KeyValuePair<Type, JniValueMarshaler>[]> JniBuiltinMarshalers = new Lazy<KeyValuePair<Type, JniValueMarshaler>[]> (InitJniBuiltinMarshalers);

		static KeyValuePair<Type, JniValueMarshaler>[] InitJniBuiltinMarshalers ()
		{
			return new []{
				new KeyValuePair<Type, JniValueMarshaler>(typeof (string), JniStringValueMarshaler.Instance),
				new KeyValuePair<Type, JniValueMarshaler>(typeof (Boolean),   JniBooleanValueMarshaler.Instance),
				new KeyValuePair<Type, JniValueMarshaler>(typeof (Boolean?),  JniNullableBooleanValueMarshaler.Instance),
				new KeyValuePair<Type, JniValueMarshaler>(typeof (SByte),   JniSByteValueMarshaler.Instance),
				new KeyValuePair<Type, JniValueMarshaler>(typeof (SByte?),  JniNullableSByteValueMarshaler.Instance),
				new KeyValuePair<Type, JniValueMarshaler>(typeof (Char),   JniCharValueMarshaler.Instance),
				new KeyValuePair<Type, JniValueMarshaler>(typeof (Char?),  JniNullableCharValueMarshaler.Instance),
				new KeyValuePair<Type, JniValueMarshaler>(typeof (Int16),   JniInt16ValueMarshaler.Instance),
				new KeyValuePair<Type, JniValueMarshaler>(typeof (Int16?),  JniNullableInt16ValueMarshaler.Instance),
				new KeyValuePair<Type, JniValueMarshaler>(typeof (Int32),   JniInt32ValueMarshaler.Instance),
				new KeyValuePair<Type, JniValueMarshaler>(typeof (Int32?),  JniNullableInt32ValueMarshaler.Instance),
				new KeyValuePair<Type, JniValueMarshaler>(typeof (Int64),   JniInt64ValueMarshaler.Instance),
				new KeyValuePair<Type, JniValueMarshaler>(typeof (Int64?),  JniNullableInt64ValueMarshaler.Instance),
				new KeyValuePair<Type, JniValueMarshaler>(typeof (Single),   JniSingleValueMarshaler.Instance),
				new KeyValuePair<Type, JniValueMarshaler>(typeof (Single?),  JniNullableSingleValueMarshaler.Instance),
				new KeyValuePair<Type, JniValueMarshaler>(typeof (Double),   JniDoubleValueMarshaler.Instance),
				new KeyValuePair<Type, JniValueMarshaler>(typeof (Double?),  JniNullableDoubleValueMarshaler.Instance),
			};
		}
	}

	static class JniBoolean {
		internal    const   string  JniTypeName = "java/lang/Boolean";

		static JniType? _TypeRef;
		static JniType TypeRef {
			get {return JniType.GetCachedJniType (ref _TypeRef, JniTypeName);}
		}

		static JniMethodInfo? init;
		internal static unsafe JniObjectReference CreateLocalRef (Boolean value)
		{
			var args    = stackalloc JniArgumentValue [1];
			args [0]    = new JniArgumentValue (value);

			TypeRef.GetCachedConstructor (ref init, "(Z)V");
			return TypeRef.NewObject (init, args);
		}

		static JniMethodInfo? booleanValue;
		internal static Boolean GetValueFromJni (ref JniObjectReference self, JniObjectReferenceOptions transfer, Type? targetType)
		{
			Debug.Assert (targetType == null || targetType == typeof (Boolean), "Expected targetType==typeof(Boolean); was: " + targetType);
			TypeRef.GetCachedInstanceMethod (ref booleanValue, "booleanValue", "()Z");
			try {
				return JniEnvironment.InstanceMethods.CallBooleanMethod (self, booleanValue);
			} finally {
				JniObjectReference.Dispose (ref self, transfer);
			}
		}
	}

	sealed class JniBooleanValueMarshaler : JniValueMarshaler<Boolean> {

		internal    static  readonly    JniBooleanValueMarshaler   Instance    = new JniBooleanValueMarshaler ();

		public override bool IsJniValueType {
			get {return true;}
		}

		public override Type MarshalType {
		    get {return typeof (Boolean);}
		}

		public override object? CreateValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return null;
			return CreateGenericValue (ref reference, options, targetType);
		}

		public override Boolean CreateGenericValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return default (Boolean);

			return JniBoolean.GetValueFromJni (ref reference, options, targetType);
		}

		public override JniValueMarshalerState CreateGenericArgumentState (Boolean value, ParameterAttributes synchronize = ParameterAttributes.In)
		{
			return new JniValueMarshalerState (new JniArgumentValue (value));
		}

		public override JniValueMarshalerState CreateGenericObjectReferenceArgumentState (Boolean value, ParameterAttributes synchronize)
		{
			var r = JniBoolean.CreateLocalRef (value);
			return new JniValueMarshalerState (r);
		}

		public override void DestroyArgumentState (object? value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}

		public override void DestroyGenericArgumentState (Boolean value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}

		public override Expression CreateParameterToManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize, Type? targetType)
		{
		    return sourceValue;
		}

		public override Expression CreateParameterFromManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize)
		{
			return sourceValue;
		}

		public override Expression CreateReturnValueFromManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue)
		{
			return sourceValue;
		}
	}

	sealed class JniNullableBooleanValueMarshaler : JniValueMarshaler<Boolean?> {

		internal    static  readonly    JniNullableBooleanValueMarshaler   Instance    = new JniNullableBooleanValueMarshaler ();

		public override Boolean? CreateGenericValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return null;

			return JniBoolean.GetValueFromJni (ref reference, options, targetType: null);
		}

		public override JniValueMarshalerState CreateGenericObjectReferenceArgumentState (Boolean? value, ParameterAttributes synchronize)
		{
		    if (!value.HasValue)
		        return new JniValueMarshalerState ();
			var r = JniBoolean.CreateLocalRef (value.Value);
			return new JniValueMarshalerState (r);
		}

		public override void DestroyGenericArgumentState (Boolean? value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}
	}

	static class JniByte {
		internal    const   string  JniTypeName = "java/lang/Byte";

		static JniType? _TypeRef;
		static JniType TypeRef {
			get {return JniType.GetCachedJniType (ref _TypeRef, JniTypeName);}
		}

		static JniMethodInfo? init;
		internal static unsafe JniObjectReference CreateLocalRef (SByte value)
		{
			var args    = stackalloc JniArgumentValue [1];
			args [0]    = new JniArgumentValue (value);

			TypeRef.GetCachedConstructor (ref init, "(B)V");
			return TypeRef.NewObject (init, args);
		}

		static JniMethodInfo? byteValue;
		internal static SByte GetValueFromJni (ref JniObjectReference self, JniObjectReferenceOptions transfer, Type? targetType)
		{
			Debug.Assert (targetType == null || targetType == typeof (SByte), "Expected targetType==typeof(SByte); was: " + targetType);
			TypeRef.GetCachedInstanceMethod (ref byteValue, "byteValue", "()B");
			try {
				return JniEnvironment.InstanceMethods.CallByteMethod (self, byteValue);
			} finally {
				JniObjectReference.Dispose (ref self, transfer);
			}
		}
	}

	sealed class JniSByteValueMarshaler : JniValueMarshaler<SByte> {

		internal    static  readonly    JniSByteValueMarshaler   Instance    = new JniSByteValueMarshaler ();

		public override bool IsJniValueType {
			get {return true;}
		}

		public override Type MarshalType {
		    get {return typeof (SByte);}
		}

		public override object? CreateValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return null;
			return CreateGenericValue (ref reference, options, targetType);
		}

		public override SByte CreateGenericValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return default (SByte);

			return JniByte.GetValueFromJni (ref reference, options, targetType);
		}

		public override JniValueMarshalerState CreateGenericArgumentState (SByte value, ParameterAttributes synchronize = ParameterAttributes.In)
		{
			return new JniValueMarshalerState (new JniArgumentValue (value));
		}

		public override JniValueMarshalerState CreateGenericObjectReferenceArgumentState (SByte value, ParameterAttributes synchronize)
		{
			var r = JniByte.CreateLocalRef (value);
			return new JniValueMarshalerState (r);
		}

		public override void DestroyArgumentState (object? value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}

		public override void DestroyGenericArgumentState (SByte value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}

		public override Expression CreateParameterToManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize, Type? targetType)
		{
		    return sourceValue;
		}

		public override Expression CreateParameterFromManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize)
		{
			return sourceValue;
		}

		public override Expression CreateReturnValueFromManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue)
		{
			return sourceValue;
		}
	}

	sealed class JniNullableSByteValueMarshaler : JniValueMarshaler<SByte?> {

		internal    static  readonly    JniNullableSByteValueMarshaler   Instance    = new JniNullableSByteValueMarshaler ();

		public override SByte? CreateGenericValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return null;

			return JniByte.GetValueFromJni (ref reference, options, targetType: null);
		}

		public override JniValueMarshalerState CreateGenericObjectReferenceArgumentState (SByte? value, ParameterAttributes synchronize)
		{
		    if (!value.HasValue)
		        return new JniValueMarshalerState ();
			var r = JniByte.CreateLocalRef (value.Value);
			return new JniValueMarshalerState (r);
		}

		public override void DestroyGenericArgumentState (SByte? value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}
	}

	static class JniCharacter {
		internal    const   string  JniTypeName = "java/lang/Character";

		static JniType? _TypeRef;
		static JniType TypeRef {
			get {return JniType.GetCachedJniType (ref _TypeRef, JniTypeName);}
		}

		static JniMethodInfo? init;
		internal static unsafe JniObjectReference CreateLocalRef (Char value)
		{
			var args    = stackalloc JniArgumentValue [1];
			args [0]    = new JniArgumentValue (value);

			TypeRef.GetCachedConstructor (ref init, "(C)V");
			return TypeRef.NewObject (init, args);
		}

		static JniMethodInfo? charValue;
		internal static Char GetValueFromJni (ref JniObjectReference self, JniObjectReferenceOptions transfer, Type? targetType)
		{
			Debug.Assert (targetType == null || targetType == typeof (Char), "Expected targetType==typeof(Char); was: " + targetType);
			TypeRef.GetCachedInstanceMethod (ref charValue, "charValue", "()C");
			try {
				return JniEnvironment.InstanceMethods.CallCharMethod (self, charValue);
			} finally {
				JniObjectReference.Dispose (ref self, transfer);
			}
		}
	}

	sealed class JniCharValueMarshaler : JniValueMarshaler<Char> {

		internal    static  readonly    JniCharValueMarshaler   Instance    = new JniCharValueMarshaler ();

		public override bool IsJniValueType {
			get {return true;}
		}

		public override Type MarshalType {
		    get {return typeof (Char);}
		}

		public override object? CreateValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return null;
			return CreateGenericValue (ref reference, options, targetType);
		}

		public override Char CreateGenericValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return default (Char);

			return JniCharacter.GetValueFromJni (ref reference, options, targetType);
		}

		public override JniValueMarshalerState CreateGenericArgumentState (Char value, ParameterAttributes synchronize = ParameterAttributes.In)
		{
			return new JniValueMarshalerState (new JniArgumentValue (value));
		}

		public override JniValueMarshalerState CreateGenericObjectReferenceArgumentState (Char value, ParameterAttributes synchronize)
		{
			var r = JniCharacter.CreateLocalRef (value);
			return new JniValueMarshalerState (r);
		}

		public override void DestroyArgumentState (object? value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}

		public override void DestroyGenericArgumentState (Char value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}

		public override Expression CreateParameterToManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize, Type? targetType)
		{
		    return sourceValue;
		}

		public override Expression CreateParameterFromManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize)
		{
			return sourceValue;
		}

		public override Expression CreateReturnValueFromManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue)
		{
			return sourceValue;
		}
	}

	sealed class JniNullableCharValueMarshaler : JniValueMarshaler<Char?> {

		internal    static  readonly    JniNullableCharValueMarshaler   Instance    = new JniNullableCharValueMarshaler ();

		public override Char? CreateGenericValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return null;

			return JniCharacter.GetValueFromJni (ref reference, options, targetType: null);
		}

		public override JniValueMarshalerState CreateGenericObjectReferenceArgumentState (Char? value, ParameterAttributes synchronize)
		{
		    if (!value.HasValue)
		        return new JniValueMarshalerState ();
			var r = JniCharacter.CreateLocalRef (value.Value);
			return new JniValueMarshalerState (r);
		}

		public override void DestroyGenericArgumentState (Char? value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}
	}

	static class JniShort {
		internal    const   string  JniTypeName = "java/lang/Short";

		static JniType? _TypeRef;
		static JniType TypeRef {
			get {return JniType.GetCachedJniType (ref _TypeRef, JniTypeName);}
		}

		static JniMethodInfo? init;
		internal static unsafe JniObjectReference CreateLocalRef (Int16 value)
		{
			var args    = stackalloc JniArgumentValue [1];
			args [0]    = new JniArgumentValue (value);

			TypeRef.GetCachedConstructor (ref init, "(S)V");
			return TypeRef.NewObject (init, args);
		}

		static JniMethodInfo? shortValue;
		internal static Int16 GetValueFromJni (ref JniObjectReference self, JniObjectReferenceOptions transfer, Type? targetType)
		{
			Debug.Assert (targetType == null || targetType == typeof (Int16), "Expected targetType==typeof(Int16); was: " + targetType);
			TypeRef.GetCachedInstanceMethod (ref shortValue, "shortValue", "()S");
			try {
				return JniEnvironment.InstanceMethods.CallShortMethod (self, shortValue);
			} finally {
				JniObjectReference.Dispose (ref self, transfer);
			}
		}
	}

	sealed class JniInt16ValueMarshaler : JniValueMarshaler<Int16> {

		internal    static  readonly    JniInt16ValueMarshaler   Instance    = new JniInt16ValueMarshaler ();

		public override bool IsJniValueType {
			get {return true;}
		}

		public override Type MarshalType {
		    get {return typeof (Int16);}
		}

		public override object? CreateValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return null;
			return CreateGenericValue (ref reference, options, targetType);
		}

		public override Int16 CreateGenericValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return default (Int16);

			return JniShort.GetValueFromJni (ref reference, options, targetType);
		}

		public override JniValueMarshalerState CreateGenericArgumentState (Int16 value, ParameterAttributes synchronize = ParameterAttributes.In)
		{
			return new JniValueMarshalerState (new JniArgumentValue (value));
		}

		public override JniValueMarshalerState CreateGenericObjectReferenceArgumentState (Int16 value, ParameterAttributes synchronize)
		{
			var r = JniShort.CreateLocalRef (value);
			return new JniValueMarshalerState (r);
		}

		public override void DestroyArgumentState (object? value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}

		public override void DestroyGenericArgumentState (Int16 value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}

		public override Expression CreateParameterToManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize, Type? targetType)
		{
		    return sourceValue;
		}

		public override Expression CreateParameterFromManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize)
		{
			return sourceValue;
		}

		public override Expression CreateReturnValueFromManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue)
		{
			return sourceValue;
		}
	}

	sealed class JniNullableInt16ValueMarshaler : JniValueMarshaler<Int16?> {

		internal    static  readonly    JniNullableInt16ValueMarshaler   Instance    = new JniNullableInt16ValueMarshaler ();

		public override Int16? CreateGenericValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return null;

			return JniShort.GetValueFromJni (ref reference, options, targetType: null);
		}

		public override JniValueMarshalerState CreateGenericObjectReferenceArgumentState (Int16? value, ParameterAttributes synchronize)
		{
		    if (!value.HasValue)
		        return new JniValueMarshalerState ();
			var r = JniShort.CreateLocalRef (value.Value);
			return new JniValueMarshalerState (r);
		}

		public override void DestroyGenericArgumentState (Int16? value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}
	}

	static class JniInteger {
		internal    const   string  JniTypeName = "java/lang/Integer";

		static JniType? _TypeRef;
		static JniType TypeRef {
			get {return JniType.GetCachedJniType (ref _TypeRef, JniTypeName);}
		}

		static JniMethodInfo? init;
		internal static unsafe JniObjectReference CreateLocalRef (Int32 value)
		{
			var args    = stackalloc JniArgumentValue [1];
			args [0]    = new JniArgumentValue (value);

			TypeRef.GetCachedConstructor (ref init, "(I)V");
			return TypeRef.NewObject (init, args);
		}

		static JniMethodInfo? intValue;
		internal static Int32 GetValueFromJni (ref JniObjectReference self, JniObjectReferenceOptions transfer, Type? targetType)
		{
			Debug.Assert (targetType == null || targetType == typeof (Int32), "Expected targetType==typeof(Int32); was: " + targetType);
			TypeRef.GetCachedInstanceMethod (ref intValue, "intValue", "()I");
			try {
				return JniEnvironment.InstanceMethods.CallIntMethod (self, intValue);
			} finally {
				JniObjectReference.Dispose (ref self, transfer);
			}
		}
	}

	sealed class JniInt32ValueMarshaler : JniValueMarshaler<Int32> {

		internal    static  readonly    JniInt32ValueMarshaler   Instance    = new JniInt32ValueMarshaler ();

		public override bool IsJniValueType {
			get {return true;}
		}

		public override Type MarshalType {
		    get {return typeof (Int32);}
		}

		public override object? CreateValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return null;
			return CreateGenericValue (ref reference, options, targetType);
		}

		public override Int32 CreateGenericValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return default (Int32);

			return JniInteger.GetValueFromJni (ref reference, options, targetType);
		}

		public override JniValueMarshalerState CreateGenericArgumentState (Int32 value, ParameterAttributes synchronize = ParameterAttributes.In)
		{
			return new JniValueMarshalerState (new JniArgumentValue (value));
		}

		public override JniValueMarshalerState CreateGenericObjectReferenceArgumentState (Int32 value, ParameterAttributes synchronize)
		{
			var r = JniInteger.CreateLocalRef (value);
			return new JniValueMarshalerState (r);
		}

		public override void DestroyArgumentState (object? value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}

		public override void DestroyGenericArgumentState (Int32 value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}

		public override Expression CreateParameterToManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize, Type? targetType)
		{
		    return sourceValue;
		}

		public override Expression CreateParameterFromManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize)
		{
			return sourceValue;
		}

		public override Expression CreateReturnValueFromManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue)
		{
			return sourceValue;
		}
	}

	sealed class JniNullableInt32ValueMarshaler : JniValueMarshaler<Int32?> {

		internal    static  readonly    JniNullableInt32ValueMarshaler   Instance    = new JniNullableInt32ValueMarshaler ();

		public override Int32? CreateGenericValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return null;

			return JniInteger.GetValueFromJni (ref reference, options, targetType: null);
		}

		public override JniValueMarshalerState CreateGenericObjectReferenceArgumentState (Int32? value, ParameterAttributes synchronize)
		{
		    if (!value.HasValue)
		        return new JniValueMarshalerState ();
			var r = JniInteger.CreateLocalRef (value.Value);
			return new JniValueMarshalerState (r);
		}

		public override void DestroyGenericArgumentState (Int32? value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}
	}

	static class JniLong {
		internal    const   string  JniTypeName = "java/lang/Long";

		static JniType? _TypeRef;
		static JniType TypeRef {
			get {return JniType.GetCachedJniType (ref _TypeRef, JniTypeName);}
		}

		static JniMethodInfo? init;
		internal static unsafe JniObjectReference CreateLocalRef (Int64 value)
		{
			var args    = stackalloc JniArgumentValue [1];
			args [0]    = new JniArgumentValue (value);

			TypeRef.GetCachedConstructor (ref init, "(J)V");
			return TypeRef.NewObject (init, args);
		}

		static JniMethodInfo? longValue;
		internal static Int64 GetValueFromJni (ref JniObjectReference self, JniObjectReferenceOptions transfer, Type? targetType)
		{
			Debug.Assert (targetType == null || targetType == typeof (Int64), "Expected targetType==typeof(Int64); was: " + targetType);
			TypeRef.GetCachedInstanceMethod (ref longValue, "longValue", "()J");
			try {
				return JniEnvironment.InstanceMethods.CallLongMethod (self, longValue);
			} finally {
				JniObjectReference.Dispose (ref self, transfer);
			}
		}
	}

	sealed class JniInt64ValueMarshaler : JniValueMarshaler<Int64> {

		internal    static  readonly    JniInt64ValueMarshaler   Instance    = new JniInt64ValueMarshaler ();

		public override bool IsJniValueType {
			get {return true;}
		}

		public override Type MarshalType {
		    get {return typeof (Int64);}
		}

		public override object? CreateValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return null;
			return CreateGenericValue (ref reference, options, targetType);
		}

		public override Int64 CreateGenericValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return default (Int64);

			return JniLong.GetValueFromJni (ref reference, options, targetType);
		}

		public override JniValueMarshalerState CreateGenericArgumentState (Int64 value, ParameterAttributes synchronize = ParameterAttributes.In)
		{
			return new JniValueMarshalerState (new JniArgumentValue (value));
		}

		public override JniValueMarshalerState CreateGenericObjectReferenceArgumentState (Int64 value, ParameterAttributes synchronize)
		{
			var r = JniLong.CreateLocalRef (value);
			return new JniValueMarshalerState (r);
		}

		public override void DestroyArgumentState (object? value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}

		public override void DestroyGenericArgumentState (Int64 value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}

		public override Expression CreateParameterToManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize, Type? targetType)
		{
		    return sourceValue;
		}

		public override Expression CreateParameterFromManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize)
		{
			return sourceValue;
		}

		public override Expression CreateReturnValueFromManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue)
		{
			return sourceValue;
		}
	}

	sealed class JniNullableInt64ValueMarshaler : JniValueMarshaler<Int64?> {

		internal    static  readonly    JniNullableInt64ValueMarshaler   Instance    = new JniNullableInt64ValueMarshaler ();

		public override Int64? CreateGenericValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return null;

			return JniLong.GetValueFromJni (ref reference, options, targetType: null);
		}

		public override JniValueMarshalerState CreateGenericObjectReferenceArgumentState (Int64? value, ParameterAttributes synchronize)
		{
		    if (!value.HasValue)
		        return new JniValueMarshalerState ();
			var r = JniLong.CreateLocalRef (value.Value);
			return new JniValueMarshalerState (r);
		}

		public override void DestroyGenericArgumentState (Int64? value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}
	}

	static class JniFloat {
		internal    const   string  JniTypeName = "java/lang/Float";

		static JniType? _TypeRef;
		static JniType TypeRef {
			get {return JniType.GetCachedJniType (ref _TypeRef, JniTypeName);}
		}

		static JniMethodInfo? init;
		internal static unsafe JniObjectReference CreateLocalRef (Single value)
		{
			var args    = stackalloc JniArgumentValue [1];
			args [0]    = new JniArgumentValue (value);

			TypeRef.GetCachedConstructor (ref init, "(F)V");
			return TypeRef.NewObject (init, args);
		}

		static JniMethodInfo? floatValue;
		internal static Single GetValueFromJni (ref JniObjectReference self, JniObjectReferenceOptions transfer, Type? targetType)
		{
			Debug.Assert (targetType == null || targetType == typeof (Single), "Expected targetType==typeof(Single); was: " + targetType);
			TypeRef.GetCachedInstanceMethod (ref floatValue, "floatValue", "()F");
			try {
				return JniEnvironment.InstanceMethods.CallFloatMethod (self, floatValue);
			} finally {
				JniObjectReference.Dispose (ref self, transfer);
			}
		}
	}

	sealed class JniSingleValueMarshaler : JniValueMarshaler<Single> {

		internal    static  readonly    JniSingleValueMarshaler   Instance    = new JniSingleValueMarshaler ();

		public override bool IsJniValueType {
			get {return true;}
		}

		public override Type MarshalType {
		    get {return typeof (Single);}
		}

		public override object? CreateValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return null;
			return CreateGenericValue (ref reference, options, targetType);
		}

		public override Single CreateGenericValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return default (Single);

			return JniFloat.GetValueFromJni (ref reference, options, targetType);
		}

		public override JniValueMarshalerState CreateGenericArgumentState (Single value, ParameterAttributes synchronize = ParameterAttributes.In)
		{
			return new JniValueMarshalerState (new JniArgumentValue (value));
		}

		public override JniValueMarshalerState CreateGenericObjectReferenceArgumentState (Single value, ParameterAttributes synchronize)
		{
			var r = JniFloat.CreateLocalRef (value);
			return new JniValueMarshalerState (r);
		}

		public override void DestroyArgumentState (object? value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}

		public override void DestroyGenericArgumentState (Single value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}

		public override Expression CreateParameterToManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize, Type? targetType)
		{
		    return sourceValue;
		}

		public override Expression CreateParameterFromManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize)
		{
			return sourceValue;
		}

		public override Expression CreateReturnValueFromManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue)
		{
			return sourceValue;
		}
	}

	sealed class JniNullableSingleValueMarshaler : JniValueMarshaler<Single?> {

		internal    static  readonly    JniNullableSingleValueMarshaler   Instance    = new JniNullableSingleValueMarshaler ();

		public override Single? CreateGenericValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return null;

			return JniFloat.GetValueFromJni (ref reference, options, targetType: null);
		}

		public override JniValueMarshalerState CreateGenericObjectReferenceArgumentState (Single? value, ParameterAttributes synchronize)
		{
		    if (!value.HasValue)
		        return new JniValueMarshalerState ();
			var r = JniFloat.CreateLocalRef (value.Value);
			return new JniValueMarshalerState (r);
		}

		public override void DestroyGenericArgumentState (Single? value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}
	}

	static class JniDouble {
		internal    const   string  JniTypeName = "java/lang/Double";

		static JniType? _TypeRef;
		static JniType TypeRef {
			get {return JniType.GetCachedJniType (ref _TypeRef, JniTypeName);}
		}

		static JniMethodInfo? init;
		internal static unsafe JniObjectReference CreateLocalRef (Double value)
		{
			var args    = stackalloc JniArgumentValue [1];
			args [0]    = new JniArgumentValue (value);

			TypeRef.GetCachedConstructor (ref init, "(D)V");
			return TypeRef.NewObject (init, args);
		}

		static JniMethodInfo? doubleValue;
		internal static Double GetValueFromJni (ref JniObjectReference self, JniObjectReferenceOptions transfer, Type? targetType)
		{
			Debug.Assert (targetType == null || targetType == typeof (Double), "Expected targetType==typeof(Double); was: " + targetType);
			TypeRef.GetCachedInstanceMethod (ref doubleValue, "doubleValue", "()D");
			try {
				return JniEnvironment.InstanceMethods.CallDoubleMethod (self, doubleValue);
			} finally {
				JniObjectReference.Dispose (ref self, transfer);
			}
		}
	}

	sealed class JniDoubleValueMarshaler : JniValueMarshaler<Double> {

		internal    static  readonly    JniDoubleValueMarshaler   Instance    = new JniDoubleValueMarshaler ();

		public override bool IsJniValueType {
			get {return true;}
		}

		public override Type MarshalType {
		    get {return typeof (Double);}
		}

		public override object? CreateValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return null;
			return CreateGenericValue (ref reference, options, targetType);
		}

		public override Double CreateGenericValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return default (Double);

			return JniDouble.GetValueFromJni (ref reference, options, targetType);
		}

		public override JniValueMarshalerState CreateGenericArgumentState (Double value, ParameterAttributes synchronize = ParameterAttributes.In)
		{
			return new JniValueMarshalerState (new JniArgumentValue (value));
		}

		public override JniValueMarshalerState CreateGenericObjectReferenceArgumentState (Double value, ParameterAttributes synchronize)
		{
			var r = JniDouble.CreateLocalRef (value);
			return new JniValueMarshalerState (r);
		}

		public override void DestroyArgumentState (object? value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}

		public override void DestroyGenericArgumentState (Double value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}

		public override Expression CreateParameterToManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize, Type? targetType)
		{
		    return sourceValue;
		}

		public override Expression CreateParameterFromManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue, ParameterAttributes synchronize)
		{
			return sourceValue;
		}

		public override Expression CreateReturnValueFromManagedExpression (JniValueMarshalerContext context, ParameterExpression sourceValue)
		{
			return sourceValue;
		}
	}

	sealed class JniNullableDoubleValueMarshaler : JniValueMarshaler<Double?> {

		internal    static  readonly    JniNullableDoubleValueMarshaler   Instance    = new JniNullableDoubleValueMarshaler ();

		public override Double? CreateGenericValue (ref JniObjectReference reference, JniObjectReferenceOptions options, Type? targetType)
		{
			if (!reference.IsValid)
				return null;

			return JniDouble.GetValueFromJni (ref reference, options, targetType: null);
		}

		public override JniValueMarshalerState CreateGenericObjectReferenceArgumentState (Double? value, ParameterAttributes synchronize)
		{
		    if (!value.HasValue)
		        return new JniValueMarshalerState ();
			var r = JniDouble.CreateLocalRef (value.Value);
			return new JniValueMarshalerState (r);
		}

		public override void DestroyGenericArgumentState (Double? value, ref JniValueMarshalerState state, ParameterAttributes synchronize)
		{
			var r   = state.ReferenceValue;
			JniObjectReference.Dispose (ref r);
			state   = new JniValueMarshalerState ();
		}
	}
}
