using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using VECTO3.Util;

namespace VECTO3.ViewModel.Interfaces {
	public interface ITyreViewModel : IComponentViewModel, ICommonComponentParameters
	{
		ITyreDeclarationInputData ModelData { get; }

		string Dimension { get; set; }
		AllowedEntry<string>[] AllowedDimensions { get; }
		double RollingResistanceCoefficient { get; set; }
		Newton FzISO { get; set; }
	}
}