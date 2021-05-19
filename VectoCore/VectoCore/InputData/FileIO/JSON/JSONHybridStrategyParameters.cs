using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONHybridStrategyParameters : JSONFile, IHybridStrategyParameters
	{
		public JSONHybridStrategyParameters(JObject json, string filename, bool tolerateMissing) : base(json, filename, tolerateMissing)
		{ }

		public double EquivalenceFactorDischarge
		{
			get
			{
				return Body["EquivalenceFactor"] == null ? Body.GetEx<double>("EquivalenceFactorDischarge") :
					Body.GetEx<double>("EquivalenceFactor");
			}
		}

		public double EquivalenceFactorCharge {
			get {
				return Body["EquivalenceFactor"] == null ? Body.GetEx<double>("EquivalenceFactorCharge") :
					Body.GetEx<double>("EquivalenceFactor");
			}
		}

		public double MinSoC
		{
			get
			{
				return Body.GetEx<double>("MinSoC") / 100.0;
			}
		}

		public double MaxSoC
		{
			get
			{
				return Body.GetEx<double>("MaxSoC") / 100.0;
			}
		}

		public double TargetSoC
		{
			get
			{
				return Body.GetEx<double>("TargetSoC") / 100.0;
			}
		}

		public Second MinimumICEOnTime
		{
			get { return Body.GetEx<double>("MinICEOnTime").SI<Second>(); }
		}

		public Second AuxBufferTime
		{
			get { return Body.GetEx<double>("AuxBufferTime").SI<Second>(); }
		}

		public Second AuxBufferChargeTime
		{
			get { return Body.GetEx<double>("AuxBufferChgTime").SI<Second>(); }
		}

		public double ICEStartPenaltyFactor
		{
			get
			{
				return Body["ICEStartPenaltyFactor"] == null ? 0 : Body.GetEx<double>("ICEStartPenaltyFactor");
			}
		}

		public double CostFactorSOCExpponent
		{
			get
			{
				return Body["CostFactorSOCExponent"] == null ? double.NaN : Body.GetEx<double>("CostFactorSOCExponent");
			}
		}
	}
}