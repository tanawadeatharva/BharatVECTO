using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace VECTO3GUI.ViewModel.Interfaces
{
	public interface IAirdragViewModel : IAirdrag, IComponentViewModel
	{
		IAirdragDeclarationInputData ModelData { get; }
	}
}
