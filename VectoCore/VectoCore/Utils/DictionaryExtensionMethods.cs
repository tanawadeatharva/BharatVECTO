using System;
using System.Collections.Generic;

namespace TUGraz.VectoCore.Utils
{
	internal static class DictionaryExtensionMethods
	{
		public static object GetValueOrNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,TKey key)
		{
			TValue value;
			return dictionary.TryGetValue(key, out value) ? (object)value : DBNull.Value;
		}
	}
}
