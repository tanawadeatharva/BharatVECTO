using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.CompletedBusRunDataFactory
{
    public abstract class DeclarationModeCompletedBusRunDataFactory
    {
		public abstract class CompletedBusBase : AbstractDeclarationVectoRunDataFactory
		{
			public IGenericCompletedBusDeclarationDataAdapter DataAdapterGeneric { get; }
			public ISpecificCompletedBusDeclarationDataAdapter DataAdapterSpecific { get; }
			protected IMultistageVIFInputData DataProvider { get; }

			//protected IDeclarationReport Report { get; set; }
			protected IVehicleDeclarationInputData PrimaryVehicle =>
				DataProvider.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle;

			protected IVehicleDeclarationInputData CompletedVehicle => DataProvider.MultistageJobInputData.JobInputData
				.ConsolidateManufacturingStage.Vehicle;

			public CompletedBusBase(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(null, report, false)
			{
				DataAdapterSpecific = dataAdapterSpecific;
				DataAdapterGeneric = dataAdapterGeneric;
				DataProvider = dataProvider;
			}

            #region Implementation of AbstractDeclarationVectoRunDataFactory

            protected override void Initialize()
			{
				if (CompletedVehicle.ExemptedVehicle || PrimaryVehicle.ExemptedVehicle) {
					return;
				}

				_segment = GetCompletedSegment(CompletedVehicle, PrimaryVehicle.AxleConfiguration);
			}

			protected override DriverData CreateDriverData(Segment segment)
			{
				return DataAdapterGeneric.CreateDriverData(segment);
			}

            protected override VectoRunData GetPowertrainConfigForReportInit()
			{
				return GetNextRun().First(x => x != null);
			}


			protected abstract VectoRunData CreateVectoRunDataGeneric(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, Segment primarySegment, int? modeIdx);

			protected abstract VectoRunData CreateVectoRunDataSpecific(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, int? modeIdx);

			protected virtual VectoRunData CreateCommonRunData(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, string modSuffix)
			{
				var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType,
					_ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));

				CheckSuperCap(PrimaryVehicle);

				var simulationRunData = new VectoRunData {
					InputData = DataProvider.MultistageJobInputData,
					Loading = loading.Key,
					Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
					DriverData = DriverData,
					ExecutionMode = ExecutionMode.Declaration,
					JobName = DataProvider.MultistageJobInputData.JobInputData.ManufacturingStages.Last().Vehicle
						.Identifier, //?!? Jobname
					ModFileSuffix = $"_{_segment.VehicleClass.GetClassNumber()}-{modSuffix}_{loading.Key}",
					Report = Report,
					Mission = mission,
					InputDataHash = DataProvider.MultistageJobInputData.XMLHash, // right hash?!?
					SimulationType = SimulationType.DistanceCycle,
					VehicleDesignSpeed = _segment.DesignSpeed,
					MaxChargingPower = PrimaryVehicle.MaxChargingPower,
				};

				return simulationRunData;
			}


            protected virtual Segment GetPrimarySegment(IVehicleDeclarationInputData primaryVehicle)
			{
				var primarySegment = DeclarationData.PrimaryBusSegments.Lookup(
					primaryVehicle.VehicleCategory, primaryVehicle.AxleConfiguration, primaryVehicle.Articulated);

				return primarySegment;
			}

			protected virtual IEnumerable<VectoRunData> CreateVectoRunData(
				Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, string fuelMode = null,
				VectoRunData.OvcHevMode ovcMode = VectoRunData.OvcHevMode.NotApplicable)
			{
				// create specific run data
				var simulationRunData = CreateVectoRunDataSpecific(mission, loading, modeIdx);
				if (simulationRunData != null) {
					yield return simulationRunData;
				}

				// create generic run data
				var primarySegment = GetPrimarySegment(PrimaryVehicle);
				var primaryMission = primarySegment.Missions.Where(
					m => {
						return m.BusParameter.DoubleDecker ==
								CompletedVehicle.VehicleCode.IsDoubleDeckerBus() &&
								m.MissionType == mission.MissionType &&
								m.BusParameter.FloorType == CompletedVehicle.VehicleCode.GetFloorType();
					}).First();
				simulationRunData = CreateVectoRunDataGeneric(
					primaryMission,
					new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(loading.Key,
						primaryMission.Loadings[loading.Key]),
					primarySegment, modeIdx);
				simulationRunData.PrimaryResult = GetPrimaryResult(fuelMode, simulationRunData);

				yield return simulationRunData;
			}


			protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, VectoRunData.OvcHevMode ovcMode = VectoRunData.OvcHevMode.NotApplicable)
			{
				throw new NotImplementedException("Not applicable for completed buses");
			}

            protected virtual IResult GetPrimaryResult(string fuelMode, VectoRunData simulationRunData)
			{
				var primaryResult = DataProvider.MultistageJobInputData.JobInputData.PrimaryVehicle.GetResult(
					simulationRunData.Mission.BusParameter.BusGroup, simulationRunData.Mission.MissionType, fuelMode,
					simulationRunData.VehicleData.Loading);
				if (primaryResult == null || !primaryResult.ResultStatus.Equals("success")) {
					throw new VectoException(
						"Failed to find results in PrimaryVehicleReport for vehicle group: {0},  mission: {1}, fuel mode: '{2}', payload: {3}. Make sure PIF and completed vehicle data match!",
						simulationRunData.Mission.BusParameter.BusGroup, simulationRunData.Mission.MissionType, fuelMode,
						simulationRunData.VehicleData.Loading);
				}

				if (primaryResult.ResultStatus != "success") {
					throw new VectoException(
						"Simulation results in PrimaryVehicleReport for vehicle group: {0},  mission: {1}, fuel mode: '{2}', payload: {3} not finished successfully.",
						simulationRunData.Mission.BusParameter.BusGroup, simulationRunData.Mission.MissionType, fuelMode,
						simulationRunData.VehicleData.Loading);
				}

				return primaryResult;
			}

			protected virtual Segment GetCompletedSegment(IVehicleDeclarationInputData vehicle, AxleConfiguration axleConfiguration)
            {
                var segment = DeclarationData.CompletedBusSegments.Lookup(
                    axleConfiguration.NumAxles(), vehicle.VehicleCode, vehicle.RegisteredClass, vehicle.NumberPassengerSeatsLowerDeck,
                    vehicle.Height, vehicle.LowEntry);
                if (!segment.Found) {
                    throw new VectoException(
                        "no segment found for vehicle configruation: vehicle category: {0}, axle configuration: {1}, articulated: {2}, vehicle code: {3}, registered class: {4}, passengersLowerDeck: {5}, height: {6}, lowEntry: {7}. completed",
                        vehicle.VehicleCategory, axleConfiguration,
                        vehicle.Articulated, vehicle.VehicleCode, vehicle.RegisteredClass.GetLabel(), vehicle.NumberPassengerSeatsLowerDeck,
                        vehicle.Height, vehicle.LowEntry);
                }

                return segment;
            }
            #endregion

			protected abstract void CreateGearboxAndGearshiftData(VectoRunData runData);
        }

        public class Conventional : CompletedBusBase
        {
            public Conventional(IMultistageVIFInputData dataProvider, IDeclarationReport report,
                ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
                IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific,
                dataAdapterGeneric)
            { }

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var engineModes = PrimaryVehicle.Components.EngineInputData
					?.EngineModes;

				for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
					var fuelMode = "single fuel mode";
					if (engineModes[modeIdx].Fuels.Count > 1) {
						fuelMode = "dual fuel mode";
					}

					foreach (var mission in _segment.Missions) {
						foreach (var loading in mission.Loadings) {
							foreach (var run in CreateVectoRunData(mission, loading, modeIdx, fuelMode)) {
								yield return run;
							}
						}
					}
				}
			}

            protected override VectoRunData CreateVectoRunDataGeneric(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, Segment primarySegment, int? modeIdx)
            {
                var simulationRunData = CreateCommonRunData(mission, loading, "Generic");

                var primaryBusAuxiliaries = PrimaryVehicle.Components.BusAuxiliaries;

                simulationRunData.VehicleData =
                    DataAdapterGeneric.CreateVehicleData(PrimaryVehicle, primarySegment, mission, loading, false);
                simulationRunData.AirdragData = DataAdapterGeneric.CreateAirdragData(null, mission, new Segment());
                simulationRunData.EngineData =
                    DataAdapterGeneric.CreateEngineData(PrimaryVehicle, modeIdx.Value, mission);
                simulationRunData.ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>();
                simulationRunData.AxleGearData =
                    DataAdapterGeneric.CreateAxleGearData(PrimaryVehicle.Components.AxleGearInputData);
                simulationRunData.AngledriveData =
                    DataAdapterGeneric.CreateAngledriveData(PrimaryVehicle.Components.AngledriveInputData);
                simulationRunData.Aux = DataAdapterGeneric.CreateAuxiliaryData(
                    PrimaryVehicle.Components.AuxiliaryInputData, primaryBusAuxiliaries, mission.MissionType,
                    primarySegment.VehicleClass, mission.BusParameter.VehicleLength,
                    PrimaryVehicle.Components.AxleWheels.NumSteeredAxles, PrimaryVehicle.VehicleType);
                simulationRunData.Retarder =
                    DataAdapterGeneric.CreateRetarderData(PrimaryVehicle.Components.RetarderInputData);

                simulationRunData.EngineData.FuelMode = 0;
                simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
                simulationRunData.BusAuxiliaries =
                    DataAdapterGeneric.CreateBusAuxiliariesData(mission, PrimaryVehicle, CompletedVehicle, simulationRunData);
                var shiftStrategyName =
                    PowertrainBuilder.GetShiftStrategyName(PrimaryVehicle.Components.GearboxInputData.Type,
                        PrimaryVehicle.VehicleType);
                simulationRunData.GearboxData = DataAdapterGeneric.CreateGearboxData(PrimaryVehicle, simulationRunData,
                    ShiftPolygonCalculator.Create(shiftStrategyName, simulationRunData.GearshiftParameters));
                simulationRunData.GearshiftParameters =
                    DataAdapterGeneric.CreateGearshiftData(
                        simulationRunData.GearboxData,
                        (simulationRunData.AxleGearData?.AxleGear.Ratio ?? 1.0) *
                        (simulationRunData.AngledriveData?.Angledrive.Ratio ?? 1.0),
                        PrimaryVehicle.EngineIdleSpeed);
                return simulationRunData;
            }

            protected override VectoRunData CreateVectoRunDataSpecific(Mission mission,
                KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, int? modeIdx)
            {
                var simulationRunData = CreateCommonRunData(mission, loading, "Specific");

                simulationRunData.VehicleData = DataAdapterSpecific.CreateVehicleData(PrimaryVehicle,
                            CompletedVehicle, _segment, mission, loading);
                simulationRunData.AirdragData = DataAdapterSpecific.CreateAirdragData(CompletedVehicle, mission);
                simulationRunData.EngineData =
                    DataAdapterGeneric.CreateEngineData(PrimaryVehicle, modeIdx.Value, mission);
                simulationRunData.ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>();
                simulationRunData.AxleGearData =
                        DataAdapterGeneric.CreateAxleGearData(PrimaryVehicle.Components.AxleGearInputData);
                simulationRunData.AngledriveData =
                    DataAdapterGeneric.CreateAngledriveData(PrimaryVehicle.Components.AngledriveInputData);
                simulationRunData.Aux = DataAdapterSpecific.CreateAuxiliaryData(
                    PrimaryVehicle.Components.AuxiliaryInputData,
                    PrimaryVehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
                    CompletedVehicle.Length, PrimaryVehicle.Components.AxleWheels.NumSteeredAxles, PrimaryVehicle.VehicleType);
                simulationRunData.Retarder = DataAdapterGeneric.CreateRetarderData(PrimaryVehicle.Components.RetarderInputData);
                simulationRunData.EngineData.FuelMode = 0;
                simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
                simulationRunData.BusAuxiliaries =
                    DataAdapterSpecific.CreateBusAuxiliariesData(mission, PrimaryVehicle, CompletedVehicle,
                        simulationRunData);

               CreateGearboxAndGearshiftData(simulationRunData);
                return simulationRunData;
            }


			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				var shiftStrategyName =
					PowertrainBuilder.GetShiftStrategyName(PrimaryVehicle.Components.GearboxInputData.Type,
						PrimaryVehicle.VehicleType);
				runData.GearboxData = DataAdapterGeneric.CreateGearboxData(PrimaryVehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));
				runData.GearshiftParameters =
					DataAdapterGeneric.CreateGearshiftData(
						runData.GearboxData,
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) *
						(runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						PrimaryVehicle.EngineIdleSpeed
					);

            }
        }

        public abstract class Hybrid : CompletedBusBase
		{
			protected Hybrid(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report,
				dataAdapterSpecific, dataAdapterGeneric) { }

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var engineModes = PrimaryVehicle.Components.EngineInputData
					?.EngineModes;

				for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
					var fuelMode = "single fuel mode";
					if (engineModes[modeIdx].Fuels.Count > 1) {
						fuelMode = "dual fuel mode";
					}

					foreach (var mission in _segment.Missions) {
						foreach (var loading in mission.Loadings) {
							// TODO: charge sustaining / charge depleting
							foreach (var run in CreateVectoRunData(mission, loading, modeIdx, fuelMode)) {
								yield return run;
							}
						}
					}
				}
			}

			#region Overrides of CompletedBusBase

			protected override VectoRunData CreateVectoRunDataGeneric(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, Segment primarySegment, int? modeIdx)
			{
				throw new NotImplementedException("dummy implementation");
			}

			protected override VectoRunData CreateVectoRunDataSpecific(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, int? modeIdx)
			{
				throw new NotImplementedException("dummy implementation");
			}

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				throw new NotImplementedException("dummy implementation");
			}

			#endregion
		}


		public abstract class SerialHybrid : Hybrid
		{
			protected SerialHybrid(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report,
				dataAdapterSpecific, dataAdapterGeneric) { }
		}

        public class HEV_S2 : SerialHybrid
        {
			public HEV_S2(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report,
				dataAdapterSpecific, dataAdapterGeneric) { }

			#region Overrides of AbstractDeclarationVectoRunDataFactory

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				throw new NotImplementedException();
			}

			#endregion
		}
       
		public class HEV_S3 : SerialHybrid
        {
            public HEV_S3(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        
		public class HEV_S4 : SerialHybrid
        {
            public HEV_S4(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        
		public class HEV_S_IEPC : SerialHybrid
        {
            public HEV_S_IEPC(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }

		public abstract class ParallelHybrid : Hybrid
		{
			protected ParallelHybrid(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report,
				dataAdapterSpecific, dataAdapterGeneric) { }
		}

        public class HEV_P1 : ParallelHybrid
        {
            public HEV_P1(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        
		public class HEV_P2 : ParallelHybrid
        {
            public HEV_P2(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        
		public class HEV_P2_5 : ParallelHybrid
        {
            public HEV_P2_5(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        
		public class HEV_P3 : ParallelHybrid
        {
            public HEV_P3(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        
		public class HEV_P4 : ParallelHybrid
        {
            public HEV_P4(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }

		public abstract class BatteryElectric : CompletedBusBase
		{
			public BatteryElectric(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report,
				dataAdapterSpecific, dataAdapterGeneric) { }

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				foreach (var mission in _segment.Missions) {
					foreach (var loading in mission.Loadings) {
						foreach (var run in CreateVectoRunData(mission, loading)) {
							run.BatteryData.Batteries.ForEach(b => b.Item2.ChargeSustainingBattery = true);
							yield return run;
						}
					}
				}
			}

			protected override VectoRunData CreateVectoRunDataGeneric(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, Segment primarySegment, int? modeIdx)
			{
				throw new NotImplementedException();
			}

            protected override VectoRunData CreateVectoRunDataSpecific(Mission mission,
                KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, int? modeIdx)
            {
                var result = CreateCommonRunData(mission, loading, "Specific");

				DataAdapterGeneric.CreateREESSData(
					componentsElectricStorage: PrimaryVehicle.Components.ElectricStorage,
					PrimaryVehicle.VehicleType,
					true,
					(bs) => result.BatteryData = bs,
					(sc) => result.SuperCapData = sc);

				if (PrimaryVehicle.VehicleType == VectoSimulationJobType.IEPC_E) {
					result.ElectricMachinesData = DataAdapterGeneric.CreateIEPCElectricMachines(PrimaryVehicle.Components.IEPC,
						result.BatteryData.CalculateAverageVoltage());
				} else {
					result.ElectricMachinesData = DataAdapterGeneric.CreateElectricMachines(PrimaryVehicle.Components.ElectricMachines,
						PrimaryVehicle.ElectricMotorTorqueLimits, result.BatteryData.CalculateAverageVoltage(), null);
				}

                result.VehicleData = DataAdapterSpecific.CreateVehicleData(PrimaryVehicle, CompletedVehicle, _segment,
					mission, loading);
				result.AirdragData = DataAdapterSpecific.CreateAirdragData(CompletedVehicle, mission);
				if (AxleGearRequired() || PrimaryVehicle.Components.AxleGearInputData != null) {
					result.AxleGearData =
						DataAdapterGeneric.CreateAxleGearData(PrimaryVehicle.Components.AxleGearInputData);
				}

				result.AngledriveData =
					DataAdapterGeneric.CreateAngledriveData(PrimaryVehicle.Components.AngledriveInputData);
				result.Aux = DataAdapterSpecific.CreateAuxiliaryData(PrimaryVehicle.Components.AuxiliaryInputData,
					PrimaryVehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
					CompletedVehicle.Length, PrimaryVehicle.Components.AxleWheels.NumSteeredAxles,
					PrimaryVehicle.VehicleType);
				result.Retarder = DataAdapterGeneric.CreateRetarderData(PrimaryVehicle.Components.RetarderInputData);
				result.MaxChargingPower = PrimaryVehicle.MaxChargingPower;
                   
				//result.EngineData.FuelMode = 0;
				result.VehicleData.VehicleClass = _segment.VehicleClass;
				result.BusAuxiliaries =
                    DataAdapterSpecific.CreateBusAuxiliariesData(mission, PrimaryVehicle, CompletedVehicle,
						result);

                CreateGearboxAndGearshiftData(result);
                return result;
            }

			
			protected virtual bool AxleGearRequired()
			{
				return PrimaryVehicle.ArchitectureID != ArchitectureID.E4;
			}

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (PrimaryVehicle.ArchitectureID == ArchitectureID.E2) {
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
            public PEV_E2(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (PrimaryVehicle.ArchitectureID != ArchitectureID.E2) {
					throw new ArgumentException(nameof(PrimaryVehicle));
				}
				runData.GearshiftParameters =
					DataAdapterGeneric.CreateGearshiftData(
						runData.GearboxData,
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) *
						(runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						null
					);

                var shiftStrategyName =
					PowertrainBuilder.GetShiftStrategyName(PrimaryVehicle.Components.GearboxInputData.Type,
						PrimaryVehicle.VehicleType);
				runData.GearboxData = DataAdapterGeneric.CreateGearboxData(PrimaryVehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));
				
            }
        }
        public class PEV_E3 : BatteryElectric
        {
            public PEV_E3(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        public class PEV_E4 : BatteryElectric
        {
            public PEV_E4(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        public class PEV_E_IEPC : BatteryElectric
        {
            public PEV_E_IEPC(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }


        public class Exempted : CompletedBusBase
        {
            public Exempted(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }

            protected override IEnumerable<VectoRunData> GetNextRun()
            {

				return CreateVectoRunData(null,
					new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(),
					0);

            }

			protected override IEnumerable <VectoRunData> CreateVectoRunData(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, string fuelMode = null,
				VectoRunData.OvcHevMode ovcMode = VectoRunData.OvcHevMode.NotApplicable)
			{
                yield return new VectoRunData() {
                    Exempted = true,
					VehicleData = DataAdapterSpecific.CreateVehicleData(PrimaryVehicle, CompletedVehicle, new Segment(), null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>()),
					Report = Report,
                    Mission = new Mission() {
                        MissionType = MissionType.ExemptedMission
                    },
                    InputData = DataProvider.MultistageJobInputData
                };
			}

			protected override VectoRunData CreateVectoRunDataGeneric(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, Segment primarySegment, int? modeIdx)
			{
				throw new NotImplementedException();
			}

			protected override VectoRunData CreateVectoRunDataSpecific(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, int? modeIdx)
			{
				throw new NotImplementedException();
			}

            protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				throw new NotImplementedException();
			}
		}
    }
}