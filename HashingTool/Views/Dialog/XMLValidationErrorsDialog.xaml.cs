using System;
using System.Collections;
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
			typeof(ICollection), typeof(XMLValidationErrorsDialog));

		public static readonly DependencyProperty ErrorCountProperty = DependencyProperty.Register("ErrorCount",
			typeof(int), typeof(XMLValidationErrorsDialog));

		public XMLValidationErrorsDialog()
		{
			InitializeComponent();
			(Content as FrameworkElement).DataContext = this;
		}

		public ICollection XMLErrors
		{
			get { return (ICollection)GetValue(XMLErrorsProperty); }
			set { SetValue(XMLErrorsProperty, value); }
		}

		public int ErrorCount
		{
			get {
				var value = GetValue(ErrorCountProperty);
				if (value != null) {
					return (int)value;
				}
				return 0;
			}
			set { SetValue(ErrorCountProperty, value); }
		}

		private void btnCopy_Click(object sender, RoutedEventArgs e)
		{
			var errors = string.Join(Environment.NewLine,(from object item in lbErrors.Items select item.ToString()).ToList());

			Clipboard.SetText( errors);
		}
	}
}
