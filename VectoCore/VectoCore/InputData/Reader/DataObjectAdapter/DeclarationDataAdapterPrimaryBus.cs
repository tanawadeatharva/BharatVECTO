using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public class DeclarationDataAdapterPrimaryBus : DeclarationDataAdapterHeavyLorry
	{
		#region Overrides of DeclarationDataAdapterHeavyLorry

		public override DriverData CreateDriverData()
		{
			var retVal = base.CreateDriverData();
			retVal.LookAheadCoasting.Enabled = false;
			return retVal;
		}


		public override VehicleData CreateVehicleData(IVehicleDeclarationInputData data, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
		{
			var retVal = base.CreateVehicleData(data, mission, loading);
			retVal.CurbMass = mission.CurbMass;
			return retVal;
		}

		public override PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto)
		{
			return null;
		}

		public override IList<VectoRunData.AuxData> CreateAuxiliaryData(
			IAuxiliariesDeclarationInputData auxInputData, IBusAuxiliariesDeclarationData busAuxData, MissionType mission,
			VehicleClass hdvClass, Meter vehicleLength)
		{
			if (auxInputData != null) {
				throw new VectoException("Only BusAuxiliaries can be provided as input!");
			}

			var retVal = new List<VectoRunData.AuxData>();

			retVal.Add(
				new VectoRunData.AuxData() {
					DemandType = AuxiliaryDemandType.Constant,
					Technology = new List<string>() { busAuxData.FanTechnology },
					ID = Constants.Auxiliaries.IDs.Fan,
					PowerDemand = DeclarationData.Fan.LookupMechanicalPowerDemand(hdvClass, mission, busAuxData.FanTechnology)
				});
			retVal.Add(
				new VectoRunData.AuxData() {
					DemandType = AuxiliaryDemandType.Constant,
					Technology = busAuxData.SteeringPumpTechnology,
					ID = Constants.Auxiliaries.IDs.SteeringPump,
					PowerDemand = DeclarationData.SteeringPumpBus.LookupMechanicalPowerDemand(
						mission, busAuxData.SteeringPumpTechnology, vehicleLength)
				});
			return retVal;
		}

		#endregion

		public virtual IAuxiliaryConfig CreateBusAuxiliariesData(
			Mission mission, IVehicleDeclarationInputData vehicleData, VectoRunData runData)
		{
			var actuations = DeclarationData.BusAuxiliaries.ActuationsMap.Lookup(runData.Mission.MissionType);

			var retVal = new AuxiliaryConfig {
				InputData = vehicleData.Components.BusAuxiliaries,
				ElectricalUserInputsConfig = GetElectricalUserConfig(mission, vehicleData, actuations, runData.VehicleData.VehicleClass),
				PneumaticUserInputsConfig = GetPneumaticUserConfig(vehicleData, mission),
				PneumaticAuxillariesConfig = CreatePneumaticAuxConfig(runData.Retarder.Type),
				Actuations = actuations,
				SSMInputs = CreateSSMModelParameters(
					vehicleData.Components.BusAuxiliaries, mission, FuelData.Diesel, runData.Loading),
				VehicleData = runData.VehicleData,
			};

			return retVal;
		}

		protected virtual ElectricsUserInputsConfig GetElectricalUserConfig(
			Mission mission, IVehicleDeclarationInputData vehicleData, IActuations actuations, VehicleClass vehicleClass)
		{
			var currentDemand = GetElectricConsumers(mission, vehicleData, actuations);
			var busAux = vehicleData.Components.BusAuxiliaries;

			var retVal = GetDefaultElectricalUserConfig();

			retVal.SmartElectrical = busAux.ElectricSupply.SmartElectrics;
			retVal.ElectricalConsumers = currentDemand;
			retVal.AlternatorMap = new SimpleAlternator(CalculateAlternatorEfficiency(busAux.ElectricSupply.Alternators)) {
				Technologies = busAux.ElectricSupply.Alternators.Select(x => x.Technology).ToList()
			};
			retVal.MaxAlternatorPower = busAux.ElectricSupply.MaxAlternatorPower;
			retVal.ElectricStorageCapacity = busAux.ElectricSupply.ElectricStorageCapacity ?? 0.SI<WattSecond>();
			
			return retVal;
		}

		protected virtual ElectricsUserInputsConfig GetDefaultElectricalUserConfig()
		{
			return new ElectricsUserInputsConfig() {
				PowerNetVoltage = Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage,
				StoredEnergyEfficiency = Constants.BusAuxiliaries.ElectricSystem.StoredEnergyEfficiency,
				ResultCardIdle = new DummyResultCard(),
				ResultCardOverrun = new DummyResultCard(),
				ResultCardTraction = new DummyResultCard(),
				AlternatorGearEfficiency = Constants.BusAuxiliaries.ElectricSystem.AlternatorGearEfficiency,
				DoorActuationTimeSecond = Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond,
			};
		}

		protected virtual double CalculateAlternatorEfficiency(IList<IAlternatorDeclarationInputData> alternators)
		{
			var sum = 0.0;
			foreach (var entry in alternators) {
				sum += DeclarationData.BusAuxiliaries.AlternatorTechnologies.Lookup(entry.Technology);
			}

			return sum / alternators.Count;
		}

		protected virtual Dictionary<string, Tuple<bool, Ampere>> GetElectricConsumers(Mission mission, IVehicleDeclarationInputData vehicleData,
			IActuations actuations)
		{
			var retVal = new Dictionary<string, Tuple<bool, Ampere>>();
			var doorDutyCycleFraction =
				(actuations.ParkBrakeAndDoors * Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond) /
				actuations.CycleTime;
			var busAux = vehicleData.Components.BusAuxiliaries;
			var electricDoors = vehicleData.DoorDriveTechnology == ConsumerTechnology.Electrically;
			
			foreach (var consumer in DeclarationData.BusAuxiliaries.DefaultElectricConsumerList.Items) {

				var applied = consumer.DefaultConsumer || consumer.Bonus 
					? 1.0 
					: GetNumberOfElectricalConsumersForMission(mission, consumer);
				var nbr = consumer.DefaultConsumer
					? GetNumberOfElectricalConsumersInVehicle(consumer.NumberInActualVehicle, mission, vehicleData)
					: 1.0;

				var dutyCycle = electricDoors && consumer.ConsumerName.Equals(
									Constants.BusAuxiliaries.ElectricalConsumers.DoorsPerVehicleConsumer,
									StringComparison.CurrentCultureIgnoreCase)
					? doorDutyCycleFraction
					: consumer.PhaseIdleTractionOn;

				var current = applied * consumer.NominalCurrent(mission.MissionType) * dutyCycle * nbr;
				if (consumer.Bonus && !VehicleHasElectricalConsumer(consumer.ConsumerName, busAux)) {
					current = 0.SI<Ampere>();
				}

				retVal[consumer.ConsumerName] = Tuple.Create(consumer.BaseVehicle, current);
				
			}

			return retVal;
		}

		private double GetNumberOfElectricalConsumersForMission(Mission mission, ElectricalConsumer consumer)
		{
			if (mission.BusParameter.ElectricalConsumers.ContainsKey(consumer.ConsumerName)) {
				return mission.BusParameter.ElectricalConsumers[consumer.ConsumerName];
			}

			return 0;
			
		}


		protected virtual bool VehicleHasElectricalConsumer(string consumerName, IBusAuxiliariesDeclarationData busAux)
		{
			switch (consumerName) {
				case "Day running lights LED bonus":
				case "Position lights LED bonus":
				case "Brake lights LED bonus": // return false;
				case "Interior lights LED bonus":
				case "Headlights LED bonus": return true;
				default: return false;
			}
		}

		protected virtual double GetNumberOfElectricalConsumersInVehicle(string nbr, Mission mission, IVehicleDeclarationInputData vehicleData)
		{
			if ("f_IntLight(L_CoC)".Equals(nbr, StringComparison.InvariantCultureIgnoreCase)) {
				return CalculateLengthDependentElectricalConsumers(mission, vehicleData);
			}

			return nbr.ToDouble();
		}

		protected virtual double CalculateLengthDependentElectricalConsumers(Mission mission, IVehicleDeclarationInputData vehicleData)
		{
			var busParams = mission.BusParameter;
			return DeclarationData.BusAuxiliaries.CalculateLengthInteriorLights(
									busParams.VehicleLength, busParams.DoubleDecker, busParams.FloorType, busParams.NumberPassengersLowerDeck)
								.Value();
		}

		protected virtual PneumaticUserInputsConfig GetPneumaticUserConfig(IVehicleDeclarationInputData vehicleData, Mission mission)
		{
			var busAux = vehicleData.Components.BusAuxiliaries;

			//throw new NotImplementedException();
			return new PneumaticUserInputsConfig() {
				KneelingHeight = mission.BusParameter.FloorType == FloorType.LowFloor
					? Constants.BusAuxiliaries.PneumaticUserConfig.DefaultKneelingHeight
					: 0.SI<Meter>(),
				CompressorGearEfficiency = Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency,
				CompressorGearRatio = busAux.PneumaticSupply.Ratio,
				CompressorMap = GetCompressorMap(busAux.PneumaticSupply.CompressorSize, busAux.PneumaticSupply.Clutch),
				SmartAirCompression = busAux.PneumaticSupply.SmartAirCompression,
				SmartRegeneration = busAux.PneumaticSupply.SmartRegeneration,
				AdBlueDosing = busAux.PneumaticConsumers.AdBlueDosing,
				Doors = ConsumerTechnology.Pneumatically,
				AirSuspensionControl = busAux.PneumaticConsumers.AirsuspensionControl,
			};
		}

		protected virtual ICompressorMap GetCompressorMap(string compressorSize, string clutchType)
		{
			var resource = "";
			switch (compressorSize) {
				case "Small":
					resource = "DEFAULT_1-Cylinder_1-Stage_393ccm.acmp";
					break;
				case "Medium Supply 1-stage":
					resource = "DEFAULT_1-Cylinder_1-Stage_393ccm.acmp";
					break;
				case "Medium Supply 2-stage":
					resource = "DEFAULT_2-Cylinder_1-Stage_650ccm.acmp";
					break;
				case "Large Supply 1-stage":
					resource = "DEFAULT_2-Cylinder_2-Stage_398ccm.acmp";
					break;
				case "Large Supply 2-stage":
					resource = "DEFAULT_3-Cylinder_2-Stage_598ccm.acmp";
					break;
				default: throw new ArgumentException(string.Format("unkown compressor size {0}"), compressorSize);
			}

			var dragCurveFactorClutch = 1.0;
			switch (clutchType) {
				case "visco": dragCurveFactorClutch = Constants.BusAuxiliaries.PneumaticUserConfig.ViscoClutchDragCurveFactor;
					break;
				case "mechanically": dragCurveFactorClutch = Constants.BusAuxiliaries.PneumaticUserConfig.MechanicClutchDragCurveFactor;
					break;
			}

			return CompressorMapReader.ReadStream(
				RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix + ".VAUXBus." + resource), dragCurveFactorClutch, $"{compressorSize} - {clutchType}");
		}

		public SSMInputs GetDefaulSSMInputs(IFuelProperties heatingFuel)
		{
			return new SSMInputs(null, heatingFuel) {
				DefaultConditions = new EnvironmentalConditionMapEntry(
					Constants.BusAuxiliaries.SteadyStateModel.DefaultTemperature,
					Constants.BusAuxiliaries.SteadyStateModel.DefaultSolar,
					1.0),
				EnvironmentalConditionsMap = DeclarationData.BusAuxiliaries.DefaultEnvironmentalConditions,
				HeatingBoundaryTemperature = Constants.BusAuxiliaries.SteadyStateModel.HeatingBoundaryTemperature,
				CoolingBoundaryTemperature = Constants.BusAuxiliaries.SteadyStateModel.CoolingBoundaryTemperature,

				SpecificVentilationPower = Constants.BusAuxiliaries.SteadyStateModel.SpecificVentilationPower,

				AuxHeaterEfficiency = Constants.BusAuxiliaries.SteadyStateModel.AuxHeaterEfficiency,
				FuelEnergyToHeatToCoolant = Constants.BusAuxiliaries.Heater.FuelEnergyToHeatToCoolant,
				CoolantHeatTransferredToAirCabinHeater = Constants.BusAuxiliaries.Heater.CoolantHeatTransferredToAirCabinHeater,
				GFactor = Constants.BusAuxiliaries.SteadyStateModel.GFactor,

				VentilationOnDuringHeating = true,
				VentilationWhenBothHeatingAndACInactive = true,
				VentilationDuringAC = true,

				MaxPossibleBenefitFromTechnologyList =
					Constants.BusAuxiliaries.SteadyStateModel.MaxPossibleBenefitFromTechnologyList,
			};
		}

		public virtual ISSMInputs CreateSSMModelParameters(IBusAuxiliariesDeclarationData busAuxInputData, Mission mission, IFuelProperties heatingFuel, LoadingType loadingType)
		{
			var busParams = mission.BusParameter;

			var hvacBusLength = busParams.HVACConfiguration == BusHVACSystemConfiguration.Configuration2
				? 2 * Constants.BusParameters.DriverCompartmentLength
				: busParams.VehicleLength;
			var hvacBusheight = DeclarationData.BusAuxiliaries.CalculateInternalHeight(mission.BusParameter.FloorType, mission.BusParameter.DoubleDecker, busParams.BodyHeight);
			var coolingPower = CalculateMaxCoolingPower(null, mission);

			var retVal = GetDefaulSSMInputs(heatingFuel);
			retVal.BusFloorType = busParams.FloorType;
			retVal.Technologies = GetSSMTechnologyBenefits(busAuxInputData, mission.BusParameter.FloorType);

			retVal.FuelFiredHeaterPower = busParams.HVACAuxHeaterPower;
			retVal.BusWindowSurface = DeclarationData.BusAuxiliaries.WindowHeight(busParams.DoubleDecker) * hvacBusLength +
									DeclarationData.BusAuxiliaries.FrontAndRearWindowArea(busParams.DoubleDecker);
			retVal.BusSurfaceArea = 2 * (hvacBusLength * busParams.VehicleWidth + hvacBusLength * busParams.BodyHeight +
										busParams.VehicleWidth * busParams.BodyHeight);
			retVal.BusVolume = hvacBusLength * busParams.VehicleWidth * hvacBusheight;

			retVal.UValue = DeclarationData.BusAuxiliaries.UValue(busParams.FloorType);
			retVal.NumberOfPassengers =
				DeclarationData.BusAuxiliaries.CalculateBusFloorSurfaceArea(hvacBusLength, busParams.VehicleWidth) *
				busParams.PassengerDensity *
				(loadingType == LoadingType.LowLoading ? mission.MissionType.GetLowLoadFactorBus() : 1.0) + 1; // add driver for 'heat input'
			retVal.VentilationRate = DeclarationData.BusAuxiliaries.VentilationRate(busParams.HVACConfiguration, false);
			retVal.VentilationRateHeating = DeclarationData.BusAuxiliaries.VentilationRate(busParams.HVACConfiguration, true);

			retVal.HVACMaxCoolingPower = coolingPower.Item1 + coolingPower.Item2;
			retVal.HVACCompressorType = busParams.HVACCompressorType; // use passenger compartment
			retVal.COP = DeclarationData.BusAuxiliaries.CalculateCOP(
			
				coolingPower.Item1, ACCompressorType.None, coolingPower.Item2, busParams.HVACCompressorType,
				busParams.FloorType);
			retVal.HVACTechnology = string.Format(
				"{0} ({1})", busParams.HVACConfiguration.GetName(),
				string.Join(", ", new[] { busParams.HVACCompressorType.GetName(), ACCompressorType.None.GetName() }));
			
			//SetHVACParameters(retVal, vehicleData, mission);

			return retVal;
		}


		protected virtual TechnologyBenefits GetSSMTechnologyBenefits(IBusAuxiliariesDeclarationData inputData, FloorType floorType)
		{
			var onVehicle = new List<SSMTechnology>();
			foreach (var item in DeclarationData.BusAuxiliaries.SSMTechnologyList) {
				if ("Adjustable coolant thermostat".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(inputData?.HVACAux.AdjustableCoolantThermostat ?? false)) {
					onVehicle.Add(item);
				}

				if ("Engine waste gas heat exchanger".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(inputData?.HVACAux.EngineWasteGasHeatExchanger ?? false)) {
					onVehicle.Add(item);
				}
			}

			return SelectBenefitForFloorType(floorType, onVehicle);
		}

		protected virtual TechnologyBenefits SelectBenefitForFloorType(FloorType floorType, List<SSMTechnology> onVehicle)
		{
			var retVal = new TechnologyBenefits();

			switch (floorType) {
				case FloorType.LowFloor:
					retVal.CValueVariation = onVehicle.Sum(x => x.LowFloorC);
					retVal.HValueVariation = onVehicle.Sum(x => x.LowFloorH);
					retVal.VCValueVariation = onVehicle.Sum(x => x.ActiveVC ? x.LowFloorV : 0);
					retVal.VHValueVariation = onVehicle.Sum(x => x.ActiveVH ? x.LowFloorV : 0);
					retVal.VVValueVariation = onVehicle.Sum(x => x.ActiveVV ? x.LowFloorV : 0);
					break;
				case FloorType.HighFloor:
					retVal.CValueVariation = onVehicle.Sum(x => x.RaisedFloorC);
					retVal.HValueVariation = onVehicle.Sum(x => x.RaisedFloorH);
					retVal.VCValueVariation = onVehicle.Sum(x => x.ActiveVC ? x.RaisedFloorV : 0);
					retVal.VHValueVariation = onVehicle.Sum(x => x.ActiveVH ? x.RaisedFloorV : 0);
					retVal.VVValueVariation = onVehicle.Sum(x => x.ActiveVV ? x.RaisedFloorV : 0);
					break;
				case FloorType.SemiLowFloor:
					retVal.CValueVariation = onVehicle.Sum(x => x.SemiLowFloorC);
					retVal.HValueVariation = onVehicle.Sum(x => x.SemiLowFloorH);
					retVal.VCValueVariation = onVehicle.Sum(x => x.ActiveVC ? x.SemiLowFloorV : 0);
					retVal.VHValueVariation = onVehicle.Sum(x => x.ActiveVH ? x.SemiLowFloorV : 0);
					retVal.VVValueVariation = onVehicle.Sum(x => x.ActiveVV ? x.SemiLowFloorV : 0);
					break;
			}

			return retVal;
		}


		protected virtual Tuple<Watt, Watt> CalculateMaxCoolingPower(IVehicleDeclarationInputData vehicleData, Mission mission)
		{
			var busParams = mission.BusParameter;

			var length = DeclarationData.BusAuxiliaries.CalculateInternalLength(
				busParams.VehicleLength, busParams.DoubleDecker, busParams.FloorType,
				busParams.NumberPassengersLowerDeck);
			var height = DeclarationData.BusAuxiliaries.CalculateInternalHeight(busParams.FloorType, busParams.DoubleDecker, busParams.BodyHeight);
			var volume = length * height * busParams.VehicleWidth;

			var driver = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.DriverMaxCoolingPower(
				busParams.HVACConfiguration, mission.MissionType);
			var passenger = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.PassengerMaxCoolingPower(
				busParams.HVACConfiguration, mission.MissionType, volume);

			return Tuple.Create(driver, passenger);
		}

		protected internal virtual IPneumaticsConsumersDemand CreatePneumaticAuxConfig(RetarderType retarderType)
		{
			return new PneumaticsConsumersDemand() {
				AdBlueInjection = Constants.BusAuxiliaries.PneumaticConsumersDemands.AdBlueInjection,
				AirControlledSuspension = Constants.BusAuxiliaries.PneumaticConsumersDemands.AirControlledSuspension,
				Braking = retarderType == RetarderType.None
					? Constants.BusAuxiliaries.PneumaticConsumersDemands.BrakingNoRetarder
					: Constants.BusAuxiliaries.PneumaticConsumersDemands.BrakingWithRetarder,
				BreakingWithKneeling = Constants.BusAuxiliaries.PneumaticConsumersDemands.BreakingAndKneeling,
				DeadVolBlowOuts = Constants.BusAuxiliaries.PneumaticConsumersDemands.DeadVolBlowOuts,
				DeadVolume = Constants.BusAuxiliaries.PneumaticConsumersDemands.DeadVolume,
				NonSmartRegenFractionTotalAirDemand =
					Constants.BusAuxiliaries.PneumaticConsumersDemands.NonSmartRegenFractionTotalAirDemand,
				SmartRegenFractionTotalAirDemand =
					Constants.BusAuxiliaries.PneumaticConsumersDemands.SmartRegenFractionTotalAirDemand,
				OverrunUtilisationForCompressionFraction =
					Constants.BusAuxiliaries.PneumaticConsumersDemands.OverrunUtilisationForCompressionFraction,
				DoorOpening = Constants.BusAuxiliaries.PneumaticConsumersDemands.DoorOpening,
				StopBrakeActuation = Constants.BusAuxiliaries.PneumaticConsumersDemands.StopBrakeActuation,
			};
		}
	}
}
