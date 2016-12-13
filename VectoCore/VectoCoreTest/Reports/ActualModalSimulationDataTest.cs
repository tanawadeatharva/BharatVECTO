using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Integration;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Reports
{
	[TestFixture]
	public class ActualModalSimulationDataTest
	{
		[Test]
		public void TestWriteActualModData()
		{
			const string jobFile = @"TestData\Integration\DeclarationMode\40t Truck\40t_Long_Haul_Truck.vecto";
			var fileWriter = new FileOutputWriter(jobFile);
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);

			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter);
			factory.WriteModalResults = true;
			factory.ActualModalData = true;

			jobContainer.AddRuns(factory);
			jobContainer.Execute();
			//jobContainer.Runs[4].Run.Run();

			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.AllCompleted);
		}
	}
}