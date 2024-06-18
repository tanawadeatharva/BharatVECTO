using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces
{
    public interface IRetarderDataAdapter
    {
        RetarderData CreateRetarderData(IRetarderInputData retarder, ArchitectureID architecture,
			IIEPCDeclarationInputData iepcInputData);
    }

	public interface IGenericRetarderDataAdapter : IRetarderDataAdapter
	{
		RetarderData CreateGenericRetarderData(IRetarderInputData retarder, VectoRunData vehicleData);
	}
}