using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent {
	public interface IHybridController : IPowerTrainComponent
	{
		IShiftStrategy ShiftStrategy { get; }

		IElectricMotorControl ElectricMotorControl(PowertrainPosition pos);
		void AddElectricMotor(PowertrainPosition pos, ElectricMotorData motorDataItem2);
	}
}