using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Connector.Ports;

namespace TUGraz.VectoCore.Models.SimulationComponent
{

	public interface ISimpleHybridController : IHybridController, ITnInPort, ITnOutPort
	{
		ITnOutPort NextComponent { get; }

		void ApplyStrategySettings(HybridStrategyResponse cfg);
	}
}