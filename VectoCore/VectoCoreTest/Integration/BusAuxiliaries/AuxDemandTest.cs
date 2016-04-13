using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace TUGraz.VectoCore.Tests.Integration.BusAuxiliaries
{
	[TestFixture]
	public class AuxDemandTest
	{
		[Test]
		[TestCase(12000, 1256, 148, 148, 6087.0317)]
		[TestCase(12000, 1256, -15, -50, 8954.1435)]
		[TestCase(15700, 1319, -35.79263, -144.0441, 9093.9511)]
		public void AuxDemandtest(double vehicleWeight, double engineSpeedRpm, double driveLinePower, double internalPower,
			double expectedPowerDemand)
		{
			MockDriver driver;
			var busAux = CreateBusAuxAdapterForTesting(vehicleWeight, out driver);

			var engineDrivelinePower = (driveLinePower * 1000).SI<Watt>();
			var engineSpeed = engineSpeedRpm.RPMtoRad();
			busAux.Initialize(engineDrivelinePower / engineSpeed, engineSpeed);

			var torque = busAux.PowerDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed,
				(internalPower * 1000).SI<Watt>() / engineSpeed, engineSpeed);

			Assert.AreEqual(expectedPowerDemand, (torque * engineSpeed).Value(), 1e-4);
		}

		[Test]
		public void AuxFCConsumptionTest()
		{
			var driveLinePower = 148;
			var engineSpeedRpm = 1256;
			var internalPower = 148;

			MockDriver driver;
			var busAux = CreateBusAuxAdapterForTesting(12000, out driver);

			var engineDrivelinePower = (driveLinePower * 1000).SI<Watt>();
			var engineSpeed = engineSpeedRpm.RPMtoRad();
			busAux.Initialize(engineDrivelinePower / engineSpeed, engineSpeed);

			var modalData = new MockModalDataContainer();

			for (int i = 0; i < 10; i++) {
				var torque = busAux.PowerDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed,
					(internalPower * 1000).SI<Watt>() / engineSpeed, engineSpeed);
				Assert.AreEqual(6087.0317, (torque * engineSpeed).Value(), 1e-3);
				busAux.CommitSimulationStep(modalData);
			}

			Assert.AreEqual(79.303, ((SI)modalData[ModalResultField.AA_TotalCycleFC_Grams]).Value(), 0.0001);

			engineDrivelinePower = -15000.SI<Watt>();
			internalPower = -50;

			for (int i = 0; i < 10; i++) {
				var torque = busAux.PowerDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed,
					(internalPower * 1000).SI<Watt>() / engineSpeed, engineSpeed);
				Assert.AreEqual(8954.1435, (torque * engineSpeed).Value(), 1e-3);
				busAux.CommitSimulationStep(modalData);
			}

			Assert.AreEqual(82.5783, ((SI)modalData[ModalResultField.AA_TotalCycleFC_Grams]).Value(), 0.0001);

			engineDrivelinePower = (driveLinePower * 1000).SI<Watt>();
			internalPower = 148;

			for (int i = 0; i < 10; i++) {
				var torque = busAux.PowerDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed,
					(internalPower * 1000).SI<Watt>() / engineSpeed, engineSpeed);
				Assert.AreEqual(6087.0317, (torque * engineSpeed).Value(), 1e-3);
				busAux.CommitSimulationStep(modalData);
			}

			Assert.AreEqual(162.4655, ((SI)modalData[ModalResultField.AA_TotalCycleFC_Grams]).Value(), 0.0001);
		}

		public static BusAuxiliariesAdapter CreateBusAuxAdapterForTesting(double vehicleWeight, out MockDriver driver)
		{
			var auxFilePath = @"TestData\Integration\BusAuxiliaries\AdvAuxTest.aaux";
			var engineFLDFilePath = @"TestData\Integration\BusAuxiliaries\24t Coach.vfld";
			var engineFCMapFilePath = @"TestData\Integration\BusAuxiliaries\24t Coach.vmap";

			var vehicle = new VehicleContainer(ExecutionMode.Engineering);
			var fcMap = FuelConsumptionMap.ReadFromFile(engineFCMapFilePath);
			var fld = EngineFullLoadCurve.ReadFromFile(engineFLDFilePath);
			var modelData = new CombustionEngineData() {
				ConsumptionMap = fcMap,
				FullLoadCurve = fld,
				IdleSpeed = 560.SI<PerSecond>()
			};

			var engine = new CombustionEngine(vehicle, modelData);
			//new Vehicle(vehicle, new VehicleData());
			driver = new MockDriver(vehicle) { VehicleStopped = false, DrivingBehavior = DrivingBehavior.Braking };
			var gbx = new MockGearbox(vehicle) { Gear = 1 };
			var brakes = new MockBrakes(vehicle);
			var busAux = new BusAuxiliariesAdapter(vehicle, auxFilePath, "Coach", vehicleWeight.SI<Kilogram>(),
				fcMap, modelData.IdleSpeed);
			return busAux;
		}
	}
}