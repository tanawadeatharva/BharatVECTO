/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

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
    internal class EngineeringVTPModeVectoRunDataFactory : IVectoRunDataFactory
    {
        protected IVTPInputDataProvider InputDataProvider;

        public EngineeringVTPModeVectoRunDataFactory(IVTPInputDataProvider ivtpProvider)
        {
            InputDataProvider = ivtpProvider;
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


            var auxRD = dao.CreateAuxiliaryData(InputDataProvider.JobInputData.Vehicle.AuxiliaryInputData(), MissionType.RegionalDelivery, segment.VehicleClass).ToList();
			foreach (var entry in auxRD) {
				entry.MissionType = MissionType.RegionalDelivery;
			}
			var auxLH = dao.CreateAuxiliaryData(InputDataProvider.JobInputData.Vehicle.AuxiliaryInputData(), MissionType.LongHaul, segment.VehicleClass).ToList();
			foreach (var entry in auxLH) {
				entry.MissionType = MissionType.LongHaul;
			}
			var auxUD = dao.CreateAuxiliaryData(InputDataProvider.JobInputData.Vehicle.AuxiliaryInputData(), MissionType.UrbanDelivery, segment.VehicleClass).ToList();
			foreach (var entry in auxUD) {
				entry.MissionType = MissionType.UrbanDelivery;
			}

			var aux = new List<VectoRunData.AuxData>();
			aux.AddRange(auxRD);
			aux.AddRange(auxLH);
			aux.AddRange(auxUD);

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
                    Retarder = retarderData,
                    PTO = ptoTransmissionData,
                    Cycle = new DrivingCycleProxy(drivingCycle, cycle.Name),
                    ExecutionMode = ExecutionMode.Engineering,
                    FanData = new AuxFanData() {
						FanCoefficients = InputDataProvider.JobInputData.FanPowerCoefficents.ToArray(),
						FanDiameter = InputDataProvider.JobInputData.FanDiameter,
						},
					SimulationType = SimulationType.VerificationTest
                };
            });
        }
    }
}