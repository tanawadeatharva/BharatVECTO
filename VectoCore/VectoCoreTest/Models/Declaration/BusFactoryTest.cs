using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	[TestFixture]
	public class BusFactoryTest
	{
		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;


		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[TestCase()]
		public void CreateRunDataPrimaryBus()
		{
			var runIdx = 2;
			var jobFile = @"TestData\XML\XMLReaderDeclaration\SchemaVersion2.6_Buses\vecto_vehicle-primary_heavyBus.xml";

			var writer = new FileOutputWriter(jobFile);
			var inputData = Path.GetExtension(jobFile) == ".xml"
				? xmlInputReader.CreateDeclaration(jobFile)
				//? new XMLDeclarationInputDataProvider(relativeJobPath, true)
				: JSONInputDataFactory.ReadJsonJob(jobFile);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer) {
				WriteModalResults = true,
				//ActualModalData = true,
				Validate = false
			};
			var jobContainer = new JobContainer(new MockSumWriter());

			var runs = factory.SimulationRuns().ToArray();
			jobContainer.AddRun(runs[runIdx]);
			runs[runIdx].Run();

			//var run = factory.SimulationRuns().First();
			//run.Run();

			//Assert.IsTrue(runs[runIdx].FinishedWithoutErrors);
		}
	}
}
