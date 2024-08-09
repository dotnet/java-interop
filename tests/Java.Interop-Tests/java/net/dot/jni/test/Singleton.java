package net.dot.jni.test;

public final class Singleton {

	static final Singleton singleton = new Singleton ();

	private Singleton() {
	}

	public static Singleton getSingleton() {
		return singleton;
	}
}
