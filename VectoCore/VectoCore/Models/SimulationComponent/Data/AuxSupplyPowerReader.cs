/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System.Collections.Generic;
using System.Data;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	/// <summary>
	/// Reader for Auxiliary Supply Power. Is used by Distance, Time, and EngineOnly based Data Parser.
	/// </summary>
	public static class AuxSupplyPowerReader
	{
		/// <summary>
		/// Reads Auxiliary Supply Power (defined by AuxSupplyPowerField-Prefix "Aux_").
		/// </summary>
		public static Dictionary<string, Watt> GetAuxiliaries(this DataRow row)
		{
			var aux = new Dictionary<string, Watt>();
			var cols = row.Table.Columns;
			var auxLen = Constants.Auxiliaries.Prefix.Length;
			for (var i = 0; i < cols.Count; i++) {
				var c = cols[i].ColumnName.ToUpper();
				if (c.Length >= auxLen && c.Substring(0, auxLen) == Constants.Auxiliaries.Prefix) {
					aux[c.Substring(auxLen)] = (row.ParseDouble(i) * Constants.Kilo).SI<Watt>();
				}
			}
			return aux;
		}
	}
}