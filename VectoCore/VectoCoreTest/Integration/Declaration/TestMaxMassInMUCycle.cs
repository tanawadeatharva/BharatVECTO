using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;

namespace TUGraz.VectoCore.Tests.Integration.Declaration
{
	[TestFixture()]
	public class TestMaxMassInMUCycle
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		[TestCase(@"TestData\Integration\TotalMassExceededInMU\Class4_Tractor_DECL.vecto")]
		public void TestMaxMassInMunicipalCycle(string jobFile)
		{
			var relativeJobPath = jobFile;
			var writer = new FileOutputWriter(relativeJobPath);
			var inputData = Path.GetExtension(relativeJobPath) == ".xml" ? new XMLDeclarationInputDataProvider(relativeJobPath, true) : JSONInputDataFactory.ReadJsonJob(relativeJobPath);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer) {
				WriteModalResults = true,
				//ActualModalData = true,
				Validate = false
			};
			var jobContainer = new JobContainer(new MockSumWriter());

			jobContainer.AddRuns(factory);

			var muRefLoadData = jobContainer.Runs.Last().Run.GetContainer().RunData;
			Assert.AreEqual(2700, muRefLoadData.VehicleData.Loading.Value());
			Assert.AreEqual(6000, muRefLoadData.VehicleData.BodyAndTrailerWeight.Value());
			Assert.AreEqual(18000, muRefLoadData.VehicleData.TotalVehicleWeight.Value());

		}
	}
}
