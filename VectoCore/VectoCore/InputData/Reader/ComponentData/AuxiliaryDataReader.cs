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
using System.Linq;
using System.Text;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class AuxiliaryDataReader
	{
		public static AuxiliaryData Create(IAuxiliaryEngineeringInputData data)
		{
			var map = ReadAuxMap(data.ID, data.DemandMap);
			return new AuxiliaryData(data.TransmissionRatio, data.EfficiencyToEngine, data.EfficiencyToSupply, map);
		}

		public static AuxiliaryData ReadFromFile(string fileName, string id)
		{
			try {
				var stream = new StreamReader(fileName);
				stream.ReadLine(); // skip header "Transmission ration to engine rpm [-]"
				var transmissionRatio = stream.ReadLine().IndulgentParse();
				stream.ReadLine(); // skip header "Efficiency to engine [-]"
				var efficiencyToEngine = stream.ReadLine().IndulgentParse();
				stream.ReadLine(); // skip header "Efficiency auxiliary to supply [-]"
				var efficiencyToSupply = stream.ReadLine().IndulgentParse();

				var m = new MemoryStream(Encoding.UTF8.GetBytes(stream.ReadToEnd()));
				var table = VectoCSVFile.ReadStream(m);
				var map = ReadAuxMap(id, table);

				return new AuxiliaryData(transmissionRatio, efficiencyToEngine, efficiencyToSupply, map);
			} catch (FileNotFoundException e) {
				throw new VectoException("Auxiliary file not found: " + fileName, e);
			}
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
			var data = table.Rows.Cast<DataRow>().Select(row => new {
				AuxiliarySpeed = row.ParseDouble(0).RPMtoRad(),
				MechanicalPower = row.ParseDouble(1).SI().Kilo.Watt.Cast<Watt>(),
				SupplyPower = row.ParseDouble(2).SI().Kilo.Watt.Cast<Watt>()
			});
			foreach (var d in data) {
				map.AddPoint(d.AuxiliarySpeed.Value(), d.SupplyPower.Value(), d.MechanicalPower.Value());
			}
		}

		private static void FillFromColumnNames(DataTable table, DelaunayMap map)
		{
			var data = table.Rows.Cast<DataRow>().Select(row => new {
				AuxiliarySpeed = row.ParseDouble(Fields.AuxSpeed).RPMtoRad(),
				MechanicalPower = row.ParseDouble(Fields.MechPower).SI().Kilo.Watt.Cast<Watt>(),
				SupplyPower = row.ParseDouble(Fields.SupplyPower).SI().Kilo.Watt.Cast<Watt>()
			});
			foreach (var d in data) {
				map.AddPoint(d.AuxiliarySpeed.Value(), d.SupplyPower.Value(), d.MechanicalPower.Value());
			}
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.AuxSpeed) && columns.Contains(Fields.MechPower) &&
					columns.Contains(Fields.SupplyPower);
		}

		public static class Fields
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