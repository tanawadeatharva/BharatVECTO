namespace TUGraz.VectoCommon.Models {
	public enum VehicleCode
	{
		NOT_APPLICABLE,
		CA,
		CB, 
		CC,
		CD,
		CE,
		CF,
		CG,
		CH,
		CI,
		CJ

	}

	public static class VehicleCodeHelper
	{

		public static VehicleCode Parse(string vehicleCode)
		{
			switch (vehicleCode)
			{
				case "CA":
					return VehicleCode.CA;
				case "CB":
					return VehicleCode.CB;
				case "CC":
					return VehicleCode.CC;
				case "CD":
					return VehicleCode.CD;
				case "CE":
					return VehicleCode.CE;
				case "CF":
					return VehicleCode.CF;
				case "CG":
					return VehicleCode.CG;
				case "CH":
					return VehicleCode.CH;
				case "CI":
					return VehicleCode.CI;
				case "CJ":
					return VehicleCode.CJ;
				default:
					return VehicleCode.NOT_APPLICABLE;
			}
		}



		public static string GetLabel(this VehicleCode self)
		{
			return self.ToString();
		}

		public static string ToXMLFormat(this VehicleCode self)
		{
			return self.ToString();
		}

		public static bool IsDoubleDeckBus(this VehicleCode self)
		{
			switch (self) {
				case VehicleCode.CF:
				case VehicleCode.CJ:
				case VehicleCode.CB:
				case VehicleCode.CH:
				case VehicleCode.CD:
					return true;
				case VehicleCode.CE:
				case VehicleCode.CI:
				case VehicleCode.CA:
				case VehicleCode.CG:
				case VehicleCode.CC:
					return false;
				default:
					return false;
			}
		}
	}

}