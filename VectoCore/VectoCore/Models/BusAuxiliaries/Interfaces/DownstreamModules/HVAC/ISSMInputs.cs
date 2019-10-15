using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC
{
	public interface ISSMInputs
	{
		ISSMBusParameters BusParameters { get; }

		ISSMBoundaryConditions BoundaryConditions { get; }

		IEnvironmentalConditions EnvironmentalConditions { get; }

		IACSystem ACSystem { get; }

		IVentilation Ventilation { get; }

		IAuxHeater AuxHeater { get; }
	}

	public interface ISSMBusParameters
	{ 

		// Bus Parameterisation	
		string BP_BusModel { get; }

		double BP_NumberOfPassengers { get; }
		FloorType BP_BusFloorType { get; }
		bool BP_DoubleDecker { get; }
		Meter BP_BusLength { get; }
		Meter BP_BusWidth { get; }
		Meter BP_BusHeight { get; }

		SquareMeter BP_BusFloorSurfaceArea { get; }
		SquareMeter BP_BusWindowSurface { get; }
		SquareMeter BP_BusSurfaceArea { get; }
		CubicMeter BP_BusVolume { get; }

		PerSquareMeter BC_PassengerDensityLowFloor { get; }
		PerSquareMeter BC_PassengerDensitySemiLowFloor { get; }
		PerSquareMeter BC_PassengerDensityRaisedFloor { get; }
		double BC_CalculatedPassengerNumber { get; }

	}

	public interface ISSMBoundaryConditions
	{

		// Boundary Conditions:			
		double BC_GFactor { get; set; }

		double BC_SolarClouding { get; }
		Watt BC_HeatPerPassengerIntoCabinW { get; }
		Kelvin BC_PassengerBoundaryTemperature { get; set; }
		WattPerKelvinSquareMeter BC_UValues { get; }
		Kelvin BC_HeatingBoundaryTemperature { get; set; }
		Kelvin BC_CoolingBoundaryTemperature { get; set; }
		Kelvin BC_TemperatureCoolingTurnsOff { get; }
		PerSecond BC_HighVentilation { get; set; }
		PerSecond BC_lowVentilation { get; set; }
		CubicMeterPerSecond BC_High { get; }
		CubicMeterPerSecond BC_Low { get; }
		Watt BC_HighVentPower { get; }
		Watt BC_LowVentPower { get; }
		JoulePerCubicMeter BC_SpecificVentilationPower { get; set; }
		double BC_AuxHeaterEfficiency { get; set; }
		JoulePerKilogramm BC_GCVDieselOrHeatingOil { get; set; }
		SquareMeterPerMeter BC_WindowAreaPerUnitBusLength { get; }
		SquareMeter BC_FrontRearWindowArea { get; }
		Kelvin BC_MaxTemperatureDeltaForLowFloorBusses { get; set; }
		double BC_MaxPossibleBenefitFromTechnologyList { get; set; }
	}

	public interface IEnvironmentalConditions
	{

		// EnviromentalConditions				
		Kelvin EC_EnviromentalTemperature { get; set; }

		WattPerSquareMeter EC_Solar { get; set; }
		IEnvironmentalConditionsMap EC_EnvironmentalConditionsMap { get; }
		string EC_EnviromentalConditions_BatchFile { get; set; }
		bool EC_EnviromentalConditions_BatchEnabled { get; set; }

	}

	public interface IACSystem
	{

		// AC-system				            
		string AC_CompressorType { get; set; }

		string AC_CompressorTypeDerived { get; }
		Watt AC_CompressorCapacitykW { get; set; }
		double AC_COP { get; }

	}

	public interface IVentilation
	{
		// Ventilation				
		bool VEN_VentilationOnDuringHeating { get; set; }

		bool VEN_VentilationWhenBothHeatingAndACInactive { get; set; }
		bool VEN_VentilationDuringAC { get; set; }
		string VEN_VentilationFlowSettingWhenHeatingAndACInactive { get; set; }
		string VEN_VentilationDuringHeating { get; set; }
		string VEN_VentilationDuringCooling { get; set; }

	}

	public interface IAuxHeater
	{ 

		// Aux. Heater				
		Watt AH_EngineWasteHeatkW { get; set; }
		Watt AH_FuelFiredHeaterkW { get; set; }
		double AH_FuelEnergyToHeatToCoolant { get; set; }
		double AH_CoolantHeatTransferredToAirCabinHeater { get; set; }
	}
}
