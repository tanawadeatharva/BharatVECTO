using System;
using System.Collections.Generic;
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
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;
using static TUGraz.VectoCommon.InputData.VectoSimulationJobType;

namespace TUGraz.VectoCore.InputData.Reader.Impl {
	public abstract class AbstractDeclarationVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{
		protected readonly IDeclarationInputDataProvider InputDataProvider;

		protected IDeclarationReport Report;
		protected abstract IDeclarationDataAdapter DataAdapter { get; }

		protected Segment _segment;

		protected bool _allowVocational;

		protected DriverData _driverdata;
		protected AirdragData _airdragData;
		protected AxleGearData _axlegearData;
		protected AngledriveData _angledriveData;
		protected GearboxData _gearboxData;
		protected RetarderData _retarderData;
		protected PTOData _ptoTransmissionData;
		protected PTOData _municipalPtoTransmissionData;
		//protected Exception InitException;
		protected ShiftStrategyParameters _gearshiftData;

		protected AbstractDeclarationVectoRunDataFactory(
			IDeclarationInputDataProvider dataProvider, IDeclarationReport report, bool checkJobType = true)
		{
			InputDataProvider = dataProvider;

			if (checkJobType) {
				if (dataProvider.JobInputData.JobType.IsOneOf(BatteryElectricVehicle, ParallelHybridVehicle, SerialHybridVehicle))
				{
					throw new VectoSimulationException("Electric and Hybrid Vehicles are not supported in Declaration Mode. Aborting Simulation.");
				}
			}
           
            Report = report;

			_allowVocational = true;
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


		}

		protected abstract Segment GetSegment(IVehicleDeclarationInputData vehicle);

		protected abstract VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading);

		protected virtual void InitializeReport()
		{
			VectoRunData powertrainConfig;
			List<List<FuelData.Entry>> fuels;
			var vehicle = InputDataProvider.JobInputData.Vehicle;
			if (vehicle.ExemptedVehicle) {
				powertrainConfig = CreateVectoRunData(vehicle, 0, null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>());
				fuels = new List<List<FuelData.Entry>>();
			} else {
				powertrainConfig = _segment.Missions.Select(
												mission => CreateVectoRunData(
													vehicle, 0, mission, mission.Loadings.First()))
											.FirstOrDefault(x => x != null);
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