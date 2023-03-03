using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoMockup.Simulation.RundataFactories
{
	internal class MockupMultistageCompletedBusRunDataFactory
	{
        DeclarationModeCompletedMultistageBusVectoRunDataFactory
		  {
		public MockupMultistageCompletedBusRunDataFactory(IMultistepBusInputDataProvider dataProvider,
            IDeclarationReport report) : base(dataProvider, report)
        {

        }


        #region Overrides of DeclarationModeCompletedMultistageBusVectoRunDataFactory

        protected override void Initialize()
        {

            _segmentCompletedBus = GetCompletedSegment(CompletedVehicle, PrimaryVehicle.AxleConfiguration);

            //base.Initialize();
        }

        protected override IEnumerable<VectoRunData> VectoRunDataHeavyBusCompleted()
        {

            return base.VectoRunDataHeavyBusCompleted();
        }

        protected override VectoRunData CreateVectoRunDataSpecific(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, int modeIdx)
        {
            var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType, _ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));

            var simulationRunData = new VectoRunData
            {
                Loading = loading.Key,
                VehicleData = DataAdapterSpecific.CreateVehicleData(PrimaryVehicle, CompletedVehicle, _segmentCompletedBus,
                    mission, loading),
                Retarder = PrimaryBusMockupRunDataFactory.CreateMockupRetarder(PrimaryVehicle),
                AirdragData = PrimaryBusMockupRunDataFactory.CreateMockupAirdragData(CompletedVehicle),
                EngineData = PrimaryBusMockupRunDataFactory.CreateMockupEngineData(PrimaryVehicle, modeIdx, CompletedVehicle.TankSystem),
                //ElectricMachinesData = PrimaryBusMockupRunDataFactory.CreateMockupElectricMachineData()
                AngledriveData = PrimaryBusMockupRunDataFactory.CreateMockupAngleDriveData(PrimaryVehicle),
                AxleGearData = PrimaryBusMockupRunDataFactory.CreateMockupAxleGearData(PrimaryVehicle),
                Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
                Mission = mission,
                GearboxData = PrimaryBusMockupRunDataFactory.CreateMockupGearboxData(PrimaryVehicle),
                InputData = InputDataProvider,
                SimulationType = SimulationType.DistanceCycle,
                ExecutionMode = ExecutionMode.Declaration,
                JobName = InputDataProvider.JobInputData.ManufacturingStages.Last().Vehicle.Identifier,
                Report = Report,
                //Aux = PrimaryBusMockupRunDataFactory.CreateMockupBusAux(CompletedVehicle),

                //            //AirdragData = DataAdapterSpecific.CreateAirdragData(CompletedVehicle, mission),
                //            //EngineData = DataAdapterSpecific.CreateEngineData(PrimaryVehicle, modeIdx, mission),
                //            //ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
                //            //GearboxData = _gearboxData,
                //            //AxleGearData = _axlegearData,
                //            //AngledriveData = _angledriveData,
                //            Aux = DataAdapterSpecific.CreateAuxiliaryData(PrimaryVehicle.Components.AuxiliaryInputData,
                //                PrimaryVehicle.Components.BusAuxiliaries, mission.MissionType, _segmentCompletedBus.VehicleClass, CompletedVehicle.Length,
                //                PrimaryVehicle.Components.AxleWheels.NumSteeredAxles),
                //Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
                //Retarder = _retarderData,
                ////DriverData = _driverData,
                //ExecutionMode = ExecutionMode.Declaration,
                //JobName = InputDataProvider.JobInputData.ManufacturingStages.Last().Vehicle.Identifier,//?!? Jobname
                ModFileSuffix = $"_{_segmentCompletedBus.VehicleClass.GetClassNumber()}-Specific_{loading.Key}",
                //Report = Report,
                //Mission = mission,
                //InputDataHash = InputDataProvider.XMLHash,// right hash?!?
                //SimulationType = SimulationType.DistanceCycle,
                //VehicleDesignSpeed = _segmentCompletedBus.DesignSpeed,
                //GearshiftParameters = _gearshiftData,
            };
            if (simulationRunData.EngineData != null)
            {
                simulationRunData.EngineData.FuelMode = 0;
            }
            simulationRunData.VehicleData.VehicleClass = _segmentCompletedBus.VehicleClass;
            simulationRunData.BusAuxiliaries = DataAdapterSpecific.CreateBusAuxiliariesData(mission, PrimaryVehicle, CompletedVehicle, simulationRunData);

            return simulationRunData;

            //return base.CreateVectoRunDataSpecific(mission, loading, modeIdx);
        }

        protected override VectoRunData CreateVectoRunDataGeneric(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, Segment primarySegment, int modeIdx)
        {
            var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType, _ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));
            return new VectoRunData()
            {
                Mission = mission,
                Loading = loading.Key,
                VehicleData = new VehicleData()
                {
                    Loading = loading.Value.Item1,

                },
                EngineData = PrimaryBusMockupRunDataFactory.CreateMockupEngineData(PrimaryVehicle, modeIdx, CompletedVehicle.TankSystem),
                JobName = InputDataProvider.JobInputData.ManufacturingStages.Last().Vehicle.Identifier,
                ExecutionMode = ExecutionMode.Declaration,
                SimulationType = SimulationType.DistanceCycle,
                Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
                Report = Report,
                ModFileSuffix = $"_{_segmentCompletedBus.VehicleClass.GetClassNumber()}-Generic_{loading.Key}",
            };
            return base.CreateVectoRunDataGeneric(mission, loading, primarySegment, modeIdx);
        }

        //#endregion
    }
}
}
