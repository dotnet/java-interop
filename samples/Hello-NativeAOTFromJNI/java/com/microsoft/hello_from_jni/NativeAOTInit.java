package com.microsoft.hello_from_jni;

class NativeAOTInit {
    static {
        System.loadLibrary("Hello-NativeAOTFromJNI");
    }

    public static native String sayHello();
}
