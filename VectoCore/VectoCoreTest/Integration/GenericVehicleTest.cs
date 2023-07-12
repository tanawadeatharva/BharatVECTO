using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration
{
    /// <summary>
    /// Make sure all shipped generic vehicles actually can be simulated
    /// </summary>
	[TestFixture]
	//[Parallelizable]
	internal class GenericVehicleTest
    {
		private static string BASE_DIR = "TestData/Shipped_Generic";
		private StandardKernel _kernel;
		private IXMLInputDataReader _xmlReader;

		private bool _simulate = false;
		private static HashSet<string> _ignoredFiles = new HashSet<string>()
		{
			@"Engineering Mode\GenericVehicleE2\BEV_ENG_PTO_invalid.vecto", //INVALID Input file 
			@"Declaration Mode\CompletedBus 31b2\airdrag.xml" //no job
        };

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			_kernel = new StandardKernel(new VectoNinjectModule());
			_xmlReader = _kernel.Get<IXMLInputDataReader>();
			
		}


		[Test, TestCaseSource(nameof(GetJSONEngineering))]
		public void GenericVehiclesEngineering(string path)
		{
			RunJsonJob(path, ExecutionMode.Engineering);
		}


		private void RunJsonJob(string path, ExecutionMode executionMode)
		{
			TestContext.Progress.WriteLine($"Running {path} ...");
			var writeReports = true;
			var inputData = JSONInputDataFactory.ReadJsonJob(path, false);

			var fileWriter = new FileOutputWriter(path);
			var runsFactory = SimulatorFactory.CreateSimulatorFactory(executionMode, inputData, fileWriter,
				writeReports ? null : new NullDeclarationReport()); //, writeReports ? null : new NullDeclarationReport());
			//DisableIterativeRuns(runsFactory);
			runsFactory.WriteModalResults = false;
			var sumWriter = new SummaryDataContainer(fileWriter); //new MockSumWriter();

			var jobContainer = new JobContainer(sumWriter);
			runsFactory.SumData = sumWriter;
			//var sumDataContainer = sumWriter;

			jobContainer.AddRuns(runsFactory);
			if (_simulate) {
				jobContainer.Execute(true);
				jobContainer.WaitFinished();
				Assert.IsTrue(jobContainer.Runs.All(r => r.Success));
			}


        }


        [Test, TestCaseSource(nameof(GetJSONDeclaration)), TestCaseSource(nameof(GetXMLDeclaration))]
		public void GenericVehiclesDeclaration(string path)
		{
			if (Ignore(path)) {
				Assert.Ignore("File should not be simulated");
			}
			if (path.EndsWith(".vecto")) {
				RunJsonJob(path, ExecutionMode.Declaration);
			} else {
				TestContext.Progress.WriteLine($"Running {path} ...");
                var writeReports = true;
				var inputData = _xmlReader.CreateDeclaration(path);
				var fileWriter = new FileOutputWriter(path);
				var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter,
					writeReports ? null : new NullDeclarationReport()); //, writeReports ? null : new NullDeclarationReport());
				var sumWriter = new SummaryDataContainer(fileWriter); //new MockSumWriter();
				runsFactory.WriteModalResults = false;
				var jobContainer = new JobContainer(sumWriter);
				runsFactory.SumData = sumWriter;
				//var sumDataContainer = sumWriter;

				jobContainer.AddRuns(runsFactory);
				if (_simulate) {
					jobContainer.Execute(true);
					jobContainer.WaitFinished();
					Assert.IsTrue(jobContainer.Runs.All(r => r.Success));
				}
			}

        }


		public static List<string> GetJSONEngineering()
		{
			return GetFiles("Engineering Mode", "*.vecto");
		}

		public static List<string> GetJSONDeclaration()
		{
			return GetFiles("Declaration Mode", "*.vecto");
        }

		public static List<string> GetXMLDeclaration()
		{
			return GetFiles("Declaration Mode", "*.xml");
		}
		private static List<string> GetFiles(string path, string searchPattern)
		{
			var dirPath = Path.Combine(BASE_DIR, path);
			List<string> vectoJobs = new List<string>();
			foreach (var fileName in Directory.EnumerateFiles(dirPath, searchPattern, SearchOption.AllDirectories))
			{
				if (Ignore(path)) {
					continue;
				}
			
				vectoJobs.Add(fileName);
			}


			return vectoJobs;
		}


		static bool Ignore(string path)
		{
			if (_ignoredFiles.Contains(Path.GetRelativePath(BASE_DIR, path))) {
				return true;

			}

			if (path.Contains("RSLT_MANUFACTURER") || path.Contains("RSLT_CUSTOMER")) {
				return true;
			}

			return false;
		}
    }
}
