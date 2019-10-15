using System;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	// Used by SSMHVAC Class
	public class SSMInputs : ISSMInputs, ISSMBoundaryConditions, IEnvironmentalConditions, IACSystem, IVentilation,
		IAuxHeater, ISSMBusParameters
	{
		private string _EC_EnviromentalConditions_BatchFile;
		private IEnvironmentalConditionsMap _EC_EnvironmentalConditionsMap;

		protected internal VehicleData _vehicle;

		private string _vectoDir;


		//public IssmInputs(bool initialiseDefaults = false, string vectoDir = "")
		//{
		//	_vectoDir = vectoDir;
		//	BP_BusModel = "";
		//	BP_BusFloorType = "";
		//	EC_EnviromentalConditions_BatchFile = "";
		//	AC_CompressorType = "";
		//	VEN_VentilationDuringCooling = "";
		//	VEN_VentilationDuringHeating = "";
		//	VEN_VentilationFlowSettingWhenHeatingAndACInactive = "";
		//	if (initialiseDefaults)
		//		SetDefaults();
		//}

		public SSMInputs(string vectoDir = "")
		{
			_vectoDir = vectoDir;
			SetDefaults();
		}

		public SSMInputs(VehicleData vehicle)
		{
			_vehicle = vehicle;
		}


		// C4/D4
		public string BP_BusModel
		{
			get { return _vehicle.ModelName; }
		}

		// C5/D5
		public double BP_NumberOfPassengers
		{
			get { return _vehicle.PassengerCount; }
		}

		// C6/D6
		public FloorType BP_BusFloorType
		{
			get { return _vehicle.FloorType; }
		}

		// C10/D10
		public bool BP_DoubleDecker
		{
			get { return _vehicle.DoubleDecker; }
		}

		// D12/C12 - ( M )
		public Meter BP_BusLength
		{
			get { return _vehicle.Length; }
		}

		// D13/C13 - ( M )
		public Meter BP_BusWidth
		{
			get { return _vehicle.Width; }
		}

		// D14/C14 - ( M )
		public Meter BP_BusHeight
		{
			get { return _vehicle.Height; }
		}


		// D7/C7 - ( M/2 )
		public SquareMeter BP_BusFloorSurfaceArea
		{
			get {
				// =IF(AND(C6="low floor",C13<=2.55,C13>=2.5),(2.55*(C12-1.2)),((C12-1.2)*C13))
				if (BP_BusFloorType == FloorType.LowFloor && BP_BusWidth <= 2.55 && BP_BusWidth >= 2.5) {
					return 2.55.SI<Meter>() * (BP_BusLength - 1.2.SI<Meter>());
				}

				return ((BP_BusLength - 1.2.SI<Meter>()) * BP_BusWidth);
			}
		}

		// D8/C8 - ( M/2 )
		public SquareMeter BP_BusSurfaceArea
		{
			get {
				// 2 * (C12*C13 + C12*C14 + C13*C14)
				return 2 * ((BP_BusLength * BP_BusWidth) + (BP_BusLength * BP_BusHeight) + (BP_BusWidth * BP_BusHeight));
			}
		}

		// D9/C9 - ( M/2 )
		public SquareMeter BP_BusWindowSurface
		{
			get {
				// =(C40*C12)+C41
				return (BC_WindowAreaPerUnitBusLength * BP_BusLength) + BC_FrontRearWindowArea;
			}
		}


		// D11/C11 - ( M/3 )
		public CubicMeter BP_BusVolume
		{
			get {
				// =(C12*C13*C14)
				return BP_BusLength * BP_BusWidth * BP_BusHeight;
			}
		}


		// C17
		public double BC_GFactor { get; set; }

		// C18            
		public double BC_SolarClouding
		{
			get {
				// =IF(C46<17,0.65,0.8)
				return EC_EnviromentalTemperature < 17 ? 0.65 : 0.8;
			}
		}

		// C19 - ( W )
		public Watt BC_HeatPerPassengerIntoCabinW
		{
			get {
				// =IF(C46<17,50,80)
				return (EC_EnviromentalTemperature < 17 ? 50 : 80).SI<Watt>();
			}
		}

		// C20 - ( oC )
		public Kelvin BC_PassengerBoundaryTemperature { get; set; }

		// C21 - ( Passenger/Metre Squared )
		public PerSquareMeter BC_PassengerDensityLowFloor
		{
			get {
				// =IF($C$10="No",3,3.7)
				return (BP_DoubleDecker ? 3.7 : 3).SI<PerSquareMeter>();
			}
		}

		// C22 - ( Passenger/Metre Squared )
		public PerSquareMeter BC_PassengerDensitySemiLowFloor
		{
			get {
				// =IF($C$10="No",2.2,3)
				return (BP_DoubleDecker ? 3 : 2.2).SI<PerSquareMeter>();
			}
		}

		// C23 - ( Passenger/Metre Squared )
		public PerSquareMeter BC_PassengerDensityRaisedFloor
		{
			get {
				// =IF($C$10="No",1.4,2)
				return (BP_DoubleDecker ? 2 : 1.4).SI<PerSquareMeter>();
			}
		}

		// C24               
		public double BC_CalculatedPassengerNumber
		{
			get {
				// =ROUND(IF($D$5<IF(D6="low floor",C21,IF(D6="semi low floor",C22,C23))*D7,$D$5,IF(D6="low floor",C21,IF(D6="semi low floor",C22,C23))*D7),0)
				var tmp = (BP_BusFloorType == FloorType.LowFloor
							? BC_PassengerDensityLowFloor
							: BP_BusFloorType == FloorType.SemiLowFloor
								? BC_PassengerDensitySemiLowFloor
								: BC_PassengerDensityRaisedFloor) * BP_BusFloorSurfaceArea;
				return Math.Round(BP_NumberOfPassengers < tmp ? BP_NumberOfPassengers : tmp.Value(), 0);
			}
		}

		// C25 - ( W/K/M3 )
		public WattPerKelvinSquareMeter BC_UValues
		{
			get {
				// =IF(D6="low floor",4,IF(D6="semi low floor",3.5,3))
				return (BP_BusFloorType == FloorType.LowFloor ? 4 : BP_BusFloorType == FloorType.SemiLowFloor ? 3.5 : 3)
					.SI<WattPerKelvinSquareMeter>();
			}
		}

		// C26 - ( oC )
		public Kelvin BC_HeatingBoundaryTemperature { get; set; }

		// C27 - ( oC )
		public Kelvin BC_CoolingBoundaryTemperature { get; set; }

		// C28 - ( oC )
		public Kelvin BC_TemperatureCoolingTurnsOff
		{
			get { return 17.0.DegCelsiusToKelvin(); }
		}

		// C29 - ( L/H )  --- !! 1/h
		public PerSecond BC_HighVentilation { get; set; }

		// C30 - ( L/H )   --- !! 1/h
		public PerSecond BC_lowVentilation { get; set; }

		// C31 - ( M3/H )
		public CubicMeterPerSecond BC_High
		{
			get {
				// =D11*C29
				return BP_BusVolume * BC_HighVentilation;
			}
		}

		// C32 - ( M3/H )
		public CubicMeterPerSecond BC_Low
		{
			get {
				// =C30*D11
				return BP_BusVolume * BC_lowVentilation;
			}
		}

		// C33 - ( W )
		public Watt BC_HighVentPower
		{
			get {
				// =C31*C35
				return BC_High * BC_SpecificVentilationPower;
			}
		}

		// C34 - ( W )
		public Watt BC_LowVentPower
		{
			get {
				// =C32*C35
				return BC_Low * BC_SpecificVentilationPower;
			}
		}

		// C35 - ( Wh/M3 )
		public JoulePerCubicMeter BC_SpecificVentilationPower { get; set; }

		// C37               
		public double BC_AuxHeaterEfficiency { get; set; }

		// C38 - ( KW/HKG )
		public JoulePerKilogramm BC_GCVDieselOrHeatingOil { get; set; }

		// C40 - ( M2/M )
		public SquareMeterPerMeter BC_WindowAreaPerUnitBusLength
		{
			get {
				// =IF($C$10="No",1.5,2.5)
				return (BP_DoubleDecker ? 2.5 : 1.5).SI<SquareMeterPerMeter>();
			}
		}

		// C41 - ( M/2 )
		public SquareMeter BC_FrontRearWindowArea
		{
			get {
				// =IF($C$10="No",5,8)
				return (BP_DoubleDecker ? 8 : 5).SI<SquareMeter>();
			}
		}

		// C42 - ( K )
		public Kelvin BC_MaxTemperatureDeltaForLowFloorBusses { get; set; }

		// C43 - ( Fraction )
		public double BC_MaxPossibleBenefitFromTechnologyList { get; set; }


		// C46 - ( oC )
		public Kelvin EC_EnviromentalTemperature { get; set; }

		// C47 - ( W/M3 )
		public WattPerSquareMeter EC_Solar { get; set; }

		// ( EC_EnviromentalTemperature and  EC_Solar) (Batch Mode)
		public IEnvironmentalConditionsMap EC_EnvironmentalConditionsMap
		{
			get { return _EC_EnvironmentalConditionsMap; }
		}

		public string EC_EnviromentalConditions_BatchFile
		{
			get { return _EC_EnviromentalConditions_BatchFile; }
			set {
				_EC_EnvironmentalConditionsMap = new EnvironmentalConditionsMap(value, _vectoDir);
				_EC_EnviromentalConditions_BatchFile = value;
			}
		}

		public bool EC_EnviromentalConditions_BatchEnabled { get; set; }


		// C53 - "Continous/2-stage/3-stage/4-stage
		public string AC_CompressorType { get; set; }

		// mechanical/electrical
		public string AC_CompressorTypeDerived
		{
			get { return AC_CompressorType == "Continuous" ? "Electrical" : "Mechanical"; }
		}

		// C54 -  ( KW )
		public Watt AC_CompressorCapacitykW { get; set; }

		// C59
		public double AC_COP
		{
			get {
				var cop = 3.5D;

				if ((AC_CompressorType != null)) {
					cop = AC_CompressorType.ToLower() == "3-stage" ? cop * 1.02 : cop;
					cop = AC_CompressorType.ToLower() == "4-stage" ? cop * 1.02 : cop;
					cop = AC_CompressorType.ToLower() == "continuous"
						? BP_BusFloorType == FloorType.LowFloor
							? cop * 1.04
							: cop * 1.06
						: cop;
				}

				return Math.Round(cop, 2);
			}
		}


		// C62 - Boolean Yes/No
		public bool VEN_VentilationOnDuringHeating { get; set; }

		// C63 - Boolean Yes/No
		public bool VEN_VentilationWhenBothHeatingAndACInactive { get; set; }

		// C64 - Boolean Yes/No
		public bool VEN_VentilationDuringAC { get; set; }

		// C65 - String high/low
		public string VEN_VentilationFlowSettingWhenHeatingAndACInactive { get; set; }

		// C66 - String high/low
		public string VEN_VentilationDuringHeating { get; set; }

		// C67 - String high/low                                               
		public string VEN_VentilationDuringCooling { get; set; }


		// C70 - ( KW )
		public Watt AH_EngineWasteHeatkW { get; set; }

		// C71 - ( KW )
		public Watt AH_FuelFiredHeaterkW { get; set; }

		public double AH_FuelEnergyToHeatToCoolant { get; set; }

		public double AH_CoolantHeatTransferredToAirCabinHeater { get; set; }


		private void SetDefaults()
		{
			// BUS Parameterisation
			// ********************
			_vehicle = new VehicleData() {
				ModelName = "DummyBus",
				PassengerCount = 47.0,
				FloorType = FloorType.HighFloor, // "raised floor",
				DoubleDecker = false,
				Length = 10.655.SI<Meter>(),
				Width = 2.55.SI<Meter>(),
				Height = 2.275.SI<Meter>(),
			};

			// BP_BusFloorSurfaceArea  : Calculated
			// BP_BusSurfaceArea : Calculated
			// BP_BusWindowSurface    : Calculated
			// BP_BusVolume : Calculated

			// BOUNDRY CONDITIONS
			// ******************

			BC_GFactor = 0.95D;

			// BC_SolarClouding As Double :Calculated
			// BC_HeatPerPassengerIntoCabinW  :Calculated
			BC_PassengerBoundaryTemperature = 12.0.DegCelsiusToKelvin();

			// BC_PassengerDensityLowFloor :Calculated
			// BC_PassengerDensitySemiLowFloor :Calculated
			// BC_PassengerDensityRaisedFloor :Calculated
			// BC_CalculatedPassengerNumber  :Calculated
			// BC_UValues :Calculated
			BC_HeatingBoundaryTemperature = 18.0.DegCelsiusToKelvin();
			BC_CoolingBoundaryTemperature = 23.0.DegCelsiusToKelvin();

			// BC_CoolingBoundaryTemperature : ReadOnly Static
			BC_HighVentilation = 20.0.SI(Unit.SI.Per.Hour).Cast<PerSecond>();
			BC_lowVentilation = 7.0.SI(Unit.SI.Per.Hour).Cast<PerSecond>();

			// BC_High  :Calculated
			// BC_Low  :Calculated
			// BC_HighVentPower  :Calculated
			// BC_LowVentPower  :Calculated
			BC_SpecificVentilationPower = 0.56.SI(Unit.SI.Watt.Hour.Per.Cubic.Meter).Cast<JoulePerCubicMeter>();

			// BC_COP :Calculated
			BC_AuxHeaterEfficiency = 0.84D;
			BC_GCVDieselOrHeatingOil = 11.8.SI(Unit.SI.Kilo.Watt.Hour.Per.Kilo.Gramm).Cast<JoulePerKilogramm>();

			// BC_WindowAreaPerUnitBusLength   :Calculated 
			// BC_FrontRearWindowArea  :Calculated
			BC_MaxTemperatureDeltaForLowFloorBusses = 3.0.SI<Kelvin>();
			BC_MaxPossibleBenefitFromTechnologyList = 0.5D;

			// Environmental Conditions
			// ************************
			EC_EnviromentalTemperature = 25.0.DegCelsiusToKelvin();
			EC_Solar = 400.0.SI<WattPerSquareMeter>();
			EC_EnviromentalConditions_BatchEnabled = true;
			EC_EnviromentalConditions_BatchFile = "DefaultClimatic.aenv";

			// AC SYSTEM
			// *********
			AC_CompressorType = "2-stage";
			AC_CompressorCapacitykW = 18.0.SI(Unit.SI.Kilo.Watt).Cast<Watt>();

			// VENTILATION
			// ***********
			VEN_VentilationOnDuringHeating = true;
			VEN_VentilationWhenBothHeatingAndACInactive = true;
			VEN_VentilationDuringAC = true;
			VEN_VentilationFlowSettingWhenHeatingAndACInactive = "high";
			VEN_VentilationDuringHeating = "high";
			VEN_VentilationDuringCooling = "high";

			// AUX HEATER
			// **********
			AH_FuelFiredHeaterkW = 30.0.SI(Unit.SI.Kilo.Watt).Cast<Watt>();
			AH_FuelEnergyToHeatToCoolant = 0.2;
			AH_CoolantHeatTransferredToAirCabinHeater = 0.75;
			AH_EngineWasteHeatkW = 0.SI<Watt>();
		}

		#region Implementation of ISSMInputs

		public ISSMBusParameters BusParameters
		{
			get { return this; }
		}

		public ISSMBoundaryConditions BoundaryConditions
		{
			get { return this; }
		}

		public IEnvironmentalConditions EnvironmentalConditions
		{
			get { return this; }
		}

		public IACSystem ACSystem
		{
			get { return this; }
		}

		public IVentilation Ventilation
		{
			get { return this; }
		}

		public IAuxHeater AuxHeater
		{
			get { return this; }
		}

		#endregion
	}
}
