using System;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class M02Impl : AbstractModule, IM2_AverageElectricalLoadDemand
	{
		private Volt _powerNetVoltage;
		private IM0_1_AverageElectricLoadDemand _m0_1;
		private IM0_NonSmart_AlternatorsSetEfficiency _module0;
		private double _alternatorPulleyEffiency;

		public M02Impl(
			IM0_1_AverageElectricLoadDemand m0_1, IM0_NonSmart_AlternatorsSetEfficiency m0, double altPulleyEfficiency,
			Volt powerNetVoltage)
		{
			if (m0_1 == null) {
				throw new ArgumentException("Electrical Consumer List must be supplied");
			}
			if (m0 == null) {
				throw new ArgumentException("Must supply module 0");
			}
			if (altPulleyEfficiency.IsEqual(0) || altPulleyEfficiency > 1) {
				throw new ArgumentException("Alternator Gear efficiency out of range.");
			}
			if (powerNetVoltage < Constants.BusAuxiliaries.ElectricConstants.PowenetVoltageMin || powerNetVoltage > Constants.BusAuxiliaries.ElectricConstants.PowenetVoltageMax) {
				throw new ArgumentException("Powernet Voltage out of known range.");
			}

			_powerNetVoltage = powerNetVoltage;
			_m0_1 = m0_1;
			_module0 = m0;
			_alternatorPulleyEffiency = altPulleyEfficiency;
		}

		#region Implementation of IM2_AverageElectricalLoadDemand

		public Watt GetAveragePowerAtCrankFromElectrics()
		{
			var electricalPowerDemandsWatts = _powerNetVoltage * _m0_1.TotalAverageDemandAmpsIncludingBaseLoad;
			var alternatorsEfficiency = _module0.AlternatorsEfficiency;
			var electricalPowerDemandsWattsDividedByAlternatorEfficiency =
				electricalPowerDemandsWatts * (1 / alternatorsEfficiency);

			var averagePowerDemandAtCrankFromElectricsWatts =
				electricalPowerDemandsWattsDividedByAlternatorEfficiency * (1 / _alternatorPulleyEffiency);

			return averagePowerDemandAtCrankFromElectricsWatts;
		}

		#endregion
	}
}
