using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public class DeclarationDataAdapterSingleBus : DeclarationDataAdapterPrimaryBus
	{
		#region Implementation of IDeclarationDataAdapter

		public override VehicleData CreateVehicleData(
			IVehicleDeclarationInputData vehicle, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
		{
			var busFloorArea = DeclarationData.BusAuxiliaries.CalculateBusFloorSurfaceArea(
				CompletedVehicle.Length,
				CompletedVehicle.Width);
			var passengerCountRef = busFloorArea * mission.BusParameter.PassengerDensity;
			var passengerCountDecl = CompletedVehicle.NuberOfPassengersUpperDeck + CompletedVehicle.NumberOfPassengersLowerDeck;

			//var refLoad = passengerCount * mission.MissionType.GetAveragePassengerMass();
			if (loading.Key != LoadingType.ReferenceLoad && loading.Key != LoadingType.LowLoading) {
				throw new VectoException("Unhandled loading type: {0}", loading.Key);
			}

			var passengerCountCalc = loading.Key == LoadingType.ReferenceLoad
				? VectoMath.Min(passengerCountRef, passengerCountDecl)
				: passengerCountRef * mission.MissionType.GetLowLoadFactorBus();
			var payload = passengerCountCalc * mission.MissionType.GetAveragePassengerMass();

			var retVal = CreateNonExemptedVehicleData(vehicle, mission, payload, passengerCountCalc);
			retVal.CurbMass = CompletedVehicle.CurbMassChassis;
			return retVal;
		}

		public override AirdragData CreateAirdragData(
			IAirdragDeclarationInputData airdragData, Mission mission, Segment segment)
		{
			if (CompletedVehicle.Components.AirdragInputData == null ||
				CompletedVehicle.Components.AirdragInputData.AirDragArea == null) {
				return DefaultAirdragData(mission);
			}

			var aerodynamicDragArea = CompletedVehicle.Components.AirdragInputData.AirDragArea;
			var retVal = SetCommonAirdragData(CompletedVehicle.Components.AirdragInputData);
			retVal.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
				aerodynamicDragArea,
				GetDeclarationAirResistanceCurve(mission.CrossWindCorrectionParameters, aerodynamicDragArea, mission.VehicleHeight),
				CrossWindCorrectionMode.DeclarationModeCorrection);

			return retVal;
		}

		#endregion


		public ISingleBusInputDataProvider SingleBusInputData { get; set; }

		protected IVehicleDeclarationInputData CompletedVehicle
		{
			get { return SingleBusInputData?.CompletedVehicle; }
		}


		#region Overrides of DeclarationDataAdapterPrimaryBus

		protected override double CalculateAlternatorEfficiency(IList<IAlternatorDeclarationInputData> alternators)
		{
			var sum = 0.0;
			foreach (var entry in alternators) {
				sum += DeclarationData.BusAuxiliaries.AlternatorTechnologies.Lookup(entry.Technology);
			}

			foreach (var entry in CompletedVehicle.Components.BusAuxiliaries.ElectricSupply.Alternators) {
				sum += DeclarationData.BusAuxiliaries.AlternatorTechnologies.Lookup(entry.Technology);
			}

			return sum / (alternators.Count + CompletedVehicle.Components.BusAuxiliaries.ElectricSupply.Alternators.Count);
		}

		protected override ElectricsUserInputsConfig GetElectricalUserConfig(
			Mission mission, IVehicleDeclarationInputData vehicleData, IActuations actuations)
		{
			var currentDemand = CalculateAverageCurrent(mission, vehicleData, actuations);
			var busAux = vehicleData.Components.BusAuxiliaries;

			return new ElectricsUserInputsConfig() {
				SmartElectrical = busAux.ElectricSupply.SmartElectrics,
				AverageCurrentDemandInclBaseLoad = currentDemand.Item1,
				AverageCurrentDemandWithoutBaseLoad = currentDemand.Item2,
				AlternatorMap =
					new SimpleAlternator(
						CalculateAlternatorEfficiency(
							busAux.ElectricSupply.Alternators.Concat(CompletedVehicle.Components.BusAuxiliaries.ElectricSupply.Alternators)
								.ToList())) {
						Technologies = busAux.ElectricSupply.Alternators
											.Concat(CompletedVehicle.Components.BusAuxiliaries.ElectricSupply.Alternators).Select(x => x.Technology)
											.ToList()
					},
				PowerNetVoltage = Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage,
				StoredEnergyEfficiency = Constants.BusAuxiliaries.ElectricSystem.StoredEnergyEfficiency,
				ResultCardIdle = new DummyResultCard(),
				ResultCardOverrun = new DummyResultCard(),
				ResultCardTraction = new DummyResultCard(),
				AlternatorGearEfficiency = Constants.BusAuxiliaries.ElectricSystem.AlternatorGearEfficiency,
				MaxAlternatorPower = busAux.ElectricSupply.MaxAlternatorPower,
				ElectricStorageCapacity = busAux.ElectricSupply.ElectricStorageCapacity ?? 0.SI<WattSecond>()
			};
		}

		protected override bool VehicleHasElectricalConsumer(string consumerName, IBusAuxiliariesDeclarationData busAux)
		{
			var elConsumer = CompletedVehicle.Components.BusAuxiliaries.ElectricConsumers;
			switch (consumerName) {
				case "Day running lights LED bonus": return elConsumer.DayrunninglightsLED;
				case "Position lights LED bonus": return elConsumer.PositionlightsLED;
				case "Brake lights LED bonus": return elConsumer.BrakelightsLED;
				case "Interior lights LED bonus": return elConsumer.InteriorLightsLED;
				case "Headlights LED bonus": return elConsumer.HeadlightsLED;
				default: return false;
			}
		}

		protected override double GetNumberOfElectricalConsumersInVehicle(string nbr, Mission mission)
		{
			if ("f_IntLight(L_CoC)".Equals(nbr, StringComparison.InvariantCultureIgnoreCase)) {
				return DeclarationData.BusAuxiliaries.CalculateLengthInteriorLights(
										CompletedVehicle.Length, IsDoubleDecker, CompletedVehicle.FloorType,
										CompletedVehicle.NumberOfPassengersLowerDeck)
									.Value();
			}

			return nbr.ToDouble();
		}

		protected override PneumaticUserInputsConfig GetPneumaticUserConfig(
			IVehicleDeclarationInputData vehicleData, Mission mission)
		{
			var retVal = base.GetPneumaticUserConfig(vehicleData, mission);
			retVal.Doors = CompletedVehicle.DoorDriveTechnology;
			retVal.KneelingHeight = VectoMath.Max(
				0.SI<Meter>(), CompletedVehicle.EntranceHeight - Constants.BusParameters.EntranceHeight);
			return retVal;
		}

		public override ISSMInputs CreateSSMModelParameters(
			IBusAuxiliariesDeclarationData busAuxInputData, Mission mission, IFuelProperties heatingFuel, LoadingType loading)
		{
			var retVal = base.CreateSSMModelParameters(busAuxInputData, mission, heatingFuel, loading) as SSMInputs;
			if (retVal == null) {
				throw new VectoException("Unknonw SSMInput Instance");
			}

			var busAux = CompletedVehicle.Components.BusAuxiliaries;

			var hvacBusLength = busAux.HVACAux.SystemConfiguration == BusHVACSystemConfiguration.Configuration2
				? 2 * Constants.BusParameters.DriverCompartmentLength
				: CompletedVehicle.Length;
			var correctedBusWidth = DeclarationData.BusAuxiliaries.CorrectedBusWidth(CompletedVehicle.Width);

			var hvacBusheight = DeclarationData.BusAuxiliaries.CalculateInternalHeight(
				CompletedVehicle.FloorType,
				IsDoubleDecker, CompletedVehicle.Height);
			var coolingPower = CalculateMaxCoolingPower(mission);

			retVal.BusFloorType = CompletedVehicle.FloorType;
			retVal.Technologies = GetSSMTechnologyBenefits(busAuxInputData, CompletedVehicle.FloorType);
			retVal.FuelFiredHeaterPower = busAux.HVACAux.AuxHeaterPower;

			retVal.BusWindowSurface = DeclarationData.BusAuxiliaries.WindowHeight(IsDoubleDecker) * hvacBusLength +
									DeclarationData.BusAuxiliaries.FrontAndRearWindowArea(IsDoubleDecker);
			retVal.BusSurfaceArea = 2 * (hvacBusLength * correctedBusWidth + hvacBusLength * CompletedVehicle.Height +
										correctedBusWidth * CompletedVehicle.Height);
			retVal.BusVolume = hvacBusLength * correctedBusWidth * hvacBusheight;
			retVal.UValue = DeclarationData.BusAuxiliaries.UValue(CompletedVehicle.FloorType);
			retVal.NumberOfPassengers =
				(DeclarationData.BusAuxiliaries.CalculateBusFloorSurfaceArea(hvacBusLength, correctedBusWidth) *
				mission.BusParameter.PassengerDensity *
				(loading == LoadingType.LowLoading ? mission.MissionType.GetLowLoadFactorBus() : 1.0)).LimitTo(
					0, CompletedVehicle.NuberOfPassengersUpperDeck + CompletedVehicle.NumberOfPassengersLowerDeck) +
				1; // add driver for 'heat input'
			retVal.VentilationRate = DeclarationData.BusAuxiliaries.VentilationRate(busAux.HVACAux.SystemConfiguration, false);
			retVal.VentilationRateHeating =
				DeclarationData.BusAuxiliaries.VentilationRate(busAux.HVACAux.SystemConfiguration, true);

			retVal.HVACMaxCoolingPower = coolingPower.Item1 + coolingPower.Item2;
			retVal.HVACCompressorType = busAux.HVACAux.CompressorTypePassenger; // use passenger compartment
			retVal.HVACTechnology = string.Format(
				"{0} ({1})", busAux.HVACAux.SystemConfiguration,
				string.Join(
					", ", new[] { busAux.HVACAux.CompressorTypePassenger.GetName(), busAux.HVACAux.CompressorTypeDriver.GetName() }));
			retVal.COP = DeclarationData.BusAuxiliaries.CalculateCOP(
				coolingPower.Item1, busAux.HVACAux.CompressorTypeDriver, coolingPower.Item2,
				busAux.HVACAux.CompressorTypePassenger,
				CompletedVehicle.FloorType);

			return retVal;
		}

		protected override Tuple<Watt, Watt> CalculateMaxCoolingPower(Mission mission)
		{
			var length = DeclarationData.BusAuxiliaries.CalculateInternalLength(
				CompletedVehicle.Length, IsDoubleDecker, CompletedVehicle.FloorType,
				CompletedVehicle.NumberOfPassengersLowerDeck);
			var height = DeclarationData.BusAuxiliaries.CalculateInternalHeight(
				CompletedVehicle.FloorType,
				IsDoubleDecker, CompletedVehicle.Height);
			var volume = length * height * DeclarationData.BusAuxiliaries.CorrectedBusWidth(CompletedVehicle.Width);

			var hvacConfiguration = CompletedVehicle.Components.BusAuxiliaries.HVACAux.SystemConfiguration;

			var driver = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.DriverMaxCoolingPower(
				hvacConfiguration, mission.MissionType);
			var passenger = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.PassengerMaxCoolingPower(
				hvacConfiguration, mission.MissionType, volume);

			return Tuple.Create(driver, passenger);
		}

		protected override TechnologyBenefits GetSSMTechnologyBenefits(
			IBusAuxiliariesDeclarationData inputData, FloorType floorType)
		{
			var onVehicle = new List<SSMTechnology>();
			var hvacTech = CompletedVehicle.Components.BusAuxiliaries.HVACAux;
			foreach (var item in DeclarationData.BusAuxiliaries.SSMTechnologyList) {
				if ("Adjustable coolant thermostat".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(inputData?.HVACAux.AdjustableCoolantThermostat ?? false)) {
					onVehicle.Add(item);
				}

				if ("Engine waste gas heat exchanger".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(inputData?.HVACAux.EngineWasteGasHeatExchanger ?? false)) {
					onVehicle.Add(item);
				}

				if ("Separate air distribution ducts".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					hvacTech.SeparateAirDistributionDucts) {
					onVehicle.Add(item);
				}
				if ("Adjustable auxiliary heater".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					hvacTech.AdjustableAuxiliaryHeater) {
					onVehicle.Add(item);
				}
				if ("Heat pump systems".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					hvacTech.HeatPump) {
					onVehicle.Add(item);
				}
				if ("Double-glazing".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					hvacTech.DoubleGlasing) {
					onVehicle.Add(item);
				}
			}

			return SelectBenefitForFloorType(floorType, onVehicle);
		}

		#endregion

		protected bool IsDoubleDecker
		{
			get { return CompletedVehicle.NuberOfPassengersUpperDeck > 0; }
		}
	}
}
