using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class GearboxAuxiliary : EngineAuxiliary
	{
		public GearboxAuxiliary(IVehicleContainer container) : base(container) {}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			//todo mk-2016-08-17: write gearbox auxiliaries to mod file?
		}
	}
}