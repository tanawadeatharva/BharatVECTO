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
			if (File.Exists("job-report.vsum")) {
				File.Delete("job-report.vsum");
			}

			if (File.Exists("job-report.pdf")) {
				File.Delete("job-report.pdf");
			}

			var fileWriter = new FileOutputWriter("job-report", "");
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);
			var inputData = JSONInputDataFactory.ReadJsonJob(@"TestData\Jobs\job-report.vecto");
			var factory = new SimulatorFactory(SimulatorFactory.FactoryMode.DeclarationMode, inputData, fileWriter);

			jobContainer.AddRuns(factory);
			jobContainer.Execute();

			jobContainer.WaitFinished();

			Assert.IsTrue(File.Exists(@"job-report.vsum"));
			Assert.IsTrue(File.Exists(@"TestData\Jobs\job-report.pdf"));
		}
	}
}