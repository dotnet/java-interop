package com.microsoft.hello_from_jni;

import com.microsoft.java_interop.JavaInteropRuntime;
import example.ManagedType;

class App {

    public static void main(String[] args) {
        System.out.println("Hello from Java!");
        JavaInteropRuntime.init();
        String s = sayHello();
        System.out.println("String returned to Java: " + s);
        ManagedType mt = new ManagedType(42);
        System.out.println("mt.getString()=" + mt.getString());
    }

    static native String sayHello();
}
