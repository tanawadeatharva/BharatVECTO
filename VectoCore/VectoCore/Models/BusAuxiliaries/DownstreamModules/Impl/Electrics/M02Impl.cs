using System;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class M02Impl : AbstractModule, IM2_AverageElectricalLoadDemand
	{
		private Volt _powerNetVoltage;
		private IElectricalConsumerList _electricalConsumers;
		private IM0_NonSmart_AlternatorsSetEfficiency _module0;
		private double _alternatorPulleyEffiency;

		public M02Impl(
			IElectricalConsumerList electricalConsumers, IM0_NonSmart_AlternatorsSetEfficiency m0, double altPulleyEfficiency,
			Volt powerNetVoltage, ISignals signals)
		{
			if (electricalConsumers == null) {
				throw new ArgumentException("Electrical Consumer List must be supplied");
			}
			if (m0 == null) {
				throw new ArgumentException("Must supply module 0");
			}
			if (altPulleyEfficiency.IsEqual(0) || altPulleyEfficiency > 1) {
				throw new ArgumentException("Alternator Gear efficiency out of range.");
			}
			if (powerNetVoltage < ElectricConstants.PowenetVoltageMin || powerNetVoltage > ElectricConstants.PowenetVoltageMax) {
				throw new ArgumentException("Powernet Voltage out of known range.");
			}

			_powerNetVoltage = powerNetVoltage;
			_electricalConsumers = electricalConsumers;
			_module0 = m0;
			_alternatorPulleyEffiency = altPulleyEfficiency;
		}

		#region Implementation of IM2_AverageElectricalLoadDemand

		public Watt GetAveragePowerDemandAtAlternator()
		{
			return _powerNetVoltage * _electricalConsumers.GetTotalAverageDemandAmps(false);
		}

		public Watt GetAveragePowerAtCrankFromElectrics()
		{
			var ElectricalPowerDemandsWatts = GetAveragePowerDemandAtAlternator();
			var alternatorsEfficiency = _module0.AlternatorsEfficiency;
			var ElectricalPowerDemandsWattsDividedByAlternatorEfficiency =
				ElectricalPowerDemandsWatts * (1 / alternatorsEfficiency);

			var averagePowerDemandAtCrankFromElectricsWatts =
				ElectricalPowerDemandsWattsDividedByAlternatorEfficiency * (1 / _alternatorPulleyEffiency);

			return averagePowerDemandAtCrankFromElectricsWatts;
		}

		#endregion
	}
}
