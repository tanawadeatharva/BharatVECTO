using TUGraz.VectoCommon.Utils;
using VectoAuxiliaries;
using VectoAuxiliaries.Pneumatics;

namespace TUGraz.VectoCore.BusAuxiliaries.DownstreamModules.Impl
{
	public class M03Impl : AbstractModule, IM3_AveragePneumaticLoadDemand
	{
		protected IPneumaticUserInputsConfig _pneumaticUserInputsConfig;
		protected IPneumaticsAuxilliariesConfig _pneumaticAuxillariesConfig;
		protected IPneumaticActuationsMAP _pneumaticsActuationsMap;
		protected ICompressorMap _pneumaticsCompressorFlowRateMap;
		protected Kilogram _vehicleMassKG;
		protected string _cycleName;
		protected ISignals _signals;
		protected SI _averagePowerDemandPerCompressorUnitFlowRateInWPerLitresPerSecond;
		protected NormLiter _totalAirDemand;

		
		public M03Impl(IPneumaticUserInputsConfig pneumaticsUserInputConfig, IPneumaticsAuxilliariesConfig pneumaticsAuxillariesConfig, IPneumaticActuationsMAP pneumaticsActuationsMap, ICompressorMap pneumaticsCompressorFlowRateMap, Kilogram vehicleMassKG, string cycleName, ISignals signals)
		{
			_pneumaticUserInputsConfig = pneumaticsUserInputConfig;
			_pneumaticAuxillariesConfig = pneumaticsAuxillariesConfig;
			_pneumaticsActuationsMap = pneumaticsActuationsMap;
			_pneumaticsCompressorFlowRateMap = pneumaticsCompressorFlowRateMap;
			_vehicleMassKG = vehicleMassKG;
			_cycleName = cycleName;
			_signals = signals;


			//'Total up the blow demands from compressor map
			_averagePowerDemandPerCompressorUnitFlowRateInWPerLitresPerSecond =
				(_pneumaticsCompressorFlowRateMap.GetAveragePowerDemandPerCompressorUnitFlowRate() / 60).SI();

			//'Calculate the Total Required Air Delivery Rate L / S
			_totalAirDemand = TotalAirDemandCalculation();
		}


		private NormLiter TotalAirDemandCalculation()
		{
			//'These calculation are done directly from formulae provided from a supplied spreadsheet.

			//'* * Breaks * *
			double numActuationsPerCycle = _pneumaticsActuationsMap.GetNumActuations(new ActuationsKey("Brakes", _cycleName));
			//'=IF(K10 = "yes", IF(COUNTBLANK(F33), G33, F33), IF(COUNTBLANK(F34), G34, F34)) * K16
			var airConsumptionPerActuationNI = _pneumaticUserInputsConfig.RetarderBrake
				? _pneumaticAuxillariesConfig.BrakingWithRetarderNIperKG
				: _pneumaticAuxillariesConfig.BrakingNoRetarderNIperKG;
			var Breaks = (numActuationsPerCycle * airConsumptionPerActuationNI * _vehicleMassKG.Value()).SI<NormLiter>();

			//'* * ParkBrakesBreakplus2Doors * *Park break +2 doors
			numActuationsPerCycle =
				_pneumaticsActuationsMap.GetNumActuations(new ActuationsKey("Park brake + 2 doors", _cycleName));
			//'=SUM(IF(K14 = "electric", 0, IF(COUNTBLANK(F36), G36, F36)), PRODUCT(K16 * IF(COUNTBLANK(F37), G37, F37)))
			airConsumptionPerActuationNI = _pneumaticUserInputsConfig.Doors == "Electric"
				? 0
				: _pneumaticAuxillariesConfig.PerDoorOpeningNI;
			airConsumptionPerActuationNI += _pneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG * _vehicleMassKG.Value();
			var ParkBrakesplus2Doors = (numActuationsPerCycle * airConsumptionPerActuationNI).SI<NormLiter>();

			//'* * Kneeling * *
			numActuationsPerCycle = _pneumaticsActuationsMap.GetNumActuations(new ActuationsKey("Kneeling", _cycleName));
			//'=IF(COUNTBLANK(F35), G35, F35) * K11 * K16
			airConsumptionPerActuationNI = _pneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM * _vehicleMassKG.Value() *
											_pneumaticUserInputsConfig.KneelingHeightMillimeters;
			var Kneeling = (numActuationsPerCycle * airConsumptionPerActuationNI).SI<NormLiter>();

			//'* * AdBlue * *
			//	'=IF(K13 = "electric", 0, G39 * F54) - Supplied Spreadsheet
			var AdBlue = (_pneumaticUserInputsConfig.AdBlueDosing == "Electric" ? 0 :
				_pneumaticAuxillariesConfig.AdBlueNIperMinute * (_signals.TotalCycleTimeSeconds / 60.0)).SI<NormLiter>();

			//'* * Regeneration * *
			//	'=SUM(R6: R9) * IF(K9 = "yes", IF(COUNTBLANK(F41), G41, F41), IF(COUNTBLANK(F40), G40, F40)) - Supplied SpreadSheet
			var Regeneration = Breaks + ParkBrakesplus2Doors + Kneeling + AdBlue;
			var regenFraction = _pneumaticUserInputsConfig.SmartRegeneration
				? _pneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand
				: _pneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand;
			Regeneration = Regeneration * regenFraction;

			//'* * DeadVolBlowOuts * *
			//	'=IF(COUNTBLANK(F43), G43, F43) / (F54 / 60) - Supplied SpreadSheet
			numActuationsPerCycle = _pneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour /
									(60.0 / (_signals.TotalCycleTimeSeconds / 60.0));
			airConsumptionPerActuationNI = _pneumaticAuxillariesConfig.DeadVolumeLitres;
			var DeadVolBlowOuts = (numActuationsPerCycle * airConsumptionPerActuationNI).SI<NormLiter>();

			//'* * AirSuspension * *
			//	'=IF(K12 = "electrically", 0, G38 * F54) - Suplied Spreadsheet
			var AirSuspension = (_pneumaticUserInputsConfig.AirSuspensionControl == "Electrically" ?  0 :
				_pneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute * (_signals.TotalCycleTimeSeconds / 60.0)).SI<NormLiter>();

			//'* * Total Air Demand**
			var TotalAirDemand = Breaks + ParkBrakesplus2Doors + Kneeling + AdBlue + Regeneration + DeadVolBlowOuts +
								AirSuspension;


			return TotalAirDemand;
		}

		
		#region Implementation of IM3_AveragePneumaticLoadDemand

		public Watt GetAveragePowerDemandAtCrankFromPneumatics()
		{
			var Sum1 = _totalAirDemand / _signals.TotalCycleTimeSeconds.SI<Second>();
			var Sum6 = _pneumaticsCompressorFlowRateMap.GetFlowRate(
							_signals.EngineSpeed.AsRPM * _pneumaticUserInputsConfig.CompressorGearRatio) /
						60;
			var pon = _pneumaticsCompressorFlowRateMap.GetPowerCompressorOn(
					_signals.EngineSpeed.AsRPM * _pneumaticUserInputsConfig.CompressorGearRatio);
			var poff = _pneumaticsCompressorFlowRateMap.GetPowerCompressorOff(
					_signals.EngineSpeed.AsRPM * _pneumaticUserInputsConfig.CompressorGearRatio);
			var diff = pon - poff;
			var Sum7 = diff;

			var Sum2 = (Sum7.Value() / Sum6.Value() * Sum1.Value()).SI<Watt>(); // ' Watt / Nl/s * Nl/s = Watt
			var Sum3 = Sum2 +
						_pneumaticsCompressorFlowRateMap.GetPowerCompressorOff(
							_signals.EngineSpeed.AsRPM * _pneumaticUserInputsConfig.CompressorGearRatio);
			var Sum4 = Sum3 * (1 / _pneumaticUserInputsConfig.CompressorGearEfficiency);
			return Sum4;
		}

		public NormLiterPerSecond AverageAirConsumedPerSecondLitre()
		{
			var Sum1 = _totalAirDemand / _signals.TotalCycleTimeSeconds.SI<Second>();
			return Sum1;
		}

		public NormLiter TotalAirDemand
		{
			get { return _totalAirDemand; }
		}
		

		#endregion
	}
}
