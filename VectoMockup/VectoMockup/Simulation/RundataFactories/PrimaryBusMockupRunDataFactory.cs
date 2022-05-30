using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoMockup.Simulation.RundataFactories
{
    public class PrimaryBusMockupRunDataFactory : DeclarationModePrimaryBusVectoRunDataFactory
    {
        public PrimaryBusMockupRunDataFactory(IDeclarationInputDataProvider dataProvider,
            IDeclarationReport report) :
            base(dataProvider, report)
        { }

        #region Overrides of AbstractDeclarationVectoRunDataFactory

        protected override void Initialize()
        {
            _segment = GetSegment(InputDataProvider.JobInputData.Vehicle);
        }

        protected override void InitializeReport()
        {
            if (InputDataProvider.JobInputData.JobType == VectoSimulationJobType.ConventionalVehicle)
            {
                base.InitializeReport();
                return;
            }

            VectoRunData powertrainConfig;
            List<List<FuelData.Entry>> fuels;
            var vehicle = InputDataProvider.JobInputData.Vehicle;
            if (vehicle.ExemptedVehicle)
            {
                powertrainConfig = CreateVectoRunData(vehicle, 0, null,
                    new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>());
                fuels = new List<List<FuelData.Entry>>();
            }
            else
            {
                powertrainConfig = _segment.Missions.Select(
                        mission => CreateVectoRunData(
                            vehicle, 0, mission, mission.Loadings.First()))
                    .FirstOrDefault(x => x != null);
                fuels = null;
            }

            Report.InitializeReport(powertrainConfig, fuels);
        }

        #region Overrides of DeclarationModePrimaryBusVectoRunDataFactory

        protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, int modeIdx,
            Mission mission,
            KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
        {

            VectoRunData runData;
            if (InputDataProvider.JobInputData.Vehicle.ExemptedVehicle)
            {
                throw new NotImplementedException();
            }
            else
            {
                var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType,
                    _ =>
                    {
                        return DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "",
                            false);
                    });


                runData = new VectoRunData()
                {
                    Loading = loading.Key,
                    Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
                    ExecutionMode = ExecutionMode.Declaration,
                    Report = Report,
                    Mission = mission,
                    SimulationType = SimulationType.DistanceCycle,
                    VehicleData = CreateMockupVehicleData(vehicle),
                    Retarder = CreateMockupRetarder(vehicle),
                    AxleGearData = CreateMockupAxleGearData(vehicle),
                    GearboxData = CreateMockupGearboxData(vehicle),
                    AngledriveData = CreateMockupAngleDriveData(vehicle),
					EngineData = CreateMockupEngineData(vehicle, modeIdx),
                    BusAuxiliaries = CreateMockupBusAux(vehicle),

                };
            }

            runData.InputData = InputDataProvider;

            return runData;
        }



        #endregion

        #endregion
        private IAuxiliaryConfig CreateMockupBusAux(IVehicleDeclarationInputData vehicle)
        {
            return new AuxiliaryConfig()
            {
                InputData = vehicle.Components.BusAuxiliaries,

            };
        }
        private RetarderData CreateMockupRetarder(IVehicleDeclarationInputData vehicle)
        {
            var xmlVehicle = vehicle as IXMLDeclarationVehicleData;
            return new RetarderData()
            {
                Type = xmlVehicle.RetarderType,

                Ratio = xmlVehicle.RetarderType.IsDedicatedComponent() ? xmlVehicle.RetarderRatio : 0,
            };
        }

        private AngledriveData CreateMockupAngleDriveData(IVehicleDeclarationInputData vehicle)
        {
            if (vehicle.Components.AngledriveInputData == null)
            {
                return null;
            }

			var componentData = vehicle.Components.AngledriveInputData;
            return new AngledriveData()
            {
                InputData = vehicle.Components.AngledriveInputData,


			

                Angledrive = new TransmissionData() {
                    Ratio = componentData.Ratio,
				},

				Manufacturer = componentData.Manufacturer,
				ModelName = componentData.Model,
				CertificationNumber = componentData.CertificationNumber,
				Date = componentData.Date,
            };
        }

        private AxleGearData CreateMockupAxleGearData(IVehicleDeclarationInputData vehicle)
        {
            if (vehicle.Components.AxleGearInputData == null)
            {
                return null;
            }

			var componentData = vehicle.Components.AxleGearInputData;
            return new AxleGearData()
            {
                InputData = vehicle.Components.AxleGearInputData,

				Manufacturer = componentData.Manufacturer,
				ModelName = componentData.Model,
				CertificationNumber = componentData.CertificationNumber,
				Date = componentData.Date,
                AxleGear = new TransmissionData()
                {
                    Ratio = vehicle.Components.AxleGearInputData.Ratio,

				}
            };
        }

        private GearboxData CreateMockupGearboxData(IVehicleDeclarationInputData vehicle)
        {
            if (vehicle.Components.GearboxInputData == null)
            {
                return null;
            }

			var componentData = vehicle.Components.GearboxInputData;
			var gears = new Dictionary<uint, GearData>();
            foreach (var gearInputData in componentData.Gears) {
				gears.Add((uint)gearInputData.Gear, new GearData() {
                    Ratio = gearInputData.Ratio,
                    MaxTorque = gearInputData.MaxTorque,
                    MaxSpeed = gearInputData.MaxInputSpeed,
				});
			}
				

			return new GearboxData()
            {
                InputData = vehicle.Components.GearboxInputData,

				Manufacturer = componentData.Manufacturer,
				ModelName = componentData.Model,
				CertificationNumber = componentData.CertificationNumber,
				Date = componentData.Date,
                Gears = gears,

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

			var componentData = vehicleData.Components.EngineInputData;
            return new CombustionEngineData()
            {
                Fuels = fuels,
                RatedPowerDeclared = vehicleData.Components.EngineInputData.RatedPowerDeclared,
                IdleSpeed = vehicleData.EngineIdleSpeed,
                InputData = vehicleData.Components.EngineInputData,
                WHRType = vehicleData.Components.EngineInputData.WHRType,
                RatedSpeedDeclared = engine.RatedSpeedDeclared,
                Displacement = engine.Displacement,

                Manufacturer = componentData.Manufacturer,
                ModelName = componentData.Model,
                CertificationNumber = componentData.CertificationNumber,
                Date = componentData.Date,


            };
        }

        private VehicleData CreateMockupVehicleData(IVehicleDeclarationInputData vehicleData)
        {
            return new VehicleData()
            {

                InputData = vehicleData,
                SleeperCab = vehicleData.SleeperCab,
                VehicleClass = _segment.VehicleClass,
                Ocv = vehicleData.OvcHev,
                VehicleCategory = vehicleData.VehicleCategory,
                ZeroEmissionVehicle = vehicleData.ZeroEmissionVehicle,
                ADAS = CreateMockupAdasData(vehicleData),

                Manufacturer = vehicleData.Manufacturer,
                ManufacturerAddress = vehicleData.ManufacturerAddress,
                ModelName = vehicleData.Model,
                VIN = vehicleData.VIN,
                LegislativeClass = vehicleData.LegislativeClass,
                AxleConfiguration = vehicleData.AxleConfiguration,
                Date = vehicleData.Date,
			};
        }

        private VehicleData.ADASData CreateMockupAdasData(IVehicleDeclarationInputData vehicleData)
        {
            var adas = vehicleData.ADAS;
            return new VehicleData.ADASData()
            {
                EcoRoll = adas.EcoRoll,
                EngineStopStart = adas.EngineStopStart,
                InputData = adas,
                PredictiveCruiseControl = adas.PredictiveCruiseControl,
            };
        }
    }
}
