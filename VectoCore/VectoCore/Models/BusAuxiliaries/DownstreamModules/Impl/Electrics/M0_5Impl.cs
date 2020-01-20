using System;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class M0_5Impl : AbstractModule, IM0_5_SmartAlternatorSetEfficiency
	{
		protected Ampere _smartIdleCurrent;
		protected double _alternatorsEfficiencyIdleResultCard;
		protected Ampere _smartTractionCurrent;
		protected double _alternatorsEfficiencyTractionOnResultCard;
		protected Ampere _smartOverrunCurrent;
		protected double _alternatorsEfficiencyOverrunResultCard;
		protected IM0_NonSmart_AlternatorsSetEfficiency _m0;
		protected IAlternatorMap _alternatorMap;
		protected IResultCard _resultCardIdle;
		protected IResultCard _resultCardTraction;
		protected IResultCard _resultCardOverrun;
		protected ISignals _signals;
		private IM0_1_AverageElectricLoadDemand _m0_1;

		public M0_5Impl(IM0_NonSmart_AlternatorsSetEfficiency m0, IM0_1_AverageElectricLoadDemand m0_1, IAlternatorMap alternatorMap, IResultCard resultCardIdle, IResultCard resultCardTraction, IResultCard resultCardOverrun, ISignals signals)
		{
			//'Sanity Check on supplied arguments, throw an argument exception
			if (m0 == null) {
				throw new ArgumentException("Module 0 must be supplied");}

			if (m0_1 == null) {
				throw new ArgumentException("ElectricalConsumablesList must be supplied even if empty");
			}
			if (alternatorMap ==null) {throw new ArgumentException("Must supply a valid alternator map");
			}
			if (resultCardIdle == null) {
				throw new ArgumentException("Result Card 'IDLE' must be supplied even if it has no contents");
			}

			if (resultCardTraction == null) {
				throw new ArgumentException("Result Card 'TRACTION' must be supplied even if it has no contents");
			}

			if (resultCardOverrun == null) {
				throw new ArgumentException("Result Card 'OVERRUN' must be supplied even if it has no contents");
			}
			if (signals == null) {
				throw new ArgumentException("No Signals Reference object was provided ");}

			//'Assignments to private variables.
			_m0 = m0;
			_m0_1 = m0_1;
			_alternatorMap = alternatorMap;
			_resultCardIdle = resultCardIdle;
			_resultCardTraction = resultCardTraction;
			_resultCardOverrun = resultCardOverrun;
			_signals = signals;
		}

		#region Implementation of IM0_5_SmartAlternatorSetEfficiency

		private Ampere HvacPlusNonBaseCurrents()
		{
			return _m0.GetHVACElectricalCurrentDemand + _m0_1.TotalAverageDemandAmpsWithoutBaseLoad;
		}

		public Ampere SmartIdleCurrent
		{
			get {
				var hvacPlusNoneBase = HvacPlusNonBaseCurrents();
				var smartIdleCurrent = _resultCardIdle.GetSmartCurrentResult(hvacPlusNoneBase);

				return smartIdleCurrent; 
			}
		}

		public double AlternatorsEfficiencyIdleResultCard
		{
			get { return _alternatorMap.GetEfficiency(_signals.EngineSpeed, SmartIdleCurrent); }
		}

		public Ampere SmartTractionCurrent
		{
			get { return _resultCardTraction.GetSmartCurrentResult(HvacPlusNonBaseCurrents()); }
		}

		public double AlternatorsEfficiencyTractionOnResultCard
		{
			get { return _alternatorMap.GetEfficiency(_signals.EngineSpeed, SmartTractionCurrent); }
		}

		public Ampere SmartOverrunCurrent
		{
			get { return _resultCardOverrun.GetSmartCurrentResult(HvacPlusNonBaseCurrents()); }
		}

		public double AlternatorsEfficiencyOverrunResultCard
		{
			get { return _alternatorMap.GetEfficiency(_signals.EngineSpeed, SmartOverrunCurrent); }
		}

		#endregion
	}
}
