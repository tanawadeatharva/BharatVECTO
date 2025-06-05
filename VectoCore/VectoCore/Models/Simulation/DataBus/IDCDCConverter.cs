using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;

namespace TUGraz.VectoCore.Models.Simulation.DataBus
{
	public interface IDCDCConverter : IElectricAuxPort, IElectricAuxConnector, IUpdateable
	{
		void ConsumerEnergy(WattSecond electricConsumerPower, bool dryRun);
	}
}