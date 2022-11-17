using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoMockup.Simulation
{
    public class MockupRun : VectoRun
	{
		private IVehicleContainer _vehicleContainer;
		public MockupRun(IVehicleContainer container) : base(container)
		{
			_vehicleContainer = container;
			
			
		}


		#region Overrides of VectoRun
		public override double Progress => 1;

		protected override IResponse DoSimulationStep()
		{
			FinishedWithoutErrors = true;

			return new ResponseCycleFinished(this);
		}

		public override bool CalculateAggregateValues()
		{
			return false;
		}

		protected override bool CheckCyclePortProgress()
		{
			return false;
		}

		protected override IResponse Initialize()
		{
			return new ResponseSuccess(this);
		}

		#endregion
	}
}
