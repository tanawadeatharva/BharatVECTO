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
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

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
			retVal.Length = mission.VehicleLength;
			retVal.Width = mission.VehicleWidth;
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

		public IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData vehicleData, VectoRunData runData)
		{
			var actuations = DeclarationData.BusAuxiliaries.ActuationsMap.Lookup(runData.Mission.MissionType);

			var retVal = new AuxiliaryConfig {
				InputData = vehicleData.Components.BusAuxiliaries,
				ElectricalUserInputsConfig = GetElectricalUserConfig(mission, vehicleData, actuations),
				PneumaticUserInputsConfig = GetPneumaticUserConfig(vehicleData),
				PneumaticAuxillariesConfig = CreatePneumaticAuxConfig(runData.Retarder.Type),
				Actuations = actuations,
				SSMInputs = CreateSSMModelParameters(runData.VehicleData, FuelData.Diesel),
				VehicleData = runData.VehicleData,
				FuelMap = runData.EngineData.Fuels.First().ConsumptionMap
			};



			return retVal;
		}

		private IElectricsUserInputsConfig GetElectricalUserConfig(Mission mission, IVehicleDeclarationInputData vehicleData, IActuations actuations)
		{
			var avgInclBase = 0.SI<Ampere>();
			var avgWithoutBase = 0.SI<Ampere>();
			var doorDutyCycleFraction =
				(actuations.ParkBrakeAndDoors * Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond) /
				actuations.CycleTime;
			var electricDoors = vehicleData.Components.BusAuxiliaries.PneumaticConsumers.DoorDriveTechnology ==
								ConsumerTechnology.Electrically;
			foreach (var consumer in DeclarationData.BusAuxiliaries.DefaultElectricConsumerList.Items) {
				var nbr = CalcNumberInVehicle(consumer.NumberInActualVehicle, mission);
				var dutyCycle = electricDoors && consumer.ConsumerName.Equals(
									Constants.BusAuxiliaries.ElectricalConsumers.DoorsPerVehicleConsumer,
									StringComparison.CurrentCultureIgnoreCase)
					? doorDutyCycleFraction
					: consumer.PhaseIdleTractionOn;

				var current = consumer.NominalCurrent(mission.MissionType) * dutyCycle * nbr;
				if (consumer.Bonus && !VehicleHasConsumer(consumer.ConsumerName, vehicleData.Components.BusAuxiliaries)) {
					current = 0.SI<Ampere>();
				}

				avgInclBase += current;
				if (!consumer.BaseVehicle) {
					avgWithoutBase += current;
				}
			}


			return new ElectricsUserInputsConfig() {
				SmartElectrical = false,
				AverageCurrentDemandInclBaseLoad = avgInclBase,
				AverageCurrentDemandWithoutBaseLoad = avgWithoutBase,
				AlternatorMap = new AlternatorMap(new List<ICombinedAlternatorMapRow>() {new CombinedAlternatorMapRow("test", 1000.RPMtoRad(), 10.SI<Ampere>(), 0.8, 1.0)  }, ""),
				PowerNetVoltage = Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage,
				ResultCardIdle = new ResultCard(new List<SmartResult>()),
				ResultCardOverrun = new ResultCard(new List<SmartResult>()),
				ResultCardTraction = new ResultCard(new List<SmartResult>()),
				AlternatorGearEfficiency = 0.9,
			};
		}

		private bool VehicleHasConsumer(string consumerName, IBusAuxiliariesDeclarationData busAux)
		{
			switch (consumerName) {
				case "Interior lights LED bonus": return busAux.ElectricConsumers.InteriorLightsLED;
				case "Day running lights LED bonus": return busAux.ElectricConsumers.DayrunninglightsLED;
				case "Position lights LED bonus": return busAux.ElectricConsumers.PositionlightsLED;
				case "Brake lights LED bonus": return busAux.ElectricConsumers.BrakelightsLED;
				case "Headlights LED bonus": return busAux.ElectricConsumers.HeadlightsLED;
				default: return false;
			}
		}

		private double CalcNumberInVehicle(string nbr, Mission mission)
		{
			if ("f_IntLight(L_CoC)".Equals(nbr, StringComparison.InvariantCultureIgnoreCase)) {
				return DeclarationData.BusAuxiliaries.CalculateLengthInteriorLights(
					mission.VehicleLength, mission.DoubleDecker, mission.FloorType, mission.NumberPassengersLowerDeck).Value();
			}
			return nbr.ToDouble();
		}

		private IPneumaticUserInputsConfig GetPneumaticUserConfig(IVehicleDeclarationInputData vehicleData)
		{
			//throw new NotImplementedException();
			return new PneumaticUserInputsConfig() {
				SmartRegeneration = false,
				KneelingHeight = 0.SI<Meter>()
			};
		}

		public virtual ISSMInputs CreateSSMModelParameters(IVehicleData vehicleData, IFuelProperties heatingFuel)
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

				// TODO! MQ 2019-19-29 Compressor Type and CompressorCapacity from input data?
				HVACCompressorType = ACCompressorType.TwoStage, // "2-stage",
				HVACMaxCoolingPower = 18.SI(Unit.SI.Kilo.Watt).Cast<Watt>(),

				AuxHeaterEfficiency = Constants.BusAuxiliaries.SteadyStateModel.AuxHeaterEfficiency,
				FuelFiredHeaterPower = Constants.BusAuxiliaries.SteadyStateModel.FuelFiredHeaterPower,
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

			DeclarationData.BusAuxiliaries.SetHVACParameters(retVal, BusHVACSystemConfiguration.Configuration6);

			return retVal;
		}

		public virtual IPneumaticsConsumersDemand CreatePneumaticAuxConfig(RetarderType retarderType)
		{
			return new PneumaticsConsumersDemand() {
				AdBlueInjection = Constants.BusAuxiliaries.PneumaticConsumersDemands.AdBlueInjection,
				AirControlledSuspension = Constants.BusAuxiliaries.PneumaticConsumersDemands.AirControlledSuspension,
				Braking = retarderType == RetarderType.None ?
					Constants.BusAuxiliaries.PneumaticConsumersDemands.BrakingNoRetarder :
					Constants.BusAuxiliaries.PneumaticConsumersDemands.BrakingWithRetarder,
				BreakingWithKneeling = Constants.BusAuxiliaries.PneumaticConsumersDemands.BreakingAndKneeling,
				DeadVolBlowOuts = Constants.BusAuxiliaries.PneumaticConsumersDemands.DeadVolBlowOuts,
				DeadVolume = Constants.BusAuxiliaries.PneumaticConsumersDemands.DeadVolume,
				NonSmartRegenFractionTotalAirDemand = Constants.BusAuxiliaries.PneumaticConsumersDemands.NonSmartRegenFractionTotalAirDemand,
				SmartRegenFractionTotalAirDemand = Constants.BusAuxiliaries.PneumaticConsumersDemands.SmartRegenFractionTotalAirDemand,
				OverrunUtilisationForCompressionFraction = Constants.BusAuxiliaries.PneumaticConsumersDemands.OverrunUtilisationForCompressionFraction,
				DoorOpening = Constants.BusAuxiliaries.PneumaticConsumersDemands.DoorOpening,
				StopBrakeActuation = Constants.BusAuxiliaries.PneumaticConsumersDemands.StopBrakeActuation,
			};
		}
	}
}
