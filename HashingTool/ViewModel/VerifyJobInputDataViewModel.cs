using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using HashingTool.Helper;
using HashingTool.ViewModel.UserControl;
using TUGraz.VectoHashing;

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
