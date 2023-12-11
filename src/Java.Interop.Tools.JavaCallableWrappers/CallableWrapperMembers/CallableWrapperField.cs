using System.IO;

namespace Java.Interop.Tools.JavaCallableWrappers.CallableWrapperMembers;

class CallableWrapperField
{
	public string FieldName { get; }
	public string TypeName { get; }
	public string Visibility { get; }
	public bool IsStatic { get; }
	public string InitializerName { get; }
	public string? Annotations { get; }

	public CallableWrapperField (string fieldName, string typeName, string visibility, bool isStatic, string initializerName, string? annotations)
	{
		FieldName = fieldName;
		TypeName = typeName;
		Visibility = visibility;
		IsStatic = isStatic;
		InitializerName = initializerName;
		Annotations = annotations;
	}

	public void Generate (TextWriter sw)
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
