using System;
using System.Globalization;
using System.Windows.Data;
using VECTO3GUI2020.Properties;

namespace VECTO3GUI2020.Helper.Converter
{
	public class PropertyNameToLabelTextConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return NameResolver.ResolveName(value as string, BusStrings.ResourceManager, Strings.ResourceManager) ?? Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}