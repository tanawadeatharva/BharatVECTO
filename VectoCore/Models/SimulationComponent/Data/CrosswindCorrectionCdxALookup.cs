using System.Collections.Generic;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Utils;


namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class CrosswindCorrectionCdxALookup : LoggingObject, ICrossWindCorrection
	{
		protected List<CrossWindCorrectionCurveReader.CrossWindCorrectionEntry> Entries;

		public CrosswindCorrectionCdxALookup(List<CrossWindCorrectionCurveReader.CrossWindCorrectionEntry> entries,
			CrossWindCorrectionMode correctionMode)
		{
			CorrectionMode = correctionMode;
			Entries = entries;
		}

		public CrossWindCorrectionMode CorrectionMode { get; internal set; }

		public void SetDataBus(IDataBus dataBus) {}

		public Watt AverageAirDragPowerLoss(MeterPerSecond v1, MeterPerSecond v2, Second dt)
		{
			var vAverage = (v1 + v2) / 2;
			var CdA = EffectiveAirDragArea(vAverage);
			Watt averageAirDragPower;
			if (v1.IsEqual(v2)) {
				averageAirDragPower = (Physics.AirDensity / 2.0 * CdA * vAverage * vAverage * vAverage).Cast<Watt>();
			} else {
				// compute the average force within the current simulation interval
				// P(t) = k * CdA * v(t)^3  , v(t) = v0 + a * t  // a != 0, P_avg = 1/T * Integral P(t) dt
				// => P_avg = (CdA * rho/2)/(4*a * dt) * (v2^4 - v1^4)
				var acceleration = (v2 - v1) / dt;
				averageAirDragPower =
					(Physics.AirDensity / 2.0 * CdA * (v2 * v2 * v2 * v2 - v1 * v1 * v1 * v1) / (4 * acceleration * dt))
						.Cast<Watt>();
			}
			return averageAirDragPower;
		}

		protected internal SquareMeter EffectiveAirDragArea(MeterPerSecond x)
		{
			var p = Entries.GetSection(c => c.Velocity < x);

			if (x < p.Item1.Velocity || p.Item2.Velocity < x) {
				//Log.Error(_data.CrossWindCorrectionMode == CrossWindCorrectionMode.VAirBetaLookupTable
				//    ? string.Format("CdExtrapol β = {0}", x)
				//    : string.Format("CdExtrapol v = {0}", x));
				Log.Error("CdExtrapol v = {0}", x);
			}

			return VectoMath.Interpolate(p.Item1.Velocity, p.Item2.Velocity,
				p.Item1.EffectiveCrossSectionArea, p.Item2.EffectiveCrossSectionArea, x);
		}
	}
}