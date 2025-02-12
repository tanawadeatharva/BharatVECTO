using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class PEVGearbox : AbstractAMTGearbox, IPEVGearbox
    {
		public PEVGearbox(IVehicleContainer container, IShiftStrategy strategy) : base(container, strategy)
		{
			_gear = new GearshiftPosition(0);
		}

	}
}