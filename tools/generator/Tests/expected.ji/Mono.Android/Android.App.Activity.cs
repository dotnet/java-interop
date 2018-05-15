using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Android.App {

	// Metadata.xml XPath class reference: path="/api/package[@name='android.app']/class[@name='Activity']"
	[global::Android.Runtime.Register ("android/app/Activity", DoNotGenerateAcw=true)]
	public partial class Activity : global::Java.Lang.Object {


		// Metadata.xml XPath field reference: path="/api/package[@name='android.app']/class[@name='Activity']/field[@name='DEFAULT_KEYS_DIALER']"
		[Register ("DEFAULT_KEYS_DIALER")]
		public const int DefaultKeysDialer = (int) 1;

		// Metadata.xml XPath field reference: path="/api/package[@name='android.app']/class[@name='Activity']/field[@name='DEFAULT_KEYS_DISABLE']"
		[Register ("DEFAULT_KEYS_DISABLE")]
		public const int DefaultKeysDisable = (int) 0;

		// Metadata.xml XPath field reference: path="/api/package[@name='android.app']/class[@name='Activity']/field[@name='DEFAULT_KEYS_SEARCH_GLOBAL']"
		[Register ("DEFAULT_KEYS_SEARCH_GLOBAL")]
		public const int DefaultKeysSearchGlobal = (int) 4;

		// Metadata.xml XPath field reference: path="/api/package[@name='android.app']/class[@name='Activity']/field[@name='DEFAULT_KEYS_SEARCH_LOCAL']"
		[Register ("DEFAULT_KEYS_SEARCH_LOCAL")]
		public const int DefaultKeysSearchLocal = (int) 3;

		// Metadata.xml XPath field reference: path="/api/package[@name='android.app']/class[@name='Activity']/field[@name='DEFAULT_KEYS_SHORTCUT']"
		[Register ("DEFAULT_KEYS_SHORTCUT")]
		public const int DefaultKeysShortcut = (int) 2;


		// Metadata.xml XPath field reference: path="/api/package[@name='android.app']/class[@name='Activity']/field[@name='FOCUSED_STATE_SET']"
		[Register ("FOCUSED_STATE_SET")]
		protected static IList<int> FocusedStateSet {
			get {
				const string __id = "FOCUSED_STATE_SET.[I";

				var __v = _members.StaticFields.GetObjectValue (__id);
				return global::Android.Runtime.JavaArray<int>.FromJniHandle (__v.Handle, JniHandleOwnership.TransferLocalRef);
			}
		}

		// Metadata.xml XPath field reference: path="/api/package[@name='android.app']/class[@name='Activity']/field[@name='RESULT_CANCELED']"
		[Register ("RESULT_CANCELED")]
		public const int ResultCanceled = (int) 0;

		// Metadata.xml XPath field reference: path="/api/package[@name='android.app']/class[@name='Activity']/field[@name='RESULT_FIRST_USER']"
		[Register ("RESULT_FIRST_USER")]
		public const int ResultFirstUser = (int) 1;

		// Metadata.xml XPath field reference: path="/api/package[@name='android.app']/class[@name='Activity']/field[@name='RESULT_OK']"
		[Register ("RESULT_OK")]
		public const int ResultOk = (int) -1;
		internal            static  readonly    JniPeerMembers  _members    = new JniPeerMembers ("android/app/Activity", typeof (Activity));
		internal static IntPtr class_ref {
			get {
				return _members.JniPeerType.PeerReference.Handle;
			}
		}

		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return _members; }
		}

		protected override IntPtr ThresholdClass {
			get { return _members.JniPeerType.PeerReference.Handle; }
		}

		protected override global::System.Type ThresholdType {
			get { return _members.ManagedPeerType; }
		}

		protected Activity (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		// Metadata.xml XPath constructor reference: path="/api/package[@name='android.app']/class[@name='Activity']/constructor[@name='Activity' and count(parameter)=0]"
		[Register (".ctor", "()V", "")]
		public unsafe Activity ()
			: base (IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
		{
			const string __id = "()V";

			if (((global::Java.Lang.Object) this).Handle != IntPtr.Zero)
				return;

			try {
				var __r = _members.InstanceMethods.StartCreateInstance (__id, ((object) this).GetType (), null);
				SetHandle (__r.Handle, JniHandleOwnership.TransferLocalRef);
				_members.InstanceMethods.FinishCreateInstance (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_getCallingPackage;
#pragma warning disable 0169
		static Delegate GetGetCallingPackageHandler ()
		{
			if (cb_getCallingPackage == null)
				cb_getCallingPackage = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, IntPtr>) n_GetCallingPackage);
			return cb_getCallingPackage;
		}

		static IntPtr n_GetCallingPackage (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return JNIEnv.NewString (__this.CallingPackage);
		}
#pragma warning restore 0169

		public virtual unsafe string CallingPackage {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='getCallingPackage' and count(parameter)=0]"
			[Register ("getCallingPackage", "()Ljava/lang/String;", "GetGetCallingPackageHandler")]
			get {
				const string __id = "getCallingPackage.()Ljava/lang/String;";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, null);
					return JNIEnv.GetString (__rm.Handle, JniHandleOwnership.TransferLocalRef);
				} finally {
				}
			}
		}

		static Delegate cb_getChangingConfigurations;
#pragma warning disable 0169
		static Delegate GetGetChangingConfigurationsHandler ()
		{
			if (cb_getChangingConfigurations == null)
				cb_getChangingConfigurations = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, int>) n_GetChangingConfigurations);
			return cb_getChangingConfigurations;
		}

		static int n_GetChangingConfigurations (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.ChangingConfigurations;
		}
#pragma warning restore 0169

		public virtual unsafe int ChangingConfigurations {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='getChangingConfigurations' and count(parameter)=0]"
			[Register ("getChangingConfigurations", "()I", "GetGetChangingConfigurationsHandler")]
			get {
				const string __id = "getChangingConfigurations.()I";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualInt32Method (__id, this, null);
					return __rm;
				} finally {
				}
			}
		}

		static Delegate cb_hasWindowFocus;
#pragma warning disable 0169
		static Delegate GetHasWindowFocusHandler ()
		{
			if (cb_hasWindowFocus == null)
				cb_hasWindowFocus = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, bool>) n_HasWindowFocus);
			return cb_hasWindowFocus;
		}

		static bool n_HasWindowFocus (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.HasWindowFocus;
		}
#pragma warning restore 0169

		public virtual unsafe bool HasWindowFocus {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='hasWindowFocus' and count(parameter)=0]"
			[Register ("hasWindowFocus", "()Z", "GetHasWindowFocusHandler")]
			get {
				const string __id = "hasWindowFocus.()Z";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, null);
					return __rm;
				} finally {
				}
			}
		}

		static Delegate cb_isImmersive;
#pragma warning disable 0169
		static Delegate GetIsImmersiveHandler ()
		{
			if (cb_isImmersive == null)
				cb_isImmersive = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, bool>) n_IsImmersive);
			return cb_isImmersive;
		}

		static bool n_IsImmersive (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.Immersive;
		}
#pragma warning restore 0169

		static Delegate cb_setImmersive_Z;
#pragma warning disable 0169
		static Delegate GetSetImmersive_ZHandler ()
		{
			if (cb_setImmersive_Z == null)
				cb_setImmersive_Z = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, bool>) n_SetImmersive_Z);
			return cb_setImmersive_Z;
		}

		static void n_SetImmersive_Z (IntPtr jnienv, IntPtr native__this, bool i)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.Immersive = i;
		}
#pragma warning restore 0169

		public virtual unsafe bool Immersive {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='isImmersive' and count(parameter)=0]"
			[Register ("isImmersive", "()Z", "GetIsImmersiveHandler")]
			get {
				const string __id = "isImmersive.()Z";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, null);
					return __rm;
				} finally {
				}
			}
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setImmersive' and count(parameter)=1 and parameter[1][@type='boolean']]"
			[Register ("setImmersive", "(Z)V", "GetSetImmersive_ZHandler")]
			set {
				const string __id = "setImmersive.(Z)V";
				try {
					JniArgumentValue* __args = stackalloc JniArgumentValue [1];
					__args [0] = new JniArgumentValue (value);
					_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
				} finally {
				}
			}
		}

		public static unsafe long InstanceCount {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='getInstanceCount' and count(parameter)=0]"
			[Register ("getInstanceCount", "()J", "GetGetInstanceCountHandler")]
			get {
				const string __id = "getInstanceCount.()J";
				try {
					var __rm = _members.StaticMethods.InvokeInt64Method (__id, null);
					return __rm;
				} finally {
				}
			}
		}

		static Delegate cb_isActivityTransitionRunning;
#pragma warning disable 0169
		static Delegate GetIsActivityTransitionRunningHandler ()
		{
			if (cb_isActivityTransitionRunning == null)
				cb_isActivityTransitionRunning = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, bool>) n_IsActivityTransitionRunning);
			return cb_isActivityTransitionRunning;
		}

		static bool n_IsActivityTransitionRunning (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.IsActivityTransitionRunning;
		}
#pragma warning restore 0169

		public virtual unsafe bool IsActivityTransitionRunning {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='isActivityTransitionRunning' and count(parameter)=0]"
			[Register ("isActivityTransitionRunning", "()Z", "GetIsActivityTransitionRunningHandler")]
			get {
				const string __id = "isActivityTransitionRunning.()Z";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, null);
					return __rm;
				} finally {
				}
			}
		}

		static Delegate cb_isChangingConfigurations;
#pragma warning disable 0169
		static Delegate GetIsChangingConfigurationsHandler ()
		{
			if (cb_isChangingConfigurations == null)
				cb_isChangingConfigurations = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, bool>) n_IsChangingConfigurations);
			return cb_isChangingConfigurations;
		}

		static bool n_IsChangingConfigurations (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.IsChangingConfigurations;
		}
#pragma warning restore 0169

		public virtual unsafe bool IsChangingConfigurations {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='isChangingConfigurations' and count(parameter)=0]"
			[Register ("isChangingConfigurations", "()Z", "GetIsChangingConfigurationsHandler")]
			get {
				const string __id = "isChangingConfigurations.()Z";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, null);
					return __rm;
				} finally {
				}
			}
		}

		public unsafe bool IsChild {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='isChild' and count(parameter)=0]"
			[Register ("isChild", "()Z", "GetIsChildHandler")]
			get {
				const string __id = "isChild.()Z";
				try {
					var __rm = _members.InstanceMethods.InvokeNonvirtualBooleanMethod (__id, this, null);
					return __rm;
				} finally {
				}
			}
		}

		static Delegate cb_isDestroyed;
#pragma warning disable 0169
		static Delegate GetIsDestroyedHandler ()
		{
			if (cb_isDestroyed == null)
				cb_isDestroyed = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, bool>) n_IsDestroyed);
			return cb_isDestroyed;
		}

		static bool n_IsDestroyed (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.IsDestroyed;
		}
#pragma warning restore 0169

		public virtual unsafe bool IsDestroyed {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='isDestroyed' and count(parameter)=0]"
			[Register ("isDestroyed", "()Z", "GetIsDestroyedHandler")]
			get {
				const string __id = "isDestroyed.()Z";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, null);
					return __rm;
				} finally {
				}
			}
		}

		static Delegate cb_isFinishing;
#pragma warning disable 0169
		static Delegate GetIsFinishingHandler ()
		{
			if (cb_isFinishing == null)
				cb_isFinishing = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, bool>) n_IsFinishing);
			return cb_isFinishing;
		}

		static bool n_IsFinishing (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.IsFinishing;
		}
#pragma warning restore 0169

		public virtual unsafe bool IsFinishing {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='isFinishing' and count(parameter)=0]"
			[Register ("isFinishing", "()Z", "GetIsFinishingHandler")]
			get {
				const string __id = "isFinishing.()Z";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, null);
					return __rm;
				} finally {
				}
			}
		}

		static Delegate cb_isInMultiWindowMode;
#pragma warning disable 0169
		static Delegate GetIsInMultiWindowModeHandler ()
		{
			if (cb_isInMultiWindowMode == null)
				cb_isInMultiWindowMode = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, bool>) n_IsInMultiWindowMode);
			return cb_isInMultiWindowMode;
		}

		static bool n_IsInMultiWindowMode (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.IsInMultiWindowMode;
		}
#pragma warning restore 0169

		public virtual unsafe bool IsInMultiWindowMode {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='isInMultiWindowMode' and count(parameter)=0]"
			[Register ("isInMultiWindowMode", "()Z", "GetIsInMultiWindowModeHandler")]
			get {
				const string __id = "isInMultiWindowMode.()Z";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, null);
					return __rm;
				} finally {
				}
			}
		}

		static Delegate cb_isInPictureInPictureMode;
#pragma warning disable 0169
		static Delegate GetIsInPictureInPictureModeHandler ()
		{
			if (cb_isInPictureInPictureMode == null)
				cb_isInPictureInPictureMode = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, bool>) n_IsInPictureInPictureMode);
			return cb_isInPictureInPictureMode;
		}

		static bool n_IsInPictureInPictureMode (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.IsInPictureInPictureMode;
		}
#pragma warning restore 0169

		public virtual unsafe bool IsInPictureInPictureMode {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='isInPictureInPictureMode' and count(parameter)=0]"
			[Register ("isInPictureInPictureMode", "()Z", "GetIsInPictureInPictureModeHandler")]
			get {
				const string __id = "isInPictureInPictureMode.()Z";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, null);
					return __rm;
				} finally {
				}
			}
		}

		static Delegate cb_isLocalVoiceInteractionSupported;
#pragma warning disable 0169
		static Delegate GetIsLocalVoiceInteractionSupportedHandler ()
		{
			if (cb_isLocalVoiceInteractionSupported == null)
				cb_isLocalVoiceInteractionSupported = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, bool>) n_IsLocalVoiceInteractionSupported);
			return cb_isLocalVoiceInteractionSupported;
		}

		static bool n_IsLocalVoiceInteractionSupported (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.IsLocalVoiceInteractionSupported;
		}
#pragma warning restore 0169

		public virtual unsafe bool IsLocalVoiceInteractionSupported {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='isLocalVoiceInteractionSupported' and count(parameter)=0]"
			[Register ("isLocalVoiceInteractionSupported", "()Z", "GetIsLocalVoiceInteractionSupportedHandler")]
			get {
				const string __id = "isLocalVoiceInteractionSupported.()Z";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, null);
					return __rm;
				} finally {
				}
			}
		}

		static Delegate cb_isTaskRoot;
#pragma warning disable 0169
		static Delegate GetIsTaskRootHandler ()
		{
			if (cb_isTaskRoot == null)
				cb_isTaskRoot = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, bool>) n_IsTaskRoot);
			return cb_isTaskRoot;
		}

		static bool n_IsTaskRoot (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.IsTaskRoot;
		}
#pragma warning restore 0169

		public virtual unsafe bool IsTaskRoot {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='isTaskRoot' and count(parameter)=0]"
			[Register ("isTaskRoot", "()Z", "GetIsTaskRootHandler")]
			get {
				const string __id = "isTaskRoot.()Z";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, null);
					return __rm;
				} finally {
				}
			}
		}

		static Delegate cb_isVoiceInteraction;
#pragma warning disable 0169
		static Delegate GetIsVoiceInteractionHandler ()
		{
			if (cb_isVoiceInteraction == null)
				cb_isVoiceInteraction = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, bool>) n_IsVoiceInteraction);
			return cb_isVoiceInteraction;
		}

		static bool n_IsVoiceInteraction (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.IsVoiceInteraction;
		}
#pragma warning restore 0169

		public virtual unsafe bool IsVoiceInteraction {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='isVoiceInteraction' and count(parameter)=0]"
			[Register ("isVoiceInteraction", "()Z", "GetIsVoiceInteractionHandler")]
			get {
				const string __id = "isVoiceInteraction.()Z";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, null);
					return __rm;
				} finally {
				}
			}
		}

		static Delegate cb_isVoiceInteractionRoot;
#pragma warning disable 0169
		static Delegate GetIsVoiceInteractionRootHandler ()
		{
			if (cb_isVoiceInteractionRoot == null)
				cb_isVoiceInteractionRoot = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, bool>) n_IsVoiceInteractionRoot);
			return cb_isVoiceInteractionRoot;
		}

		static bool n_IsVoiceInteractionRoot (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.IsVoiceInteractionRoot;
		}
#pragma warning restore 0169

		public virtual unsafe bool IsVoiceInteractionRoot {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='isVoiceInteractionRoot' and count(parameter)=0]"
			[Register ("isVoiceInteractionRoot", "()Z", "GetIsVoiceInteractionRootHandler")]
			get {
				const string __id = "isVoiceInteractionRoot.()Z";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, null);
					return __rm;
				} finally {
				}
			}
		}

		static Delegate cb_getLastNonConfigurationInstance;
#pragma warning disable 0169
		static Delegate GetGetLastNonConfigurationInstanceHandler ()
		{
			if (cb_getLastNonConfigurationInstance == null)
				cb_getLastNonConfigurationInstance = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, IntPtr>) n_GetLastNonConfigurationInstance);
			return cb_getLastNonConfigurationInstance;
		}

		static IntPtr n_GetLastNonConfigurationInstance (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return JNIEnv.ToLocalJniHandle (__this.LastNonConfigurationInstance);
		}
#pragma warning restore 0169

		public virtual unsafe global::Java.Lang.Object LastNonConfigurationInstance {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='getLastNonConfigurationInstance' and count(parameter)=0]"
			[Register ("getLastNonConfigurationInstance", "()Ljava/lang/Object;", "GetGetLastNonConfigurationInstanceHandler")]
			get {
				const string __id = "getLastNonConfigurationInstance.()Ljava/lang/Object;";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, null);
					return global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
				} finally {
				}
			}
		}

		static Delegate cb_getLocalClassName;
#pragma warning disable 0169
		static Delegate GetGetLocalClassNameHandler ()
		{
			if (cb_getLocalClassName == null)
				cb_getLocalClassName = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, IntPtr>) n_GetLocalClassName);
			return cb_getLocalClassName;
		}

		static IntPtr n_GetLocalClassName (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return JNIEnv.NewString (__this.LocalClassName);
		}
#pragma warning restore 0169

		public virtual unsafe string LocalClassName {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='getLocalClassName' and count(parameter)=0]"
			[Register ("getLocalClassName", "()Ljava/lang/String;", "GetGetLocalClassNameHandler")]
			get {
				const string __id = "getLocalClassName.()Ljava/lang/String;";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, null);
					return JNIEnv.GetString (__rm.Handle, JniHandleOwnership.TransferLocalRef);
				} finally {
				}
			}
		}

		static Delegate cb_getMaxNumPictureInPictureActions;
#pragma warning disable 0169
		static Delegate GetGetMaxNumPictureInPictureActionsHandler ()
		{
			if (cb_getMaxNumPictureInPictureActions == null)
				cb_getMaxNumPictureInPictureActions = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, int>) n_GetMaxNumPictureInPictureActions);
			return cb_getMaxNumPictureInPictureActions;
		}

		static int n_GetMaxNumPictureInPictureActions (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.MaxNumPictureInPictureActions;
		}
#pragma warning restore 0169

		public virtual unsafe int MaxNumPictureInPictureActions {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='getMaxNumPictureInPictureActions' and count(parameter)=0]"
			[Register ("getMaxNumPictureInPictureActions", "()I", "GetGetMaxNumPictureInPictureActionsHandler")]
			get {
				const string __id = "getMaxNumPictureInPictureActions.()I";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualInt32Method (__id, this, null);
					return __rm;
				} finally {
				}
			}
		}

		public unsafe global::Android.App.Activity Parent {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='getParent' and count(parameter)=0]"
			[Register ("getParent", "()Landroid/app/Activity;", "GetGetParentHandler")]
			get {
				const string __id = "getParent.()Landroid/app/Activity;";
				try {
					var __rm = _members.InstanceMethods.InvokeNonvirtualObjectMethod (__id, this, null);
					return global::Java.Lang.Object.GetObject<global::Android.App.Activity> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
				} finally {
				}
			}
		}

		static Delegate cb_getRequestedOrientation;
#pragma warning disable 0169
		static Delegate GetGetRequestedOrientationHandler ()
		{
			if (cb_getRequestedOrientation == null)
				cb_getRequestedOrientation = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, int>) n_GetRequestedOrientation);
			return cb_getRequestedOrientation;
		}

		static int n_GetRequestedOrientation (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.RequestedOrientation;
		}
#pragma warning restore 0169

		static Delegate cb_setRequestedOrientation_I;
#pragma warning disable 0169
		static Delegate GetSetRequestedOrientation_IHandler ()
		{
			if (cb_setRequestedOrientation_I == null)
				cb_setRequestedOrientation_I = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, int>) n_SetRequestedOrientation_I);
			return cb_setRequestedOrientation_I;
		}

		static void n_SetRequestedOrientation_I (IntPtr jnienv, IntPtr native__this, int requestedOrientation)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.RequestedOrientation = requestedOrientation;
		}
#pragma warning restore 0169

		public virtual unsafe int RequestedOrientation {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='getRequestedOrientation' and count(parameter)=0]"
			[Register ("getRequestedOrientation", "()I", "GetGetRequestedOrientationHandler")]
			get {
				const string __id = "getRequestedOrientation.()I";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualInt32Method (__id, this, null);
					return __rm;
				} finally {
				}
			}
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setRequestedOrientation' and count(parameter)=1 and parameter[1][@type='int']]"
			[Register ("setRequestedOrientation", "(I)V", "GetSetRequestedOrientation_IHandler")]
			set {
				const string __id = "setRequestedOrientation.(I)V";
				try {
					JniArgumentValue* __args = stackalloc JniArgumentValue [1];
					__args [0] = new JniArgumentValue (value);
					_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
				} finally {
				}
			}
		}

		static Delegate cb_getTaskId;
#pragma warning disable 0169
		static Delegate GetGetTaskIdHandler ()
		{
			if (cb_getTaskId == null)
				cb_getTaskId = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, int>) n_GetTaskId);
			return cb_getTaskId;
		}

		static int n_GetTaskId (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.TaskId;
		}
#pragma warning restore 0169

		public virtual unsafe int TaskId {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='getTaskId' and count(parameter)=0]"
			[Register ("getTaskId", "()I", "GetGetTaskIdHandler")]
			get {
				const string __id = "getTaskId.()I";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualInt32Method (__id, this, null);
					return __rm;
				} finally {
				}
			}
		}

		public unsafe global::Java.Lang.ICharSequence TitleFormatted {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='getTitle' and count(parameter)=0]"
			[Register ("getTitle", "()Ljava/lang/CharSequence;", "GetGetTitleHandler")]
			get {
				const string __id = "getTitle.()Ljava/lang/CharSequence;";
				try {
					var __rm = _members.InstanceMethods.InvokeNonvirtualObjectMethod (__id, this, null);
					return global::Java.Lang.Object.GetObject<Java.Lang.ICharSequence> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
				} finally {
				}
			}
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setTitle' and count(parameter)=1 and parameter[1][@type='java.lang.CharSequence']]"
			[Register ("setTitle", "(Ljava/lang/CharSequence;)V", "GetSetTitle_Ljava_lang_CharSequence_Handler")]
			set {
				const string __id = "setTitle.(Ljava/lang/CharSequence;)V";
				IntPtr native_value = CharSequence.ToLocalJniHandle (value);
				try {
					JniArgumentValue* __args = stackalloc JniArgumentValue [1];
					__args [0] = new JniArgumentValue (native_value);
					_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
				} finally {
					JNIEnv.DeleteLocalRef (native_value);
				}
			}
		}

		public string Title {
			get { return TitleFormatted == null ? null : TitleFormatted.ToString (); }
			set {
				global::Java.Lang.String jls = value == null ? null : new global::Java.Lang.String (value);
				TitleFormatted = jls;
				if (jls != null) jls.Dispose ();
			}
		}

		public unsafe int TitleColor {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='getTitleColor' and count(parameter)=0]"
			[Register ("getTitleColor", "()I", "GetGetTitleColorHandler")]
			get {
				const string __id = "getTitleColor.()I";
				try {
					var __rm = _members.InstanceMethods.InvokeNonvirtualInt32Method (__id, this, null);
					return __rm;
				} finally {
				}
			}
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setTitleColor' and count(parameter)=1 and parameter[1][@type='int']]"
			[Register ("setTitleColor", "(I)V", "GetSetTitleColor_IHandler")]
			set {
				const string __id = "setTitleColor.(I)V";
				try {
					JniArgumentValue* __args = stackalloc JniArgumentValue [1];
					__args [0] = new JniArgumentValue (value);
					_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
				} finally {
				}
			}
		}

		public unsafe int VolumeControlStream {
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='getVolumeControlStream' and count(parameter)=0]"
			[Register ("getVolumeControlStream", "()I", "GetGetVolumeControlStreamHandler")]
			get {
				const string __id = "getVolumeControlStream.()I";
				try {
					var __rm = _members.InstanceMethods.InvokeNonvirtualInt32Method (__id, this, null);
					return __rm;
				} finally {
				}
			}
			// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setVolumeControlStream' and count(parameter)=1 and parameter[1][@type='int']]"
			[Register ("setVolumeControlStream", "(I)V", "GetSetVolumeControlStream_IHandler")]
			set {
				const string __id = "setVolumeControlStream.(I)V";
				try {
					JniArgumentValue* __args = stackalloc JniArgumentValue [1];
					__args [0] = new JniArgumentValue (value);
					_members.InstanceMethods.InvokeNonvirtualVoidMethod (__id, this, __args);
				} finally {
				}
			}
		}

		static Delegate cb_closeContextMenu;
#pragma warning disable 0169
		static Delegate GetCloseContextMenuHandler ()
		{
			if (cb_closeContextMenu == null)
				cb_closeContextMenu = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_CloseContextMenu);
			return cb_closeContextMenu;
		}

		static void n_CloseContextMenu (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.CloseContextMenu ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='closeContextMenu' and count(parameter)=0]"
		[Register ("closeContextMenu", "()V", "GetCloseContextMenuHandler")]
		public virtual unsafe void CloseContextMenu ()
		{
			const string __id = "closeContextMenu.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_closeOptionsMenu;
#pragma warning disable 0169
		static Delegate GetCloseOptionsMenuHandler ()
		{
			if (cb_closeOptionsMenu == null)
				cb_closeOptionsMenu = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_CloseOptionsMenu);
			return cb_closeOptionsMenu;
		}

		static void n_CloseOptionsMenu (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.CloseOptionsMenu ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='closeOptionsMenu' and count(parameter)=0]"
		[Register ("closeOptionsMenu", "()V", "GetCloseOptionsMenuHandler")]
		public virtual unsafe void CloseOptionsMenu ()
		{
			const string __id = "closeOptionsMenu.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='dismissDialog' and count(parameter)=1 and parameter[1][@type='int']]"
		[Obsolete (@"deprecated")]
		[Register ("dismissDialog", "(I)V", "")]
		public unsafe void DismissDialog (int id)
		{
			const string __id = "dismissDialog.(I)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (id);
				_members.InstanceMethods.InvokeNonvirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='dismissKeyboardShortcutsHelper' and count(parameter)=0]"
		[Register ("dismissKeyboardShortcutsHelper", "()V", "")]
		public unsafe void DismissKeyboardShortcutsHelper ()
		{
			const string __id = "dismissKeyboardShortcutsHelper.()V";
			try {
				_members.InstanceMethods.InvokeNonvirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_enterPictureInPictureMode;
#pragma warning disable 0169
		static Delegate GetEnterPictureInPictureModeHandler ()
		{
			if (cb_enterPictureInPictureMode == null)
				cb_enterPictureInPictureMode = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_EnterPictureInPictureMode);
			return cb_enterPictureInPictureMode;
		}

		static void n_EnterPictureInPictureMode (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.EnterPictureInPictureMode ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='enterPictureInPictureMode' and count(parameter)=0]"
		[Obsolete (@"deprecated")]
		[Register ("enterPictureInPictureMode", "()V", "GetEnterPictureInPictureModeHandler")]
		public virtual unsafe void EnterPictureInPictureMode ()
		{
			const string __id = "enterPictureInPictureMode.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_findViewById_I;
#pragma warning disable 0169
		static Delegate GetFindViewById_IHandler ()
		{
			if (cb_findViewById_I == null)
				cb_findViewById_I = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, int, IntPtr>) n_FindViewById_I);
			return cb_findViewById_I;
		}

		static IntPtr n_FindViewById_I (IntPtr jnienv, IntPtr native__this, int id)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return JNIEnv.ToLocalJniHandle (__this.FindViewById (id));
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='findViewById' and count(parameter)=1 and parameter[1][@type='int']]"
		[Register ("findViewById", "(I)Ljava/lang/Object;", "GetFindViewById_IHandler")]
		[global::Java.Interop.JavaTypeParameters (new string [] {"T extends android.view.View"})]
		public virtual unsafe global::Java.Lang.Object FindViewById (int id)
		{
			const string __id = "findViewById.(I)Ljava/lang/Object;";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (id);
				var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, __args);
				return (Java.Lang.Object) global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
			}
		}

		static Delegate cb_finish;
#pragma warning disable 0169
		static Delegate GetFinishHandler ()
		{
			if (cb_finish == null)
				cb_finish = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_Finish);
			return cb_finish;
		}

		static void n_Finish (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.Finish ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='finish' and count(parameter)=0]"
		[Register ("finish", "()V", "GetFinishHandler")]
		public virtual unsafe void Finish ()
		{
			const string __id = "finish.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_finishActivity_I;
#pragma warning disable 0169
		static Delegate GetFinishActivity_IHandler ()
		{
			if (cb_finishActivity_I == null)
				cb_finishActivity_I = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, int>) n_FinishActivity_I);
			return cb_finishActivity_I;
		}

		static void n_FinishActivity_I (IntPtr jnienv, IntPtr native__this, int requestCode)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.FinishActivity (requestCode);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='finishActivity' and count(parameter)=1 and parameter[1][@type='int']]"
		[Register ("finishActivity", "(I)V", "GetFinishActivity_IHandler")]
		public virtual unsafe void FinishActivity (int requestCode)
		{
			const string __id = "finishActivity.(I)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (requestCode);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_finishActivityFromChild_Landroid_app_Activity_I;
#pragma warning disable 0169
		static Delegate GetFinishActivityFromChild_Landroid_app_Activity_IHandler ()
		{
			if (cb_finishActivityFromChild_Landroid_app_Activity_I == null)
				cb_finishActivityFromChild_Landroid_app_Activity_I = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, IntPtr, int>) n_FinishActivityFromChild_Landroid_app_Activity_I);
			return cb_finishActivityFromChild_Landroid_app_Activity_I;
		}

		static void n_FinishActivityFromChild_Landroid_app_Activity_I (IntPtr jnienv, IntPtr native__this, IntPtr native_child, int requestCode)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			global::Android.App.Activity child = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (native_child, JniHandleOwnership.DoNotTransfer);
			__this.FinishActivityFromChild (child, requestCode);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='finishActivityFromChild' and count(parameter)=2 and parameter[1][@type='android.app.Activity'] and parameter[2][@type='int']]"
		[Register ("finishActivityFromChild", "(Landroid/app/Activity;I)V", "GetFinishActivityFromChild_Landroid_app_Activity_IHandler")]
		public virtual unsafe void FinishActivityFromChild (global::Android.App.Activity child, int requestCode)
		{
			const string __id = "finishActivityFromChild.(Landroid/app/Activity;I)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [2];
				__args [0] = new JniArgumentValue ((child == null) ? IntPtr.Zero : ((global::Java.Lang.Object) child).Handle);
				__args [1] = new JniArgumentValue (requestCode);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_finishAffinity;
#pragma warning disable 0169
		static Delegate GetFinishAffinityHandler ()
		{
			if (cb_finishAffinity == null)
				cb_finishAffinity = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_FinishAffinity);
			return cb_finishAffinity;
		}

		static void n_FinishAffinity (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.FinishAffinity ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='finishAffinity' and count(parameter)=0]"
		[Register ("finishAffinity", "()V", "GetFinishAffinityHandler")]
		public virtual unsafe void FinishAffinity ()
		{
			const string __id = "finishAffinity.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_finishAfterTransition;
#pragma warning disable 0169
		static Delegate GetFinishAfterTransitionHandler ()
		{
			if (cb_finishAfterTransition == null)
				cb_finishAfterTransition = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_FinishAfterTransition);
			return cb_finishAfterTransition;
		}

		static void n_FinishAfterTransition (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.FinishAfterTransition ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='finishAfterTransition' and count(parameter)=0]"
		[Register ("finishAfterTransition", "()V", "GetFinishAfterTransitionHandler")]
		public virtual unsafe void FinishAfterTransition ()
		{
			const string __id = "finishAfterTransition.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_finishAndRemoveTask;
#pragma warning disable 0169
		static Delegate GetFinishAndRemoveTaskHandler ()
		{
			if (cb_finishAndRemoveTask == null)
				cb_finishAndRemoveTask = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_FinishAndRemoveTask);
			return cb_finishAndRemoveTask;
		}

		static void n_FinishAndRemoveTask (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.FinishAndRemoveTask ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='finishAndRemoveTask' and count(parameter)=0]"
		[Register ("finishAndRemoveTask", "()V", "GetFinishAndRemoveTaskHandler")]
		public virtual unsafe void FinishAndRemoveTask ()
		{
			const string __id = "finishAndRemoveTask.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_finishFromChild_Landroid_app_Activity_;
#pragma warning disable 0169
		static Delegate GetFinishFromChild_Landroid_app_Activity_Handler ()
		{
			if (cb_finishFromChild_Landroid_app_Activity_ == null)
				cb_finishFromChild_Landroid_app_Activity_ = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, IntPtr>) n_FinishFromChild_Landroid_app_Activity_);
			return cb_finishFromChild_Landroid_app_Activity_;
		}

		static void n_FinishFromChild_Landroid_app_Activity_ (IntPtr jnienv, IntPtr native__this, IntPtr native_child)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			global::Android.App.Activity child = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (native_child, JniHandleOwnership.DoNotTransfer);
			__this.FinishFromChild (child);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='finishFromChild' and count(parameter)=1 and parameter[1][@type='android.app.Activity']]"
		[Register ("finishFromChild", "(Landroid/app/Activity;)V", "GetFinishFromChild_Landroid_app_Activity_Handler")]
		public virtual unsafe void FinishFromChild (global::Android.App.Activity child)
		{
			const string __id = "finishFromChild.(Landroid/app/Activity;)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue ((child == null) ? IntPtr.Zero : ((global::Java.Lang.Object) child).Handle);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_invalidateOptionsMenu;
#pragma warning disable 0169
		static Delegate GetInvalidateOptionsMenuHandler ()
		{
			if (cb_invalidateOptionsMenu == null)
				cb_invalidateOptionsMenu = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_InvalidateOptionsMenu);
			return cb_invalidateOptionsMenu;
		}

		static void n_InvalidateOptionsMenu (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.InvalidateOptionsMenu ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='invalidateOptionsMenu' and count(parameter)=0]"
		[Register ("invalidateOptionsMenu", "()V", "GetInvalidateOptionsMenuHandler")]
		public virtual unsafe void InvalidateOptionsMenu ()
		{
			const string __id = "invalidateOptionsMenu.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_moveTaskToBack_Z;
#pragma warning disable 0169
		static Delegate GetMoveTaskToBack_ZHandler ()
		{
			if (cb_moveTaskToBack_Z == null)
				cb_moveTaskToBack_Z = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, bool, bool>) n_MoveTaskToBack_Z);
			return cb_moveTaskToBack_Z;
		}

		static bool n_MoveTaskToBack_Z (IntPtr jnienv, IntPtr native__this, bool nonRoot)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.MoveTaskToBack (nonRoot);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='moveTaskToBack' and count(parameter)=1 and parameter[1][@type='boolean']]"
		[Register ("moveTaskToBack", "(Z)Z", "GetMoveTaskToBack_ZHandler")]
		public virtual unsafe bool MoveTaskToBack (bool nonRoot)
		{
			const string __id = "moveTaskToBack.(Z)Z";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (nonRoot);
				var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, __args);
				return __rm;
			} finally {
			}
		}

		static Delegate cb_onAttachedToWindow;
#pragma warning disable 0169
		static Delegate GetOnAttachedToWindowHandler ()
		{
			if (cb_onAttachedToWindow == null)
				cb_onAttachedToWindow = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_OnAttachedToWindow);
			return cb_onAttachedToWindow;
		}

		static void n_OnAttachedToWindow (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnAttachedToWindow ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onAttachedToWindow' and count(parameter)=0]"
		[Register ("onAttachedToWindow", "()V", "GetOnAttachedToWindowHandler")]
		public virtual unsafe void OnAttachedToWindow ()
		{
			const string __id = "onAttachedToWindow.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_onBackPressed;
#pragma warning disable 0169
		static Delegate GetOnBackPressedHandler ()
		{
			if (cb_onBackPressed == null)
				cb_onBackPressed = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_OnBackPressed);
			return cb_onBackPressed;
		}

		static void n_OnBackPressed (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnBackPressed ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onBackPressed' and count(parameter)=0]"
		[Register ("onBackPressed", "()V", "GetOnBackPressedHandler")]
		public virtual unsafe void OnBackPressed ()
		{
			const string __id = "onBackPressed.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_onChildTitleChanged_Landroid_app_Activity_Ljava_lang_CharSequence_;
#pragma warning disable 0169
		static Delegate GetOnChildTitleChanged_Landroid_app_Activity_Ljava_lang_CharSequence_Handler ()
		{
			if (cb_onChildTitleChanged_Landroid_app_Activity_Ljava_lang_CharSequence_ == null)
				cb_onChildTitleChanged_Landroid_app_Activity_Ljava_lang_CharSequence_ = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, IntPtr, IntPtr>) n_OnChildTitleChanged_Landroid_app_Activity_Ljava_lang_CharSequence_);
			return cb_onChildTitleChanged_Landroid_app_Activity_Ljava_lang_CharSequence_;
		}

		static void n_OnChildTitleChanged_Landroid_app_Activity_Ljava_lang_CharSequence_ (IntPtr jnienv, IntPtr native__this, IntPtr native_childActivity, IntPtr native_title)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			global::Android.App.Activity childActivity = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (native_childActivity, JniHandleOwnership.DoNotTransfer);
			global::Java.Lang.ICharSequence title = global::Java.Lang.Object.GetObject<global::Java.Lang.ICharSequence> (native_title, JniHandleOwnership.DoNotTransfer);
			__this.OnChildTitleChanged (childActivity, title);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onChildTitleChanged' and count(parameter)=2 and parameter[1][@type='android.app.Activity'] and parameter[2][@type='java.lang.CharSequence']]"
		[Register ("onChildTitleChanged", "(Landroid/app/Activity;Ljava/lang/CharSequence;)V", "GetOnChildTitleChanged_Landroid_app_Activity_Ljava_lang_CharSequence_Handler")]
		protected virtual unsafe void OnChildTitleChanged (global::Android.App.Activity childActivity, global::Java.Lang.ICharSequence title)
		{
			const string __id = "onChildTitleChanged.(Landroid/app/Activity;Ljava/lang/CharSequence;)V";
			IntPtr native_title = CharSequence.ToLocalJniHandle (title);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [2];
				__args [0] = new JniArgumentValue ((childActivity == null) ? IntPtr.Zero : ((global::Java.Lang.Object) childActivity).Handle);
				__args [1] = new JniArgumentValue (native_title);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
				JNIEnv.DeleteLocalRef (native_title);
			}
		}

		protected void OnChildTitleChanged (global::Android.App.Activity childActivity, string title)
		{
			global::Java.Lang.String jls_title = title == null ? null : new global::Java.Lang.String (title);
			OnChildTitleChanged (childActivity, jls_title);
			jls_title?.Dispose ();
		}

		static Delegate cb_onContentChanged;
#pragma warning disable 0169
		static Delegate GetOnContentChangedHandler ()
		{
			if (cb_onContentChanged == null)
				cb_onContentChanged = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_OnContentChanged);
			return cb_onContentChanged;
		}

		static void n_OnContentChanged (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnContentChanged ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onContentChanged' and count(parameter)=0]"
		[Register ("onContentChanged", "()V", "GetOnContentChangedHandler")]
		public virtual unsafe void OnContentChanged ()
		{
			const string __id = "onContentChanged.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_onCreateDescription;
#pragma warning disable 0169
		static Delegate GetOnCreateDescriptionHandler ()
		{
			if (cb_onCreateDescription == null)
				cb_onCreateDescription = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, IntPtr>) n_OnCreateDescription);
			return cb_onCreateDescription;
		}

		static IntPtr n_OnCreateDescription (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return CharSequence.ToLocalJniHandle (__this.OnCreateDescriptionFormatted ());
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onCreateDescription' and count(parameter)=0]"
		[Register ("onCreateDescription", "()Ljava/lang/CharSequence;", "GetOnCreateDescriptionHandler")]
		public virtual unsafe global::Java.Lang.ICharSequence OnCreateDescriptionFormatted ()
		{
			const string __id = "onCreateDescription.()Ljava/lang/CharSequence;";
			try {
				var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, null);
				return global::Java.Lang.Object.GetObject<Java.Lang.ICharSequence> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
			}
		}

		public string OnCreateDescription ()
		{
			global::Java.Lang.ICharSequence __result = OnCreateDescriptionFormatted ();
			var __rsval = __result?.ToString ();
			return __rsval;
		}

		static Delegate cb_onDestroy;
#pragma warning disable 0169
		static Delegate GetOnDestroyHandler ()
		{
			if (cb_onDestroy == null)
				cb_onDestroy = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_OnDestroy);
			return cb_onDestroy;
		}

		static void n_OnDestroy (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnDestroy ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onDestroy' and count(parameter)=0]"
		[Register ("onDestroy", "()V", "GetOnDestroyHandler")]
		protected virtual unsafe void OnDestroy ()
		{
			const string __id = "onDestroy.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_onDetachedFromWindow;
#pragma warning disable 0169
		static Delegate GetOnDetachedFromWindowHandler ()
		{
			if (cb_onDetachedFromWindow == null)
				cb_onDetachedFromWindow = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_OnDetachedFromWindow);
			return cb_onDetachedFromWindow;
		}

		static void n_OnDetachedFromWindow (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnDetachedFromWindow ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onDetachedFromWindow' and count(parameter)=0]"
		[Register ("onDetachedFromWindow", "()V", "GetOnDetachedFromWindowHandler")]
		public virtual unsafe void OnDetachedFromWindow ()
		{
			const string __id = "onDetachedFromWindow.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_onEnterAnimationComplete;
#pragma warning disable 0169
		static Delegate GetOnEnterAnimationCompleteHandler ()
		{
			if (cb_onEnterAnimationComplete == null)
				cb_onEnterAnimationComplete = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_OnEnterAnimationComplete);
			return cb_onEnterAnimationComplete;
		}

		static void n_OnEnterAnimationComplete (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnEnterAnimationComplete ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onEnterAnimationComplete' and count(parameter)=0]"
		[Register ("onEnterAnimationComplete", "()V", "GetOnEnterAnimationCompleteHandler")]
		public virtual unsafe void OnEnterAnimationComplete ()
		{
			const string __id = "onEnterAnimationComplete.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_onLocalVoiceInteractionStarted;
#pragma warning disable 0169
		static Delegate GetOnLocalVoiceInteractionStartedHandler ()
		{
			if (cb_onLocalVoiceInteractionStarted == null)
				cb_onLocalVoiceInteractionStarted = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_OnLocalVoiceInteractionStarted);
			return cb_onLocalVoiceInteractionStarted;
		}

		static void n_OnLocalVoiceInteractionStarted (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnLocalVoiceInteractionStarted ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onLocalVoiceInteractionStarted' and count(parameter)=0]"
		[Register ("onLocalVoiceInteractionStarted", "()V", "GetOnLocalVoiceInteractionStartedHandler")]
		public virtual unsafe void OnLocalVoiceInteractionStarted ()
		{
			const string __id = "onLocalVoiceInteractionStarted.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_onLocalVoiceInteractionStopped;
#pragma warning disable 0169
		static Delegate GetOnLocalVoiceInteractionStoppedHandler ()
		{
			if (cb_onLocalVoiceInteractionStopped == null)
				cb_onLocalVoiceInteractionStopped = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_OnLocalVoiceInteractionStopped);
			return cb_onLocalVoiceInteractionStopped;
		}

		static void n_OnLocalVoiceInteractionStopped (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnLocalVoiceInteractionStopped ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onLocalVoiceInteractionStopped' and count(parameter)=0]"
		[Register ("onLocalVoiceInteractionStopped", "()V", "GetOnLocalVoiceInteractionStoppedHandler")]
		public virtual unsafe void OnLocalVoiceInteractionStopped ()
		{
			const string __id = "onLocalVoiceInteractionStopped.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_onLowMemory;
#pragma warning disable 0169
		static Delegate GetOnLowMemoryHandler ()
		{
			if (cb_onLowMemory == null)
				cb_onLowMemory = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_OnLowMemory);
			return cb_onLowMemory;
		}

		static void n_OnLowMemory (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnLowMemory ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onLowMemory' and count(parameter)=0]"
		[Register ("onLowMemory", "()V", "GetOnLowMemoryHandler")]
		public virtual unsafe void OnLowMemory ()
		{
			const string __id = "onLowMemory.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_onMultiWindowModeChanged_Z;
#pragma warning disable 0169
		static Delegate GetOnMultiWindowModeChanged_ZHandler ()
		{
			if (cb_onMultiWindowModeChanged_Z == null)
				cb_onMultiWindowModeChanged_Z = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, bool>) n_OnMultiWindowModeChanged_Z);
			return cb_onMultiWindowModeChanged_Z;
		}

		static void n_OnMultiWindowModeChanged_Z (IntPtr jnienv, IntPtr native__this, bool isInMultiWindowMode)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnMultiWindowModeChanged (isInMultiWindowMode);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onMultiWindowModeChanged' and count(parameter)=1 and parameter[1][@type='boolean']]"
		[Obsolete (@"deprecated")]
		[Register ("onMultiWindowModeChanged", "(Z)V", "GetOnMultiWindowModeChanged_ZHandler")]
		public virtual unsafe void OnMultiWindowModeChanged (bool isInMultiWindowMode)
		{
			const string __id = "onMultiWindowModeChanged.(Z)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (isInMultiWindowMode);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_onNavigateUp;
#pragma warning disable 0169
		static Delegate GetOnNavigateUpHandler ()
		{
			if (cb_onNavigateUp == null)
				cb_onNavigateUp = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, bool>) n_OnNavigateUp);
			return cb_onNavigateUp;
		}

		static bool n_OnNavigateUp (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.OnNavigateUp ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onNavigateUp' and count(parameter)=0]"
		[Register ("onNavigateUp", "()Z", "GetOnNavigateUpHandler")]
		public virtual unsafe bool OnNavigateUp ()
		{
			const string __id = "onNavigateUp.()Z";
			try {
				var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, null);
				return __rm;
			} finally {
			}
		}

		static Delegate cb_onNavigateUpFromChild_Landroid_app_Activity_;
#pragma warning disable 0169
		static Delegate GetOnNavigateUpFromChild_Landroid_app_Activity_Handler ()
		{
			if (cb_onNavigateUpFromChild_Landroid_app_Activity_ == null)
				cb_onNavigateUpFromChild_Landroid_app_Activity_ = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, IntPtr, bool>) n_OnNavigateUpFromChild_Landroid_app_Activity_);
			return cb_onNavigateUpFromChild_Landroid_app_Activity_;
		}

		static bool n_OnNavigateUpFromChild_Landroid_app_Activity_ (IntPtr jnienv, IntPtr native__this, IntPtr native_child)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			global::Android.App.Activity child = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (native_child, JniHandleOwnership.DoNotTransfer);
			bool __ret = __this.OnNavigateUpFromChild (child);
			return __ret;
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onNavigateUpFromChild' and count(parameter)=1 and parameter[1][@type='android.app.Activity']]"
		[Register ("onNavigateUpFromChild", "(Landroid/app/Activity;)Z", "GetOnNavigateUpFromChild_Landroid_app_Activity_Handler")]
		public virtual unsafe bool OnNavigateUpFromChild (global::Android.App.Activity child)
		{
			const string __id = "onNavigateUpFromChild.(Landroid/app/Activity;)Z";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue ((child == null) ? IntPtr.Zero : ((global::Java.Lang.Object) child).Handle);
				var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, __args);
				return __rm;
			} finally {
			}
		}

		static Delegate cb_onPause;
#pragma warning disable 0169
		static Delegate GetOnPauseHandler ()
		{
			if (cb_onPause == null)
				cb_onPause = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_OnPause);
			return cb_onPause;
		}

		static void n_OnPause (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnPause ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onPause' and count(parameter)=0]"
		[Register ("onPause", "()V", "GetOnPauseHandler")]
		protected virtual unsafe void OnPause ()
		{
			const string __id = "onPause.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_onPictureInPictureModeChanged_Z;
#pragma warning disable 0169
		static Delegate GetOnPictureInPictureModeChanged_ZHandler ()
		{
			if (cb_onPictureInPictureModeChanged_Z == null)
				cb_onPictureInPictureModeChanged_Z = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, bool>) n_OnPictureInPictureModeChanged_Z);
			return cb_onPictureInPictureModeChanged_Z;
		}

		static void n_OnPictureInPictureModeChanged_Z (IntPtr jnienv, IntPtr native__this, bool isInPictureInPictureMode)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnPictureInPictureModeChanged (isInPictureInPictureMode);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onPictureInPictureModeChanged' and count(parameter)=1 and parameter[1][@type='boolean']]"
		[Obsolete (@"deprecated")]
		[Register ("onPictureInPictureModeChanged", "(Z)V", "GetOnPictureInPictureModeChanged_ZHandler")]
		public virtual unsafe void OnPictureInPictureModeChanged (bool isInPictureInPictureMode)
		{
			const string __id = "onPictureInPictureModeChanged.(Z)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (isInPictureInPictureMode);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_onPostResume;
#pragma warning disable 0169
		static Delegate GetOnPostResumeHandler ()
		{
			if (cb_onPostResume == null)
				cb_onPostResume = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_OnPostResume);
			return cb_onPostResume;
		}

		static void n_OnPostResume (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnPostResume ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onPostResume' and count(parameter)=0]"
		[Register ("onPostResume", "()V", "GetOnPostResumeHandler")]
		protected virtual unsafe void OnPostResume ()
		{
			const string __id = "onPostResume.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_onRequestPermissionsResult_IarrayLjava_lang_String_arrayI;
#pragma warning disable 0169
		static Delegate GetOnRequestPermissionsResult_IarrayLjava_lang_String_arrayIHandler ()
		{
			if (cb_onRequestPermissionsResult_IarrayLjava_lang_String_arrayI == null)
				cb_onRequestPermissionsResult_IarrayLjava_lang_String_arrayI = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, int, IntPtr, IntPtr>) n_OnRequestPermissionsResult_IarrayLjava_lang_String_arrayI);
			return cb_onRequestPermissionsResult_IarrayLjava_lang_String_arrayI;
		}

		static void n_OnRequestPermissionsResult_IarrayLjava_lang_String_arrayI (IntPtr jnienv, IntPtr native__this, int requestCode, IntPtr native_permissions, IntPtr native_grantResults)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			string[] permissions = (string[]) JNIEnv.GetArray (native_permissions, JniHandleOwnership.DoNotTransfer, typeof (string));
			int[] grantResults = (int[]) JNIEnv.GetArray (native_grantResults, JniHandleOwnership.DoNotTransfer, typeof (int));
			__this.OnRequestPermissionsResult (requestCode, permissions, grantResults);
			if (permissions != null)
				JNIEnv.CopyArray (permissions, native_permissions);
			if (grantResults != null)
				JNIEnv.CopyArray (grantResults, native_grantResults);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onRequestPermissionsResult' and count(parameter)=3 and parameter[1][@type='int'] and parameter[2][@type='java.lang.String[]'] and parameter[3][@type='int[]']]"
		[Register ("onRequestPermissionsResult", "(I[Ljava/lang/String;[I)V", "GetOnRequestPermissionsResult_IarrayLjava_lang_String_arrayIHandler")]
		public virtual unsafe void OnRequestPermissionsResult (int requestCode, string[] permissions, int[] grantResults)
		{
			const string __id = "onRequestPermissionsResult.(I[Ljava/lang/String;[I)V";
			IntPtr native_permissions = JNIEnv.NewArray (permissions);
			IntPtr native_grantResults = JNIEnv.NewArray (grantResults);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [3];
				__args [0] = new JniArgumentValue (requestCode);
				__args [1] = new JniArgumentValue (native_permissions);
				__args [2] = new JniArgumentValue (native_grantResults);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
				if (permissions != null) {
					JNIEnv.CopyArray (native_permissions, permissions);
					JNIEnv.DeleteLocalRef (native_permissions);
				}
				if (grantResults != null) {
					JNIEnv.CopyArray (native_grantResults, grantResults);
					JNIEnv.DeleteLocalRef (native_grantResults);
				}
			}
		}

		static Delegate cb_onRestart;
#pragma warning disable 0169
		static Delegate GetOnRestartHandler ()
		{
			if (cb_onRestart == null)
				cb_onRestart = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_OnRestart);
			return cb_onRestart;
		}

		static void n_OnRestart (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnRestart ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onRestart' and count(parameter)=0]"
		[Register ("onRestart", "()V", "GetOnRestartHandler")]
		protected virtual unsafe void OnRestart ()
		{
			const string __id = "onRestart.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_onResume;
#pragma warning disable 0169
		static Delegate GetOnResumeHandler ()
		{
			if (cb_onResume == null)
				cb_onResume = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_OnResume);
			return cb_onResume;
		}

		static void n_OnResume (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnResume ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onResume' and count(parameter)=0]"
		[Register ("onResume", "()V", "GetOnResumeHandler")]
		protected virtual unsafe void OnResume ()
		{
			const string __id = "onResume.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_onRetainNonConfigurationInstance;
#pragma warning disable 0169
		static Delegate GetOnRetainNonConfigurationInstanceHandler ()
		{
			if (cb_onRetainNonConfigurationInstance == null)
				cb_onRetainNonConfigurationInstance = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, IntPtr>) n_OnRetainNonConfigurationInstance);
			return cb_onRetainNonConfigurationInstance;
		}

		static IntPtr n_OnRetainNonConfigurationInstance (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return JNIEnv.ToLocalJniHandle (__this.OnRetainNonConfigurationInstance ());
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onRetainNonConfigurationInstance' and count(parameter)=0]"
		[Register ("onRetainNonConfigurationInstance", "()Ljava/lang/Object;", "GetOnRetainNonConfigurationInstanceHandler")]
		public virtual unsafe global::Java.Lang.Object OnRetainNonConfigurationInstance ()
		{
			const string __id = "onRetainNonConfigurationInstance.()Ljava/lang/Object;";
			try {
				var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, null);
				return global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
			}
		}

		static Delegate cb_onSearchRequested;
#pragma warning disable 0169
		static Delegate GetOnSearchRequestedHandler ()
		{
			if (cb_onSearchRequested == null)
				cb_onSearchRequested = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, bool>) n_OnSearchRequested);
			return cb_onSearchRequested;
		}

		static bool n_OnSearchRequested (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.OnSearchRequested ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onSearchRequested' and count(parameter)=0]"
		[Register ("onSearchRequested", "()Z", "GetOnSearchRequestedHandler")]
		public virtual unsafe bool OnSearchRequested ()
		{
			const string __id = "onSearchRequested.()Z";
			try {
				var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, null);
				return __rm;
			} finally {
			}
		}

		static Delegate cb_onStart;
#pragma warning disable 0169
		static Delegate GetOnStartHandler ()
		{
			if (cb_onStart == null)
				cb_onStart = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_OnStart);
			return cb_onStart;
		}

		static void n_OnStart (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnStart ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onStart' and count(parameter)=0]"
		[Register ("onStart", "()V", "GetOnStartHandler")]
		protected virtual unsafe void OnStart ()
		{
			const string __id = "onStart.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_onStateNotSaved;
#pragma warning disable 0169
		static Delegate GetOnStateNotSavedHandler ()
		{
			if (cb_onStateNotSaved == null)
				cb_onStateNotSaved = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_OnStateNotSaved);
			return cb_onStateNotSaved;
		}

		static void n_OnStateNotSaved (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnStateNotSaved ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onStateNotSaved' and count(parameter)=0]"
		[Register ("onStateNotSaved", "()V", "GetOnStateNotSavedHandler")]
		public virtual unsafe void OnStateNotSaved ()
		{
			const string __id = "onStateNotSaved.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_onStop;
#pragma warning disable 0169
		static Delegate GetOnStopHandler ()
		{
			if (cb_onStop == null)
				cb_onStop = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_OnStop);
			return cb_onStop;
		}

		static void n_OnStop (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnStop ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onStop' and count(parameter)=0]"
		[Register ("onStop", "()V", "GetOnStopHandler")]
		protected virtual unsafe void OnStop ()
		{
			const string __id = "onStop.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_onTitleChanged_Ljava_lang_CharSequence_I;
#pragma warning disable 0169
		static Delegate GetOnTitleChanged_Ljava_lang_CharSequence_IHandler ()
		{
			if (cb_onTitleChanged_Ljava_lang_CharSequence_I == null)
				cb_onTitleChanged_Ljava_lang_CharSequence_I = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, IntPtr, int>) n_OnTitleChanged_Ljava_lang_CharSequence_I);
			return cb_onTitleChanged_Ljava_lang_CharSequence_I;
		}

		static void n_OnTitleChanged_Ljava_lang_CharSequence_I (IntPtr jnienv, IntPtr native__this, IntPtr native_title, int color)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			global::Java.Lang.ICharSequence title = global::Java.Lang.Object.GetObject<global::Java.Lang.ICharSequence> (native_title, JniHandleOwnership.DoNotTransfer);
			__this.OnTitleChanged (title, color);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onTitleChanged' and count(parameter)=2 and parameter[1][@type='java.lang.CharSequence'] and parameter[2][@type='int']]"
		[Register ("onTitleChanged", "(Ljava/lang/CharSequence;I)V", "GetOnTitleChanged_Ljava_lang_CharSequence_IHandler")]
		protected virtual unsafe void OnTitleChanged (global::Java.Lang.ICharSequence title, int color)
		{
			const string __id = "onTitleChanged.(Ljava/lang/CharSequence;I)V";
			IntPtr native_title = CharSequence.ToLocalJniHandle (title);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [2];
				__args [0] = new JniArgumentValue (native_title);
				__args [1] = new JniArgumentValue (color);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
				JNIEnv.DeleteLocalRef (native_title);
			}
		}

		protected void OnTitleChanged (string title, int color)
		{
			global::Java.Lang.String jls_title = title == null ? null : new global::Java.Lang.String (title);
			OnTitleChanged (jls_title, color);
			jls_title?.Dispose ();
		}

		static Delegate cb_onTrimMemory_I;
#pragma warning disable 0169
		static Delegate GetOnTrimMemory_IHandler ()
		{
			if (cb_onTrimMemory_I == null)
				cb_onTrimMemory_I = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, int>) n_OnTrimMemory_I);
			return cb_onTrimMemory_I;
		}

		static void n_OnTrimMemory_I (IntPtr jnienv, IntPtr native__this, int level)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnTrimMemory (level);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onTrimMemory' and count(parameter)=1 and parameter[1][@type='int']]"
		[Register ("onTrimMemory", "(I)V", "GetOnTrimMemory_IHandler")]
		public virtual unsafe void OnTrimMemory (int level)
		{
			const string __id = "onTrimMemory.(I)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (level);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_onUserInteraction;
#pragma warning disable 0169
		static Delegate GetOnUserInteractionHandler ()
		{
			if (cb_onUserInteraction == null)
				cb_onUserInteraction = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_OnUserInteraction);
			return cb_onUserInteraction;
		}

		static void n_OnUserInteraction (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnUserInteraction ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onUserInteraction' and count(parameter)=0]"
		[Register ("onUserInteraction", "()V", "GetOnUserInteractionHandler")]
		public virtual unsafe void OnUserInteraction ()
		{
			const string __id = "onUserInteraction.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_onUserLeaveHint;
#pragma warning disable 0169
		static Delegate GetOnUserLeaveHintHandler ()
		{
			if (cb_onUserLeaveHint == null)
				cb_onUserLeaveHint = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_OnUserLeaveHint);
			return cb_onUserLeaveHint;
		}

		static void n_OnUserLeaveHint (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnUserLeaveHint ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onUserLeaveHint' and count(parameter)=0]"
		[Register ("onUserLeaveHint", "()V", "GetOnUserLeaveHintHandler")]
		protected virtual unsafe void OnUserLeaveHint ()
		{
			const string __id = "onUserLeaveHint.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_onVisibleBehindCanceled;
#pragma warning disable 0169
		static Delegate GetOnVisibleBehindCanceledHandler ()
		{
			if (cb_onVisibleBehindCanceled == null)
				cb_onVisibleBehindCanceled = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_OnVisibleBehindCanceled);
			return cb_onVisibleBehindCanceled;
		}

		static void n_OnVisibleBehindCanceled (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnVisibleBehindCanceled ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onVisibleBehindCanceled' and count(parameter)=0]"
		[Obsolete (@"deprecated")]
		[Register ("onVisibleBehindCanceled", "()V", "GetOnVisibleBehindCanceledHandler")]
		public virtual unsafe void OnVisibleBehindCanceled ()
		{
			const string __id = "onVisibleBehindCanceled.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_onWindowFocusChanged_Z;
#pragma warning disable 0169
		static Delegate GetOnWindowFocusChanged_ZHandler ()
		{
			if (cb_onWindowFocusChanged_Z == null)
				cb_onWindowFocusChanged_Z = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, bool>) n_OnWindowFocusChanged_Z);
			return cb_onWindowFocusChanged_Z;
		}

		static void n_OnWindowFocusChanged_Z (IntPtr jnienv, IntPtr native__this, bool hasFocus)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnWindowFocusChanged (hasFocus);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='onWindowFocusChanged' and count(parameter)=1 and parameter[1][@type='boolean']]"
		[Register ("onWindowFocusChanged", "(Z)V", "GetOnWindowFocusChanged_ZHandler")]
		public virtual unsafe void OnWindowFocusChanged (bool hasFocus)
		{
			const string __id = "onWindowFocusChanged.(Z)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (hasFocus);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_openOptionsMenu;
#pragma warning disable 0169
		static Delegate GetOpenOptionsMenuHandler ()
		{
			if (cb_openOptionsMenu == null)
				cb_openOptionsMenu = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_OpenOptionsMenu);
			return cb_openOptionsMenu;
		}

		static void n_OpenOptionsMenu (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OpenOptionsMenu ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='openOptionsMenu' and count(parameter)=0]"
		[Register ("openOptionsMenu", "()V", "GetOpenOptionsMenuHandler")]
		public virtual unsafe void OpenOptionsMenu ()
		{
			const string __id = "openOptionsMenu.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_overridePendingTransition_II;
#pragma warning disable 0169
		static Delegate GetOverridePendingTransition_IIHandler ()
		{
			if (cb_overridePendingTransition_II == null)
				cb_overridePendingTransition_II = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, int, int>) n_OverridePendingTransition_II);
			return cb_overridePendingTransition_II;
		}

		static void n_OverridePendingTransition_II (IntPtr jnienv, IntPtr native__this, int enterAnim, int exitAnim)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OverridePendingTransition (enterAnim, exitAnim);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='overridePendingTransition' and count(parameter)=2 and parameter[1][@type='int'] and parameter[2][@type='int']]"
		[Register ("overridePendingTransition", "(II)V", "GetOverridePendingTransition_IIHandler")]
		public virtual unsafe void OverridePendingTransition (int enterAnim, int exitAnim)
		{
			const string __id = "overridePendingTransition.(II)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [2];
				__args [0] = new JniArgumentValue (enterAnim);
				__args [1] = new JniArgumentValue (exitAnim);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_postponeEnterTransition;
#pragma warning disable 0169
		static Delegate GetPostponeEnterTransitionHandler ()
		{
			if (cb_postponeEnterTransition == null)
				cb_postponeEnterTransition = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_PostponeEnterTransition);
			return cb_postponeEnterTransition;
		}

		static void n_PostponeEnterTransition (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.PostponeEnterTransition ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='postponeEnterTransition' and count(parameter)=0]"
		[Register ("postponeEnterTransition", "()V", "GetPostponeEnterTransitionHandler")]
		public virtual unsafe void PostponeEnterTransition ()
		{
			const string __id = "postponeEnterTransition.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_recreate;
#pragma warning disable 0169
		static Delegate GetRecreateHandler ()
		{
			if (cb_recreate == null)
				cb_recreate = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_Recreate);
			return cb_recreate;
		}

		static void n_Recreate (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.Recreate ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='recreate' and count(parameter)=0]"
		[Register ("recreate", "()V", "GetRecreateHandler")]
		public virtual unsafe void Recreate ()
		{
			const string __id = "recreate.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_releaseInstance;
#pragma warning disable 0169
		static Delegate GetReleaseInstanceHandler ()
		{
			if (cb_releaseInstance == null)
				cb_releaseInstance = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, bool>) n_ReleaseInstance);
			return cb_releaseInstance;
		}

		static bool n_ReleaseInstance (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.ReleaseInstance ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='releaseInstance' and count(parameter)=0]"
		[Register ("releaseInstance", "()Z", "GetReleaseInstanceHandler")]
		public virtual unsafe bool ReleaseInstance ()
		{
			const string __id = "releaseInstance.()Z";
			try {
				var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, null);
				return __rm;
			} finally {
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='removeDialog' and count(parameter)=1 and parameter[1][@type='int']]"
		[Obsolete (@"deprecated")]
		[Register ("removeDialog", "(I)V", "")]
		public unsafe void RemoveDialog (int id)
		{
			const string __id = "removeDialog.(I)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (id);
				_members.InstanceMethods.InvokeNonvirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_reportFullyDrawn;
#pragma warning disable 0169
		static Delegate GetReportFullyDrawnHandler ()
		{
			if (cb_reportFullyDrawn == null)
				cb_reportFullyDrawn = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_ReportFullyDrawn);
			return cb_reportFullyDrawn;
		}

		static void n_ReportFullyDrawn (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.ReportFullyDrawn ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='reportFullyDrawn' and count(parameter)=0]"
		[Register ("reportFullyDrawn", "()V", "GetReportFullyDrawnHandler")]
		public virtual unsafe void ReportFullyDrawn ()
		{
			const string __id = "reportFullyDrawn.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='requestPermissions' and count(parameter)=2 and parameter[1][@type='java.lang.String[]'] and parameter[2][@type='int']]"
		[Register ("requestPermissions", "([Ljava/lang/String;I)V", "")]
		public unsafe void RequestPermissions (string[] permissions, int requestCode)
		{
			const string __id = "requestPermissions.([Ljava/lang/String;I)V";
			IntPtr native_permissions = JNIEnv.NewArray (permissions);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [2];
				__args [0] = new JniArgumentValue (native_permissions);
				__args [1] = new JniArgumentValue (requestCode);
				_members.InstanceMethods.InvokeNonvirtualVoidMethod (__id, this, __args);
			} finally {
				if (permissions != null) {
					JNIEnv.CopyArray (native_permissions, permissions);
					JNIEnv.DeleteLocalRef (native_permissions);
				}
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='requestShowKeyboardShortcuts' and count(parameter)=0]"
		[Register ("requestShowKeyboardShortcuts", "()V", "")]
		public unsafe void RequestShowKeyboardShortcuts ()
		{
			const string __id = "requestShowKeyboardShortcuts.()V";
			try {
				_members.InstanceMethods.InvokeNonvirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_requestVisibleBehind_Z;
#pragma warning disable 0169
		static Delegate GetRequestVisibleBehind_ZHandler ()
		{
			if (cb_requestVisibleBehind_Z == null)
				cb_requestVisibleBehind_Z = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, bool, bool>) n_RequestVisibleBehind_Z);
			return cb_requestVisibleBehind_Z;
		}

		static bool n_RequestVisibleBehind_Z (IntPtr jnienv, IntPtr native__this, bool visible)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.RequestVisibleBehind (visible);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='requestVisibleBehind' and count(parameter)=1 and parameter[1][@type='boolean']]"
		[Obsolete (@"deprecated")]
		[Register ("requestVisibleBehind", "(Z)Z", "GetRequestVisibleBehind_ZHandler")]
		public virtual unsafe bool RequestVisibleBehind (bool visible)
		{
			const string __id = "requestVisibleBehind.(Z)Z";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (visible);
				var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, __args);
				return __rm;
			} finally {
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='requestWindowFeature' and count(parameter)=1 and parameter[1][@type='int']]"
		[Register ("requestWindowFeature", "(I)Z", "")]
		public unsafe bool RequestWindowFeature (int featureId)
		{
			const string __id = "requestWindowFeature.(I)Z";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (featureId);
				var __rm = _members.InstanceMethods.InvokeNonvirtualBooleanMethod (__id, this, __args);
				return __rm;
			} finally {
			}
		}

		static Delegate cb_setContentView_I;
#pragma warning disable 0169
		static Delegate GetSetContentView_IHandler ()
		{
			if (cb_setContentView_I == null)
				cb_setContentView_I = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, int>) n_SetContentView_I);
			return cb_setContentView_I;
		}

		static void n_SetContentView_I (IntPtr jnienv, IntPtr native__this, int layoutResID)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.SetContentView (layoutResID);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setContentView' and count(parameter)=1 and parameter[1][@type='int']]"
		[Register ("setContentView", "(I)V", "GetSetContentView_IHandler")]
		public virtual unsafe void SetContentView (int layoutResID)
		{
			const string __id = "setContentView.(I)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (layoutResID);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setDefaultKeyMode' and count(parameter)=1 and parameter[1][@type='int']]"
		[Register ("setDefaultKeyMode", "(I)V", "")]
		public unsafe void SetDefaultKeyMode (int mode)
		{
			const string __id = "setDefaultKeyMode.(I)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (mode);
				_members.InstanceMethods.InvokeNonvirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setFeatureDrawableAlpha' and count(parameter)=2 and parameter[1][@type='int'] and parameter[2][@type='int']]"
		[Register ("setFeatureDrawableAlpha", "(II)V", "")]
		public unsafe void SetFeatureDrawableAlpha (int featureId, int alpha)
		{
			const string __id = "setFeatureDrawableAlpha.(II)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [2];
				__args [0] = new JniArgumentValue (featureId);
				__args [1] = new JniArgumentValue (alpha);
				_members.InstanceMethods.InvokeNonvirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setFeatureDrawableResource' and count(parameter)=2 and parameter[1][@type='int'] and parameter[2][@type='int']]"
		[Register ("setFeatureDrawableResource", "(II)V", "")]
		public unsafe void SetFeatureDrawableResource (int featureId, int resId)
		{
			const string __id = "setFeatureDrawableResource.(II)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [2];
				__args [0] = new JniArgumentValue (featureId);
				__args [1] = new JniArgumentValue (resId);
				_members.InstanceMethods.InvokeNonvirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_setFinishOnTouchOutside_Z;
#pragma warning disable 0169
		static Delegate GetSetFinishOnTouchOutside_ZHandler ()
		{
			if (cb_setFinishOnTouchOutside_Z == null)
				cb_setFinishOnTouchOutside_Z = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, bool>) n_SetFinishOnTouchOutside_Z);
			return cb_setFinishOnTouchOutside_Z;
		}

		static void n_SetFinishOnTouchOutside_Z (IntPtr jnienv, IntPtr native__this, bool finish)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.SetFinishOnTouchOutside (finish);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setFinishOnTouchOutside' and count(parameter)=1 and parameter[1][@type='boolean']]"
		[Register ("setFinishOnTouchOutside", "(Z)V", "GetSetFinishOnTouchOutside_ZHandler")]
		public virtual unsafe void SetFinishOnTouchOutside (bool finish)
		{
			const string __id = "setFinishOnTouchOutside.(Z)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (finish);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_setPersistent_Z;
#pragma warning disable 0169
		static Delegate GetSetPersistent_ZHandler ()
		{
			if (cb_setPersistent_Z == null)
				cb_setPersistent_Z = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, bool>) n_SetPersistent_Z);
			return cb_setPersistent_Z;
		}

		static void n_SetPersistent_Z (IntPtr jnienv, IntPtr native__this, bool isPersistent)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.SetPersistent (isPersistent);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setPersistent' and count(parameter)=1 and parameter[1][@type='boolean']]"
		[Register ("setPersistent", "(Z)V", "GetSetPersistent_ZHandler")]
		public virtual unsafe void SetPersistent (bool isPersistent)
		{
			const string __id = "setPersistent.(Z)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (isPersistent);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setProgress' and count(parameter)=1 and parameter[1][@type='int']]"
		[Obsolete (@"deprecated")]
		[Register ("setProgress", "(I)V", "")]
		public unsafe void SetProgress (int progress)
		{
			const string __id = "setProgress.(I)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (progress);
				_members.InstanceMethods.InvokeNonvirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setProgressBarIndeterminate' and count(parameter)=1 and parameter[1][@type='boolean']]"
		[Obsolete (@"deprecated")]
		[Register ("setProgressBarIndeterminate", "(Z)V", "")]
		public unsafe void SetProgressBarIndeterminate (bool indeterminate)
		{
			const string __id = "setProgressBarIndeterminate.(Z)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (indeterminate);
				_members.InstanceMethods.InvokeNonvirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setProgressBarIndeterminateVisibility' and count(parameter)=1 and parameter[1][@type='boolean']]"
		[Obsolete (@"deprecated")]
		[Register ("setProgressBarIndeterminateVisibility", "(Z)V", "")]
		public unsafe void SetProgressBarIndeterminateVisibility (bool visible)
		{
			const string __id = "setProgressBarIndeterminateVisibility.(Z)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (visible);
				_members.InstanceMethods.InvokeNonvirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setProgressBarVisibility' and count(parameter)=1 and parameter[1][@type='boolean']]"
		[Obsolete (@"deprecated")]
		[Register ("setProgressBarVisibility", "(Z)V", "")]
		public unsafe void SetProgressBarVisibility (bool visible)
		{
			const string __id = "setProgressBarVisibility.(Z)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (visible);
				_members.InstanceMethods.InvokeNonvirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setResult' and count(parameter)=1 and parameter[1][@type='int']]"
		[Register ("setResult", "(I)V", "")]
		public unsafe void SetResult (int resultCode)
		{
			const string __id = "setResult.(I)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (resultCode);
				_members.InstanceMethods.InvokeNonvirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setSecondaryProgress' and count(parameter)=1 and parameter[1][@type='int']]"
		[Obsolete (@"deprecated")]
		[Register ("setSecondaryProgress", "(I)V", "")]
		public unsafe void SetSecondaryProgress (int secondaryProgress)
		{
			const string __id = "setSecondaryProgress.(I)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (secondaryProgress);
				_members.InstanceMethods.InvokeNonvirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_setShowWhenLocked_Z;
#pragma warning disable 0169
		static Delegate GetSetShowWhenLocked_ZHandler ()
		{
			if (cb_setShowWhenLocked_Z == null)
				cb_setShowWhenLocked_Z = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, bool>) n_SetShowWhenLocked_Z);
			return cb_setShowWhenLocked_Z;
		}

		static void n_SetShowWhenLocked_Z (IntPtr jnienv, IntPtr native__this, bool showWhenLocked)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.SetShowWhenLocked (showWhenLocked);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setShowWhenLocked' and count(parameter)=1 and parameter[1][@type='boolean']]"
		[Register ("setShowWhenLocked", "(Z)V", "GetSetShowWhenLocked_ZHandler")]
		public virtual unsafe void SetShowWhenLocked (bool showWhenLocked)
		{
			const string __id = "setShowWhenLocked.(Z)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (showWhenLocked);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_setTitle_I;
#pragma warning disable 0169
		static Delegate GetSetTitle_IHandler ()
		{
			if (cb_setTitle_I == null)
				cb_setTitle_I = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, int>) n_SetTitle_I);
			return cb_setTitle_I;
		}

		static void n_SetTitle_I (IntPtr jnienv, IntPtr native__this, int titleId)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.SetTitle (titleId);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setTitle' and count(parameter)=1 and parameter[1][@type='int']]"
		[Register ("setTitle", "(I)V", "GetSetTitle_IHandler")]
		public virtual unsafe void SetTitle (int titleId)
		{
			const string __id = "setTitle.(I)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (titleId);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_setTurnScreenOn_Z;
#pragma warning disable 0169
		static Delegate GetSetTurnScreenOn_ZHandler ()
		{
			if (cb_setTurnScreenOn_Z == null)
				cb_setTurnScreenOn_Z = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, bool>) n_SetTurnScreenOn_Z);
			return cb_setTurnScreenOn_Z;
		}

		static void n_SetTurnScreenOn_Z (IntPtr jnienv, IntPtr native__this, bool turnScreenOn)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.SetTurnScreenOn (turnScreenOn);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setTurnScreenOn' and count(parameter)=1 and parameter[1][@type='boolean']]"
		[Register ("setTurnScreenOn", "(Z)V", "GetSetTurnScreenOn_ZHandler")]
		public virtual unsafe void SetTurnScreenOn (bool turnScreenOn)
		{
			const string __id = "setTurnScreenOn.(Z)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (turnScreenOn);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_setVisible_Z;
#pragma warning disable 0169
		static Delegate GetSetVisible_ZHandler ()
		{
			if (cb_setVisible_Z == null)
				cb_setVisible_Z = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, bool>) n_SetVisible_Z);
			return cb_setVisible_Z;
		}

		static void n_SetVisible_Z (IntPtr jnienv, IntPtr native__this, bool visible)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.SetVisible (visible);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='setVisible' and count(parameter)=1 and parameter[1][@type='boolean']]"
		[Register ("setVisible", "(Z)V", "GetSetVisible_ZHandler")]
		public virtual unsafe void SetVisible (bool visible)
		{
			const string __id = "setVisible.(Z)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (visible);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_shouldShowRequestPermissionRationale_Ljava_lang_String_;
#pragma warning disable 0169
		static Delegate GetShouldShowRequestPermissionRationale_Ljava_lang_String_Handler ()
		{
			if (cb_shouldShowRequestPermissionRationale_Ljava_lang_String_ == null)
				cb_shouldShowRequestPermissionRationale_Ljava_lang_String_ = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, IntPtr, bool>) n_ShouldShowRequestPermissionRationale_Ljava_lang_String_);
			return cb_shouldShowRequestPermissionRationale_Ljava_lang_String_;
		}

		static bool n_ShouldShowRequestPermissionRationale_Ljava_lang_String_ (IntPtr jnienv, IntPtr native__this, IntPtr native_permission)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			string permission = JNIEnv.GetString (native_permission, JniHandleOwnership.DoNotTransfer);
			bool __ret = __this.ShouldShowRequestPermissionRationale (permission);
			return __ret;
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='shouldShowRequestPermissionRationale' and count(parameter)=1 and parameter[1][@type='java.lang.String']]"
		[Register ("shouldShowRequestPermissionRationale", "(Ljava/lang/String;)Z", "GetShouldShowRequestPermissionRationale_Ljava_lang_String_Handler")]
		public virtual unsafe bool ShouldShowRequestPermissionRationale (string permission)
		{
			const string __id = "shouldShowRequestPermissionRationale.(Ljava/lang/String;)Z";
			IntPtr native_permission = JNIEnv.NewString (permission);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (native_permission);
				var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, __args);
				return __rm;
			} finally {
				JNIEnv.DeleteLocalRef (native_permission);
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='showDialog' and count(parameter)=1 and parameter[1][@type='int']]"
		[Obsolete (@"deprecated")]
		[Register ("showDialog", "(I)V", "")]
		public unsafe void ShowDialog (int id)
		{
			const string __id = "showDialog.(I)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (id);
				_members.InstanceMethods.InvokeNonvirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

		static Delegate cb_showLockTaskEscapeMessage;
#pragma warning disable 0169
		static Delegate GetShowLockTaskEscapeMessageHandler ()
		{
			if (cb_showLockTaskEscapeMessage == null)
				cb_showLockTaskEscapeMessage = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_ShowLockTaskEscapeMessage);
			return cb_showLockTaskEscapeMessage;
		}

		static void n_ShowLockTaskEscapeMessage (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.ShowLockTaskEscapeMessage ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='showLockTaskEscapeMessage' and count(parameter)=0]"
		[Register ("showLockTaskEscapeMessage", "()V", "GetShowLockTaskEscapeMessageHandler")]
		public virtual unsafe void ShowLockTaskEscapeMessage ()
		{
			const string __id = "showLockTaskEscapeMessage.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_startLockTask;
#pragma warning disable 0169
		static Delegate GetStartLockTaskHandler ()
		{
			if (cb_startLockTask == null)
				cb_startLockTask = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_StartLockTask);
			return cb_startLockTask;
		}

		static void n_StartLockTask (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.StartLockTask ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='startLockTask' and count(parameter)=0]"
		[Register ("startLockTask", "()V", "GetStartLockTaskHandler")]
		public virtual unsafe void StartLockTask ()
		{
			const string __id = "startLockTask.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_startPostponedEnterTransition;
#pragma warning disable 0169
		static Delegate GetStartPostponedEnterTransitionHandler ()
		{
			if (cb_startPostponedEnterTransition == null)
				cb_startPostponedEnterTransition = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_StartPostponedEnterTransition);
			return cb_startPostponedEnterTransition;
		}

		static void n_StartPostponedEnterTransition (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.StartPostponedEnterTransition ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='startPostponedEnterTransition' and count(parameter)=0]"
		[Register ("startPostponedEnterTransition", "()V", "GetStartPostponedEnterTransitionHandler")]
		public virtual unsafe void StartPostponedEnterTransition ()
		{
			const string __id = "startPostponedEnterTransition.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_stopLocalVoiceInteraction;
#pragma warning disable 0169
		static Delegate GetStopLocalVoiceInteractionHandler ()
		{
			if (cb_stopLocalVoiceInteraction == null)
				cb_stopLocalVoiceInteraction = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_StopLocalVoiceInteraction);
			return cb_stopLocalVoiceInteraction;
		}

		static void n_StopLocalVoiceInteraction (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.StopLocalVoiceInteraction ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='stopLocalVoiceInteraction' and count(parameter)=0]"
		[Register ("stopLocalVoiceInteraction", "()V", "GetStopLocalVoiceInteractionHandler")]
		public virtual unsafe void StopLocalVoiceInteraction ()
		{
			const string __id = "stopLocalVoiceInteraction.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_stopLockTask;
#pragma warning disable 0169
		static Delegate GetStopLockTaskHandler ()
		{
			if (cb_stopLockTask == null)
				cb_stopLockTask = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr>) n_StopLockTask);
			return cb_stopLockTask;
		}

		static void n_StopLockTask (IntPtr jnienv, IntPtr native__this)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.StopLockTask ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='stopLockTask' and count(parameter)=0]"
		[Register ("stopLockTask", "()V", "GetStopLockTaskHandler")]
		public virtual unsafe void StopLockTask ()
		{
			const string __id = "stopLockTask.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_takeKeyEvents_Z;
#pragma warning disable 0169
		static Delegate GetTakeKeyEvents_ZHandler ()
		{
			if (cb_takeKeyEvents_Z == null)
				cb_takeKeyEvents_Z = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, bool>) n_TakeKeyEvents_Z);
			return cb_takeKeyEvents_Z;
		}

		static void n_TakeKeyEvents_Z (IntPtr jnienv, IntPtr native__this, bool get)
		{
			global::Android.App.Activity __this = global::Java.Lang.Object.GetObject<global::Android.App.Activity> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.TakeKeyEvents (get);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='android.app']/class[@name='Activity']/method[@name='takeKeyEvents' and count(parameter)=1 and parameter[1][@type='boolean']]"
		[Register ("takeKeyEvents", "(Z)V", "GetTakeKeyEvents_ZHandler")]
		public virtual unsafe void TakeKeyEvents (bool get)
		{
			const string __id = "takeKeyEvents.(Z)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (get);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
			}
		}

	}
}
