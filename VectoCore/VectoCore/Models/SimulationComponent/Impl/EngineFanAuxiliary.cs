using System;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class EngineFanAuxiliary
	{
		protected readonly double[] FanCoefficients;

		protected readonly double FanDiameter;

		public EngineFanAuxiliary(double[] fanParameters, Meter fanDiameter)
		{
			if (fanParameters.Length < 3) {
				throw new ArgumentException("Three fan parameters are required!");
			}
			FanCoefficients = fanParameters;
			FanDiameter = fanDiameter.ConvertToMilliMeter();
		}

		public Watt PowerDemand(PerSecond fanSpeed)
		{

			return (FanCoefficients[0] * Math.Pow(fanSpeed.AsRPM / FanCoefficients[1], 3) * Math.Pow(FanDiameter / FanCoefficients[2], 5) * 1000).SI<Watt>();
		}
	}
}