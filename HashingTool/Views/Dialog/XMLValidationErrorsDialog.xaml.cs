using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HashingTool.ViewModel.UserControl;

namespace HashingTool.Views
{
	/// <summary>
	/// Interaction logic for XMLValidationErrorsDialog.xaml
	/// </summary>
	public partial class XMLValidationErrorsDialog : Window
	{
		public static readonly DependencyProperty XMLErrorsProperty = DependencyProperty.Register("XMLErrors",
			typeof(ObservableCollection<string>),
			typeof(XMLValidationErrorsDialog));

		public XMLValidationErrorsDialog()
		{
			InitializeComponent();
			(Content as FrameworkElement).DataContext = this;
		}

		public ObservableCollection<string> XMLErrors
		{
			get { return (ObservableCollection<string>)GetValue(XMLErrorsProperty); }
			set { SetValue(XMLErrorsProperty, value); }
		}
	}
}
