using System.Collections.Generic;
using System.Windows.Input;

namespace HashingTool.ViewModel
{
	public class HomeViewModel : ObservableObject, IMainView
	{
		
		
		public string Name
		{
			get { return "Home"; }
		}

		public List<IMainView> MainViewModels
		{
			get { return ApplicationViewModel.AvailableViews; }
		}

		

		public ICommand ShowHomeViewCommand
		{
			get { return ApplicationViewModel.HomeView; }
		}

	}
}
