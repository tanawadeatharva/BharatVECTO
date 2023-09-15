using System;

namespace TUGraz.VectoCommon.Models
{
	public enum IMCTechnology
	{
		NotApplicable,
		None,
		OverheadPantograph,
		OverheadTrolley,
		GroundRail,
		Wireless
	}

	public static class IMCTechnologyHelper
	{
		public static string GetLabel(this IMCTechnology tech)
		{
			switch (tech) {
				case IMCTechnology.NotApplicable:
					return "Not Applicable";
				case IMCTechnology.None:
					return "None";
				case IMCTechnology.OverheadPantograph:
					return "Overhead Pantograph";
				case IMCTechnology.OverheadTrolley:
					return "Overhead Trolley";
				case IMCTechnology.GroundRail:
					return "Ground Rail";
				case IMCTechnology.Wireless:
					return "Wireless";
				default:
					throw new ArgumentOutOfRangeException(nameof(tech), tech, null);
			}
		}
	}
}