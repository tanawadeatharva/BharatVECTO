using System.Text.RegularExpressions;
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

		public static string GetLabel(this BusHVACSystemConfiguration hvacConfiguration)
		{
			if (hvacConfiguration == BusHVACSystemConfiguration.Unknown) { 
					return BusHVACSystemConfiguration.Unknown.ToString();
			}

			return Prefix + " " + hvacConfiguration.ToString().Replace(Prefix, "");
		}

		public static string GetName(this BusHVACSystemConfiguration hvacConfig)
		{
			if (hvacConfig == BusHVACSystemConfiguration.Unknown) {
				return "Unknown";
			}

			return hvacConfig.ToString().Replace(Prefix, "");
		}

		public static string GetXmlFormat(this BusHVACSystemConfiguration hvacConfiguration)
		{
			if (hvacConfiguration == BusHVACSystemConfiguration.Unknown) {
				return "0";
			}

			return GetName(hvacConfiguration);
		}

	}
}