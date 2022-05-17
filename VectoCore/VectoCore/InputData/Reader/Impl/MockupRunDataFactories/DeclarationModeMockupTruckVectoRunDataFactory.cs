using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;

namespace TUGraz.VectoCore.InputData.Reader.Impl.MockupRunDataFactories
{
    public class DeclarationModeMockupTruckVectoRunDataFactory : DeclarationModeTruckVectoRunDataFactory
    {
		public DeclarationModeMockupTruckVectoRunDataFactory(IDeclarationInputDataProvider dataProvider,
			IDeclarationReport report) : base(dataProvider, report, false)
		{
			if (report is IMockupReport mockupReport) {
				mockupReport.Mockup = true;
			}
		}

		#region Overrides of AbstractDeclarationVectoRunDataFactory

		protected override IDeclarationDataAdapter DataAdapter { get; }
		protected override IEnumerable<VectoRunData> GetNextRun()
		{
			var nextRun =  base.GetNextRun();


			return nextRun;
		}


		protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission,
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
		{
			VectoRunData runData;
			if (InputDataProvider.JobInputData.Vehicle.ExemptedVehicle)
			{
				runData = new VectoRunData
				{
					Exempted = true,
					Report = Report,
					Mission = new Mission() { MissionType = MissionType.ExemptedMission },
					VehicleData = CreateMockupVehicleData(vehicle),
					InputDataHash = InputDataProvider.XMLHash
				};
			} else {
				var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType, _ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));
				runData = new VectoRunData()
				{
					Loading = loading.Key,
					Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
					ExecutionMode = ExecutionMode.Declaration,
					Report = Report,
					Mission = mission,
					SimulationType = SimulationType.DistanceCycle,
					VehicleData = CreateMockupVehicleData(vehicle),
					EngineData = CreateMockupEngineData(vehicle, modeIdx),

				};
			}

			runData.InputData = InputDataProvider;

			return runData;



		}


		protected override void Initialize()
		{
			_segment = GetSegment(InputDataProvider.JobInputData.Vehicle);

		}

		#endregion



		private CombustionEngineData CreateMockupEngineData(IVehicleDeclarationInputData vehicleData, int modeIdx)
		{

			var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;
			var engineMode = engineModes[modeIdx];
			var fuels = new List<CombustionEngineFuelData>();
			foreach (var fuel in engineMode.Fuels) {
				fuels.Add(new CombustionEngineFuelData() {
					FuelData = DeclarationData.FuelData.Lookup(fuel.FuelType, vehicleData.TankSystem)
				});
			}

			return new CombustionEngineData() {
				Fuels = fuels,
			};
		}
		

		private VehicleData CreateMockupVehicleData(IVehicleDeclarationInputData vehicleData)
		{
			return new VehicleData() {
				InputData = vehicleData,
				SleeperCab = vehicleData?.SleeperCab
			};
		}
	}
}
