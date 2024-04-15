using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
    public class ADASDataAdapter : IADASDataAdapter
	{

		public VehicleData.ADASData CreateADAS(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			return new VehicleData.ADASData
			{
				EngineStopStart = adas.EngineStopStart,
				PredictiveCruiseControl = adas.PredictiveCruiseControl,
				InputData = adas,
				EcoRoll = ((adas.PredictiveCruiseControl == PredictiveCruiseControlType.None) 
							&& adas.EcoRoll.WithoutEngineStop())
					? EcoRollType.None
					: adas.EcoRoll,
			};
		}
	}
}