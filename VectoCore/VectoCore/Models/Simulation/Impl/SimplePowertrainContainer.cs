using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation.Impl {
	public class SimplePowertrainContainer : VehicleContainer
	{
		public SimplePowertrainContainer(VectoRunData runData, IModalDataContainer modData = null) : base(runData.ExecutionMode, modData)
		{
			RunData = runData;
		}

		public IDriverDemandOutPort VehiclePort
		{
			get { return (Vehicle as Vehicle)?.OutPort(); }
		}

		public ITnOutPort GearboxOutPort
		{
			get { return (Gearbox as IGearbox)?.OutPort(); }
		}

		
		public override Second AbsTime { get { return 0.SI<Second>(); } }
	}
}