using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HashingTool.ViewModel;
using Microsoft.Win32;

namespace HashingTool.Views
{
	/// <summary>
	/// Interaction logic for HashComponentData.xaml
	/// </summary>
	public partial class HashComponentData : UserControl
	{
		public HashComponentData()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new XMLValidationErrorsDialog();
			dialog.XMLErrors = (DataContext as HashComponentDataViewModel).XMLValidationErrors;

			dialog.ShowDialog();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(tbDigestValue.Text);
		}
	}
}
