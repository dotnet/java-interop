using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.SourceWriter
{
	[Flags]
	public enum Visibility
	{
		Private = 0b000,
		Public = 0b001,
		Protected = 0b010,
		Internal = 0b100
	}
}
