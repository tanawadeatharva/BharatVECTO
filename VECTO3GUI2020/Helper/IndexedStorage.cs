using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VECTO3GUI2020.Helper
{
	//Helper class that can be used to store values of the same type that are identified by a string
	public class IndexedStorage<T> where T : IEquatable<T>
	{
		private Dictionary<string, T> indexedStorageDictionary = new Dictionary<string, T>();
		private readonly Action<string> _valueChangedCallback;



		public IndexedStorage(Action<string> valueChangedCallback = null)
		{
			_valueChangedCallback = valueChangedCallback;
		}


		public T this[string identifier]
		{
			get
			{
				if (!indexedStorageDictionary.ContainsKey(identifier))
				{
					indexedStorageDictionary.Add(identifier, default(T));
				}
				return indexedStorageDictionary[identifier];
			}
			set
			{
				var oldValue = default(T);
				if (indexedStorageDictionary.ContainsKey(identifier))
				{
					oldValue = indexedStorageDictionary[identifier];
				}
				else
				{
					indexedStorageDictionary.Add(identifier, default(T));
				}
				indexedStorageDictionary[identifier] = value;
				if (!value.Equals(oldValue))
				{
					_valueChangedCallback?.Invoke(identifier);
				}
			}
		}
	}

}
