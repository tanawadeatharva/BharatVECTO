using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	internal class VAirBetaCrosswindCorrection : LoggingObject, ICrossWindCorrection
	{
		protected SquareMeter AirDragArea { get; set; }

		protected List<AirDragBetaEntry> AirDragEntries;
		protected IDataBus DataBus;

		public VAirBetaCrosswindCorrection(SquareMeter airDragArea, DataTable betaTable)
		{
			AirDragArea = airDragArea;
			if (betaTable.Columns.Count != 2) {
				throw new VectoException("VAir/Beta Crosswind Correction must consist of 2 columns");
			}
			if (betaTable.Rows.Count < 2) {
				throw new VectoException("VAir/Beta Crosswind Correction must consist of at least 2 rows");
			}
			if (HeaderIsValid(betaTable.Columns)) {
				AirDragEntries = CreateFromColumnNames(betaTable);
			} else {
				Log.Warn("VAir/Beta Crosswind Correction header Line is not valid");
				AirDragEntries = CreateFromColumnIndices(betaTable);
			}
		}

		private static List<AirDragBetaEntry> CreateFromColumnIndices(DataTable betaTable)
		{
			return (from DataRow row in betaTable.Rows
				select
					new AirDragBetaEntry() {
						Beta = row.ParseDouble(0),
						DeltaCdA = row.ParseDouble(1).SI<SquareMeter>()
					}).ToList();
		}

		private static List<AirDragBetaEntry> CreateFromColumnNames(DataTable betaTable)
		{
			return (from DataRow row in betaTable.Rows
				select
					new AirDragBetaEntry() {
						Beta = row.ParseDouble(Fields.Beta),
						DeltaCdA = row.ParseDouble(Fields.DeltaCdxA).SI<SquareMeter>()
					}).ToList();
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.Beta) && columns.Contains(Fields.DeltaCdxA);
		}


		public void SetDataBus(IDataBus dataBus)
		{
			DataBus = dataBus;
		}

		public Watt AverageAirDragPowerLoss(MeterPerSecond v1, MeterPerSecond v2, Second dt)
		{
			if (DataBus == null) {
				throw new VectoException("Databus is not set - can't access vAir, beta!");
			}
			var vAir = DataBus.CycleData.LeftSample.AirSpeedRelativeToVehicle;
			var beta = DataBus.CycleData.LeftSample.WindYawAngle;

			var airDragForce = (AirDragArea + DeltaCdA(beta)) * Physics.AirDensity / 2.0 * vAir * vAir;
			var vAverage = (v1 + v2) / 2;
			if (v1.IsEqual(v2)) {
				return (airDragForce * vAverage).Cast<Watt>();
			}
			var acceleration = (v2 - v1) / dt;
			return (airDragForce * (v2 * v2 - v1 * v1) / (2 * acceleration * dt)).Cast<Watt>();
		}

		protected SquareMeter DeltaCdA(double beta)
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

		protected class AirDragBetaEntry
		{
			public double Beta;
			public SquareMeter DeltaCdA;
		}

		private static class Fields
		{
			public const string Beta = "Beta";

			public const string DeltaCdxA = "Delta CdA";
		}
	}
}