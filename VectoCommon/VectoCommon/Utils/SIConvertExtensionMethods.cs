using System;
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
			if (obj.GetType() != this.GetType())
				return false;
			return Equals((ConvertedSI)obj);
		}

		public override int GetHashCode()
		{
			unchecked {
				return (_value.GetHashCode() * 397) ^ (Units != null ? Units.GetHashCode() : 0);
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
						Units + "]";
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

        public static ConvertedSI ConvertToLiterPer100Kilometer(this VolumePerMeter value)
        {
            return value == null ? null : new ConvertedSI(value.Value() * (10*10*10) * (100*1000), "l/100km");
        }
        
        public static ConvertedSI ConvertToLiterPer100TonKiloMeter(this VolumePerMeterMass value)
        {
            const int CubicMeterToLiter = 10 * 10 * 10;
            const int MeterTo100KiloMeter = 100 * Kilo;
            const int KilogrammToTon = Kilo;

            return value == null ? null  : new ConvertedSI(value.Value() * CubicMeterToLiter * (MeterTo100KiloMeter * KilogrammToTon), "l/100tkm");
        }

		public static ConvertedSI ConvertToLiterPerCubicMeter100KiloMeter(this VolumePerMeterVolume value)
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

        public static ConvertedSI ConvertToGrammPerCubicMeterKiloMeter(this KilogramPerMeterCubicMeter value)
        {
            return new ConvertedSI(value.Value()  * Kilo * Kilo, "g/m^3km");
        }

        public static ConvertedSI ConvertToGrammPerTonKilometer(this KilogramPerMeterMass value)
        {
            return new ConvertedSI(value.Value() * Kilo * Kilo * Kilo, "g/tkm");
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
            return new ConvertedSI(value.AsRPM, "rpm");
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



