package com.microsoft.hello_from_jni;

class App {

    public static void main(String[] args) {
        System.out.println("Hello from Java!");
        NativeAOTInit.sayHello();
    }
}
