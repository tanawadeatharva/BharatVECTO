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

		private static readonly Dictionary<string, AuxiliaryType> StrToAux = AuxToStr.ToDictionary(kv => kv.Value,
			kv => kv.Key);

		public static AuxiliaryType Parse(string s)
		{
			AuxiliaryType aux;
			return StrToAux.TryGetValue(s, out aux) ? aux : AuxiliaryType.Fan;
		}

		public static string ToString(AuxiliaryType t)
		{
			return AuxToStr[t];
		}
	}
}