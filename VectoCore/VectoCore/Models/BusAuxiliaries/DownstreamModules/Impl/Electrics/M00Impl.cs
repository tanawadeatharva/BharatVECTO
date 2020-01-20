using System;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class M00Impl : AbstractModule, IM0_NonSmart_AlternatorsSetEfficiency
	{
		protected IAlternatorMap _alternatorEfficiencyMap;
		protected Volt _powernetVoltage;
		protected ISignals _signals;
		protected Watt _ElectricalPowerW;
		private IM0_1_AverageElectricLoadDemand _m0_1;

		public M00Impl(IM0_1_AverageElectricLoadDemand m0_1, IAlternatorMap alternatorEfficiencyMap, Volt powernetVoltage,
			ISignals signals, ISSMTOOL ssmHvac)
		{
			if (m0_1 == null) {
				throw new ArgumentException("No ElectricalConsumersList Supplied");
			}

			if (alternatorEfficiencyMap == null) {
				throw new ArgumentException("No Alternator Efficiency Map Supplied");
			}

			if (powernetVoltage < Constants.BusAuxiliaries.ElectricConstants.PowenetVoltageMin || powernetVoltage > Constants.BusAuxiliaries.ElectricConstants.PowenetVoltageMax) {
				throw new ArgumentException("Powernet Voltage out of range");
			}

			if (signals == null) {
				throw new ArgumentException("No Signals reference was supplied.");
			}

			_m0_1 = m0_1;
			
			_alternatorEfficiencyMap = alternatorEfficiencyMap;

			_powernetVoltage = powernetVoltage;

			_signals = signals;

			_ElectricalPowerW = ssmHvac.ElectricalWAdjusted;
		}

		#region Implementation of IM0_NonSmart_AlternatorsSetEfficiency

		public Ampere GetHVACElectricalCurrentDemand
		{
			get { return _ElectricalPowerW / _powernetVoltage; }
		}

		public double AlternatorsEfficiency
		{
			get {
				var baseCurrentDemandAmps = _m0_1.TotalAverageDemandAmpsIncludingBaseLoad; // _electricalConsumersList.GetTotalAverageDemandAmps(false);
				var totalDemandAmps = baseCurrentDemandAmps + GetHVACElectricalCurrentDemand;
				return _alternatorEfficiencyMap.GetEfficiency(_signals.EngineSpeed, totalDemandAmps);
			}
		}

		#endregion
	}
}
