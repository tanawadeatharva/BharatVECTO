using System;

namespace TUGraz.VectoCommon.BusAuxiliaries {
	public enum FloorType
	{
		Unknown = 0,
		LowFloor,
		HighFloor,
		SemiLowFloor, // no longer required?
	}

	public static class FloorTypeHelper
	{
		public static string GetLabel(this FloorType self)
		{
			switch (self)
			{
				case FloorType.Unknown:
					return "Unknown";
				case FloorType.LowFloor:
					return "Low Floor";
				case FloorType.HighFloor:
					return "High Floor";
				case FloorType.SemiLowFloor:
					return "Semi Low Floor";
				default:
					throw new ArgumentOutOfRangeException(nameof(self), self, null);
			}
		}


	}
}