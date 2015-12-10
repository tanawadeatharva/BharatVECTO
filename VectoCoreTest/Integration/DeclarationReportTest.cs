using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestClass]
	public class DeclarationReportTest
	{
		[TestMethod]
		public void DeclarationReport_Test()
		{
			const string jobFile = @"TestData\Jobs\job-report.vecto";

			if (File.Exists(@"TestData\Jobs\job-report.vsum")) {
				File.Delete(@"TestData\Jobs\job-report.vsum");
			}

			if (File.Exists(@"TestData\Jobs\job-report.pdf")) {
				File.Delete(@"TestData\Jobs\job-report.pdf");
			}

			var fileWriter = new FileOutputWriter(jobFile);
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);
			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var factory = new SimulatorFactory(SimulatorFactory.FactoryMode.DeclarationMode, inputData, fileWriter);

			jobContainer.AddRuns(factory);
			jobContainer.Execute();

			jobContainer.WaitFinished();

			Assert.IsTrue(File.Exists(@"TestData\Jobs\job-report.vsum"));
			Assert.IsTrue(File.Exists(@"TestData\Jobs\job-report.pdf"));
		}
	}
}