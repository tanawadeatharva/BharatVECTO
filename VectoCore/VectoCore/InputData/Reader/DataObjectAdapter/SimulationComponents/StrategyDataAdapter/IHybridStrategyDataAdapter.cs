using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.StrategyDataAdapter
{
	public interface IHybridStrategyDataAdapter
	{
		// parallel hybrid params
		HybridStrategyParameters CreateHybridStrategyParameters(
			BatterySystemData batterySystemData,
			SuperCapData superCap,
			VectoRunData.OvcHevMode ovcMode,
			LoadingType loading,
			VehicleClass vehicleClass,
			MissionType missionType, ArchitectureID archID, CombustionEngineData engineData, GearboxData gearboxData,
			TableData boostingLimitations);

		// serial hybrid params
		HybridStrategyParameters CreateHybridStrategyParameters(BatterySystemData batterySystemData,
			SuperCapData superCapData, Kilogram vehicleMass, VectoRunData.OvcHevMode ovcMode);
	}
}