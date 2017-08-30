using System;
using System.ComponentModel;
using HashingTool.Model;

namespace HashingTool.ViewModel
{
	public class HashComponentDataViewModel : ObservableObject, IMainView
	{
		private MainWindowViewModel _homeView;

		public HashComponentDataViewModel(MainWindowViewModel homeView)
		{
			_homeView = homeView;
		}

		public string Name
		{
			get { return "Hash Component Data"; }
		}
	}
}
