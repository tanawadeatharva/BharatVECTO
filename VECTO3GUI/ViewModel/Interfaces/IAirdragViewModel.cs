using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace VECTO3GUI.ViewModel.Interfaces
{
	public interface IAirdragViewModel : IComponentViewModel, ICommonComponentParameters
	{
		IAirdragDeclarationInputData ModelData { get; }

		bool UseMeasuredValues { get; set; }
		SquareMeter CdxA_0 { get; set; }
		SquareMeter TransferredCdxA { get; set; }
		SquareMeter DeclaredCdxA { get; set; }
	}
}
