namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public interface IHybridControllerInfo
	{
		GearInfo SelectedGear { get; }

		//IList<PowertrainPosition> ElectricMotors { get; }

		//NewtonMeter ElectricMotorTorque(PowertrainPosition pos);
	}
}