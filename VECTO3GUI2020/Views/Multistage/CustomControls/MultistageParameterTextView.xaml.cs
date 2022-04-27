using System.Windows.Controls;
using System.Windows.Input;

namespace VECTO3GUI2020.Views.Multistage.CustomControls
{
    /// <summary>
    /// Interaction logic for MultistageParameterTextView.xaml
    /// </summary>
    public partial class MultistageParameterTextView : UserControl
    {
        public MultistageParameterTextView()
        {
            InitializeComponent();
        }

		private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			EditingEnabledCheckBox.IsChecked = true;
		}
	}
}
