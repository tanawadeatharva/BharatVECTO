using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation.DataBus;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public interface IElectricMotor : IPowerTrainComponent, IElectricMotorInfo
	{
		void Connect(IElectricSystem powersupply);
	}
}