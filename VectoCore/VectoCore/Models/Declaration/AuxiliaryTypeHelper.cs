using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.Models.Declaration
{
	public static class AuxiliaryTypeHelper
	{
		private static readonly Dictionary<AuxiliaryType, string> AuxToStr = new Dictionary<AuxiliaryType, string> {
			{ AuxiliaryType.Fan, Constants.Auxiliaries.Names.Fan },
			{ AuxiliaryType.SteeringPump, Constants.Auxiliaries.Names.SteeringPump },
			{ AuxiliaryType.HVAC, Constants.Auxiliaries.Names.HeatingVentilationAirCondition },
			{ AuxiliaryType.PneumaticSystem, Constants.Auxiliaries.Names.PneumaticSystem },
			{ AuxiliaryType.ElectricSystem, Constants.Auxiliaries.Names.ElectricSystem },
		};

		private static readonly Dictionary<AuxiliaryType, string> AuxToKey = new Dictionary<AuxiliaryType, string> {
			{ AuxiliaryType.Fan, Constants.Auxiliaries.IDs.Fan },
			{ AuxiliaryType.SteeringPump, Constants.Auxiliaries.IDs.SteeringPump },
			{ AuxiliaryType.HVAC, Constants.Auxiliaries.IDs.HeatingVentilationAirCondition },
			{ AuxiliaryType.PneumaticSystem, Constants.Auxiliaries.IDs.PneumaticSystem },
			{ AuxiliaryType.ElectricSystem, Constants.Auxiliaries.IDs.ElectricSystem },
		};

		private static readonly Dictionary<string, AuxiliaryType> StrToAux = AuxToStr.ToDictionary(kv => kv.Value,
			kv => kv.Key);

		public static AuxiliaryType Parse(string s)
		{
			AuxiliaryType aux;
			return StrToAux.TryGetValue(s, out aux) ? aux : AuxiliaryType.Fan;
		}

		public static AuxiliaryType ParseKey(string s)
		{
			return AuxToKey.FirstOrDefault(x => x.Value.Equals(s, StringComparison.InvariantCultureIgnoreCase)).Key;
		}

		public static string ToString(AuxiliaryType t)
		{
			return AuxToStr[t];
		}

		public static string GetAuxKey(AuxiliaryType t)
		{
			return AuxToKey[t];
		}

		public static string Key(this AuxiliaryType t)
		{
			return AuxToKey[t];
		}

		public static string Name(this AuxiliaryType t)
		{
			return AuxToStr[t];
		}
	}
}