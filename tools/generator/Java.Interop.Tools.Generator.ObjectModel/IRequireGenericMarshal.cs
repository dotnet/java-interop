using System;
using System.Collections.Generic;

namespace MonoDroid.Generation
{
	internal interface IRequireGenericMarshal
	{
		bool MayHaveManagedGenericArguments { get; }
		string GetGenericJavaObjectTypeOverride ();
		string ToInteroperableJavaObject (string varname);
	}
}

