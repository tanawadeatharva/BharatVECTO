using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
	public class FuelCellDataAdapter : IFuelCellDataAdapter
	{
		public FuelCellSystemDeclarationData CreateFuelCells(IFuelCellSystemDeclarationInputData fuelCellSystem)
		{
			int KiloWattFactor = 1000;
			List<FuelCellModuleData> modules = new List<FuelCellModuleData>();
			foreach(var fuelCellModule in fuelCellSystem.FuelCellModules)
			{
				modules.Add(new FuelCellModuleData()
				{
					Count = fuelCellModule.Count,
					MaxPower = fuelCellModule.MaxPower,
					MinPower = fuelCellModule.MinPower,
					FuelCell = new FuelCellComponentData()
					{
						FCSRatedPower = fuelCellModule.FuelCell.FCSRatedPower,
						PowerOutputConsumptionMap = fuelCellModule.FuelCell.FuelCellPowerOutputConsumptionMap
						.AsEnumerable()
						.Select(r => new PowerOutputConsumption
						{
							PowerOutput = r.Field<string>("powerOutput").ToDouble().SI<Watt>() * KiloWattFactor,
							FuelConsumption = r.Field<string>("fuelConsumption").ToDouble().SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>()
						}).ToList(),
					}
				});
			}

			return new FuelCellSystemDeclarationData(modules);
		}
	}
}
