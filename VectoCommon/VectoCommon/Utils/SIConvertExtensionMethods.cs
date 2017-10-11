using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUGraz.VectoCommon.Utils
{

    //class: SummaryDataContainer : LoggingObject, IDisposable
    //method: public virtual void Write(IModalDataContainer modData, int jobNr, int runNr, VectoRunData runData)
    //  //row[DISTANCE] = distance.ConvertTo(Unit.SI.Kilo.Meter);
    //  row[DISTANCE] = distance.ConvertToKiloMeter();
    //  row[CO2_TKM] = kilogramPerMeter.ConvertToGrammPerKiloMeter() / vehicleLoading.ConvertToTon();
    //  row[SPEED] = speed.ConvertToKiloMeterPerHour();
    //  row[CO2_KM] = kilogramPerMeter.ConvertToGrammPerKiloMeter();
    //  row[CO2_TKM] = kilogramPerMeter.ConvertToGrammPerKiloMeter() / vehicleLoading.ConvertToTon();
    //  row[CO2_M3KM] = kilogramPerMeter.ConvertToGrammPerKiloMeter() / cargoVolume;
    // ConvertedSI ConvertToGrammPerHour(...)
    // 
    // row[ENGINE_DISPLACEMENT] = runData.EngineData.Displacement.ConvertToCubicCentiMeter();
    // ConvertedSI ConvertToKiloWattHour(..)

    //method: private static void WriteFuelconsumptionEntries(IModalDataContainer modData, DataRow row, Kilogram vehicleLoading,
    //                row[FCFINAL_LITERPER100TKM] = fcPer100lkm /
    //                     vehicleLoading.ConvertToTon();
    // --------------------
    // SI Data Type are used
    public class ConvertedSI  : SI
    {

        private string _units;
        //private double _value;

        public ConvertedSI(double value,UnitInstance ui,string units) :base(ui,value)
        {
            _units = units;
           // _value = value;
        }
        //public override string ToString()
        //{
        //    return ToString(null);
        //}

        //private string ToString(string format)
        //{
        //    if (string.IsNullOrEmpty(format))
        //    {
        //        format = "F4";
        //    }

        //    return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:" + format + "} [{2}]", _value, format, _units);
        //}

        //public static implicit operator SI(ConvertedSI convertedSI)
        //{
        //    return new SI(Unit.SI,convertedSI._value);
        //}
    }

    public static class SIConvertExtensionMethods
    {
        public static ConvertedSI ConvertToGramm(this SI value)
        {
            return new ConvertedSI(value.Value()*1000, Unit.SI.Kilo.Gramm, "g");
        }
        public static ConvertedSI ConvertToTon(this SI value)
        {
            return new ConvertedSI(value.Value() * 1000, Unit.SI.Kilo.Gramm, "Ton");
        }
        public static ConvertedSI ConvertToKiloMeterPerHour(this SI value)
        {
            return new ConvertedSI(value.Value() / 1000 *60*60 , Unit.SI.Kilo.Meter.Per.Hour, "km/h");
        }
        public static ConvertedSI ConvertToGrammPerKiloMeter(this SI value)
        {
            return new ConvertedSI(value.Value() * 1000 / 1000, Unit.SI.Gramm.Per.Kilo.Meter, "g/km");
        }
        public static ConvertedSI ConvertToGrammPerHour(this SI value)
        {
            return new ConvertedSI(value.Value() * 1000 / 60 / 60, Unit.SI.Gramm.Per.Hour, "g/h");
        }

        public static ConvertedSI ConvertToKiloMeter(this SI value)
        {
            return new ConvertedSI(value.Value() / 1000, Unit.SI.Kilo.Meter, "km");
        }

        public static ConvertedSI ConvertToCubicCentiMeter(this SI value)
        {
            return new ConvertedSI(value.Value() * 100 * 100 * 100, Unit.SI.Cubic.Centi.Meter, "cm^3");
        }

        public static ConvertedSI ConvertToKiloWattHour(this SI value)
        {
            return new ConvertedSI(value.Value() / 1000/60/60, Unit.SI.Kilo.Watt.Hour, "kWh");
        }
        public static ConvertedSI ConvertToKiloWatt(this SI value)
        {
            return new ConvertedSI(value.Value() / 1000, Unit.SI.Kilo.Watt, "kW");
        }

        public static ConvertedSI ConvertToRoundsPerMinute(this SI value)
        {
            return new ConvertedSI(value.Value() * 2 * Math.PI / 60, Unit.SI.Rounds.Per.Minute ,"rpm");
        }
        public static ConvertedSI ConvertToKiloGrammPerSecond(this SI value)
        {
            return new ConvertedSI(value.Value() , Unit.SI.Kilo.Gramm.Per.Second, "kg/s");
        }
        public static ConvertedSI ConvertToCubicDeziMeter(this SI value)
        {
            return new ConvertedSI(value.Value()*10*10*10, Unit.SI.Cubic.Dezi.Meter, "dm^3");
        }
        public static ConvertedSI ConvertToRadianPerSecond(this SI value)
        {
            return new ConvertedSI(value.Value(), Unit.SI.Radian.Per.Second, "rps");
        }
        public static ConvertedSI ConvertToMilliMeter(this SI value)
        {
            return new ConvertedSI(value.Value() * 1000, Unit.SI.Milli.Meter, "mm");
        }
    }
}



