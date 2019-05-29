using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation
{
	public interface ISimulatorFactoryFactory
	{
		ISimulatorFactory Factory(
			ExecutionMode mode, IInputDataProvider dataProvider, IOutputDataWriter writer,
			IDeclarationReport declarationReport = null, IVTPReport vtpReport = null, bool validate = true);
	}
}
