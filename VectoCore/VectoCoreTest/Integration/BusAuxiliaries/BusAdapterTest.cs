using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Tests.Utils;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace TUGraz.VectoCore.Tests.Integration.BusAuxiliaries
{
	[TestFixture]
	public class BusAdapterTest
	{
		[Test]
		[TestCase(12000, 1256, 148, 148, 6087.0317)]
		[TestCase(12000, 1256, -48, -148, 6087.0317)]
		[TestCase(12000, 1256, 48, -148, 6087.0317)]
		[TestCase(12000, 800, 148, 148, 6377.2026)]
		[TestCase(12000, 800, -48, -148, 6377.2026)]
		[TestCase(12000, 800, 48, -148, 6377.2026)]
		public void TestNoSmartAuxDuringDrive(double vehicleWeight, double engineSpeedRpm, double driveLinePower,
			double internalPower, double expectedPowerDemand)
		{
			MockDriver driver;
			var busAux = AuxDemandTest.CreateBusAuxAdapterForTesting(vehicleWeight, out driver);

			driver.DrivingBehavior = DrivingBehavior.Driving;

			var engineDrivelinePower = (driveLinePower * 1000).SI<Watt>();
			var engineSpeed = engineSpeedRpm.RPMtoRad();
			busAux.Initialize(engineDrivelinePower / engineSpeed, engineSpeed);

			var torque = busAux.PowerDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed,
				(internalPower * 1000).SI<Watt>() / engineSpeed, engineSpeed);

			Assert.AreEqual(expectedPowerDemand, (torque * engineSpeed).Value(), 1e-4);
		}

		[Test]
		[TestCase(12000, 1256, 148, 148, 6087.0317)]
		[TestCase(12000, 1256, -28, -27, 6087.0317)]
		[TestCase(12000, 1256, -28, -29, 6087.0317)]
		[TestCase(12000, 1256, -128, -28, 6087.0317)]
		[TestCase(12000, 1256, 28, -28, 6087.0317)]
		[TestCase(12000, 800, 148, 148, 6377.2026)]
		[TestCase(12000, 800, -14, -13, 6377.2026)]
		[TestCase(12000, 800, -14, -15, 6377.2026)]
		[TestCase(12000, 800, -35, -14, 6377.2026)]
		[TestCase(12000, 800, 35, -14, 6377.2026)]
		public void TestNoSmartAuxDuringCoasting(double vehicleWeight, double engineSpeedRpm, double driveLinePower,
			double internalPower, double expectedPowerDemand)
		{
			// this test is to make sure that the aux power-demand does not jump between average and smart power demand
			// when searching for the operating point for coasting (i.e. power demand (internal Power) is close to the motoring curve, 
			// intependent of power demand of power train)
			MockDriver driver;
			var busAux = AuxDemandTest.CreateBusAuxAdapterForTesting(vehicleWeight, out driver);

			driver.DrivingBehavior = DrivingBehavior.Coasting;

			var engineDrivelinePower = (driveLinePower * 1000).SI<Watt>();
			var engineSpeed = engineSpeedRpm.RPMtoRad();
			busAux.Initialize(engineDrivelinePower / engineSpeed, engineSpeed);

			var torque = busAux.PowerDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed,
				(internalPower * 1000).SI<Watt>() / engineSpeed, engineSpeed);

			Assert.AreEqual(expectedPowerDemand, (torque * engineSpeed).Value(), 1e-4);
		}

		[Test]
		[TestCase(12000, 1256, -48, -148, 8954.14355)]
		[TestCase(12000, 1256, 48, -148, 8954.14355)]
		[TestCase(12000, 800, -48, -148, 8281.51367)]
		[TestCase(12000, 800, 48, -148, 8281.51367)]
		public void TestSmartAuxDuringBrake(double vehicleWeight, double engineSpeedRpm, double driveLinePower,
			double internalPower, double expectedPowerDemand)
		{
			MockDriver driver;
			var busAux = AuxDemandTest.CreateBusAuxAdapterForTesting(vehicleWeight, out driver);

			driver.DrivingBehavior = DrivingBehavior.Braking;

			var engineDrivelinePower = (driveLinePower * 1000).SI<Watt>();
			var engineSpeed = engineSpeedRpm.RPMtoRad();
			busAux.Initialize(engineDrivelinePower / engineSpeed, engineSpeed);

			var torque = busAux.PowerDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed,
				(internalPower * 1000).SI<Watt>() / engineSpeed, engineSpeed);

			Assert.AreEqual(expectedPowerDemand, (torque * engineSpeed).Value(), 1e-3);
		}
	}
}