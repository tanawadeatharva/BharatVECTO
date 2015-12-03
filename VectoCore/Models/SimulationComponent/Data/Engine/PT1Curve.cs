/*
* Copyright 2015 European Union
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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Engine
{
	public class PT1Curve : LookupData<PerSecond, Second>
	{
		private List<KeyValuePair<PerSecond, Second>> _entries;

		public static PT1Curve ReadFromFile(string fileName)
		{
			return new PT1Curve(fileName);
		}

		protected PT1Curve(string file)
		{
			DataTable data;
			try {
				data = VectoCSVFile.Read(file);
			} catch (Exception ex) {
				throw new VectoException("ERROR while reading PT1 Curve File: " + ex.Message);
			}

			ParseData(data);
		}


		protected override void ParseData(DataTable data)
		{
			if (data.Columns.Count < 4) {
				throw new VectoException("FullLoadCurve/PT1 Data File must consist of at least 4 columns.");
			}

			if (data.Rows.Count < 2) {
				throw new VectoException(
					"FullLoadCurve/PT1 must consist of at least two lines with numeric values (below file header)");
			}

			if (HeaderIsValid(data.Columns)) {
				_entries = data.Rows.Cast<DataRow>()
					.Select(
						r =>
							new KeyValuePair<PerSecond, Second>(r.ParseDouble(Fields.EngineSpeed).RPMtoRad(),
								r.ParseDouble(Fields.PT1).SI<Second>()))
					.OrderBy(x => x.Key)
					.ToList();
			} else {
				_entries = data.Rows.Cast<DataRow>()
					.Select(
						r => new KeyValuePair<PerSecond, Second>(r.ParseDouble(0).RPMtoRad(), r.ParseDouble(3).SI<Second>()))
					.OrderBy(x => x.Key)
					.ToList();
			}
		}

		protected bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.EngineSpeed) && columns.Contains(Fields.PT1);
		}

		/// <summary>
		///     [rad/s] => [s]
		/// </summary>
		/// <param name="key">[rad/s]</param>
		/// <returns>[s]</returns>
		public override Second Lookup(PerSecond key)
		{
			var index = 1;
			if (key < _entries[0].Key) {
				Log.Error("requested rpm below minimum rpm in pt1 - extrapolating. n: {0}, rpm_min: {1}",
					key.ConvertTo().Rounds.Per.Minute, _entries[0].Key.ConvertTo().Rounds.Per.Minute);
			} else {
				index = _entries.FindIndex(x => x.Key > key);
				if (index <= 0) {
					index = (key > _entries[0].Key) ? _entries.Count - 1 : 1;
				}
			}

			var pt1 = VectoMath.Interpolate(_entries[index - 1].Key, _entries[index].Key, _entries[index - 1].Value,
				_entries[index].Value, key);
			if (pt1 < 0) {
				throw new VectoException("The calculated value must not be smaller than 0. Value: " + pt1);
			}
			return pt1;
		}

		private static class Fields
		{
			/// <summary>
			///     [rpm] engine speed
			/// </summary>
			public const string EngineSpeed = "engine speed";

			/// <summary>
			///     [s] time constant
			/// </summary>
			public const string PT1 = "PT1";
		}
	}
}