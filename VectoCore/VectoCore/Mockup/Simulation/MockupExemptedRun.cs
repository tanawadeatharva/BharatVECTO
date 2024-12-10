using System;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoMockup.Simulation
{
	internal class MockupExemptedRun : ExemptedRun
	{
		public MockupExemptedRun(IExemptedVehicleContainer data, Action<ModalDataContainer> writeSumData) : base(data, writeSumData) { }

		#region Overrides of ExemptedRun

		protected override void CheckValidInput()
		{
			return;
		}

		#endregion
	}
}