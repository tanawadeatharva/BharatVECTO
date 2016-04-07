using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class MockBrakes : VectoSimulationComponent, IBrakes
	{
		public MockBrakes(IVehicleContainer vehicle) : base(vehicle)
		{
			BrakePower = 0.SI<Watt>();
		}

		public Watt BrakePower { get; set; }

		protected override void DoWriteModalResults(IModalDataContainer container) {}

		protected override void DoCommitSimulationStep() {}
	}
}