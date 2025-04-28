namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	using System;
	using System.Collections.Generic;
	using TUGraz.VectoCommon.Utils;
	using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;

	public class FuelCellSystemDeclarationData
	{
        public FuelCellSystemDeclarationData(IList<FuelCellModuleData> _fuelCellModules)
		{
			FuelCellModules = _fuelCellModules ?? throw new ArgumentNullException();
        }
        
		public IList<FuelCellModuleData> FuelCellModules { get; }

		public FuelCellSystemData ConvertToEngineeringData()
		{
			var fcStrings = new List<FuelCellStringData>();
			foreach (var module in FuelCellModules)
			{
				fcStrings.Add(new FuelCellStringData(module));
			}

			var fcSystemData = new FuelCellSystemData();
			fcSystemData.FuelCellStrings = fcStrings;

			return fcSystemData;
		}
	}

	public class FuelCellModuleData
	{
		public int Count { get; internal set; }

		public Watt MaxPower { get; internal set; }

		public Watt MinPower { get; internal set; }

		public FuelCellComponentData FuelCell { get; set; }
	}

	public class FuelCellComponentData
	{
		public Watt FCSRatedPower { get; internal set; }

		public IList<PowerOutputConsumption> PowerOutputConsumptionMap { get; internal set; }
	}

	public class PowerOutputConsumption
	{
		public Watt PowerOutput { get; set; }

		public KilogramPerSecond FuelConsumption { get; set; }
	}
}
