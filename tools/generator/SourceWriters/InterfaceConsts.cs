using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoDroid.Generation;
using Xamarin.Android.Binder;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	public class InterfaceConsts : ClassWriter
	{
		public InterfaceConsts ()
		{
			Name = "InterfaceConsts";

			IsPublic = true;
			IsStatic = true;
		}
	}
}
