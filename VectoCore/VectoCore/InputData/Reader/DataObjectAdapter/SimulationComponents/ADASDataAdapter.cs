using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
	public interface IADASDataAdapter
	{
		VehicleData.ADASData CreateADAS(IAdvancedDriverAssistantSystemDeclarationInputData adas);
	}
	public class ADASDataAdapter : IADASDataAdapter
	{

		public VehicleData.ADASData CreateADAS(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			return new VehicleData.ADASData
			{
				EngineStopStart = adas.EngineStopStart,
				EcoRoll = adas.EcoRoll,
				PredictiveCruiseControl = adas.PredictiveCruiseControl,
				InputData = adas
			};
		}
	}
}