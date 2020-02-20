using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using VECTO3.Util;

namespace VECTO3.ViewModel.Interfaces {
	public interface IAxleViewModel : IComponentViewModel
	{
		IAxleDeclarationInputData ModelData { get; }

		int AxleIndex { get; }
		AxleType AxleType { get; set; }
		bool TwinTyres { get; set; }
		bool Steered { get; set; }
		ITyreViewModel Tyre { get; }
		AllowedEntry<AxleType>[] AllowedAxleTypes { get; }
		
	}
}