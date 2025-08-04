using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Mockup.Simulation
{

	public class MockupPowertrainBuilder : IPowertrainBuilder
	{
		#region Implementation of IPowertrainBuilder

		public IVehicleContainer Build(VectoRunData data, IModalDataContainer modData, ISumData sumWriter = null)
		{
			return new VehicleContainer(data, modData, sumWriter, null);
		}

		public IExemptedVehicleContainer BuildExempted(VectoRunData data)
		{
			return new ExemptedVehicleContainer(data, null, null, null);
		}

		#endregion
	}
}