using System;
using System.Globalization;
using System.Windows.Data;
using TUGraz.VectoCommon.Utils;

namespace VECTO3GUI2020.Helper.Converter
{
    class SIToUnitString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if(value is SI SIValue)
            {
                if (SIValue.UnitString == "1/s")
                {
                    return "rpm";
                }
                return SIValue.UnitString;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
