using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;


namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public class DeclarationModeCompletedBusVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{
		protected static readonly object CyclesCacheLock = new object();

		protected static readonly Dictionary<MissionType, DrivingCycleData> CyclesCache =
			new Dictionary<MissionType, DrivingCycleData>();

		protected readonly IDeclarationInputDataProvider InputDataProvider;
		protected IDeclarationReport Report;


		protected Segment _segmentCompletedBus;
		
		protected AxleGearData _axlegearData;
		protected AngledriveData _angledriveData;
		protected GearboxData _gearboxData;
		protected RetarderData _retarderData;

		protected IAlternatorMap _alternatorMap;
		protected ICompressorMap _compressorMap;
		//protected IPneumaticsConsumersDemand _consumersDeclarationData;

		protected CombustionEngineData _combustionEngineData;

		//protected Exception InitException;
		protected ShiftStrategyParameters _gearshiftData;

		//private VehicleData _tmpVehicleData;
		private DriverData _driverData;


		
		protected DeclarationDataAdapterCompletedBusSpecific DataAdapterSpecific = new DeclarationDataAdapterCompletedBusSpecific();

		protected DeclarationDataAdapterCompletedBusGeneric DataAdapterGeneric = new DeclarationDataAdapterCompletedBusGeneric();

		public DeclarationModeCompletedBusVectoRunDataFactory(
			IDeclarationInputDataProvider dataProvider, IDeclarationReport report)
		{
			InputDataProvider = dataProvider;
			Report = report;
		}

		protected IVehicleDeclarationInputData PrimaryVehicle
		{
			get { return InputDataProvider.PrimaryVehicleData.Vehicle; }
		}

		protected IVehicleDeclarationInputData CompletedVehicle
		{
			get { return InputDataProvider.JobInputData.Vehicle; }
		}

		public IEnumerable<VectoRunData> NextRun()
		{
			Initialize();
			if (Report != null) {
				InitializeReport();
			}

			return GetNextRun();
		}

		protected virtual void InitializeReport()
		{
			var powertrainConfig = _segmentCompletedBus.Missions.Select(
														mission => CreateVectoRunDataSpecific(
															 mission, mission.Loadings.First()))
													.FirstOrDefault(x => x != null);
			
			Report.InitializeReport(powertrainConfig, new List<List<FuelData.Entry>>());
		}

		

		protected virtual void Initialize()
		{
			if (CompletedVehicle.ExemptedVehicle || PrimaryVehicle.ExemptedVehicle) {
				return;
			}
			 
			_segmentCompletedBus = GetCompletedSegment(CompletedVehicle, PrimaryVehicle.AxleConfiguration);

			var tmpVehicleData = DataAdapterSpecific.CreateVehicleData(PrimaryVehicle, CompletedVehicle, _segmentCompletedBus.Missions.First(),
																_segmentCompletedBus.Missions.First().Loadings.First());
			tmpVehicleData.VehicleCategory = VehicleCategory.GenericBusVehicle;

			_combustionEngineData = DataAdapterGeneric.CreateEngineData(PrimaryVehicle);

			_axlegearData = DataAdapterGeneric.CreateAxleGearData(PrimaryVehicle.Components.AxleGearInputData);

			_angledriveData = DataAdapterGeneric.CreateAngledriveData(PrimaryVehicle.Components.AngledriveInputData);
			
			var tmpRunData = new VectoRunData() {
				ShiftStrategy = InputDataProvider.JobInputData.ShiftStrategy,
				GearboxData = new GearboxData() {
					Type = PrimaryVehicle.Components.GearboxInputData.Type,
				}
			};
			var tmpStrategy = PowertrainBuilder.GetShiftStrategy(tmpRunData, new SimplePowertrainContainer(tmpRunData));
			
			_gearboxData = DataAdapterGeneric.CreateGearboxData(PrimaryVehicle, new VectoRunData() { EngineData = _combustionEngineData, AxleGearData = _axlegearData, VehicleData = tmpVehicleData },
				tmpStrategy);

			_gearshiftData = DataAdapterGeneric.CreateGearshiftData(
				_gearboxData, _axlegearData.AxleGear.Ratio * (_angledriveData?.Angledrive.Ratio ?? 1.0), _combustionEngineData.IdleSpeed);

			_retarderData = DataAdapterGeneric.CreateRetarderData(PrimaryVehicle.Components.RetarderInputData);
				

			_driverData = DataAdapterGeneric.CreateDriverData();
			_driverData.AccelerationCurve = AccelerationCurveReader.ReadFromStream(_segmentCompletedBus.AccelerationFile);
		}

		protected virtual IEnumerable<VectoRunData> GetNextRun()
		{
			if (InputDataProvider.JobInputData.Vehicle.VehicleCategory == VehicleCategory.HeavyBusCompletedVehicle) {
				return VectoRunDataHeavyBusCompleted();
			}

			return new List<VectoRunData>();
		}

		private IEnumerable<VectoRunData> VectoRunDataHeavyBusCompleted()
		{
			foreach (var mission in _segmentCompletedBus.Missions) {
				foreach (var loading in mission.Loadings) {
					var simulationRunData = CreateVectoRunDataSpecific(mission, loading);
					if (simulationRunData != null) {
						yield return simulationRunData;
					}

					var primarySegment = GetPrimarySegment(PrimaryVehicle);
					var primaryMission = primarySegment.Missions.Where(
						m => {
							return m.BusParameter.DoubleDecker == CompletedVehicle.VehicleCode.IsDoubleDeckerBus() &&
									m.MissionType == mission.MissionType;
						}).First();
					simulationRunData = CreateVectoRunDataGeneric(
						primaryMission, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(loading.Key, primaryMission.Loadings[loading.Key]),
						primarySegment);
					yield return simulationRunData;
				}
			}
		}

		protected virtual Segment GetPrimarySegment(IVehicleDeclarationInputData primaryVehicle)
		{
			var completedVehicle = InputDataProvider.JobInputData.Vehicle;

			var primarySegment = DeclarationData.PrimaryBusSegments.Lookup(
				primaryVehicle.VehicleCategory, primaryVehicle.AxleConfiguration, primaryVehicle.Articulated,
				primaryVehicle.FloorType, completedVehicle.VehicleCode.IsDoubleDeckerBus());

			return primarySegment;
		}


		protected virtual Segment GetCompletedSegment(IVehicleDeclarationInputData vehicle, AxleConfiguration axleConfiguration)
		{
			if (vehicle.VehicleCategory != VehicleCategory.HeavyBusCompletedVehicle) {
				throw new VectoException(
					"Invalid vehicle category for bus factory! {0}", vehicle.VehicleCategory.GetCategoryName());
			}

			var segment = DeclarationData.CompletedBusSegments.Lookup(
				axleConfiguration.NumAxles(), vehicle.VehicleCode, vehicle.RegisteredClass, vehicle.NumberOfPassengersLowerDeck,
				vehicle.Height, vehicle.FloorType == FloorType.LowFloor);
			if (!segment.Found) {
				throw new VectoException(
					"no segment found for vehicle configruation: vehicle category: {0}, axle configuration: {1}, articulated: {2}, primary",
					vehicle.VehicleCategory, axleConfiguration,
					vehicle.Articulated);
			}

			return segment;
		}


		protected VectoRunData CreateVectoRunDataSpecific(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
		{
			DrivingCycleData cycle;
			lock (CyclesCacheLock) {
				if (CyclesCache.ContainsKey(mission.MissionType)) {
					cycle = CyclesCache[mission.MissionType];
				} else {
					cycle = DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false);
					CyclesCache.Add(mission.MissionType, cycle);
				}
			}
			
			

			var simulationRunData = new VectoRunData {
				Loading = loading.Key,
				VehicleData = DataAdapterSpecific.CreateVehicleData(PrimaryVehicle, CompletedVehicle, 
					mission, loading),
				AirdragData = DataAdapterSpecific.CreateAirdragData(CompletedVehicle, mission),
				EngineData = _combustionEngineData,
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				AngledriveData = _angledriveData,
				Aux = DataAdapterSpecific.CreateAuxiliaryData(PrimaryVehicle.Components.AuxiliaryInputData,
					PrimaryVehicle.Components.BusAuxiliaries, mission.MissionType, _segmentCompletedBus.VehicleClass, CompletedVehicle.Length),
				Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
				Retarder = _retarderData,
				DriverData = _driverData,
				ExecutionMode = ExecutionMode.Declaration,
				JobName = InputDataProvider.JobInputData.JobName,
				ModFileSuffix = "_" + _segmentCompletedBus.VehicleClass.GetClassNumber() + "-Specific_" + loading.Key.ToString(),
				Report = Report,
				Mission = mission,
				InputDataHash = InputDataProvider.XMLHash,
				SimulationType = SimulationType.DistanceCycle,
				VehicleDesignSpeed = _segmentCompletedBus.DesignSpeed,
				GearshiftParameters = _gearshiftData,
			};
			simulationRunData.EngineData.FuelMode = 0;
			simulationRunData.VehicleData.VehicleClass = _segmentCompletedBus.VehicleClass;
			simulationRunData.BusAuxiliaries = DataAdapterSpecific.CreateBusAuxiliariesData(mission, PrimaryVehicle, CompletedVehicle, simulationRunData);
			
			return simulationRunData;
		}

		
		protected VectoRunData CreateVectoRunDataGeneric(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, Segment primarySegment)
		{
			DrivingCycleData cycle;
			lock (CyclesCacheLock) {
				if (CyclesCache.ContainsKey(mission.MissionType)) {
					cycle = CyclesCache[mission.MissionType];
				} else {
					cycle = DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false);
					CyclesCache.Add(mission.MissionType, cycle);
				}
			}

			var primaryBusAuxiliaries = PrimaryVehicle.Components.BusAuxiliaries;

			var simulationRunData = new VectoRunData {
				Loading = loading.Key,
				VehicleData = DataAdapterGeneric.CreateVehicleData(PrimaryVehicle, mission, loading),
				AirdragData = DataAdapterGeneric.CreateAirdragData(null, mission, new Segment()),
				EngineData = _combustionEngineData,
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				AngledriveData = _angledriveData,
				Aux = DataAdapterGeneric.CreateAuxiliaryData(PrimaryVehicle.Components.AuxiliaryInputData,
					primaryBusAuxiliaries, mission.MissionType, primarySegment.VehicleClass,
					mission.BusParameter.VehicleLength),
				Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
				Retarder = _retarderData,
				DriverData = _driverData,
				ExecutionMode = ExecutionMode.Declaration,
				JobName = InputDataProvider.JobInputData.JobName,
				ModFileSuffix = "_" + _segmentCompletedBus.VehicleClass.GetClassNumber() + "-Generic_" + loading.Key.ToString(),
				Report = Report,
				Mission = mission,
				InputDataHash = InputDataProvider.XMLHash,
				SimulationType = SimulationType.DistanceCycle,
				VehicleDesignSpeed = _segmentCompletedBus.DesignSpeed,
				GearshiftParameters = _gearshiftData,
			};
			simulationRunData.EngineData.FuelMode = 0;
			simulationRunData.VehicleData.VehicleClass = _segmentCompletedBus.VehicleClass;
			simulationRunData.BusAuxiliaries =
				DataAdapterGeneric.CreateBusAuxiliariesData(mission, PrimaryVehicle, simulationRunData);

			return simulationRunData;
		}

	}
}
