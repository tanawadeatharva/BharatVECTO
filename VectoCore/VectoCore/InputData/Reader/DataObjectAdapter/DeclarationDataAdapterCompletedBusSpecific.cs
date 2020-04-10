using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public class DeclarationDataAdapterCompletedBusSpecific : DeclarationDataAdapterCompletedBusGeneric
	{

		public VehicleData CreateVehicleData(IVehicleDeclarationInputData primaryVehicle,
			IVehicleDeclarationInputData completedVehicle, Mission mission, 
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
		{
			var passengers = GetNumberOfPassengers(
				mission, completedVehicle.Length, completedVehicle.Width,
				completedVehicle.NumberOfPassengersLowerDeck + completedVehicle.NumberOfPassengersUpperDeck, loading.Key);

			var vehicleData = base.CreateVehicleData(primaryVehicle, mission, loading);

			vehicleData.VIN = completedVehicle.VIN;
			vehicleData.LegislativeClass = completedVehicle.LegislativeClass;
			vehicleData.VehicleCategory = VehicleCategory.HeavyBusCompletedVehicle;
			vehicleData.Manufacturer = completedVehicle.Manufacturer;
			vehicleData.ModelName = completedVehicle.Model;
			vehicleData.ManufacturerAddress = completedVehicle.ManufacturerAddress;
			vehicleData.CurbMass = completedVehicle.CurbMassChassis;

			vehicleData.Loading = passengers * mission.MissionType.GetAveragePassengerMass();
			vehicleData.PassengerCount = passengers;
			vehicleData.GrossVehicleMass = completedVehicle.GrossVehicleMassRating;
			vehicleData.DigestValueInput = completedVehicle.DigestValue?.DigestValue ?? "";
			
			return vehicleData;
		}

		public AirdragData CreateAirdragData(IVehicleDeclarationInputData completedVehicle, Mission mission)
		{
			if (!mission.BusParameter.AirDragMeasurementAllowed ||
				completedVehicle.Components.AirdragInputData?.AirDragArea == null) {
				return new AirdragData() {
					CertificationMethod = CertificationMethod.StandardValues,
					DeclaredAirdragArea = mission.DefaultCDxA,
					CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
						mission.DefaultCDxA,
						GetDeclarationAirResistanceCurve(
							mission.CrossWindCorrectionParameters, mission.DefaultCDxA, completedVehicle.Height + mission.BusParameter.DeltaHeight),
						CrossWindCorrectionMode.DeclarationModeCorrection)
				};
			}

			var retVal = SetCommonAirdragData(completedVehicle.Components.AirdragInputData);
			retVal.CrossWindCorrectionMode = CrossWindCorrectionMode.DeclarationModeCorrection;
			var aerodynamicDragArea = completedVehicle.Components.AirdragInputData.AirDragArea;

			retVal.DeclaredAirdragArea = aerodynamicDragArea;
			retVal.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
				aerodynamicDragArea,
				GetDeclarationAirResistanceCurve(
					mission.CrossWindCorrectionParameters,
					aerodynamicDragArea,
					completedVehicle.Height + mission.BusParameter.DeltaHeight),
				CrossWindCorrectionMode.DeclarationModeCorrection);

			return retVal;
		}

		public IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle, IVehicleDeclarationInputData completedVehicle, VectoRunData runData)
		{
			var actuations = DeclarationData.BusAuxiliaries.ActuationsMap.Lookup(runData.Mission.MissionType);
			var primaryBusAuxiliaries = primaryVehicle.Components.BusAuxiliaries;

			return new AuxiliaryConfig {
				InputData = completedVehicle.Components.BusAuxiliaries,
				ElectricalUserInputsConfig = CreateElectricsUserInputsConfig(
					primaryBusAuxiliaries, completedVehicle, mission, actuations),
				PneumaticUserInputsConfig = CreatePneumaticUserInputsConfig(
					primaryBusAuxiliaries, completedVehicle),
				PneumaticAuxillariesConfig = CreatePneumaticAuxConfig(runData.Retarder.Type),
				Actuations = actuations,
				SSMInputs = GetCompletedSSMInput(mission, completedVehicle, primaryBusAuxiliaries, runData.Loading),
				VehicleData = runData.VehicleData
			};
		}

		protected ElectricsUserInputsConfig CreateElectricsUserInputsConfig(IBusAuxiliariesDeclarationData primaryBusAuxiliaries,
			IVehicleDeclarationInputData completedVehicle, Mission mission, IActuations actuations)
		{
			var currentDemand = GetElectricConsumers(mission, completedVehicle, actuations);

			var retVal = GetDefaultElectricalUserConfig();


			retVal.SmartElectrical = primaryBusAuxiliaries.ElectricSupply.SmartElectrics;
			retVal.ElectricalConsumers = currentDemand;
			retVal.AlternatorMap = new SimpleAlternator(
				CalculateAlternatorEfficiency(
					primaryBusAuxiliaries.ElectricSupply.Alternators
										.Concat(completedVehicle.Components.BusAuxiliaries.ElectricSupply.Alternators).ToList())) {
				Technologies = primaryBusAuxiliaries.ElectricSupply.Alternators
													.Concat(completedVehicle.Components.BusAuxiliaries.ElectricSupply.Alternators).Select(x => x.Technology)
													.ToList()
			};
			retVal.MaxAlternatorPower = primaryBusAuxiliaries.ElectricSupply.MaxAlternatorPower;
			retVal.ElectricStorageCapacity = primaryBusAuxiliaries.ElectricSupply.ElectricStorageCapacity ?? 0.SI<WattSecond>();
			
			return retVal;
		}

		protected PneumaticUserInputsConfig CreatePneumaticUserInputsConfig(IBusAuxiliariesDeclarationData primaryBusAuxiliaries,
			IVehicleDeclarationInputData completedVehicle)
		{
			return new PneumaticUserInputsConfig {
				CompressorMap = GetCompressorMap(primaryBusAuxiliaries.PneumaticSupply.CompressorSize, primaryBusAuxiliaries.PneumaticSupply.Clutch),
				CompressorGearEfficiency = Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency,
				CompressorGearRatio = primaryBusAuxiliaries.PneumaticSupply.Ratio,
				SmartAirCompression = primaryBusAuxiliaries.PneumaticSupply.SmartAirCompression,
				SmartRegeneration = primaryBusAuxiliaries.PneumaticSupply.SmartRegeneration,
				KneelingHeight = VectoMath.Max(0.SI<Meter>(),
					completedVehicle.EntranceHeight - Constants.BusParameters.EntranceHeight),
				AirSuspensionControl = primaryBusAuxiliaries.PneumaticConsumers.AirsuspensionControl,
				AdBlueDosing = primaryBusAuxiliaries.PneumaticConsumers.AdBlueDosing,
				Doors = completedVehicle.DoorDriveTechnology
			};
		}

		private SSMInputs GetCompletedSSMInput(Mission mission, IVehicleDeclarationInputData completedVehicle,
			IBusAuxiliariesDeclarationData primaryBusAux, LoadingType loadingType)
		{
			var isDoubleDecker = completedVehicle.VehicleCode.IsDoubleDeckerBus();
			var hvacConfiguration = completedVehicle.Components.BusAuxiliaries.HVACAux.SystemConfiguration;
			var hvacBusLength = hvacConfiguration == BusHVACSystemConfiguration.Configuration2
				? 2 * Constants.BusParameters.DriverCompartmentLength
				: completedVehicle.Length;

			var hvacBusHeight = DeclarationData.BusAuxiliaries.CalculateInternalHeight(completedVehicle.VehicleCode.GetFloorType(),
																						isDoubleDecker, completedVehicle.Height);
			var hvacBusWidth = DeclarationData.BusAuxiliaries.CorrectedBusWidth(completedVehicle.Width);
			var coolingPower = CalculateMaxCoolingPower(completedVehicle, mission);

			var busAux = completedVehicle.Components.BusAuxiliaries.HVACAux;
			var floorType = completedVehicle.VehicleCode.GetFloorType();

			var ssmInputs =GetDefaulSSMInputs(FuelData.Diesel);
			
			ssmInputs.BusFloorType = completedVehicle.VehicleCode.GetFloorType();
			ssmInputs.Technologies = CreateTechnologyBenefits(completedVehicle, primaryBusAux);
			ssmInputs.FuelFiredHeaterPower = busAux.AuxHeaterPower;
			ssmInputs.BusWindowSurface = DeclarationData.BusAuxiliaries.WindowHeight(isDoubleDecker) * hvacBusLength +
										DeclarationData.BusAuxiliaries.FrontAndRearWindowArea(isDoubleDecker);
			ssmInputs.BusSurfaceArea = 2 * (hvacBusLength * hvacBusWidth + hvacBusLength *
											completedVehicle.Height + hvacBusWidth * completedVehicle.Height);
			ssmInputs.BusVolume = hvacBusLength * hvacBusWidth * hvacBusHeight;

			ssmInputs.UValue = DeclarationData.BusAuxiliaries.UValue(completedVehicle.VehicleCode.GetFloorType());
			ssmInputs.NumberOfPassengers = GetNumberOfPassengers(
				mission, hvacBusLength, hvacBusWidth,
				completedVehicle.NumberOfPassengersLowerDeck + completedVehicle.NumberOfPassengersUpperDeck, loadingType) + 1; // add driver for 'heat input'
			ssmInputs.VentilationRate = DeclarationData.BusAuxiliaries.VentilationRate(hvacConfiguration, false);
			ssmInputs.VentilationRateHeating = DeclarationData.BusAuxiliaries.VentilationRate(hvacConfiguration, true);

			ssmInputs.HVACMaxCoolingPower = coolingPower.Item1 + coolingPower.Item2;
			ssmInputs.HVACCompressorType = busAux.CompressorTypePassenger; // use passenger compartment
			ssmInputs.HVACTechnology = string.Format(
				"{0} ({1})", busAux.SystemConfiguration.GetName(),
				string.Join(", ", new[] { busAux.CompressorTypePassenger.GetName(), busAux.CompressorTypeDriver.GetName() })); ;
			ssmInputs.COP = DeclarationData.BusAuxiliaries.CalculateCOP(
				coolingPower.Item1, busAux.CompressorTypeDriver, coolingPower.Item2, busAux.CompressorTypePassenger,
				floorType);

			return ssmInputs;
		}

		
		public TechnologyBenefits CreateTechnologyBenefits(IVehicleDeclarationInputData completedVehicle,
			IBusAuxiliariesDeclarationData primaryBusAux)
		{
			var onVehicle = new List<SSMTechnology>();
			var completedBuxAux = completedVehicle.Components.BusAuxiliaries;
			
			foreach (var item in DeclarationData.BusAuxiliaries.SSMTechnologyList)
			{
				if ("Double-glazing".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(completedBuxAux?.HVACAux.DoubleGlasing ?? false))
				{
					onVehicle.Add(item);
				}
				if ("Heat pump systems".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(completedBuxAux?.HVACAux.HeatPump ?? false))
				{
					onVehicle.Add(item);
				}
				if ("Adjustable auxiliary heater".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(completedBuxAux?.HVACAux.AdjustableAuxiliaryHeater ?? false))
				{
					onVehicle.Add(item);
				}
				if ("Separate air distribution ducts".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(completedBuxAux?.HVACAux.SeparateAirDistributionDucts ?? false))
				{
					onVehicle.Add(item);
				}
				if ("Adjustable coolant thermostat".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(primaryBusAux?.HVACAux.AdjustableCoolantThermostat ?? false))
				{
					onVehicle.Add(item);
				}
				if ("Engine waste gas heat exchanger".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(primaryBusAux?.HVACAux.EngineWasteGasHeatExchanger ?? false))
				{
					onVehicle.Add(item);
				}
			}

			return SelectBenefitForFloorType(completedVehicle.VehicleCode.GetFloorType(), onVehicle);
		}

		

		protected override Tuple<Watt, Watt> CalculateMaxCoolingPower(IVehicleDeclarationInputData completedVehicle,
			Mission mission)
		{
			var isDoubleDecker = completedVehicle.VehicleCode.IsDoubleDeckerBus();
			var floorType = completedVehicle.VehicleCode.GetFloorType();
			var hvacConfiguration = completedVehicle.Components.BusAuxiliaries.HVACAux.SystemConfiguration;
			
			var length = DeclarationData.BusAuxiliaries.CalculateInternalLength(
			 	completedVehicle.Length ,isDoubleDecker, floorType, 
				completedVehicle.NumberOfPassengersLowerDeck);
			var height = DeclarationData.BusAuxiliaries.CalculateInternalHeight(floorType, isDoubleDecker, completedVehicle.Height);
			var volume = length * height * completedVehicle.Width;

			var driver = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.DriverMaxCoolingPower(
			 	hvacConfiguration, mission.MissionType);
			var passenger = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.PassengerMaxCoolingPower(
				hvacConfiguration, mission.MissionType, volume);

			return Tuple.Create(driver, passenger);
		}




		#region Avarage Current Demand Calculation


		protected override bool VehicleHasElectricalConsumer(string consumerName, IBusAuxiliariesDeclarationData busAux)
		{
			if (consumerName == "Day running lights LED bonus" && busAux.ElectricConsumers.DayrunninglightsLED)
				return true;
			if (consumerName == "Position lights LED bonus" && busAux.ElectricConsumers.PositionlightsLED)
				return true;
			if (consumerName == "Brake lights LED bonus" && busAux.ElectricConsumers.BrakelightsLED)
				return true;
			if (consumerName == "Interior lights LED bonus" && busAux.ElectricConsumers.InteriorLightsLED)
				return true;
			if (consumerName == "Headlights LED bonus" && busAux.ElectricConsumers.HeadlightsLED)
				return true;

			return false;
		}

		protected override double CalculateLengthDependentElectricalConsumers(Mission mission, IVehicleDeclarationInputData vehicleData)
		{
			var busParams = mission.BusParameter;
			return DeclarationData.BusAuxiliaries.CalculateLengthInteriorLights(
									vehicleData.Length, vehicleData.VehicleCode.IsDoubleDeckerBus(), vehicleData.VehicleCode.GetFloorType(), busParams.NumberPassengersLowerDeck)
								.Value();
		}


		#endregion


		protected double GetNumberOfPassengers(Mission mission, Meter length, Meter width, double registeredPassengers, LoadingType loading)
		{
			var busFloorArea = DeclarationData.BusAuxiliaries.CalculateBusFloorSurfaceArea(length, width);
			var passengerCountRef = busFloorArea * mission.BusParameter.PassengerDensity;
			//var passengerCountDecl = completedVehicle.NuberOfPassengersUpperDeck + completedVehicle.NumberOfPassengersLowerDeck;
			if (loading != LoadingType.ReferenceLoad && loading != LoadingType.LowLoading) {
				throw new VectoException("Unhandled loading type: {0}", loading);
			}

			return loading == LoadingType.ReferenceLoad
				? VectoMath.Min(passengerCountRef, registeredPassengers)
				: passengerCountRef * mission.MissionType.GetLowLoadFactorBus();
		}
		
		
		


	}
}
