using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
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
		public abstract class LorryBase : AbstractDeclarationVectoRunDataFactory
		{
			public ILorryDeclarationDataAdapter DataAdapter { get; }
			public IDeclarationInputDataProvider InputDataProvider { get; }
			public IDeclarationReport Report { get; }

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
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, false)
			{
				DataAdapter = declarationDataAdapter;
				InputDataProvider = dataProvider;
				Report = report;
			}
			/// <summary>
			/// Sets <see cref="VectoRunData.Loading"/>
			///  ,<see cref="VectoRunData.JobName"/>
			///  ,<see cref="VectoRunData.JobType"/>
			///  ,<see cref="VectoRunData.Mission"/>
			///  ,<see cref="VectoRunData.Report"/>
			///  ,<see cref="VectoRunData.ExecutionMode"/>
			///  ,<see cref="VectoRunData.Cycle"/>
			///  ,<see cref="VectoRunData.SimulationType"/>
			///  ,<see cref="VectoRunData.InputData"/>
			///  ,<see cref="VectoRunData.ModFileSuffix"/>
			///  ,<see cref="VectoRunData.VehicleDesignSpeed"/>
			///  ,<see cref="VectoRunData.InputDataHash"/>
			/// </summary>
			/// <param name="vehicle"></param>
			/// <param name="modeIdx"></param>
			/// <param name="mission"></param>
			/// <param name="loading"></param>
			/// <param name="engineModes"></param>
			/// <param name="segment"></param>
			/// <returns></returns>
			protected VectoRunData CreateCommonRunData(IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				IList<IEngineModeDeclarationInputData> engineModes,
				Segment segment)
			{
				var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType,
					_ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));
				var simulationRunData = new VectoRunData
				{
					Loading = loading.Key,
					JobName = InputDataProvider.JobInputData.JobName,
					JobType = vehicle.VehicleType,
					Mission = mission,
					Report = Report,
					ExecutionMode = ExecutionMode.Declaration,
					Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
					SimulationType = SimulationType.DistanceCycle,
					InputData = InputDataProvider,
					ModFileSuffix = (engineModes.Count > 1 ? $"_EngineMode{modeIdx}_" : "") + loading.Key,
					VehicleDesignSpeed = segment.DesignSpeed,
					InputDataHash = InputDataProvider.XMLHash,
				};
				return simulationRunData;
			}


			protected override void Initialize()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				if (vehicle.ExemptedVehicle)
				{
					return;
				}

				_segment = GetSegment(vehicle);
				_driverdata = DataAdapter.CreateDriverData();
				_driverdata.AccelerationCurve = AccelerationCurveReader.ReadFromStream(_segment.AccelerationFile);
				var tempVehicle = DataAdapter.CreateVehicleData(vehicle, _segment, _segment.Missions.First(),
														_segment.Missions.First().Loadings.First(), _allowVocational);
				_airdragData = DataAdapter.CreateAirdragData(vehicle.Components.AirdragInputData,
													_segment.Missions.First(), _segment);
				if (InputDataProvider.JobInputData.Vehicle.AxleConfiguration.AxlegearIncludedInGearbox())
				{
					_axlegearData = DataAdapter.CreateDummyAxleGearData(InputDataProvider.JobInputData.Vehicle.Components.GearboxInputData);
				}
				else
				{
					_axlegearData = DataAdapter.CreateAxleGearData(InputDataProvider.JobInputData.Vehicle.Components.AxleGearInputData);
				}
				_angledriveData = DataAdapter.CreateAngledriveData(InputDataProvider.JobInputData.Vehicle.Components.AngledriveInputData);
				var tmpRunData = new VectoRunData()
				{
					GearboxData = new GearboxData()
					{
						Type = vehicle.Components.GearboxInputData.Type,
					}
				};
				var tmpStrategy = PowertrainBuilder.GetShiftStrategy(new SimplePowertrainContainer(tmpRunData));
				var tmpEngine = DataAdapter.CreateEngineData(
					vehicle, vehicle.Components.EngineInputData.EngineModes[0], _segment.Missions.First());
				_gearboxData = DataAdapter.CreateGearboxData(
					vehicle, new VectoRunData() { EngineData = tmpEngine, AxleGearData = _axlegearData, VehicleData = tempVehicle },
					tmpStrategy);

				_retarderData = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData);

				_ptoTransmissionData = DataAdapter.CreatePTOTransmissionData(vehicle.Components.PTOTransmissionInputData);

				_municipalPtoTransmissionData = PTODataAdapterLorry.DefaultPTOData();
				_gearshiftData = DataAdapter.CreateGearshiftData(
					_gearboxData, _axlegearData.AxleGear.Ratio * (_angledriveData?.Angledrive.Ratio ?? 1.0), tmpEngine.IdleSpeed);

			}
			#region Implementation of IVectoRunDataFactory

			public IEnumerable<VectoRunData> NextRun()
			{
				Initialize();
				if (Report != null)
				{
					InitializeReport();
				}

				return GetNextRun();
			}

			protected Segment GetSegment(IVehicleDeclarationInputData vehicle)
			{
				_allowVocational = true;
				Segment segment;
				try
				{
					segment = DeclarationData.TruckSegments.Lookup(
						vehicle.VehicleCategory, vehicle.AxleConfiguration, vehicle.GrossVehicleMassRating,
						vehicle.CurbMassChassis,
						vehicle.VocationalVehicle);
				}
				catch (VectoException)
				{
					_allowVocational = false;
					segment = DeclarationData.TruckSegments.Lookup(
						vehicle.VehicleCategory, vehicle.AxleConfiguration, vehicle.GrossVehicleMassRating,
						vehicle.CurbMassChassis,
						false);
				}

				if (!segment.Found)
				{
					throw new VectoException(
						"no segment found for vehicle configuration: vehicle category: {0}, axle configuration: {1}, GVMR: {2}",
						vehicle.VehicleCategory, vehicle.AxleConfiguration,
						vehicle.GrossVehicleMassRating);
				}
				return segment;
			}


			#endregion
		}

		public sealed class Conventional : LorryBase
		{
			public Conventional(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

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
							var simulationRunData = CreateVectoRunData(vehicle, modeIdx, mission, loading);
							yield return simulationRunData;
						}
					}
				}
			}

			


			protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
			{
				var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;
				var engineMode = engineModes[modeIdx];

				var simulationRunData = CreateCommonRunData(vehicle, modeIdx, mission, loading, engineModes, _segment);



				simulationRunData.VehicleData =
				DataAdapter.CreateVehicleData(vehicle, _segment, mission, loading, _allowVocational);

				simulationRunData.AirdragData =
					DataAdapter.CreateAirdragData(vehicle.Components.AirdragInputData, mission, _segment);

				simulationRunData.EngineData =
					DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode,
						mission); // _engineData.Copy(), // a copy is necessary because every run has a different correction factor!

				simulationRunData.ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>();
				simulationRunData.GearboxData = _gearboxData;
				simulationRunData.AxleGearData = _axlegearData;
				simulationRunData.AngledriveData = _angledriveData;
				simulationRunData.Aux = DataAdapter.CreateAuxiliaryData(
					vehicle.Components.AuxiliaryInputData,
					vehicle.Components.BusAuxiliaries, mission.MissionType,
					_segment.VehicleClass, vehicle.Length,
					vehicle.Components.AxleWheels.NumSteeredAxles);

				simulationRunData.Retarder = _retarderData;
				simulationRunData.DriverData = _driverdata;
				simulationRunData.PTO = mission.MissionType == MissionType.MunicipalUtility
					? _municipalPtoTransmissionData
					: _ptoTransmissionData;
				


				simulationRunData.GearshiftParameters = _gearshiftData;
				
			
				simulationRunData.EngineData.FuelMode = modeIdx;
				simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
				simulationRunData.VehicleData.InputData = vehicle;
				return simulationRunData;
			}

			
			#endregion
		}



		public abstract class BatteryElectric : LorryBase
		{
			public BatteryElectric(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report,
				declarationDataAdapter)
			{

			}
			#region Overrides of AbstractDeclarationVectoRunDataFactory

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				throw new NotImplementedException();
			}
			protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		public class PEV_E2 : BatteryElectric
		{
			public PEV_E2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class PEV_E3 : BatteryElectric
		{
			public PEV_E3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class PEV_E4 : BatteryElectric
		{
			public PEV_E4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class PEV_E_IEPC : BatteryElectric
		{
			public PEV_E_IEPC(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class Exempted : LorryBase
		{
			public Exempted(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of AbstractDeclarationVectoRunDataFactory

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				throw new NotImplementedException();
			}

			protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
			{
				throw new NotImplementedException();
			}

			#endregion
		}
		
	}
}