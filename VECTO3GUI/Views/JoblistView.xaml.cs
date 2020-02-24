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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VECTO3GUI.ViewModel;
using VECTO3GUI.ViewModel.Impl;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.Views
{
	/// <summary>
	/// Interaction logic for HomeView.xaml
	/// </summary>
	public partial class JoblistView : UserControl
	{
		public JoblistView()
		{
			InitializeComponent();
		}

		public JoblistView(IJoblistViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}

		private void Joblisting_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var listView = (ListView)sender;
			if (listView.SelectedItems.Count == 0) {
				return;
			}

			var model = (IJoblistViewModel)DataContext;
			model.EditJob.Execute(listView.SelectedItem);
		}
	}
}
