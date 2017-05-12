using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestFixture]
	public class TorqueLimitsTest
	{
		const string GearboxLimitJobDecl_865 =
			@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_GbxTorqueLimits\Class2_RigidTruck_gbxTqLimit-865_DECL.vecto";

		const string GearboxLimitJobDecl_800 =
			@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_GbxTorqueLimits\Class2_RigidTruck_gbxTqLimit-800_DECL.vecto";


		const string VehicleLimitJobDecl_910 =
			@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_VehTorqueLimits\Class2_RigidTruck_vehTqLimit-910_DECL.vecto";

		const string VehicleLimitJobDecl_850 =
			@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_VehTorqueLimits\Class2_RigidTruck_vehTqLimit-850_DECL.vecto";

		const string GearboxSpeedLimitJobDecl =
			@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_GbxSpeedLimits\Class2_RigidTruck_DECL.vecto";

		[TestCase()]
		public void TestGearboxTorqueLimitsAbove90FLD()
		{
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(GearboxLimitJobDecl_865);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputDataProvider, null);

			var run = factory.DataReader.NextRun().First();

			var engineData = run.EngineData;

			// check default FLD
			Assert.AreEqual(956, engineData.FullLoadCurves[0].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[0].MaxDragTorque.Value());

			// check first gear - limited by gbx
			Assert.AreEqual(865, engineData.FullLoadCurves[1].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[1].MaxDragTorque.Value());

			// check fourth gear - limited by gbx but not applicaple
			Assert.AreEqual(956, engineData.FullLoadCurves[4].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[4].MaxDragTorque.Value());

			// check last gear - limited by gbx but not applicaple
			Assert.AreEqual(956, engineData.FullLoadCurves[6].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[6].MaxDragTorque.Value());
		}

		[TestCase()]
		public void TestGearboxTorqueLimitsBelow90FLD()
		{
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(GearboxLimitJobDecl_800);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputDataProvider, null);

			var run = factory.DataReader.NextRun().First();

			var engineData = run.EngineData;

			// check default FLD
			Assert.AreEqual(956, engineData.FullLoadCurves[0].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[0].MaxDragTorque.Value());

			// check first gear - limited by gbx
			Assert.AreEqual(800, engineData.FullLoadCurves[1].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[1].MaxDragTorque.Value());

			// check fourth gear - limited by gbx
			Assert.AreEqual(800, engineData.FullLoadCurves[4].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[4].MaxDragTorque.Value());

			// check last gear - limited by gbx
			Assert.AreEqual(800, engineData.FullLoadCurves[6].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[6].MaxDragTorque.Value());
		}

		[TestCase()]
		public void TestVehicleTorqueLimitsAbove95FLD()
		{
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(VehicleLimitJobDecl_910);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputDataProvider, null);

			var run = factory.DataReader.NextRun().First();

			var engineData = run.EngineData;

			// check default FLD
			Assert.AreEqual(956, engineData.FullLoadCurves[0].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[0].MaxDragTorque.Value());

			// check first gear - limited by vehicle but not applicaple
			Assert.AreEqual(956, engineData.FullLoadCurves[1].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[1].MaxDragTorque.Value());

			// check fourth gear - limited by vehicle but not applicaple
			Assert.AreEqual(956, engineData.FullLoadCurves[4].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[4].MaxDragTorque.Value());

			// check last gear - limited by vehicle but not applicaple
			Assert.AreEqual(956, engineData.FullLoadCurves[6].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[6].MaxDragTorque.Value());
		}

		[TestCase()]
		public void TestVehicleTorqueLimitsBelow95FLD()
		{
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(VehicleLimitJobDecl_850);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputDataProvider, null);

			var run = factory.DataReader.NextRun().First();

			var engineData = run.EngineData;

			// check default FLD
			Assert.AreEqual(956, engineData.FullLoadCurves[0].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[0].MaxDragTorque.Value());

			// check first gear - limited by vehicle but not applicaple
			Assert.AreEqual(956, engineData.FullLoadCurves[1].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[1].MaxDragTorque.Value());

			// check fourth gear - limited by vehicle
			Assert.AreEqual(850, engineData.FullLoadCurves[4].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[4].MaxDragTorque.Value());

			// check last gear - limited by vehicle 
			Assert.AreEqual(850, engineData.FullLoadCurves[6].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[6].MaxDragTorque.Value());
		}

		[TestCase(GearboxLimitJobDecl_800),
		TestCase(GearboxLimitJobDecl_865),
		TestCase(VehicleLimitJobDecl_850),
		TestCase(VehicleLimitJobDecl_910)]
		public void TestRunTorqueLimitedSimulations(string file)
		{
			var fileWriter = new FileOutputWriter(file);
			var sumData = new SummaryDataContainer(fileWriter);
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(file);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputDataProvider, fileWriter) {
				WriteModalResults = true
			};


			var jobContainer = new JobContainer(sumData);
			jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}

		[TestCase(GearboxSpeedLimitJobDecl)]
		public void TestRunGbxSpeedLimitedSimulations(string file)
		{
			var fileWriter = new FileOutputWriter(file);
			var sumData = new SummaryDataContainer(fileWriter);
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(file);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputDataProvider, fileWriter) {
				WriteModalResults = true,
				ActualModalData = true
			};


			var jobContainer = new JobContainer(sumData);
			jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}
	}
}