using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Tests.Models.Simulation;

namespace TUGraz.VectoCore.Tests.Integration.SimulationRuns
{
	[TestFixture]
	public class TestSimulationRuns
	{
		[Category("UserBugs"),
		TestCase("Kies-20161115"),
		TestCase("Mandl-20161115"),
		TestCase("Silberholz-20161121")]
		public static void RunJob_Eng(string jobName)
		{
			var inputData = JSONInputDataFactory.ReadJsonJob("TestData\\Bugs\\" + jobName + "\\job.vecto");
			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputData, null);
			var jobContainer = new JobContainer(new MockSumWriter());
			jobContainer.AddRuns(factory);
			jobContainer.Execute();
			jobContainer.WaitFinished();
			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}
	}
}