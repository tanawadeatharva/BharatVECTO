using System;
using System.Text.RegularExpressions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.Utils
{
	public static class AuxiliaryTypeHelper
	{
		public static AuxiliaryType Parse(string s)
		{
			
			switch (Regex.Replace(s, @"\s+", "")){
				case Constants.Auxiliaries.Names.Fan:
					return AuxiliaryType.Fan;
				case Constants.Auxiliaries.Names.SteeringPump:
					return AuxiliaryType.SteeringPump;
				case Constants.Auxiliaries.Names.HeatingVentilationAirCondition:
					return AuxiliaryType.HVAC;
				case Constants.Auxiliaries.Names.ElectricSystem:
					return AuxiliaryType.ElectricSystem;
				case Constants.Auxiliaries.Names.PneumaticSystem:
					return AuxiliaryType.PneumaticSystem;
				default:
					throw new ArgumentOutOfRangeException("s", s, "Could not parse auxiliary type string.");
			}
		}

		public static string ToString(AuxiliaryType t)
		{
			switch (t) {
				case AuxiliaryType.Fan:
					return Constants.Auxiliaries.Names.Fan;
				case AuxiliaryType.SteeringPump:
					return Constants.Auxiliaries.Names.SteeringPump;
				case AuxiliaryType.HVAC:
					return Constants.Auxiliaries.Names.HeatingVentilationAirCondition;
				case AuxiliaryType.PneumaticSystem:
					return Constants.Auxiliaries.Names.PneumaticSystem;
				case AuxiliaryType.ElectricSystem:
					return Constants.Auxiliaries.Names.ElectricSystem;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}