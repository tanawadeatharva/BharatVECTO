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
		protected IActuations Actuations;
		protected ICompressorMap _pneumaticsCompressorFlowRateMap;
		protected Kilogram _vehicleMassKG;
		protected ISignals _signals;
		protected NormLiter _totalAirDemand;
		

		//public M03Impl(IPneumaticUserInputsConfig pneumaticsUserInputConfig, IPneumaticsAuxilliariesConfig pneumaticsAuxillariesConfig, IPneumaticActuationsMap pneumaticsActuationsMap, ICompressorMap pneumaticsCompressorFlowRateMap, Kilogram vehicleMassKG, string cycleName, ISignals signals)
		public M03Impl(IAuxiliaryConfig auxConfig, ICompressorMap compressorMap, IActuations actuations, ISignals signals)
		{
			_pneumaticUserInputsConfig = auxConfig.PneumaticUserInputsConfig;
			_pneumaticAuxillariesConfig = auxConfig.PneumaticAuxillariesConfig;
			Actuations = actuations;
			_pneumaticsCompressorFlowRateMap = compressorMap;
			_vehicleMassKG = auxConfig.VehicleData.TotalVehicleMass;
			
			_signals = signals;

			//'Calculate the Total Required Air Delivery Rate L / S
			_totalAirDemand = TotalAirDemandCalculation();
		}


		private NormLiter TotalAirDemandCalculation()
		{
			//'These calculation are done directly from formulae provided from a supplied spreadsheet.


			//'* * Breaks * *
			double numActuationsPerCycle = Actuations.Braking;
			//'=IF(K10 = "yes", IF(COUNTBLANK(F33), G33, F33), IF(COUNTBLANK(F34), G34, F34)) * K16
			var airConsumptionPerActuation =  _pneumaticAuxillariesConfig.Braking * _vehicleMassKG;
			var breaks = numActuationsPerCycle * airConsumptionPerActuation ;

			//'* * ParkBrakesBreakplus2Doors * *Park break +2 doors
			numActuationsPerCycle = Actuations.ParkBrakeAndDoors;
			//'=SUM(IF(K14 = "electric", 0, IF(COUNTBLANK(F36), G36, F36)), PRODUCT(K16 * IF(COUNTBLANK(F37), G37, F37)))
			airConsumptionPerActuation = _pneumaticUserInputsConfig.Doors == ConsumerTechnology.Electrically
				? 0.SI<NormLiter>()
				: _pneumaticAuxillariesConfig.DoorOpening;
			airConsumptionPerActuation += _pneumaticAuxillariesConfig.StopBrakeActuation * _vehicleMassKG;
			var parkBrakesplus2Doors = (numActuationsPerCycle * airConsumptionPerActuation);

			//'* * Kneeling * *
			numActuationsPerCycle = Actuations.Kneeling;
			//'=IF(COUNTBLANK(F35), G35, F35) * K11 * K16
			airConsumptionPerActuation = _pneumaticAuxillariesConfig.BreakingWithKneeling *
										_pneumaticUserInputsConfig.KneelingHeightMillimeters * _vehicleMassKG;
			var kneeling = (numActuationsPerCycle * airConsumptionPerActuation);

			//'* * AdBlue * *
			//	'=IF(K13 = "electric", 0, G39 * F54) - Supplied Spreadsheet
			var adBlue = _pneumaticUserInputsConfig.AdBlueDosing == ConsumerTechnology.Electrically
				? 0.SI<NormLiter>()
				: _pneumaticAuxillariesConfig.AdBlueInjection * Actuations.CycleTime;

			//'* * Regeneration * *
			//	'=SUM(R6: R9) * IF(K9 = "yes", IF(COUNTBLANK(F41), G41, F41), IF(COUNTBLANK(F40), G40, F40)) - Supplied SpreadSheet
			var regeneration = breaks + parkBrakesplus2Doors + kneeling + adBlue;
			var regenFraction = _pneumaticUserInputsConfig.SmartRegeneration
				? _pneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand
				: _pneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand;
			regeneration = regeneration * regenFraction;

			//'* * DeadVolBlowOuts * *
			//	'=IF(COUNTBLANK(F43), G43, F43) / (F54 / 60) - Supplied SpreadSheet
			//numActuationsPerCycle =;
			airConsumptionPerActuation = _pneumaticAuxillariesConfig.DeadVolume;
			var deadVolBlowOuts = (airConsumptionPerActuation * _pneumaticAuxillariesConfig.DeadVolBlowOuts *
									Actuations.CycleTime).Cast<NormLiter>();

			//'* * AirSuspension * *
			//	'=IF(K12 = "electrically", 0, G38 * F54) - Suplied Spreadsheet
			var airSuspension = _pneumaticUserInputsConfig.AirSuspensionControl == ConsumerTechnology.Electrically
				? 0.SI<NormLiter>()
				: _pneumaticAuxillariesConfig.AirControlledSuspension * Actuations.CycleTime;

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

			var sum1 = _totalAirDemand / Actuations.CycleTime;
			var sum6 = cmp.FlowRate;
			
			var sum7 = cmp.PowerOn - cmp.PowerOff;

			var sum2 = (sum7 / sum6 * sum1); // ' Watt / Nl/s * Nl/s = Watt
			var sum3 = sum2 + cmp.PowerOff;
			var sum4 = sum3 * (1 / _pneumaticUserInputsConfig.CompressorGearEfficiency);
			return sum4;
		}

		public NormLiterPerSecond AverageAirConsumed()
		{
			var sum1 = _totalAirDemand / Actuations.CycleTime;
			return sum1;
		}

		public NormLiter TotalAirDemand
		{
			get { return _totalAirDemand; }
		}
		

		#endregion
	}
}
