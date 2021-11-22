using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class APTNGearbox : PEVGearbox
	{
		public APTNGearbox(IVehicleContainer container, IShiftStrategy strategy) : base(container, strategy) {
			ModelData.TractionInterruption = 0.SI<Second>();
		}

		public override void TriggerGearshift(Second absTime, Second dt)
		{
			
		}

		public override bool GearEngaged(Second absTime)
		{
			return true;
		}
	}
}