using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace HashingTool.Views
{
	/// <summary>
	/// Interaction logic for AboutDialog.xaml
	/// </summary>
	public partial class AboutDialog : Window
	{
		public AboutDialog()
		{
			InitializeComponent();
		}

		private void EUPL_Link(object sender, MouseButtonEventArgs e)
		{
			Process.Start("https://joinup.ec.europa.eu/community/eupl/og_page/eupl");

		}

		private void Supportmail(object sender, MouseButtonEventArgs e)
		{
			Process.Start("mailto:vecto@jrc.ec.europa.eu");
		}
	}
}
