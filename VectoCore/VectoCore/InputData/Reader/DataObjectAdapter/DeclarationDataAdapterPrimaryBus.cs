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
		public AirdragData CreateAirdragData(Mission mission)
		{
			return DefaultAirdragData(mission);
		}

		#region Overrides of DeclarationDataAdapterTruck

		public override VehicleData CreateVehicleData(IVehicleDeclarationInputData data, Mission mission, Kilogram loading)
		{
			var retVal = base.CreateVehicleData(data, mission, loading);
			retVal.CurbMass = mission.CurbMass;
			retVal.Length = mission.BusParameter.VehicleLength;
			retVal.Width = mission.BusParameter.VehicleWidth;
			retVal.Height = mission.VehicleHeight;
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
					PowerDemand = DeclarationData.Fan.Lookup(hdvClass, mission, busAuxData.FanTechnology).PowerDemand
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

		public IAuxiliaryConfig CreateBusAuxiliariesData(
			Mission mission, IVehicleDeclarationInputData vehicleData, VectoRunData runData)
		{
			var actuations = DeclarationData.BusAuxiliaries.ActuationsMap.Lookup(runData.Mission.MissionType);

			var retVal = new AuxiliaryConfig {
				InputData = vehicleData.Components.BusAuxiliaries,
				ElectricalUserInputsConfig = GetElectricalUserConfig(mission, vehicleData, actuations),
				PneumaticUserInputsConfig = GetPneumaticUserConfig(vehicleData, mission),
				PneumaticAuxillariesConfig = CreatePneumaticAuxConfig(runData.Retarder.Type),
				Actuations = actuations,
				SSMInputs = CreateSSMModelParameters(runData.VehicleData, mission, FuelData.Diesel),
				VehicleData = runData.VehicleData,
				FuelMap = runData.EngineData.Fuels.First().ConsumptionMap
			};

			return retVal;
		}

		private IElectricsUserInputsConfig GetElectricalUserConfig(
			Mission mission, IVehicleDeclarationInputData vehicleData, IActuations actuations)
		{
			var currentDemand = CalculateAverageCurrent(mission, vehicleData, actuations);
			var busAux = vehicleData.Components.BusAuxiliaries;

			return new ElectricsUserInputsConfig() {
				SmartElectrical = busAux.ElectricSupply.SmartElectrics,
				AverageCurrentDemandInclBaseLoad = currentDemand.Item1,
				AverageCurrentDemandWithoutBaseLoad = currentDemand.Item2,
				AlternatorMap = new SimpleAlternator(CalculateAlternatorEfficiency(busAux.ElectricSupply.Alternators)),
				PowerNetVoltage = Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage,
				ResultCardIdle = busAux.ElectricSupply.ResultCards != null
					? new ResultCard(
						busAux.ElectricSupply.ResultCards.Idle.Select(x => new SmartResult(x.Current, x.SmartCurrent)).ToList())
					: (IResultCard)new DummyResultCard(),
				ResultCardOverrun = busAux.ElectricSupply.ResultCards != null
					? new ResultCard(
						busAux.ElectricSupply.ResultCards.Overrun.Select(x => new SmartResult(x.Current, x.SmartCurrent)).ToList())
					: (IResultCard)new DummyResultCard(),
				ResultCardTraction = busAux.ElectricSupply.ResultCards != null
					? new ResultCard(
						busAux.ElectricSupply.ResultCards.Traction.Select(x => new SmartResult(x.Current, x.SmartCurrent)).ToList())
					: (IResultCard)new DummyResultCard(),
				AlternatorGearEfficiency = Constants.BusAuxiliaries.ElectricSystem.AlternatorGearEfficiency,
			};
		}

		private double CalculateAlternatorEfficiency(IList<IAlternatorDeclarationInputData> alternators)
		{
			var sum = 0.0;
			foreach (var entry in alternators) {
				sum += DeclarationData.BusAuxiliaries.AlternatorTechnologies.Lookup(entry.Technology);
			}

			return sum / alternators.Count;
		}

		private Tuple<Ampere, Ampere> CalculateAverageCurrent(
			Mission mission, IVehicleDeclarationInputData vehicleData, IActuations actuations)
		{
			var avgInclBase = 0.SI<Ampere>();
			var avgWithoutBase = 0.SI<Ampere>();
			var doorDutyCycleFraction =
				(actuations.ParkBrakeAndDoors * Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond) /
				actuations.CycleTime;
			var busAux = vehicleData.Components.BusAuxiliaries;
			var electricDoors = busAux.PneumaticConsumers.DoorDriveTechnology ==
								ConsumerTechnology.Electrically;
			foreach (var consumer in DeclarationData.BusAuxiliaries.DefaultElectricConsumerList.Items) {
				var nbr = CalcNumberInVehicle(consumer.NumberInActualVehicle, mission);
				var dutyCycle = electricDoors && consumer.ConsumerName.Equals(
									Constants.BusAuxiliaries.ElectricalConsumers.DoorsPerVehicleConsumer,
									StringComparison.CurrentCultureIgnoreCase)
					? doorDutyCycleFraction
					: consumer.PhaseIdleTractionOn;

				var current = consumer.NominalCurrent(mission.MissionType) * dutyCycle * nbr;
				if (consumer.Bonus && !VehicleHasConsumer(consumer.ConsumerName, busAux)) {
					current = 0.SI<Ampere>();
				}

				avgInclBase += current;
				if (!consumer.BaseVehicle) {
					avgWithoutBase += current;
				}
			}

			return Tuple.Create(avgInclBase, avgWithoutBase);
		}

		private bool VehicleHasConsumer(string consumerName, IBusAuxiliariesDeclarationData busAux)
		{
			switch (consumerName) {
				case "Day running lights LED bonus":
				case "Position lights LED bonus":
				case "Brake lights LED bonus": return false;
				case "Interior lights LED bonus":
				case "Headlights LED bonus": return true;
				default: return false;
			}
		}

		private double CalcNumberInVehicle(string nbr, Mission mission)
		{
			if ("f_IntLight(L_CoC)".Equals(nbr, StringComparison.InvariantCultureIgnoreCase)) {
				var busParams = mission.BusParameter;
				return DeclarationData.BusAuxiliaries.CalculateLengthInteriorLights(
					busParams.VehicleLength, busParams.DoubleDecker, busParams.FloorType, busParams.NumberPassengersLowerDeck).Value();
			}

			return nbr.ToDouble();
		}

		private IPneumaticUserInputsConfig GetPneumaticUserConfig(IVehicleDeclarationInputData vehicleData, Mission mission)
		{
			var busAux = vehicleData.Components.BusAuxiliaries;

			//throw new NotImplementedException();
			return new PneumaticUserInputsConfig() {
				KneelingHeight = mission.BusParameter.FloorType == FloorType.LowFloor
					? Constants.BusAuxiliaries.PneumaticUserConfig.DefaultKneelingHeight
					: 0.SI<Meter>(),
				CompressorGearEfficiency = Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency,
				CompressorGearRatio = busAux.PneumaticSupply.Ratio,
				CompressorMap = GetCompressorMap(busAux.PneumaticSupply.CompressorSize),
				SmartAirCompression = busAux.PneumaticSupply.SmartAirCompression,
				SmartRegeneration = busAux.PneumaticSupply.SmartRegeneration,
			};
		}

		private ICompressorMap GetCompressorMap(string compressorSize)
		{
			var resource = "";
			switch (compressorSize) {
				case "Small":
				case "Small + visco clutch":
				case "Small + mech. clutch":
					resource = "DEFAULT_1-Cylinder_1-Stage_393ccm.ACMP";
					break;
				case "Medium Supply 1-stage":
				case "Medium Supply 1-stage + visco clutch":
				case "Medium Supply 1-stage + mech. clutch":
					resource = "DEFAULT_1-Cylinder_1-Stage_393ccm.ACMP";
					break;
				case "Medium Supply 2-stage":
				case "Medium Supply 2-stage + visco clutch":
				case "Medium Supply 2-stage + mech. clutch":
					resource = "DEFAULT_2-Cylinder_1-Stage_650ccm.ACMP";
					break;
				case "Large Supply 1-stage":
				case "Large Supply 1-stage + visco clutch":
				case "Large Supply 1-stage + mech. clutch":
					resource = "DEFAULT_2-Cylinder_2-Stage_398ccm.ACMP";
					break;
				case "Large Supply 2-stage":
				case "Large Supply 2-stage + visco clutch":
				case "Large Supply 2-stage + mech. clutch":
					resource = "DEFAULT_3-Cylinder_2-Stage_598ccm.ACMP";
					break;
				default: throw new ArgumentException(string.Format("unkown compressor size {0}"), compressorSize);
			}

			return CompressorMapReader.ReadStream(
				RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix + ".VAUXBuses." + resource));
		}

		public virtual ISSMInputs CreateSSMModelParameters(IVehicleData vehicleData, Mission mission, IFuelProperties heatingFuel)
		{
			var retVal = new SSMInputs(vehicleData, null, heatingFuel) {
				Technologies = DeclarationData.BusAuxiliaries.SSMTechnologyList,
				DefaultConditions = new EnvironmentalConditionMapEntry(
					Constants.BusAuxiliaries.SteadyStateModel.DefaultTemperature,
					Constants.BusAuxiliaries.SteadyStateModel.DefaultSolar,
					1.0),
				EnvironmentalConditionsMap = DeclarationData.BusAuxiliaries.DefaultEnvironmentalConditions,
				HeatingBoundaryTemperature = Constants.BusAuxiliaries.SteadyStateModel.HeatingBoundaryTemperature,
				CoolingBoundaryTemperature = Constants.BusAuxiliaries.SteadyStateModel.CoolingBoundaryTemperature,

				//HighVentilation = Constants.BusAuxiliaries.SteadyStateModel.HighVentilation,
				//LowVentilation = Constants.BusAuxiliaries.SteadyStateModel.LowVentilation,
				SpecificVentilationPower = Constants.BusAuxiliaries.SteadyStateModel.SpecificVentilationPower,

				HVACMaxCoolingPower = CalculateMaxCoolingPower(vehicleData, mission),
				HVACCompressorType = mission.BusParameter.HVACCompressorType,

				AuxHeaterEfficiency = Constants.BusAuxiliaries.SteadyStateModel.AuxHeaterEfficiency,
				FuelFiredHeaterPower = mission.BusParameter.HVACAuxHeaterPower,
				FuelEnergyToHeatToCoolant = Constants.BusAuxiliaries.Heater.FuelEnergyToHeatToCoolant,
				CoolantHeatTransferredToAirCabinHeater = Constants.BusAuxiliaries.Heater.CoolantHeatTransferredToAirCabinHeater,
				GFactor = Constants.BusAuxiliaries.SteadyStateModel.GFactor,
				VentilationOnDuringHeating = true,
				VentilationWhenBothHeatingAndACInactive = true,
				VentilationDuringAC = true,

				//VentilationDuringHeating = VentilationLevel.High,
				//VentilationDuringCooling = VentilationLevel.High,
				//VentilationFlowSettingWhenHeatingAndACInactive = VentilationLevel.High,
				MaxPossibleBenefitFromTechnologyList =
					Constants.BusAuxiliaries.SteadyStateModel.MaxPossibleBenefitFromTechnologyList,

			};

			DeclarationData.BusAuxiliaries.SetHVACParameters(retVal, mission.BusParameter.HVACConfiguration);

			return retVal;
		}

		private Watt CalculateMaxCoolingPower(IVehicleData vehicleData, Mission mission)
		{
			var busParams = mission.BusParameter;

			var length = DeclarationData.BusAuxiliaries.CalculateInternalLength(
				busParams.VehicleLength, busParams.DoubleDecker, busParams.FloorType,
				busParams.NumberPassengersLowerDeck);
			var height = DeclarationData.BusAuxiliaries.CalculateInternalHeight(mission.VehicleHeight);
			var volume = length * height * busParams.VehicleWidth;

			// todo: subtract driver compartment from passenger compartment for certain configurations.

			var driver = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.DriverMaxCoolingPower(
				busParams.HVACConfiguration, mission.MissionType);
			var passenger = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.PassengerMaxCoolingPower(
				busParams.HVACConfiguration, mission.MissionType, volume);

			return driver + passenger;
		}

		public virtual IPneumaticsConsumersDemand CreatePneumaticAuxConfig(RetarderType retarderType)
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
