using System;
using System.Collections.ObjectModel;

namespace Java.Interop.Tools.JavaTypeSystem;

public class ApiImporterOptions
{
	public Collection<string> SupportedRegisterAttributes { get; } = ["Android.Runtime.RegisterAttribute"];
}
