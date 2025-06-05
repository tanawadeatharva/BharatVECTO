using System;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public class FCHVDeclarationDataAdapter : AbstractSimulationDataAdapter
	{
		private EngineeringDataAdapter dataAdapter;

		public FCHVDeclarationDataAdapter(DataSource dataSource)
		{
			dataAdapter = new EngineeringDataAdapter()
			{
				JobFilePath = dataSource.SourceFile
			};
		}

		public FuelCellPowerMap CreateFuelCellPowerMap(
			IModalDataContainer modalData,
			FuelCellSystemData fcsData,
			BatterySystemData batterySystemData)
		{
			return dataAdapter.CreateFuelCellPowerMap(modalData, fcsData, batterySystemData);
		}

		public FuelCellSystemShareMap CreateFuelCellShareMap(FuelCellSystemData fuelCellSystemData)
		{
			return dataAdapter.CreateFuelCellShareMap(fuelCellSystemData);
		}

		public BatterySystemData CreateFuelCellPreProcessingBattery(
			FuelCellSystemData fuelCellSystemData,
			BatterySystemData batterySystemData,
			out Tuple<int, BatteryData> fuelCellBattery)
		{
			return dataAdapter.CreateFuelCellPreProcessingBattery(
				fuelCellSystemData,
				batterySystemData,
				out fuelCellBattery);
		}
	}
}
