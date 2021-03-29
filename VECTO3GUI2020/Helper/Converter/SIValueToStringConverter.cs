using System;
using System.Globalization;
using System.Windows.Data;
using TUGraz.VectoCommon.Utils;

namespace VECTO3GUI2020.Helper.Converter
{
    class SIValueToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is SI SIvalue)
            {
                return SIvalue.ToGUIFormat();
            }


            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
