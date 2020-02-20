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
using VECTO3.ViewModel;
using VECTO3.ViewModel.Interfaces;

namespace VECTO3.Views
{
	/// <summary>
	/// Interaction logic for JobEditView.xaml
	/// </summary>
	public partial class JobEditView : UserControl
	{
		public JobEditView()
		{
			InitializeComponent();
		}

		public JobEditView(IJobEditViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}

		
	}
}
