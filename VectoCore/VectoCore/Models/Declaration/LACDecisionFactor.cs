using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	/// <summary>
	/// Class for Look Ahead Coasting Decision Factor (DF_coast)
	/// </summary>
	public sealed class LACDecisionFactor : LookupData<MeterPerSecond, MeterPerSecond, double>
	{
		private readonly LACDecisionFactorVTarget _vTarget = new LACDecisionFactorVTarget();
		private readonly LACDecisionFactorVdrop _vDrop = new LACDecisionFactorVdrop();

		public override double Lookup(MeterPerSecond targetVelocity, MeterPerSecond velocityDrop)
		{
			// normalize values inverse from [0 .. 1] to [2.5 .. 1]
			return 2.5 - 1.5 * _vTarget.Lookup(targetVelocity) * _vDrop.Lookup(velocityDrop);
		}

		protected override void ParseData(DataTable table) {}

		private sealed class LACDecisionFactorVdrop : LookupData<MeterPerSecond, double>
		{
			private const string ResourceId = "TUGraz.VectoCore.Resources.Declaration.LAC-DF-Vdrop.csv";

			public LACDecisionFactorVdrop()
			{
				ParseData(ReadCsvResource(ResourceId));
			}

			public override double Lookup(MeterPerSecond targetVelocity)
			{
				var section = Data.GetSection(kv => kv.Key < targetVelocity);
				return VectoMath.Interpolate(section.Item1.Key, section.Item2.Key, section.Item1.Value, section.Item2.Value,
					targetVelocity);
			}

			protected override void ParseData(DataTable table)
			{
				if (table.Columns.Count < 2) {
					throw new VectoException("LAC Decision Factor File for Vdrop must consist of at least 2 columns.");
				}

				if (table.Rows.Count < 2) {
					throw new VectoException(
						"LAC Decision Factor File for Vdrop must consist of at least two lines with numeric values (below file header)");
				}

				if (table.Columns.Contains("v_target") && table.Columns.Contains("decision factor")) {
					Data = table.Rows.Cast<DataRow>()
						.ToDictionary(r => r.ParseDouble("v_drop").KMPHtoMeterPerSecond(), r => r.ParseDouble("decision factor"));
				} else {
					Data = table.Rows.Cast<DataRow>()
						.ToDictionary(r => r.ParseDouble(0).KMPHtoMeterPerSecond(), r => r.ParseDouble(1));
				}
			}
		}

		private sealed class LACDecisionFactorVTarget : LookupData<MeterPerSecond, double>
		{
			private const string ResourceId = "TUGraz.VectoCore.Resources.Declaration.LAC-DF-Vtarget.csv";

			public LACDecisionFactorVTarget()
			{
				ParseData(ReadCsvResource(ResourceId));
			}

			public override double Lookup(MeterPerSecond targetVelocity)
			{
				var section = Data.GetSection(kv => kv.Key < targetVelocity);
				return VectoMath.Interpolate(section.Item1.Key, section.Item2.Key, section.Item1.Value, section.Item2.Value,
					targetVelocity);
			}

			protected override void ParseData(DataTable table)
			{
				if (table.Columns.Count < 2) {
					throw new VectoException("LAC Decision Factor File for Vtarget must consist of at least 2 columns.");
				}

				if (table.Rows.Count < 2) {
					throw new VectoException(
						"LAC Decision Factor File for Vtarget must consist of at least two lines with numeric values (below file header)");
				}

				if (table.Columns.Contains("v_target") && table.Columns.Contains("decision factor")) {
					Data = table.Rows.Cast<DataRow>()
						.ToDictionary(r => r.ParseDouble("v_target").KMPHtoMeterPerSecond(), r => r.ParseDouble("decision_factor"));
				} else {
					Data = table.Rows.Cast<DataRow>()
						.ToDictionary(r => r.ParseDouble(0).KMPHtoMeterPerSecond(), r => r.ParseDouble(1));
				}
			}
		}
	}
}