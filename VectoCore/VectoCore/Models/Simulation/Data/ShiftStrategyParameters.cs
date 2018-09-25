using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ShiftStrategy;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ShiftStrategy;

namespace TUGraz.VectoCore.Models.Simulation.Data {
	public class ShiftStrategyParameters
	{
		public PredictionDurationLookup PredictionDurationLookup { get; internal set; }
		public ShareTorque99lLookup ShareTorque99L { get; internal set; }
	}
}