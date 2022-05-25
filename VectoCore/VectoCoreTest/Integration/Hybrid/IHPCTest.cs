using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration.Hybrid;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class IHPCTest
{
	[OneTimeSetUp]
	public void RunBeforeAnyTests()
	{
		Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		//InitGraphWriter();
	}

	public const string IHPCTEst = @"TestData\Hybrids\GenericIHPC\12SpeedGbx\IHPC Group 5.vecto";

	[
		TestCase(IHPCTEst, 0, TestName = "IHPC Group 5 DriveCycle LongHaul"),
		TestCase(IHPCTEst, 1, TestName = "IHPC Group 5 DriveCycle Coach"),
		TestCase(IHPCTEst, 2, TestName = "IHPC Group 5 DriveCycle Construction"),
		TestCase(IHPCTEst, 3, TestName = "IHPC Group 5 DriveCycle HeavyUrban"),
		TestCase(IHPCTEst, 4, TestName = "IHPC Group 5 DriveCycle Interurban"),
		TestCase(IHPCTEst, 5, TestName = "IHPC Group 5 DriveCycle MunicipalUtility"),
		TestCase(IHPCTEst, 6, TestName = "IHPC Group 5 DriveCycle RegionalDelivery"),
		TestCase(IHPCTEst, 7, TestName = "IHPC Group 5 DriveCycle Suburban"),
		TestCase(IHPCTEst, 8, TestName = "IHPC Group 5 DriveCycle Urban"),
		TestCase(IHPCTEst, 9, TestName = "IHPC Group 5 DriveCycle UrbanDelivery"),
	]
	public void P2HybridGroup5DriveCycle(string jobFile, int cycleIdx)
	{ RunHybridJob(jobFile, cycleIdx); }

	// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
	public void RunHybridJob(string jobFile, int cycleIdx, int? startDistance = null)
	{
		var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

		var writer = new FileOutputWriter(jobFile);
		var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, writer);
		factory.Validate = false;
		factory.WriteModalResults = true;

		var sumContainer = new SummaryDataContainer(writer);
		var jobContainer = new JobContainer(sumContainer);

		factory.SumData = sumContainer;

		var run = factory.SimulationRuns().ToArray()[cycleIdx];

		if (startDistance != null) {
			//(run.GetContainer().BatteryInfo as Battery).PreviousState.StateOfCharge = 0.317781;
			(run.GetContainer().RunData.Cycle as DrivingCycleProxy).Entries = run.GetContainer().RunData.Cycle
				.Entries.Where(x => x.Distance >= startDistance.Value).ToList();
			//run.GetContainer().MileageCounter.Distance = startDistance;
			//run.GetContainer().DrivingCycleInfo.CycleStartDistance = startDistance;
		}

		Assert.NotNull(run);

		var pt = run.GetContainer();

		Assert.NotNull(pt);

		run.Run();
		Assert.IsTrue(run.FinishedWithoutErrors);

		//jobContainer.AddRuns(factory);
		//jobContainer.Execute();
		//jobContainer.WaitFinished();
		//Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
	}
}