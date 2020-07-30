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
		HybridP3,
		HybridP4,

		BatteryElectricB4,
		BatteryElectricB3,
		BatteryElectricB2,
	}

	public static class PowertrainPositionHelper
	{
		public const string HybridPrefix = "Hybrid";
		public const string BatteryElectriPrefix = "BatteryElectric";

		public static PowertrainPosition Parse(string pos)
		{
			if (pos.StartsWith("P",StringComparison.InvariantCultureIgnoreCase)) {
				return (HybridPrefix + pos).ParseEnum<PowertrainPosition>();
			}

			if (pos.StartsWith("B", StringComparison.InvariantCultureIgnoreCase)) {
				return (BatteryElectriPrefix + pos).ParseEnum<PowertrainPosition>();
			}
			throw new VectoException("invalid powertrain position {0}", pos);
		}

		public static string GetName(this PowertrainPosition pos)
		{
			return pos.ToString().Replace(HybridPrefix, "").Replace(BatteryElectriPrefix, "");
		}

		public static bool IsBatteryElectric(this PowertrainPosition pos)
		{
			return pos == PowertrainPosition.BatteryElectricB2 || pos == PowertrainPosition.BatteryElectricB3 ||
					pos == PowertrainPosition.BatteryElectricB4;
		}

		public static bool IsParallelHybrid(this PowertrainPosition pos)
		{
			switch (pos) {
				case PowertrainPosition.HybridP0:
				case PowertrainPosition.HybridP1:
				case PowertrainPosition.HybridP2:
				case PowertrainPosition.HybridP3:
				case PowertrainPosition.HybridP4:
					return true;
				default:
					return false;
			}
		}
	}
}