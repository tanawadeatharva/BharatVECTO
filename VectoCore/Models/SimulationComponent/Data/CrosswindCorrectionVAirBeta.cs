using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	internal class CrosswindCorrectionVAirBeta : LoggingObject, ICrossWindCorrection
	{
		protected SquareMeter AirDragArea { get; set; }

		protected List<CrossWindCorrectionCurveReader.AirDragBetaEntry> AirDragEntries;
		protected IDataBus DataBus;

		public CrosswindCorrectionVAirBeta(SquareMeter airDragArea,
			List<CrossWindCorrectionCurveReader.AirDragBetaEntry> entries)
		{
			AirDragArea = airDragArea;
			AirDragEntries = entries;
		}

		public void SetDataBus(IDataBus dataBus)
		{
			DataBus = dataBus;
		}

		public CrossWindCorrectionMode CorrectionMode
		{
			get { return CrossWindCorrectionMode.VAirBetaLookupTable; }
		}

		public Watt AverageAirDragPowerLoss(MeterPerSecond v1, MeterPerSecond v2, Second dt)
		{
			if (DataBus == null) {
				throw new VectoException("Databus is not set - can't access vAir, beta!");
			}
			var vAir = DataBus.CycleData.LeftSample.AirSpeedRelativeToVehicle;
			var beta = DataBus.CycleData.LeftSample.WindYawAngle;

			// F_air(t) = k * CdA_korr * v_air^2   // assumption: v_air = const for the current interval
			// P(t) = F_air(t) * v(t) , v(t) = v1 + a * t
			// P_avg = 1/T * Integral P(t) dt
			// P_avg = k * CdA_korr * v_air^2 * (v1 + v2) / 2
			var airDragForce = (AirDragArea + DeltaCdxA(Math.Abs(beta))) * Physics.AirDensity / 2.0 * vAir * vAir;
			var vAverage = (v1 + v2) / 2;

			return (airDragForce * vAverage).Cast<Watt>();
		}

		protected SquareMeter DeltaCdxA(double beta)
		{
			var idx = FindIndex(beta);
			return VectoMath.Interpolate(AirDragEntries[idx - 1].Beta, AirDragEntries[idx].Beta, AirDragEntries[idx - 1].DeltaCdA,
				AirDragEntries[idx].DeltaCdA, beta);
		}

		protected int FindIndex(double beta)
		{
			if (beta < AirDragEntries.First().Beta) {
				throw new VectoSimulationException("Beta / CdxA Lookup table does not cover beta={0}", beta);
			}
			if (beta > AirDragEntries.Last().Beta) {
				throw new VectoSimulationException("Beta / CdxA Lookup table does not cover beta={0}", beta);
			}
			int index;
			AirDragEntries.GetSection(x => x.Beta < beta, out index);
			return index + 1;
		}
	}
}