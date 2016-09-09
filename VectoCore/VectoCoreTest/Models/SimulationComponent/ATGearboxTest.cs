using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
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
		TestCase(40, 300, 5),
		TestCase(40, 600, 4),
		TestCase(70, 100, 6),
		TestCase(70, 300, 6),
		TestCase(70, 600, 6),
		]
		public void TestATGearInitialize(double vehicleSpeed, double torque, int expectedGear)
		{
			var vehicleContainer = new MockVehicleContainer(); //(ExecutionMode.Engineering);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineDataFile);
			vehicleContainer.Engine = new CombustionEngine(vehicleContainer,engineData);
			var gearboxData = MockSimulationDataFactory.CreateGearboxDataFromFile(GearboxDataFile, EngineDataFile, false);
			var gearbox = new ATGearbox(vehicleContainer, gearboxData, new ATShiftStrategy(gearboxData, vehicleContainer), engineData.Inertia);

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

		[Test,
		TestCase(GearboxType.ATSerial, TestName = "Drive TorqueConverter - Serial"),
		TestCase(GearboxType.ATPowerSplit, TestName = "Drive TorqueConverter - PowerSplit")]
		public void TestATGearboxDriveTorqueConverter(GearboxType gbxType)
		{
			var cycleData = @"   0,  0, 0,    2
								20,  8, 0,    0
							   200,  0, 0,    2";
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);
			var run = ATPowerTrain.CreateEngineeringRun(cycle, gbxType,
				string.Format("AT_Vehicle_Drive-TC-{0}.vmod", gbxType==GearboxType.ATSerial ? "ser" : "ps"));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[Test,
		TestCase(GearboxType.ATSerial, TestName = "ShiftUp TorqueConverter - Serial"),
		TestCase(GearboxType.ATPowerSplit, TestName = "ShiftUp TorqueConverter - PowerSplit")]
		public void TestATGearboxShiftUp(GearboxType gbxType)
		{
			var cycleData = @"  0,  0, 0,    2
							  500, 40, 0,    0";
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);
			var run = ATPowerTrain.CreateEngineeringRun(cycle, gbxType,
				string.Format("AT_Vehicle_Drive-TC_shiftup-{0}.vmod", gbxType==GearboxType.ATSerial ? "ser" : "ps"));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[Test,
		TestCase(GearboxType.ATSerial, TestName = "ShiftDown TorqueConverter - Serial"),
		TestCase(GearboxType.ATPowerSplit, TestName = "ShiftDown TorqueConverter - PowerSplit")]
		public void TestATGearboxShiftDown(GearboxType gbxType)
		{
			var cycleData = @"  0, 70, 0,    0
							  500,  0, 0,    2";
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);
			var run = ATPowerTrain.CreateEngineeringRun(cycle, gbxType,
				string.Format("AT_Vehicle_Drive-TC_shiftdown-{0}.vmod", gbxType==GearboxType.ATSerial ? "ser" : "ps"));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}


		[Test,
		TestCase("Urban", GearboxType.ATSerial),
		TestCase("Suburban", GearboxType.ATSerial),
		TestCase("Interurban", GearboxType.ATSerial),
		TestCase("HeavyUrban", GearboxType.ATSerial),
		TestCase("Urban", GearboxType.ATPowerSplit),
		TestCase("Suburban", GearboxType.ATPowerSplit),
		TestCase("Interurban", GearboxType.ATPowerSplit),
		TestCase("HeavyUrban", GearboxType.ATPowerSplit)
		]
		public void TestATGearboxDriveCycle(string cycleName, GearboxType gbxType)
		{
			Assert.IsTrue(gbxType.AutomaticTransmission());
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle(cycleName);
			var run = ATPowerTrain.CreateEngineeringRun(cycle, gbxType,
				string.Format("AT_Vehicle_Drive-TC_{0}-{1}.vmod", cycleName, gbxType==GearboxType.ATSerial ? "ser" : "ps"));

			var sumWriter =
				new SummaryDataContainer(
					new FileOutputWriter(string.Format("AT_Vehicle_Drive-TC_{0}-{1}", cycleName, gbxType == GearboxType.ATSerial ? "ser" : "ps")));
			((VehicleContainer)run.GetContainer()).WriteSumData = (writer, mass, loading) =>
				sumWriter.Write(run.GetContainer().ModalData, cycleName, string.Format("{0}-{1}", 0, 0),
					cycleName + Constants.FileExtensions.CycleFile, mass, loading);
			run.Run();
			sumWriter.Finish();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}
	}
}