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
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	internal class DeclarationVTPModeVectoRunDataFactory : IVectoRunDataFactory
	{
		protected IVTPDeclarationJobInputData JobInputData;
		private DriverData _driverdata;
		private AirdragData _airdragData;
		private CombustionEngineData _engineData;
		private AxleGearData _axlegearData;
		private AngledriveData _angledriveData;
		private GearboxData _gearboxData;
		private RetarderData _retarderData;
		private PTOData _ptoTransmissionData;
		protected List<VectoRunData.AuxData> _auxVTP;
		protected Segment _segment;
		private DeclarationDataAdapter _dao;
		protected Exception _initException;

		public IVTPReport Report;

		public DeclarationVTPModeVectoRunDataFactory(IVTPDeclarationInputDataProvider ivtpProvider, IVTPReport report) : this(
			ivtpProvider.JobInputData, report) { }

		protected DeclarationVTPModeVectoRunDataFactory(IVTPDeclarationJobInputData job, IVTPReport report)
		{
			JobInputData = job;
			Report = report;
			try {
				Initialize();
				if (Report != null) {
					InitializeReport();
				}
			} catch (Exception e) {
				_initException = e;
			}
		}

		private void InitializeReport()
		{
			var powertrainConfig = new VectoRunData() {
				VehicleData =
					_dao.CreateVehicleData(
						JobInputData.Vehicle, _segment.Missions.First(),
						_segment.Missions.First().Loadings.First().Value, _segment.MunicipalBodyWeight),
				AirdragData = _airdragData,
				EngineData = _engineData,
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				Retarder = _retarderData,
				Aux =
					_dao.CreateAuxiliaryData(
						JobInputData.Vehicle.AuxiliaryInputData(),
						_segment.Missions.First().MissionType,
						_segment.VehicleClass),
			};
			powertrainConfig.VehicleData.VehicleClass = _segment.VehicleClass;
			Report.InputDataHash = JobInputData.VectoJobHash;
			Report.ManufacturerRecord = JobInputData.ManufacturerReportInputData;
			Report.ManufacturerRecordHash = JobInputData.VectoManufacturerReportHash;
			Report.InitializeReport(powertrainConfig);
		}


		protected void Initialize()
		{
			_dao = new DeclarationDataAdapter();
			var vehicle = JobInputData.Vehicle;
			_segment = DeclarationData.Segments.Lookup(
				vehicle.VehicleCategory,
				vehicle.AxleConfiguration,
				vehicle.GrossVehicleMassRating,
				vehicle.CurbMassChassis);
			_driverdata = _dao.CreateDriverData();
			_driverdata.AccelerationCurve = AccelerationCurveReader.ReadFromStream(_segment.AccelerationFile);
			var tempVehicle = _dao.CreateVehicleData(
				vehicle, _segment.Missions.First(),
				_segment.Missions.First().Loadings.First().Value, _segment.MunicipalBodyWeight);
			_airdragData = _dao.CreateAirdragData(
				vehicle.AirdragInputData,
				_segment.Missions.First(), _segment);
			_engineData = _dao.CreateEngineData(
				vehicle.EngineInputData,
				vehicle.EngineIdleSpeed,
				vehicle.GearboxInputData, vehicle.TorqueLimits);
			_axlegearData = _dao.CreateAxleGearData(vehicle.AxleGearInputData);
			_angledriveData = _dao.CreateAngledriveData(vehicle.AngledriveInputData);
			_gearboxData = _dao.CreateGearboxData(
				vehicle.GearboxInputData, _engineData,
				_axlegearData.AxleGear.Ratio,
				tempVehicle.DynamicTyreRadius, tempVehicle.VehicleCategory);
			_retarderData = _dao.CreateRetarderData(vehicle.RetarderInputData);

			_ptoTransmissionData =
				_dao.CreatePTOTransmissionData(vehicle.PTOTransmissionInputData);

			_auxVTP = CreateVTPAuxData(_dao, vehicle, _segment);
		}

		#region Implementation of IVectoRunDataFactory

		public virtual IEnumerable<VectoRunData> NextRun()
		{
			if (_initException != null) {
				throw _initException;
			}

			// simulate the LongHaul cycle with RefLoad
			var mission = _segment.Missions.FirstOrDefault(m => m.MissionType == DeclarationData.VTPMode.SelectedMission);
			if (mission == null) {
				throw new VectoException("Mission {0} not found in segmentation matrix", DeclarationData.VTPMode.SelectedMission);
			}
			var loading = mission.Loadings.FirstOrDefault(l => l.Key == DeclarationData.VTPMode.SelectedLoading);
			var runData = CreateVectoRunData(_segment, mission, loading.Value);
			runData.ModFileSuffix = loading.Key.ToString();
			var cycle = DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false);
			runData.Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString());
			runData.DriverData = _driverdata;
			runData.Aux = _dao.CreateAuxiliaryData(
				JobInputData.Vehicle.AuxiliaryInputData(), mission.MissionType, _segment.VehicleClass);
			runData.ExecutionMode = ExecutionMode.Declaration;
			runData.SimulationType = SimulationType.DistanceCycle;
			runData.Mission = mission;
			runData.Loading = loading.Key;
			yield return runData;
				
			
			// simulate the Measured cycle
			var vtpCycle = JobInputData.Cycles.FirstOrDefault();
			if (vtpCycle == null) {
				throw new VectoException("no VTP-Cycle provided!");
			}
			var drivingCycle = DrivingCycleDataReader.ReadFromDataTable(vtpCycle.CycleData, vtpCycle.Name, false);

			// Loading is not relevant as we use P_wheel
			var vtpRunData = CreateVectoRunData(_segment, _segment.Missions.First(), 0.SI<Kilogram>());
			vtpRunData.Cycle = new DrivingCycleProxy(drivingCycle, vtpCycle.Name);
			vtpRunData.Aux = _auxVTP;
			vtpRunData.FanData = new AuxFanData() {
				FanCoefficients = JobInputData.FanPowerCoefficents.ToArray(),
				FanDiameter = JobInputData.FanDiameter,
			};
			vtpRunData.ExecutionMode = ExecutionMode.Declaration;
			vtpRunData.SimulationType = SimulationType.VerificationTest;
			vtpRunData.Mission = new Mission() {
				MissionType = MissionType.VerificationTest
			};
			var ncvStd = DeclarationData.FuelData.Lookup(JobInputData.Vehicle.EngineInputData.FuelType).LowerHeatingValue;
			var ncvCorrection = ncvStd / JobInputData.NetCalorificValueTestFuel;
			var mileageCorrection = GetMileagecorrectionFactor(JobInputData.Mileage);
			vtpRunData.VTPData = new VTPData() {
				CorrectionFactor = ncvCorrection * mileageCorrection,
				FuelNetCalorificValue = JobInputData.NetCalorificValueTestFuel //0.SI<JoulePerKilogramm>()
			};
			yield return vtpRunData;
			
		}

		private double GetMileagecorrectionFactor(Meter mileage)
		{
			if (mileage > DeclarationData.VTPMode.RunInThreshold) {
				return 1;
			}

			return DeclarationData.VTPMode.EvolutionCoefficient + (1 - DeclarationData.VTPMode.EvolutionCoefficient) * mileage /
					DeclarationData.VTPMode.RunInThreshold;
		}

		protected VectoRunData CreateVectoRunData(Segment segment, Mission mission, Kilogram loading)
		{
			return new VectoRunData {
				JobName = JobInputData.Vehicle.VIN,
				EngineData = _engineData,
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				AngledriveData = _angledriveData,
				VehicleData = _dao.CreateVehicleData(
					JobInputData.Vehicle, mission,
					loading, segment.MunicipalBodyWeight),
				AirdragData = _airdragData,
				DriverData = null,
				AdvancedAux = null,
				Retarder = _retarderData,
				PTO = _ptoTransmissionData,
				Report = Report,
			};
		}

		protected virtual List<VectoRunData.AuxData> CreateVTPAuxData(
			DeclarationDataAdapter dao, IVehicleDeclarationInputData vehicle, Segment segment)
		{
			var auxRD = dao.CreateAuxiliaryData(
								vehicle.AuxiliaryInputData(), MissionType.RegionalDelivery, segment.VehicleClass)
							.ToList();
			foreach (var entry in auxRD) {
				entry.MissionType = MissionType.RegionalDelivery;
			}

			var auxLH = dao.CreateAuxiliaryData(
								vehicle.AuxiliaryInputData(), MissionType.LongHaul, segment.VehicleClass)
							.ToList();
			foreach (var entry in auxLH) {
				entry.MissionType = MissionType.LongHaul;
			}

			var auxUD = dao.CreateAuxiliaryData(
								vehicle.AuxiliaryInputData(), MissionType.UrbanDelivery, segment.VehicleClass)
							.ToList();
			foreach (var entry in auxUD) {
				entry.MissionType = MissionType.UrbanDelivery;
			}

			var aux = new List<VectoRunData.AuxData>();
			aux.AddRange(auxRD);
			aux.AddRange(auxLH);
			aux.AddRange(auxUD);

			aux.RemoveAll(x => x.ID == Constants.Auxiliaries.IDs.Fan);
			aux.Add(
				new VectoRunData.AuxData {
					DemandType = AuxiliaryDemandType.Direct,
					ID = DrivingCycleDataReader.Fields.AdditionalAuxPowerDemand
				});
			return aux;
		}

		#endregion
	}
}
