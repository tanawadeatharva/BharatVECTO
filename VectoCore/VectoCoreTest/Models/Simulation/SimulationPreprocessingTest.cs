using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestFixture]
	public class SimulationPreprocessingTest
	{
		public const string Class9Decl =
			@"TestData\Generic Vehicles\Declaration Mode\Class9_RigidTruck_6x2\Class9_RigidTruck_DECL.vecto";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase(Class9Decl),
		]
		public void TestSimulationPreprocessingGradability(string jobFile)
		{
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter) {
				ModalResults1Hz = false,
				WriteModalResults = true,
				ActualModalData = false,
				Validate = false,
			};

			jobContainer.AddRuns(runsFactory);

			foreach (var i in new[] { 0, 1, 4, 5 }) {

				//jobContainer.Runs[i].Run.Run();

				var lookup = SimulationRunPreprocessingEcoRoll(jobContainer.Runs[i].Run);

				Console.WriteLine(string.Format("run: {0} vehicle mass: {1}", jobContainer.Runs[i].Run.RunName, jobContainer.Runs[i].Run.GetContainer().RunData.VehicleData.TotalVehicleWeight));
				foreach (var tuple in lookup.Data) {
					Console.WriteLine("velocity: {0}, slope: {1},", tuple.Key, tuple.Value);
				}
				Console.WriteLine();
			}

			//Assert.AreEqual(0.2004, lookup.Data[4].Item1.Value(), 1e-3);
			//Assert.AreEqual(0.1225, lookup._data[4].Item2.Value(), 1e-3);

			//Assert.AreEqual(0.0710, lookup._data[8].Item1.Value(), 1e-3);
			//Assert.AreEqual(0.0719, lookup._data[8].Item2.Value(), 1e-3);

			//Assert.AreEqual(0.0187, lookup._data[12].Item1.Value(), 1e-3);
			//Assert.AreEqual(0.0176, lookup._data[12].Item2.Value(), 1e-3);

		}

		protected virtual EcoRollSlopeData SimulationRunPreprocessingEcoRoll(IVectoRun run)
		{
			var data = run.GetContainer().RunData;
			var modData = new ModalDataContainer(data, null, new[] {FuelData.Diesel}, null, false);
			var builder = new PowertrainBuilder(modData);
			var simpleContainer = new SimplePowertrainContainer(data);
			builder.BuildSimplePowertrain(data, simpleContainer);

			var tmp = new EcoRollSlopeData();
			var preprocessor = new PCCEcoRollEngineStopPreprocessor(simpleContainer, tmp, 50.KMPHtoMeterPerSecond(), 85.KMPHtoMeterPerSecond());
			var t = Stopwatch.StartNew();

			preprocessor.RunPreprocessing();
			t.Stop();
			Console.WriteLine(t.ElapsedMilliseconds);

			t = Stopwatch.StartNew();
			t.Stop();
			//Console.WriteLine(t.ElapsedMilliseconds);

			return tmp;
		}
	}
}
