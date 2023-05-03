using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.HeavyLorryRunDataFactory;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.SingleBus
{

	public abstract class DeclarationModeSingleBusRunDataFactory
	{
		public abstract class SingleBusBase : AbstractDeclarationVectoRunDataFactory
        {
			protected ISingleBusDeclarationDataAdapter DataAdapter { get; }

			public ISingleBusInputDataProvider SingleBusDataProvider { get; }

			protected SingleBusBase(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report)
			{
				DataAdapter = dataAdapter;
				Report = report;
				SingleBusDataProvider = dataProvider;
			}

			#region Overrides of AbstractDeclarationVectoRunDataFactory

			protected override void Initialize()
			{
				var vehicle = SingleBusDataProvider.JobInputData.Vehicle;
				if (vehicle.ExemptedVehicle) {
					return;
				}

				_segment = GetSegment(SingleBusDataProvider);
				
			}

			protected virtual VectoRunData CreateCommonRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
			{
				var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType,
					_ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));

                CheckSuperCap(SingleBusDataProvider.PrimaryVehicle);

				var simulationRunData = new VectoRunData {
					InputData = SingleBusDataProvider,
					Loading = loading.Key,
					Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
					DriverData = DriverData,
					ExecutionMode = ExecutionMode.Declaration,
					JobName = SingleBusDataProvider.JobInputData.Vehicle
						.Identifier, //?!? Jobname
					ModFileSuffix = $"_{_segment.VehicleClass.GetClassNumber()}_{loading.Key}",
					Report = Report,
					Mission = mission,
					InputDataHash = SingleBusDataProvider.XMLHash, // right hash?!?
					SimulationType = SimulationType.DistanceCycle,
					VehicleDesignSpeed = _segment.DesignSpeed,
					MaxChargingPower = SingleBusDataProvider.PrimaryVehicle.MaxChargingPower,
				};

				return simulationRunData;
            }

			#endregion
			protected Segment GetSegment(ISingleBusInputDataProvider singleBus)
			{
				var vehicle = singleBus.JobInputData.Vehicle;
				var completedVehicle = singleBus.CompletedVehicle;
				var primaryVehicle = singleBus.PrimaryVehicle;

				var segment = DeclarationData.CompletedBusSegments.Lookup(
					primaryVehicle.AxleConfiguration.NumAxles(), completedVehicle.VehicleCode, completedVehicle.RegisteredClass, completedVehicle.NumberPassengerSeatsLowerDeck,
					completedVehicle.Height, completedVehicle.LowEntry);
				if (!segment.Found)
				{
					throw new VectoException(
						"no segment found for vehicle configruation: vehicle category: {0}, axle configuration: {1}, articulated: {2}, vehicle code: {3}, registered class: {4}, passengersLowerDeck: {5}, height: {6}, lowfloor: {7}. completed",
						vehicle.VehicleCategory, primaryVehicle.AxleConfiguration,
						vehicle.Articulated, completedVehicle.VehicleCode, completedVehicle.RegisteredClass.GetLabel(), completedVehicle.NumberPassengerSeatsLowerDeck,
						completedVehicle.Height, completedVehicle.LowEntry);
				}
				foreach (var mission in segment.Missions)
				{
					mission.VehicleHeight = completedVehicle.Height + mission.BusParameter.DeltaHeight;
					mission.BusParameter.VehicleLength = completedVehicle.Length;
				}
				return segment;
			}

			protected override DriverData CreateDriverData(Segment segment)
			{
				return DataAdapter.CreateDriverData(segment);
			}

			protected override VectoRunData GetPowertrainConfigForReportInit()
			{
				return GetNextRun().First(x => x != null);
			}

            protected abstract void CreateGearboxAndGearshiftData(VectoRunData runData);

			protected abstract bool AxleGearRequired();
        }

		

		public class Conventional : SingleBusBase
		{
			public Conventional(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter)
			{

			}

			protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, VectoRunData.OvcHevMode ovcMode = VectoRunData.OvcHevMode.NotApplicable)
			{
				var completedVehicle = SingleBusDataProvider.CompletedVehicle;
                var primaryVehicle = SingleBusDataProvider.PrimaryVehicle;
                var doubleDecker = completedVehicle.NumberPassengerSeatsUpperDeck > 0;
                if (mission.BusParameter.DoubleDecker != doubleDecker) {
                    return null;
                }

                var engine = vehicle.Components.EngineInputData;
                var engineModes = engine.EngineModes;
                var engineMode = engineModes[modeIdx.Value];

                var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType, _ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));

				var runData = CreateCommonRunData(mission, loading);

                runData.VehicleData = DataAdapter.CreateVehicleData(SingleBusDataProvider, _segment, mission, loading, _allowVocational); //Primary
				runData.AirdragData = DataAdapter.CreateAirdragData(completedVehicle, mission); //Single
				runData.EngineData = DataAdapter.CreateEngineData(vehicle, engineMode, mission); //Primary
				runData.ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>();
				runData.AxleGearData = DataAdapter.CreateAxleGearData(primaryVehicle.Components.AxleGearInputData);
				runData.AngledriveData =
					DataAdapter.CreateAngledriveData(primaryVehicle.Components.AngledriveInputData);
				runData.Aux = DataAdapter.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData,
					vehicle.Components.BusAuxiliaries, mission.MissionType,
					_segment.VehicleClass, vehicle.Length ?? mission.BusParameter.VehicleLength,
					vehicle.Components.AxleWheels.NumSteeredAxles, vehicle.VehicleType);
				runData.Retarder = DataAdapter.CreateRetarderData(primaryVehicle.Components.RetarderInputData);
				
				runData.EngineData.FuelMode = modeIdx.Value;
				runData.VehicleData.VehicleClass = _segment.VehicleClass;
				runData.BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(mission, primaryVehicle, completedVehicle, runData);
				
				CreateGearboxAndGearshiftData(runData);
                return runData;
            }

			

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				var primaryVehicle = SingleBusDataProvider.PrimaryVehicle;
                var shiftStrategyName =
					PowertrainBuilder.GetShiftStrategyName(primaryVehicle.Components.GearboxInputData.Type,
						primaryVehicle.VehicleType);
				runData.GearboxData = DataAdapter.CreateGearboxData(primaryVehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						runData.GearboxData,
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) *
						(runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						primaryVehicle.EngineIdleSpeed
					);
			}

			protected override bool AxleGearRequired()
			{
				return true;
			}

			protected override IEnumerable<VectoRunData> GetNextRun()
			{

				var vehicle = SingleBusDataProvider.JobInputData.Vehicle;

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

        }

        public abstract class Hybrid : SingleBusBase
		{
			protected Hybrid(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var engineModes = SingleBusDataProvider.PrimaryVehicle.Components.EngineInputData
					?.EngineModes;

				for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
					
					foreach (var mission in _segment.Missions) {
						foreach (var loading in mission.Loadings) {
							if (SingleBusDataProvider.PrimaryVehicle.OvcHev) {
								if (SingleBusDataProvider.PrimaryVehicle.MaxChargingPower.IsEqual(0)) {
									throw new VectoException(
										"MaxChargingPower has to be greater than 0 if OVC is selected");
								}
								yield return CreateVectoRunData(null, mission, loading, modeIdx, VectoRunData.OvcHevMode.ChargeDepleting);
								yield return CreateVectoRunData(null, mission, loading, modeIdx, VectoRunData.OvcHevMode.ChargeSustaining);
							} else {
								yield return CreateVectoRunData(null, mission, loading, modeIdx, VectoRunData.OvcHevMode.ChargeSustaining);
							}
						}
					}
				}
			}

			protected void CheckMaxChargingPowerPresent(IVehicleDeclarationInputData vehicle)
			{
				if (vehicle.OvcHev && vehicle.MaxChargingPower == null) {
					throw new VectoException($"{XMLNames.Vehicle_MaxChargingPower} must be set for OVC Vehicles");
				}
			}
		}

		public abstract class SerialHybrid : Hybrid
		{
			protected SerialHybrid(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }

            protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, Mission mission,
                KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
                int? modeIdx = null, VectoRunData.OvcHevMode ovcMode = VectoRunData.OvcHevMode.NotApplicable)
            {
                //CheckMaxChargingPowerPresent(vehicle);
                var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
                var engineModes = engine.EngineModes;
                var engineMode = engineModes[modeIdx.Value];
                var runData = CreateCommonRunData(mission, loading);


                runData.DriverData = DriverData;
                runData.AirdragData =
                    DataAdapter.CreateAirdragData(SingleBusDataProvider.CompletedVehicle, mission);
                runData.VehicleData = DataAdapter.CreateVehicleData(SingleBusDataProvider, _segment, mission, loading, _allowVocational);


                runData.EngineData = DataAdapter.CreateEngineData(vehicle, engineMode, mission);

                DataAdapter.CreateREESSData(vehicle.Components.ElectricStorage, vehicle.VehicleType, vehicle.OvcHev,
                    ((batteryData) => runData.BatteryData = batteryData),
                    ((sCdata => runData.SuperCapData = sCdata)));

                runData.ElectricMachinesData = DataAdapter.CreateElectricMachines(
                    vehicle.Components.ElectricMachines, vehicle.ElectricMotorTorqueLimits,
                    runData.BatteryData.CalculateAverageVoltage());

                if (vehicle.VehicleType == VectoSimulationJobType.IEPC_S) {
                    var iepcData = DataAdapter.CreateIEPCElectricMachines(vehicle.Components.IEPC,
                        runData.BatteryData.CalculateAverageVoltage());
                    iepcData.ForEach(iepc => runData.ElectricMachinesData.Add(iepc));
                }

                if (AxleGearRequired()) {
                    runData.AxleGearData = DataAdapter.CreateAxleGearData(vehicle.Components.AxleGearInputData);
                }

                runData.Retarder = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData);

                runData.Aux = DataAdapter.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData,
                    vehicle.Components.BusAuxiliaries, mission.MissionType,
                    _segment.VehicleClass, vehicle.Length, vehicle.Components.AxleWheels.NumSteeredAxles,
                    VectoSimulationJobType.SerialHybridVehicle);
                runData.BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(
                    mission, SingleBusDataProvider.PrimaryVehicle, SingleBusDataProvider.CompletedVehicle , runData);

                CreateGearboxAndGearshiftData(runData);

                runData.HybridStrategyParameters =
                    DataAdapter.CreateHybridStrategy(runData.BatteryData, runData.SuperCapData, runData.VehicleData.TotalVehicleMass,
                        ovcMode, loading.Key, runData.VehicleData.VehicleClass, mission.MissionType);

                if (ovcMode != VectoRunData.OvcHevMode.NotApplicable) {
                    if (runData.BatteryData != null) {
                        runData.BatteryData.InitialSoC = runData.HybridStrategyParameters.InitialSoc;
                    }

                    if (runData.SuperCapData != null) {
                        runData.SuperCapData.InitialSoC = runData.HybridStrategyParameters.InitialSoc;
                    }
                }

                if (ovcMode != VectoRunData.OvcHevMode.NotApplicable && runData.InputData.JobInputData.Vehicle.OvcHev) {
                    runData.ModFileSuffix += ovcMode == VectoRunData.OvcHevMode.ChargeSustaining ? "CS" : "CD";
                }

                if (ovcMode == VectoRunData.OvcHevMode.ChargeDepleting) {
                    runData.BatteryData.Batteries.ForEach(b => b.Item2.ChargeSustainingBattery = true);
                }

                runData.OVCMode = ovcMode;

                return runData;
            }

            protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
            {
                if (SingleBusDataProvider.PrimaryVehicle.ArchitectureID.IsOneOf(ArchitectureID.S2, ArchitectureID.S_IEPC)) {
                    throw new ArgumentException(nameof(SingleBusDataProvider.PrimaryVehicle.ArchitectureID));
                }
                runData.GearshiftParameters = new ShiftStrategyParameters() {
                    StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
                    StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
                };
            }

            protected override bool AxleGearRequired()
            {
                return InputDataProvider.JobInputData.Vehicle.Components.AxleGearInputData != null;
            }
        }
		public  class HEV_S2 : SerialHybrid
		{
			protected HEV_S2(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (SingleBusDataProvider.PrimaryVehicle.ArchitectureID != ArchitectureID.S2) {
					throw new ArgumentException(nameof(SingleBusDataProvider.PrimaryVehicle));
				}

				var primaryVehicle = SingleBusDataProvider.PrimaryVehicle;
				var shiftStrategyName =
					PowertrainBuilder.GetShiftStrategyName(primaryVehicle.Components.GearboxInputData.Type,
						primaryVehicle.VehicleType);
				runData.GearboxData = DataAdapter.CreateGearboxData(primaryVehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						runData.GearboxData,
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) *
						(runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						primaryVehicle.EngineIdleSpeed
					);
			}
        }
        
		public class HEV_S3 : SerialHybrid
		{
			protected HEV_S3(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

		public class HEV_S4 : SerialHybrid
		{
			protected HEV_S4(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}


		public class HEV_S_IEPC : SerialHybrid
		{
			protected HEV_S_IEPC(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

        public abstract class ParallelHybrid : Hybrid
		{
			public ParallelHybrid(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }

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

				var runData = CreateCommonRunData(mission, loading);

				runData.VehicleData =
					DataAdapter.CreateVehicleData(SingleBusDataProvider, _segment, mission, loading, _allowVocational);
				runData.AirdragData = DataAdapter.CreateAirdragData(SingleBusDataProvider.CompletedVehicle, mission);
				runData.EngineData =
					DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode, mission);
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
					mission, SingleBusDataProvider.PrimaryVehicle, SingleBusDataProvider.CompletedVehicle, runData);

				CreateGearboxAndGearshiftData(runData);

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
						mission.MissionType, vehicle.BoostingLimitations, runData.GearboxData, runData.EngineData,
						vehicle.ArchitectureID);

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

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
            {
				var primaryVehicle = SingleBusDataProvider.PrimaryVehicle;
				var shiftStrategyName =
					PowertrainBuilder.GetShiftStrategyName(primaryVehicle.Components.GearboxInputData.Type,
						primaryVehicle.VehicleType);
				runData.GearboxData = DataAdapter.CreateGearboxData(primaryVehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						runData.GearboxData,
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) *
						(runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						primaryVehicle.EngineIdleSpeed
					);
			}

            protected override bool AxleGearRequired()
            {
                return true;
            }
        }

		public class HEV_P1 : ParallelHybrid
		{
			public HEV_P1(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

		public class HEV_P2 : ParallelHybrid
		{
			public HEV_P2(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

		public class HEV_P2_5 : ParallelHybrid
		{
			public HEV_P2_5(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

		public class HEV_P3 : ParallelHybrid
		{
			public HEV_P3(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

		public class HEV_P4 : ParallelHybrid
		{
			public HEV_P4(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

		public abstract class BatteryElectric : SingleBusBase
		{
			public BatteryElectric(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				foreach (var mission in _segment.Missions) {
					foreach (var loading in mission.Loadings) {
						var run = CreateVectoRunData(null, mission, loading);
						run.BatteryData.Batteries.ForEach(b => b.Item2.ChargeSustainingBattery = true);
						yield return run;
					}
				}
			}

            protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData _, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
                int? modeIdx = null, VectoRunData.OvcHevMode ovcMode = VectoRunData.OvcHevMode.NotApplicable)
			{
				var vehicle = SingleBusDataProvider.PrimaryVehicle;

                var result = CreateCommonRunData(mission, loading);

                DataAdapter.CreateREESSData(
                    componentsElectricStorage: SingleBusDataProvider.PrimaryVehicle.Components.ElectricStorage,
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

                result.VehicleData = DataAdapter.CreateVehicleData(null, _segment, mission, loading, _allowVocational);
                result.AirdragData = DataAdapter.CreateAirdragData(SingleBusDataProvider.CompletedVehicle, mission);
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
                    mission, SingleBusDataProvider.PrimaryVehicle, SingleBusDataProvider.CompletedVehicle, result);
                CreateGearboxAndGearshiftData(result);

                return result;
            }
            protected override bool AxleGearRequired()
            {
                return InputDataProvider.JobInputData.Vehicle.ArchitectureID != ArchitectureID.E4;
            }

            protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
            {
                if (SingleBusDataProvider.PrimaryVehicle.ArchitectureID == ArchitectureID.E2) {
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
			public PEV_E2(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (SingleBusDataProvider.PrimaryVehicle.ArchitectureID != ArchitectureID.E2) {
					throw new ArgumentException(nameof(SingleBusDataProvider.PrimaryVehicle));
				}

				var primaryVehicle = SingleBusDataProvider.PrimaryVehicle;
				var shiftStrategyName =
					PowertrainBuilder.GetShiftStrategyName(primaryVehicle.Components.GearboxInputData.Type,
						primaryVehicle.VehicleType);
				runData.GearboxData = DataAdapter.CreateGearboxData(primaryVehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						runData.GearboxData,
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) *
						(runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						primaryVehicle.EngineIdleSpeed
					);
			}
        }

		public class PEV_E3 : BatteryElectric
		{
			public PEV_E3(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

		public class PEV_E4 : BatteryElectric
		{
			public PEV_E4(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}

		public class PEV_E_IEPC : BatteryElectric
		{
			public PEV_E_IEPC(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }
		}


		public class Exempted : SingleBusBase
		{
			public Exempted(ISingleBusInputDataProvider dataProvider, IDeclarationReport report, ISingleBusDeclarationDataAdapter dataAdapter) : base(dataProvider, report, dataAdapter) { }

			#region Overrides of SingleBusBase

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				throw new NotImplementedException("Exempted SingleBus Simulation is not supported!");
			}

			protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, VectoRunData.OvcHevMode ovcMode = VectoRunData.OvcHevMode.NotApplicable)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region Overrides of SingleBusBase

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				throw new NotImplementedException();
			}

			protected override bool AxleGearRequired()
			{
				throw new NotImplementedException();
			}

			#endregion
		}


	}

    //internal class DeclarationModeSingleBusVectoRunDataFactory : DeclarationModePrimaryBusVectoRunDataFactory
    //{
    //    protected new DeclarationDataAdapterSingleBus _dao = new DeclarationDataAdapterSingleBus();
    //    private ISingleBusInputDataProvider _singleBusInputData;

    //    public DeclarationModeSingleBusVectoRunDataFactory(ISingleBusInputDataProvider singleBusInputData, IDeclarationReport report) : base(singleBusInputData, report)
    //    {
    //        _singleBusInputData = singleBusInputData;
    //        _dao.SingleBusInputData = singleBusInputData;
    //    }


    //    protected override Segment GetSegment(IVehicleDeclarationInputData vehicle)
    //    {
    //        //if (vehicle.VehicleCategory != VehicleCategory.HeavyBusCompletedVehicle) {
    //        //	throw new VectoException(
    //        //		"Invalid vehicle category for bus factory! {0}", vehicle.VehicleCategory.GetCategoryName());
    //        //}

    //        var completedVehicle = _singleBusInputData.CompletedVehicle;

    //        var segment = DeclarationData.CompletedBusSegments.Lookup(
    //            _singleBusInputData.PrimaryVehicle.AxleConfiguration.NumAxles(), completedVehicle.VehicleCode, completedVehicle.RegisteredClass, completedVehicle.NumberPassengerSeatsLowerDeck,
    //            completedVehicle.Height, completedVehicle.LowEntry);
    //        if (!segment.Found)
    //        {
    //            throw new VectoException(
    //                "no segment found for vehicle configruation: vehicle category: {0}, axle configuration: {1}, articulated: {2}, vehicle code: {3}, registered class: {4}, passengersLowerDeck: {5}, height: {6}, lowfloor: {7}. completed",
    //                vehicle.VehicleCategory, _singleBusInputData.PrimaryVehicle.AxleConfiguration,
    //                vehicle.Articulated, completedVehicle.VehicleCode, completedVehicle.RegisteredClass.GetLabel(), completedVehicle.NumberPassengerSeatsLowerDeck,
    //                completedVehicle.Height, completedVehicle.LowEntry);
    //        }
    //        foreach (var mission in segment.Missions)
    //        {
    //            mission.VehicleHeight = completedVehicle.Height + mission.BusParameter.DeltaHeight;
    //            mission.BusParameter.VehicleLength = completedVehicle.Length;
    //        }

    //        return segment;
    //    }


    //    protected override ISingleBusDeclarationDataAdapter DataAdapter => _dao;


    //    protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
    //    {
    //        var doubleDecker = _singleBusInputData.CompletedVehicle.NumberPassengerSeatsUpperDeck > 0;
    //        if (mission.BusParameter.DoubleDecker != doubleDecker)
    //        {
    //            return null;
    //        }

    //        var engine = vehicle.Components.EngineInputData;
    //        var engineModes = engine.EngineModes;
    //        var engineMode = engineModes[modeIdx];

    //        var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType, _ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));

    //        var simulationRunData = new VectoRunData
    //        {
    //            Loading = loading.Key,
    //            VehicleData = DataAdapter.CreateVehicleData(vehicle, _segment, mission, loading, _allowVocational),
    //            AirdragData = _dao.CreateAirdragData(_singleBusInputData.CompletedVehicle, mission),
    //            EngineData = DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode, mission),
    //            ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
    //            GearboxData = _gearboxData,
    //            AxleGearData = _axlegearData,
    //            AngledriveData = _angledriveData,
    //            Aux = DataAdapter.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData,
    //                                                vehicle.Components.BusAuxiliaries, mission.MissionType,
    //                                                _segment.VehicleClass, vehicle.Length ?? mission.BusParameter.VehicleLength,
    //                                                vehicle.Components.AxleWheels.NumSteeredAxles),
    //            Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
    //            Retarder = _retarderData,
    //            DriverData = _driverdata,
    //            ExecutionMode = ExecutionMode.Declaration,
    //            JobName = InputDataProvider.JobInputData.JobName,
    //            ModFileSuffix = $"{(engineModes.Count > 1 ? $"_EngineMode{modeIdx}_" : "")}" +
    //                            $"_{mission.BusParameter.BusGroup.GetClassNumber()}-Single_{loading.Key}",
    //            Report = Report,
    //            Mission = mission,
    //            InputDataHash = InputDataProvider.XMLHash,
    //            SimulationType = SimulationType.DistanceCycle,
    //            GearshiftParameters = _gearshiftData,
    //            VehicleDesignSpeed = _segment.DesignSpeed,
    //            //ShiftStrategy = InputDataProvider.JobInputData.ShiftStrategy
    //        };
    //        simulationRunData.EngineData.FuelMode = modeIdx;
    //        simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
    //        simulationRunData.BusAuxiliaries = _dao.CreateBusAuxiliariesData(mission, _singleBusInputData.PrimaryVehicle, _singleBusInputData.CompletedVehicle, simulationRunData);
    //        return simulationRunData;
    //    }

    //    protected override void InitializeReport()
    //    {
    //        VectoRunData powertrainConfig;
    //        List<List<FuelData.Entry>> fuels;
    //        var vehicle = InputDataProvider.JobInputData.Vehicle;
    //        if (vehicle.ExemptedVehicle)
    //        {
    //            powertrainConfig = CreateVectoRunData(vehicle, 0, null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>());
    //            fuels = new List<List<FuelData.Entry>>();
    //        }
    //        else
    //        {
    //            powertrainConfig = _segment.Missions.Select(
    //                    mission => CreateVectoRunData(
    //                        vehicle, 0, mission, mission.Loadings.First()))
    //                .FirstOrDefault(x => x != null);
    //            fuels = vehicle.Components.EngineInputData.EngineModes.Select(x => x.Fuels.Select(f => DeclarationData.FuelData.Lookup(f.FuelType, _singleBusInputData.CompletedVehicle.TankSystem)).ToList())
    //                .ToList();
    //        }
    //        Report.InitializeReport(powertrainConfig, fuels);
    //    }
    //}


}
