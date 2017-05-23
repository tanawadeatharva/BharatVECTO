using System;
using System.Diagnostics.CodeAnalysis;

namespace TUGraz.VectoCommon.Models
{
	public enum FuelType
	{
		// ReSharper disable InconsistentNaming
		DieselCI,
		EthanolCI,
		PetrolPI,
		EthanolPI,
		LPG,
		NG
		// ReSharper restore InconsistentNaming
	}

	public static class FuelTypeHelper
	{
		public static string GetLabel(this FuelType ftype)
		{
			switch (ftype) {
				case FuelType.DieselCI:
					return "Diesel CI";
				case FuelType.EthanolCI:
					return "Ethanol CI";
				case FuelType.PetrolPI:
					return "Petrol PI";
				case FuelType.EthanolPI:
					return "Ethanol PI";
				case FuelType.LPG:
					return "LPG";
				case FuelType.NG:
					return "NG";
				default:
					throw new ArgumentOutOfRangeException("fuel type", ftype, null);
			}
		}

		public static string ToXMLFormat(this FuelType ftype)
		{
			return ftype.GetLabel();
		}
	}
}