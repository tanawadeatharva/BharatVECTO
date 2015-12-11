/*
* Copyright 2015 Graz University of Technology
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl5
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System;
using Newtonsoft.Json;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.FileIO
{
	/// <summary>
	/// "Header": {
	///		"CreatedBy": "Raphael Luz IVT TU-Graz (85407225-fc3f-48a8-acda-c84a05df6837)",
	///		"Date": "29.07.2015 16:59:03",
	///		"AppVersion": "2.2",
	///		"FileVersion": 7
	/// },
	/// </summary>
	public class JsonDataHeader
	{
		[JsonProperty(Required = Required.Always)] public string CreatedBy;
		[JsonProperty(Required = Required.Always), JsonConverter(typeof(DateTimeFallbackDeserializer))] public DateTime Date;
		[JsonProperty(Required = Required.Always)] public string AppVersion;
		[JsonProperty(Required = Required.Always)] public uint FileVersion;

		#region Equality members

		protected bool Equals(JsonDataHeader other)
		{
			return string.Equals(CreatedBy, other.CreatedBy) && Date.Equals(other.Date) &&
					string.Equals(AppVersion, other.AppVersion) && FileVersion.Equals(other.FileVersion);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != GetType()) {
				return false;
			}
			return Equals((JsonDataHeader)obj);
		}

		public override int GetHashCode()
		{
			unchecked {
				var hashCode = (CreatedBy != null ? CreatedBy.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ Date.GetHashCode();
				hashCode = (hashCode * 397) ^ (AppVersion != null ? AppVersion.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ FileVersion.GetHashCode();
				return hashCode;
			}
		}

		#endregion
	}
}