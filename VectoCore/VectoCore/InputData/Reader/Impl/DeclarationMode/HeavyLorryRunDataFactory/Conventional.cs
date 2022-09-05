using System;
using System.Collections.Generic;
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

namespace TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.HeavyLorryRunDataFactory
{
	public abstract partial class DeclarationModeHeavyLorryRunDataFactory
	{
		
		public sealed class Conventional : LorryBase
		{
			public Conventional(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of LorryBase

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;

				var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;

				for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
					foreach (var mission in _segment.Missions) {
						if (mission.MissionType.IsEMS() &&
							engine.RatedPowerDeclared.IsSmaller(DeclarationData.MinEnginePowerForEMS)) {
							continue;
						}

						foreach (var loading in mission.Loadings) {
							var simulationRunData = CreateVectoRunData(vehicle, modeIdx, mission, loading, true);
							yield return simulationRunData;
						}
					}
				}
			}

			protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;
				var engineMode = engineModes[modeIdx];

				var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType,
					_ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));

				var simulationRunData = new VectoRunData {
					Loading = loading.Key,
					VehicleData = DataAdapter.CreateVehicleData(vehicle, _segment, mission, loading, allowVocational),
					VehicleDesignSpeed = _segment.DesignSpeed,
					AirdragData = DataAdapter.CreateAirdragData(vehicle.Components.AirdragInputData, mission, _segment),
					EngineData =
						DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode,
							mission), // _engineData.Copy(), // a copy is necessary because every run has a different correction factor!
					ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
					GearboxData = _gearboxData,
					AxleGearData = _axlegearData,
					AngledriveData = _angledriveData,
					Aux = DataAdapter.CreateAuxiliaryData(
						vehicle.Components.AuxiliaryInputData,
						vehicle.Components.BusAuxiliaries, mission.MissionType,
						_segment.VehicleClass, vehicle.Length,
						vehicle.Components.AxleWheels.NumSteeredAxles),
					Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
					Retarder = _retarderData,
					DriverData = _driverdata,
					ExecutionMode = ExecutionMode.Declaration,
					JobName = InputDataProvider.JobInputData.JobName,
					ModFileSuffix = (engineModes.Count > 1 ? $"_EngineMode{modeIdx}_" : "") + loading.Key,
					Report = Report,
					Mission = mission,
					PTO = mission.MissionType == MissionType.MunicipalUtility
						? _municipalPtoTransmissionData
						: _ptoTransmissionData,
					InputDataHash = InputDataProvider.XMLHash,
					SimulationType = SimulationType.DistanceCycle,
					GearshiftParameters = _gearshiftData,
					InputData = InputDataProvider
				};
				simulationRunData.EngineData.FuelMode = modeIdx;
				simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
				simulationRunData.VehicleData.InputData = vehicle;
				return simulationRunData;
			}
			#endregion
		}

		public class HEV_S4 : LorryBase
		{
			public HEV_S4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_S_IEPC : LorryBase
		{
			public HEV_S_IEPC(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_P1 : LorryBase
		{
			public HEV_P1(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_P2 : LorryBase
		{
			public HEV_P2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_P2_5 : LorryBase
		{
			public HEV_P2_5(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_P3 : LorryBase
		{
			public HEV_P3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_P4 : LorryBase
		{
			public HEV_P4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class PEV_E2 : LorryBase
		{
			public PEV_E2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class PEV_E3 : LorryBase
		{
			public PEV_E3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class PEV_E4 : LorryBase
		{
			public PEV_E4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class PEV_E_IEPC : LorryBase
		{
			public PEV_E_IEPC(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class Exempted : LorryBase
		{
			public Exempted(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}
	}
}