using System.Windows.Input;
using HashingTool.Helper;
using HashingTool.ViewModel.UserControl;

namespace HashingTool.ViewModel
{
	public class VerifyJobInputDataViewModel : VectoJobFile, IMainView
	{
		public VerifyJobInputDataViewModel() : base("Verify VECTO Job", HashingHelper.IsJobFile, HashingHelper.HashJobFile) {}


		public ICommand ShowHomeViewCommand
		{
			get { return ApplicationViewModel.HomeView; }
		}

		public XMLFile JobFile
		{
			get { return _xmlFile; }
		}
	}
}
