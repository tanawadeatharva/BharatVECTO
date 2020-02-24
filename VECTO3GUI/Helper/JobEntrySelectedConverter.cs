using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace VECTO3GUI.Helper
{
	public class JobEntrySelectedConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var jobEntry = (ViewModel.Impl.JobEntry)((ListViewItem)value)?.Content;
			return jobEntry != null && jobEntry.Selected;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
