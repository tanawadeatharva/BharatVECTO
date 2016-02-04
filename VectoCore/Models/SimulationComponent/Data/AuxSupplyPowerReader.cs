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

using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	/// <summary>
	/// Reader for Auxiliary Supply Power. Is used by Distance, Time, and EngineOnly based Data Parser.
	/// </summary>
	public static class AuxSupplyPowerReader
	{
		private const string AuxSupplyPowerField = "Aux_";

		/// <summary>
		/// [W]. Reads Auxiliary Supply Power (defined by Fields.AuxiliarySupplyPower-Prefix).
		/// </summary>
		public static Dictionary<string, Watt> Read(DataRow row)
		{
			var auxCols = row.Table.Columns.Cast<DataColumn>().
				Where(col => col.ColumnName.StartsWith(AuxSupplyPowerField));

			return auxCols.ToDictionary(key => key.ColumnName,
				value => row.ParseDouble(value).SI().Kilo.Watt.Cast<Watt>());
		}
	}
}