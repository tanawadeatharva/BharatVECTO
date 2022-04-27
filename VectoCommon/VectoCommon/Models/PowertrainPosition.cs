using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.InputData {
	public enum PowertrainPosition
	{
		HybridPositionNotSet, // this has to be the first entrie so that it is used as default for not initialized fields!
		HybridP0,
		HybridP1,
		HybridP2,
		HybridP2_5,
		HybridP3,
		HybridP4,

		GEN,

		BatteryElectricE4,
		BatteryElectricE3,
		BatteryElectricE2,
	}

	public static class PowertrainPositionHelper
	{
		public const string HybridPrefix = "Hybrid";
		public const string BatteryElectriPrefix = "BatteryElectric";

		public static PowertrainPosition Parse(string pos)
		{
			if (pos.EndsWith(nameof(PowertrainPosition.GEN))) {
				return PowertrainPosition.GEN;
			}

			if (pos.StartsWith("P",StringComparison.InvariantCultureIgnoreCase)) {
				return (HybridPrefix + pos).Replace(".", "_").ParseEnum<PowertrainPosition>();
			}

			if (pos.StartsWith("B", StringComparison.InvariantCultureIgnoreCase)) {
				return (BatteryElectriPrefix + pos.Replace("B", "E")).ParseEnum<PowertrainPosition>();
			}
			if (pos.StartsWith("E", StringComparison.InvariantCultureIgnoreCase)) {
				return (BatteryElectriPrefix + pos).ParseEnum<PowertrainPosition>();
			}
			throw new VectoException("invalid powertrain position {0}", pos);
		}

		public static string GetName(this PowertrainPosition pos)
		{
			return pos.ToString().Replace(HybridPrefix, "").Replace(BatteryElectriPrefix, "").Replace("_", ".");
		}

		public static string GetLabel(this PowertrainPosition pos)
		{
			switch (pos) {
				case PowertrainPosition.HybridP0:
				case PowertrainPosition.HybridP1:
				case PowertrainPosition.HybridP2:
				case PowertrainPosition.HybridP3:
				case PowertrainPosition.HybridP4:
					return pos.ToString().Replace(HybridPrefix, "");
				case PowertrainPosition.HybridP2_5:
					return pos.ToString().Replace(HybridPrefix, "").Replace("_", ".");
				case PowertrainPosition.GEN:
					return nameof(PowertrainPosition.GEN);
			}
			return pos.ToString().Replace(BatteryElectriPrefix, "").Replace("B", "E");
		}

		public static bool IsBatteryElectric(this PowertrainPosition pos)
		{
			return pos == PowertrainPosition.BatteryElectricE2 || pos == PowertrainPosition.BatteryElectricE3 ||
					pos == PowertrainPosition.BatteryElectricE4;
		}

		public static bool IsParallelHybrid(this PowertrainPosition pos)
		{
			switch (pos) {
				case PowertrainPosition.HybridP0:
				case PowertrainPosition.HybridP1:
				case PowertrainPosition.HybridP2:
				case PowertrainPosition.HybridP2_5:
				case PowertrainPosition.HybridP3:
				case PowertrainPosition.HybridP4:
					return true;
				default:
					return false;
			}
		}

		public static string ToXmlFormat(this PowertrainPosition pos)
		{
			switch (pos) {
				case PowertrainPosition.HybridPositionNotSet:
					throw new ArgumentException("Hybrid position not set");
				case PowertrainPosition.HybridP0: 
				case PowertrainPosition.HybridP1:
				case PowertrainPosition.HybridP2:
				case PowertrainPosition.HybridP2_5:
				case PowertrainPosition.HybridP3:
				case PowertrainPosition.HybridP4:
				case PowertrainPosition.BatteryElectricE4:
				case PowertrainPosition.BatteryElectricE3:
				case PowertrainPosition.BatteryElectricE2:
					return GetPositionWithoutPrefix(pos.ToString());
				case PowertrainPosition.GEN:
					return "GEN";
				default:
					throw new ArgumentOutOfRangeException(nameof(pos), pos, null);
			}
		}

		private static string GetPositionWithoutPrefix(string position)
		{
			return position.Replace(BatteryElectriPrefix, "").Replace("E", "").Replace(HybridPrefix, "").Replace("P", "")
				.Replace("_", ".");
		}
		public static bool IsSerialHybrid(this PowertrainPosition pos)
		{
			return IsBatteryElectric(pos);
		}
	}
}