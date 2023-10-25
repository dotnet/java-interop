package com.microsoft.hello_from_jni;

class App {

    public static void main(String[] args) {
        System.out.println("Hello from Java!");
        String s = NativeAOTInit.sayHello();
        System.out.println("String returned to Java: " + s);
    }
}
