using System.Windows;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.Common;

namespace VECTO3GUI2020.Helper
{
	public class WindowHelper : IWindowHelper
    {
		

		public void ShowWindow(object viewModel)
		{
			var window = new Window {
				Content = viewModel,
				Width = 800,
				Height = 600,
				SizeToContent = SizeToContent.WidthAndHeight,
				WindowStartupLocation = WindowStartupLocation.CenterScreen
			};
			

			if (viewModel is IViewModelBase vmBase) {
				window.Title = vmBase.Title;
			}

			window.Show();
			window.SizeToContent = SizeToContent.Manual;
		}


	}
}
