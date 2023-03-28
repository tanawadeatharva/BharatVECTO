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
		//DriverData CreateDriverData();
		//AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, Segment segment);
        //AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData);
        //AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData);
        //CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle, IEngineModeDeclarationInputData engineMode, Mission mission);
        //GearboxData CreateGearboxData(
        //	IVehicleDeclarationInputData inputData, VectoRunData runData,
        //	IShiftPolygonCalculator shiftPolygonCalc);
        //ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed);
        //RetarderData CreateRetarderData(IRetarderInputData retarderData);
        //PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData);
        //IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData, IBusAuxiliariesDeclarationData busAuxData, MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles);
        //AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData);
		VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission first, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> keyValuePair, bool allowVocational);
		RetarderData CreateRetarderData(IRetarderInputData retarderData, PowertrainPosition position = PowertrainPosition.HybridPositionNotSet);

		List<Tuple<PowertrainPosition, ElectricMotorData>> CreateIEPCElectricMachines(
			IIEPCDeclarationInputData iepc, Volt averageVoltage);
	}

	public interface ILorryDeclarationDataAdapter : IDeclarationDataAdapter
	{
		DriverData CreateDriverData(Segment segment);

		AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData);

		AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData);

		AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData);

		GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData,
			VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalc);

		PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData, IGearboxDeclarationInputData gbx);
		PTOData CreatePTOCycleData(IGearboxDeclarationInputData gbx, IPTOTransmissionInputData pto);



		ShiftStrategyParameters CreateGearshiftData(double axleRatio, PerSecond engineIdlingSpeed, GearboxType gearboxType, int gearsCount);

        //VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission,
        //    KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational);

        AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, Segment segment);

		CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle,
			IEngineModeDeclarationInputData engineMode, Mission mission);

		IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
			IBusAuxiliariesDeclarationData busAuxData, MissionType missionType, VehicleClass vehicleClass,
			Meter vehicleLength, int? numSteeredAxles, VectoSimulationJobType jobType);
		
		
		IList<Tuple<PowertrainPosition, ElectricMotorData>> CreateElectricMachines(IElectricMachinesDeclarationInputData electricMachines, IDictionary<PowertrainPosition, IList<Tuple<Volt, TableData>>> torqueLimits, Volt averageVoltage, GearList gears = null);

		void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
			VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData,
			Action<SuperCapData> setSuperCapData);

		//BatterySystemData CreateBatteryData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
		//	VectoSimulationJobType jobType,
		//	bool ovc);
		//SuperCapData CreateSuperCapData(IElectricStorageSystemDeclarationInputData componentsElectricStorage);
		
		HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
			SuperCapData runDataSuperCapData, Kilogram vehicleMass, VectoRunData.OvcHevMode ovcMode, LoadingType loading, VehicleClass vehicleClass, MissionType missionType);

		HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
			SuperCapData runDataSuperCapData,
			Kilogram vehicleMass,
			VectoRunData.OvcHevMode ovcMode,
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

		DriverData CreateDriverData(Segment segment);

		AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData);
		AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData);
		AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData);

		GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData,
			VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalc);

		ShiftStrategyParameters CreateGearshiftData(double axleRatio, PerSecond engineIdlingSpeed,
			GearboxType gearboxType, int gearsCount);

		CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle,
			IEngineModeDeclarationInputData engineMode, Mission mission);

		IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
			IBusAuxiliariesDeclarationData busAuxData, MissionType missionType, VehicleClass vehicleClass,
			Meter vehicleLength, int? numSteeredAxles, VectoSimulationJobType jobType);

		AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, Segment segment);

		IList<Tuple<PowertrainPosition, ElectricMotorData>> CreateElectricMachines(IElectricMachinesDeclarationInputData electricMachines, IDictionary<PowertrainPosition, IList<Tuple<Volt, TableData>>> torqueLimits, Volt averageVoltage, GearList gears = null);

		void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
			VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData,
			Action<SuperCapData> setSuperCapData);
	}

	public interface IGenericCompletedBusDeclarationDataAdapter : IDeclarationDataAdapter
	{

        AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, Segment segment);

		CombustionEngineData CreateEngineData(IVehicleDeclarationInputData primaryVehicle, int modeIdx,
			Mission mission);

		IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
			IBusAuxiliariesDeclarationData busAuxData, MissionType missionType, VehicleClass vehicleClass,
			Meter vehicleLength, int? numSteeredAxles, VectoSimulationJobType jobType);

		AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData);

		AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData);

		GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData,
			VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalc);

		ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed);

		DriverData CreateDriverData(Segment segment);

		IAuxiliaryConfig CreateBusAuxiliariesData(
			Mission mission, IVehicleDeclarationInputData vehicleData, VectoRunData runData);

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

		CombustionEngineData CreateEngineData(IVehicleDeclarationInputData primaryVehicle, int modeIdx,
			Mission mission);
	}

	public interface ISingleBusDeclarationDataAdapter
	{
		AirdragData CreateAirdragData(IVehicleDeclarationInputData completedVehicle, Mission mission);
		CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle,
			IEngineModeDeclarationInputData engineMode, Mission mission);
		DriverData CreateDriverData(Segment segment);
		AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gearboxInputData);
		AxleGearData CreateAxleGearData(IAxleGearInputData axleGearInputData);
		AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData);
		GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData,
			VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalc);
		RetarderData CreateRetarderData(IRetarderInputData retarderData);

		ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed);
		IEnumerable<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxInputData,
			IBusAuxiliariesDeclarationData busAuxInput, MissionType mission, VehicleClass segment, Meter vehicleLength,
			int? numSteeredAxles, VectoSimulationJobType jobType);
		IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle, IVehicleDeclarationInputData completedVehicle, VectoRunData simulationRunData);

		VehicleData CreateVehicleData(ISingleBusInputDataProvider vehicle, Segment segment, Mission mission,
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational);
	}
}