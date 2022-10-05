using System.IO;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;

namespace TUGraz.VectoCore.Tests.Integration.Declaration.HeavyLorry;

[TestFixture]
public class HeavyLorrySimulation
{

	private const string BASE_DIR = @"TestData\Integration\DeclarationMode\V24_DeclarationMode\";
	private StandardKernel _kernel;
	private IXMLInputDataReader _xmlReader;

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		_xmlReader = _kernel.Get<IXMLInputDataReader>();
	}

	[TestCase(@"HeavyLorry\PEV_heavyLorry_AMT_E2_realistic.xml"),
	TestCase(@"HeavyLorry\Conventional_heavyLorry_AMT.xml"),
	TestCase(@"HeavyLorry\Conventional_heavyLorry_AMT.xml", false),
	TestCase(@"HeavyLorry\PEV_heavyLorry_AMT_E2_realistic.xml", false),
	TestCase(@"HeavyLorry\PEV_heavyLorry_E3_realistic.xml"),
	TestCase(@"HeavyLorry\PEV_heavyLorry_E3_realistic.xml", false),
	TestCase(@"HeavyLorry\PEV_heavyLorry_E3_realistic_TorqueLimits.xml"),
	TestCase(@"HeavyLorry\PEV_heavyLorry_E3_realistic_TorqueLimits.xml", false),
	TestCase(@"HeavyLorry\PEV_heavyLorry_E3_realistic_municipal.xml", false),
	TestCase(@"HeavyLorry\PEV_heavyLorry_E3_realistic_municipal.xml"),
	TestCase(@"HeavyLorry\PEV_heavyLorry_E4.xml")]
	public void HeavyLorrySimulationTest(string jobFile, bool multiThreaded = true)
	{
		RunSimulation(jobFile, multiThreaded);
	}


	public void RunSimulation(string jobFile, bool multiThreaded = true)
	{
		var filePath = Path.Combine(BASE_DIR, jobFile);
		var dataProvider = _xmlReader.CreateDeclaration(filePath);
		var fileWriter = new FileOutputWriter(filePath);
		var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter);
		var jobContainer = new JobContainer(new MockSumWriter()) { };
		jobContainer.AddRuns(runsFactory);
		PrintRuns(jobContainer, null);
		jobContainer.Execute(multiThreaded);
		jobContainer.WaitFinished();
		PrintRuns(jobContainer, fileWriter);
		PrintFiles(fileWriter);

	}

	private void PrintRuns(JobContainer jobContainer, FileOutputWriter fileWriter = null)
	{
		foreach (var keyValuePair in jobContainer.GetProgress()) {
			TestContext.WriteLine($"{keyValuePair.Key}: {keyValuePair.Value.CycleName} {keyValuePair.Value.RunName} {keyValuePair.Value.Error?.Message}" );
			//if (fileWriter != null && keyValuePair.Value.Success) {
			//	TestContext.AddTestAttachment(fileWriter.GetModDataFileName(keyValuePair.Value.RunName, keyValuePair.Value.CycleName, keyValuePair.Value.RunSuffix), keyValuePair.Value.RunName);
   //         }
	
		}
	}

	private void PrintFiles(FileOutputWriter fileWriter)
	{
		foreach (var keyValuePair in fileWriter.GetWrittenFiles()) {
			TestContext.WriteLine($"{keyValuePair.Key} written to {keyValuePair.Value}");
			TestContext.AddTestAttachment(keyValuePair.Value, keyValuePair.Key.ToString());
		}
		
	}
}
