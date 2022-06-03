using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.InputData
{
	public enum PowertrainPosition
	{
		HybridPositionNotSet, // this has to be the first entry so that it is used as default for not initialized fields!
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

		IEPC,
		IHPC
	}

	public static class PowertrainPositionHelper
	{
		public const string HybridPrefix = "Hybrid";
		public const string BatteryElectricPrefix = "BatteryElectric";

		public static PowertrainPosition Parse(string pos)
		{
			if (pos.Equals(nameof(PowertrainPosition.GEN))) {
				return PowertrainPosition.GEN;
			}

			if (pos.StartsWith("P", StringComparison.InvariantCultureIgnoreCase)) {
				return (HybridPrefix + pos).Replace(".", "_").ParseEnum<PowertrainPosition>();
			}

			if (pos.StartsWith("B", StringComparison.InvariantCultureIgnoreCase)) {
				return (BatteryElectricPrefix + pos.Replace("B", "E")).ParseEnum<PowertrainPosition>();
			}
			if (pos.StartsWith("E", StringComparison.InvariantCultureIgnoreCase)) {
				return (BatteryElectricPrefix + pos).ParseEnum<PowertrainPosition>();
			}

			if (pos.Equals(nameof(PowertrainPosition.IHPC), StringComparison.InvariantCultureIgnoreCase)) {
				return PowertrainPosition.IHPC;
			}

			throw new VectoException("invalid powertrain position {0}", pos);
		}

		public static string GetName(this PowertrainPosition pos)
		{
			return pos.ToString().Replace(HybridPrefix, "").Replace(BatteryElectricPrefix, "").Replace("_", ".");
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
				case PowertrainPosition.HybridPositionNotSet:
					return "Position not set";
			}
			return pos.ToString().Replace(BatteryElectricPrefix, "").Replace("B", "E");
		}

		public static bool IsBatteryElectric(this PowertrainPosition pos)
		{
			switch (pos) {
				case PowertrainPosition.BatteryElectricE2:
				case PowertrainPosition.BatteryElectricE3:
				case PowertrainPosition.BatteryElectricE4:
				case PowertrainPosition.IEPC:
					return true;
				default:
					return false;
			}
		}

		public static bool IsParallelHybrid(this PowertrainPosition pos)
		{
			switch (pos) {
				//case PowertrainPosition.HybridP0: // special case currently modelled in BusAuxiliary as SmartAlternator.
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

		public static bool IsSerialHybrid(this PowertrainPosition pos)
		{
			return IsBatteryElectric(pos);
		}
	}
}