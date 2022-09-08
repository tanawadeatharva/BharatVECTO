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

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public interface IDeclarationDataAdapter
	{
		//DriverData CreateDriverData();
		VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational);
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
    }

	public interface ILorryDeclarationDataAdapter : IDeclarationDataAdapter
	{
		DriverData CreateDriverData();

		AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData);

		AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData);

		AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData);

		GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData,
			VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalc);

		PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData);

		RetarderData CreateRetarderData(IRetarderInputData retarderData);

		ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed);

        //VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission,
        //    KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational);

        AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, Segment segment);

		CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle,
			IEngineModeDeclarationInputData engineMode, Mission mission);

		IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
			IBusAuxiliariesDeclarationData busAuxData, MissionType missionType, VehicleClass vehicleClass,
			Meter vehicleLength, int? numSteeredAxles);
	}

	public interface IPrimaryBusDeclarationDataAdapter : IDeclarationDataAdapter
	{
		IAuxiliaryConfig CreateBusAuxiliariesData(
			Mission mission, IVehicleDeclarationInputData vehicleData, VectoRunData runData);

		DriverData CreateDriverData();

		AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData);
		AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData);
		AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData);

		GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData,
			VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalc);

		RetarderData CreateRetarderData(IRetarderInputData retarderData);
		PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData);

		ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed);

		//VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission,
		//    KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational);

		CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle,
			IEngineModeDeclarationInputData engineMode, Mission mission);

		IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
			IBusAuxiliariesDeclarationData busAuxData, MissionType missionType, VehicleClass vehicleClass,
			Meter vehicleLength, int? numSteeredAxles);

		AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, Segment segment);
	}

	public interface IGenericCompletedBusDeclarationDataAdapter : IDeclarationDataAdapter
	{
        //VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission,
        //    KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational);

        AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, Segment segment);

		CombustionEngineData CreateEngineData(IVehicleDeclarationInputData primaryVehicle, int modeIdx,
			Mission mission);

		IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
			IBusAuxiliariesDeclarationData busAuxData, MissionType missionType, VehicleClass vehicleClass,
			Meter vehicleLength, int? numSteeredAxles);

		AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData);

		AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData);

		GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData,
			VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalc);

		ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed);

		RetarderData CreateRetarderData(IRetarderInputData retarderData);

		DriverData CreateDriverData();

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
			Meter vehicleLength, int? numSteeredAxles);

		AirdragData CreateAirdragData(IVehicleDeclarationInputData completedVehicle, Mission mission);

		CombustionEngineData CreateEngineData(IVehicleDeclarationInputData primaryVehicle, int modeIdx,
			Mission mission);
	}

	public interface ISingleBusDeclarationDataAdapter : IDeclarationDataAdapter
	{
		AirdragData CreateAirdragData(IVehicleDeclarationInputData completedVehicle, Mission mission);
		CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle,
			IEngineModeDeclarationInputData engineMode, Mission mission);
		DriverData CreateDriverData();
		AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gearboxInputData);
		AxleGearData CreateAxleGearData(IAxleGearInputData axleGearInputData);
		AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData);
		GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData,
			VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalc);
		RetarderData CreateRetarderData(IRetarderInputData retarderData);

		ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed);
		IEnumerable<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxInputData, IBusAuxiliariesDeclarationData busAuxInput, MissionType mission, VehicleClass segment, Meter vehicleLength, int? numSteeredAxles);
		IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle, IVehicleDeclarationInputData completedVehicle, VectoRunData simulationRunData);
	}
}