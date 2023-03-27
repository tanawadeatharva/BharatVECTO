using System;
using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;

namespace TUGraz.VectoCore.Tests.Integration.Declaration.PrimaryBus;

public class PrimaryBusSimulation
{

	private const string BASE_DIR = @"TestData\Integration\DeclarationMode\2nd_AmendmDeclMode\";
	private StandardKernel _kernel;
	private IXMLInputDataReader _xmlReader;

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		_xmlReader = _kernel.Get<IXMLInputDataReader>();
	}


    [
	TestCase(@"PrimaryBus/Conventional/primary_heavyBus group41_nonSmart.xml", 0, TestName = "2nd Amendment PrimaryBus Conventional"),
	TestCase(@"PrimaryBus/PEV/PEV_primaryBus_AMT_E2.xml", 0, TestName = "2nd Amendment PrimaryBus PEV E2"),
	]
	public void PrimaryBusSimulationTest(string jobFile, int runIdx)
	{
		RunSimulation(jobFile, runIdx);
	}


	public void RunSimulation(string jobFile, int runIdx)
	{
		var filePath = Path.Combine(BASE_DIR, jobFile);
		var dataProvider = _xmlReader.CreateDeclaration(filePath);
		var fileWriter = new FileOutputWriter(filePath);
		var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter);
		runsFactory.WriteModalResults = true;
		var jobContainer = new JobContainer(new MockSumWriter()) { };
		//var jobContainer = new JobContainer(new MockSumWriter()) { };

		if (runIdx < 0) {
			jobContainer.AddRuns(runsFactory);
		} else {
			var run = runsFactory.SimulationRuns().Skip(runIdx).First();
			jobContainer.AddRun(run);
		}

		PrintRuns(jobContainer, null);

		jobContainer.Execute();
		jobContainer.WaitFinished();
		Assert.IsTrue(jobContainer.AllCompleted);
		Assert.IsTrue(jobContainer.Runs.TrueForAll(runEntry => runEntry.Success));
		PrintRuns(jobContainer, fileWriter);
		PrintFiles(fileWriter);
	}


	private void PrintRuns(JobContainer jobContainer, FileOutputWriter fileWriter = null)
	{
		foreach (var keyValuePair in jobContainer.GetProgress()) {
			TestContext.WriteLine($"{keyValuePair.Key}: {keyValuePair.Value.CycleName} {keyValuePair.Value.RunName} {keyValuePair.Value.Error?.Message}");
			//if (fileWriter != null && keyValuePair.Value.Success) {
			//	TestContext.AddTestAttachment(fileWriter.GetModDataFileName(keyValuePair.Value.RunName, keyValuePair.Value.CycleName, keyValuePair.Value.RunSuffix), keyValuePair.Value.RunName);
			//         }

		}
	}

	private void PrintFiles(FileOutputWriter fileWriter)
	{
		//if (fileWriter.GetWrittenFiles().Count == 0) {
		//	Assert.Fail("No files written\n");
		//}
		foreach (var keyValuePair in fileWriter.GetWrittenFiles()) {
			TestContext.WriteLine($"{keyValuePair.Key} written to {keyValuePair.Value}");
			TestContext.AddTestAttachment(keyValuePair.Value, keyValuePair.Key.ToString());
		}
	}
}