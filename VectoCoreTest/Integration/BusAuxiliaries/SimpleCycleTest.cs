using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TUGraz.VectoCore.Tests.Integration.BusAuxiliaries
{
	[TestClass]
	public class SimpleCycleTest
	{
		[TestMethod]
		public void TestSimpleCycle()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_stop_85_stop_85_level);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAux_DriverStrategy_Drive_stop_85_stop_85_level.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}
	}
}