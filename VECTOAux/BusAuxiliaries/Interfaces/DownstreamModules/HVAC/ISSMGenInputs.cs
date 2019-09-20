namespace DownstreamModules.HVAC
{
	public interface ISSMGenInputs
	{

		// Bus Parameterisation	
		string BP_BusModel { get; set; }
		double BP_NumberOfPassengers { get; set; }
		string BP_BusFloorType { get; set; }
		bool BP_DoubleDecker { get; set; }
		double BP_BusLength { get; set; }
		double BP_BusWidth { get; set; }
		double BP_BusHeight { get; set; }

		double BP_BusFloorSurfaceArea { get; }
		double BP_BusWindowSurface { get; }
		double BP_BusSurfaceAreaM2 { get; }
		double BP_BusVolume { get; }

		// Boundary Conditions:			
		double BC_GFactor { get; set; }
		double BC_SolarClouding { get; }
		double BC_HeatPerPassengerIntoCabinW { get; }
		double BC_PassengerBoundaryTemperature { get; set; }
		double BC_PassengerDensityLowFloor { get; }
		double BC_PassengerDensitySemiLowFloor { get; }
		double BC_PassengerDensityRaisedFloor { get; }
		double BC_CalculatedPassengerNumber { get; }
		double BC_UValues { get; }
		double BC_HeatingBoundaryTemperature { get; set; }
		double BC_CoolingBoundaryTemperature { get; set; }
		double BC_TemperatureCoolingTurnsOff { get; }
		double BC_HighVentilation { get; set; }
		double BC_lowVentilation { get; set; }
		double BC_High { get; }
		double BC_Low { get; }
		double BC_HighVentPowerW { get; }
		double BC_LowVentPowerW { get; }
		double BC_SpecificVentilationPower { get; set; }
		double BC_AuxHeaterEfficiency { get; set; }
		double BC_GCVDieselOrHeatingOil { get; set; }
		double BC_WindowAreaPerUnitBusLength { get; }
		double BC_FrontRearWindowArea { get; }
		double BC_MaxTemperatureDeltaForLowFloorBusses { get; set; }
		double BC_MaxPossibleBenefitFromTechnologyList { get; set; }

		// EnviromentalConditions				
		double EC_EnviromentalTemperature { get; set; }
		double EC_Solar { get; set; }
		IEnvironmentalConditionsMap EC_EnvironmentalConditionsMap { get; }
		string EC_EnviromentalConditions_BatchFile { get; set; }
		bool EC_EnviromentalConditions_BatchEnabled { get; set; }

		// AC-system				            
		string AC_CompressorType { get; set; }
		string AC_CompressorTypeDerived { get; }
		double AC_CompressorCapacitykW { get; set; }
		double AC_COP { get; }

		// Ventilation				
		bool VEN_VentilationOnDuringHeating { get; set; }
		bool VEN_VentilationWhenBothHeatingAndACInactive { get; set; }
		bool VEN_VentilationDuringAC { get; set; }
		string VEN_VentilationFlowSettingWhenHeatingAndACInactive { get; set; }
		string VEN_VentilationDuringHeating { get; set; }
		string VEN_VentilationDuringCooling { get; set; }

		// Aux. Heater				
		double AH_EngineWasteHeatkW { get; set; }
		double AH_FuelFiredHeaterkW { get; set; }
		double AH_FuelEnergyToHeatToCoolant { get; set; }
		double AH_CoolantHeatTransferredToAirCabinHeater { get; set; }
	}
}
