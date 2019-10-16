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
		public string BusModel
		{
			get { return _vehicle.ModelName; }
		}

		// C5/D5
		public double NumberOfPassengers
		{
			get { return _vehicle.PassengerCount; }
		}

		// C6/D6
		public FloorType BusFloorType
		{
			get { return _vehicle.FloorType; }
		}

		// C10/D10
		public bool DoubleDecker
		{
			get { return _vehicle.DoubleDecker; }
		}

		// D12/C12 - ( M )
		public Meter BusLength
		{
			get { return _vehicle.Length; }
		}

		// D13/C13 - ( M )
		public Meter BusWidth
		{
			get { return _vehicle.Width; }
		}

		// D14/C14 - ( M )
		public Meter BusHeight
		{
			get { return _vehicle.Height; }
		}


		// D7/C7 - ( M/2 )
		public SquareMeter BusFloorSurfaceArea
		{
			get {
				// =IF(AND(C6="low floor",C13<=2.55,C13>=2.5),(2.55*(C12-1.2)),((C12-1.2)*C13))
				if (BusFloorType == FloorType.LowFloor && BusWidth <= 2.55 && BusWidth >= 2.5) {
					return 2.55.SI<Meter>() * (BusLength - 1.2.SI<Meter>());
				}

				return ((BusLength - 1.2.SI<Meter>()) * BusWidth);
			}
		}

		// D8/C8 - ( M/2 )
		public SquareMeter BusSurfaceArea
		{
			get {
				// 2 * (C12*C13 + C12*C14 + C13*C14)
				return 2 * ((BusLength * BusWidth) + (BusLength * BusHeight) + (BusWidth * BusHeight));
			}
		}

		// D9/C9 - ( M/2 )
		public SquareMeter BusWindowSurface
		{
			get {
				// =(C40*C12)+C41
				return (WindowAreaPerUnitBusLength * BusLength) + FrontRearWindowArea;
			}
		}


		// D11/C11 - ( M/3 )
		public CubicMeter BusVolume
		{
			get {
				// =(C12*C13*C14)
				return BusLength * BusWidth * BusHeight;
			}
		}


		// C17
		public double GFactor { get; set; }

		// C18            
		public double SolarClouding
		{
			get {
				// =IF(C46<17,0.65,0.8)
				return EnviromentalTemperature < 17 ? 0.65 : 0.8;
			}
		}

		// C19 - ( W )
		public Watt HeatPerPassengerIntoCabin
		{
			get {
				// =IF(C46<17,50,80)
				return (EnviromentalTemperature < 17 ? 50 : 80).SI<Watt>();
			}
		}

		// C20 - ( oC )
		public Kelvin PassengerBoundaryTemperature { get; set; }

		// C21 - ( Passenger/Metre Squared )
		public PerSquareMeter PassengerDensityLowFloor
		{
			get {
				// =IF($C$10="No",3,3.7)
				return (DoubleDecker ? 3.7 : 3).SI<PerSquareMeter>();
			}
		}

		// C22 - ( Passenger/Metre Squared )
		public PerSquareMeter PassengerDensitySemiLowFloor
		{
			get {
				// =IF($C$10="No",2.2,3)
				return (DoubleDecker ? 3 : 2.2).SI<PerSquareMeter>();
			}
		}

		// C23 - ( Passenger/Metre Squared )
		public PerSquareMeter PassengerDensityRaisedFloor
		{
			get {
				// =IF($C$10="No",1.4,2)
				return (DoubleDecker ? 2 : 1.4).SI<PerSquareMeter>();
			}
		}

		// C24               
		public double CalculatedPassengerNumber
		{
			get {
				// =ROUND(IF($D$5<IF(D6="low floor",C21,IF(D6="semi low floor",C22,C23))*D7,$D$5,IF(D6="low floor",C21,IF(D6="semi low floor",C22,C23))*D7),0)
				var tmp = (BusFloorType == FloorType.LowFloor
							? PassengerDensityLowFloor
							: BusFloorType == FloorType.SemiLowFloor
								? PassengerDensitySemiLowFloor
								: PassengerDensityRaisedFloor) * BusFloorSurfaceArea;
				return Math.Round(NumberOfPassengers < tmp ? NumberOfPassengers : tmp.Value(), 0);
			}
		}

		// C25 - ( W/K/M3 )
		public WattPerKelvinSquareMeter UValue
		{
			get {
				// =IF(D6="low floor",4,IF(D6="semi low floor",3.5,3))
				return (BusFloorType == FloorType.LowFloor ? 4 : BusFloorType == FloorType.SemiLowFloor ? 3.5 : 3)
					.SI<WattPerKelvinSquareMeter>();
			}
		}

		// C26 - ( oC )
		public Kelvin HeatingBoundaryTemperature { get; set; }

		// C27 - ( oC )
		public Kelvin CoolingBoundaryTemperature { get; set; }

		// C28 - ( oC )
		public Kelvin TemperatureCoolingTurnsOff
		{
			get { return 17.0.DegCelsiusToKelvin(); }
		}

		// C29 - ( L/H )  --- !! 1/h
		public PerSecond HighVentilation { get; set; }

		// C30 - ( L/H )   --- !! 1/h
		public PerSecond LowVentilation { get; set; }

		// C31 - ( M3/H )
		public CubicMeterPerSecond HighVolumeExchange
		{
			get {
				// =D11*C29
				return BusVolume * HighVentilation;
			}
		}

		// C32 - ( M3/H )
		public CubicMeterPerSecond LowVolumeExchange
		{
			get {
				// =C30*D11
				return BusVolume * LowVentilation;
			}
		}

		// C33 - ( W )
		public Watt HighVentPower
		{
			get {
				// =C31*C35
				return HighVolumeExchange * SpecificVentilationPower;
			}
		}

		// C34 - ( W )
		public Watt LowVentPower
		{
			get {
				// =C32*C35
				return LowVolumeExchange * SpecificVentilationPower;
			}
		}

		// C35 - ( Wh/M3 )
		public JoulePerCubicMeter SpecificVentilationPower { get; set; }

		// C37               
		public double AuxHeaterEfficiency { get; set; }

		// C38 - ( KW/HKG )
		public JoulePerKilogramm GCVDieselOrHeatingOil { get; set; }

		// C40 - ( M2/M )
		public SquareMeterPerMeter WindowAreaPerUnitBusLength
		{
			get {
				// =IF($C$10="No",1.5,2.5)
				return (DoubleDecker ? 2.5 : 1.5).SI<SquareMeterPerMeter>();
			}
		}

		// C41 - ( M/2 )
		public SquareMeter FrontRearWindowArea
		{
			get {
				// =IF($C$10="No",5,8)
				return (DoubleDecker ? 8 : 5).SI<SquareMeter>();
			}
		}

		// C42 - ( K )
		public Kelvin MaxTemperatureDeltaForLowFloorBusses { get; set; }

		// C43 - ( Fraction )
		public double MaxPossibleBenefitFromTechnologyList { get; set; }


		// C46 - ( oC )
		public Kelvin EnviromentalTemperature { get; set; }

		// C47 - ( W/M3 )
		public WattPerSquareMeter Solar { get; set; }

		// ( EC_EnviromentalTemperature and  EC_Solar) (Batch Mode)
		public IEnvironmentalConditionsMap EnvironmentalConditionsMap
		{
			get { return _EC_EnvironmentalConditionsMap; }
		}

		public string EnviromentalConditions_BatchFile
		{
			get { return _EC_EnviromentalConditions_BatchFile; }
			set {
				_EC_EnvironmentalConditionsMap = new EnvironmentalConditionsMap(value, _vectoDir);
				_EC_EnviromentalConditions_BatchFile = value;
			}
		}

		public bool EnviromentalConditions_BatchEnabled { get; set; }


		// C53 - "Continous/2-stage/3-stage/4-stage
		public string CompressorType { get; set; }

		// mechanical/electrical
		public string CompressorTypeDerived
		{
			get { return CompressorType == "Continuous" ? "Electrical" : "Mechanical"; }
		}

		// C54 -  ( KW )
		public Watt CompressorCapacity { get; set; }

		// C59
		public double COP
		{
			get {
				var cop = 3.5D;

				if ((CompressorType != null)) {
					cop = CompressorType.ToLower() == "3-stage" ? cop * 1.02 : cop;
					cop = CompressorType.ToLower() == "4-stage" ? cop * 1.02 : cop;
					cop = CompressorType.ToLower() == "continuous"
						? BusFloorType == FloorType.LowFloor
							? cop * 1.04
							: cop * 1.06
						: cop;
				}

				return Math.Round(cop, 2);
			}
		}


		// C62 - Boolean Yes/No
		public bool VentilationOnDuringHeating { get; set; }

		// C63 - Boolean Yes/No
		public bool VentilationWhenBothHeatingAndACInactive { get; set; }

		// C64 - Boolean Yes/No
		public bool VentilationDuringAC { get; set; }

		// C65 - String high/low
		public string VentilationFlowSettingWhenHeatingAndACInactive { get; set; }

		// C66 - String high/low
		public string VentilationDuringHeating { get; set; }

		// C67 - String high/low                                               
		public string VentilationDuringCooling { get; set; }


		// C70 - ( KW )
		public Watt EngineWasteHeatkW { get; set; }

		// C71 - ( KW )
		public Watt FuelFiredHeaterkW { get; set; }

		public double FuelEnergyToHeatToCoolant { get; set; }

		public double CoolantHeatTransferredToAirCabinHeater { get; set; }


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

			GFactor = 0.95D;

			// BC_SolarClouding As Double :Calculated
			// BC_HeatPerPassengerIntoCabinW  :Calculated
			PassengerBoundaryTemperature = 12.0.DegCelsiusToKelvin();

			// BC_PassengerDensityLowFloor :Calculated
			// BC_PassengerDensitySemiLowFloor :Calculated
			// BC_PassengerDensityRaisedFloor :Calculated
			// BC_CalculatedPassengerNumber  :Calculated
			// BC_UValues :Calculated
			HeatingBoundaryTemperature = 18.0.DegCelsiusToKelvin();
			CoolingBoundaryTemperature = 23.0.DegCelsiusToKelvin();

			// BC_CoolingBoundaryTemperature : ReadOnly Static
			HighVentilation = 20.0.SI(Unit.SI.Per.Hour).Cast<PerSecond>();
			LowVentilation = 7.0.SI(Unit.SI.Per.Hour).Cast<PerSecond>();

			// BC_High  :Calculated
			// BC_Low  :Calculated
			// BC_HighVentPower  :Calculated
			// BC_LowVentPower  :Calculated
			SpecificVentilationPower = 0.56.SI(Unit.SI.Watt.Hour.Per.Cubic.Meter).Cast<JoulePerCubicMeter>();

			// BC_COP :Calculated
			AuxHeaterEfficiency = 0.84D;
			GCVDieselOrHeatingOil = 11.8.SI(Unit.SI.Kilo.Watt.Hour.Per.Kilo.Gramm).Cast<JoulePerKilogramm>();

			// BC_WindowAreaPerUnitBusLength   :Calculated 
			// BC_FrontRearWindowArea  :Calculated
			MaxTemperatureDeltaForLowFloorBusses = 3.0.SI<Kelvin>();
			MaxPossibleBenefitFromTechnologyList = 0.5D;

			// Environmental Conditions
			// ************************
			EnviromentalTemperature = 25.0.DegCelsiusToKelvin();
			Solar = 400.0.SI<WattPerSquareMeter>();
			EnviromentalConditions_BatchEnabled = true;
			EnviromentalConditions_BatchFile = "DefaultClimatic.aenv";

			// AC SYSTEM
			// *********
			CompressorType = "2-stage";
			CompressorCapacity = 18.0.SI(Unit.SI.Kilo.Watt).Cast<Watt>();

			// VENTILATION
			// ***********
			VentilationOnDuringHeating = true;
			VentilationWhenBothHeatingAndACInactive = true;
			VentilationDuringAC = true;
			VentilationFlowSettingWhenHeatingAndACInactive = "high";
			VentilationDuringHeating = "high";
			VentilationDuringCooling = "high";

			// AUX HEATER
			// **********
			FuelFiredHeaterkW = 30.0.SI(Unit.SI.Kilo.Watt).Cast<Watt>();
			FuelEnergyToHeatToCoolant = 0.2;
			CoolantHeatTransferredToAirCabinHeater = 0.75;
			EngineWasteHeatkW = 0.SI<Watt>();
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
