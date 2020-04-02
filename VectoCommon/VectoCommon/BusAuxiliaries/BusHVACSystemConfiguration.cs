using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries {
	public enum BusHVACSystemConfiguration
	{
		Unknown,
		Configuration1,
		Configuration2,
		Configuration3,
		Configuration4,
		Configuration5,
		Configuration6,
		Configuration7,
		Configuration8,
		Configuration9,
	}

	public static class BusHVACSystemConfigurationHelper
	{
		private const string Prefix = "Configuration";

		public static BusHVACSystemConfiguration Parse(string text)
		{
			return (Prefix + text).ParseEnum<BusHVACSystemConfiguration>();
		}

		public static string GetName(this BusHVACSystemConfiguration auxCfg)
		{
			if (auxCfg == BusHVACSystemConfiguration.Unknown) {
				return "unknonwn";
			}

			return auxCfg.ToString().Replace(Prefix, "");
		}
	}
}