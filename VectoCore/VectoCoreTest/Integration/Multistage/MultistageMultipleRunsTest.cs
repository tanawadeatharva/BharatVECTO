using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration.Multistage
{
	[TestFixture]
	public class MultistageMultipleRunsTest
	{
		private const string TestDataDir = "TestData\\Integration\\Multistage\\";

		private const string CompletedDiesel = TestDataDir + "newVifCompletedDiesel.json";
		private const string CompletedExempted = TestDataDir + "newVifExempted.json";

		private const string InterimExempted = TestDataDir + "newVifExemptedIncomplete.json";
		private const string InterimDiesel = TestDataDir + "newVifInterimDiesel.json";

		private FileOutputWriter _sumFileWriter;
		private SummaryDataContainer _sumContainer;
		private JobContainer _jobContainer;
		private ExecutionMode _mode = ExecutionMode.Declaration;

		private string _outputDirectory;

		private Stopwatch _stopWatch;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[SetUp]
		public void SetUp()
		{
			_stopWatch = new Stopwatch();
			_stopWatch.Start();
			_outputDirectory = TestContext.CurrentContext.TestDirectory + TestContext.CurrentContext.Test.Name;
			_sumFileWriter = new FileOutputWriter(_outputDirectory);
			_sumContainer = new SummaryDataContainer(_sumFileWriter);
			_jobContainer = new JobContainer(_sumContainer);
		}

		[TearDown]
		public void TearDown()
		{
			_stopWatch.Stop();
			TestContext.WriteLine($"Execution time: {_stopWatch.Elapsed}");
		}

		[Test]//, Timeout(3000)]
		public void ExemptedPrimaryAndCompletedTest()
		{
			var inputFile = Path.GetFullPath(CompletedExempted);
			var input = JSONInputDataFactory.ReadJsonJob(inputFile);
			var fileWriter = StartSimulation(input);


			while (!_jobContainer.AllCompleted) {
				//Busy wait
			}

			var writtenFiles = fileWriter.GetWrittenFiles();
			ShowWrittenFiles(fileWriter.GetWrittenFiles());

			Assert.That(writtenFiles.ContainsKey(ReportType.DeclarationReportManufacturerXML));
			Assert.That(writtenFiles.ContainsKey(ReportType.DeclarationReportCustomerXML));
			Assert.That(writtenFiles.ContainsKey(ReportType.DeclarationReportMultistageVehicleXML));
			
		}

		
		[Test]//, Timeout(3000)]
		public void ExemptedPrimaryAndInterimTest()
		{
			var inputFile = Path.GetFullPath(InterimExempted);
			var input = JSONInputDataFactory.ReadJsonJob(inputFile);
			var fileWriter = StartSimulation(input);


			while (!_jobContainer.AllCompleted)
			{
				//Busy wait
			}

			var writtenFiles = fileWriter.GetWrittenFiles();
			ShowWrittenFiles(fileWriter.GetWrittenFiles());

			Assert.That(writtenFiles.ContainsKey(ReportType.DeclarationReportManufacturerXML));
			Assert.That(writtenFiles.ContainsKey(ReportType.DeclarationReportCustomerXML));
			Assert.That(writtenFiles.ContainsKey(ReportType.DeclarationReportMultistageVehicleXML));
		}

		//SpecialCase II
		[Test, Timeout(1000 * 10 * 60)]
		public void PrimaryAndCompletedTest()
		{
			var inputFile = Path.GetFullPath(CompletedDiesel);
			var input = JSONInputDataFactory.ReadJsonJob(inputFile);
			var fileWriter = StartSimulation(input);


			while (!_jobContainer.AllCompleted)
			{
				//Busy wait
			}

			var writtenFiles = fileWriter.GetWrittenFiles();
			ShowWrittenFiles(fileWriter.GetWrittenFiles());

			Assert.That(writtenFiles.ContainsKey(ReportType.DeclarationReportManufacturerXML));
			Assert.That(writtenFiles.ContainsKey(ReportType.DeclarationReportCustomerXML));
			Assert.That(writtenFiles.ContainsKey(ReportType.DeclarationReportMultistageVehicleXML));


		}

		//SpecialCase I
		[Test, Timeout(1000 * 10 * 60)]
		public void PrimaryAndInterimTest()
		{
			var inputFile = Path.GetFullPath(InterimDiesel);
			var input = JSONInputDataFactory.ReadJsonJob(inputFile);
			var fileWriter = StartSimulation(input);


			while (!_jobContainer.AllCompleted)
			{
				//Busy wait
			}

			var writtenFiles = fileWriter.GetWrittenFiles();
			ShowWrittenFiles(fileWriter.GetWrittenFiles());

			Assert.That(writtenFiles.ContainsKey(ReportType.DeclarationReportManufacturerXML));
			Assert.That(writtenFiles.ContainsKey(ReportType.DeclarationReportCustomerXML));
			Assert.That(writtenFiles.ContainsKey(ReportType.DeclarationReportMultistageVehicleXML));
		}


		private FileOutputWriter StartSimulation(IInputDataProvider input)
		{
			var fileWriter = new FileOutputWriter(_outputDirectory);
			var runsFactory = new SimulatorFactory(_mode, input, fileWriter)
			{
				WriteModalResults = true,
				ModalResults1Hz = true,
				Validate = true,
				ActualModalData = true,
				SerializeVectoRunData = true,
			};

			var timeout = 1000;


			_jobContainer.AddRuns(runsFactory);
			_jobContainer.Execute();
			return fileWriter;
		}

		private void ShowWrittenFiles(IDictionary<ReportType, string> getWrittenFiles)
		{
			if (getWrittenFiles.Count == 0)
			{
				TestContext.WriteLine("No Files Written");
			}
			foreach (var keyValuePair in getWrittenFiles)
			{
				TestContext.WriteLine(keyValuePair.Key.ToString());
				TestContext.WriteLine(keyValuePair.Value.ToString());
			}
		}



	}
}