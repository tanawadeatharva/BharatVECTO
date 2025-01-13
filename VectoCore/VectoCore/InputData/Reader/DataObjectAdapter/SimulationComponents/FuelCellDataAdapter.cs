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
							PowerOutput = r.Field<string>("powerOutput").ToDouble().SI<Watt>(),
							FuelConsumption = r.Field<string>("fuelConsumption").ToDouble().ConvertToKilogramPerSecond(),
						}).ToList(),
					}
				});
			}

			return new FuelCellSystemDeclarationData(modules);
		}
	}

	public static class DoubleExtensions
	{
		public static KilogramPerSecond ConvertToKilogramPerSecond(this double gramsPerHour)
		{
			double Kilo = 1000;
			double SecondsPerHour = 3600;

			return (gramsPerHour / (Kilo * SecondsPerHour)).SI<KilogramPerSecond>();
		}
	}
}
