using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.InputData {
	public enum PowertrainPosition
	{
		HybridPositionNotSet, // this has to be the first entrie so that it is used as default for not initialized fields!
		HybridP0,
		HybridP1,
		HybridP2,
		HybridP3,
		HybridP4,

		BatteryElectricB4,
		BatteryElectricB3,
		BatteryElectricB2,
	}

	public static class PowertrainPositionHelper
	{
		public const string Prefix = "Hybrid";

		public static PowertrainPosition Parse(string pos)
		{
			return (Prefix + pos).ParseEnum<PowertrainPosition>();
		}

		public static string GetName(this PowertrainPosition pos)
		{
			return pos.ToString().Replace(Prefix, "");
		}
	}
}