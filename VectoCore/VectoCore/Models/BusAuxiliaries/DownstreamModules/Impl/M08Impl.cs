using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
{
	public class M08Impl : AbstractModule, IM8
	{
		protected Watt _auxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries;
		protected Watt _smartElectricalAlternatorPowerGenAtCrank;
		protected bool _compressorFlag;

		protected IM1_AverageHVACLoadDemand _m1;
		protected IM6 _m6;
		protected IM7 _m7;
		protected ISignals _signals;

		public M08Impl(IM1_AverageHVACLoadDemand m1, IM6 m6, IM7 m7, ISignals signals)
		{
			_m1 = m1;
			_m6 = m6;
			_m7 = m7;
			_signals = signals;
		}


		protected override void DoCalculate()
		{
			var sum1 = _m7.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank +
						_m7.SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank;
			var sum2 = _m7.SmartElectricalOnlyAuxAltPowerGenAtCrank + _m6.AveragePowerDemandAtCrankFromPneumatics;
			var sum3 = _m7.SmartPneumaticOnlyAuxAirCompPowerGenAtCrank + _m6.AvgPowerDemandAtCrankFromElectricsIncHVAC;
			var sum4 = _m6.AvgPowerDemandAtCrankFromElectricsIncHVAC + _m6.AveragePowerDemandAtCrankFromPneumatics;
			var sw1 = _signals.SmartPneumatics ? sum1 : sum2;
			var sw2 = _signals.SmartPneumatics ? sum3 : sum4;
			var sw5 = _signals.SmartElectrics ? sw1 : sw2;
			var sw6 = !_signals.EngineStopped;
			var sum5 = _m1.AveragePowerDemandAtCrankFromHVACMechanicalsWatts() + sw5;
			var sum6 = sw6 ? sum5 : 0.SI<Watt>();

			var sw3 = _signals.SmartPneumatics ? _m7.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank :
				_m7.SmartElectricalOnlyAuxAltPowerGenAtCrank;

			var sw4 = _signals.SmartElectrics ? _m6.SmartElecAndPneumaticsCompressorFlag : _m6.SmartPneumaticsOnlyCompressorFlag;

			_auxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries = sum6;
			_smartElectricalAlternatorPowerGenAtCrank = sw3;
			_compressorFlag = sw4;
		}

		#region Implementation of IM8

		public Watt AuxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _auxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries; }
		}

		public Watt SmartElectricalAlternatorPowerGenAtCrank
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _smartElectricalAlternatorPowerGenAtCrank; }
		}

		public bool CompressorFlag
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _compressorFlag; }
		}

		#endregion
	}
}
