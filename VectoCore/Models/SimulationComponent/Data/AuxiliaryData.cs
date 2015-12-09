using System.Collections;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class AuxiliaryData
	{
		public double EfficiencyToSupply { get; set; }
		public double TransmissionRatio { get; set; }
		public double EfficiencyToEngine { get; set; }

		private readonly DelauneyMap _map = new DelauneyMap();

		public Watt GetPowerDemand(PerSecond nAuxiliary, Watt powerAuxOut)
		{
			return _map.Interpolate(nAuxiliary.Value(), powerAuxOut.Value()).SI<Watt>();
		}


		public static AuxiliaryData ReadFromFile(string fileName)
		{
			var auxData = new AuxiliaryData();

			try {
				var stream = new StreamReader(fileName);
				stream.ReadLine(); // skip header "Transmission ration to engine rpm [-]"
				auxData.TransmissionRatio = stream.ReadLine().IndulgentParse();
				stream.ReadLine(); // skip header "Efficiency to engine [-]"
				auxData.EfficiencyToEngine = stream.ReadLine().IndulgentParse();
				stream.ReadLine(); // skip header "Efficiency auxiliary to supply [-]"
				auxData.EfficiencyToSupply = stream.ReadLine().IndulgentParse();

				var m = new MemoryStream(Encoding.UTF8.GetBytes(stream.ReadToEnd()));
				var table = VectoCSVFile.ReadStream(m);

				// todo: @@@ check for valid header columns, otherwise use index
				if (HeaderIsValid(table.Columns)) {
					FillFromColumnNames(table, auxData._map);
				} else {
					FillFromColumnIndizes(table, auxData._map);
				}

				auxData._map.Triangulate();

				return auxData;
			} catch (FileNotFoundException e) {
				throw new VectoException("Auxiliary file not found: " + fileName, e);
			}
		}

		private static void FillFromColumnIndizes(DataTable table, DelauneyMap map)
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

		private static void FillFromColumnNames(DataTable table, DelauneyMap map)
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

		internal AuxiliaryData(IAuxiliaryInputData data)
		{
			TransmissionRatio = data.TransmissionRatio;
			EfficiencyToEngine = data.EfficiencyToEngine;
			EfficiencyToSupply = data.EfficiencyToSupply;
			if (HeaderIsValid(data.DemandMap.Columns)) {
				FillFromColumnNames(data.DemandMap, _map);
			} else {
				FillFromColumnIndizes(data.DemandMap, _map);
			}

			_map.Triangulate();
		}

		private AuxiliaryData() {}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.AuxSpeed) && columns.Contains(Fields.MechPower) &&
					columns.Contains(Fields.SupplyPower);
		}

		private static class Fields
		{
			public const string AuxSpeed = "Auxiliary speed";
			public const string MechPower = "Mechanical power";
			public const string SupplyPower = "Supply power";
		}
	}
}