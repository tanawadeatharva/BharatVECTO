/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

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