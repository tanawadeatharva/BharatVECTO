using System;
using System.ComponentModel;
using System.Windows.Input;
using HashingTool.Model;

namespace HashingTool.ViewModel
{
	public class HashComponentDataViewModel : ObservableObject, IMainView
	{
		private readonly ApplicationViewModel _applicationViewModel;

		public HashComponentDataViewModel() {}

		public HashComponentDataViewModel(ApplicationViewModel applicationViewModel)
		{
			_applicationViewModel = applicationViewModel;
		}

		public string Name
		{
			get { return "Hash Component Data"; }
		}

		public ICommand ShowHomeViewCommand
		{
			get { return ApplicationViewModel.HomeView; }
		}
	}
}
