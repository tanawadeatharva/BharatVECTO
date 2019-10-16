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
		string BusModel { get; }

		double NumberOfPassengers { get; }
		FloorType BusFloorType { get; }
		bool DoubleDecker { get; }
		Meter BusLength { get; }
		Meter BusWidth { get; }
		Meter BusHeight { get; }

		SquareMeter BusFloorSurfaceArea { get; }
		SquareMeter BusWindowSurface { get; }
		SquareMeter BusSurfaceArea { get; }
		CubicMeter BusVolume { get; }

		PerSquareMeter PassengerDensityLowFloor { get; }
		PerSquareMeter PassengerDensitySemiLowFloor { get; }
		PerSquareMeter PassengerDensityRaisedFloor { get; }
		double CalculatedPassengerNumber { get; }

	}

	public interface ISSMBoundaryConditions
	{
		// Boundary Conditions:			
		double GFactor { get; set; }
		double SolarClouding { get; }
		Watt HeatPerPassengerIntoCabin { get; }
		Kelvin PassengerBoundaryTemperature { get; set; }
		WattPerKelvinSquareMeter UValue { get; }
		Kelvin HeatingBoundaryTemperature { get; set; }
		Kelvin CoolingBoundaryTemperature { get; set; }
		Kelvin TemperatureCoolingTurnsOff { get; }
		PerSecond HighVentilation { get; set; }
		PerSecond LowVentilation { get; set; }
		CubicMeterPerSecond HighVolumeExchange { get; }
		CubicMeterPerSecond LowVolumeExchange { get; }
		Watt HighVentPower { get; }
		Watt LowVentPower { get; }
		JoulePerCubicMeter SpecificVentilationPower { get; set; }
		double AuxHeaterEfficiency { get; set; }
		JoulePerKilogramm GCVDieselOrHeatingOil { get; set; }
		SquareMeterPerMeter WindowAreaPerUnitBusLength { get; }
		SquareMeter FrontRearWindowArea { get; }
		Kelvin MaxTemperatureDeltaForLowFloorBusses { get; set; }
		double MaxPossibleBenefitFromTechnologyList { get; set; }
	}

	public interface IEnvironmentalConditions
	{
		// EnviromentalConditions				
		Kelvin EnviromentalTemperature { get; set; }
		WattPerSquareMeter Solar { get; set; }
		IEnvironmentalConditionsMap EnvironmentalConditionsMap { get; }
		string EnviromentalConditions_BatchFile { get; set; }
		bool EnviromentalConditions_BatchEnabled { get; set; }

	}

	public interface IACSystem
	{
		// AC-system				            
		string CompressorType { get; set; }
		string CompressorTypeDerived { get; }
		Watt CompressorCapacity { get; set; }
		double COP { get; }

	}

	public interface IVentilation
	{
		// Ventilation				
		bool VentilationOnDuringHeating { get; set; }

		bool VentilationWhenBothHeatingAndACInactive { get; set; }
		bool VentilationDuringAC { get; set; }
		string VentilationFlowSettingWhenHeatingAndACInactive { get; set; }
		string VentilationDuringHeating { get; set; }
		string VentilationDuringCooling { get; set; }

	}

	public interface IAuxHeater
	{ 
		// Aux. Heater				
		Watt EngineWasteHeatkW { get; set; }
		Watt FuelFiredHeaterkW { get; set; }
		double FuelEnergyToHeatToCoolant { get; set; }
		double CoolantHeatTransferredToAirCabinHeater { get; set; }
	}
}
