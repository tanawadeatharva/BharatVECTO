using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;


namespace TUGraz.VectoCore.BusAuxiliaries.DownstreamModules.Impl
{
	public class M14Impl : AbstractModule, IM14
	{
		protected IM13 M13;
		protected ISignals Signals;
		protected IHVACConstants Constants;
		protected ISSMTOOL SSM;

		protected Kilogram _totalCycleFcGrams;
		protected Liter _totalCycleFcLitres;

		public M14Impl(IM13 m13, ISSMTOOL ssm, IHVACConstants hvacConstants, ISignals signals)
		{
			M13 = m13;
			SSM = ssm;
			Constants = hvacConstants;
			Signals = signals;
		}


		protected override void DoCalculate()
		{
			var s1 = M13.WHTCTotalCycleFuelConsumptionGrams * Constants.DieselGCVJperGram;
			var s2 = SSM.GenInputs.AH_FuelEnergyToHeatToCoolant * s1;
			var s3 = s2 * SSM.GenInputs.AH_CoolantHeatTransferredToAirCabinHeater;
			var s4 = s3 / Signals.CurrentCycleTimeInSeconds.SI<Second>();
			var s5 = Signals.CurrentCycleTimeInSeconds.SI<Second>(); // ' / 3600
			var s6 = (s5 * SSM.FuelPerHBaseAsjusted(s4.Value() / 1000).SI(Unit.SI.Liter.Per.Hour)).Cast<Liter>() * Constants.FuelDensity;
			var s7 = M13.WHTCTotalCycleFuelConsumptionGrams + s6;
			var s8 = (s7 / (Constants.FuelDensity)).Cast<Liter>();
			_totalCycleFcGrams = s7;
			_totalCycleFcLitres = s8;
			
		}


		#region Implementation of IM14

		public Kilogram TotalCycleFCGrams
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _totalCycleFcGrams;
			}
		}

		public Liter TotalCycleFCLitres
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _totalCycleFcLitres;
			}
		}

		#endregion
	}
}
