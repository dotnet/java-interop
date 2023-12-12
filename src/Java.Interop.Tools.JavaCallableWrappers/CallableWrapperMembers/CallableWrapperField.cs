using System.IO;

namespace Java.Interop.Tools.JavaCallableWrappers.CallableWrapperMembers;

class CallableWrapperField
{
	public string FieldName { get; set; }
	public string TypeName { get; set; }
	public string Visibility { get; set; }
	public bool IsStatic { get; set; }
	public string InitializerName { get; set; }
	public string? Annotations { get; set; }

	public CallableWrapperField (string fieldName, string typeName, string visibility, string initializerName)
	{
		FieldName = fieldName;
		TypeName = typeName;
		Visibility = visibility;
		InitializerName = initializerName;
	}

	public void Generate (TextWriter sw, CallableWrapperWriterOptions options)
	{
		sw.WriteLine ();

		if (Annotations is not null)
			sw.WriteLine (Annotations);

		sw.Write ("\t");
		sw.Write (Visibility);
		sw.Write (' ');

		if (IsStatic)
			sw.Write ("static ");

		sw.Write (TypeName);
		sw.Write (' ');

		sw.Write (FieldName);
		sw.Write (" = ");

		sw.Write (InitializerName);
		sw.WriteLine (" ();");
	}
}
