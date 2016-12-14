using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Android.Tools.ApiXmlAdjuster
{
	public partial class JavaApi
	{
		public event Action<IJavaInfoItem> NewExtensibleCreated;

		public void OnNewExtensibleCreated (IJavaInfoItem item)
		{
			if (NewExtensibleCreated != null)
				NewExtensibleCreated (item);
		}

		Dictionary<IJavaInfoItem, IList<object>> extensions = new Dictionary<IJavaInfoItem, IList<object>> ();

		IList<object> EnsureExtensionsListFor (IJavaInfoItem item)
		{
			IList<object> l;
			if (!extensions.TryGetValue (item, out l)) {
				l = new List<object> ();
				extensions.Add (item, l);
			}
			return l;
		}

		public T GetExtension<T> (IJavaInfoItem item)
		{
			return EnsureExtensionsListFor (item).OfType<T> ().FirstOrDefault ();
		}

		public void SetExtension<T> (IJavaInfoItem item, T value)
		{
			var l = EnsureExtensionsListFor (item);
			var existing = l.OfType<T> ();
			if (existing is T)
				l.Remove (existing);
			l.Add (value);
		}
	}

	public partial interface IJavaInfoItem
	{
		JavaApi Api { get; }
	}

	public static class IJavaInfoItemExtensions
	{
		public static T GetExtension<T> (this IJavaInfoItem item)
		{
			return item.Api.GetExtension<T> (item);
		}

		public static IJavaInfoItem SetExtension<T> (this IJavaInfoItem item, T value)
		{
			item.Api.SetExtension (item, value);
			return item;
		}
	}

	public partial class JavaPackage : IJavaInfoItem
	{
		JavaApi IJavaInfoItem.Api {
			get {
				return Parent;
			}
		}

		partial void Initialize ()
		{
			((IJavaInfoItem) this).Api.OnNewExtensibleCreated (this);
		}
	}

	public abstract partial class JavaType : IJavaInfoItem
	{
		public JavaApi Api {
			get { return Parent.Parent; }
		}

		partial void Initialize ()
		{
			((IJavaInfoItem)this).Api.OnNewExtensibleCreated (this);
		}
	}

	public partial class JavaMember : IJavaInfoItem
	{
		public JavaApi Api {
			get { return Parent.Parent.Parent; }
		}

		partial void Initialize ()
		{
			((IJavaInfoItem)this).Api.OnNewExtensibleCreated (this);
		}
	}

	public class SourceIdentifier
	{
		public SourceIdentifier (string sourceUri)
		{
			SourceUri = sourceUri;
		}

		public string SourceUri { get; set; }
	}
}
