using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public interface IElectricMotor : IPowerTrainComponent, ITnOutPort, IElectricMotorInfo, IUpdateable
	{
		void Connect(IElectricSystem powersupply);

		BusAuxiliariesAdapter BusAux { set; }
	}

	public interface ITestpowertrainElectricMotor : IElectricMotor, ITnOutPort
	{
		IElectricMotorControl Control { get; }
		IElectricSystem GetElectricSystem { get; }
	}

	public interface ITestPowertrainElectricMotorControl : IElectricMotorControl
	{
		bool EmOff { get; set; }
	}
}