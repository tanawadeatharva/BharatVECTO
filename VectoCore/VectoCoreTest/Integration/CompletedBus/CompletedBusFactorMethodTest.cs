using System;
using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration.CompletedBus
{

	[TestFixture()]
	public class CompletedBusFactorMethodTest
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			

		}

		[TestCase()]
		public void TestCompletedBus()
		{
			var relativeJobPath = @"TestData\Integration\Buses\FactorMethod\CompletedBus.vecto";

			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(relativeJobPath), Path.GetFileName(relativeJobPath)));
			var inputData = JSONInputDataFactory.ReadJsonJob(relativeJobPath);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer) {
				WriteModalResults = true,
				//ActualModalData = true,
				Validate = false
			};
			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			jobContainer.AddRuns(factory);
			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();

			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat<Exception>(progress.Select(r => r.Value.Error)));

			//return jobContainer;
		}
	}
}
