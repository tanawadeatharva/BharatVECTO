namespace HashingTool.ViewModel
{
	public class VerifyInputDataViewModel : ObservableObject, IMainView
	{
		public VerifyInputDataViewModel(MainWindowViewModel mainWindowViewModel) {}

		public string Name
		{
			get { return "Verify Input Data"; }
		}
	}
}
