using System;
using DownstreamModules.Electrics;
using Electrics;
using Hvac;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces;

namespace TUGraz.VectoCore.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	public class M01Impl : AbstractModule, IM1_AverageHVACLoadDemand
	{
		protected IM0_NonSmart_AlternatorsSetEfficiency _m0;
		protected Double _alternatorGearEfficiency;
		protected Double _compressorGearEfficiency;
		protected ISignals _signals;
		protected Volt _powernetVoltage;
		protected ISSMTOOL _steadyStateModel;

		protected Watt _ElectricalPowerW;
		protected Watt _MechanicalPowerW;
		protected LiterPerSecond _FuelingLPerH;

		public M01Impl(IM0_NonSmart_AlternatorsSetEfficiency m0, double altGearEfficiency, double compressorGearEfficiency, Volt powernetVoltage, ISignals signals, ISSMTOOL ssm)
		{
			//'Sanity Check - Illegal operations without all params.
			if (m0 == null) {
				throw new ArgumentException("Module0 as supplied is null");
			}

			if (altGearEfficiency < ElectricConstants.AlternatorPulleyEfficiencyMin ||
				altGearEfficiency > ElectricConstants.AlternatorPulleyEfficiencyMax) {
				throw new ArgumentException(
					string.Format
					(
						"Gear efficiency must be between {0} and {1}",
						ElectricConstants.AlternatorPulleyEfficiencyMin, ElectricConstants.AlternatorPulleyEfficiencyMax));
			}
			if (signals == null) {
				throw new Exception("Signals object as supplied is null");
			}

			if (powernetVoltage < ElectricConstants.PowenetVoltageMin || powernetVoltage > ElectricConstants.PowenetVoltageMax) {
				throw new ArgumentException(
					string.Format(
						"PowenetVoltage supplied must be in the range {0} to {1}", ElectricConstants.PowenetVoltageMin,
						ElectricConstants.PowenetVoltageMax));
			}

			if (ssm == null ) {
				throw new ArgumentException("Steady State model was not supplied");
			}

			if (compressorGearEfficiency < 0 || altGearEfficiency > 1) {
				throw new ArgumentException(String.Format("Compressor Gear efficiency must be between {0} and {1}", 0, 1));
			}

			//'Assign
			_m0 = m0;
			_alternatorGearEfficiency = altGearEfficiency;
			_signals = signals;

			_compressorGearEfficiency = compressorGearEfficiency;
			_powernetVoltage = powernetVoltage;


			_steadyStateModel = ssm;

			_ElectricalPowerW = ssm.ElectricalWAdjusted.SI<Watt>();
			_MechanicalPowerW = ssm.MechanicalWBaseAdjusted.SI<Watt>();
			_FuelingLPerH = ssm.FuelPerHBaseAdjusted.SI(Unit.SI.Liter.Per.Hour).Cast<LiterPerSecond>(); // ' SI(Of LiterPerHour)()

		}

		#region Implementation of IM1_AverageHVACLoadDemand

		public Watt AveragePowerDemandAtCrankFromHVACMechanicalsWatts()
		{
			return _MechanicalPowerW * (1 / _compressorGearEfficiency);
		}

		public Watt AveragePowerDemandAtAlternatorFromHVACElectricsWatts()
		{
			return _ElectricalPowerW;
		}

		public Watt AveragePowerDemandAtCrankFromHVACElectricsWatts()
		{
			return _ElectricalPowerW * (1 / _m0.AlternatorsEfficiency / _alternatorGearEfficiency);
		}

		public LiterPerSecond HVACFuelingLitresPerHour()
		{
			return _FuelingLPerH;
		}

		#endregion
	}
}
