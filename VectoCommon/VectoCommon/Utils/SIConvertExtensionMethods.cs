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

        public static implicit operator double(ConvertedSI self)
        {
            return self._value;
        }

        public static implicit operator ConvertedSI(SI self)
        {
            return new ConvertedSI(self.Value(), self.GetUnitString());
        }

        public override string ToString()
        {
            return _value.ToString(CultureInfo.InvariantCulture); // + " [" + _units + "]";
        }
    }

    public static class SIConvertExtensionMethods
    {
        private const int Kilo = 1000;
        private const int SecondsPerHour = 60 * 60;
        
        public static ConvertedSI ConvertToGramm(this SI value)
        {
            return new ConvertedSI(value.Value() * Kilo, "g");
        }
        public static ConvertedSI ConvertToTon(this SI value)
        {
            return new ConvertedSI(value.Value() * Kilo, "Ton");
        }
        public static ConvertedSI ConvertToKiloMeterPerHour(this SI value)
        {
            return new ConvertedSI(value.Value() * SecondsPerHour / Kilo, "km/h");
        }
        public static ConvertedSI ConvertToGrammPerKiloMeter(this SI value)
        {
            return value == null ? null : new ConvertedSI(value.Value() * Kilo / Kilo, "g/km");
        }

        public static ConvertedSI ConvertToLiterPer100Kilometer(this SI value)
        {
            return new ConvertedSI(value.Value() * (10*10*10) / (100*1000), "l/100km");
        }
        
        public static ConvertedSI ConvertToLiterPer100TonKiloMeter(this SI value)
        {
            const int CubicMeterToLiter = 10 * 10 * 10;
            const int MeterTo100KiloMeter = 100 * 1000;
            const int KilogrammToTon = 1000;
            return new ConvertedSI(value.Value() * CubicMeterToLiter / (MeterTo100KiloMeter * KilogrammToTon), "l/100tkm");
        }


        
        public static ConvertedSI ConvertToLiterPerCubicMeter100KiloMeter(this SI value)
        {
            const int CubicMeterToLiter = 10 * 10 * 10;
            const int MeterTo100KiloMeter = 100 * 1000;
            return new ConvertedSI(value.Value() * CubicMeterToLiter / MeterTo100KiloMeter, "l/100m^3km");
        }

        public static ConvertedSI ConvertToGrammPerHour(this SI value)
        {
            return new ConvertedSI(value.Value() * Kilo / SecondsPerHour, "g/h");
        }

        public static ConvertedSI ConvertToKiloMeter(this SI value)
        {
            return new ConvertedSI(value.Value() / Kilo, "km");
        }

        public static ConvertedSI ConvertToCubicCentiMeter(this SI value)
        {
            return new ConvertedSI(value.Value() * 100 * 100 * 100, "cm^3");
        }

        public static ConvertedSI ConvertToGrammPerCubicMeterKiloMeter(this SI value)
        {
            return new ConvertedSI(value.Value() * Kilo / Kilo * Kilo / Kilo / Kilo / Kilo, "g/m^3km");
        }

        public static ConvertedSI ConvertToGrammPerTonKilometer(this SI value)
        {
            return new ConvertedSI(value.Value() * Kilo / Kilo * Kilo, "g/tkm");
        }


        public static ConvertedSI ConvertToLiterPer100KiloMeter(this SI value)
        {
            return new ConvertedSI(value.Value() * 10 * 10 * 10 * 100 * Kilo, "l/100km");
        }

        public static ConvertedSI ConvertToKiloWattHour(this SI value)
        {
            return new ConvertedSI(value.Value() / Kilo / SecondsPerHour, "kWh");
        }
        public static ConvertedSI ConvertToKiloWatt(this SI value)
        {
            return new ConvertedSI(value.Value() / Kilo, "kW");
        }

        public static ConvertedSI ConvertToRoundsPerMinute(this SI value)
        {
            return new ConvertedSI(value.Value() * 2 * Math.PI / 60, "rpm");
        }
        public static ConvertedSI ConvertToCubicDeziMeter(this SI value)
        {
            return new ConvertedSI(value.Value() * 10 * 10 * 10, "dm^3");
        }
        public static ConvertedSI ConvertToMilliMeter(this SI value)
        {
            return new ConvertedSI(value.Value() * Kilo, "mm");
        }
    }
}



