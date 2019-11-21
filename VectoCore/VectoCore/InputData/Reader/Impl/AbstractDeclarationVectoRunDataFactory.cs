using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.Impl {
	public abstract class AbstractDeclarationVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{
		protected static readonly object CyclesCacheLock = new object();

		protected static readonly Dictionary<MissionType, DrivingCycleData> CyclesCache =
			new Dictionary<MissionType, DrivingCycleData>();

		protected readonly IDeclarationInputDataProvider InputDataProvider;

		protected IDeclarationReport Report;
		protected abstract IDeclarationDataAdapter DataAdapter { get; }

		protected Segment _segment;
		protected DriverData _driverdata;
		protected AirdragData _airdragData;
		protected AxleGearData _axlegearData;
		protected AngledriveData _angledriveData;
		protected GearboxData _gearboxData;
		protected RetarderData _retarderData;
		protected PTOData _ptoTransmissionData;
		protected PTOData _municipalPtoTransmissionData;
		//protected Exception InitException;

		protected AbstractDeclarationVectoRunDataFactory(
			IDeclarationInputDataProvider dataProvider, IDeclarationReport report)
		{
			InputDataProvider = dataProvider;
			Report = report;

			//try {
			//	Initialize();
			//	if (Report != null) {
			//		InitializeReport();
			//	}
			//} catch (Exception e) {
			//	InitException = e;
			//}
		}

		public IEnumerable<VectoRunData> NextRun()
		{
		
			Initialize();
			if (Report != null) {
				InitializeReport();
			}

			return GetNextRun();
		}

		protected abstract IEnumerable<VectoRunData> GetNextRun();

		protected virtual void Initialize()
		{
			var vehicle = InputDataProvider.JobInputData.Vehicle;
			if (vehicle.ExemptedVehicle) {
				return;
			}

			_segment = GetSegment(vehicle);
			_driverdata = DataAdapter.CreateDriverData();
			_driverdata.AccelerationCurve = AccelerationCurveReader.ReadFromStream(_segment.AccelerationFile);
			var tempVehicle = DataAdapter.CreateVehicleData(vehicle, _segment.Missions.First(),
													_segment.Missions.First().Loadings.First().Value);
			_airdragData = DataAdapter.CreateAirdragData(vehicle.Components.AirdragInputData,
												_segment.Missions.First(), _segment);
			_axlegearData = DataAdapter.CreateAxleGearData(InputDataProvider.JobInputData.Vehicle.Components.AxleGearInputData);
			_angledriveData = DataAdapter.CreateAngledriveData(InputDataProvider.JobInputData.Vehicle.Components.AngledriveInputData);
			var tmpEngine = DataAdapter.CreateEngineData(
				vehicle, vehicle.Components.EngineInputData.EngineModes[0], _segment.Missions.First());
			_gearboxData = DataAdapter.CreateGearboxData(vehicle.Components.GearboxInputData, tmpEngine,
												_axlegearData.AxleGear.Ratio,
												tempVehicle.DynamicTyreRadius, tempVehicle.VehicleCategory, vehicle.Components.TorqueConverterInputData);
			_retarderData = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData);

			_ptoTransmissionData = DataAdapter.CreatePTOTransmissionData(vehicle.Components.PTOTransmissionInputData);

			_municipalPtoTransmissionData = CreateDefaultPTOData();
		}

		protected abstract Segment GetSegment(IVehicleDeclarationInputData vehicle);
		

		protected virtual void InitializeReport()
		{
			VectoRunData powertrainConfig;
			List<List<FuelData.Entry>> fuels;
			if (InputDataProvider.JobInputData.Vehicle.ExemptedVehicle) {
				powertrainConfig = new VectoRunData() {
					Exempted = true,
					VehicleData = DataAdapter.CreateVehicleData(InputDataProvider.JobInputData.Vehicle, null, null),
					InputDataHash = InputDataProvider.XMLHash
				};
				fuels = new List<List<FuelData.Entry>>();
			} else {
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				powertrainConfig = new VectoRunData() {
					VehicleData =
						DataAdapter.CreateVehicleData(
							InputDataProvider.JobInputData.Vehicle, _segment.Missions.First(),
							_segment.Missions.First().Loadings.First().Value),
					AirdragData = _airdragData,
					EngineData = DataAdapter.CreateEngineData(vehicle, vehicle.Components.EngineInputData.EngineModes[0], _segment.Missions.First()),
					GearboxData = _gearboxData,
					AxleGearData = _axlegearData,
					Retarder = _retarderData,
					Aux =
						DataAdapter.CreateAuxiliaryData(
							InputDataProvider.JobInputData.Vehicle.Components.AuxiliaryInputData,
							_segment.Missions.First().MissionType,
							_segment.VehicleClass),
					PTO = _ptoTransmissionData,
					InputDataHash = InputDataProvider.XMLHash
				};
				powertrainConfig.VehicleData.VehicleClass = _segment.VehicleClass;
				fuels = vehicle.Components.EngineInputData.EngineModes.Select(x => x.Fuels.Select(f => DeclarationData.FuelData.Lookup(f.FuelType, vehicle.TankSystem)).ToList())
								.ToList();
			}
			Report.InitializeReport(powertrainConfig, fuels);
		}

		protected virtual PTOData CreateDefaultPTOData()
		{
			return new PTOData() {
				TransmissionType = DeclarationData.PTO.DefaultPTOTechnology,
				LossMap = PTOIdleLossMapReader.ReadFromStream(RessourceHelper.ReadStream(DeclarationData.PTO.DefaultPTOIdleLosses)),
				PTOCycle =
					DrivingCycleDataReader.ReadFromStream(RessourceHelper.ReadStream(DeclarationData.PTO.DefaultPTOActivationCycle),
														CycleType.PTO, "PTO", false)
			};
		}
	}
}