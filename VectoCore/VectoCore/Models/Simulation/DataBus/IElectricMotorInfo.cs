using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.Simulation.DataBus
{
	public interface IElectricMotorInfo
	{
		//NewtonMeter ElectricDragTorque(PerSecond electricMotorSpeed, Second simulationInterval, DrivingBehavior drivingBehavior);

		PerSecond ElectricMotorSpeed { get; }
		PowertrainPosition Position { get; }
		PerSecond MaxSpeed { get; }
		Watt DragPower(Volt volt, PerSecond electricMotorSpeed);
		Watt MaxPowerDrive(Volt volt, PerSecond inAngularVelocity);
		NewtonMeter GetTorqueForElectricPower(Volt volt, Watt electricPower, PerSecond avgEmSpeed, Second dt);

		bool DeRatingActive { get; }
	}
}