using System.Collections.ObjectModel;
using System.Windows.Input;
using TUGraz.VectoCommon.InputData;
using VECTO3GUI.Util;

namespace VECTO3GUI.ViewModel.Interfaces
{
	public interface IJobEditViewModel 
	{

		string JobFile { get; }

		IInputDataProvider InputDataProvider { get; set; }

		bool DeclarationMode { get; }
		ObservableCollection<Component> Components { get; }
		ICommand EditComponent { get; }
		IComponentViewModel CurrentComponent { get; }

		ICommand SaveJob { get; }
		ICommand SaveToJob { get; }
		ICommand CloseJob { get; }

	}
}
