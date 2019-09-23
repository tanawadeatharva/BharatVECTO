namespace TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.HVAC
{
	public interface ISSMCalculate
	{
		ISSMRun Run1 { get; set; }
		ISSMRun Run2 { get; set; }


		double ElectricalWBase { get; }
		double MechanicalWBase { get; }
		double FuelPerHBase { get; }

		double ElectricalWAdjusted { get; }
		double MechanicalWBaseAdjusted { get; }
		double FuelPerHBaseAdjusted { get; }


		// BaseValues
		// - Heating
		double BaseHeatingW_Mechanical { get; }
		double BaseHeatingW_ElectricalCoolingHeating { get; }
		double BaseHeatingW_ElectricalVentilation { get; }
		double BaseHeatingW_FuelFiredHeating { get; }

		// Cooling                                                    
		double BaseCoolingW_Mechanical { get; }
		double BaseCoolingW_ElectricalCoolingHeating { get; }
		double BaseCoolingW_ElectricalVentilation { get; }
		double BaseCoolingW_FuelFiredHeating { get; }

		// Cooling
		double BaseVentilationW_Mechanical { get; }
		double BaseVentilationW_ElectricalCoolingHeating { get; }
		double BaseVentilationW_ElectricalVentilation { get; }
		double BaseVentilationW_FuelFiredHeating { get; }


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
