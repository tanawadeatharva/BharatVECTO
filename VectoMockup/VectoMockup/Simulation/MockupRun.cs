using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation.Impl.Mockup
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
