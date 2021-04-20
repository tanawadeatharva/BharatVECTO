using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VECTO3GUI2020.Helper.Converter
{
    class EnumConverter : IValueConverter
    {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			/*
			 *Enum dummyEnum;
					if (underlyingType != null) {
						dummyEnum = Enum.Parse(underlyingType, underlyingType.GetEnumNames()[0]);
                    } else {
						dummyEnum = Enum.Parse(dynType, dynType.GetEnumNames()[0]);
					}
			 *
			 *
			 *
			 *
			 */

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}
}
