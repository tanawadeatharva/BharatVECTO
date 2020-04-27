using System;
using System.Collections.Generic;
using System.IO;
using Castle.Core.Internal;
using Newtonsoft.Json;

namespace VECTO3GUI.Helper
{
	public static class SerializeHelper
	{
		public static void SerializeToFile<T>(string filePath, T file)
		{
			if (filePath.IsNullOrEmpty() || file == null)
				return;

			var serializedObject = JsonConvert.SerializeObject(file, Formatting.Indented);
			File.WriteAllText(filePath, serializedObject);
		}

		public static T DeserializeToObject<T>(string filePath)
		{
			if (filePath.IsNullOrEmpty())
				return default(T);

			using (var file = File.OpenText(filePath)) {
				var serializer = new JsonSerializer();
				var result = serializer.Deserialize(file,typeof(T));
				if (result == null)
					return default(T);

				return (T)result;
			}
		}

	}
}
