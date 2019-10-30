using System;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	public class M01Impl : AbstractModule, IM1_AverageHVACLoadDemand
	{
		protected IM0_NonSmart_AlternatorsSetEfficiency _m0;
		protected Double _alternatorGearEfficiency;
		protected Double _compressorGearEfficiency;
		
		protected Watt _ElectricalPowerW;
		protected Watt _MechanicalPowerW;
		protected KilogramPerSecond _FuelingLPerH;

		public M01Impl(IM0_NonSmart_AlternatorsSetEfficiency m0, double altGearEfficiency, double compressorGearEfficiency, ISSMTOOL ssm)
		{
			//'Sanity Check - Illegal operations without all params.
			if (m0 == null) {
				throw new ArgumentException("Module0 as supplied is null");
			}

			if (altGearEfficiency < ElectricConstants.AlternatorPulleyEfficiencyMin ||
				altGearEfficiency > ElectricConstants.AlternatorPulleyEfficiencyMax) {
				throw new ArgumentException(string.Format("Gear efficiency must be between {0} and {1}",
						ElectricConstants.AlternatorPulleyEfficiencyMin, ElectricConstants.AlternatorPulleyEfficiencyMax));
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
			

			_compressorGearEfficiency = compressorGearEfficiency;
			
			_ElectricalPowerW = ssm.ElectricalWAdjusted;
			_MechanicalPowerW = ssm.MechanicalWBaseAdjusted;
			_FuelingLPerH = ssm.FuelPerHBaseAdjusted; // ' SI(Of LiterPerHour)()

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

		public KilogramPerSecond HVACFuelingLitresPerHour()
		{
			return _FuelingLPerH;
		}

		#endregion
	}
}
