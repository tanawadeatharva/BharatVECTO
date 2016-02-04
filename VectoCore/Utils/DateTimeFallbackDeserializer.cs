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

using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TUGraz.VectoCore.Utils
{
	/// <summary>
	/// Accepts following date formats:
	/// * German: d.M.yyyy HH:mm:ss
	/// * English: M/d/yyyy HH:mm:ss tt
	/// * ISO8601: yyyy-MM-DDTHH:mm:ssZ
	/// * Local culture format (based on current localization of the user)
	/// Output is in ISO8601 format.
	/// </summary>
	public class DateTimeFallbackDeserializer : IsoDateTimeConverter
	{
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			DateTime dateTime;
			if (reader.TokenType == JsonToken.Date) {
				return reader.Value;
			}
			if (DateTime.TryParseExact((string)reader.Value, new[] { "d.M.yyyy HH:mm:ss", "M/d/yyyy HH:mm:ss tt" },
				CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime)) {
				return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
			}

			if (DateTime.TryParse((string)reader.Value, out dateTime)) {
				return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
			}
			return base.ReadJson(reader, objectType, existingValue, serializer);
		}
	}
}