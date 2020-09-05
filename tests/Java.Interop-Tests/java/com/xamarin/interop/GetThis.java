package com.xamarin.interop;

import java.util.ArrayList;

import com.xamarin.java_interop.GCUserPeerable;

public class GetThis implements GCUserPeerable {

	static  final   String  assemblyQualifiedName   = "Java.InteropTests.GetThis, Java.Interop-Tests, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	static {
		com.xamarin.java_interop.ManagedPeer.registerNativeMembers (
				GetThis.class,
				assemblyQualifiedName,
				"");
	}

	ArrayList<Object>       managedReferences     = new ArrayList<Object>();

	public GetThis () {
		if (GetThis.class == getClass ()) {
			com.xamarin.java_interop.ManagedPeer.construct (
					this,
					"Java.InteropTests.GetThis, Java.Interop-Tests, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null",
					""
			);
		}
	}
    
	public final GetThis getThis() {
		return this;
	}

	public void jiAddManagedReference (java.lang.Object obj)
	{
		managedReferences.add (obj);
	}

	public void jiClearManagedReferences ()
	{
		managedReferences.clear ();
	}
}
