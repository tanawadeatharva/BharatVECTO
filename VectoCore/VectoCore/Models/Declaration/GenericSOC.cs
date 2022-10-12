using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{

	public class GenericSOC : LookupData<VectoSimulationJobType, bool, GenericSOC.GenericSOCData>
    {
		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".GenericSOC.csv";
		protected override string ErrorMessage => "GenericSOC Lookup Error: no value found. Key: '{0}'";

		private double _factor = 1d/100;
		public struct GenericSOCData
		{
			public double SOCMin;
			public double SOCMax;
			public double GenericDetorioration;
		}

		public GenericSOC()
		{
		}

		#region Overrides of LookupData

		#region Overrides of LookupData<VectoSimulationJobType,bool,GenericSOCData>
		
		public override GenericSOCData Lookup(VectoSimulationJobType jobType, bool ovc)
		{
			return base.Lookup(jobType, ovc);
		}

		#endregion


		protected override void ParseData(DataTable table)
		{
			foreach (DataRow row in table.Rows) {
				var architecture = row["architecture"];
				switch (architecture) {
					case "PEV":
						CreatePEVEntries(row);
						break;
					case "HEV":
						CreateHEVEntries(row);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
		#endregion


		private void CreatePEVEntries(DataRow row)
		{
			var architectures = new VectoSimulationJobType[] {
				VectoSimulationJobType.BatteryElectricVehicle,
				VectoSimulationJobType.IEPC_E
			};
			var val = GenericSocData(row);
			foreach (var archs in architectures) {
				Data.Add(new Tuple<VectoSimulationJobType, bool>(archs, row.ParseBoolean("ovc")), val);
			}
			
		}

		private GenericSOCData GenericSocData(DataRow row)
		{
			var val = new GenericSOCData() {
				SOCMin = row.ParseDouble("genericsocmin") * _factor,
				SOCMax = row.ParseDouble("genericsocmax") * _factor,
				GenericDetorioration = row.ParseDouble("genericdeterioration") * _factor,
			};
			return val;
		}

		private void CreateHEVEntries(DataRow row)
		{
			var val = GenericSocData(row);
			var architectures = new VectoSimulationJobType[] {
				VectoSimulationJobType.ParallelHybridVehicle,
				VectoSimulationJobType.SerialHybridVehicle,
				VectoSimulationJobType.IHPC,
				VectoSimulationJobType.IEPC_S
			};
			foreach (var archs in architectures)
			{
				Data.Add(new Tuple<VectoSimulationJobType, bool>(archs, row.ParseBoolean("ovc")), val);
			}
		}


	}
}
