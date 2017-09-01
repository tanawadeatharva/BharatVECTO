using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace HashingTool.Helper
{
	public class CollectionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var val = value as ICollection;
			if (val == null) {
				throw new ArgumentException("can only convert collections!");
			}
			if (targetType == typeof(object) || targetType == typeof(string)) {
				var tmp = new string[val.Count];
				var i = 0;
				foreach (var entry in val) {
					tmp[i++] = entry.ToString();
				}
				return string.Join(", ", tmp);
			}
			throw new ArgumentException("Unhandled target type");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
