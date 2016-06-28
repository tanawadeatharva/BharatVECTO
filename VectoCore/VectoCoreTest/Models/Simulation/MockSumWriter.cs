using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	public class MockSumWriter : SummaryDataContainer
	{
		public override void Write(IModalDataContainer modData, string jobFileName, string jobName,
			string cycleFileName, Kilogram vehicleMass, Kilogram vehicleLoading) {}

		public override void Finish() {}
	}
}