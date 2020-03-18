using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.InputData {
	public enum PowertrainPosition
	{
		HybridP0,
		HybridP1,
		HybridP2,
		HybridP3,
		HybridP4
	}

	public static class PowertrainPositionHelper
	{
		public const string Prefix = "Hybrid";

		public static PowertrainPosition Parse(string pos)
		{
			return (Prefix + pos).ParseEnum<PowertrainPosition>();
		}
	}
}