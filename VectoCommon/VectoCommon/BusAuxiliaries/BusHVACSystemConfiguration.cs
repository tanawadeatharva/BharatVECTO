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

		public static string GetLabel(this BusHVACSystemConfiguration havacConfiguration)
		{
			switch (havacConfiguration) {

				case BusHVACSystemConfiguration.Configuration1:
					return $"{Prefix} 1";
				case BusHVACSystemConfiguration.Configuration2:
					return $"{Prefix} 2";
				case BusHVACSystemConfiguration.Configuration3:
					return $"{Prefix} 3";
				case BusHVACSystemConfiguration.Configuration4:
					return $"{Prefix} 4";
				case BusHVACSystemConfiguration.Configuration5:
					return $"{Prefix} 5";
				case BusHVACSystemConfiguration.Configuration6:
					return $"{Prefix} 6";
				case BusHVACSystemConfiguration.Configuration7:
					return $"{Prefix} 7";
				case BusHVACSystemConfiguration.Configuration8:
					return $"{Prefix} 8";
				case BusHVACSystemConfiguration.Configuration9:
					return $"{Prefix} 9";
				default: 
					return BusHVACSystemConfiguration.Unknown.ToString();
			}
		}

		public static string GetXmlFormat(this BusHVACSystemConfiguration havacConfiguration)
		{
			switch (havacConfiguration)
			{
				case BusHVACSystemConfiguration.Configuration1:
					return "1";
				case BusHVACSystemConfiguration.Configuration2:
					return "2";
				case BusHVACSystemConfiguration.Configuration3:
					return "3";
				case BusHVACSystemConfiguration.Configuration4:
					return "4";
				case BusHVACSystemConfiguration.Configuration5:
					return "5";
				case BusHVACSystemConfiguration.Configuration6:
					return "6";
				case BusHVACSystemConfiguration.Configuration7:
					return "7";
				case BusHVACSystemConfiguration.Configuration8:
					return "8";
				case BusHVACSystemConfiguration.Configuration9:
					return "9";
				default:
					return "0";
			}
		}

	}
}