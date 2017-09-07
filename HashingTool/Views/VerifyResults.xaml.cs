using System.Windows;
using System.Windows.Controls;
using HashingTool.ViewModel;

namespace HashingTool.Views
{
	/// <summary>
	/// Interaction logic for VerifyResults.xaml
	/// </summary>
	/// 

	

	public partial class VerifyResults : UserControl
	{
		public VerifyResults()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var ui = sender as FrameworkElement;
			if (ui == null)
				return;
			var context = ui.DataContext as HashedXMLFile;
			if (context == null)
				return;

			var dialog = new XMLValidationErrorsDialog();
			dialog.XMLErrors = context.XMLFile.XMLValidationErrors;
			dialog.ShowDialog();

			//MessageBox.Show("Hello World", "test", MessageBoxButton.OK);
		}
	}
}
