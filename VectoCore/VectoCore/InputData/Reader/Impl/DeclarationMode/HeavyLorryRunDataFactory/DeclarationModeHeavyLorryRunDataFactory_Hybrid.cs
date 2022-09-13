using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.HeavyLorryRunDataFactory
{
	public abstract partial class DeclarationModeHeavyLorryRunDataFactory
	{
		public abstract class Hybrid : LorryBase
		{
			public Hybrid(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report,
				declarationDataAdapter)
			{

			}

			#region Overrides of AbstractDeclarationVectoRunDataFactory

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;

				var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;

				for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++)
				{
					foreach (var mission in _segment.Missions)
					{
						if (mission.MissionType.IsEMS() &&
							engine.RatedPowerDeclared.IsSmaller(DeclarationData.MinEnginePowerForEMS))
						{
							continue;
						}

						foreach (var loading in mission.Loadings)
						{
							var simulationRunData = CreateVectoRunData(vehicle, modeIdx, mission, loading);
							yield return simulationRunData;
						}
					}
				}
			}

			protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		public class HEV_S2 : Hybrid
		{
			public HEV_S2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report,
				declarationDataAdapter)
			{

			}

			#region Overrides of Hybrid

			protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
			{

				var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;
				var engineMode = engineModes[modeIdx];
				var runData = CreateCommonRunData(vehicle, modeIdx, mission, loading, engineModes, _segment);
				runData.VehicleData = DataAdapter.CreateVehicleData(vehicle, _segment, mission, loading, _allowVocational);
				runData.EngineData = DataAdapter.CreateEngineData(vehicle, engineMode, mission);
				//TODO HM use updated value for InitialSOC
				runData.BatteryData = DataAdapter.CreateBatteryData(vehicle.Components.ElectricStorage);
				runData.SuperCapData = DataAdapter.CreateSuperCapData(vehicle.Components.ElectricStorage);
				runData.ElectricMachinesData = DataAdapter.CreateElectricMachines(vehicle.Components.ElectricMachines, vehicle.ElectricMotorTorqueLimits, runData.BatteryData.CalculateAverageVoltage());
				runData.GearboxData = _gearboxData;
				runData.AirdragData = _airdragData;
				runData.AngledriveData = _angledriveData;
				runData.HybridStrategyParameters =
					DataAdapter.CreateHybridStrategy(runData.BatteryData, runData.SuperCapData);
				runData.Aux = DataAdapter.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData,
					vehicle.Components.BusAuxiliaries,
					mission.MissionType,
					_segment.VehicleClass, 
					vehicle.Length,
					vehicle.Components.AxleWheels.NumSteeredAxles);
				



				return runData;
			}

			#endregion
		}

		public class HEV_S3 : Hybrid
		{
			public HEV_S3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_S4 : Hybrid
		{
			public HEV_S4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_S_IEPC : Hybrid
		{
			public HEV_S_IEPC(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_P1 : Hybrid
		{
			public HEV_P1(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_P2 : Hybrid
		{
			public HEV_P2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_P2_5 : Hybrid
		{
			public HEV_P2_5(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_P3 : Hybrid
		{
			public HEV_P3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_P4 : Hybrid
		{
			public HEV_P4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}
	}
}