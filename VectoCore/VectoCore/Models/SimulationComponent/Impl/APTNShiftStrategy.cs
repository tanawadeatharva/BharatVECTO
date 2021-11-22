using TUGraz.VectoCore.Models.Simulation;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class APTNShiftStrategy : PEVAMTShiftStrategy
	{
		public APTNShiftStrategy(IVehicleContainer dataBus) : base(dataBus) { }

		public new static string Name => "APT-N";
	}
}