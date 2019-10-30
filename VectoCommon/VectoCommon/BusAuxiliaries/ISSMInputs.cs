using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;


namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public interface ISSMInputs
	{
		bool SSMDisabled { get; }

		ISSMBusParameters BusParameters { get; }

		ITechlistBenefitLines Technologies { get; }

		ISSMBoundaryConditions BoundaryConditions { get; }

		IEnvironmentalConditions EnvironmentalConditions { get; }

		IACSystem ACSystem { get; }

		IVentilation Ventilation { get; }

		IAuxHeater AuxHeater { get; }
		string Source { get; }
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
		double GFactor { get; }

		double SolarClouding(Kelvin envTemp);

		Watt HeatPerPassengerIntoCabin(Kelvin envTemp);
		//Kelvin PassengerBoundaryTemperature { get; }
		WattPerKelvinSquareMeter UValue { get; }
		Kelvin HeatingBoundaryTemperature { get; }
		Kelvin CoolingBoundaryTemperature { get; }
		Kelvin TemperatureCoolingTurnsOff { get; }
		PerSecond HighVentilation { get; }
		PerSecond LowVentilation { get; }
		CubicMeterPerSecond HighVolumeExchange { get; }
		CubicMeterPerSecond LowVolumeExchange { get; }
		Watt HighVentPower { get; }
		Watt LowVentPower { get; }
		JoulePerCubicMeter SpecificVentilationPower { get; }
		double AuxHeaterEfficiency { get; }
		JoulePerKilogramm GCVDieselOrHeatingOil { get; }
		SquareMeterPerMeter WindowAreaPerUnitBusLength { get; }
		SquareMeter FrontRearWindowArea { get; }
		Kelvin MaxTemperatureDeltaForLowFloorBusses { get; }
		double MaxPossibleBenefitFromTechnologyList { get; }
	}

	public interface IEnvironmentalConditions
	{
		// EnviromentalConditions				
		//Kelvin EnviromentalTemperature { get; }

		//WattPerSquareMeter Solar { get; }

		IEnvironmentalConditionsMapEntry DefaultConditions { get; }

		IEnvironmentalConditionsMap EnvironmentalConditionsMap { get; }
		//string EnviromentalConditions_BatchFile { get; }
		bool BatchMode { get; }

		string Source { get; }
	}

	public interface IACSystem
	{
		// AC-system				            
		string CompressorType { get; }

		string CompressorTypeDerived { get; }
		Watt CompressorCapacity { get; }
		double COP { get; }
	}

	public interface IVentilation
	{
		// Ventilation				
		bool VentilationOnDuringHeating { get; }

		bool VentilationWhenBothHeatingAndACInactive { get; }
		bool VentilationDuringAC { get; set; }
		VentilationLevel VentilationFlowSettingWhenHeatingAndACInactive { get; }
		VentilationLevel VentilationDuringHeating { get; }
		VentilationLevel VentilationDuringCooling { get; }
	}

	public enum VentilationLevel
	{
		Low,
		High
	}

	public interface IAuxHeater
	{
		// Aux. Heater				
		//Watt EngineWasteHeatkW { get; }

		Watt FuelFiredHeaterPower { get; }
		double FuelEnergyToHeatToCoolant { get; }
		double CoolantHeatTransferredToAirCabinHeater { get; }
	}
}
