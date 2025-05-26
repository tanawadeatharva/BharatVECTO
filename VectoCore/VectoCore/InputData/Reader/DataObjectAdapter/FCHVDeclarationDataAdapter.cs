using System;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public class FCHVDeclarationDataAdapter : AbstractSimulationDataAdapter
	{
		private static EngineeringDataAdapter DataAdapter = new EngineeringDataAdapter();

		public static FuelCellPowerMap CreateFuelCellPowerMap(
			IModalDataContainer modalData,
			FuelCellSystemData fcsData,
			BatterySystemData batterySystemData)
		{
			return DataAdapter.CreateFuelCellPowerMap(modalData, fcsData, batterySystemData);
		}

		public static FuelCellSystemShareMap CreateFuelCellShareMap(FuelCellSystemData fuelCellSystemData)
		{
			return DataAdapter.CreateFuelCellShareMap(fuelCellSystemData);
		}

		public static BatterySystemData CreateFuelCellPreProcessingBattery(
			FuelCellSystemData fuelCellSystemData,
			BatterySystemData batterySystemData,
			out Tuple<int, BatteryData> fuelCellBattery)
		{
			return DataAdapter.CreateFuelCellPreProcessingBattery(
				fuelCellSystemData,
				batterySystemData,
				out fuelCellBattery);
		}
	}
}
