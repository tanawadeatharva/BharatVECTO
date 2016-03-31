using NUnit.Framework;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace TUGraz.VectoCore.Tests.Integration.BusAuxiliaries
{
	[TestFixture]
	public class AuxDemandTest
	{
		[Test]
		[TestCase(1256, 148, 148, 4537.96826F)]
		[TestCase(1256, -15, -50, 7405.0791)]
		public void AuxDemandtest(double engineSpeedRpm, double driveLinePower, double internalPower,
			double expectedPowerDemand)
		{
			var auxFilePath = @"TestData\Integration\BusAuxiliaries\AdvAuxTest.aaux";
			var engineFLDFilePath = @"TestData\Integration\BusAuxiliaries\24t Coach.vfld";
			var engineFCMapFilePath = @"TestData\Integration\BusAuxiliaries\24t Coach.vmap";

			var vehicle = new VehicleContainer();
			var fcMap = FuelConsumptionMap.ReadFromFile(engineFCMapFilePath);
			var fld = EngineFullLoadCurve.ReadFromFile(engineFLDFilePath);
			var modelData = new CombustionEngineData() {
				ConsumptionMap = fcMap,
				FullLoadCurve = fld,
				IdleSpeed = 560.SI<PerSecond>()
			};

			var engine = new CombustionEngine(vehicle, modelData);
			//new Vehicle(vehicle, new VehicleData());
			var driver = new MockDriver(vehicle) { VehicleStopped = false };
			var gbx = new MockGearbox(vehicle) { Gear = 1 };

			var busAux = new BusAuxiliariesAdapter(vehicle, auxFilePath, "Coach", 12000.SI<Kilogram>(),
				fcMap, modelData.IdleSpeed);


			var engineDrivelinePower = (driveLinePower * 1000).SI<Watt>();
			var engineSpeed = engineSpeedRpm.RPMtoRad();
			busAux.Initialize(engineDrivelinePower / engineSpeed, engineSpeed);

			var power = busAux.PowerDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed,
				(internalPower * 1000).SI<Watt>() / engineSpeed, engineSpeed);

			Assert.AreEqual(expectedPowerDemand, (power * engineSpeed).Value(), 1e-4);
		}
	}
}