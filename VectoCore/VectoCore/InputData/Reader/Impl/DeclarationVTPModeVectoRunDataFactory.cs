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

		public DeclarationVTPModeVectoRunDataFactory(IVTPDeclarationInputDataProvider ivtpProvider, IVTPReport report) : this(ivtpProvider.JobInputData, report)
		{}

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
					_dao.CreateVehicleData(JobInputData.Vehicle, _segment.Missions.First(),
											_segment.Missions.First().Loadings.First().Value, _segment.MunicipalBodyWeight),
				AirdragData = _airdragData,
				EngineData = _engineData,
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				Retarder = _retarderData,
				Aux =
					_dao.CreateAuxiliaryData(JobInputData.Vehicle.AuxiliaryInputData(),
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
			_axlegearData = _dao.CreateAxleGearData(vehicle.AxleGearInputData, false);
			_angledriveData = _dao.CreateAngledriveData(vehicle.AngledriveInputData, false);
			_gearboxData = _dao.CreateGearboxData(
				vehicle.GearboxInputData, _engineData,
				_axlegearData.AxleGear.Ratio,
				tempVehicle.DynamicTyreRadius, tempVehicle.VehicleCategory, false);
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
			foreach (var mission in _segment.Missions.Where(m => m.MissionType == DeclarationData.VTPMode.SelectedMission)) {
				foreach (var loading in mission.Loadings.Where(l => l.Key == DeclarationData.VTPMode.SelectedLoading)) {
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
				}
			}

			// simulate the Measured cycle
			foreach (var cycle in JobInputData.Cycles) {
				var drivingCycle = DrivingCycleDataReader.ReadFromDataTable(cycle.CycleData, cycle.Name, false);
				// Loading is not relevant as we use P_wheel
				var runData = CreateVectoRunData(_segment, _segment.Missions.First(), 0.SI<Kilogram>());
				runData.Cycle = new DrivingCycleProxy(drivingCycle, cycle.Name);
				runData.Aux = _auxVTP;
				runData.FanData = new AuxFanData() {
					FanCoefficients = JobInputData.FanPowerCoefficents.ToArray(),
					FanDiameter = JobInputData.FanDiameter,
				};
				runData.ExecutionMode = ExecutionMode.Declaration;
				runData.SimulationType = SimulationType.VerificationTest;
				runData.Mission = new Mission() {
					MissionType = MissionType.VerificationTest
				};
				runData.VTPData = new VTPData() {
					CorrectionFactor = 1,
					FuelNetCalorificValue = 0.SI<JoulePerKilogramm>()
				};
				yield return runData;
			}

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

		protected virtual List<VectoRunData.AuxData> CreateVTPAuxData(DeclarationDataAdapter dao, IVehicleDeclarationInputData vehicle, Segment segment)
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
