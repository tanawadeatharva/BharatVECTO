/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
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

	internal class DeclarationVTPModeVectoRunDataFactoryHeavyBusPrimary : IVectoRunDataFactory
	{
		
	}
	internal class DeclarationVTPModeVectoRunDataFactoryLorries : IVectoRunDataFactory
	{
		protected IVTPDeclarationJobInputData JobInputData;
		protected DriverData Driverdata;
		protected AirdragData AirdragData;
		protected CombustionEngineData EngineData;
		protected AxleGearData AxlegearData;
		protected AngledriveData AngledriveData;
		protected GearboxData GearboxData;
		protected RetarderData RetarderData;
		protected PTOData PTOTransmissionData;
		protected List<VectoRunData.AuxData> AuxVTP;
		protected Segment Segment;
		protected DeclarationDataAdapterHeavyLorry Dao;
		protected Exception InitException;

		public IVTPReport Report;
		protected ShiftStrategyParameters GearshiftData;

		public DeclarationVTPModeVectoRunDataFactoryLorries(IVTPDeclarationInputDataProvider ivtpProvider, IVTPReport report) : this(
			ivtpProvider.JobInputData, report) { }

		protected DeclarationVTPModeVectoRunDataFactoryLorries(IVTPDeclarationJobInputData job, IVTPReport report)
		{
			JobInputData = job;
			Report = report;
			try {
				Initialize();
				if (Report != null) {
					InitializeReport();
				}
			} catch (Exception e) {
				InitException = e;
			}
		}

		private void InitializeReport()
		{
			var powertrainConfig = new VectoRunData() {
				VehicleData =
					Dao.CreateVehicleData(
						JobInputData.Vehicle, Segment, Segment.Missions.First(),
						Segment.Missions.First().Loadings.First()),
				AirdragData = AirdragData,
				EngineData = EngineData,
				GearboxData = GearboxData,
				AxleGearData = AxlegearData,
				Retarder = RetarderData,
				Aux =
					Dao.CreateAuxiliaryData(
						JobInputData.Vehicle.Components.AuxiliaryInputData,
						JobInputData.Vehicle.Components.BusAuxiliaries,
						Segment.Missions.First().MissionType,
						Segment.VehicleClass, JobInputData.Vehicle.Length),
			};
			powertrainConfig.VehicleData.VehicleClass = Segment.VehicleClass;
			Report.InputDataHash = JobInputData.VectoJobHash;
			Report.ManufacturerRecord = JobInputData.ManufacturerReportInputData;
			Report.ManufacturerRecordHash = JobInputData.VectoManufacturerReportHash;
			var fuels = JobInputData.Vehicle.Components.EngineInputData.EngineModes.Select(
										x => x.Fuels.Select(f => DeclarationData.FuelData.Lookup(f.FuelType, JobInputData.Vehicle.TankSystem))
											.ToList())
									.ToList();
			Report.InitializeReport(powertrainConfig, fuels);
		}


		protected void Initialize()
		{
			Dao = new DeclarationDataAdapterHeavyLorry();
			var vehicle = JobInputData.Vehicle;
			Segment = DeclarationData.TruckSegments.Lookup(
				vehicle.VehicleCategory,
				vehicle.AxleConfiguration,
				vehicle.GrossVehicleMassRating,
				vehicle.CurbMassChassis,
				vehicle.VocationalVehicle);
			Driverdata = Dao.CreateDriverData();
			Driverdata.AccelerationCurve = AccelerationCurveReader.ReadFromStream(Segment.AccelerationFile);
			var tempVehicle = Dao.CreateVehicleData(
				vehicle, Segment, Segment.Missions.First(),
				Segment.Missions.First().Loadings.First());

			var vtpMission = Segment.VehicleClass.IsMediumLorry()
				? DeclarationData.VTPMode.SelectedMissionMediumLorry
				: DeclarationData.VTPMode.SelectedMissionHeavyLorry;

			AirdragData = Dao.CreateAirdragData(
				vehicle.Components.AirdragInputData,
				Segment.Missions.First(), Segment);
			EngineData = Dao.CreateEngineData(
				vehicle, vehicle.Components.EngineInputData.EngineModes.First(),
				new Mission() { MissionType = vtpMission });
			AxlegearData = JobInputData.Vehicle.Components.GearboxInputData.DifferentialIncluded
				? Dao.CreateDummyAxleGearData(JobInputData.Vehicle.Components.GearboxInputData)
				: Dao.CreateAxleGearData(vehicle.Components.AxleGearInputData);
			AngledriveData = Dao.CreateAngledriveData(vehicle.Components.AngledriveInputData);

			GearboxData = Dao.CreateGearboxData(
				vehicle, new VectoRunData() { EngineData = EngineData, AxleGearData = AxlegearData, VehicleData = tempVehicle },
				null);
			RetarderData = Dao.CreateRetarderData(vehicle.Components.RetarderInputData);

			PTOTransmissionData =
				Dao.CreatePTOTransmissionData(vehicle.Components.PTOTransmissionInputData);

			GearshiftData = Dao.CreateGearshiftData(
				GearboxData, AxlegearData.AxleGear.Ratio * (AngledriveData?.Angledrive.Ratio ?? 1.0), EngineData.IdleSpeed);

			AuxVTP = CreateVTPAuxData(vehicle);
		}

		#region Implementation of IVectoRunDataFactory

		public virtual IEnumerable<VectoRunData> NextRun()
		{
			if (InitException != null) {
				throw InitException;
			}

			// simulate the LongHaul cycle with RefLoad
			var vtpMission = Segment.VehicleClass.IsMediumLorry() 
				? DeclarationData.VTPMode.SelectedMissionMediumLorry
				: DeclarationData.VTPMode.SelectedMissionHeavyLorry;
			var mission = Segment.Missions.FirstOrDefault(m => m.MissionType == vtpMission);
			if (mission == null) {
				throw new VectoException("Mission {0} not found in segmentation matrix", DeclarationData.VTPMode.SelectedMissionHeavyLorry);
			}

			var loading = mission.Loadings.FirstOrDefault(l => l.Key == DeclarationData.VTPMode.SelectedLoading);
			var runData = CreateVectoRunData(Segment, mission, loading.Value);
			runData.ModFileSuffix = loading.Key.ToString();
			var cycle = DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false);
			runData.Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString());
			runData.DriverData = Driverdata;
			runData.Aux = Dao.CreateAuxiliaryData(
				JobInputData.Vehicle.Components.AuxiliaryInputData, JobInputData.Vehicle.Components.BusAuxiliaries, mission.MissionType, Segment.VehicleClass, JobInputData.Vehicle.Length);
			runData.ExecutionMode = ExecutionMode.Declaration;
			runData.SimulationType = SimulationType.DistanceCycle;
			runData.Mission = mission;
			runData.Loading = loading.Key;
			runData.VehicleData.VehicleClass = Segment.VehicleClass;
			runData.VehicleData.LegislativeClass = JobInputData.Vehicle.LegislativeClass;
			yield return runData;

			// simulate the Measured cycle
			var vtpCycle = JobInputData.Cycles.FirstOrDefault();
			if (vtpCycle == null) {
				throw new VectoException("no VTP-Cycle provided!");
			}

			var drivingCycle = DrivingCycleDataReader.ReadFromDataTable(vtpCycle.CycleData, vtpCycle.Name, false);

			// Loading is not relevant as we use P_wheel
			var vtpRunData = CreateVectoRunData(Segment, Segment.Missions.First(), Tuple.Create<Kilogram, double?>( 0.SI<Kilogram>(), null));
			vtpRunData.Cycle = new DrivingCycleProxy(drivingCycle, vtpCycle.Name);
			vtpRunData.Aux = AuxVTP;
			vtpRunData.FanData = GetFanData();
			vtpRunData.ExecutionMode = ExecutionMode.Declaration;
			vtpRunData.SimulationType = SimulationType.VerificationTest;
			vtpRunData.Mission = new Mission() {
				MissionType = MissionType.VerificationTest
			};
			vtpRunData.VehicleData.VehicleClass = Segment.VehicleClass;
			vtpRunData.VehicleData.LegislativeClass = JobInputData.Vehicle.LegislativeClass;

			//var ncvStd = DeclarationData.FuelData.Lookup(JobInputData.Vehicle.Components.EngineInputData.FuelType).LowerHeatingValueVecto;
			//var ncvCorrection = ncvStd / JobInputData.NetCalorificValueTestFuel;
			var mileageCorrection = GetMileagecorrectionFactor(JobInputData.Mileage);
			vtpRunData.VTPData = new VTPData() {
				CorrectionFactor = mileageCorrection,
			};
			yield return vtpRunData;
		}

		protected virtual AuxFanData GetFanData()
		{
			return new AuxFanData() {
				FanCoefficients = DeclarationData.VTPMode.FanParameters,
				FanDiameter = JobInputData.FanDiameter,
			};
		}

		private double GetMileagecorrectionFactor(Meter mileage)
		{
			if (mileage > DeclarationData.VTPMode.RunInThreshold) {
				return 1;
			}

			return DeclarationData.VTPMode.EvolutionCoefficient + (1 - DeclarationData.VTPMode.EvolutionCoefficient) * mileage /
					DeclarationData.VTPMode.RunInThreshold;
		}

		protected VectoRunData CreateVectoRunData(Segment segment, Mission mission, Tuple<Kilogram, double?> loading)
		{
			return new VectoRunData {
				JobName = JobInputData.Vehicle.VIN,
				EngineData = EngineData,
				GearboxData = GearboxData,
				GearshiftParameters = GearshiftData,
				AxleGearData = AxlegearData,
				AngledriveData = AngledriveData,
				VehicleData = Dao.CreateVehicleData(
					JobInputData.Vehicle, segment, mission,
					new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(LoadingType.ReferenceLoad, loading)),
				AirdragData = AirdragData,
				DriverData = null,
				BusAuxiliaries = null,
				Retarder = RetarderData,
				PTO = PTOTransmissionData,
				Report = Report,
			};
		}

		protected virtual List<VectoRunData.AuxData> CreateVTPAuxData(IVehicleDeclarationInputData vehicle)
		{
			var auxRD = Dao.CreateAuxiliaryData(
								vehicle.Components.AuxiliaryInputData, vehicle.Components.BusAuxiliaries, MissionType.RegionalDelivery, Segment.VehicleClass, vehicle.Length)
							.ToList();
			foreach (var entry in auxRD) {
				entry.MissionType = MissionType.RegionalDelivery;
			}

			var auxLH = Dao.CreateAuxiliaryData(
								vehicle.Components.AuxiliaryInputData, vehicle.Components.BusAuxiliaries, MissionType.LongHaul, Segment.VehicleClass, vehicle.Length)
							.ToList();
			foreach (var entry in auxLH) {
				entry.MissionType = MissionType.LongHaul;
			}

			var auxUD = Dao.CreateAuxiliaryData(
								vehicle.Components.AuxiliaryInputData, vehicle.Components.BusAuxiliaries, MissionType.UrbanDelivery, Segment.VehicleClass, vehicle.Length)
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
