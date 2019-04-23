using System;
using System.Collections.Generic;
using System.Data;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class ATShiftStrategyVoith : ATShiftStrategy
	{
		public const double DownhillSlope = -5;
		public const double UphillSlope = 5;

		protected ShiftStrategyParameters shiftParameters;

		internal Dictionary<int, ShiftLineSet> UpshiftLines = new Dictionary<int, ShiftLineSet>();
		internal Dictionary<int, ShiftLineSet> DownshiftLines = new Dictionary<int, ShiftLineSet>();

		public ATShiftStrategyVoith(VectoRunData data, IDataBus dataBus) : base(data, dataBus)
		{
			shiftParameters = data.GearshiftParameters;
			InitializeShiftLines(shiftParameters.GearshiftLines);
		}

		private void InitializeShiftLines(TableData lines)
		{
			var slopeDh = VectoMath.InclinationToAngle(DownhillSlope / 100.0);
			var slopeLevel = VectoMath.InclinationToAngle(0);
			var slopeUh = VectoMath.InclinationToAngle(UphillSlope / 100.0);

			foreach (DataRow row in lines.Rows) {
				var shift = row[ShiftLinesColumns.Shift].ToString().Split('-');
				var g1 = shift[0].ToInt();
				var g2 = shift[1].ToInt();
				var upshift = true;
				if (g1 + 1 == g2) {
					// upshift
					upshift = true;
				} else if (g2 + 1 == g1) {
					// downshift
					upshift = false;
				} else {
					throw new VectoException("invalid shift entry: {0}-{1}", g1, g2);
				}

				var loadStage = row[ShiftLinesColumns.LoadStage].ToString().ToInt();

				var nDhAmaxLower = row.Field<string>(ShiftLinesColumns.nDhAmaxLower).ToDouble().RPMtoRad();
				var nLevelAmaxLower = row.Field<string>(ShiftLinesColumns.nLevelAmaxLower).ToDouble()
					.RPMtoRad();
				var nUhAmaxLower = row.Field<string>( ShiftLinesColumns.nUhAmaxLower).ToDouble().RPMtoRad();


				var nDhAminLower = GetAlternativeIfEmpty(row, ShiftLinesColumns.nDhAminLower, ShiftLinesColumns.nDhAmaxLower).RPMtoRad();
				var nLevelAminLower = GetAlternativeIfEmpty(row, ShiftLinesColumns.nLevelAminLower, ShiftLinesColumns.nLevelAmaxLower).RPMtoRad();
				var nUhAminLower = GetAlternativeIfEmpty(row, ShiftLinesColumns.nUhAminLower, ShiftLinesColumns.nUhAmaxLower).RPMtoRad();


				var nDhAminUpper = GetAlternativeIfEmpty(row, ShiftLinesColumns.nDhAminUpper, ShiftLinesColumns.nDhAminLower, ShiftLinesColumns.nDhAmaxLower)
					.RPMtoRad();
				var nLevelAminUpper = GetAlternativeIfEmpty(
					row, ShiftLinesColumns.nLevelAminUpper, ShiftLinesColumns.nLevelAminLower, ShiftLinesColumns.nLevelAmaxLower).RPMtoRad();
				var nUhAminUpper = GetAlternativeIfEmpty(row, ShiftLinesColumns.nUhAminUpper, ShiftLinesColumns.nUhAminLower, ShiftLinesColumns.nUhAmaxLower)
					.RPMtoRad();

				var nDhAmaxUpper = GetAlternativeIfEmpty(row, ShiftLinesColumns.nDhAmaxUpper, ShiftLinesColumns.nDhAmaxLower).RPMtoRad();
				var nLevelAmaxUpper = GetAlternativeIfEmpty(row, ShiftLinesColumns.nLevelAmaxUpper, ShiftLinesColumns.nLevelAmaxLower)
					.RPMtoRad();
				var nUhAmaxUpper = GetAlternativeIfEmpty(row, ShiftLinesColumns.nUhAmaxUpper, ShiftLinesColumns.nUhAmaxLower).RPMtoRad();

				if (upshift) {
					if (!UpshiftLines.ContainsKey(g1)) {
						UpshiftLines[g1] = new ShiftLineSet();
					}
					var shiftLineSet = UpshiftLines[g1];
					if (shiftLineSet.LoadStages.ContainsKey(loadStage)) {
						throw new VectoException(
							"Gearshift entries for upshift {0}-{1} load stage {2} already defined!", g1, g2, loadStage);
					}

					var entry = new ShiftLines();

					entry.LowerBound.entriesAMin.Add(Tuple.Create(slopeDh, nDhAminLower));
					entry.LowerBound.entriesAMin.Add(Tuple.Create(slopeLevel, nLevelAminLower));
					entry.LowerBound.entriesAMin.Add(Tuple.Create(slopeUh, nUhAminLower));

					entry.LowerBound.entriesAMax.Add(Tuple.Create(slopeDh, nDhAmaxLower));
					entry.LowerBound.entriesAMax.Add(Tuple.Create(slopeLevel, nLevelAmaxLower));
					entry.LowerBound.entriesAMax.Add(Tuple.Create(slopeUh, nUhAmaxLower));

					entry.UpperBound.entriesAMin.Add(Tuple.Create(slopeDh, nDhAminUpper));
					entry.UpperBound.entriesAMin.Add(Tuple.Create(slopeLevel, nLevelAminUpper));
					entry.UpperBound.entriesAMin.Add(Tuple.Create(slopeUh, nUhAminUpper));

					entry.UpperBound.entriesAMax.Add(Tuple.Create(slopeDh, nDhAmaxUpper));
					entry.UpperBound.entriesAMax.Add(Tuple.Create(slopeLevel, nLevelAmaxUpper));
					entry.UpperBound.entriesAMax.Add(Tuple.Create(slopeUh, nUhAmaxUpper));

					shiftLineSet.LoadStages[loadStage] = entry;
				} else {
					if (!DownshiftLines.ContainsKey(g1)) {
						DownshiftLines[g1] = new ShiftLineSet();
					}
					var shiftLineSet = DownshiftLines[g1];
					if (shiftLineSet.LoadStages.ContainsKey(loadStage)) {
						throw new VectoException(
							"Gearshift entries for downshift {0}-{1} load stage {2} already defined!", g1, g2, loadStage);
					}

					var entry = new ShiftLines();

					entry.LowerBound.entriesAMin.Add(Tuple.Create(slopeDh, nDhAminLower));
					entry.LowerBound.entriesAMin.Add(Tuple.Create(slopeLevel, nLevelAminLower));
					entry.LowerBound.entriesAMin.Add(Tuple.Create(slopeUh, nUhAminLower));

					entry.LowerBound.entriesAMax.Add(Tuple.Create(slopeDh, nDhAmaxLower));
					entry.LowerBound.entriesAMax.Add(Tuple.Create(slopeLevel, nLevelAmaxLower));
					entry.LowerBound.entriesAMax.Add(Tuple.Create(slopeUh, nUhAmaxLower));

					entry.UpperBound.entriesAMin.Add(Tuple.Create(slopeDh, nDhAminUpper));
					entry.UpperBound.entriesAMin.Add(Tuple.Create(slopeLevel, nLevelAminUpper));
					entry.UpperBound.entriesAMin.Add(Tuple.Create(slopeUh, nUhAminUpper));

					entry.UpperBound.entriesAMax.Add(Tuple.Create(slopeDh, nDhAmaxUpper));
					entry.UpperBound.entriesAMax.Add(Tuple.Create(slopeLevel, nLevelAmaxUpper));
					entry.UpperBound.entriesAMax.Add(Tuple.Create(slopeUh, nUhAmaxUpper));

					shiftLineSet.LoadStages[loadStage] = entry;
				}
			}
		}

		private double GetAlternativeIfEmpty(DataRow row, string col1, string col2, string col3)
		{
			return string.IsNullOrWhiteSpace(row[col1].ToString())
				? (string.IsNullOrWhiteSpace(row[col2].ToString())
					? row.Field<string>(col3).ToDouble()
					: row.Field<string>(col2).ToDouble())
				: row.Field<string>(col1).ToDouble();
		}

		private double GetAlternativeIfEmpty(DataRow row, string col1, string col2)
		{
			return string.IsNullOrWhiteSpace(row[col1].ToString())
				? row.Field<string>(col2).ToDouble()
				: row.Field<string>(col1).ToDouble();
		}

		public new static string Name
		{
			get { return "AT shift strategy Voith proposal"; }
		}


		#region Overrides of ATShiftStrategy

		public override bool ShiftRequired(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			return base.ShiftRequired(
				absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, lastShiftTime);
		}


		protected override bool CheckUpshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			return base.CheckUpshift(
				absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, lastShiftTime);
		}


		protected override bool CheckDownshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			return base.CheckDownshift(
				absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, lastShiftTime);
		}

		#endregion

		protected class ShiftLinesColumns
		{
			public const string Shift = "shift";
			public const string LoadStage = "loadstage";

			public const string nDhAminLower = "n_dh_amin_lower";
			public const string nLevelAminLower = "n_level_amin_lower";
			public const string nUhAminLower = "n_uh_amin_lower";

			public const string nDhAmaxLower = "n_dh_amax_lower";
			public const string nLevelAmaxLower = "n_level_amax_lower";
			public const string nUhAmaxLower = "n_uh_amax_lower";

			public const string nDhAminUpper = "n_dh_amin_upper";
			public const string nLevelAminUpper = "n_level_amin_upper";
			public const string nUhAminUpper = "n_uh_amin_upper";

			public const string nDhAmaxUpper = "n_dh_amax_upper";
			public const string nLevelAmaxUpper = "n_level_amax_upper";
			public const string nUhAmaxUpper = "n_uh_amax_upper";
		}
	}

	internal class ShiftLineSet
	{
		public Dictionary<int, ShiftLines> LoadStages = new Dictionary<int, ShiftLines>();

		public PerSecond LookupShiftSpeed(
			int loadStage, Radian gradient, MeterPerSquareSecond acceleration, MeterPerSquareSecond aMin,
			MeterPerSquareSecond aMax)
		{
			if (!LoadStages.ContainsKey(loadStage)) {
				throw new VectoException("No Shiftlines for load stage {0} found", loadStage);
			}

			var shiftLinesSet = LoadStages[loadStage];

			//var slope = (Math.Tan(gradient.Value()) * 100).LimitTo(
			//	ATShiftStrategyVoith.DownhillSlope, ATShiftStrategyVoith.UphillSlope);

			gradient = gradient.LimitTo(
				VectoMath.InclinationToAngle(ATShiftStrategyVoith.DownhillSlope),
				VectoMath.InclinationToAngle(ATShiftStrategyVoith.UphillSlope));
			var shiftSpeedsLower = shiftLinesSet.LowerBound.LookupShiftSpeed(gradient);
			var shiftSpeedsUpper = shiftLinesSet.UpperBound.LookupShiftSpeed(gradient);
			var acc = acceleration.LimitTo(aMin, aMax);

			var shiftSpeed1 = VectoMath.Interpolate(aMin, aMax, shiftSpeedsLower.ShiftSpeedAMin, shiftSpeedsLower.ShiftSpeedAMax, acc);
			var shiftSpeed2 = VectoMath.Interpolate(aMin, aMax, shiftSpeedsUpper.ShiftSpeedAMin, shiftSpeedsUpper.ShiftSpeedAMax, acc);

			return (shiftSpeed1 + shiftSpeed2) / 2.0;
		}
	}

	internal class ShiftLines
	{
		public readonly ShiftSpeeds LowerBound = new ShiftSpeeds();
		public readonly ShiftSpeeds UpperBound = new ShiftSpeeds();
	}

	internal class ShiftSpeeds
	{
		//private Tuple<Radian, Tuple<PerSecond, PerSecond>>[] entries;

		internal readonly List<Tuple<Radian, PerSecond>> entriesAMin = new List<Tuple<Radian, PerSecond>>();
		internal readonly List<Tuple<Radian, PerSecond>> entriesAMax = new List<Tuple<Radian, PerSecond>>();


		public ShiftSpeedTuple LookupShiftSpeed(Radian gradent)
		{
			var sectLow = entriesAMin.GetSection(x => x.Item1 < gradent);
			var sectHigh = entriesAMax.GetSection(x => x.Item1 < gradent);

			return new ShiftSpeedTuple(
				VectoMath.Interpolate(sectLow.Item1.Item1, sectLow.Item2.Item1, sectLow.Item1.Item2, sectLow.Item2.Item2, gradent),
				VectoMath.Interpolate(
					sectHigh.Item1.Item1, sectHigh.Item2.Item1, sectHigh.Item1.Item2, sectHigh.Item2.Item2, gradent));
		}
	}

	internal class ShiftSpeedTuple
	{
		public  PerSecond ShiftSpeedAMin { get; } 
		public  PerSecond ShiftSpeedAMax { get; }

		public ShiftSpeedTuple(PerSecond shiftSpeedAMin, PerSecond shiftSpeedAMax)
		{
			ShiftSpeedAMin = shiftSpeedAMin;
			ShiftSpeedAMax = shiftSpeedAMax;
		}

	}
}
