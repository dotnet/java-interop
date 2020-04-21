using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.SourceWriter
{
	public class ClassWriter : TypeWriter
	{
		public List<ConstructorWriter> Constructors { get; } = new List<ConstructorWriter> ();

		public override void WriteConstructors (CodeWriter writer)
		{
			foreach (var ctor in Constructors) {
				ctor.Write (writer);
				writer.WriteLine ();
			}
		}
	}
}
