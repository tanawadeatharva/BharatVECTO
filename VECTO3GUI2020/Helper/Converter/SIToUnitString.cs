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
			if (value == null) {
				return Binding.DoNothing;
			}


            if(value is SI SIValue)
            {
                if (SIValue.UnitString == "1/s")
                {
                    return "rpm";
                }
                return SIValue.UnitString;
            }

			if (value is ConvertedSI convertedSI) {
				return convertedSI.Units;
			}



            //TRY GET DYNAMIC UNIT STRING
			try {
				dynamic type = value?.GetType();
				if (type == null) {
					return Binding.DoNothing;
				}

				var unitString = type.GetUnitString();
				return unitString;
			} catch (Exception e){
				return Binding.DoNothing;
			}

		}

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
