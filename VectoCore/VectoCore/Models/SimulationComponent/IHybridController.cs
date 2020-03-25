using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.Models.SimulationComponent {
	public interface IHybridController : IPowerTrainComponent
	{
		IShiftStrategy ShiftStrategy { get; }

		IElectricMotorControl ElectricMotorControl(PowertrainPosition pos);
		void AddElectricMotor(PowertrainPosition pos);
	}
}