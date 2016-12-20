using System.Globalization;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Integration;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	public class GearboxShiftLossesTest
	{
		private static AxleGearData CreateAxleGearData(GearboxType gbxType)
		{
			var ratio = gbxType == GearboxType.ATSerial ? 6.2 : 5.8;
			return new AxleGearData {
				AxleGear = new GearData {
					Ratio = ratio,
					LossMap = TransmissionLossMapReader.Create(0.95, ratio, "Axlegear"),
				}
			};
		}

		[Test,
		TestCase(200, 2u, 562, 620, 36.4855),
		TestCase(400, 2u, 562, 620, 55.9878),
		TestCase(600, 2u, 562, 620, 75.4901),
		TestCase(800, 2u, 562, 620, 94.9924),
		TestCase(200, 2u, 562, 600, 39.6371),
		TestCase(400, 2u, 562, 600, 59.4279),
		TestCase(600, 2u, 562, 600, 79.218),
		TestCase(800, 2u, 562, 600, 99.0095),
		TestCase(400, 3u, 500, 490, 9.6354),
		TestCase(400, 3u, 550, 490, 17.325),
		TestCase(600, 3u, 550, 490, 33.5574),
		]
		public void TestShiftLossComputation(double torqueDemand, uint gear, double preShiftRpm, double postShiftRpm,
			double expectedShiftLoss)
		{
			var engineInertia = 5.SI<KilogramSquareMeter>();

			var gearboxData = ATPowerTrain.CreateGearboxData(GearboxType.ATSerial);

			var container = new VehicleContainer(ExecutionMode.Engineering);

			var cycleDataStr = "0, 0, 0, 2\n100, 20, 0, 0\n1000, 50, 0, 0";
			var cycleData = SimpleDrivingCycles.CreateCycleData(cycleDataStr);
			var cycle = new MockDrivingCycle(container, cycleData);

			var axleGear = new AxleGear(container, CreateAxleGearData(GearboxType.ATSerial));
			axleGear.Connect(new MockComponent());

			var wheels = new Wheels(container, 0.5.SI<Meter>(), 9.5.SI<KilogramSquareMeter>());

			var vehicle = new MockVehicle(container);
			var driver = new MockDriver(container);
			vehicle.MyVehicleSpeed = 10.KMPHtoMeterPerSecond();
			driver.DriverBehavior = DrivingBehavior.Driving;
			var engine = new CombustionEngine(container,
				MockSimulationDataFactory.CreateEngineDataFromFile(ATPowerTrain.EngineFile));
			container.Engine = engine;
			var gbx = new ATGearbox(container, gearboxData, new ATShiftStrategy(gearboxData, container), engineInertia);
			gbx.Connect(engine);
			gbx.IdleController = new MockIdleController();

			var init = gbx.Initialize(0.SI<NewtonMeter>(), preShiftRpm.RPMtoRad());

			gbx.Gear = gear;

			var absTime = 20.SI<Second>();
			var dt = 0.5.SI<Second>();
			var response = gbx.Request(absTime, dt, 0.SI<NewtonMeter>(), preShiftRpm.RPMtoRad());
			axleGear.Request(absTime, dt, 0.SI<NewtonMeter>(), preShiftRpm.RPMtoRad());

			Assert.IsInstanceOf<ResponseSuccess>(response);
			container.CommitSimulationStep(absTime, dt);
			absTime += dt;

			response = gbx.Request(absTime, dt, torqueDemand.SI<NewtonMeter>(), postShiftRpm.RPMtoRad());
			Assert.IsInstanceOf<ResponseFailTimeInterval>(response);

			dt = ((ResponseFailTimeInterval)response).DeltaT;
			response = gbx.Request(absTime, dt, torqueDemand.SI<NewtonMeter>(), postShiftRpm.RPMtoRad());

			Assert.IsInstanceOf<ResponseSuccess>(response);
			Assert.AreEqual(expectedShiftLoss, gbx.CurrentState.PowershiftLosses.Value(), 1e-3);
			Assert.AreEqual(gear + (postShiftRpm > preShiftRpm ? 1 : -1), gbx.Gear);
		}
	}
}