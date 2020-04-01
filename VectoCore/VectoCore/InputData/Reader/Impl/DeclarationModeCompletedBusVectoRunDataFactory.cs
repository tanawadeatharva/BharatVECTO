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
using TUGraz.VectoCore.Models.SimulationComponent.Data;
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


		protected Segment _segment;
		
		protected DriverData _driverdata;
		protected AirdragData _airdragData;
		protected AxleGearData _axlegearData;
		protected AngledriveData _angledriveData;
		protected GearboxData _gearboxData;
		protected RetarderData _retarderData;

		protected IAlternatorMap _alternatorMap;
		protected ICompressorMap _compressorMap;
		protected IPneumaticsConsumersDemand _consumersDeclarationData;

		protected CombustionEngineData _combustionEngineData;

		//protected Exception InitException;
		protected ShiftStrategyParameters _gearshiftData;

		private VehicleData _tmpVehicleData;


		protected IDeclarationDataAdapter DataAdapter { get; }


		protected DeclarationDataAdapterCompletedBus DataAdapterCompleted = new DeclarationDataAdapterCompletedBus();

		protected DeclarationDataAdapterPrimaryBus DataAdapterPrimary = new DeclarationDataAdapterPrimaryBus();

		public DeclarationModeCompletedBusVectoRunDataFactory(
			IDeclarationInputDataProvider dataProvider, IDeclarationReport report)
		{
			InputDataProvider = dataProvider;
			Report = report;
		}

		public IEnumerable<VectoRunData> NextRun()
		{
			Initialize();
			if (Report != null) {
				//InitializeReport();
			}

			return GetNextRun();
		}

		protected virtual void InitializeReport()
		{
			var vehicle = InputDataProvider.JobInputData.Vehicle;

			var powertrainConfig = _segment.Missions.Select(
														mission => CreateVectoRunDataSpecific(
															vehicle, InputDataProvider.PrimaryVehicleData.Vehicle, mission, mission.Loadings.First()))
													.FirstOrDefault(x => x != null);
			
			Report.InitializeReport(powertrainConfig, new List<List<FuelData.Entry>>());
		}

		

		protected virtual void Initialize()
		{
			var vehicle = InputDataProvider.JobInputData.Vehicle;
			var primaryVehicle = InputDataProvider.PrimaryVehicleData.Vehicle;
			if (vehicle.ExemptedVehicle) {
				return;
			}
			 
			_segment = GetCompletedSegment(vehicle, primaryVehicle.AxleConfiguration);

			//_driverdata = DataAdapterCompleted.CreateDriverData();
			//_driverdata.AccelerationCurve = AccelerationCurveReader.ReadFromStream(_segment.AccelerationFile);
			////var tempVehicle = DataAdapter.CreateVehicleData(
			////	vehicle, _segment.Missions.First(),
			////	_segment.Missions.First().Loadings.First());
			//_airdragData = DataAdapterCompleted.CreateAirdragData(
			//	vehicle.Components.AirdragInputData,
			//	_segment.Missions.First(), _segment);

			//_axlegearData = DeclarationData.FactorMethodBus.CreateAxlegearData(primaryVehicle.Components.AxleGearInputData);

			//_angledriveData = null;
			//var tmpRunData = new VectoRunData() {
			//	ShiftStrategy = InputDataProvider.JobInputData.ShiftStrategy,
			//	GearboxData = new GearboxData() {
			//		Type = vehicle.Components.GearboxInputData.Type,
			//	}
			//};

			////var tmpStrategy = PowertrainBuilder.GetShiftStrategy(tmpRunData, new SimplePowertrainContainer(tmpRunData));
			//var tmpEngine = DeclarationData.FactorMethodBus.CreateBusEngineData(primaryVehicle.Components.EngineInputData);
			//_gearboxData = DeclarationData.FactorMethodBus.CreateGearboxData(primaryVehicle.Components.GearboxInputData);

			//_retarderData = DataAdapterCompleted.CreateRetarderData(vehicle.Components.RetarderInputData);

			//_ptoTransmissionData = null;

			//_municipalPtoTransmissionData = null;

			_tmpVehicleData = new VehicleData {
				DynamicTyreRadius = DeclarationData.FactorMethodBus.GetDynamicTyreRadius(primaryVehicle),
				VehicleCategory = VehicleCategory.GenericBusVehicle
			};


			_combustionEngineData = DeclarationData.FactorMethodBus.CreateBusEngineData(primaryVehicle);

			_axlegearData = DeclarationData.FactorMethodBus.CreateAxlegearData(primaryVehicle.Components.AxleGearInputData);

			_angledriveData = DeclarationData.FactorMethodBus.CreateAngledriveData(
								primaryVehicle.Components.AngledriveInputData,
								primaryVehicle.Components.AxleGearInputData.Ratio);
			
			_gearboxData = DeclarationData.FactorMethodBus.CreateGearboxData(primaryVehicle,
				new VectoRunData() { EngineData = _combustionEngineData, AxleGearData = _axlegearData, VehicleData = _tmpVehicleData},
				null);

			_gearshiftData = DataAdapterPrimary.CreateGearshiftData(
				_gearboxData, _axlegearData.AxleGear.Ratio * (_angledriveData?.Angledrive.Ratio ?? 1.0), _combustionEngineData.IdleSpeed);


			var primaryBusAuxiliaries = primaryVehicle.Components.BusAuxiliaries;

			_alternatorMap = new SimpleAlternator(
				DataAdapterPrimary.CalculateAlternatorEfficiency(primaryBusAuxiliaries.ElectricSupply.Alternators
					.Concat(vehicle.Components.BusAuxiliaries.ElectricSupply.Alternators).ToList()));

			_compressorMap = DataAdapterPrimary.GetCompressorMap(primaryBusAuxiliaries.PneumaticSupply.CompressorSize,
				primaryBusAuxiliaries.PneumaticSupply.Clutch);

			var retarderType = primaryVehicle.Components.RetarderInputData.Type;
			_consumersDeclarationData = DataAdapterPrimary.CreatePneumaticAuxConfig(retarderType);

			_retarderData =
				DeclarationData.FactorMethodBus.CreateRetarderData(primaryVehicle.Components.RetarderInputData);
		
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
			var completedVehicle = InputDataProvider.JobInputData.Vehicle;
			var primaryVehicle = InputDataProvider.PrimaryVehicleData.Vehicle;
			foreach (var mission in _segment.Missions) {
				foreach (var loading in mission.Loadings) {
					var simulationRunData = CreateVectoRunDataSpecific(primaryVehicle, completedVehicle, mission, loading);
					if (simulationRunData != null) {
						yield return simulationRunData;
					}

					var primarySegment = GetPrimarySegment(primaryVehicle);
					var primaryMission = primarySegment.Missions.Where(
						m => {
							return m.BusParameter.DoubleDecker == completedVehicle.VehicleCode.IsDoubleDeckBus() &&
									m.MissionType == mission.MissionType;
						}).First();
					simulationRunData = CreateVectoRunDataGeneric(primaryVehicle, completedVehicle, primaryMission, loading);
					yield return simulationRunData;
				}
			}
		}

		protected virtual Segment GetPrimarySegment(IVehicleDeclarationInputData primaryVehicle)
		{
			var completedVehicle = InputDataProvider.JobInputData.Vehicle;

			var primarySegment = DeclarationData.PrimaryBusSegments.Lookup(
				primaryVehicle.VehicleCategory, primaryVehicle.AxleConfiguration, primaryVehicle.Articulated,
				primaryVehicle.FloorType, completedVehicle.VehicleCode.IsDoubleDeckBus());

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


		protected VectoRunData CreateVectoRunDataSpecific(
			IVehicleDeclarationInputData primaryVehicle, IVehicleDeclarationInputData completedVehicle,
			Mission mission, KeyValuePair<LoadingType, Kilogram> loading)
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


			//var mergedBusAux = new CombinedBusAuxiliaries(
			//	primaryVehicle.Components.BusAuxiliaries, completedVehicle.Components.BusAuxiliaries);

			//var simulationRunData = new VectoRunData {
			//	Loading = loading.Key,
			//	VehicleData = DataAdapterCompleted.CreateVehicleData(primaryVehicle, mission, loading),
			//	AirdragData = DataAdapterCompleted.CreateAirdragData(null, mission, new Segment()),
			//  EngineData = DeclarationData.FactorMethodBus.CreateBusEngineData(primaryVehicle.Components.EngineInputData), 
			//	GearboxData = _gearboxData,
			//	AxleGearData = _axlegearData,
			//	AngledriveData = _angledriveData,
			//	Aux = DataAdapterCompleted.CreateAuxiliaryData(
			//		primaryVehicle.Components.AuxiliaryInputData,
			//		mergedBusAux, mission.MissionType, _segment.VehicleClass,
			//		primaryVehicle.Length ?? mission.BusParameter.VehicleLength),
			//	Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
			//	Retarder = _retarderData,
			//	DriverData = _driverdata,
			//	ExecutionMode = ExecutionMode.Declaration,
			//	JobName = InputDataProvider.JobInputData.JobName,
			//	ModFileSuffix = "_specific_" + mission.BusParameter.BusGroup.GetClassNumber() + "_" + loading.Key.ToString(),
			//	Report = Report,
			//	Mission = mission,
			//	InputDataHash = InputDataProvider.XMLHash,
			//	SimulationType = SimulationType.DistanceCycle,
			//	GearshiftParameters = _gearshiftData,
			//	ShiftStrategy = InputDataProvider.JobInputData.ShiftStrategy
			//};
			//simulationRunData.EngineData.FuelMode = 0;
			//simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
			//simulationRunData.BusAuxiliaries = DataAdapterPrimary.CreateBusAuxiliariesData(
			//	mission, InputDataProvider.JobInputData.Vehicle, simulationRunData);
			//return simulationRunData;


			// TODO MQ completedSegment == _segment?
			var completedSegment = GetCompletedSegment(completedVehicle, primaryVehicle.AxleConfiguration);
			var primaryBusAuxiliaries = primaryVehicle.Components.BusAuxiliaries;

			var simulationRunData = new VectoRunData {

				Loading = loading.Key,
				VehicleData = DataAdapterCompleted.CreateVehicleData(primaryVehicle, completedVehicle, 
					mission, loading, _tmpVehicleData.DynamicTyreRadius),
				AirdragData = DataAdapterCompleted.CreateAirdragData(completedVehicle, mission),
				EngineData = _combustionEngineData,
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				AngledriveData = _angledriveData,
				GearshiftParameters = _gearshiftData,
				Aux = DataAdapterPrimary.CreateAuxiliaryData(primaryVehicle.Components.AuxiliaryInputData,
				primaryBusAuxiliaries, mission.MissionType, completedSegment.VehicleClass,
				completedVehicle.Length)
			};
			
			var auxiliaryConfig = new AuxiliaryConfig {
				ElectricalUserInputsConfig = DataAdapterCompleted.CreateElectricsUserInputsConfig(
					primaryBusAuxiliaries, completedVehicle, mission, _alternatorMap),

				PneumaticUserInputsConfig = DataAdapterCompleted.CreatePneumaticUserInputsConfig(
					primaryBusAuxiliaries, completedVehicle, _compressorMap),

				PneumaticAuxillariesConfig = _consumersDeclarationData
			};
			
			auxiliaryConfig.SSMInputs = GetCompletedSSMInput(mission, completedVehicle, primaryBusAuxiliaries, loading);

			simulationRunData.BusAuxiliaries = auxiliaryConfig;

			simulationRunData.Retarder = _retarderData;
			
			// todo MQ 20200401: move driver data to initialize method and set member.
			simulationRunData.DriverData = DataAdapterCompleted.CreateDriverData(completedSegment);
			
			simulationRunData.Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString());
			
			return simulationRunData;
		}

		private SSMInputs GetCompletedSSMInput(Mission mission, IVehicleDeclarationInputData completedVehicle,
			IBusAuxiliariesDeclarationData primaryBusAux, KeyValuePair<LoadingType, Kilogram> loading)
		{
			var ssmInputs = new SSMInputs(null);
			DataAdapterCompleted.SetSSMBusParameters(ssmInputs, completedVehicle, mission, loading);

			ssmInputs.Technologies = DataAdapterCompleted.CreateTechnologyBenefits(completedVehicle,
				primaryBusAux, DataAdapterPrimary);

			DeclarationData.FactorMethodBus.SetBoundaryConditions(ssmInputs);
			DeclarationData.FactorMethodBus.SetEnvironmentalConditions(ssmInputs);

			DataAdapterCompleted.SetSSMInputs(ssmInputs, mission, loading, completedVehicle);

			return ssmInputs;
		}



		protected VectoRunData CreateVectoRunDataGeneric(
			IVehicleDeclarationInputData primaryVehicle, IVehicleDeclarationInputData completedVehicle,
			Mission mission, KeyValuePair<LoadingType, Kilogram> loading)
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

			//var simulationRunData = new VectoRunData {
			//	Loading = loading.Key,
			//	VehicleData = DataAdapterPrimary.CreateVehicleData(primaryVehicle, mission, loading),
			//	AirdragData = DataAdapterPrimary.CreateAirdragData(null, mission, new Segment()),
			//	EngineData = DeclarationData.FactorMethodBus.CreateBusEngineData(primaryVehicle.Components.EngineInputData),
			//	GearboxData = _gearboxData,
			//	AxleGearData = _axlegearData,
			//	AngledriveData = _angledriveData,
			//	Aux = DataAdapterPrimary.CreateAuxiliaryData(
			//		primaryVehicle.Components.AuxiliaryInputData,
			//		completedVehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
			//		primaryVehicle.Length ?? mission.BusParameter.VehicleLength),
			//	Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
			//	Retarder = _retarderData,
			//	DriverData = _driverdata,
			//	ExecutionMode = ExecutionMode.Declaration,
			//	JobName = InputDataProvider.JobInputData.JobName,
			//	ModFileSuffix = "_generic_" + mission.BusParameter.BusGroup.GetClassNumber() + "_" + loading.Key.ToString(),
			//	Report = Report,
			//	Mission = mission,
			//	InputDataHash = InputDataProvider.XMLHash,
			//	SimulationType = SimulationType.DistanceCycle,
			//	GearshiftParameters = _gearshiftData,
			//	ShiftStrategy = InputDataProvider.JobInputData.ShiftStrategy
			//};
			//simulationRunData.EngineData.FuelMode = 0;
			//simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
			//simulationRunData.BusAuxiliaries = DataAdapterPrimary.CreateBusAuxiliariesData(
			//	mission, InputDataProvider.JobInputData.Vehicle, simulationRunData);
			//return simulationRunData;

			var primarySegment = GetPrimarySegment(primaryVehicle);
			var primaryBusAuxiliaries = primaryVehicle.Components.BusAuxiliaries;

			var simulationRunData = new VectoRunData {
				Loading = loading.Key,
				VehicleData = DataAdapterPrimary.CreateVehicleData(primaryVehicle, mission, loading,
					_tmpVehicleData.DynamicTyreRadius),
				AirdragData = DataAdapterPrimary.CreateAirdragData(null, mission, new Segment()),
				EngineData = _combustionEngineData,
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				AngledriveData = _angledriveData,
				GearshiftParameters = _gearshiftData,
				Aux = DataAdapterPrimary.CreateAuxiliaryData(primaryVehicle.Components.AuxiliaryInputData,
					primaryBusAuxiliaries, mission.MissionType, primarySegment.VehicleClass,
					completedVehicle.Length)
			};
			
			var auxiliaryConfig = new AuxiliaryConfig {
				ElectricalUserInputsConfig = DataAdapterPrimary.CreateElectricalUserInputsConfig(
					primaryVehicle, _alternatorMap, mission),

				PneumaticUserInputsConfig = DataAdapterPrimary.CreatePneumaticUserInputsConfig(
					primaryBusAuxiliaries, _compressorMap),

				PneumaticAuxillariesConfig = _consumersDeclarationData
			};

			auxiliaryConfig.SSMInputs = GetPrimaryVehicleSSMInput(mission, primaryVehicle);

			simulationRunData.BusAuxiliaries = auxiliaryConfig;

			simulationRunData.Retarder = _retarderData;
			
			// todo: use driver data from initialize method
			simulationRunData.DriverData = DataAdapterPrimary.CreateDriverData(primarySegment, primaryVehicle);

			simulationRunData.Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString());

			return simulationRunData;

		}


		private SSMInputs GetPrimaryVehicleSSMInput(Mission mission, IVehicleDeclarationInputData primaryVehicle)
		{
			var ssmInputs = new SSMInputs(null);

			DataAdapterPrimary.SetSSMBusParameters(ssmInputs, mission);

			ssmInputs.Technologies = DataAdapterPrimary.CreateTechnologyBenefits(mission, primaryVehicle);

			DeclarationData.FactorMethodBus.SetBoundaryConditions(ssmInputs);
			DeclarationData.FactorMethodBus.SetEnvironmentalConditions(ssmInputs);

			DataAdapterPrimary.SetSSMInputs(ssmInputs, mission);

			return ssmInputs;
		}
	}
}
