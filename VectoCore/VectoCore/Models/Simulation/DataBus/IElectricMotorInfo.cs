using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.Simulation.DataBus
{
	public interface IElectricMotorInfo
	{
		//NewtonMeter ElectricDragTorque(PerSecond electricMotorSpeed, Second simulationInterval, DrivingBehavior drivingBehavior);

		PerSecond ElectricMotorSpeed { get; }
		PowertrainPosition Position { get; }
	}
}