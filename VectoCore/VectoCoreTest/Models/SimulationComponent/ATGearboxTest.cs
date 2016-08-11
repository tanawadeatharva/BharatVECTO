using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Integration;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestFixture]
	public class ATGearboxTest
	{
		public const string EngineDataFile = @"TestData\Components\AT_GBX\Engine.veng";
		public const string GearboxDataFile = @"TestData\Components\AT_GBX\GearboxSerial.vgbx";

		[Test,
		TestCase(0, 100, 1),
		TestCase(0, 200, 1),
		TestCase(5, 100, 1),
		TestCase(5, 300, 1),
		TestCase(5, 600, 1),
		TestCase(15, 100, 3),
		TestCase(15, 300, 3),
		TestCase(15, 600, 2),
		TestCase(40, 100, 6),
		TestCase(40, 300, 4),
		TestCase(40, 600, 4),
		TestCase(70, 100, 6),
		TestCase(70, 300, 6),
		TestCase(70, 600, 6),
		]
		public void TestATGearInitialize(double vehicleSpeed, double torque, int expectedGear)
		{
			var vehicleContainer = new MockVehicleContainer(); //(ExecutionMode.Engineering);
			vehicleContainer.Engine = new CombustionEngine(vehicleContainer,
				MockSimulationDataFactory.CreateEngineDataFromFile(EngineDataFile));
			var gearboxData = MockSimulationDataFactory.CreateGearboxDataFromFile(GearboxDataFile, EngineDataFile, false);
			var gearbox = new ATGearbox(vehicleContainer, gearboxData, new ATShiftStrategy(gearboxData, vehicleContainer));

			vehicleContainer.VehicleSpeed = vehicleSpeed.KMPHtoMeterPerSecond();

			var tnPort = new MockTnOutPort();
			gearbox.Connect(tnPort);

			// r_dyn = 0.465m, i_axle = 6.2
			var angularVelocity = vehicleSpeed.KMPHtoMeterPerSecond() / 0.465.SI<Meter>() * 6.2;
			var response = gearbox.Initialize(torque.SI<NewtonMeter>(), angularVelocity);

			Assert.IsInstanceOf(typeof(ResponseSuccess), response);
			Assert.AreEqual(expectedGear, gearbox.Gear);
			Assert.AreEqual(vehicleSpeed.IsEqual(0), gearbox.Disengaged);
		}

		[Test]
		public void TestATGearboxDriveTorqueConverter()
		{
			var cycleData = @"   0,  0, 0,    2
                                20,  8, 0,    0
							   200,  0, 0,    2";
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);
			var run = ATPowerTrain.CreateEngineeringRun(cycle, "AT_Vehicle_Drive-TC.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[Test]
		public void TestATGearboxShiftUp()
		{
			var cycleData = @"  0,  0, 0,    2
							  500, 40, 0,    0";
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);
			var run = ATPowerTrain.CreateEngineeringRun(cycle, "AT_Vehicle_Drive-TC_shiftup.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[Test]
		public void TestATGearboxShiftDown()
		{
			var cycleData = @"  0, 70, 0,    0
							  500,  0, 0,    2";
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);
			var run = ATPowerTrain.CreateEngineeringRun(cycle, "AT_Vehicle_Drive-TC_shiftdown.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}


		[Test]
		public void TestATGearboxDriveUrban()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("Urban");
			var run = ATPowerTrain.CreateEngineeringRun(cycle, "AT_Vehicle_Drive-TC_Urban.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[Test]
		public void TestATGearboxDriveSuburban()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("Suburban");
			var run = ATPowerTrain.CreateEngineeringRun(cycle, "AT_Vehicle_Drive-TC_Suburban.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[Test]
		public void TestATGearboxDriveInterurban()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("Interurban");
			var run = ATPowerTrain.CreateEngineeringRun(cycle, "AT_Vehicle_Drive-TC_Interurban.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[Test]
		public void TestATGearboxDriveHeavyurban()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("HeavyUrban");
			var run = ATPowerTrain.CreateEngineeringRun(cycle, "AT_Vehicle_Drive-TC_Heavyurban.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}
	}
}