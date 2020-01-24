using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;


namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public interface ISSMInputs
	{
		bool SSMDisabled { get; }

		ISSMBusParameters BusParameters { get; }

		ISSMTechnologies Technologies { get; }

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
		//string BusModel { get; }

		double NumberOfPassengers { get; }
		FloorType BusFloorType { get; }
		//bool DoubleDecker { get; }
		//Meter BusLength { get; }
		//Meter BusWidth { get; }
		//Meter BusHeight { get; }

		//SquareMeter BusFloorSurfaceArea { get; }
		SquareMeter BusWindowSurface { get; }
		SquareMeter BusSurfaceArea { get; }
		//CubicMeter BusVolume { get; }

		//PerSquareMeter PassengerDensityLowFloor { get; }
		//PerSquareMeter PassengerDensitySemiLowFloor { get; }
		//PerSquareMeter PassengerDensityRaisedFloor { get; }
		//double CalculatedPassengerNumber { get; }
	}

	public interface ISSMBoundaryConditions
	{
		// Boundary Conditions:			
		double GFactor { get; }

		double SolarClouding(Kelvin envTemp);

		Watt HeatPerPassengerIntoCabin(Kelvin envTemp);
		WattPerKelvinSquareMeter UValue { get; }
		Kelvin HeatingBoundaryTemperature { get; }
		Kelvin CoolingBoundaryTemperature { get; }
		Kelvin TemperatureCoolingTurnsOff { get; }
		PerSecond VentilationRate { get; }
		CubicMeterPerSecond VolumeExchange { get; }
		Watt VentPower { get; }
		JoulePerCubicMeter SpecificVentilationPower { get; }
		double AuxHeaterEfficiency { get; }
		JoulePerKilogramm GCVDieselOrHeatingOil { get; }
		//SquareMeterPerMeter WindowAreaPerUnitBusLength { get; }
		//SquareMeter FrontRearWindowArea { get; }
		Kelvin MaxTemperatureDeltaForLowFloorBusses { get; }
		double MaxPossibleBenefitFromTechnologyList { get; }
	}

	public interface IEnvironmentalConditions
	{
		// EnviromentalConditions				

		IEnvironmentalConditionsMapEntry DefaultConditions { get; }

		IEnvironmentalConditionsMap EnvironmentalConditionsMap { get; }
		//string EnviromentalConditions_BatchFile { get; }
		bool BatchMode { get; }

		string Source { get; }
	}

	public interface IACSystem
	{
		// AC-system				            
		ACCompressorType HVACCompressorType { get; }
		Watt HVACMaxCoolingPower { get; }
		double COP { get; }
	}

	public interface IVentilation
	{
		// Ventilation				
		bool VentilationOnDuringHeating { get; }

		bool VentilationWhenBothHeatingAndACInactive { get; }
		bool VentilationDuringAC { get; set; }

		//VentilationLevel VentilationFlowSettingWhenHeatingAndACInactive { get; }
		//VentilationLevel VentilationDuringHeating { get; }
		//VentilationLevel VentilationDuringCooling { get; }
	}

	
	public enum ACCompressorType
	{
		Unknown,
		None,
		TwoStage,
		ThreeStage,
		FourStage,
		Continuous
	}

	public static class ACCompressorTypeExtensions
	{
		public static ACCompressorType ParseEnum(string txt)
		{
			switch (txt) {
				case "2-stage": return ACCompressorType.TwoStage;
				case "3-stage": return ACCompressorType.ThreeStage;
				case "4-stage": return ACCompressorType.FourStage;
				default: return txt.ParseEnum<ACCompressorType>();
			}
		}

		public static string ToString(this ACCompressorType type)
		{
			switch (type) {
				case ACCompressorType.TwoStage: return "2-stage";
				case ACCompressorType.ThreeStage: return "3-stage";
				case ACCompressorType.FourStage: return "4-stage";
				default: return type.ToString();
			}
		}

		public static bool IsElectrical(this ACCompressorType type)
		{
			return type == ACCompressorType.Continuous;
		}

		public static bool IsMechanical(this ACCompressorType type)
		{
			return type != ACCompressorType.Continuous;
		}

		public static double COP(this ACCompressorType type, FloorType floortype)
		{
			var cop = 3.5;

			switch (type) {
				case ACCompressorType.None:
				case ACCompressorType.Unknown:
					return 0;
				case ACCompressorType.TwoStage:
					return cop;
				case ACCompressorType.ThreeStage:
				case ACCompressorType.FourStage:
					return cop * 1.02;
				case ACCompressorType.Continuous:
					return floortype == FloorType.LowFloor
						? cop * 1.04
						: cop * 1.06;
				default: throw new ArgumentOutOfRangeException();
			}
		}
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
