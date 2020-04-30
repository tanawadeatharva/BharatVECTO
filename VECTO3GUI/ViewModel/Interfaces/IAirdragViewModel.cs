using System.Windows.Input;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace VECTO3GUI.ViewModel.Interfaces
{
	public interface IAirdragViewModel : IAirdrag, IComponentViewModel
	{
		IAirdragDeclarationInputData ModelData { get; }
		bool IsEditable { get; }

		bool NoAirdragData { get; }
		bool UseMeasurementData { get; }

		ICommand LoadFileCommand { get; }
		ICommand AirdragConfigCommand { get; }

	}
}
