using System;

namespace TUGraz.VectoCore.Models.Declaration {
	public enum WHRType
	{
		None,
		MechanicalOnly,
		ElectricalOnly,
		MechanicalAndElectrical
	}

	public static class WHRTypeHelper
	{
		public static string ToXMLFormat(this WHRType whrType)
		{
			switch (whrType) {
				case WHRType.None: return "none";
				case WHRType.MechanicalOnly: return "mechanical only";
				case WHRType.ElectricalOnly: return "electrical only";
				case WHRType.MechanicalAndElectrical: return "mechanical and electrical";
				default: throw new ArgumentOutOfRangeException(nameof(whrType), whrType, null);
			}
		}
	}
}