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
        public abstract class CompletedBusBase : IVectoRunDataFactory
        {
            private Segment _segmentCompletedBus;
            private AxleGearData _axlegearData;
            private AngledriveData _angledriveData;
            //private GearboxData _gearboxData;
            //private ShiftStrategyParameters _gearshiftData;
            private RetarderData _retarderData;
            private DriverData _driverData
                ;
            public IGenericCompletedBusDeclarationDataAdapter DataAdapterGeneric { get; }
            public ISpecificCompletedBusDeclarationDataAdapter DataAdapterSpecific { get; }
            protected IMultistageVIFInputData DataProvider { get; }
            protected IDeclarationReport Report { get; set; }
            protected IVehicleDeclarationInputData PrimaryVehicle => DataProvider.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle;
            protected IVehicleDeclarationInputData CompletedVehicle => DataProvider.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage.Vehicle;

            public CompletedBusBase(IMultistageVIFInputData dataProvider, IDeclarationReport report,
                ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
                IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric)
            {
                DataAdapterSpecific = dataAdapterSpecific;
                DataAdapterGeneric = dataAdapterGeneric;
                DataProvider = dataProvider;
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

            protected virtual void InitializeReport()
            {
                var powertrainConfig = GetPowertrainConfigForReportInit();
                Report.InitializeReport(powertrainConfig);
            }

            protected virtual VectoRunData GetPowertrainConfigForReportInit()
            {
                return _segmentCompletedBus.Missions.Select(
                        mission => CreateVectoRunDataSpecific(
                            mission, mission.Loadings.First(), 0))
                    .FirstOrDefault(x => x != null);
            }

            protected virtual IEnumerable<VectoRunData> GetNextRun()
            {
                var engineModes = PrimaryVehicle.Components.EngineInputData
                    ?.EngineModes;

                for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
                    var fuelMode = "single fuel mode";
                    if (engineModes[modeIdx].Fuels.Count > 1) {
                        fuelMode = "dual fuel mode";
                    }

                    foreach (var vectoRunData in CreateVectoRunDataForMissions(modeIdx, fuelMode))
                        yield return vectoRunData;
                }
            }

            protected virtual VectoRunData CreateVectoRunDataGeneric(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, Segment primarySegment, int modeIdx)
            {
                var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType, _ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));

                var primaryBusAuxiliaries = PrimaryVehicle.Components.BusAuxiliaries;

                var simulationRunData = new VectoRunData {
                    InputData = DataProvider.MultistageJobInputData,
                    Loading = loading.Key,
                    VehicleData = DataAdapterGeneric.CreateVehicleData(PrimaryVehicle, primarySegment, mission, loading, false),
                    AirdragData = DataAdapterGeneric.CreateAirdragData(null, mission, new Segment()),
                    EngineData = DataAdapterGeneric.CreateEngineData(PrimaryVehicle, modeIdx, mission),
                    ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
                    //GearboxData = _gearboxData,
                    AxleGearData = _axlegearData,
                    AngledriveData = _angledriveData,
                    Aux = DataAdapterGeneric.CreateAuxiliaryData(PrimaryVehicle.Components.AuxiliaryInputData,
                        primaryBusAuxiliaries, mission.MissionType, primarySegment.VehicleClass,
                        mission.BusParameter.VehicleLength,
                        PrimaryVehicle.Components.AxleWheels.NumSteeredAxles, PrimaryVehicle.VehicleType),
                    Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
                    Retarder = _retarderData,
                    DriverData = _driverData,
                    ExecutionMode = ExecutionMode.Declaration,
                    JobName = DataProvider.MultistageJobInputData.JobInputData.ManufacturingStages.Last().Vehicle.Identifier,
                    ModFileSuffix = $"_{_segmentCompletedBus.VehicleClass.GetClassNumber()}-Generic_{loading.Key}",
                    Report = Report,
                    Mission = mission,
                    InputDataHash = DataProvider.MultistageJobInputData.XMLHash,
                    SimulationType = SimulationType.DistanceCycle,
                    VehicleDesignSpeed = _segmentCompletedBus.DesignSpeed,
                    MaxChargingPower = PrimaryVehicle.MaxChargingPower,
                    //GearshiftParameters = _gearshiftData,
                };
                simulationRunData.EngineData.FuelMode = 0;
                simulationRunData.VehicleData.VehicleClass = _segmentCompletedBus.VehicleClass;
                simulationRunData.BusAuxiliaries =
                    DataAdapterGeneric.CreateBusAuxiliariesData(mission, PrimaryVehicle, simulationRunData);
                var shiftStrategyName =
                    PowertrainBuilder.GetShiftStrategyName(PrimaryVehicle.Components.GearboxInputData.Type,
                        PrimaryVehicle.VehicleType);
                simulationRunData.GearboxData = DataAdapterGeneric.CreateGearboxData(PrimaryVehicle, simulationRunData,
                    ShiftPolygonCalculator.Create(shiftStrategyName, simulationRunData.GearshiftParameters));
                simulationRunData.GearshiftParameters =
                    DataAdapterGeneric.CreateGearshiftData(
                        simulationRunData.GearboxData,
                        (simulationRunData.AxleGearData?.AxleGear.Ratio ?? 1.0) * (simulationRunData.AngledriveData?.Angledrive.Ratio ?? 1.0),
                        PrimaryVehicle.EngineIdleSpeed
                    );
                return simulationRunData;
            }
            protected virtual VectoRunData CreateVectoRunDataSpecific(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, int modeIdx)
            {
                var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType, _ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));

                var simulationRunData = new VectoRunData {
                    InputData = DataProvider.MultistageJobInputData,
                    Loading = loading.Key,
                    VehicleData = DataAdapterSpecific.CreateVehicleData(PrimaryVehicle, CompletedVehicle, _segmentCompletedBus,
                        mission, loading),
                    AirdragData = DataAdapterSpecific.CreateAirdragData(CompletedVehicle, mission),
                    EngineData = DataAdapterSpecific.CreateEngineData(PrimaryVehicle, modeIdx, mission),
                    ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
                    //GearboxData = _gearboxData,
                    AxleGearData = _axlegearData,
                    AngledriveData = _angledriveData,
                    Aux = DataAdapterSpecific.CreateAuxiliaryData(PrimaryVehicle.Components.AuxiliaryInputData,
                        PrimaryVehicle.Components.BusAuxiliaries, mission.MissionType, _segmentCompletedBus.VehicleClass, CompletedVehicle.Length,
                        PrimaryVehicle.Components.AxleWheels.NumSteeredAxles, PrimaryVehicle.VehicleType),
                    Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
                    Retarder = _retarderData,
                    DriverData = _driverData,
                    ExecutionMode = ExecutionMode.Declaration,
                    JobName = DataProvider.MultistageJobInputData.JobInputData.ManufacturingStages.Last().Vehicle.Identifier,//?!? Jobname
                    ModFileSuffix = $"_{_segmentCompletedBus.VehicleClass.GetClassNumber()}-Specific_{loading.Key}",
                    Report = Report,
                    Mission = mission,
                    InputDataHash = DataProvider.MultistageJobInputData.XMLHash,// right hash?!?
                    SimulationType = SimulationType.DistanceCycle,
                    VehicleDesignSpeed = _segmentCompletedBus.DesignSpeed,
                    MaxChargingPower = PrimaryVehicle.MaxChargingPower,
                    //GearshiftParameters = _gearshiftData,
                };
                simulationRunData.EngineData.FuelMode = 0;
                simulationRunData.VehicleData.VehicleClass = _segmentCompletedBus.VehicleClass;
                simulationRunData.BusAuxiliaries = DataAdapterSpecific.CreateBusAuxiliariesData(mission, PrimaryVehicle, CompletedVehicle, simulationRunData);

                var shiftStrategyName =
                    PowertrainBuilder.GetShiftStrategyName(PrimaryVehicle.Components.GearboxInputData.Type,
                        PrimaryVehicle.VehicleType);
                simulationRunData.GearboxData = DataAdapterGeneric.CreateGearboxData(PrimaryVehicle, simulationRunData,
                    ShiftPolygonCalculator.Create(shiftStrategyName, simulationRunData.GearshiftParameters));
                simulationRunData.GearshiftParameters =
                    DataAdapterGeneric.CreateGearshiftData(
                        simulationRunData.GearboxData,
                        (simulationRunData.AxleGearData?.AxleGear.Ratio ?? 1.0) * (simulationRunData.AngledriveData?.Angledrive.Ratio ?? 1.0),
                        PrimaryVehicle.EngineIdleSpeed
                    );
                return simulationRunData;
            }
            protected virtual Segment GetPrimarySegment(IVehicleDeclarationInputData primaryVehicle)
            {
                var primarySegment = DeclarationData.PrimaryBusSegments.Lookup(
                    primaryVehicle.VehicleCategory, primaryVehicle.AxleConfiguration, primaryVehicle.Articulated);

                return primarySegment;
            }


            private IEnumerable<VectoRunData> CreateVectoRunDataForMissions(int modeIdx, string fuelMode)
            {
                foreach (var mission in _segmentCompletedBus.Missions) {
                    foreach (var loading in mission.Loadings) {
                        var simulationRunData = CreateVectoRunDataSpecific(mission, loading, modeIdx);
                        if (simulationRunData != null) {
                            yield return simulationRunData;
                        }

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

                        simulationRunData.PrimaryResult = primaryResult;

                        yield return simulationRunData;
                    }
                }
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
            protected virtual void Initialize()
            {
                if (CompletedVehicle.ExemptedVehicle || PrimaryVehicle.ExemptedVehicle) {
                    return;
                }

                _segmentCompletedBus = GetCompletedSegment(CompletedVehicle, PrimaryVehicle.AxleConfiguration);

                //var combustionEngineData = DataAdapterGeneric.CreateEngineData(PrimaryVehicle, 0, _segmentCompletedBus.Missions.First());

                _axlegearData = DataAdapterGeneric.CreateAxleGearData(PrimaryVehicle.Components.AxleGearInputData);

                _angledriveData = DataAdapterGeneric.CreateAngledriveData(PrimaryVehicle.Components.AngledriveInputData);

                //var tmpRunData = new VectoRunData()
                //{
                //	GearboxData = new GearboxData()
                //	{
                //		Type = PrimaryVehicle.Components.GearboxInputData.Type,
                //	}
                //};
                //var tmpStrategy = PowertrainBuilder.GetShiftStrategy(new SimplePowertrainContainer(tmpRunData));

                //_gearboxData = DataAdapterGeneric.CreateGearboxData(PrimaryVehicle, new VectoRunData() { EngineData = combustionEngineData, AxleGearData = _axlegearData, VehicleData = tmpVehicleData },
                //	tmpStrategy);

                //_gearshiftData = DataAdapterGeneric.CreateGearshiftData(
                //	_gearboxData, _axlegearData.AxleGear.Ratio * (_angledriveData?.Angledrive.Ratio ?? 1.0), combustionEngineData.IdleSpeed);

                _retarderData = DataAdapterGeneric.CreateRetarderData(PrimaryVehicle.Components.RetarderInputData);


                _driverData = DataAdapterGeneric.CreateDriverData(_segmentCompletedBus);
                //_driverData.AccelerationCurve = AccelerationCurveReader.ReadFromStream(_segmentCompletedBus.AccelerationFile);
            }


        }

        public class Conventional :
            CompletedBusBase
        {
            public Conventional(IMultistageVIFInputData dataProvider, IDeclarationReport report,
                ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
                IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific,
                dataAdapterGeneric)
            { }
        }
        public class HEV_S2 : CompletedBusBase
        {
            public HEV_S2(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        public class HEV_S3 : CompletedBusBase
        {
            public HEV_S3(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        public class HEV_S4 : CompletedBusBase
        {
            public HEV_S4(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        public class HEV_S_IEPC : CompletedBusBase
        {
            public HEV_S_IEPC(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        public class HEV_P1 : CompletedBusBase
        {
            public HEV_P1(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        public class HEV_P2 : CompletedBusBase
        {
            public HEV_P2(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        public class HEV_P2_5 : CompletedBusBase
        {
            public HEV_P2_5(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        public class HEV_P3 : CompletedBusBase
        {
            public HEV_P3(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        public class HEV_P4 : CompletedBusBase
        {
            public HEV_P4(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        public class PEV_E1 : CompletedBusBase
        {
            public PEV_E1(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        public class PEV_E2 : CompletedBusBase
        {
            public PEV_E2(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        public class PEV_E3 : CompletedBusBase
        {
            public PEV_E3(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        public class PEV_E4 : CompletedBusBase
        {
            public PEV_E4(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }
        public class PEV_E_IEPC : CompletedBusBase
        {
            public PEV_E_IEPC(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }
        }

        public class Exempted : CompletedBusBase
        {
            public Exempted(IMultistageVIFInputData dataProvider, IDeclarationReport report, ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific, IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric) : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric) { }

            protected override IEnumerable<VectoRunData> GetNextRun()
            {
                return new[] { GetPowertrainConfigForReportInit() };
            }

            protected override VectoRunData GetPowertrainConfigForReportInit()
            {
                return new VectoRunData() {
                    Exempted = true,
                    VehicleData = new VehicleData() {
                        ModelName = CompletedVehicle.Model,
                        Manufacturer = CompletedVehicle.Manufacturer,
                        ManufacturerAddress = CompletedVehicle.ManufacturerAddress,
                        VIN = CompletedVehicle.VIN,
                        LegislativeClass = CompletedVehicle.LegislativeClass,
                        RegisteredClass = CompletedVehicle.RegisteredClass,
                        VehicleCode = CompletedVehicle.VehicleCode,
                        VehicleCategory = VehicleCategory.HeavyBusCompletedVehicle,
                        CurbMass = CompletedVehicle.CurbMassChassis,
                        GrossVehicleMass = CompletedVehicle.GrossVehicleMassRating,
                        ZeroEmissionVehicle = PrimaryVehicle.ZeroEmissionVehicle,
                        MaxNetPower1 = PrimaryVehicle.MaxNetPower1,
                        InputData = CompletedVehicle
                    },
                    Report = Report,
                    Mission = new Mission() {
                        MissionType = MissionType.ExemptedMission
                    },
                    InputData = DataProvider.MultistageJobInputData
                };
            }
        }
    }
}