package com.microsoft.java_interop;

public class JavaInteropRuntime {
    static {
        System.loadLibrary("Hello-NativeAOTFromJNI");
    }

    private JavaInteropRuntime() {
    }

    public static native void init();
}
