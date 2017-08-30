using System.Windows.Input;

namespace HashingTool.ViewModel
{
	public class VerifyInputDataViewModel : ObservableObject, IMainView
	{
		private readonly ApplicationViewModel _applicationViewModel;
		public VerifyInputDataViewModel() {}

		public VerifyInputDataViewModel(ApplicationViewModel applicationViewModel)
		{
			_applicationViewModel = applicationViewModel;
		}

		public string Name
		{
			get { return "Verify Input Data"; }
		}

		public ICommand ShowHomeViewCommand
		{
			get { return ApplicationViewModel.HomeView; }
		}
	}
}
