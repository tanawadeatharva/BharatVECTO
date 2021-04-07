using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Windows.Data;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI2020.Util;
using SIUtils = VECTO3GUI2020.Util.SIUtils;

namespace VECTO3GUI2020.Helper.Converter
{
    class SIValueToStringConverter : IValueConverter
	{
		private SI _si;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
			if (value == null) {
				_si = null;
				return value;
			}
            if(value is SI SIvalue) {
				_si = SIvalue;
				return SIvalue.ToGUIFormat();
			}


            return Binding.DoNothing;
        }

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
			if (_si == null) {
				return value;
			}

			try {
				var newSi = SIUtils.CreateSIValue(_si.GetType(), Double.Parse(value as string));
				return newSi;
			} catch (Exception e) {
				return value;
			}
		}
    }
}
