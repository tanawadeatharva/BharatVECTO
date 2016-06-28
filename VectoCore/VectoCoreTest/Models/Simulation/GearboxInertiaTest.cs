using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Tests.Integration;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestFixture]
	public class GearboxInertiaTest
	{
		[Test]
		public void RunWithGearboxInertia()
		{
			var cycleData = "0, 0, 0, 2\n" +
							"1000, 80, 0, 0";

			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);
			var run = CoachPowerTrain.CreateEngineeringRun(cycle, "RunWithGearboxInertia.vmod",
				gearBoxInertia: 0.12.SI<KilogramSquareMeter>());

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}
	}
}