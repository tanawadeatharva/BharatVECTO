using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public interface IDeclarationDataAdapter
	{
		DriverData CreateDriverData(Segment segment);

		VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission first, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> keyValuePair, bool allowVocational);
		
		GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData,
			VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalc);

		ShiftStrategyParameters CreateGearshiftData(double axleRatio, PerSecond engineIdlingSpeed, GearboxType gearboxType, int gearsCount);

		RetarderData CreateRetarderData(IRetarderInputData retarderData, PowertrainPosition position = PowertrainPosition.HybridPositionNotSet);
		
		AxleGearData CreateAxleGearData(IAxleGearInputData axleGearInputData);

		AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData);

        List<Tuple<PowertrainPosition, ElectricMotorData>> CreateIEPCElectricMachines(
			IIEPCDeclarationInputData iepc, Volt averageVoltage);

		IList<Tuple<PowertrainPosition, ElectricMotorData>> CreateElectricMachines(IElectricMachinesDeclarationInputData electricMachines, IDictionary<PowertrainPosition, IList<Tuple<Volt, TableData>>> torqueLimits, Volt averageVoltage, GearList gears = null);

		void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
			VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData,
			Action<SuperCapData> setSuperCapData);
    }

    public interface ILorryDeclarationDataAdapter : IDeclarationDataAdapter
	{

		AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData);

		PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData, IGearboxDeclarationInputData gbx);
		PTOData CreatePTOCycleData(IGearboxDeclarationInputData gbx, IPTOTransmissionInputData pto);

        AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, Segment segment);

		CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle,
			IEngineModeDeclarationInputData engineMode, Mission mission);

		IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
			IBusAuxiliariesDeclarationData busAuxData, MissionType missionType, VehicleClass vehicleClass,
			Meter vehicleLength, int? numSteeredAxles, VectoSimulationJobType jobType);
		
		HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
			SuperCapData runDataSuperCapData, Kilogram vehicleMass, OvcHevMode ovcMode, LoadingType loading, VehicleClass vehicleClass, MissionType missionType);

		HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
			SuperCapData runDataSuperCapData,
			Kilogram vehicleMass,
			OvcHevMode ovcMode,
			LoadingType loading,
			VehicleClass vehicleClass,
			MissionType missionType,
			TableData boostingLimitations,
			GearboxData gearboxData,
			CombustionEngineData engineData,
			ArchitectureID archId);
	}

	public interface IPrimaryBusDeclarationDataAdapter : IDeclarationDataAdapter
	{
		IAuxiliaryConfig CreateBusAuxiliariesData(
			Mission mission, IVehicleDeclarationInputData vehicleData, VectoRunData runData);

		AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData);
		
		CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle,
			IEngineModeDeclarationInputData engineMode, Mission mission);

		IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
			IBusAuxiliariesDeclarationData busAuxData, MissionType missionType, VehicleClass vehicleClass,
			Meter vehicleLength, int? numSteeredAxles, VectoSimulationJobType jobType);

		AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, Segment segment);

		// serial hybrid strategy
		HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
			SuperCapData runDataSuperCapData, Kilogram vehicleMass, OvcHevMode ovcMode, LoadingType loading, VehicleClass vehicleClass, MissionType missionType);

		// paralllel hybrid strategy
        HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
			SuperCapData runDataSuperCapData, Kilogram vehicleMass, OvcHevMode ovcMode,
			LoadingType loading, VehicleClass vehicleClass, MissionType missionType, TableData boostingLimitations,
			GearboxData gearboxData, CombustionEngineData engineData, ArchitectureID architectureId);
	}

	public interface IGenericCompletedBusDeclarationDataAdapter : IDeclarationDataAdapter
	{
		AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, Segment segment);

		CombustionEngineData CreateEngineData(IVehicleDeclarationInputData primaryVehicle, int modeIdx,
			Mission mission);

		IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
			IBusAuxiliariesDeclarationData busAuxData, MissionType missionType, VehicleClass vehicleClass,
			Meter vehicleLength, int? numSteeredAxles, VectoSimulationJobType jobType);

		IAuxiliaryConfig CreateBusAuxiliariesData(
			Mission mission, IVehicleDeclarationInputData primaryVehicle, IVehicleDeclarationInputData completedVehicle, VectoRunData runData);

		// serial hybrid strategy
        HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
			SuperCapData runDataSuperCapData, Kilogram vehicleMass, OvcHevMode ovcMode, LoadingType loading, VehicleClass vehicleClass, MissionType missionType);

		// parallel hybrid strategy
        HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
			SuperCapData runDataSuperCapData, Kilogram vehicleMass, OvcHevMode ovcMode,
			LoadingType loading, VehicleClass vehicleClass, MissionType missionType, TableData boostingLimitations,
			GearboxData gearboxData, CombustionEngineData engineData, ArchitectureID architectureId);

    }

	public interface ISpecificCompletedBusDeclarationDataAdapter
	{
		IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle,
			IVehicleDeclarationInputData completedVehicle, VectoRunData runData);

		VehicleData CreateVehicleData(IVehicleDeclarationInputData primaryVehicle,
			IVehicleDeclarationInputData completedVehicle, Segment segment, Mission mission,
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading);

		IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
			IBusAuxiliariesDeclarationData busAuxData, MissionType missionType, VehicleClass vehicleClass,
			Meter vehicleLength, int? numSteeredAxles, VectoSimulationJobType jobType);

		AirdragData CreateAirdragData(IVehicleDeclarationInputData completedVehicle, Mission mission);

	}

	public interface ISingleBusDeclarationDataAdapter : IDeclarationDataAdapter
	{
		AirdragData CreateAirdragData(IVehicleDeclarationInputData completedVehicle, Mission mission);
		CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle, 
			IEngineModeDeclarationInputData engineMode, Mission mission);
		
		
		IEnumerable<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxInputData,
			IBusAuxiliariesDeclarationData busAuxInput, MissionType mission, VehicleClass segment, Meter vehicleLength,
			int? numSteeredAxles, VectoSimulationJobType jobType);
		IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle, IVehicleDeclarationInputData completedVehicle, VectoRunData simulationRunData);

		VehicleData CreateVehicleData(ISingleBusInputDataProvider vehicle, Segment segment, Mission mission,
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational);

		// serial hybrid strategy
		HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
			SuperCapData runDataSuperCapData, Kilogram vehicleMass, OvcHevMode ovcMode, LoadingType loading, VehicleClass vehicleClass, MissionType missionType);

		// parallel hybrid strategy
        HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
			SuperCapData runDataSuperCapData, Kilogram vehicleMass, OvcHevMode ovcMode,
			LoadingType loading, VehicleClass vehicleClass, MissionType missionType, TableData boostingLimitations,
			GearboxData gearboxData, CombustionEngineData engineData, ArchitectureID architectureId);
    }
}