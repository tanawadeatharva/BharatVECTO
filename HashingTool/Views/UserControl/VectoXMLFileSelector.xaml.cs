using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using HashingTool.ViewModel;
using HashingTool.ViewModel.UserControl;

namespace HashingTool.Views
{
	/// <summary>
	/// Interaction logic for VectoXMLFileSelector.xaml
	/// </summary>	
	public partial class VectoXMLFileSelector : UserControl
	{
		public static readonly DependencyProperty XMLFileProperty = DependencyProperty.Register("XMLFile", typeof(XMLFile),
			typeof(VectoXMLFileSelector));


		public VectoXMLFileSelector()
		{
			InitializeComponent();
			(Content as FrameworkElement).DataContext = this;
		}

		public XMLFile XMLFile
		{
			get { return (XMLFile)GetValue(XMLFileProperty); }
			set { SetValue(XMLFileProperty, value); }
		}
	}
}
