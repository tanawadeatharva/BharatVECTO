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
using System.Collections.Generic;
using System.Data;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public class HeatingVentilationAirConditioning : LookupData<MissionType, VehicleClass, Watt>
	{
		private readonly Dictionary<Tuple<MissionType, VehicleClass>, Watt> _data =
			new Dictionary<Tuple<MissionType, VehicleClass>, Watt>();

		protected const string ResourceId = "TUGraz.VectoCore.Resources.Declaration.VAUX.HVAC-Table.csv";

		public HeatingVentilationAirConditioning()
		{
			ParseData(ReadCsvResource(ResourceId));
		}

		public override Watt Lookup(MissionType mission, VehicleClass hdvClass)
		{
			try {
				return _data[Tuple.Create(mission, hdvClass)];
			} catch (KeyNotFoundException) {
				throw new VectoException("Auxiliary Lookup Error: No value found for HVAC with mission '{0}' and HDVClass '{1}'",
					mission, hdvClass);
			}
		}

		protected override void ParseData(DataTable table)
		{
			_data.Clear();
			NormalizeTable(table);

			foreach (DataRow row in table.Rows) {
				var hdvClass = VehicleClassHelper.Parse(row.Field<string>("hdvclass/power"));
				foreach (MissionType mission in Enum.GetValues(typeof(MissionType))) {
					_data[Tuple.Create(mission, hdvClass)] = row.ParseDouble(mission.ToString().ToLower()).SI<Watt>();
				}
			}
		}
	}
}