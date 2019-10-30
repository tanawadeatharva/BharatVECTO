using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
{
	public class M14Impl : AbstractModule, IM14
	{
		protected readonly IM13 M13;
		protected readonly ISignals Signals;
		protected readonly ISSMTOOL SSM;

		protected Kilogram _totalCycleFcGrams;
		private readonly IFuelProperties EngineFuel;


		public M14Impl(IM13 m13, ISSMTOOL ssm, IFuelProperties engineFuel, ISignals signals)
		{
			M13 = m13;
			SSM = ssm;
			EngineFuel = engineFuel;
			Signals = signals;
		}


		protected override void DoCalculate()
		{
			var s1 = M13.WHTCTotalCycleFuelConsumption * EngineFuel.LowerHeatingValueVectoEngine;
			var s2 = SSM.SSMInputs.AuxHeater.FuelEnergyToHeatToCoolant * s1;
			var s3 = s2 * SSM.SSMInputs.AuxHeater.CoolantHeatTransferredToAirCabinHeater;
			var s4 = s3 / Signals.CurrentCycleTimeInSeconds.SI<Second>();
			var s5 = Signals.CurrentCycleTimeInSeconds.SI<Second>(); 
			var s6 = s5 * SSM.FuelPerHBaseAsjusted(s4); 
			var s7 = M13.WHTCTotalCycleFuelConsumption + s6;
			_totalCycleFcGrams = s7;
		}


		#region Implementation of IM14

		public Kilogram TotalCycleFC
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _totalCycleFcGrams;
			}
		}

		

		#endregion
	}
}
