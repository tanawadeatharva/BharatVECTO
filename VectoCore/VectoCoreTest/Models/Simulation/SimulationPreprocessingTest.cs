using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
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
		private StandardKernel _kernel;
		private IXMLInputDataReader xmlInputReader;

		public const string Class9Decl =
			@"TestData\Generic Vehicles\Declaration Mode\Class9_RigidTruck_6x2\Class9_RigidTruck_DECL.vecto";

		public const string Class9DeclAT =
			@"TestData\Integration\ADAS\Group9_AT_PCC.xml";

		public const string Class5Eng = @"TestData\Integration\ADAS\Group5PCCEng\Class5_Tractor_ENG.vecto";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[TestCase(Class9Decl),
		 TestCase(Class9DeclAT)
		]
		public void TestSimulationPreprocessingPccRollSlope(string jobFile)
		{
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = Path.GetExtension(jobFile) == ".xml" ? xmlInputReader.CreateDeclaration(jobFile) : JSONInputDataFactory.ReadJsonJob(jobFile);
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

				Console.WriteLine(string.Format("run: {0}{1} vehicle mass: {2}", jobContainer.Runs[i].Run.RunName, jobContainer.Runs[i].Run.RunSuffix, jobContainer.Runs[i].Run.GetContainer().RunData.VehicleData.TotalVehicleMass));
				foreach (var tuple in lookup) {
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

		[TestCase(Class5Eng),
		]
		public void TestSimulationPreprocessingPccRollSlopeEng(string jobFile)
		{
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(ExecutionMode.Engineering, dataProvider, fileWriter) {
				ModalResults1Hz = false,
				WriteModalResults = true,
				ActualModalData = false,
				Validate = false,
			};

			jobContainer.AddRuns(runsFactory);

			var i = 0;

			//jobContainer.Runs[i].Run.Run();

			var lookup = SimulationRunPreprocessingEcoRoll(jobContainer.Runs[i].Run);

			Console.WriteLine(string.Format("run: {0}{1} vehicle mass: {2}", jobContainer.Runs[i].Run.RunName, jobContainer.Runs[i].Run.RunSuffix, jobContainer.Runs[i].Run.GetContainer().RunData.VehicleData.TotalVehicleMass));
			foreach (var tuple in lookup) {
				Console.WriteLine("velocity: {0}, slope: {1},", tuple.Key, tuple.Value);
			}
			Console.WriteLine();		

		}


		[TestCase(Class9Decl, ExecutionMode.Declaration, 0),
		TestCase(@"TestData\Integration\ADAS\Group5PCCEng\Class5_Tractor_ENG.vecto", ExecutionMode.Engineering, 0),
		TestCase(@"TestData\Integration\ADAS\Group5PCCEng\Class5_Tractor_ENG.vecto", ExecutionMode.Engineering, 1),
		]
		public void TestSimulationPreprocessingPccSegments(string jobFile, ExecutionMode mode, int i)
		{
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(mode, dataProvider, fileWriter) {
				ModalResults1Hz = false,
				WriteModalResults = true,
				ActualModalData = false,
				Validate = false,
			};

			jobContainer.AddRuns(runsFactory);

			//foreach (var i in new[] { 0, 1, 4, 5 }) {

			var segments = SimulationRunPreprocessingPCCSegments(jobContainer.Runs[i].Run);

			Console.WriteLine(string.Format("run: {0}{1} vehicle mass: {2}", jobContainer.Runs[i].Run.RunName, jobContainer.Runs[i].Run.RunSuffix, jobContainer.Runs[i].Run.GetContainer().RunData.VehicleData.TotalVehicleMass));
			foreach (var entry in segments.Segments) {
				Console.WriteLine("x_start: {0}, x_vLow: {1}, x_end: {2}", entry.StartDistance, entry.DistanceMinSpeed, entry.EndDistance);
			}
			Console.WriteLine();

			Console.WriteLine("s,pcc");
			Console.WriteLine("0,0");
			foreach (var entry in segments.Segments) {
				Console.WriteLine("{0},0", entry.StartDistance.Value());
				Console.WriteLine("{0},-5", entry.StartDistance.Value());
				Console.WriteLine("{0},-5", entry.DistanceMinSpeed.Value());
				Console.WriteLine("{0},5", entry.DistanceMinSpeed.Value());
				Console.WriteLine("{0},5", entry.EndDistance.Value());
				Console.WriteLine("{0},0", entry.EndDistance.Value());
			}
			//}

			//Assert.AreEqual(0.2004, lookup.Data[4].Item1.Value(), 1e-3);
			//Assert.AreEqual(0.1225, lookup._data[4].Item2.Value(), 1e-3);

			//Assert.AreEqual(0.0710, lookup._data[8].Item1.Value(), 1e-3);
			//Assert.AreEqual(0.0719, lookup._data[8].Item2.Value(), 1e-3);

			//Assert.AreEqual(0.0187, lookup._data[12].Item1.Value(), 1e-3);
			//Assert.AreEqual(0.0176, lookup._data[12].Item2.Value(), 1e-3);

		}



		protected virtual Dictionary<MeterPerSecond, Radian> SimulationRunPreprocessingEcoRoll(IVectoRun run)
		{
			var data = run.GetContainer().RunData;
			var modData = new ModalDataContainer(data, null, new[] {FuelData.Diesel}, null, false);
			var builder = new PowertrainBuilder(modData);
			var simpleContainer = new SimplePowertrainContainer(data);
			builder.BuildSimplePowertrain(data, simpleContainer);

			var tmp = new Dictionary<MeterPerSecond, Radian>();
			var preprocessor = new PCCEcoRollEngineStopPreprocessor(simpleContainer, tmp, 50.KMPHtoMeterPerSecond(), 90.KMPHtoMeterPerSecond());
			//preprocessor.SpeedStep = 1.KMPHtoMeterPerSecond();
			var t = Stopwatch.StartNew();

			preprocessor.RunPreprocessing();
			t.Stop();
			Console.WriteLine(t.ElapsedMilliseconds);

			t = Stopwatch.StartNew();
			t.Stop();
			//Console.WriteLine(t.ElapsedMilliseconds);

			return tmp;
		}

		protected virtual PCCSegments SimulationRunPreprocessingPCCSegments(IVectoRun run)
		{
			var data = run.GetContainer().RunData;
			var modData = new ModalDataContainer(data, null, new[] { FuelData.Diesel }, null, false);
			var builder = new PowertrainBuilder(modData);
			var simpleContainer = new SimplePowertrainContainer(data);
			builder.BuildSimplePowertrain(data, simpleContainer);

			var tmp = new PCCSegments();
			var preprocessor = new PCCSegmentPreprocessor(simpleContainer, tmp, data.DriverData.PCC);
			var t = Stopwatch.StartNew();

			preprocessor.RunPreprocessing();
			t.Stop();
			Console.WriteLine(t.ElapsedMilliseconds);

			t = Stopwatch.StartNew();
			t.Stop();
			//Console.WriteLine(t.ElapsedMilliseconds);

			return tmp;
		}



		[TestCase(Class9Decl, 20, 2, 18.9937),
		 TestCase(Class9Decl, 20, 3, 18.6561),
		 TestCase(Class9Decl, 20, 2.5, 18.8248),
		 TestCase(Class9Decl, 25, 0, 24.6687),
		 TestCase(Class9Decl, 25, 1, 24.3307),
		 TestCase(Class9Decl, 25, 0.5, 24.4997),
		 TestCase(Class9Decl, 87, -4, 87.9234),
		 TestCase(Class9Decl, 87, -5, 88.2732),
		 TestCase(Class9Decl, 87, -4.65, 88.15116)
			]
		public void TestSimulationPreprocessingVelocityDuringTractionInterruption(string jobFile, double vPre, double grad, double vPost)
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
			var modData = new ModalDataContainer(data, null, new[] { FuelData.Diesel }, null, false);
			var builder = new PowertrainBuilder(modData);
			var simpleContainer = new SimplePowertrainContainer(data);
			builder.BuildSimplePowertrain(data, simpleContainer);

			var tmp = new VelocityRollingLookup();
			var preprocessor = new VelocitySpeedGearshiftPreprocessor(tmp, 1.SI<Second>(), simpleContainer, minGradient: -12, maxGradient: 12);
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
			var modData = new ModalDataContainer(data, null, new[] {FuelData.Diesel }, null, false);
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
			var modData = new ModalDataContainer(data, null, new[] {FuelData.Diesel }, null, false);
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
