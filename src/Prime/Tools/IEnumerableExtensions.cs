using System;
using System.Collections.Generic;

namespace Prime.Tools
{
	internal static class IEnumerableExtensions
	{
		public static int IndexOf<T>(this IEnumerable<T> enumeration, T item)
		{
			var index = 0;
			foreach (var currentItem in enumeration)
			{
				if (ReferenceEquals(currentItem, item))
					return index;
				index++;
			}

			return -1;
		}

		public static IEnumerable<T> GetRecursive<T>(this T obj, Func<T, T> getItem)
		{
			while (obj != null)
			{
				yield return obj;

				obj = getItem(obj);
			}
		}
	}
}