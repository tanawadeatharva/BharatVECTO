using System;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;

namespace TUGraz.VectoCore.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class M00Impl : AbstractModule, IM0_NonSmart_AlternatorsSetEfficiency
	{
		protected Ampere _getHVACElectricalPowerDemandAmps;
		protected double _alternatorsEfficiency;
		protected IElectricalConsumerList _electricalConsumersList;
		protected IAlternatorMap _alternatorEfficiencyMap;
		protected Volt _powernetVoltage;
		protected ISignals _signals;
		protected ISSMTOOL _steadyStateModelHVAC;
		protected Watt _ElectricalPowerW;
		protected Watt _MechanicalPowerW;
		protected LiterPerSecond _FuelingLPerH;

		public M00Impl(
			IElectricalConsumerList electricalConsumers, IAlternatorMap alternatorEfficiencyMap, Volt powernetVoltage,
			ISignals signals, ISSMTOOL ssmHvac)
		{
			if (electricalConsumers == null) {
				throw new ArgumentException("No ElectricalConsumersList Supplied");
			}

			if (alternatorEfficiencyMap == null) {
				throw new ArgumentException("No Alternator Efficiency Map Supplied");
			}

			if (powernetVoltage < ElectricConstants.PowenetVoltageMin || powernetVoltage > ElectricConstants.PowenetVoltageMax) {
				throw new ArgumentException("Powernet Voltage out of range");
			}

			if (signals == null) {
				throw new ArgumentException("No Signals reference was supplied.");
			}

			_electricalConsumersList = electricalConsumers;

			_alternatorEfficiencyMap = alternatorEfficiencyMap;

			_powernetVoltage = powernetVoltage;

			_signals = signals;

			_steadyStateModelHVAC = ssmHvac;

			_ElectricalPowerW = ssmHvac.ElectricalWAdjusted.SI<Watt>();

			_MechanicalPowerW = ssmHvac.MechanicalWBaseAdjusted.SI<Watt>();

			_FuelingLPerH = ssmHvac.FuelPerHBaseAdjusted.SI<LiterPerSecond>();
		}

		#region Implementation of IM0_NonSmart_AlternatorsSetEfficiency

		public Ampere GetHVACElectricalPowerDemandAmps
		{
			get { return _ElectricalPowerW / _powernetVoltage; }
		}

		public double AlternatorsEfficiency
		{
			get {
				var baseCurrentDemandAmps = _electricalConsumersList.GetTotalAverageDemandAmps(false);
				var totalDemandAmps = baseCurrentDemandAmps + GetHVACElectricalPowerDemandAmps;
				return _alternatorEfficiencyMap.GetEfficiency(_signals.EngineSpeed.AsRPM, totalDemandAmps).Efficiency;
			}
		}

		#endregion
	}
}
