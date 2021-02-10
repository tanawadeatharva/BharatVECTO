using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Models.EngineeringMode
{
	[TestFixture]
	public class EngineeringModeBusAuxTest
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

		const string JobFile = @"TestData\Integration\Buses\EngineeringMode\InterurbanBus_ENG_BusAux.vecto";
		const string JobFile_SmartES = @"TestData\Integration\Buses\EngineeringMode\InterurbanBus_ENG_BusAux_SmartES.vecto";
		const string JobFile_SmartPS = @"TestData\Integration\Buses\EngineeringMode\InterurbanBus_ENG_BusAux_SmartPS.vecto";
		const string JobFile_SmartES_SmartPS = @"TestData\Integration\Buses\EngineeringMode\InterurbanBus_ENG_BusAux_SmartES-SmartPS.vecto";

		[
		TestCase(JobFile, 0, TestName = "InterurbanBus ENG BusAux NonSmart Interurban"),
		TestCase(JobFile, 1, TestName = "InterurbanBus ENG BusAux NonSmart Coach"),
		TestCase(JobFile, 2, TestName = "InterurbanBus ENG BusAux NonSmart Urban"),
		TestCase(JobFile, 3, TestName = "InterurbanBus ENG BusAux NonSmart Suburban"),
		TestCase(JobFile, 4, TestName = "InterurbanBus ENG BusAux NonSmart HeavyUrban"),

		TestCase(JobFile_SmartES, 0, TestName = "InterurbanBus ENG BusAux Smart-ES Interurban"),
		TestCase(JobFile_SmartES, 1, TestName = "InterurbanBus ENG BusAux Smart-ES Coach"),
		TestCase(JobFile_SmartES, 2, TestName = "InterurbanBus ENG BusAux Smart-ES Urban"),
		TestCase(JobFile_SmartES, 3, TestName = "InterurbanBus ENG BusAux Smart-ES Suburban"),
		TestCase(JobFile_SmartES, 4, TestName = "InterurbanBus ENG BusAux Smart-ES HeavyUrban"),

		TestCase(JobFile_SmartPS, 0, TestName = "InterurbanBus ENG BusAux Smart-PS Interurban"),
		TestCase(JobFile_SmartPS, 1, TestName = "InterurbanBus ENG BusAux Smart-PS Coach"),
		TestCase(JobFile_SmartPS, 2, TestName = "InterurbanBus ENG BusAux Smart-PS Urban"),
		TestCase(JobFile_SmartPS, 3, TestName = "InterurbanBus ENG BusAux Smart-PS Suburban"),
		TestCase(JobFile_SmartPS, 4, TestName = "InterurbanBus ENG BusAux Smart-PS HeavyUrban"),

		TestCase(JobFile_SmartES_SmartPS, 0, TestName = "InterurbanBus ENG BusAux Smart-ES Smart-PS Interurban"),
		TestCase(JobFile_SmartES_SmartPS, 1, TestName = "InterurbanBus ENG BusAux Smart-ES Smart-PS Coach"),
		TestCase(JobFile_SmartES_SmartPS, 2, TestName = "InterurbanBus ENG BusAux Smart-ES Smart-PS Urban"),
		TestCase(JobFile_SmartES_SmartPS, 3, TestName = "InterurbanBus ENG BusAux Smart-ES Smart-PS Suburban"),
		TestCase(JobFile_SmartES_SmartPS, 4, TestName = "InterurbanBus ENG BusAux Smart-ES Smart-PS HeavyUrban"),
		]
		public void InterurbanBus_BusAuxTest(string jobFile, int runIdx)
		{
			var writer = new FileOutputWriter(jobFile);
			var inputData = Path.GetExtension(jobFile) == ".xml"
				? xmlInputReader.CreateDeclaration(jobFile)
				//? new XMLDeclarationInputDataProvider(relativeJobPath, true)
				: JSONInputDataFactory.ReadJsonJob(jobFile);

			var sumContainer = new SummaryDataContainer(writer);
			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputData, writer) {
				WriteModalResults = true,
                SumData = sumContainer,
                //ActualModalData = true,
                Validate = false
			};

			var jobContainer = new JobContainer(sumContainer);

			var run = factory.SimulationRuns().ToArray()[runIdx];

			Assert.NotNull(run);

			jobContainer.AddRun(run);

			var pt = run.GetContainer();
			Assert.NotNull(pt);

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			jobContainer.Execute();
			jobContainer.WaitFinished();
		}
	}
}