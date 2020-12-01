using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public interface IHybridControllerInfo
	{
		GearInfo SelectedGear { get; }
		
		PerSecond ICESpeed { get; }
		bool GearboxEngaged { get; }


		Second SimulationInterval { get; }

		PerSecond ElectricMotorSpeed(PowertrainPosition pos);

		//IList<PowertrainPosition> ElectricMotors { get; }

		//NewtonMeter ElectricMotorTorque(PowertrainPosition pos);
	}

	public interface IHybridControllerCtl
	{
		void RepeatDrivingAction(Second absTime);
	}
}