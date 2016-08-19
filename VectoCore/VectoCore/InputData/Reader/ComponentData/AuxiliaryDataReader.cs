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
	public sealed class AuxiliaryDataReader
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
			/// <summary>
			/// [1/min]
			/// </summary>
			public const string AuxSpeed = "Auxiliary speed";

			/// <summary>
			/// [kW]
			/// </summary>
			public const string MechPower = "Mechanical power";

			/// <summary>
			/// [kW]
			/// </summary>
			public const string SupplyPower = "Supply power";
		}
	}
}