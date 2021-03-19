using TUGraz.VectoCommon.BusAuxiliaries;

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

		public static string GetLabel(this VehicleCode? self)
		{
			return self.ToString();
		}

		public static string ToXMLFormat(this VehicleCode? self)
		{
			return self.ToString();
		}

		public static bool IsDoubleDeckerBus(this VehicleCode? self)
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

		public static FloorType GetFloorType(this VehicleCode? vehicleCode)
		{
			switch (vehicleCode) {
				case VehicleCode.CA:
				case VehicleCode.CB:
				case VehicleCode.CC:
				case VehicleCode.CD:
					return FloorType.HighFloor;
				default:
					return FloorType.LowFloor;
			}
		}

		public static bool IsOpenDeckBus(this VehicleCode self)
		{
			switch (self) {
				case VehicleCode.CI:
				case VehicleCode.CJ:
					return true;
				default:
					return false;
			}
		}
	}

}