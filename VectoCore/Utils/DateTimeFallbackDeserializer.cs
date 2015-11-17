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