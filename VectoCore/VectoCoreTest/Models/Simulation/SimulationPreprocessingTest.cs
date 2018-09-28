using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;

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

		[TestCase(Class9Decl, 20, 2, 18.9937),
		 TestCase(Class9Decl, 20, 3, 18.6561),
		 TestCase(Class9Decl, 20, 2.5, 18.8248),
		 TestCase(Class9Decl, 25, 0, 24.6687),
		 TestCase(Class9Decl, 25, 1, 24.3307),
		 TestCase(Class9Decl, 25, 0.5, 24.4997),
		 TestCase(Class9Decl, 87, -4, 87.6204),
		 TestCase(Class9Decl, 87, -5, 87.9566),
		 TestCase(Class9Decl, 87, -4.65, 87.8390)
			]
		public void TestSimulationPreprocessingVelocityDuringTractionInterruption(string jobFile, double vPre, double grad, double vPost)
		{
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider =  JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter) {
				ModalResults1Hz = false,
				WriteModalResults = true,
				ActualModalData = false,
				Validate = false,
			};

			jobContainer.AddRuns(runsFactory);

			
			var i = 0;
			//jobContainer.Runs[i].Run.Run();
			
			var lookup = SimulationRunPreprocessingVelocityTractionInterruption(jobContainer.Runs[i].Run);

			var velocityDrop = lookup.Interpolate(vPre.KMPHtoMeterPerSecond(), VectoMath.InclinationToAngle(grad / 100.0));
			Assert.AreEqual(vPost, velocityDrop.AsKmph, 1e-3);


			//var rnd = new Random(99);
			//var t = Stopwatch.StartNew();
			//for (var j = 0; j < 100000; j++) {
			//	var vEnd = lookup.Interpolate(
			//		(5 + rnd.NextDouble() * 110).KMPHtoMeterPerSecond(),
			//		VectoMath.InclinationToAngle((rnd.NextDouble() * 20 - 10) / 100.0));
			//}
			//t.Stop();
			//Console.WriteLine(t.ElapsedMilliseconds);

		}

		
		protected virtual VelocityRollingLookup SimulationRunPreprocessingVelocityTractionInterruption(IVectoRun run)
		{
			var data = run.GetContainer().RunData;
			var modData = new ModalDataContainer(data, null, null, false);
			var builder = new PowertrainBuilder(modData);
			var simpleContainer = new SimplePowertrainContainer(data);
			builder.BuildSimplePowertrain(data, simpleContainer);

			var tmp = new VelocityRollingLookup();
			var preprocessor = new VelocitySpeedGearshiftPreprocessor(tmp, 1.SI<Second>(),simpleContainer,  minGradient: -12, maxGradient: 12);
			var t = Stopwatch.StartNew();

			preprocessor.RunPreprocessing();
			t.Stop();
			//Console.WriteLine(t.ElapsedMilliseconds);

			t = Stopwatch.StartNew();
			t.Stop();
			//Console.WriteLine(t.ElapsedMilliseconds);

			return tmp;
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


			var i = 1;
			//jobContainer.Runs[i].Run.Run();

			var lookup = SimulationRunPreprocessingGradability(jobContainer.Runs[i].Run);

			foreach (var tuple in lookup._data) {
				Console.WriteLine("gear: {0}, maxTorque gradability: {1}, redTorque gradeabitlity: {2}", tuple.Key, tuple.Value.Item1, tuple.Value.Item2);
			}

			Assert.AreEqual(0.2004, lookup._data[4].Item1.Value(), 1e-3);
			Assert.AreEqual(0.1225, lookup._data[4].Item2.Value(), 1e-3);

			Assert.AreEqual(0.0710, lookup._data[8].Item1.Value(), 1e-3);
			Assert.AreEqual(0.0719, lookup._data[8].Item2.Value(), 1e-3);

			Assert.AreEqual(0.0187, lookup._data[12].Item1.Value(), 1e-3);
			Assert.AreEqual(0.0176, lookup._data[12].Item2.Value(), 1e-3);

		}


		protected virtual MaxGradabilityLookup SimulationRunPreprocessingGradability(IVectoRun run)
		{
			var data = run.GetContainer().RunData;
			var modData = new ModalDataContainer(data, null, null, false);
			var builder = new PowertrainBuilder(modData);
			var simpleContainer = new SimplePowertrainContainer(data);
			builder.BuildSimplePowertrain(data, simpleContainer);

			var tmp = new MaxGradabilityLookup();
			var preprocessor = new MaxGradabilityPreprocessor(tmp, run.GetContainer().RunData, simpleContainer);
			var t = Stopwatch.StartNew();

			preprocessor.RunPreprocessing();
			t.Stop();
			//Console.WriteLine(t.ElapsedMilliseconds);

			t = Stopwatch.StartNew();
			t.Stop();
			//Console.WriteLine(t.ElapsedMilliseconds);

			return tmp;
		}


		[TestCase(Class9Decl),
		]
		public void TestSimulationPreprocessingEngineSpeedDriveOff(string jobFile)
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


			var i = 1;
			//jobContainer.Runs[i].Run.Run();

			var lookup = SimulationRunPreprocessingEngineSpeedDriveOff(jobContainer.Runs[i].Run);

			foreach (var tuple in lookup) {
				Console.WriteLine("gear: {0}, engineSpeed: {1}", tuple.Key, tuple.Value);
			}

			//Assert.AreEqual(0.2004, lookup._data[4].Item1.Value(), 1e-3);
			//Assert.AreEqual(0.1225, lookup._data[4].Item2.Value(), 1e-3);

			//Assert.AreEqual(0.0710, lookup._data[8].Item1.Value(), 1e-3);
			//Assert.AreEqual(0.0719, lookup._data[8].Item2.Value(), 1e-3);

			//Assert.AreEqual(0.0187, lookup._data[12].Item1.Value(), 1e-3);
			//Assert.AreEqual(0.0176, lookup._data[12].Item2.Value(), 1e-3);

		}


		protected virtual Dictionary<uint, PerSecond> SimulationRunPreprocessingEngineSpeedDriveOff(IVectoRun run)
		{
			var data = run.GetContainer().RunData;
			var modData = new ModalDataContainer(data, null, null, false);
			var builder = new PowertrainBuilder(modData);
			var simpleContainer = new SimplePowertrainContainer(data);
			builder.BuildSimplePowertrain(data, simpleContainer);

			var tmp = new Dictionary<uint, PerSecond>();
			var preprocessor = new EngineSpeedDriveOffPreprocessor(tmp, run.GetContainer().RunData, simpleContainer);
			var t = Stopwatch.StartNew();

			preprocessor.RunPreprocessing();
			t.Stop();
			//Console.WriteLine(t.ElapsedMilliseconds);

			t = Stopwatch.StartNew();
			t.Stop();
			//Console.WriteLine(t.ElapsedMilliseconds);

			return tmp;
		}
	}
}
