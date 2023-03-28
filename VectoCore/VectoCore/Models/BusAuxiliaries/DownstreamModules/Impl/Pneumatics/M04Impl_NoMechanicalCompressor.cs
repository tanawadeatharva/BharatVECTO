using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics
{
	public class M04Impl_NoMechanicalCompressor : IM4_AirCompressor
	{
		#region Implementation of IAbstractModule

		public void ResetCalculations()
		{
			
		}

		#endregion

		#region Implementation of IM4_AirCompressor

		public double PulleyGearRatio => double.NaN;
		public double PulleyGearEfficiency => double.NaN;
		public NormLiterPerSecond GetFlowRate()
		{
			return 0.SI<NormLiterPerSecond>();
		}

		public Watt GetPowerCompressorOff()
		{
			return 0.SI<Watt>();
		}

		public Watt GetPowerCompressorOn()
		{
			return 0.SI<Watt>();
		}

		public Watt GetPowerDifference()
		{
			return 0.SI<Watt>();
		}

		public JoulePerNormLiter GetAveragePowerDemandPerCompressorUnitFlowRate()
		{
			return 0.SI<JoulePerNormLiter>();
		}

		#endregion
	}
}