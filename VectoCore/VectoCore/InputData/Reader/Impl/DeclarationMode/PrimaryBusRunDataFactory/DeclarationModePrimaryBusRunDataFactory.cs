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
using TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.PrimaryBusRunDataFactory
{
	public abstract class DeclarationModePrimaryBusRunDataFactory
	{
		public abstract class PrimaryBusBase : AbstractDeclarationVectoRunDataFactory
		{
			#region Implementation of IVectoRunDataFactory

			public IPrimaryBusDeclarationDataAdapter DataAdapter { get; }
			public IDeclarationInputDataProvider DataProvider { get; }

			public IDeclarationReport Report { get; }
       
            protected PrimaryBusBase(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, false)
            {
				DataAdapter = declarationDataAdapter;
				DataProvider = dataProvider;
				Report = report;
			}

			#region Overrides of AbstractDeclarationVectoRunDataFactory

			protected override DriverData CreateDriverData(Segment segment)
			{
				return DataAdapter.CreateDriverData(segment);
			}

			#endregion

			protected override VectoRunData GetPowertrainConfigForReportInit()
			{
				return GetNextRun().First(x => x != null);
			}

			protected Segment GetSegment(IVehicleDeclarationInputData vehicle)
			{
				if (vehicle.VehicleCategory != VehicleCategory.HeavyBusPrimaryVehicle)
				{
					throw new VectoException(
						"Invalid vehicle category for bus factory! {0}", vehicle.VehicleCategory.GetCategoryName());
				}

				var segment = DeclarationData.PrimaryBusSegments.Lookup(
					vehicle.VehicleCategory, vehicle.AxleConfiguration, vehicle.Articulated);
				if (!segment.Found)
				{
					throw new VectoException(
						"no segment found for vehicle configruation: vehicle category: {0}, axle configuration: {1}, articulated: {2}, primary",
						vehicle.VehicleCategory, vehicle.AxleConfiguration,
						vehicle.Articulated);
				}

				return segment;
			}

			protected override void Initialize()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				if (vehicle.ExemptedVehicle)
				{
					return;
				}

				_segment = GetSegment(vehicle);
				
			}

			#endregion

			protected VectoRunData CreateCommonRunData(IVehicleDeclarationInputData vehicle, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				Segment segment,
				IList<IEngineModeDeclarationInputData> engineModes = null, int modeIdx = 0)
			{
				var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType, _ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));

				CheckSuperCap(vehicle);

				var simulationRunData = new VectoRunData {
					InputData = DataProvider,
					Loading = loading.Key,
					JobType = vehicle.VehicleType,
					Mission = mission,
					InputDataHash = InputDataProvider.XMLHash,
					SimulationType = SimulationType.DistanceCycle,
					Report = Report,
					JobName = InputDataProvider.JobInputData.JobName,
					ModFileSuffix = $"{(engineModes?.Count > 1 ? $"_EngineMode{modeIdx}_" : "")}" +
									$"_{mission.BusParameter.BusGroup.GetClassNumber()}_{loading.Key}",
					MaxChargingPower = vehicle.MaxChargingPower,
					VehicleDesignSpeed = segment.DesignSpeed,
					Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
					ExecutionMode = ExecutionMode.Declaration,
				};

				return simulationRunData;
			}

			/// <summary>
			/// Super caps are not allowed for ovc hevs or pevs
			/// </summary>
			protected void CheckSuperCap(IVehicleDeclarationInputData vehicle)
			{
				if (vehicle.VehicleType == VectoSimulationJobType.BatteryElectricVehicle || vehicle.OvcHev) {
					if (vehicle.Components.ElectricStorage.ElectricStorageElements.Any(e =>
							e.REESSPack.StorageType == REESSType.SuperCap)) {
						throw new VectoException("Super caps are not allowed for OVC-HEVs or PEVs");
					}
				}

				if (vehicle.Components.ElectricStorage?.ElectricStorageElements == null) {
					return;
				}

				var hasSuperCap = vehicle.Components.ElectricStorage.ElectricStorageElements.Any(e =>
					e.REESSPack.StorageType == REESSType.SuperCap);
				var hasBattery = vehicle.Components.ElectricStorage.ElectricStorageElements.Any(e =>
					e.REESSPack.StorageType == REESSType.Battery);

				if (hasSuperCap && hasBattery) {
					//Already handled by XML Schema
					throw new VectoException("Super caps AND batteries are not supported");
				}
			}

			protected abstract void CreateGearboxAndGearshiftData(IVehicleDeclarationInputData vehicle,
				VectoRunData runData);
		}

		public class Conventional : PrimaryBusBase
		{
			public Conventional(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

            #region Overrides of PrimaryBusBase

            protected override IEnumerable<VectoRunData> GetNextRun()
            {
                var vehicle = DataProvider.JobInputData.Vehicle;
                var engine = vehicle.Components.EngineInputData;
                var engineModes = engine.EngineModes;

                for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
                    foreach (var mission in _segment.Missions) {
                        foreach (var loading in mission.Loadings) {
                            var simulationRunData = CreateVectoRunData(vehicle, mission, loading, modeIdx);
                            if (simulationRunData == null) {
                                continue;
                            }
                            yield return simulationRunData;
                        }
                    }
                }
            }

            #endregion

            protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle,
				Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx,
				VectoRunData.OvcHevMode ovcMode = VectoRunData.OvcHevMode.NotApplicable)
			{
				var engine = vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;
				if (!modeIdx.HasValue) {
					throw new VectoException("Engine mode has to be specified for conventional vehicle");
				}
				var engineMode = engineModes[modeIdx.Value];

				var simulationRunData = CreateCommonRunData(vehicle, mission, loading, _segment, engineModes, modeIdx.Value);

				simulationRunData.VehicleData = DataAdapter.CreateVehicleData(vehicle, _segment, mission, loading, _allowVocational);
				simulationRunData.AirdragData = DataAdapter.CreateAirdragData(null, mission, new Segment());
				simulationRunData.EngineData = DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode, mission);
				simulationRunData.ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>();
				simulationRunData.AxleGearData = DataAdapter.CreateAxleGearData(vehicle.Components.AxleGearInputData);
                simulationRunData.AngledriveData = DataAdapter.CreateAngledriveData(vehicle.Components.AngledriveInputData);
                simulationRunData.Aux = DataAdapter.CreateAuxiliaryData(
					vehicle.Components.AuxiliaryInputData,
					vehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
					vehicle.Length ?? mission.BusParameter.VehicleLength,
					vehicle.Components.AxleWheels.NumSteeredAxles, vehicle.VehicleType);
				simulationRunData.Retarder = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData);
				simulationRunData.DriverData = DriverData;
				

                simulationRunData.EngineData.FuelMode = modeIdx.Value;
				simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
				simulationRunData.BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(
					mission, InputDataProvider.JobInputData.Vehicle, simulationRunData);
				
				CreateGearboxAndGearshiftData(vehicle, simulationRunData);
				return simulationRunData;
			}

			#region Overrides of PrimaryBusBase

			protected override void CreateGearboxAndGearshiftData(IVehicleDeclarationInputData vehicle, VectoRunData runData)
			{
				var shiftStrategyName = PowertrainBuilder.GetShiftStrategyName(vehicle.Components.GearboxInputData.Type, vehicle.VehicleType);
				runData.GearboxData = DataAdapter.CreateGearboxData(vehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));
				
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) * (runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						vehicle.EngineIdleSpeed, 
						vehicle.Components.GearboxInputData.Type, 
						vehicle.Components.GearboxInputData.Gears.Count);

			}

			#endregion
		}

		public abstract class Hybrid : PrimaryBusBase
		{
			protected Hybrid(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

            protected override IEnumerable<VectoRunData> GetNextRun()
            {
                var vehicle = DataProvider.JobInputData.Vehicle;
                var engine = vehicle.Components.EngineInputData;
                var engineModes = engine.EngineModes;

                for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
                    foreach (var mission in _segment.Missions) {
                        foreach (var loading in mission.Loadings) {

							if (vehicle.OvcHev) {
								if (vehicle.MaxChargingPower.IsEqual(0)) {
									throw new VectoException(
										"MaxChargingPower has to be greater than 0 if OVC is selected");
								}
								yield return CreateVectoRunData(vehicle, mission, loading, modeIdx, VectoRunData.OvcHevMode.ChargeDepleting);
								yield return CreateVectoRunData(vehicle, mission, loading, modeIdx, VectoRunData.OvcHevMode.ChargeSustaining);
							} else {
								yield return CreateVectoRunData(vehicle, mission, loading, modeIdx, VectoRunData.OvcHevMode.ChargeSustaining);
							}
						}
                    }
                }
            }
        }

		public abstract class SerialHybrid : Hybrid
		{
			protected SerialHybrid(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report,
				declarationDataAdapter)
			{

			}

			protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, VectoRunData.OvcHevMode ovcMode = VectoRunData.OvcHevMode.NotApplicable)
			{
				throw new NotImplementedException();
			}

			protected override void CreateGearboxAndGearshiftData(IVehicleDeclarationInputData vehicle, VectoRunData runData)
			{
				if (vehicle.ArchitectureID.IsOneOf(ArchitectureID.S2, ArchitectureID.S_IEPC)) {
					throw new ArgumentException(nameof(vehicle.ArchitectureID));
				}
				runData.GearshiftParameters = new ShiftStrategyParameters() {
					StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
					StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
				};
			}
		}


		public class HEV_S2 : SerialHybrid
		{
			public HEV_S2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			protected override void CreateGearboxAndGearshiftData(IVehicleDeclarationInputData vehicle, VectoRunData runData)
			{
				if (vehicle.ArchitectureID != ArchitectureID.E2) {
					throw new ArgumentException(nameof(vehicle));
				}

				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) * (runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						null, 
						vehicle.Components.GearboxInputData.Type,
						vehicle.Components.GearboxInputData.Gears.Count);


				var shiftStrategyName =
					PowertrainBuilder.GetShiftStrategyName(vehicle.Components.GearboxInputData.Type,
						vehicle.VehicleType);
				runData.GearboxData = DataAdapter.CreateGearboxData(vehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));

			}
		}

		public class HEV_S3 : SerialHybrid
		{
			public HEV_S3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

		}

		public class HEV_S4 : SerialHybrid
		{
			public HEV_S4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

		}

		public class HEV_S_IEPC : SerialHybrid
		{
			public HEV_S_IEPC(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			protected override void CreateGearboxAndGearshiftData(IVehicleDeclarationInputData vehicle, VectoRunData runData)
			{
				if (vehicle.ArchitectureID != ArchitectureID.E2) {
					throw new ArgumentException(nameof(vehicle));
				}

				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) * (runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						null, 
						vehicle.Components.GearboxInputData.Type,
						vehicle.Components.GearboxInputData.Gears.Count);


				var shiftStrategyName =
					PowertrainBuilder.GetShiftStrategyName(vehicle.Components.GearboxInputData.Type,
						vehicle.VehicleType);
				runData.GearboxData = DataAdapter.CreateGearboxData(vehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));

			}
		}

		public abstract class ParallelHybrid : Hybrid
		{
			protected ParallelHybrid(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, VectoRunData.OvcHevMode ovcMode = VectoRunData.OvcHevMode.NotApplicable)
			{
				var engine = vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;
				if (!modeIdx.HasValue) {
					throw new VectoException("Engine mode has to be specified for parallel hybrid vehicle");
				}
				var engineMode = engineModes[modeIdx.Value];

				var runData = CreateCommonRunData(vehicle, mission, loading, _segment, engineModes, modeIdx.Value);

				runData.VehicleData = DataAdapter.CreateVehicleData(vehicle, _segment, mission, loading, _allowVocational);
				runData.AirdragData = DataAdapter.CreateAirdragData(null, mission, new Segment());
				runData.EngineData = DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode, mission);
				DataAdapter.CreateREESSData(vehicle.Components.ElectricStorage, vehicle.VehicleType, vehicle.OvcHev,
					((batteryData) => runData.BatteryData = batteryData),
					((sCdata => runData.SuperCapData = sCdata)));
				runData.ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>();
				runData.AxleGearData = DataAdapter.CreateAxleGearData(vehicle.Components.AxleGearInputData);
				runData.AngledriveData = DataAdapter.CreateAngledriveData(vehicle.Components.AngledriveInputData);
				runData.Aux = DataAdapter.CreateAuxiliaryData(
					vehicle.Components.AuxiliaryInputData,
					vehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
					vehicle.Length ?? mission.BusParameter.VehicleLength,
					vehicle.Components.AxleWheels.NumSteeredAxles, vehicle.VehicleType);
				runData.Retarder = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData);
				runData.DriverData = DriverData;


				runData.EngineData.FuelMode = modeIdx.Value;
				runData.VehicleData.VehicleClass = _segment.VehicleClass;
				runData.BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(
					mission, InputDataProvider.JobInputData.Vehicle, runData);

				CreateGearboxAndGearshiftData(vehicle, runData);

				runData.ElectricMachinesData = DataAdapter.CreateElectricMachines(
					vehicle.Components.ElectricMachines, vehicle.ElectricMotorTorqueLimits,
					runData.BatteryData.CalculateAverageVoltage(), runData.GearboxData.GearList);

				runData.HybridStrategyParameters =
					DataAdapter.CreateHybridStrategy(runData.BatteryData,
						runData.SuperCapData,
						runData.VehicleData.TotalVehicleMass,
						ovcMode, loading.Key,
						//runData.VehicleData.VehicleClass,
						mission.BusParameter.BusGroup,
						mission.MissionType, vehicle.BoostingLimitations, runData.GearboxData, runData.EngineData, vehicle.ArchitectureID);

				if (ovcMode != VectoRunData.OvcHevMode.NotApplicable) {
					if (runData.BatteryData?.InitialSoC != null) {
						runData.BatteryData.InitialSoC = runData.HybridStrategyParameters.InitialSoc;
					}

					if (runData.SuperCapData?.InitialSoC != null) {
						runData.SuperCapData.InitialSoC = runData.HybridStrategyParameters.InitialSoc;
					}
				}

				if (ovcMode == VectoRunData.OvcHevMode.ChargeDepleting) {
					runData.BatteryData.Batteries.ForEach(b => b.Item2.ChargeSustainingBattery = true);
				}

				if (ovcMode == VectoRunData.OvcHevMode.ChargeSustaining) {
					runData.IterativeRunStrategy = new OVCHevIterativeRunStrategy();
				}
				if (ovcMode != VectoRunData.OvcHevMode.NotApplicable && runData.InputData.JobInputData.Vehicle.OvcHev) {
					runData.ModFileSuffix += ovcMode == VectoRunData.OvcHevMode.ChargeSustaining ? "CS" : "CD";
				}
				runData.OVCMode = ovcMode;
				return runData;
			}

			protected override void CreateGearboxAndGearshiftData(IVehicleDeclarationInputData vehicle, VectoRunData runData)
			{
				var shiftStrategyName = PowertrainBuilder.GetShiftStrategyName(vehicle.Components.GearboxInputData.Type, vehicle.VehicleType);
				
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) * (runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						vehicle.EngineIdleSpeed,
						vehicle.Components.GearboxInputData.Type,
						vehicle.Components.GearboxInputData.Gears.Count);
				
				runData.GearboxData = DataAdapter.CreateGearboxData(vehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));

			}

		}

		public class HEV_P1 : ParallelHybrid
		{
			public HEV_P1(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

		}

		public class HEV_P2 : ParallelHybrid
		{
			public HEV_P2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

		}

		public class HEV_P2_5 : ParallelHybrid
		{
			public HEV_P2_5(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			
		}

		public class HEV_P3 : ParallelHybrid
		{
			public HEV_P3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

		}

		public class HEV_P4 : ParallelHybrid
		{
			public HEV_P4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

		}

		public abstract class BatteryElectric : PrimaryBusBase
		{
			public BatteryElectric(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of PrimaryBusBase

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				foreach (var mission in _segment.Missions) {
					foreach (var loading in mission.Loadings) {
						var simulationRunData = CreateVectoRunData(vehicle, mission, loading);
						simulationRunData.BatteryData.Batteries.ForEach(t => t.Item2.ChargeSustainingBattery = true);
						yield return simulationRunData;
					}
				}
			}

			#endregion

			protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, VectoRunData.OvcHevMode ovcMode = VectoRunData.OvcHevMode.NotApplicable)
			{
				var result = CreateCommonRunData(vehicle, mission, loading, _segment);


				DataAdapter.CreateREESSData(
					componentsElectricStorage: vehicle.Components.ElectricStorage,
					vehicle.VehicleType,
					true,
					(bs) => result.BatteryData = bs,
					(sc) => result.SuperCapData = sc);

				
				if (vehicle.VehicleType == VectoSimulationJobType.IEPC_E) {
					result.ElectricMachinesData = DataAdapter.CreateIEPCElectricMachines(vehicle.Components.IEPC,
						result.BatteryData.CalculateAverageVoltage());
				} else {
					result.ElectricMachinesData = DataAdapter.CreateElectricMachines(vehicle.Components.ElectricMachines,
						vehicle.ElectricMotorTorqueLimits, result.BatteryData.CalculateAverageVoltage(), null);
				}

				result.VehicleData = DataAdapter.CreateVehicleData(vehicle, _segment, mission, loading, _allowVocational);
				result.AirdragData = DataAdapter.CreateAirdragData(null, mission, new Segment());
				if (AxleGearRequired() || vehicle.Components.AxleGearInputData != null) {
					result.AxleGearData = DataAdapter.CreateAxleGearData(vehicle.Components.AxleGearInputData);
				}

				result.AngledriveData = DataAdapter.CreateAngledriveData(vehicle.Components.AngledriveInputData);
				result.Aux = DataAdapter.CreateAuxiliaryData(
					vehicle.Components.AuxiliaryInputData,
					vehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
					vehicle.Length ?? mission.BusParameter.VehicleLength,
					vehicle.Components.AxleWheels.NumSteeredAxles, vehicle.VehicleType);
				result.Retarder = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData);
				result.DriverData = DriverData;

				result.VehicleData.VehicleClass = _segment.VehicleClass;
				result.BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(
					mission, InputDataProvider.JobInputData.Vehicle, result);
				CreateGearboxAndGearshiftData(vehicle, result);
				
				return result;
			}
			protected virtual bool AxleGearRequired()
			{
				return InputDataProvider.JobInputData.Vehicle.ArchitectureID != ArchitectureID.E4;
			}

			protected override void CreateGearboxAndGearshiftData(IVehicleDeclarationInputData vehicle, VectoRunData runData)
			{
				if (vehicle.ArchitectureID == ArchitectureID.E2) {
					throw new ArgumentException();
				}
				runData.GearshiftParameters = new ShiftStrategyParameters() {
					StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
					StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
				};
			}
		}


		public class PEV_E2 : BatteryElectric
		{
			public PEV_E2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of PrimaryBusBase

			protected override void CreateGearboxAndGearshiftData(IVehicleDeclarationInputData vehicle, VectoRunData runData)
			{
				if (vehicle.ArchitectureID != ArchitectureID.E2) {
					throw new ArgumentException(nameof(vehicle));
				}

				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) * (runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						null,
						vehicle.Components.GearboxInputData.Type,
						vehicle.Components.GearboxInputData.Gears.Count);


				var shiftStrategyName =
					PowertrainBuilder.GetShiftStrategyName(vehicle.Components.GearboxInputData.Type,
						vehicle.VehicleType);
				runData.GearboxData = DataAdapter.CreateGearboxData(vehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));

			}
			#endregion
		}

		public class PEV_E3 : BatteryElectric
		{
			public PEV_E3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

		}

		public class PEV_E4 : BatteryElectric
		{
			public PEV_E4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

		}

		public class PEV_E_IEPC : BatteryElectric
		{
			public PEV_E_IEPC(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }


			protected override bool AxleGearRequired()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				var iepcInput = vehicle.Components.IEPC;
				var axleGearRequired = !iepcInput.DifferentialIncluded && !iepcInput.DesignTypeWheelMotor;
				if (axleGearRequired && vehicle.Components.AxleGearInputData == null) {
					throw new VectoException(
						$"Axlegear reqhired for selected type of IEPC! DifferentialIncluded: {iepcInput.DifferentialIncluded}, DesignTypeWheelMotor: {iepcInput.DesignTypeWheelMotor}");
				}

				var numGearsPowermap =
					iepcInput.VoltageLevels.Select(x => Tuple.Create(x.VoltageLevel, x.PowerMap.Count)).ToArray();
				var gearCount = iepcInput.Gears.Count;
				var numGearsDrag = iepcInput.DragCurves.Count;

				if (numGearsPowermap.Any(x => x.Item2 != gearCount)) {
					throw new VectoException(
						$"Number of gears for voltage levels does not match! PowerMaps: {numGearsPowermap.Select(x => $"{x.Item1}: {x.Item2}").Join()}; Gear count: {gearCount}");
				}

				if (numGearsDrag > 1 && numGearsDrag != gearCount) {
					throw new VectoException(
						$"Number of gears drag curve does not match gear count! DragCurve {numGearsDrag}; Gear count: {gearCount}");
				}

				return axleGearRequired || vehicle.Components.AxleGearInputData != null;

			}

			protected override void CreateGearboxAndGearshiftData(IVehicleDeclarationInputData vehicle, VectoRunData runData)
			{
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						runData.AxleGearData?.AxleGear.Ratio ?? 1.0,
						null,
						GearboxType.APTN,
						vehicle.Components.IEPC.Gears.Count
					);
				var shiftStrategyName =
					PowertrainBuilder.GetShiftStrategyName(GearboxType.APTN,
						vehicle.VehicleType);
				runData.GearboxData = DataAdapter.CreateGearboxData(vehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));

			}
		}

		public class Exempted : PrimaryBusBase
		{
			public Exempted(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }

			#region Overrides of PrimaryBusBase

			protected override VectoRunData GetPowertrainConfigForReportInit()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				return CreateVectoRunData(vehicle, null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(), 0);
			}

			protected override void CreateGearboxAndGearshiftData(IVehicleDeclarationInputData vehicle, VectoRunData runData)
			{
				throw new NotImplementedException();
			}

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;

				var simulationRunData = CreateVectoRunData(vehicle,
					null,
					new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(),
					0);

				yield return simulationRunData;
			}

			#endregion

			protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle,
				Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx,
				VectoRunData.OvcHevMode ovcMode = VectoRunData.OvcHevMode.NotApplicable)
			{
				var runData = new VectoRunData {
					InputData = DataProvider,
					Exempted = true,
					Report = Report,
					Mission = new Mission { MissionType = MissionType.ExemptedMission },
					VehicleData = DataAdapter.CreateVehicleData(InputDataProvider.JobInputData.Vehicle, new Segment(),
						null,
						new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(LoadingType.ReferenceLoad,
							Tuple.Create<Kilogram, double?>(0.SI<Kilogram>(), null)), _allowVocational),
					InputDataHash = InputDataProvider.XMLHash
				};
				return runData;
			}


		}
	}
}