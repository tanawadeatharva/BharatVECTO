using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.HeavyLorryRunDataFactory;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;

namespace TUGraz.VectoMockup.Simulation.RundataFactories
{
    public class MockupLorryVectoRunDataFactory : DeclarationModeHeavyLorryRunDataFactory.Conventional
    {
        public MockupLorryVectoRunDataFactory(IDeclarationInputDataProvider dataProvider,
            IDeclarationReport report,
			ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter)
        {

        }

        #region Overrides of AbstractDeclarationVectoRunDataFactory

        //protected override IDeclarationDataAdapter DataAdapter { get; }
        protected override IEnumerable<VectoRunData> GetNextRun()
        {
			if (InputDataProvider.JobInputData.Vehicle.ExemptedVehicle) {
				yield return CreateVectoRunData(InputDataProvider.JobInputData.Vehicle, 0, null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>());
			} else {
				foreach (var vectoRunData in VectoRunDataTruckNonExempted()) {
					yield return vectoRunData;
				}
			}

   //         var nextRun = base.GetNextRun();
			//return nextRun;
        }

        protected override void InitializeReport()
        {
            if (InputDataProvider.JobInputData.JobType == VectoSimulationJobType.ConventionalVehicle)
            {
                base.InitializeReport();
                return;
            }
            VectoRunData powertrainConfig;
            var vehicle = InputDataProvider.JobInputData.Vehicle;
            if (vehicle.ExemptedVehicle)
            {
                powertrainConfig = CreateVectoRunData(vehicle, 0, null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>());
            }
            else
            {
                powertrainConfig = _segment.Missions.Select(
                        mission => CreateVectoRunData(
                            vehicle, 0, mission, mission.Loadings.First()))
                    .FirstOrDefault(x => x != null);
            }
            Report.InitializeReport(powertrainConfig);
        }

		private IEnumerable<VectoRunData> VectoRunDataTruckNonExempted()
		{
			switch (InputDataProvider.JobInputData.JobType) {
				case VectoSimulationJobType.ConventionalVehicle:
				case VectoSimulationJobType.ParallelHybridVehicle:
				case VectoSimulationJobType.SerialHybridVehicle:
                case VectoSimulationJobType.IEPC_S:
                case VectoSimulationJobType.IHPC:
					return VectoRunDataConventionalTruckNonExempted();
				case VectoSimulationJobType.BatteryElectricVehicle:
                case VectoSimulationJobType.IEPC_E:
					return VectoRunDataBatteryElectricVehicle();
				case VectoSimulationJobType.EngineOnlySimulation:
				case VectoSimulationJobType.FCHV:
				default:
					throw new ArgumentOutOfRangeException();
			}
			//return VectoRunDataConventionalTruckNonExempted();
		}

		private IEnumerable<VectoRunData> VectoRunDataBatteryElectricVehicle()
		{
			var vehicle = InputDataProvider.JobInputData.Vehicle;
			foreach (var mission in _segment.Missions) {
				foreach (var loading in mission.Loadings) {
					var simulationRunData = CreateVectoRunData(vehicle, 0, mission, loading);
					yield return simulationRunData;
				}

			}
		}

		private IEnumerable<VectoRunData> VectoRunDataConventionalTruckNonExempted()
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

        protected virtual VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission,
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
				runData.VehicleData.InputData = vehicle;
			}
            else
            {
				var cycle = CycleFactory.GetDeclarationCycle(mission);

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
                    GearboxData = CreateMockupGearboxData(vehicle),
                    AxleGearData = CreateMockupAxleGearData(vehicle),

                    JobType = InputDataProvider.JobInputData.JobType,

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

        private AxleGearData CreateMockupAxleGearData(IVehicleDeclarationInputData vehicle)
        {
            if (vehicle.Components.AxleGearInputData == null)
            {
                return null;
            }

            return new AxleGearData()
            {
                InputData = vehicle.Components.AxleGearInputData,
            };
        }

        private GearboxData CreateMockupGearboxData(IVehicleDeclarationInputData vehicle)
        {
            if (vehicle.Components.GearboxInputData == null)
            {
                return null;
            }

            return new GearboxData()
            {
                InputData = vehicle.Components.GearboxInputData,
            };
        }


        private CombustionEngineData CreateMockupEngineData(IVehicleDeclarationInputData vehicleData, int modeIdx)
        {

            var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
            if (engine == null)
            {
                return null;
            }
            var engineModes = engine.EngineModes;
            var engineMode = engineModes[modeIdx];
            var fuels = new List<CombustionEngineFuelData>();
            foreach (var fuel in engineMode.Fuels)
            {
                fuels.Add(new CombustionEngineFuelData()
                {
                    FuelData = DeclarationData.FuelData.Lookup(fuel.FuelType, vehicleData.TankSystem)
                });
            }

            return new CombustionEngineData()
            {
                Fuels = fuels,
                RatedPowerDeclared = vehicleData.Components.EngineInputData.RatedPowerDeclared
            };
        }


        private VehicleData CreateMockupVehicleData(IVehicleDeclarationInputData vehicleData)
        {
            return new VehicleData()
            {
                InputData = vehicleData,
                SleeperCab = vehicleData.SleeperCab,
                VehicleClass = _segment.VehicleClass,
                OffVehicleCharging = vehicleData.OvcHev,
                VocationalVehicle = vehicleData.VocationalVehicle,
            };
        }
    }
}
