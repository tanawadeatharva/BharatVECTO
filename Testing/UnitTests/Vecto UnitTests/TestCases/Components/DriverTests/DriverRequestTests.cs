using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.DriverTests;

public class DriverRequestTests
{
	public const double Tolerance = 0.001;

    [TestCase]
    public void DriverAccelerationTest()
    {
		var cycleData = DrivingCycleDataReader.ReadFromStream("s,v,grad,stop\n0,0,0,10\n10,20,0,0\n20,21,0,0\n30,22,0,0\n40,23,0,0\n50,24,0,0\n60,25,0,0\n70,26,0,0\n80,27,0,0\n90,28,0,0\n100,29,0,0".ToStream(), CycleType.DistanceBased, "DummyCycle", false);
		var vehicleContainer = GetMockVehicleContainer(cycleData);

		var vehicleMock = Mock.Get(vehicleContainer.VehicleInfo);
        var vehicle = vehicleMock.Object;
        var driver = new Driver(vehicleContainer, vehicleContainer.RunData.DriverData, new DefaultDriverStrategy(vehicleContainer));


		MeterPerSquareSecond reqAcc = null;
		Radian reqGradient = null;
		Second reqSimInterval = null;

        var mockPort = new Mock<IDriverDemandOutPort>();
		mockPort.Setup(p =>
				p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<MeterPerSquareSecond>(), It.IsAny<Radian>(), It.IsAny<bool>()))
			.Returns((Second t, Second dt, MeterPerSquareSecond acc, Radian grad, bool dryryb) => {
				reqAcc = acc;
				reqGradient = grad;
				reqSimInterval = dt;
				return new ResponseSuccess(this) {
					SimulationInterval = dt
				};
			});
		
		mockPort.Setup(p => p.Initialize(It.IsAny<MeterPerSecond>(), It.IsAny<Radian>(), It.IsAny<MeterPerSquareSecond>())).Returns(
			(MeterPerSecond velocity, Radian gradient, MeterPerSquareSecond acc) => new ResponseSuccess(this));
		driver.Connect(mockPort.Object);

		var vehicleSpeed = 0.SI<MeterPerSecond>();
        vehicleMock.Setup(v => v.VehicleSpeed).Returns(() => vehicleSpeed);
        var absTime = 0.SI<Second>();
        var ds = 1.SI<Meter>();
        var gradient = 0.SI<Radian>();

        var targetVelocity = 5.SI<MeterPerSecond>();

        //			var response = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

        var accelerations = new[] {
                1.01570922, 1.384540943, 1.364944972, 1.350793466, 1.331848649, 1.314995215, 1.2999934,
                1.281996392, 1.255462262
            };
        var simulationIntervals = new[] {
                1.403234648, 0.553054094, 0.405255346, 0.33653593, 0.294559444, 0.26555781, 0.243971311, 0.22711761,
                0.213554656
            };

        // accelerate from 0 to just below the target velocity and test derived simulation intervals & accelerations
        for (var i = 0; i < accelerations.Length; i++) {
            var tmpResponse = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

            Assert.IsInstanceOf<ResponseSuccess>(tmpResponse);
            Assert.AreEqual(accelerations[i], reqAcc.Value(), Tolerance);
            Assert.AreEqual(simulationIntervals[i], tmpResponse.SimulationInterval.Value(), Tolerance);

            vehicleContainer.CommitSimulationStep(absTime, tmpResponse.SimulationInterval);
            absTime += tmpResponse.SimulationInterval;
			vehicleSpeed +=
                (tmpResponse.SimulationInterval * reqAcc).Cast<MeterPerSecond>();
        }

        // full acceleration would exceed target velocity, driver should limit acceleration such that target velocity is reached...
        var response = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

        Assert.IsInstanceOf<ResponseSuccess>(response);
        Assert.AreEqual(0.899715479, reqAcc.Value(), Tolerance);
        Assert.AreEqual(0.203734517, response.SimulationInterval.Value(), Tolerance);

        vehicleContainer.CommitSimulationStep(absTime, response.SimulationInterval);
        absTime += response.SimulationInterval;
		vehicleSpeed +=
            (response.SimulationInterval * reqAcc).Cast<MeterPerSecond>();

        Assert.AreEqual(targetVelocity.Value(), vehicleSpeed.Value(), Tolerance);

        // vehicle has reached target velocity, no further acceleration necessary...

        response = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

        Assert.IsInstanceOf<ResponseSuccess>(response);
        Assert.AreEqual(0, reqAcc.Value(), Tolerance);
        Assert.AreEqual(0.2, response.SimulationInterval.Value(), Tolerance);
    }

	private static DriverData GetDriverData()
	{
		return new DriverData { };
	}

	private static IVehicleContainer GetMockVehicleContainer(DrivingCycleData drivingCycleData)
	{
		var runData = new VectoRunData() {
			GearshiftParameters = new ShiftStrategyParameters() {
				StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
				StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
			},
            DriverData = GetDriverData(),
            Cycle = drivingCycleData,
            VehicleData = new VehicleData() {
                VehicleCategory = VehicleCategory.RigidTruck
			}
		};
		var container = new Mock<IVehicleContainer>();
		container.Setup(c => c.RunData).Returns(runData);
		var vehicle = new Mock<IVehicleInfo>();
		container.Setup(c => c.VehicleInfo).Returns(vehicle.Object);
		vehicle.Setup(v => v.VehicleSpeed).Returns(0.KMPHtoMeterPerSecond());
		var cycle = new Mock<IDrivingCycleInfo>();
		container.Setup(c => c.DrivingCycleInfo).Returns(cycle.Object);
		cycle.Setup(c => c.CycleStartDistance).Returns(0.SI<Meter>());
		var milage = new Mock<IMileageCounter>();
		container.Setup(c => c.MileageCounter).Returns(milage.Object);
		milage.Setup(m => m.Distance).Returns(0.SI<Meter>());
		var pt = new Mock<IPowertainInfo>();
		container.Setup(c => c.PowertrainInfo).Returns(pt.Object);
		pt.Setup(p => p.HasCombustionEngine).Returns(true);
		pt.Setup(p => p.HasElectricMotor).Returns(false);
		var iceCtl = new Mock<IEngineControl>();
		container.Setup(c => c.EngineCtl).Returns(iceCtl.Object);
		return container.Object;
	}

    //[TestCase]
    //public void DriverDecelerationTest()
    //{
    //    var driverData = MockSimulationDataFactory.CreateDriverDataFromFile(JobFile);
    //    var cycleData = DrivingCycleDataReader.ReadFromStream("s,v,grad,stop\n0,0,0,10\n10,20,0,0\n20,21,0,0\n30,22,0,0\n40,23,0,0\n50,24,0,0\n60,25,0,0\n70,26,0,0\n80,27,0,0\n90,28,0,0\n100,29,0,0\n110,20,0,0\n120,21,0,0\n130,22,0,0\n140,23,0,0\n150,24,0,0\n160,25,0,0\n170,26,0,0\n180,27,0,0\n190,28,0,0\n200,29,0,0".ToStream(), CycleType.DistanceBased, "DummyCycle", false);
    //    var vehicleContainer = new VehicleContainer(ExecutionMode.Engineering) {
    //        RunData = new VectoRunData() {
    //            VehicleData = new VehicleData() { VehicleCategory = VehicleCategory.RigidTruck },
    //            Cycle = cycleData,
    //            DriverData = driverData
    //        }
    //    };
    //    var vehicle = new MockVehicle(vehicleContainer);
    //    new MockEngine(vehicleContainer);
    //    new EngineOnlyGearboxInfo(vehicleContainer);
    //    new ATClutchInfo(vehicleContainer);

    //    var cycle = new MockDrivingCycle(vehicleContainer, cycleData);
    //    var brakes = new Brakes(vehicleContainer);

    //    var driver = new Driver(vehicleContainer, driverData, new DefaultDriverStrategy(vehicleContainer));
    //    driver.Connect(vehicle.OutPort());

    //    vehicle.MyVehicleSpeed = 5.SI<MeterPerSecond>();
    //    var absTime = 0.SI<Second>();
    //    var ds = 1.SI<Meter>();
    //    var gradient = 0.SI<Radian>();

    //    var targetVelocity = 0.SI<MeterPerSecond>();

    //    //			var response = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

    //    var accelerations = new[] {
    //            -0.68799597, -0.690581291, -0.693253225, -0.696020324, -0.698892653, -0.701882183, -0.695020765,
    //            -0.677731071,
    //            -0.660095846, -0.642072941, -0.623611107, -0.604646998, -0.58510078, -0.56497051, -0.547893288,
    //            -0.529859078,
    //            -0.510598641, -0.489688151, -0.466386685, -0.425121905
    //        };
    //    var simulationIntervals = new[] {
    //            0.202830428, 0.20884052, 0.215445127, 0.222749141, 0.230885341, 0.240024719, 0.250311822, 0.26182762,
    //            0.274732249,
    //            0.289322578, 0.305992262, 0.325276486, 0.34792491, 0.37502941, 0.408389927, 0.451003215, 0.5081108,
    //            0.590388012,
    //            0.724477573, 1.00152602
    //        };

    //    // accelerate from 0 to just below the target velocity and test derived simulation intervals & accelerations
    //    for (var i = 0; i < accelerations.Length; i++) {
    //        var tmpResponse = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

    //        Assert.IsInstanceOf<ResponseSuccess>(tmpResponse);
    //        Assert.AreEqual(accelerations[i], vehicle.LastRequest.acceleration.Value(), Tolerance);
    //        Assert.AreEqual(simulationIntervals[i], tmpResponse.SimulationInterval.Value(), Tolerance);

    //        vehicleContainer.CommitSimulationStep(absTime, tmpResponse.SimulationInterval);
    //        absTime += tmpResponse.SimulationInterval;
    //        vehicle.MyVehicleSpeed +=
    //            (tmpResponse.SimulationInterval * vehicle.LastRequest.acceleration).Cast<MeterPerSecond>();
    //    }

    //    var response = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

    //    Assert.IsInstanceOf<ResponseSuccess>(response);
    //    Assert.AreEqual(-0.308576594, vehicle.LastRequest.acceleration.Value(), Tolerance);
    //    Assert.AreEqual(2.545854078, response.SimulationInterval.Value(), Tolerance);

    //    vehicleContainer.CommitSimulationStep(absTime, response.SimulationInterval);
    //    //absTime += response.SimulationInterval;
    //    vehicle.MyVehicleSpeed +=
    //        (response.SimulationInterval * vehicle.LastRequest.acceleration).Cast<MeterPerSecond>();

    //    Assert.AreEqual(targetVelocity.Value(), vehicle.MyVehicleSpeed.Value(), Tolerance);
    //}

}