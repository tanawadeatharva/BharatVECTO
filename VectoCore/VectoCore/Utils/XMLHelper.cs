using System;
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Utils {

	public static class XMLHelper {

		
		public static object[] ValueAsUnit(Kilogram mass, string unit, uint? decimals = 0)
		{
			switch (unit) {
				case "t": return ValueAsUnit(mass.ConvertToTon(), unit, decimals);
				case "kg": return ValueAsUnit(mass.Value(), unit, decimals);
			}
			throw new NotImplementedException(string.Format("unknown unit '{0}'", unit));
		}

		public static object[] ValueAsUnit(Watt power, string unit, uint? decimals = 0)
		{
			switch (unit) {
				case "kW": return ValueAsUnit(power.ConvertToKiloWatt(), unit, decimals);
				case "W": return ValueAsUnit(power.Value(), unit, decimals);
			}
			throw new NotImplementedException(string.Format("unknown unit '{0}'", unit));
		}

		public static object[] ValueAsUnit(CubicMeter volume, string unit, uint? decimals = 0)
		{
			switch (unit) {
				case "ltr": return ValueAsUnit(volume.ConvertToCubicDeziMeter(), unit, decimals);
				case "ccm": return ValueAsUnit(volume.ConvertToCubicCentiMeter(), unit, decimals);
				case "m3": return ValueAsUnit(volume.Value(), unit, decimals);
			}
			throw new NotImplementedException(string.Format("unknown unit '{0}'", unit));
		}

		public static object[] ValueAsUnit(PerSecond angSpeed, string unit, uint? decimals = 0)
		{
			switch (unit) {
				case "rpm": return ValueAsUnit(angSpeed.ConvertToRoundsPerMinute(), unit, decimals);
			}
			throw new NotImplementedException(string.Format("unknown unit '{0}'", unit));
		}
		

		public static object[] ValueAsUnit(MeterPerSecond speed, string unit, uint? decimals)
		{
			switch (unit) {
				case "km/h": return ValueAsUnit(speed.ConvertToKiloMeterPerHour(), unit, decimals);
			}
			throw new NotImplementedException(string.Format("unknown unit '{0}'", unit));
		}

		public static object[] ValueAsUnit(MeterPerSquareSecond acc, string unit, uint? decimals)
		{
			switch (unit) {
				case "m/s²": return ValueAsUnit(acc.Value(), unit, decimals);
			}
			throw new NotImplementedException(string.Format("unknown unit '{0}'", unit));
		}

		private static object[] ValueAsUnit(double value, string unit, uint? decimals)
		{
			return new object[] {
				new XAttribute(XMLNames.Report_Results_Unit_Attr, unit),
				value.ToXMLFormat(decimals)
			};
		}
	}
}