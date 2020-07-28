using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONHybridStrategyParameters : JSONFile, IHybridStrategyParameters
	{
		public JSONHybridStrategyParameters(JObject json, string filename, bool tolerateMissing) : base(json, filename, tolerateMissing)
		{ }

		public double EquivalenceFactor
		{
			get
			{
				return Body.GetEx<double>("EquivalenceFactor");
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
	}
}