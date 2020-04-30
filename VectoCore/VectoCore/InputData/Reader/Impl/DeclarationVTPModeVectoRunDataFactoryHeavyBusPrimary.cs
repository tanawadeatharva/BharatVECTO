using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl {
	internal class DeclarationVTPModeVectoRunDataFactoryHeavyBusPrimary : AbstractVTPModeVectoRunDataFactory
	{
		private DeclarationDataAdapterPrimaryBus _dao;

		public DeclarationVTPModeVectoRunDataFactoryHeavyBusPrimary(
			IVTPDeclarationInputDataProvider ivtpProvider, IVTPReport report) : base(ivtpProvider.JobInputData, report)
		{
			
		}

		protected DeclarationVTPModeVectoRunDataFactoryHeavyBusPrimary(IVTPDeclarationJobInputData vtpJob, IVTPReport report): base(vtpJob, report) { }


		#region Implementation of IVectoRunDataFactory

		protected override IDeclarationDataAdapter Dao { get {
			return _dao ?? (_dao = new DeclarationDataAdapterPrimaryBus());
		} }


		protected override IEnumerable<VectoRunData.AuxData> GetAuxiliaryData(MissionType missionType)
		{
			// TODO MQ: length?
			return Dao.CreateAuxiliaryData(
				JobInputData.Vehicle.Components.AuxiliaryInputData,
				JobInputData.Vehicle.Components.BusAuxiliaries,
				missionType,
				Segment.VehicleClass, 10.SI<Meter>());

		}

		#endregion

		protected override void Initialize()
		{
			var vehicle = JobInputData.Vehicle;
			Segment = DeclarationData.PrimaryBusSegments.Lookup(
				vehicle.VehicleCategory, vehicle.AxleConfiguration, vehicle.Articulated, vehicle.FloorType);

			Driverdata = Dao.CreateDriverData();
			Driverdata.AccelerationCurve = AccelerationCurveReader.ReadFromStream(Segment.AccelerationFile);
			var tempVehicle = Dao.CreateVehicleData(
				vehicle, Segment, Segment.Missions.First(),
				Segment.Missions.First().Loadings.First());

			var vtpMission = DeclarationData.VTPMode.SelectedMissionHeavyBus;
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

		protected virtual List<VectoRunData.AuxData> CreateVTPAuxData(IVehicleDeclarationInputData vehicle)
		{
			return new List<VectoRunData.AuxData>();
		}

		public override IEnumerable<VectoRunData> NextRun()
		{
			throw new NotImplementedException();
		}

		protected override AuxFanData GetFanData()
		{
			return new AuxFanData() {
				FanCoefficients = JobInputData.FanPowerCoefficents.ToArray(),
				FanDiameter = JobInputData.FanDiameter,
			};
		}
	}
}