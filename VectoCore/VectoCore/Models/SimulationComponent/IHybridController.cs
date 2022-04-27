using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent {
	public interface IHybridController : IPowerTrainComponent, IHybridControllerInfo, IHybridControllerCtl
	{
		IShiftStrategy ShiftStrategy { get; }

		SimpleComponentState PreviousState { get; }

		IElectricMotorControl ElectricMotorControl(PowertrainPosition pos);
		void AddElectricMotor(PowertrainPosition pos, ElectricMotorData motorDataItem2);
		//ResponseDryRun RequestDryRun(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, HybridStrategyResponse strategySettings);
	}
}