using System;
using System.Globalization;

namespace TUGraz.VectoCommon.Utils
{
    public class ConvertedSI
    {
        private readonly double _value;
        private readonly string _units;

        public ConvertedSI(double value, string units)
        {
            _value = value;
            _units = units;
        }

		protected bool Equals(ConvertedSI other)
		{
			return _value.Equals(other._value) && string.Equals(_units, other._units);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != this.GetType())
				return false;
			return Equals((ConvertedSI)obj);
		}

		public override int GetHashCode()
		{
			unchecked {
				return (_value.GetHashCode() * 397) ^ (_units != null ? _units.GetHashCode() : 0);
			}
		}

		public static implicit operator double(ConvertedSI self)
        {
            return self._value;
        }

        public static implicit operator ConvertedSI(SI self)
        {
            return self == null ? null : new ConvertedSI(self.Value(), self.GetUnitString());
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
						_units + "]";
			}

			return (_value * outputFactor.Value).ToString("F" + decimals.Value, CultureInfo.InvariantCulture);
		}
	}

    public static class SIConvertExtensionMethods
    {
        private const int Kilo = 1000;
        private const int SecondsPerHour = 60 * 60;
        
        public static ConvertedSI ConvertToGramm(this Kilogram value)
        {
            return new ConvertedSI(value.Value() * Kilo, "g");
        }
        public static ConvertedSI ConvertToTon(this Kilogram value)
        {
            return new ConvertedSI(value.Value() / Kilo, "Ton");
        }
        public static ConvertedSI ConvertToKiloMeterPerHour(this MeterPerSecond value)
        {
            return new ConvertedSI(value.Value() * SecondsPerHour / Kilo, "km/h");
        }
        public static ConvertedSI ConvertToGrammPerKiloMeter(this KilogramPerMeter value)
        {
            return value == null ? null : new ConvertedSI(value.Value() * Kilo * Kilo, "g/km");
        }

        public static ConvertedSI ConvertToLiterPer100Kilometer(this SI value)
        {
            return value == null ? null : new ConvertedSI(value.Value() * (10*10*10) * (100*1000), "l/100km");
        }
        
        public static ConvertedSI ConvertToLiterPer100TonKiloMeter(this SI value)
        {
            const int CubicMeterToLiter = 10 * 10 * 10;
            const int MeterTo100KiloMeter = 100 * Kilo;
            const int KilogrammToTon = Kilo;

            return value == null ? null  : new ConvertedSI(value.Value() * CubicMeterToLiter * (MeterTo100KiloMeter * KilogrammToTon), "l/100tkm");
        }

		public static ConvertedSI ConvertToLiterPerCubicMeter100KiloMeter(this SI value)
        {
            const int CubicMeterToLiter = 10 * 10 * 10;
            const int MeterTo100KiloMeter = 100 * Kilo;
            return new ConvertedSI(value.Value() * CubicMeterToLiter * MeterTo100KiloMeter, "l/100m^3km");
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
            return new ConvertedSI(value.Value() * 100 * 100 * 100, "cm^3");
        }

        public static ConvertedSI ConvertToGrammPerCubicMeterKiloMeter(this SI value)
        {
            return new ConvertedSI(value.Value()  * Kilo * Kilo, "g/m^3km");
        }

        public static ConvertedSI ConvertToGrammPerTonKilometer(this SI value)
        {
            return new ConvertedSI(value.Value() * Kilo * Kilo * Kilo, "g/tkm");
        }
		
        public static ConvertedSI ConvertToLiterPer100KiloMeter(this SI value)
        {
            return new ConvertedSI(value.Value() * 10 * 10 * 10 * 100 * Kilo, "l/100km");
        }

        public static ConvertedSI ConvertToKiloWattHour(this WattSecond value)
        {
            return new ConvertedSI(value.Value() / Kilo / SecondsPerHour, "kWh");
        }
        public static ConvertedSI ConvertToKiloWatt(this Watt value)
        {
            return new ConvertedSI(value.Value() / Kilo, "kW");
        }

        public static ConvertedSI ConvertToRoundsPerMinute(this PerSecond value)
        {
            return new ConvertedSI(value.Value() * 2 * Math.PI / 60, "rpm");
        }
        public static ConvertedSI ConvertToCubicDeziMeter(this CubicMeter value)
        {
            return new ConvertedSI(value.Value() * 10 * 10 * 10, "dm^3");
        }
        public static ConvertedSI ConvertToMilliMeter(this Meter value)
        {
            return new ConvertedSI(value.Value() * Kilo, "mm");
        }
    }
}



