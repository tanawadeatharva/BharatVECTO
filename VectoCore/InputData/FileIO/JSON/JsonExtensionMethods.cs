/*
* Copyright 2015, 2016 Graz University of Technology,
* Institute of Internal Combustion Engines and Thermodynamics,
* Institute of Technical Informatics
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

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