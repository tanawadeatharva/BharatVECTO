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
	TestCase(@"HeavyLorry\PEV_heavyLorry_E3_realistic.xml"),
	TestCase(@"HeavyLorry\PEV_heavyLorry_E4.xml")]
	public void HeavyLorrySimulationTest(string jobFile)
	{
		RunSimulation(jobFile);
	}


	public void RunSimulation(string jobFile)
	{
		var filePath = Path.Combine(BASE_DIR, jobFile);
		var dataProvider = _xmlReader.CreateDeclaration(filePath);
		var fileWriter = new FileOutputWriter(filePath);
		var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter);
		var jobContainer = new JobContainer(new MockSumWriter()) { };
		jobContainer.AddRuns(runsFactory);
		PrintRuns(jobContainer);
		jobContainer.Execute(true);
		jobContainer.WaitFinished();
		PrintRuns(jobContainer);

	}

	private void PrintRuns(JobContainer jobContainer)
	{
		foreach (var keyValuePair in jobContainer.GetProgress()) {
			TestContext.WriteLine($"{keyValuePair.Key}: {keyValuePair.Value.CycleName} {keyValuePair.Value.RunName} {keyValuePair.Value.Error?.Message}" );
		}
	}
}