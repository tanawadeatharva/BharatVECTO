using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

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

	public abstract partial class DeclarationModeHeavyLorryRunDataFactory
	{
		public class HEV_S3 : LorryBase
		{
			public HEV_S3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}
	}

	public abstract partial class DeclarationModeHeavyLorryRunDataFactory
	{
		public class HEV_S2 : LorryBase
		{
			public HEV_S2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

	}

	/// <summary>
	/// This class is just used to improve the readability
	/// </summary>
	public abstract partial class DeclarationModeHeavyLorryRunDataFactory
	{
		public abstract class LorryBase : IVectoRunDataFactory
		{
			public IDeclarationDataAdapter DataAdapter { get; }
			public IDeclarationInputDataProvider InputDataProvider { get; }
			public IDeclarationReport Report { get; }

			protected Segment _segment;
			protected DriverData _driverdata;
			protected AirdragData _airdragData;
			protected AxleGearData _axlegearData;
			protected AngledriveData _angledriveData;
			protected GearboxData _gearboxData;
			protected RetarderData _retarderData;
			protected PTOData _ptoTransmissionData;
			protected PTOData _municipalPtoTransmissionData;
			protected ShiftStrategyParameters _gearshiftData;
			private bool _allowVocational;


			protected LorryBase(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter)
			{
				DataAdapter = declarationDataAdapter;
				InputDataProvider = dataProvider;
				Report = report;
			}


			#region Implementation of IVectoRunDataFactory

			public IEnumerable<VectoRunData> NextRun()
			{
				Initialize();
				if (Report != null) {
					InitializeReport();
				}

				return GetNextRun();
			}

			public Segment GetSegment(IVehicleDeclarationInputData vehicle, out bool allowVocational)
			{
				allowVocational = true;
				Segment segment;
				try {
					segment = DeclarationData.TruckSegments.Lookup(
						vehicle.VehicleCategory, vehicle.AxleConfiguration, vehicle.GrossVehicleMassRating,
						vehicle.CurbMassChassis,
						vehicle.VocationalVehicle);
				} catch (VectoException) {
					allowVocational = false;
					segment = DeclarationData.TruckSegments.Lookup(
						vehicle.VehicleCategory, vehicle.AxleConfiguration, vehicle.GrossVehicleMassRating,
						vehicle.CurbMassChassis,
						false);
				}

				if (!segment.Found) {
					throw new VectoException(
						"no segment found for vehicle configuration: vehicle category: {0}, axle configuration: {1}, GVMR: {2}",
						vehicle.VehicleCategory, vehicle.AxleConfiguration,
						vehicle.GrossVehicleMassRating);
				}

				return segment;
			}

			public void Initialize()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				_segment = GetSegment(vehicle, out var allowVocational);
				_allowVocational = allowVocational;
				_driverdata = DataAdapter.CreateDriverData();
				_driverdata.AccelerationCurve = AccelerationCurveReader.ReadFromStream(_segment.AccelerationFile);
				var tempVehicle = DataAdapter.CreateVehicleData(vehicle, _segment, _segment.Missions.First(),
					_segment.Missions.First().Loadings.First(), allowVocational);
				_airdragData = DataAdapter.CreateAirdragData(vehicle.Components.AirdragInputData,
					_segment.Missions.First(), _segment);
				if (InputDataProvider.JobInputData.Vehicle.AxleConfiguration.AxlegearIncludedInGearbox()) {
					_axlegearData =
						DataAdapter.CreateDummyAxleGearData(InputDataProvider.JobInputData.Vehicle.Components
							.GearboxInputData);
				} else {
					_axlegearData =
						DataAdapter.CreateAxleGearData(InputDataProvider.JobInputData.Vehicle.Components
							.AxleGearInputData);
				}

				_angledriveData =
					DataAdapter.CreateAngledriveData(InputDataProvider.JobInputData.Vehicle.Components
						.AngledriveInputData);
				var tmpRunData = new VectoRunData() {
					GearboxData = new GearboxData() {
						Type = vehicle.Components.GearboxInputData.Type,
					}
				};
				var tmpStrategy = PowertrainBuilder.GetShiftStrategy(new SimplePowertrainContainer(tmpRunData));
				var tmpEngine = DataAdapter.CreateEngineData(
					vehicle, vehicle.Components.EngineInputData.EngineModes[0], _segment.Missions.First());
				_gearboxData = DataAdapter.CreateGearboxData(
					vehicle,
					new VectoRunData()
						{ EngineData = tmpEngine, AxleGearData = _axlegearData, VehicleData = tempVehicle },
					tmpStrategy);

				_retarderData = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData);

				_ptoTransmissionData =
					DataAdapter.CreatePTOTransmissionData(vehicle.Components.PTOTransmissionInputData);

				_municipalPtoTransmissionData = CreateDefaultPTOData();
				_gearshiftData = DataAdapter.CreateGearshiftData(
					_gearboxData, _axlegearData.AxleGear.Ratio * (_angledriveData?.Angledrive.Ratio ?? 1.0),
					tmpEngine.IdleSpeed);
			}


			protected PTOData CreateDefaultPTOData()
			{
				return new PTOData() {
					TransmissionType = DeclarationData.PTO.DefaultPTOTechnology,
					LossMap = PTOIdleLossMapReader.ReadFromStream(
						RessourceHelper.ReadStream(DeclarationData.PTO.DefaultPTOIdleLosses)),
					PTOCycle =
						DrivingCycleDataReader.ReadFromStream(
							RessourceHelper.ReadStream(DeclarationData.PTO.DefaultPTOActivationCycle),
							CycleType.PTO, "PTO", false)
				};
			}

			protected virtual VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, int modeIdx,
				Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				throw new NotImplementedException();
			}

			public virtual void InitializeReport()
			{
				VectoRunData powertrainConfig;
				List<List<FuelData.Entry>> fuels;
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				if (vehicle.ExemptedVehicle) {
					powertrainConfig = CreateVectoRunData(vehicle, 0, null,
						new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(), _allowVocational);
					fuels = new List<List<FuelData.Entry>>();
				} else {
					powertrainConfig = _segment.Missions.Select(
							mission => CreateVectoRunData(
								vehicle, 0, mission, mission.Loadings.First(), _allowVocational))
						.FirstOrDefault(x => x != null);
					fuels = vehicle.Components.EngineInputData.EngineModes.Select(x =>
							x.Fuels.Select(f => DeclarationData.FuelData.Lookup(f.FuelType, vehicle.TankSystem))
								.ToList())
						.ToList();
				}

				Report.InitializeReport(powertrainConfig, fuels);
			}

			protected virtual IEnumerable<VectoRunData> GetNextRun()
			{
				throw new NotImplementedException();
			}

			#endregion
		}
	}
}