using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class VTPGearbox : CycleGearbox
	{
		public VTPGearbox(IVehicleContainer container, VectoRunData runData) : base(container, runData) { }

		protected override uint GetGearFromCycle()
		{
			return DataBus.CycleData.LeftSample.Gear;
		}

		public override bool ClutchClosed(Second absTime)
		{
			return DataBus.CycleData.LeftSample.Gear != 0;
		}
	}
}