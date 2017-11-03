using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.InputData.Reader.Impl {
    internal class EngineeringEPTPModeVectoRunDataFactory : IVectoRunDataFactory
    {
        protected IEPTPInputDataProvider InputDataProvider;

        public EngineeringEPTPModeVectoRunDataFactory(IEPTPInputDataProvider eptpProvider)
        {
            InputDataProvider = eptpProvider;
        }

        public IEnumerable<VectoRunData> NextRun()
        {
            var dao = new DeclarationDataAdapter();
            var segment = DeclarationData.Segments.Lookup(InputDataProvider.JobInputData.Vehicle.VehicleCategory,
                InputDataProvider.JobInputData.Vehicle.AxleConfiguration,
                InputDataProvider.JobInputData.Vehicle.GrossVehicleMassRating,
                InputDataProvider.JobInputData.Vehicle.CurbMassChassis);
            var driverdata = dao.CreateDriverData();
            driverdata.AccelerationCurve = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
            var tempVehicle = dao.CreateVehicleData(InputDataProvider.JobInputData.Vehicle, segment.Missions.First(),
                segment.Missions.First().Loadings.First().Value, segment.MunicipalBodyWeight);
            var airdragData = dao.CreateAirdragData(InputDataProvider.JobInputData.Vehicle.AirdragInputData,
                segment.Missions.First(), segment);
            var engineData = dao.CreateEngineData(InputDataProvider.JobInputData.Vehicle.EngineInputData,
                InputDataProvider.JobInputData.Vehicle.EngineIdleSpeed,
                InputDataProvider.JobInputData.Vehicle.GearboxInputData, InputDataProvider.JobInputData.Vehicle.TorqueLimits);
            var axlegearData = dao.CreateAxleGearData(InputDataProvider.JobInputData.Vehicle.AxleGearInputData, false);
            var angledriveData = dao.CreateAngledriveData(InputDataProvider.JobInputData.Vehicle.AngledriveInputData, false);
            var gearboxData = dao.CreateGearboxData(InputDataProvider.JobInputData.Vehicle.GearboxInputData, engineData,
                axlegearData.AxleGear.Ratio,
                tempVehicle.DynamicTyreRadius, tempVehicle.VehicleCategory, false);
            var retarderData = dao.CreateRetarderData(InputDataProvider.JobInputData.Vehicle.RetarderInputData);

            var ptoTransmissionData = dao.CreatePTOTransmissionData(InputDataProvider.JobInputData.Vehicle.PTOTransmissionInputData);


            var aux = dao.CreateAuxiliaryData(InputDataProvider.JobInputData.Vehicle.AuxiliaryInputData(), MissionType.RegionalDelivery, segment.VehicleClass).ToList();
            aux.RemoveAll(x => x.ID == Constants.Auxiliaries.IDs.Fan);
            aux.Add(new VectoRunData.AuxData {
                DemandType = AuxiliaryDemandType.Direct,
                ID = DrivingCycleDataReader.Fields.AdditionalAuxPowerDemand
            });

            return InputDataProvider.JobInputData.Cycles.Select(cycle => {
                var drivingCycle = DrivingCycleDataReader.ReadFromDataTable(cycle.CycleData, cycle.Name, false);
                return new VectoRunData {
                    JobName = InputDataProvider.JobInputData.Vehicle.VIN,
                    EngineData = engineData,
                    GearboxData = gearboxData,
                    AxleGearData = axlegearData,
                    AngledriveData = angledriveData,
                    VehicleData = dao.CreateVehicleData(InputDataProvider.JobInputData.Vehicle, segment.Missions.First(),
                        0.SI<Kilogram>(), segment.MunicipalBodyWeight),
                    AirdragData =airdragData,
                    DriverData = null,
                    Aux = aux,
                    AdvancedAux = null,
                    Retarder = dao.CreateRetarderData(InputDataProvider.JobInputData.Vehicle.RetarderInputData),
                    PTO = ptoTransmissionData,
                    Cycle = new DrivingCycleProxy(drivingCycle, cycle.Name),
                    ExecutionMode = ExecutionMode.Engineering
                };
            });
        }
    }
}