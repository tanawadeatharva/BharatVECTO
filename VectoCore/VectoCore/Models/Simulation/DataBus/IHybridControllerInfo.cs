using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public interface IHybridControllerInfo
	{
		GearInfo SelectedGear { get; }

		PerSecond ElectricMotorSpeed(PowertrainPosition pos);

		//IList<PowertrainPosition> ElectricMotors { get; }

		//NewtonMeter ElectricMotorTorque(PowertrainPosition pos);
	}
}