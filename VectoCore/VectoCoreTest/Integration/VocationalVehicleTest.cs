using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestFixture]
	public class VocationalVehicleTest
	{
		const string Class4Vocational = @"Testdata\Integration\DeclarationMode\Class4_Vocational\Rigid Truck_4x2_vehicle-class-4_EURO6_2018.xml";
		const string Class5Vocational = @"Testdata\Integration\DeclarationMode\Class5_Vocational\Tractor_4x2_vehicle-class-5_EURO6_2018.xml";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		[TestCase(Class4Vocational, 1),
			TestCase(Class5Vocational, 2)]
		public void VocationalTest(string filename, int numRuns)
		{
			var writer = new FileOutputWriter(filename);
			var inputData = new XMLDeclarationInputDataProvider(filename, true); //.ReadJsonJob(relativeJobPath);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer) {
				WriteModalResults = true,
				ActualModalData = true
			};
			var jobContainer = new JobContainer(new MockSumWriter());

			var runs = factory.SimulationRuns().ToList();
			Assert.AreEqual(numRuns, runs.Count);
			foreach (var run in runs) {
				jobContainer.AddRun(run);
			}
			//jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat<Exception>(progress.Select(r => r.Value.Error)));
		}
	}
}
