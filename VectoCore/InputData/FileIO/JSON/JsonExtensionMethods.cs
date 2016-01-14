using Newtonsoft.Json.Linq;
using TUGraz.VectoCore.Exceptions;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public static class JsonExtensionMethods
	{
		public static JToken GetEx(this JObject jobject, string property)
		{
			var val = jobject[property];
			if (val == null) {
				throw new InvalidFileFormatException("Key {0} not found", property);
			}
			return val;
		}

		public static JToken GetEx(this JToken jtoken, string property)
		{
			var val = jtoken[property];
			if (val == null) {
				throw new InvalidFileFormatException("Key {0} not found", property);
			}
			return val;
		}

		public static T GetEx<T>(this JObject value, string property)
		{
			return GetEx(value, property).Value<T>();
		}

		public static T GetEx<T>(this JToken value, string property)
		{
			return GetEx(value, property).Value<T>();
		}
	}
}