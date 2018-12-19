using System;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation.Impl {
	internal class ExemptedRun : VectoRun
	{
		private Action<ModalDataContainer> _writeSumData;

		public ExemptedRun(VehicleContainer data, Action<ModalDataContainer> writeSumData) : base(data)
		{
			_writeSumData = writeSumData;
		}

		#region Overrides of VectoRun

		public override double Progress { get { return 1; } }

		public override string CycleName { get { return "ExemptedVehicle"; } }

		public override string RunSuffix { get { return ""; } }

		protected override IResponse DoSimulationStep()
		{
			FinishedWithoutErrors = true;
			_writeSumData(null);
			return new ResponseCycleFinished();
		}

		protected override IResponse Initialize()
		{
			return new ResponseSuccess();
		}

		#endregion
	}
}