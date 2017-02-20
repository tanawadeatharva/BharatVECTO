/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
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

using System.Data;
using System.IO;
using System.Text;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	/// <summary>
	/// Reads the auxiliary demand map.
	/// </summary>
	public static class AuxiliaryDataReader
	{
		/// <summary>
		/// Factory method.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static AuxiliaryData Create(IAuxiliaryEngineeringInputData data)
		{
			var map = ReadAuxMap(data.ID, data.DemandMap);
			return new AuxiliaryData(data.ID, data.TransmissionRatio, data.EfficiencyToEngine, data.EfficiencyToSupply, map);
		}

		private static DelaunayMap ReadAuxMap(string id, DataTable table)
		{
			var map = new DelaunayMap(id);
			if (HeaderIsValid(table.Columns)) {
				FillFromColumnNames(table, map);
			} else {
				FillFromColumnIndizes(table, map);
			}

			map.Triangulate();
			return map;
		}

		private static void FillFromColumnIndizes(DataTable table, DelaunayMap map)
		{
			for (var i = 0; i < table.Rows.Count; i++) {
				var row = table.Rows[i];
				map.AddPoint(row.ParseDouble(0).RPMtoRad().Value(), row.ParseDouble(2), row.ParseDouble(1));
			}
		}

		private static void FillFromColumnNames(DataTable table, DelaunayMap map)
		{
			for (var i = 0; i < table.Rows.Count; i++) {
				var row = table.Rows[i];
				map.AddPoint(row.ParseDouble(Fields.AuxSpeed).RPMtoRad().Value(), row.ParseDouble(Fields.SupplyPower),
					row.ParseDouble(Fields.MechPower));
			}
		}

		public static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.AuxSpeed) && columns.Contains(Fields.MechPower) &&
					columns.Contains(Fields.SupplyPower);
		}

		internal static class Fields
		{
			/// <summary>[1/min]</summary>
			public const string AuxSpeed = "Auxiliary speed";

			/// <summary>[kW]</summary>
			public const string MechPower = "Mechanical power";

			/// <summary>[kW]</summary>
			public const string SupplyPower = "Supply power";
		}
	}
}