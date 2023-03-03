using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC
{
	public interface ISSMCalculate
	{
		
		Watt ElectricalWBase { get; }
		Watt MechanicalWBase { get; }
		//KilogramPerSecond FuelPerHBase { get; }

		Watt ElectricalWAdjusted { get; }
		Watt MechanicalWBaseAdjusted { get; }

        //Watt AverageHeatingPowerDemand { get; }

        //Watt AverageAuxHeaterPower { get; }

        //Watt AverageHeatingPowerHeatPumpElectric { get; }

        //Watt AverageHeatingPowerHeatPumpMech { get; }

        //Watt AverageHeatingPowerElectricHeater { get; }

        //double TechListAdjustedHeatingW_FuelFiredHeating { get; }

        HeaterPower CalculateAverageHeatingDemand();

	}
}
