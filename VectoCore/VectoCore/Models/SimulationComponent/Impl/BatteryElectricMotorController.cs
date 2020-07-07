using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {
	public class BatteryElectricMotorController : IElectricMotorControl
	{
		private VehicleContainer DataBus;
		private ElectricSystem ElectricSystem;

		public BatteryElectricMotorController(VehicleContainer container, ElectricSystem es)
		{
			DataBus = container;
			ElectricSystem = es;
		}

		#region Implementation of IElectricMotorControl

		public NewtonMeter MechanicalAssistPower(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond prevOutAngularVelocity, PerSecond currOutAngularVelocity,
			PowertrainPosition position, bool dryRun)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}