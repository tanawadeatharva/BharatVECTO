using System;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	// Used by SSMHVAC Class
	public class SSMInputs : ISSMInputs, ISSMBoundaryConditions, IEnvironmentalConditions, IACSystem, IVentilation,
		IAuxHeater, ISSMBusParameters
	{
		private IFuelProperties HeatingFuel;

		public SSMInputs(IVehicleData vehicle, string source, IFuelProperties heatingFuel = null)
		{
			Vehicle = vehicle;
			Source = source;
			HeatingFuel = heatingFuel ?? FuelData.Diesel;
		}

		public string Source { get; }

		public IVehicleData Vehicle { get; }

		public bool SSMDisabled { get; set; }

		// C4/D4
		public string BusModel
		{
			get { return Vehicle.ModelName; }
		}

		// C5/D5
		public double NumberOfPassengers
		{
			get { return Vehicle.PassengerCount; }
		}

		// C6/D6
		public FloorType BusFloorType
		{
			get { return Vehicle.FloorType; }
		}

		// C10/D10
		public bool DoubleDecker
		{
			get { return Vehicle.DoubleDecker; }
		}

		// D12/C12 - ( M )
		public Meter BusLength
		{
			get { return Vehicle.Length; }
		}

		// D13/C13 - ( M )
		public Meter BusWidth
		{
			get { return Vehicle.Width; }
		}

		// D14/C14 - ( M )
		public Meter BusHeight
		{
			get { return Vehicle.Height; }
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

		//PassengerBoundaryTemperature = 17

		// C18            
		public double SolarClouding(Kelvin enviromentalTemperature)
		{
			
				// =IF(C46<17,0.65,0.8)
				return  enviromentalTemperature < Constants.BusAuxiliaries.SteadyStateModel.PassengerBoundaryTemperature
					? Constants.BusAuxiliaries.SteadyStateModel.SolarCloudingLow
					: Constants.BusAuxiliaries.SteadyStateModel.SolarCloudingHigh;
			
		}

		// C19 - ( W )
		public Watt HeatPerPassengerIntoCabin(Kelvin enviromentalTemperature)
		{
				// =IF(C46<17,50,80)
				return enviromentalTemperature < Constants.BusAuxiliaries.SteadyStateModel.PassengerBoundaryTemperature
					? Constants.BusAuxiliaries.SteadyStateModel.HeatPerPassengerIntoCabinLow
					: Constants.BusAuxiliaries.SteadyStateModel.HeatPerPassengerIntoCabinHigh;	
		}

		// C20 - ( oC )
		//public Kelvin PassengerBoundaryTemperature { get; set; }

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
		public PerSecond VentilationRate { get; set; }

		// C30 - ( L/H )   --- !! 1/h
		//public PerSecond LowVentilation { get; set; }

		// C31 - ( M3/H )
		public CubicMeterPerSecond VolumeExchange
		{
			get {
				// =D11*C29
				return BusVolume * VentilationRate;
			}
		}

		// C32 - ( M3/H )
		//public CubicMeterPerSecond LowVolumeExchange
		//{
		//	get {
		//		// =C30*D11
		//		return BusVolume * LowVentilation;
		//	}
		//}

		// C33 - ( W )
		public Watt VentPower
		{
			get {
				// =C31*C35
				return VolumeExchange * SpecificVentilationPower;
			}
		}

		// C34 - ( W )
		//public Watt LowVentPower
		//{
		//	get {
		//		// =C32*C35
		//		return LowVolumeExchange * SpecificVentilationPower;
		//	}
		//}

		// C35 - ( Wh/M3 )
		public JoulePerCubicMeter SpecificVentilationPower { get; set; }

		// C37               
		public double AuxHeaterEfficiency { get; set; }

		// C38 - ( KW/HKG )
		public JoulePerKilogramm GCVDieselOrHeatingOil { get { return HeatingFuel.LowerHeatingValueVecto; } }

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
		public Kelvin MaxTemperatureDeltaForLowFloorBusses
		{
			get { return Constants.BusAuxiliaries.SteadyStateModel.MaxTemperatureDeltaForLowFloorBusses; }
		}

		// C43 - ( Fraction )
		public double MaxPossibleBenefitFromTechnologyList { get; set; }


		// C46 - ( oC )
		//public Kelvin EnviromentalTemperature { get; set; }

		// C47 - ( W/M3 )
		//public WattPerSquareMeter Solar { get; set; }

		public IEnvironmentalConditionsMapEntry DefaultConditions { get; set; }

		// ( EC_EnviromentalTemperature and  EC_Solar) (Batch Mode)
		public IEnvironmentalConditionsMap EnvironmentalConditionsMap { get; set; }

		public bool BatchMode
		{
			get { return EnvironmentalConditionsMap != null && EnvironmentalConditionsMap.GetEnvironmentalConditions().Any(); }
		}


		// C53 - "Continous/2-stage/3-stage/4-stage
		public ACCompressorType HVACCompressorType { get; set; }

		// mechanical/electrical
		public string CompressorTypeDerived
		{
			get { return HVACCompressorType == ACCompressorType.Continuous ? "Electrical" : "Mechanical"; }
		}

		// C54 -  ( KW )
		public Watt HVACMaxCoolingPower { get; set; }

		// C59
		public double COP
		{
			get {
				var cop = 3.5;

				switch (HVACCompressorType) {
					case ACCompressorType.TwoStage: break;
					case ACCompressorType.ThreeStage: 
					case ACCompressorType.FourStage:
						cop = cop * 1.02;
						break;
					case ACCompressorType.Continuous:
						cop = BusFloorType == FloorType.LowFloor
							? cop * 1.04
							: cop * 1.06;
						break;
					default: throw new ArgumentOutOfRangeException();
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
		//public VentilationLevel VentilationFlowSettingWhenHeatingAndACInactive { get; set; }

		//// C66 - String high/low
		//public VentilationLevel VentilationDuringHeating { get; set; }

		//// C67 - String high/low                                               
		//public VentilationLevel VentilationDuringCooling { get; set; }


		// C70 - ( KW )
		//public Watt EngineWasteHeatkW { get; set; }

		// C71 - ( KW )
		public Watt FuelFiredHeaterPower { get; set; }

		public double FuelEnergyToHeatToCoolant { get; set; }

		public double CoolantHeatTransferredToAirCabinHeater { get; set; }


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

		public ISSMTechnologies Technologies { get; set; }


		#endregion
	}
}
