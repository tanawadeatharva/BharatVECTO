using System.Windows;
using System.Windows.Automation.Peers;
using HashingTool.ViewModel.UserControl;

namespace HashingTool.Views
{
	/// <summary>
	/// Interaction logic for VectoXMLFileSelector.xaml
	/// </summary>	
	public partial class VectoXMLFileSelector
	{
		public static readonly DependencyProperty XMLFileProperty = DependencyProperty.Register("XMLFile",
			typeof(XMLFileSelector),
			typeof(VectoXMLFileSelector));


		public VectoXMLFileSelector()
		{
			InitializeComponent();
			(Content as FrameworkElement).DataContext = this;
		}

		public XMLFileSelector XMLFile
		{
			get { return (XMLFileSelector)GetValue(XMLFileProperty); }
			set { SetValue(XMLFileProperty, value); }
		}

		private void btnDetails_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new XMLValidationErrorsDialog();
			dialog.XMLErrors = XMLFile.XMLValidationErrors;
			dialog.ErrorCount = XMLFile.XMLValidationErrors.Count;
			dialog.ShowDialog();
		}
	}
}
