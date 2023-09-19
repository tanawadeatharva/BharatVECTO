/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
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
using System.Diagnostics;
using System.Globalization;

namespace TUGraz.VectoCommon.Utils
{
	public class ConvertedSI
	{
		private readonly double _value;
		public string Units { get; }

		public ConvertedSI(double value, string units)
		{
			_value = value;
			Units = units;
		}

		public string GetUnitString()
		{
			return Units;
		}

		public double Value => _value;

		protected bool Equals(ConvertedSI other)
		{
			return _value.Equals(other._value) && string.Equals(Units, other.Units);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != GetType())
				return false;
			return Equals((ConvertedSI)obj);
		}

		public override int GetHashCode()
		{
			unchecked {
				return (_value.GetHashCode() * 397) ^ (Units != null ? Units.GetHashCode() : 0);
			}
		}

		[DebuggerHidden]
		public ConvertedSI Abs()
		{
			return new ConvertedSI(Math.Abs(Value), Units);
		}

		public static implicit operator double(ConvertedSI self)
		{
			return self._value;
		}

		public static implicit operator ConvertedSI(SI self)
		{
			return self == null ? null : new ConvertedSI(self.Value(), self.UnitString);
		}

		public override string ToString()
		{
			// todo mk2017-10-13: decimal places?
			return _value.ToString(CultureInfo.InvariantCulture); // + " [" + _units + "]";
		}

		public object ToString(CultureInfo invariantCulture)
		{
			throw new NotImplementedException();
		}

		public string ToOutputFormat(uint? decimals = null, double? outputFactor = null, bool? showUnit = null)
		{
			decimals = decimals ?? 4;
			outputFactor = outputFactor ?? 1.0;
			showUnit = showUnit ?? false;

			if (showUnit.Value) {
				return (_value * outputFactor.Value).ToString("F" + decimals.Value, CultureInfo.InvariantCulture) + " [" +
						Units + "]";
			}

			return (_value * outputFactor.Value).ToString("F" + decimals.Value, CultureInfo.InvariantCulture);
		}
	}

	public static class SIConvertExtensionMethods
	{
		private const int Kilo = 1000;
		private const double Mega = 1e6;
		private const int SecondsPerHour = 60 * 60;
		private const int SecondsPerMinute = 60 * 60;
		const int CubicMeterToLiter = 10 * 10 * 10;
		const int CubicMeterToCubicCentimeter = 100 * 100 * 100;
		const int MeterTo100KiloMeter = 100 * Kilo;
		const int KilogrammToTon = Kilo;

		public static ConvertedSI ConvertToGramm(this Kilogram value)
		{
			return new ConvertedSI(value.Value() * Kilo, "g");
		}
		public static ConvertedSI ConvertToTon(this Kilogram value)
		{
			return new ConvertedSI(value.Value() / Kilo, "t");
		}
		public static ConvertedSI ConvertToKiloMeterPerHour(this MeterPerSecond value)
		{
			return new ConvertedSI(value.Value() * SecondsPerHour / Kilo, "km/h");
		}
		public static ConvertedSI ConvertToGrammPerKiloMeter(this KilogramPerMeter value)
		{
			return value == null ? null : new ConvertedSI(value.Value() * Kilo * Kilo, "g/km");
		}

		public static ConvertedSI ConvertToGrammPerPassengerKilometer(this KilogramPerMeter value)
		{
			return value == null ? null : new ConvertedSI(value.Value() * Kilo * Kilo, "g/p-km");
		}

		public static ConvertedSI ConvertToKiloWattHourPerKiloMeter(this JoulePerMeter value)
		{
			return new ConvertedSI(value.Value() / SecondsPerHour, "kWh/km");
		}


        public static ConvertedSI ConvertToGramPerKiloWattHour(this SpecificFuelConsumption value)
		{
			return new ConvertedSI(value.Value() * SecondsPerHour * Mega, "g/kWh");
		}

		public static ConvertedSI ConvertToGramPerKiloWattHour(this KilogramPerWattSecond value)
		{
			return new ConvertedSI(value.Value() * SecondsPerHour *Mega, "g/kWh");
		}

		public static ConvertedSI ConvertToLiterPer100Kilometer(this VolumePerMeter value)
		{
			return value == null ? null : new ConvertedSI(value.Value() * (10*10*10) * (100*1000), "l/100km");
		}
		
		public static ConvertedSI ConvertToLiterPer100TonKiloMeter(this VolumePerMeterMass value)
		{
			return value == null ? null  : new ConvertedSI(value.Value() * CubicMeterToLiter * (MeterTo100KiloMeter * KilogrammToTon), "l/100tkm");
		}

		public static ConvertedSI ConvertToLiterPerCubicMeter100KiloMeter(this VolumePerMeterVolume value)
		{
			return new ConvertedSI(value.Value() * CubicMeterToLiter * MeterTo100KiloMeter, "l/100m³-km");
		}

		public static ConvertedSI ConvertToGrammPerHour(this KilogramPerSecond value)
		{
			return new ConvertedSI(value.Value() * Kilo * SecondsPerHour, "g/h");
		}

		public static ConvertedSI ConvertToKiloMeter(this Meter value)
		{
			return new ConvertedSI(value.Value() / Kilo, "km");	
		}

		public static ConvertedSI ConvertToCubicCentiMeter(this CubicMeter value)
		{
			return new ConvertedSI(value.Value() * CubicMeterToCubicCentimeter, "cm³");
		}

		public static ConvertedSI ConvertToGrammPerCubicMeterKiloMeter(this KilogramPerMeterCubicMeter value)
		{
			return new ConvertedSI(value.Value()  * Kilo * Kilo, "g/m³-km");
		}

		public static ConvertedSI ConvertToGrammPerTonKilometer(this KilogramPerMeterMass value)
		{
			return new ConvertedSI(value.Value() * Kilo * Kilo * Kilo, "g/t-km");
		}

		public static ConvertedSI ConvertToPerKiloWattHour(this PerWattSecond value)
		{
			return new ConvertedSI(value.Value() * 3600e3, "#/kWh");
		}
				
		public static ConvertedSI ConvertToMilliGramPerKiloWattHour(this KilogramPerWattSecond value)
		{
			return new ConvertedSI(value.Value() * 3600e9, "mg/kWh");
		}

		public static ConvertedSI ConvertToKiloWattHour(this WattSecond value)
		{
			return new ConvertedSI(value.Value() / Kilo / SecondsPerHour, "kWh");
		}
		
		public static ConvertedSI ConvertToKiloWattHourPerKiloMeter(this WattSecondPerMeter value)
		{
			return new ConvertedSI(value.Value() / Kilo / SecondsPerHour * Kilo, "kWh/km");
		}

		public static ConvertedSI ConvertToKiloWattHourPerPassengerKiloMeter(this WattSecondPerMeter value)
		{
			return new ConvertedSI(value.Value() / Kilo / SecondsPerHour * Kilo, "kWh/p-km");
		}

		public static ConvertedSI ConvertToKiloWattHourPerTonKiloMeter(this WattSecondPerMeterKilogram value)
		{
			return new ConvertedSI(value.Value() / Kilo / SecondsPerHour * Kilo * KilogrammToTon, "kWh/t-km");
		}

		public static ConvertedSI ConvertToKiloWattHourPerCubicMeterKiloMeter(this WattSecondPerCubicMeterMeter value)
		{
			return new ConvertedSI(value.Value() / Kilo / SecondsPerHour * Kilo, "kWh/m³-km");
		}

		public static ConvertedSI ConvertToMegaJoulePerKiloMeter(this WattSecondPerMeter value)
		{
			return new ConvertedSI(value.Value() / Mega * Kilo, "MJ/km");
		}
		public static ConvertedSI ConvertToMegaJoulePerPassengerKiloMeter(this WattSecondPerMeter value)
		{
			return new ConvertedSI(value.Value() / Mega * Kilo, "MJ/p-km");
		}

		public static ConvertedSI ConvertToMegaJoulePerTonKiloMeter(this WattSecondPerMeterKilogram value)
		{
			return new ConvertedSI(value.Value() / Mega * Kilo * KilogrammToTon, "MJ/t-km");
		}

		public static ConvertedSI ConvertToMegaJoulePerCubicMeterKiloMeter(this WattSecondPerCubicMeterMeter value)
		{
			return new ConvertedSI(value.Value() / Mega * Kilo , "MJ/m³-km");
		}

		public static ConvertedSI ConvertToWattHour(this WattSecond value)
		{
			return new ConvertedSI(value.Value() / SecondsPerHour, "Wh");
		}
		public static ConvertedSI ConvertToKiloWatt(this Watt value)
		{
			return new ConvertedSI(value.Value() / Kilo, "kW");
		}

		public static ConvertedSI ConvertToRoundsPerMinute(this PerSecond value)
		{
			return new ConvertedSI(value.AsRPM, "rpm");
		}

		public static ConvertedSI ConvertToPerHour(this PerSecond value)
		{
			return new ConvertedSI(value.Value() * SecondsPerHour, "1/h");
		}

		public static ConvertedSI ConvertToWattHourPerCubicMeter(this JoulePerCubicMeter value)
		{
			return new ConvertedSI(value.Value() / SecondsPerHour, "Wh/m^3");
		}

		public static ConvertedSI ConvertToCubicDeziMeter(this CubicMeter value)
		{
			return new ConvertedSI(value.Value() * CubicMeterToLiter, "dm^3");
		}
		public static ConvertedSI ConvertToMilliMeter(this Meter value)
		{
			return new ConvertedSI(value.Value() * Kilo, "mm");
		}

		public static ConvertedSI ConvertToMegaJoulePerKilogram(this JoulePerKilogramm value)
		{
			return new ConvertedSI(value.Value() / Kilo / Kilo, "MJ/kg");
		}

		public static ConvertedSI ConvertToKiloWattHourPerKilogram(this JoulePerKilogramm value)
		{
			return new ConvertedSI(value.Value() / SecondsPerHour / Kilo, "kWh/kg");
		}

		public static ConvertedSI ConvertToMinutes(this Second sec)
		{
			return new ConvertedSI(sec.Value() / SecondsPerMinute, "min");
		}

		public static ConvertedSI ConvertToNlPerMin(this NormLiterPerSecond nlps)
		{
			return new ConvertedSI(nlps.Value() * SecondsPerMinute, "Nl/min");
		}

		public static ConvertedSI ConvertToMegaJoulePerKilometer(this JoulePerMeter jpm)
		{
			return new ConvertedSI(jpm.Value() / Mega * Kilo, "MJ/km");
		}

		public static ConvertedSI ConvertToMegaJoulePerPassengerKilometer(this JoulePerMeter jpm)
		{
			return new ConvertedSI(jpm.Value() / Mega * Kilo, "MJ/p-km");
		}

		public static ConvertedSI ConvertToMegaJoulePerTonKiloMeter(this JoulePerKilogramMeter jpkgm)
		{
			return new ConvertedSI(jpkgm.Value() /Mega * Kilo * Kilo, "MJ/t-km");
		}

		public static ConvertedSI ConvertToMegaJoulePerCubicMeterKiloMeter(this JoulePerCubicMeterMeter jpm3m)
		{
			return new ConvertedSI(jpm3m.Value() / Mega * Kilo, "MJ/m³-km");
		}

		public static ConvertedSI ConvertToLiterPer100KiloMeter(this CubicMeterPerMeter jpm3m)
		{
			return new ConvertedSI(jpm3m.Value() * CubicMeterToLiter * MeterTo100KiloMeter, "l/100km");
		}

		public static ConvertedSI ConvertToLiterPerPassengerKiloMeter(this CubicMeterPerMeter jpm3m)
		{
			return new ConvertedSI(jpm3m.Value() * CubicMeterToLiter * Kilo, "l/p-km");
		}

		public static ConvertedSI ConvertToLiterPerTonKiloMeter(this CubicMeterPerKilogramMeter m3pkmm)
		{
			return new ConvertedSI(m3pkmm.Value() * CubicMeterToLiter * Kilo * Kilo, "l/t-km");
		}

		public static ConvertedSI ConvertToLiterPerCubicMeterKiloMeter(this CubicMeterPerCubicMeterMeter m3pm3m)
		{
			return new ConvertedSI(m3pm3m.Value() * CubicMeterToLiter * Kilo, "l/m³-km");
		}

	

		public static Meter ConvertToMeter(this ConvertedSI mm)
		{
			if (mm.Units != "mm") {
				return null;
			}
			//return ElementExists(XMLNames.Bus_HeighIntegratedBody)
			//	? GetDouble(XMLNames.Bus_HeighIntegratedBody).SI(Unit.SI.Milli.Meter).Cast<Meter>()
			//	: null;

			return mm.Value.SI(Unit.SI.Milli.Meter).Cast<Meter>();
		}
	}
}



