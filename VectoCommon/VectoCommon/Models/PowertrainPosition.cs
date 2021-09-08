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

		public static PowertrainPosition Parse(string pos, string schemaName)
		{
			return pos == nameof(PowertrainPosition.GEN) ? PowertrainPosition.GEN : Parse(GetPowertrainPositionType(pos, schemaName));
		}

		public static PowertrainPosition Parse(string pos)
		{
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

		private static string GetPowertrainPositionType(string pos, string schemaName)
		{
			switch (schemaName) {
				case "Vehicle_HEV-Px_HeavyLorryDeclarationType":
				case "Vehicle_HEV-Px_MediumLorryDeclarationType":
				case "Vehicle_HEV-Px_PrimaryBusDeclarationType":
				case "Components_HEV-Px_LorryType":
				case "Components_HEV-Px_PrimaryBusType":
					return $"P{pos}";
				case "Vehicle_HEV-Sx_HeavyLorryDeclarationType":
				case "Vehicle_HEV-Sx_MediumLorryDeclarationType":
				case "Vehicle_HEV-Sx_PrimaryBusDeclarationType":
				case "Vehicle_HEV-IEPC-S_HeavyLorryDeclarationType":
				case "Vehicle_HEV-IEPC-S_MediumLorryDeclarationType":
				case "Vehicle_HEV-IEPC-S_PrimaryBusDeclarationType":
				case "Vehicle_PEV_HeavyLorryDeclarationType":
				case "Vehicle_PEV_MediumLorryDeclarationType":
				case "Vehicle_PEV_PrimaryBusDeclarationType":
				case "Components_HEV-S2_LorryType":
				case "Components_HEV-S3_LorryType":
				case "Components_HEV-S4_LorryType":
				case "Components_HEV-S2_PrimaryBusType":
				case "Components_HEV-S3_PrimaryBusType":
				case "Components_HEV-S4_PrimaryBusType":
				case "Components_PEV-E2_LorryType":
				case "Components_PEV-E3_LorryType":
				case "Components_PEV-E4_LorryType":
				case "Components_PEV-E2_PrimaryBusType":
					return $"E{pos}";
				default:
					return null;
			}
		}
	}
}