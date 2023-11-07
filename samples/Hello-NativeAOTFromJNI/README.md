# Hello From JNI

[JNI][0] supports *two* modes of operation:

 1. Native code creates the JVM, e.g. via [`JNI_CreateJavaVM()`][1], or
 2. The JVM already exists, and calls [`JNI_OnLoad()`][2] when loading a native library.

Java.Interop samples and unit tests rely on the first approach.

.NET Android / neé Xamarin.Android is the second approach.

Bring an example of the latter into a Java.Interop sample, using [NativeAOT][3].

## Building

Building a native library with NativeAOT requires a Release configuration build.
For in-repo use, that means that xamarin/Java.Interop itself needs to be built in
Release configuration:

```sh
% dotnet build -c Release -t:Prepare
% dotnet build -c Release
```

Once Java.Interop itself is built, you can build the sample:

```sh
% dotnet publish -c Release -r osx-x64
```

The resulting native library contains the desired symbols:

```sh
% nm bin/Release/osx-x64/publish/Hello-NativeAOTFromJNI.dylib | grep ' S ' 
000000000016f5a0 S _JNI_OnLoad
000000000016f5d0 S _JNI_OnUnload
000000000016f2f0 S _Java_com_microsoft_hello_1from_1jni_App_sayHello
000000000016f620 S _Java_com_microsoft_java_1interop_JavaInteropRuntime_init
```

Use the `RunJavaSample` target to run Java, which will run
`System.loadLibrary("Hello-NativeAOTFromJNI")`, which will cause the
NativeAOT-generated `libHello-NativeAOTFromJNI.dylib` to be run:

```sh
% dotnet build -c Release -r osx-x64 -t:RunJavaSample  -v m --nologo --no-restore
  Hello from Java!
  C# init()
  Hello from .NET NativeAOT!
  String returned to Java: Hello from .NET NativeAOT!
  C# RegisterNativeMembers(JniType(Name='example/ManagedType' PeerReference=0x7fd0a00072d8/G), "Example.ManagedType, Hello-NativeAOTFromJNI", "getString:()Ljava/lang/String;:__export__
  ")
  # jonp: called `Example.ManagedType/__<$>_jni_marshal_methods.__RegisterNativeMembers()` w/ 1 methods to register.
  mt.getString()=Hello from C#, via Java.Interop!

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:00.73

% (cd bin/Release/osx-x64/publish ; java -cp hello-from-java.jar:java-interop.jar com/microsoft/hello_from_jni/App)
Hello from Java!
C# init()
Hello from .NET NativeAOT!
String returned to Java: Hello from .NET NativeAOT!
C# RegisterNativeMembers(JniType(Name='example/ManagedType' PeerReference=0x7fa822114598/G), "Example.ManagedType, Hello-NativeAOTFromJNI", "getString:()Ljava/lang/String;:__export__
")
# jonp: called `Example.ManagedType/__<$>_jni_marshal_methods.__RegisterNativeMembers()` w/ 1 methods to register.
mt.getString()=Hello from C#, via Java.Interop!
```

Note the use of `(cd …; java …)` so that `libHello-NativeAOTFromJNI.dylib` is
in the current working directory, so that it can be found.

# Known Unknowns

With this sample "done" (-ish), there are several "future research directions" to
make NativeAOT + Java *viable*.

## GC

Firstly, there's the open GC question: NativeAOT doesn't provide a "GC Bridge"
like MonoVM does, so how do we support cross-VM object references?

  * [Collecting Cyclic Garbage across Foreign Function Interfaces: Who Takes the Last Piece of Cake?](https://pldi23.sigplan.org/details/pldi-2023-pldi/25/Collecting-Cyclic-Garbage-across-Foreign-Function-Interfaces-Who-Takes-the-Last-Piec)
  * [`JavaScope`?](https://github.com/jonpryor/java.interop/commits/jonp-registration-scope)
    (Less a "solution" and more a "Glorious Workaround".)

## `Type.GetType()`

Next, Java.Interop and .NET Android make *extensive* use of `Type.GetType()`,
which doesn't quite work "the same" in NativeAOT.  It works when using a string
constant:

```csharp
var type = Type.GetType ("System.Int32, System.Runtime");
```

It fails if the string comes from "elsewhere", even if it's a type that exists.

Unfortunately, we do this *everywhere* in Java.Interop.  Consider this more
complete Java Callable Wrapper fragment:

```java
public class ManagedType
	extends java.lang.Object
	implements
		com.xamarin.java_interop.GCUserPeerable
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"getString:()Ljava/lang/String;:__export__\n" +
			"";
		com.xamarin.java_interop.ManagedPeer.registerNativeMembers (
				ManagedType.class,
				"Example.ManagedType, Hello-NativeAOTFromJNI",
				__md_methods);
	}


	public ManagedType (int p0)
	{
		super ();
		if (getClass () == ManagedType.class) {
			com.xamarin.java_interop.ManagedPeer.construct (
					this,
					"Example.ManagedType, Hello-NativeAOTFromJNI",
					"System.Int32, System.Runtime",
					new java.lang.Object[] { p0 });
		}
	}


	public native java.lang.String getString ();
}
```

There are *two* places that assembly-qualified names are used, both of which
currently wind up at `Type.GetType()`:

  * `ManagedPeer.RegisterNativeMembers()` is given an assembly-qualified name
    to register the `native` methods.
  * `ManagedPeer.Construct()` is given a `:`-separated list of assembly-qualified
    names for each parameter type.  This is done to lookup a `ConstructorInfo`.

This sample "fixes" `ManagedPeer.RegisterNativeMembers()` by adding a new
`JniRuntime.JniTypeManager.RegisterNativeMembers()` overload which *avoids* the
`Type.GetType()` call, which allows `NativeAotTypeManager` to "do something else".

This sample "avoids" `ManagedPeer.Construct()` by not using any parameter types
in the constructor!  If we add any, e.g. via this patch:

```diff
diff --git a/samples/Hello-NativeAOTFromJNI/JavaInteropRuntime.cs b/samples/Hello-NativeAOTFromJNI/JavaInteropRuntime.cs
index 607bd73f..7ed83c59 100644
--- a/samples/Hello-NativeAOTFromJNI/JavaInteropRuntime.cs
+++ b/samples/Hello-NativeAOTFromJNI/JavaInteropRuntime.cs
@@ -31,9 +31,18 @@ static class JavaInteropRuntime
 				ValueManager        = new NativeAotValueManager (),
 			};
 			runtime = options.CreateJreVM ();
+#pragma warning disable IL2057
+			var t = Type.GetType (CreateTypeName (), throwOnError: true);
+#pragma warning restore IL2057
+			Console.WriteLine ($"# jonp: found System.Int32: {t}");
 		}
 		catch (Exception e) {
 			Console.Error.WriteLine ($"JavaInteropRuntime.init: error: {e}");
 		}
 	}
+
+	static string CreateTypeName () =>
+		new System.Text.StringBuilder ().Append ("System").Append (".").Append ("Int32")
+			.Append (", ").Append ("System").Append (".").Append ("Runtime")
+			.ToString ();
 }
diff --git a/samples/Hello-NativeAOTFromJNI/ManagedType.cs b/samples/Hello-NativeAOTFromJNI/ManagedType.cs
index c5224a40..5db7af84 100644
--- a/samples/Hello-NativeAOTFromJNI/ManagedType.cs
+++ b/samples/Hello-NativeAOTFromJNI/ManagedType.cs
@@ -5,14 +5,17 @@ using Java.Interop;
 [JniTypeSignature ("example/ManagedType")]
 class ManagedType : Java.Lang.Object {
 
-	[JavaCallableConstructor]
-	public ManagedType ()
+	[JavaCallableConstructor(SuperConstructorExpression="")]
+	public ManagedType (int value)
 	{
+		this.value = value;
 	}
 
+	int value;
+
 	[JavaCallable ("getString")]
 	public Java.Lang.String GetString ()
 	{
-		return new Java.Lang.String ("Hello from C#, via Java.Interop!");
+		return new Java.Lang.String ($"Hello from C#, via Java.Interop!  Value={value}.");
 	}
 }
diff --git a/samples/Hello-NativeAOTFromJNI/java/com/microsoft/hello_from_jni/App.java b/samples/Hello-NativeAOTFromJNI/java/com/microsoft/hello_from_jni/App.java
index f6d6fff2..f4764cf1 100644
--- a/samples/Hello-NativeAOTFromJNI/java/com/microsoft/hello_from_jni/App.java
+++ b/samples/Hello-NativeAOTFromJNI/java/com/microsoft/hello_from_jni/App.java
@@ -10,7 +10,7 @@ class App {
         JavaInteropRuntime.init();
         String s = sayHello();
         System.out.println("String returned to Java: " + s);
-        ManagedType mt = new ManagedType();
+        ManagedType mt = new ManagedType(42);
         System.out.println("mt.getString()=" + mt.getString());
     }
 
```

this will fail at runtime:

```
Exception in thread "main" com.xamarin.java_interop.internal.JavaProxyThrowable: System.IO.FileNotFoundException: Could not resolve assembly 'System.Runtime'.
   at System.Reflection.TypeNameParser.ResolveAssembly(String) + 0x97
   at System.Reflection.TypeNameParser.GetType(String, ReadOnlySpan`1, String) + 0x32
   at System.Reflection.TypeNameParser.NamespaceTypeName.ResolveType(TypeNameParser&, String) + 0x17
   at System.Reflection.TypeNameParser.GetType(String, Func`2, Func`4, Boolean, Boolean, Boolean, String) + 0x99
   at Java.Interop.ManagedPeer.GetParameterTypes(String) + 0xc1
   at Java.Interop.ManagedPeer.Construct(IntPtr jnienv, IntPtr klass, IntPtr n_self, IntPtr n_assemblyQualifiedName, IntPtr n_constructorSignature, IntPtr n_constructorArguments) + 0x293
	at com.xamarin.java_interop.ManagedPeer.construct(Native Method)
	at example.ManagedType.<init>(ManagedType.java:23)
	at com.microsoft.hello_from_jni.App.main(App.java:13)
```

This isn't impossible -- a straightforward fix would be to declare `native`
methods for each constructor overload -- but fixing this gets increasingly difficult.

(Possible "quick hack": replace `Type.GetType()` use with calls to something
on `JniRuntime.JniTypeManager`, allowing a subclass to provide its own
mapping?  This feels "duplicative" of dotnet/runtime, though.)

## Type Maps

A "derivative" of the `Type.GetType()` problem is that Java.Interop needs a way
to associate a Java type to a .NET `System.Type` instance, for all manner of
reasons.  (One such reason: `JniRuntime.JniValueManager.GetValue()` needs to
know the associated type so that it can create a "peer wrapper", if needed.)

Java.Interop unit tests "hack" around this by using a dictionary in TestJVM,
and `Hello-NativeAOTFromJNI` follows suite.  This isn't a "real" answer, though.

.NET Android has a very complicated typemap mechanism that involves a table
between the Java JNI name and an { assembly name, type token } pair, along with
copious use of MonoVM embedding API such as `mono_class_get()`.  ***A Lot***
of effort has gone into making type maps performant.

How do we "do" type maps in NativeAOT?  We may need to consider some equivalent
to the iOS "static registrar", and this also needs to support getting `Type`
instances for non-`public` types.  There are also concerns about initialization
overhead; a `Dictionary<string, Type>` will require loading and resolving
*all* the `Type` instances as part of startup, which *can't* be good for
reducing startup time.  What other data structure could be used?

[0]: https://docs.oracle.com/javase/8/docs/technotes/guides/jni/spec/jniTOC.html
[1]: https://docs.oracle.com/javase/8/docs/technotes/guides/jni/spec/invocation.html#creating_the_vm
[2]: https://docs.oracle.com/javase/8/docs/technotes/guides/jni/spec/invocation.html#JNJI_OnLoad
[3]: https://github.com/dotnet/samples/blob/main/core/nativeaot/NativeLibrary/README.md