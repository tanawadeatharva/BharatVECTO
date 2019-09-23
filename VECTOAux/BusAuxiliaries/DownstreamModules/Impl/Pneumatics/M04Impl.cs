using System;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.PneumaticSystem;

namespace TUGraz.VectoCore.BusAuxiliaries.DownstreamModules.Impl.Pneumatics
{
	public class M04Impl : AbstractModule, IM4_AirCompressor
	{
		private const double MinRatio = 1;
		private const double MaxRatio = 10;
		private const double MinEff = 0;
		private const double MaxEff = 1;

		private double _pulleyGearRatio;
		private double _pulleyGearEfficiency;
		private ICompressorMap _map;
		private ISignals _signals;

		public M04Impl(ICompressorMap map, double pulleyGearRatio, double pulleyGearEfficiency, ISignals signals)
		{
			_map = map;
			_pulleyGearRatio = pulleyGearRatio;
			_pulleyGearEfficiency = pulleyGearEfficiency;
			_signals = signals;
		}

		#region Implementation of IM4_AirCompressor

		public double PulleyGearRatio
		{
			get { return _pulleyGearRatio; }
			set {
				if (value < MinRatio || value > MaxRatio) {
					throw new ArgumentOutOfRangeException(
						"pulleyGearRatio", value,
						string.Format("Invalid value, should be in the range {0} to {1}", MinRatio, MaxRatio));
				}

				_pulleyGearRatio = value;
			}
		}

		public double PulleyGearEfficiency
		{
			get { return _pulleyGearEfficiency; }
			set {
				if (value < MinEff || value > MaxEff) {
					throw new ArgumentOutOfRangeException(
						"pulleyGearEfficiency", value,
						String.Format("Invalid value, should be in the range {0} to {1}", MinEff, MaxEff)
					);
				}

				_pulleyGearEfficiency = value;
			}
		}

		public bool Initialise()
		{
			return _map.Initialise();
		}

		public NormLiterPerSecond GetFlowRate()
		{
			var compressorRpm = _signals.EngineSpeed.AsRPM * PulleyGearRatio;
			return _map.GetFlowRate(compressorRpm) / 60;
		}

		public Watt GetPowerCompressorOff()
		{
			return GetCompressorPower(false);
		}

		public Watt GetPowerCompressorOn()
		{
			return GetCompressorPower(true);
		}

		public Watt GetPowerDifference()
		{
			var powerOn = GetPowerCompressorOn();
			var powerOff = GetPowerCompressorOff();
			return powerOn - powerOff;
		}

		public SI GetAveragePowerDemandPerCompressorUnitFlowRate()
		{
			return _map.GetAveragePowerDemandPerCompressorUnitFlowRate().SI();
		}

		#endregion

		private Watt GetCompressorPower(bool compressorOn)
		{
			var compressorRpm = _signals.EngineSpeed.AsRPM * PulleyGearRatio;
			return compressorOn ? _map.GetPowerCompressorOn(compressorRpm) : _map.GetPowerCompressorOff(compressorRpm);
		}
	}
}
