using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC
{
	public interface ISSMCalculate
	{
		//ISSMRun Run1 { get; set; }
		//ISSMRun Run2 { get; set; }


		Watt ElectricalWBase { get; }
		Watt MechanicalWBase { get; }
		KilogramPerSecond FuelPerHBase { get; }

		Watt ElectricalWAdjusted { get; }
		Watt MechanicalWBaseAdjusted { get; }
		KilogramPerSecond FuelPerHBaseAdjusted { get; }


		// BaseValues
		// - Heating
		Watt BaseHeatingW_Mechanical { get; }
		Watt BaseHeatingW_ElectricalCoolingHeating { get; }
		Watt BaseHeatingW_ElectricalVentilation { get; }
		Watt BaseHeatingW_FuelFiredHeating { get; }

		// Cooling                                                    
		Watt BaseCoolingW_Mechanical { get; }
		Watt BaseCoolingW_ElectricalCoolingHeating { get; }
		Watt BaseCoolingW_ElectricalVentilation { get; }
		Watt BaseCoolingW_FuelFiredHeating { get; }

		// Cooling
		Watt BaseVentilationW_Mechanical { get; }
		Watt BaseVentilationW_ElectricalCoolingHeating { get; }
		Watt BaseVentilationW_ElectricalVentilation { get; }
		Watt BaseVentilationW_FuelFiredHeating { get; }


		// TechListBenefits
		// - Heating
		double TechListAdjustedHeatingW_Mechanical { get; }
		double TechListAdjustedHeatingW_ElectricalCoolingHeating { get; }
		double TechListAdjustedHeatingW_ElectricalVentilation { get; }
		double TechListAdjustedHeatingW_FuelFiredHeating { get; }

		// Cooling          TechListAdjusted                                      
		double TechListAdjustedCoolingW_Mechanical { get; }
		double TechListAdjustedCoolingW_ElectricalCoolingHeating { get; }
		double TechListAdjustedCoolingW_ElectricalVentilation { get; }
		double TechListAdjustedCoolingW_FuelFiredHeating { get; }

		// Cooling          TechListAdjusted
		double TechListAdjustedVentilationW_Mechanical { get; }
		double TechListAdjustedVentilationW_ElectricalCoolingHeating { get; }
		double TechListAdjustedVentilationW_ElectricalVentilation { get; }
		double TechListAdjustedVentilationW_FuelFiredHeating { get; }
	}
}
