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

		private FileOutputWriter _fileoutputWriter;
		private TempFileOutputWriter _tempFileOutputWriter;
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
			StartSimulation(input);


			while (!_jobContainer.AllCompleted) {
				//Busy wait
			}

			var writtenFiles = GetWrittenFiles();
			ShowWrittenFiles(writtenFiles);

			Assert.IsTrue(writtenFiles.Contains(_tempFileOutputWriter.XMLFullReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLFullReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLCustomerReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLMultistageReportFileName));
			
		}

		
		[Test]//, Timeout(3000)]
		public void ExemptedPrimaryAndInterimTest()
		{
			var inputFile = Path.GetFullPath(InterimExempted);
			var input = JSONInputDataFactory.ReadJsonJob(inputFile);
			StartSimulation(input);


			while (!_jobContainer.AllCompleted)
			{
				//Busy wait
			}

			var writtenFiles = GetWrittenFiles();
			ShowWrittenFiles(writtenFiles);

			Assert.IsTrue(writtenFiles.Contains(_tempFileOutputWriter.XMLFullReportName));
			Assert.IsFalse(writtenFiles.Contains(_fileoutputWriter.XMLFullReportName));
			Assert.IsFalse(writtenFiles.Contains(_fileoutputWriter.XMLCustomerReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLMultistageReportFileName));

		}

		private IList<string> GetWrittenFiles()
		{
			var files = new List<string>();
			var outputWriters = _jobContainer.GetOutputDataWriters();
			foreach (var outputDataWriter in outputWriters) {
				files.AddRange(outputDataWriter.GetWrittenFiles().Values);
			}

			return files;
		}

		//SpecialCase II
		[Test, Timeout(1000 * 10 * 60)]
		public void PrimaryAndCompletedTest()
		{
			var inputFile = Path.GetFullPath(CompletedDiesel);
			var input = JSONInputDataFactory.ReadJsonJob(inputFile);
			StartSimulation(input);


			while (!_jobContainer.AllCompleted)
			{
				//Busy wait
			}

			var writtenFiles = GetWrittenFiles();
			ShowWrittenFiles(writtenFiles);

			Assert.IsTrue(writtenFiles.Contains(_tempFileOutputWriter.XMLFullReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLFullReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLCustomerReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLMultistageReportFileName));


		}

		//SpecialCase I
		[Test, Timeout(1000 * 10 * 60)]
		public void PrimaryAndInterimTest()
		{
			var inputFile = Path.GetFullPath(InterimDiesel);
			var input = JSONInputDataFactory.ReadJsonJob(inputFile);
			
			
			StartSimulation(input);


			while (!_jobContainer.AllCompleted)
			{
				//Busy wait
			}

			var writtenFiles = GetWrittenFiles();
			ShowWrittenFiles(writtenFiles);

			Assert.IsTrue(writtenFiles.Contains(_tempFileOutputWriter.XMLFullReportName));
			Assert.IsFalse(writtenFiles.Contains(_fileoutputWriter.XMLFullReportName));
			Assert.IsFalse(writtenFiles.Contains(_fileoutputWriter.XMLCustomerReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLMultistageReportFileName));
		}


		private void StartSimulation(IInputDataProvider input)
		{
			_fileoutputWriter = new FileOutputWriter(_outputDirectory);
			_tempFileOutputWriter = new TempFileOutputWriter(_outputDirectory);
			var runsFactory = new SimulatorFactory(_mode, input, _fileoutputWriter)
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
		}

		private void ShowWrittenFiles(IList<string> writtenFiles)
		{
			if (writtenFiles.Count == 0)
			{
				TestContext.WriteLine("No Files Written");
			}
			foreach (var fileName in writtenFiles)
			{
				TestContext.WriteLine(fileName);
			}
		}



	}
}