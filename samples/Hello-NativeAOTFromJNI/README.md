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
% dotnet build -c Release
% dotnet publish -r osx-x64
```

The resulting native library contains the desired symbols:

```sh
% nm bin/Release/osx-x64/publish/Hello-NativeAOTFromJNI.dylib | grep ' S ' 
00000000000cb710 S _JNI_OnLoad
00000000000cb820 S _JNI_OnUnload
00000000000cb840 S _Java_com_microsoft_hello_1from_1jni_NativeAOTInit_sayHello
```

Use the `RunJavaSample` target to run Java, which will run
`System.loadLibrary("Hello-NativeAOTFromJNI")`, which will cause the
NativeAOT-generated `libHello-NativeAOTFromJNI.dylib` to be run:

```sh
% dotnet build -c Release -r osx-x64 -t:RunJavaSample  -v m --nologo --no-restore
  Hello from Java!
  Hello from .NET NativeAOT!

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:00.83

% (cd bin/Release/osx-x64/publish ; java -cp hello-from-java.jar:java-interop.jar com/microsoft/hello_from_jni/App)
Hello from Java!
Hello from .NET NativeAOT!
```

Note the use of `(cd …; java …)` so that `libHello-NativeAOTFromJNI.dylib` is
in the current working directory, so that it can be found.

[0]: https://docs.oracle.com/javase/8/docs/technotes/guides/jni/spec/jniTOC.html
[1]: https://docs.oracle.com/javase/8/docs/technotes/guides/jni/spec/invocation.html#creating_the_vm
[2]: https://docs.oracle.com/javase/8/docs/technotes/guides/jni/spec/invocation.html#JNJI_OnLoad
[3]: https://github.com/dotnet/samples/blob/main/core/nativeaot/NativeLibrary/README.md