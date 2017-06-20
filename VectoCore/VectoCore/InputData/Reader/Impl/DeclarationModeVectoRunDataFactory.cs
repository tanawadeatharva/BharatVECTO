/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
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

using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;


namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public class DeclarationModeVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{
		private static readonly object CyclesCacheLock = new object();

		private static readonly Dictionary<MissionType, DrivingCycleData> CyclesCache =
			new Dictionary<MissionType, DrivingCycleData>();

		protected IDeclarationInputDataProvider InputDataProvider;

		protected IDeclarationReport Report;
		private DeclarationDataAdapter _dao;
		private Segment _segment;
		private DriverData _driverdata;
		private AirdragData _airdragData;
		private CombustionEngineData _engineData;
		private AxleGearData _axlegearData;
		private AngledriveData _angledriveData;
		private GearboxData _gearboxData;
		private RetarderData _retarderData;
		private PTOData _ptoTransmissionData;
		private PTOData _municipalPtoTransmissionData;

		internal DeclarationModeVectoRunDataFactory(IDeclarationInputDataProvider dataProvider, IDeclarationReport report)
		{
			InputDataProvider = dataProvider;
			Report = report;

			Initialize();
			if (Report != null) {
				InitializeReport();
			}
		}
		
		private void Initialize()
		{
			_dao = new DeclarationDataAdapter();
			_segment = GetVehicleClassification(InputDataProvider.VehicleInputData.VehicleCategory,
				InputDataProvider.VehicleInputData.AxleConfiguration,
				InputDataProvider.VehicleInputData.GrossVehicleMassRating, InputDataProvider.VehicleInputData.CurbMassChassis);
			_driverdata = _dao.CreateDriverData(InputDataProvider.DriverInputData);
			_driverdata.AccelerationCurve = AccelerationCurveReader.ReadFromStream(_segment.AccelerationFile);

			var tempVehicle = _dao.CreateVehicleData(InputDataProvider.VehicleInputData, _segment.Missions.First(),
				_segment.Missions.First().Loadings.First().Value, _segment.MunicipalBodyWeight);
			_airdragData = _dao.CreateAirdragData(InputDataProvider.AirdragInputData, _segment.Missions.First(), _segment);
			_engineData = _dao.CreateEngineData(InputDataProvider.EngineInputData,
				InputDataProvider.VehicleInputData.EngineIdleSpeed,
				InputDataProvider.GearboxInputData, InputDataProvider.VehicleInputData.TorqueLimits);
			_axlegearData = _dao.CreateAxleGearData(InputDataProvider.AxleGearInputData, false);
			_angledriveData = _dao.CreateAngledriveData(InputDataProvider.AngledriveInputData, false);
			_gearboxData = _dao.CreateGearboxData(InputDataProvider.GearboxInputData, _engineData, _axlegearData.AxleGear.Ratio,
				tempVehicle.DynamicTyreRadius, false);
			_retarderData = _dao.CreateRetarderData(InputDataProvider.RetarderInputData);

			_ptoTransmissionData = _dao.CreatePTOTransmissionData(InputDataProvider.PTOTransmissionInputData);

			_municipalPtoTransmissionData = CreateDefaultPTOData();
		}

		private void InitializeReport()
		{
			var powertrainConfig = new VectoRunData() {
				VehicleData =
					_dao.CreateVehicleData(InputDataProvider.VehicleInputData, _segment.Missions.First(),
						_segment.Missions.First().Loadings.First().Value, _segment.MunicipalBodyWeight),
				AirdragData = _airdragData,
				EngineData = _engineData,
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				Retarder = _retarderData,
				Aux = _dao.CreateAuxiliaryData(InputDataProvider.AuxiliaryInputData(), _segment.Missions.First().MissionType,
					_segment.VehicleClass),
				InputDataHash = InputDataProvider.XMLHash
			};
			Report.InitializeReport(powertrainConfig, _segment);
		}

		public IEnumerable<VectoRunData> NextRun()
		{
			foreach (var mission in _segment.Missions) {
				if (mission.MissionType.IsEMS() &&
					_engineData.RatedPowerDeclared.IsSmaller(DeclarationData.MinEnginePowerForEMS)) {
					continue;
				}
				DrivingCycleData cycle;
				lock (CyclesCacheLock) {
					if (CyclesCache.ContainsKey(mission.MissionType)) {
						cycle = CyclesCache[mission.MissionType];
					} else {
						cycle = DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false);
						CyclesCache.Add(mission.MissionType, cycle);
					}
				}
				foreach (var loading in mission.Loadings) {
					var simulationRunData = new VectoRunData {
						Loading = loading.Key,
						VehicleData =
							_dao.CreateVehicleData(InputDataProvider.VehicleInputData, mission, loading.Value, _segment.MunicipalBodyWeight),
						AirdragData = _dao.CreateAirdragData(InputDataProvider.AirdragInputData, mission, _segment),
						EngineData = _engineData.Copy(),
						GearboxData = _gearboxData,
						AxleGearData = _axlegearData,
						AngledriveData = _angledriveData,
						Aux = _dao.CreateAuxiliaryData(InputDataProvider.AuxiliaryInputData(), mission.MissionType,
							_segment.VehicleClass),
						Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
						Retarder = _retarderData,
						DriverData = _driverdata,
						ExecutionMode = ExecutionMode.Declaration,
						JobName = InputDataProvider.JobInputData().JobName,
						ModFileSuffix = loading.Key.ToString(),
						Report = Report,
						Mission = mission,
						PTO = mission.MissionType == MissionType.MunicipalUtility
							? _municipalPtoTransmissionData
							: _ptoTransmissionData,
						InputDataHash = InputDataProvider.XMLHash
					};
					simulationRunData.EngineData.FuelConsumptionCorrectionFactor = DeclarationData.WHTCCorrection.Lookup(
						mission.MissionType.GetNonEMSMissionType(), _engineData.WHTCRural, _engineData.WHTCUrban, _engineData.WHTCMotorway) *
																					_engineData.ColdHotCorrectionFactor;
					simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
					yield return simulationRunData;
				}
			}
		}

		private PTOData CreateDefaultPTOData()
		{
			return new PTOData() {
				TransmissionType = DeclarationData.PTO.DefaultPTOTechnology,
				LossMap = PTOIdleLossMapReader.ReadFromStream(RessourceHelper.ReadStream(DeclarationData.PTO.DefaultPTOIdleLosses)),
				PTOCycle =
					DrivingCycleDataReader.ReadFromStream(RessourceHelper.ReadStream(DeclarationData.PTO.DefaultPTOActivationCycle),
						CycleType.PTO, "PTO", false)
			};
		}

		internal Segment GetVehicleClassification(VehicleCategory category, AxleConfiguration axles, Kilogram grossMassRating,
			Kilogram curbWeight)
		{
			return DeclarationData.Segments.Lookup(category, axles, grossMassRating, curbWeight);
		}
	}
}