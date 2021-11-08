using TUGraz.VectoCore.Models.Simulation;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class APTNGearbox : PEVGearbox
	{
		public APTNGearbox(IVehicleContainer container, IShiftStrategy strategy) : base(container, strategy) { }
	}
}