using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
{
	public class M07Impl : AbstractModule, IM7
	{
		protected Watt _smartElectricalAndPneumaticAuxAltPowerGenAtCrank;
		protected Watt _smartElectricalAndPneumaticAuxAirCompPowerGenAtCrank;
		protected Watt _smartElectricalOnlyAuxAltPowerGenAtCrank;
		protected Watt _smartPneumaticOnlyAuxAirCompPowerGenAtCrank;

		protected IM5_SmartAlternatorSetGeneration _m5;
		protected IM6 _m6;
		protected ISignals _signals;

		public M07Impl(IM5_SmartAlternatorSetGeneration m5, IM6 m6, ISignals signals)
		{
			_m5 = m5;
			_m6 = m6;
			_signals = signals;
		}

		protected override void DoCalculate()
		{
			var idle = _signals.EngineSpeed <= _signals.EngineIdleSpeed &&
						(!_signals.ClutchEngaged || _signals.InNeutral);

			var sw1 = idle
				? _m5.AlternatorsGenerationPowerAtCrankIdle()
				: _m5.AlternatorsGenerationPowerAtCrankTractionOn();

			var c1 = _m6.OverrunFlag && _signals.ClutchEngaged && _signals.InNeutral == false;
			var sw2 = c1 ? _m6.SmartElecAndPneumaticAltPowerGenAtCrank : sw1;

			var sw3 = c1 ? _m6.SmartElecAndPneumaticAirCompPowerGenAtCrank : _m6.AveragePowerDemandAtCrankFromPneumatics;

			var sw4 = c1 ? _m6.SmartElecOnlyAltPowerGenAtCrank : sw1;

			var sw5 = c1 ? _m6.SmartPneumaticOnlyAirCompPowerGenAtCrank : _m6.AveragePowerDemandAtCrankFromPneumatics;

			_smartElectricalAndPneumaticAuxAltPowerGenAtCrank = sw2;
			_smartElectricalAndPneumaticAuxAirCompPowerGenAtCrank = sw3;
			_smartElectricalOnlyAuxAltPowerGenAtCrank = sw4;
			_smartPneumaticOnlyAuxAirCompPowerGenAtCrank = sw5;
		}

		#region Implementation of IM7

		public Watt SmartElectricalAndPneumaticAuxAltPowerGenAtCrank
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _smartElectricalAndPneumaticAuxAltPowerGenAtCrank;
			}
		}

		public Watt SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _smartElectricalAndPneumaticAuxAirCompPowerGenAtCrank;
			}
		}

		public Watt SmartElectricalOnlyAuxAltPowerGenAtCrank
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _smartElectricalOnlyAuxAltPowerGenAtCrank;
			}
		}

		public Watt SmartPneumaticOnlyAuxAirCompPowerGenAtCrank
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _smartPneumaticOnlyAuxAirCompPowerGenAtCrank;
			}
		}

		#endregion
	}
}
