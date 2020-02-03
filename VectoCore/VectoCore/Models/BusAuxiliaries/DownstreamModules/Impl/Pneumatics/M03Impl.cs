using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics
{
	public class M03Impl : AbstractModule, IM3_AveragePneumaticLoadDemand
	{
		protected IPneumaticUserInputsConfig _pneumaticUserInputsConfig;
		protected IPneumaticsConsumersDemand _pneumaticAuxillariesConfig;
		protected ICompressorMap _pneumaticsCompressorFlowRateMap;
		protected Kilogram _vehicleMassKG;
		protected ISignals _signals;


		//public M03Impl(IPneumaticUserInputsConfig pneumaticsUserInputConfig, IPneumaticsAuxilliariesConfig pneumaticsAuxillariesConfig, IPneumaticActuationsMap pneumaticsActuationsMap, ICompressorMap pneumaticsCompressorFlowRateMap, Kilogram vehicleMassKG, string cycleName, ISignals signals)
		public M03Impl(IAuxiliaryConfig auxConfig, ICompressorMap compressorMap, IActuations actuations, ISignals signals)
		{
			_pneumaticUserInputsConfig = auxConfig.PneumaticUserInputsConfig;
			_pneumaticAuxillariesConfig = auxConfig.PneumaticAuxillariesConfig;
			_pneumaticsCompressorFlowRateMap = compressorMap;
			_vehicleMassKG = auxConfig.VehicleData.TotalVehicleMass;
			
			_signals = signals;

			//'Calculate the Total Required Air Delivery Rate L / S
			TotalAirDemand = TotalAirDemandCalculation(actuations);
			AverageAirConsumed = TotalAirDemand / actuations.CycleTime;
		}


		public NormLiter TotalAirDemandCalculation(IActuations actuations)
		{
			//'* * Breaks * *
			var airConsumptionPerActuation =  _pneumaticAuxillariesConfig.Braking * _vehicleMassKG;
			var breaks = actuations.Braking * airConsumptionPerActuation ;

			//'* * ParkBrakesBreakplus2Doors * *Park break +2 doors
			airConsumptionPerActuation = _pneumaticUserInputsConfig.Doors == ConsumerTechnology.Electrically
				? 0.SI<NormLiter>()
				: _pneumaticAuxillariesConfig.DoorOpening;
			airConsumptionPerActuation += _pneumaticAuxillariesConfig.StopBrakeActuation * _vehicleMassKG;
			var parkBrakesplus2Doors = (actuations.ParkBrakeAndDoors * airConsumptionPerActuation);

			//'* * Kneeling * *
			airConsumptionPerActuation = _pneumaticAuxillariesConfig.BreakingWithKneeling *
										_pneumaticUserInputsConfig.KneelingHeight * _vehicleMassKG;
			var kneeling = (actuations.Kneeling * airConsumptionPerActuation);

			//'* * AdBlue * *
			var adBlue = _pneumaticUserInputsConfig.AdBlueDosing == ConsumerTechnology.Electrically
				? 0.SI<NormLiter>()
				: _pneumaticAuxillariesConfig.AdBlueInjection * actuations.CycleTime;

			//'* * Regeneration * *
			var regeneration = breaks + parkBrakesplus2Doors + kneeling + adBlue;
			var regenFraction = _pneumaticUserInputsConfig.SmartRegeneration
				? _pneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand
				: _pneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand;
			regeneration = regeneration * regenFraction;

			//'* * DeadVolBlowOuts * *
			airConsumptionPerActuation = _pneumaticAuxillariesConfig.DeadVolume;
			var deadVolBlowOuts = (airConsumptionPerActuation * _pneumaticAuxillariesConfig.DeadVolBlowOuts *
									actuations.CycleTime).Cast<NormLiter>();

			//'* * AirSuspension * *
			var airSuspension = _pneumaticUserInputsConfig.AirSuspensionControl == ConsumerTechnology.Electrically
				? 0.SI<NormLiter>()
				: _pneumaticAuxillariesConfig.AirControlledSuspension * actuations.CycleTime;

			//'* * Total Air Demand**
			var totalAirDemand = breaks + parkBrakesplus2Doors + kneeling + adBlue + regeneration + deadVolBlowOuts +
								airSuspension;
			
			return totalAirDemand;
		}

		
		#region Implementation of IM3_AveragePneumaticLoadDemand

		public Watt GetAveragePowerDemandAtCrankFromPneumatics()
		{
			var cmp = _pneumaticsCompressorFlowRateMap.Interpolate(
				_signals.EngineSpeed * _pneumaticUserInputsConfig.CompressorGearRatio);

			var sum6 = cmp.FlowRate;
			var sum7 = cmp.PowerOn - cmp.PowerOff;
			var sum2 = (sum7 / sum6 * AverageAirConsumed); // ' Watt / Nl/s * Nl/s = Watt
			var sum3 = sum2 + cmp.PowerOff;
			var sum4 = sum3 * (1 / _pneumaticUserInputsConfig.CompressorGearEfficiency);
			return sum4;
		}

		public NormLiterPerSecond AverageAirConsumed { get; }

		public NormLiter TotalAirDemand { get; }

		#endregion
	}
}
