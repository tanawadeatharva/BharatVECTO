using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter {
	public interface IDeclarationDataAdapter
	{
		DriverData CreateDriverData();
		VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Mission mission, Kilogram loading);
		AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, Segment segment);
		AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData);
		AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData);
		CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle, IEngineModeDeclarationInputData engineMode, Mission mission);

		GearboxData CreateGearboxData(
			IGearboxDeclarationInputData gearboxData, CombustionEngineData engineData, double axleGearRatio, Meter rDyn,
			VehicleCategory vehicleCategory, ITorqueConverterDeclarationInputData torqueConverterData);

		RetarderData CreateRetarderData(IRetarderInputData retarderData);
		PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData);
		IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData, MissionType missionType, VehicleClass vehicleClass);
	}
}