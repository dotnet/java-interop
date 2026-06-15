#nullable enable
using System;

namespace Java.Interop {

	[AttributeUsage (AttributeTargets.Constructor, AllowMultiple=false)]
	internal sealed class JavaCallableConstructorAttribute : Attribute {

		public JavaCallableConstructorAttribute ()
		{
		}

		public  string?     SuperConstructorExpression  {get; set;}
		public  string?     Signature                   {get; set;}
	}
}
