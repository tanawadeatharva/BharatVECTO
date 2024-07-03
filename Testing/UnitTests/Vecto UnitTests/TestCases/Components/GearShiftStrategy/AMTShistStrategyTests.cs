using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.GearShiftStrategy;

public class AMTShistStrategyTests
{
    [TestCase(8, 7, 1800, 750, typeof(ResponseGearShift)),
TestCase(7, 6, 1800, 750, typeof(ResponseGearShift)),
TestCase(6, 5, 1800, 750, typeof(ResponseGearShift)),
TestCase(5, 4, 1800, 750, typeof(ResponseGearShift)),
TestCase(4, 3, 1800, 750, typeof(ResponseGearShift)),
TestCase(3, 2, 1800, 750, typeof(ResponseGearShift)),
TestCase(2, 1, 1900, 750, typeof(ResponseGearShift)),
TestCase(1, 1, 1200, 700, typeof(ResponseSuccess)),
TestCase(8, 4, 15000, 200, typeof(ResponseGearShift)),]
    public void Gearbox_ShiftDown_ACEA_Shiftlines(int gear, int newGear, double t, double n, Type responseType)
    {

        //var gearboxData = MockSimulationDataFactory.CreateGearboxDataFromFile(GearboxDataFile, EngineDataFile);

        //var gearboxInput = JSONInputDataFactory.ReadGearbox(GearboxDataFile);
        //var engineInput = JSONInputDataFactory.ReadEngine(EngineDataFile);
        //var dao = new DeclarationDataAdapterHeavyLorry.Conventional();
        //var engineData = dao.CreateEngineData(new MockDeclarationVehicleInputData() {
        //    EngineInputData = engineInput,
        //    GearboxInputData = gearboxInput
        //}, engineInput.EngineModes.First(), new Mission() {
        //    MissionType = MissionType.LongHaul
        //});
		var ratios = new[] { 0.0, 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };
        var gearboxData = new GearboxData {
            Gears = new Dictionary<uint, GearData>()
		};
		for (uint i = 1; i < ratios.Length; i++) {
			gearboxData.Gears[i] = new GearData {
				Ratio = ratios[i],
			};
		}
		//foreach (var entry in gearboxData.Gears) {
  //          entry.Value.ShiftPolygon = DeclarationData.Gearbox.ComputeManualTransmissionShiftPolygon(
  //              (int)(entry.Key - 1), engineData.FullLoadCurves.First().Value,
  //              gearboxInput.Gears, engineData, ((IAxleGearInputData)gearboxInput).Ratio, 0.5.SI<Meter>());
  //      }

        //var container = new VehicleContainer(ExecutionMode.Engineering) { RunData = GetDummyRunData(gearboxData) };
        //var gearbox = new Gearbox(container, new AMTShiftStrategy(container));
        //var cycleData = DrivingCycleDataReader.ReadFromStream("s,v,grad,stop\n0,0,0,10\n10,20,0,0\n20,21,0,0\n30,22,0,0\n40,23,0,0\n50,24,0,0\n60,25,0,0\n70,26,0,0\n80,27,0,0\n90,28,0,0\n100,29,0,0".ToStream(), CycleType.DistanceBased, "DummyCycle", false);
        //var cycle = new MockDrivingCycle(container, cycleData);
        //container.RunData = new VectoRunData() { Cycle = cycleData };

        //var driver = new MockDriver(container);
        //var port = new MockTnOutPort() { EngineN95hSpeed = 2000.RPMtoRad() };
        //gearbox.InPort().Connect(port);
        //var vehicle = new MockVehicle(container) { MyVehicleSpeed = 10.SI<MeterPerSecond>() };
        //container.EngineInfo = port;

		var runData = new VectoRunData() {
            GearboxData = gearboxData,
            GearshiftParameters = new ShiftStrategyParameters() {
                StartSpeed = 2.SI<MeterPerSecond>(),
            },
            EngineData = new CombustionEngineData() {
                IdleSpeed = 560.RPMtoRad(),
                FullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>() {
					{0u, FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(EngineFldHdr, EngineFldData))}
				}
			},
            AxleGearData = new AxleGearData() {
                AxleGear = new TransmissionData() {
                    Ratio = 3.240355
                }
			},
            VehicleData = new VehicleData() {
				DynamicTyreRadius = 0.492.SI<Meter>(),
            }
            
		};

		var container = new Mock<IVehicleContainer>();
		container.Setup(c => c.RunData).Returns(runData);
		var veh = new Mock<IVehicleInfo>();
		veh.Setup(v => v.VehicleSpeed).Returns(10.SI<MeterPerSecond>());
		container.Setup(c => c.VehicleInfo).Returns(veh.Object);
		var eng = new Mock<IEngineInfo>();
		container.Setup(c => c.EngineInfo).Returns(eng.Object);
		eng.Setup(e => e.EngineIdleSpeed).Returns(560.RPMtoRad());

		var shiftStrategy = new AMTShiftStrategy(container.Object);

        
        // the first element 0.0 is just a placeholder for axlegear, not used in this test

        var absTime = 0.SI<Second>();
        var dt = 2.SI<Second>();

        var expectedN = n.RPMtoRad();
        var angularVelocity = expectedN / ratios[gear];
        //gearbox.OutPort().Initialize(1.SI<NewtonMeter>(), angularVelocity);
		shiftStrategy.InitGear(0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, 1.SI<NewtonMeter>(),
			angularVelocity);

        var expectedT = t.SI<NewtonMeter>();
		var torque = expectedT * ratios[gear];


        //gearbox.Gear = new GearshiftPosition((uint)gear);
        //container.AbsTime = absTime;
        //var gearShiftResponse = gearbox.OutPort().Request(absTime, dt, torque, angularVelocity, false);
        //Assert.IsTrue(gearShiftResponse.GetType() == responseType);
		var shiftRequired = shiftStrategy.ShiftRequired(absTime, dt, torque, angularVelocity, expectedT, expectedN,
			new GearshiftPosition((uint)gear), -double.MaxValue.SI<Second>(), new ResponseSuccess(this));

        Assert.IsTrue(shiftRequired);

        absTime += dt;
        //var successResponse = (ResponseSuccess)gearbox.OutPort().Request(absTime, dt, torque, angularVelocity, false);
        //Assert.AreEqual((uint)newGear, container.GearboxInfo.Gear.Gear);
    }

    //[TestCase(7, 8, 1000, 1400, typeof(ResponseGearShift)),
    //TestCase(6, 8, 1000, 1400, typeof(ResponseGearShift)),
    //TestCase(5, 6, 1000, 1400, typeof(ResponseGearShift)),
    //TestCase(4, 5, 1000, 1400, typeof(ResponseGearShift)),
    //TestCase(3, 4, 1000, 1400, typeof(ResponseGearShift)),
    //TestCase(2, 4, 1000, 1400, typeof(ResponseGearShift)),
    //TestCase(1, 2, 1000, 1400, typeof(ResponseGearShift)),
    //TestCase(8, 8, 1000, 1400, typeof(ResponseSuccess)),
    //TestCase(1, 6, 200, 9000, typeof(ResponseGearShift)),]
    //public void Gearbox_ShiftUp(int gear, int newGear, double tq, double n, Type responseType)
    //{
    //    var gearboxData = MockSimulationDataFactory.CreateGearboxDataFromFile(GearboxDataFile, EngineDataFile);

    //    var gearboxInput = JSONInputDataFactory.ReadGearbox(GearboxDataFile);
    //    var engineInput = JSONInputDataFactory.ReadEngine(EngineDataFile);
    //    var dao = new DeclarationDataAdapterHeavyLorry.Conventional();
    //    var engineData = dao.CreateEngineData(new MockDeclarationVehicleInputData() {
    //        EngineInputData = engineInput,
    //        GearboxInputData = gearboxInput
    //    }, engineInput.EngineModes.First(), new Mission() {
    //        MissionType = MissionType.LongHaul
    //    });
    //    foreach (var entry in gearboxData.Gears) {
    //        entry.Value.ShiftPolygon = DeclarationData.Gearbox.ComputeManualTransmissionShiftPolygon(
    //            (int)(entry.Key - 1), engineData.FullLoadCurves.First().Value,
    //            gearboxInput.Gears, engineData, ((IAxleGearInputData)gearboxInput).Ratio, 0.5.SI<Meter>());
    //    }

    //    var cycleData = DrivingCycleDataReader.ReadFromStream("s,v,grad,stop\n0,0,0,10\n10,20,0,0\n20,21,0,0\n30,22,0,0\n40,23,0,0\n50,24,0,0\n60,25,0,0\n70,26,0,0\n80,27,0,0\n90,28,0,0\n100,29,0,0".ToStream(), CycleType.DistanceBased, "DummyCycle", false);
    //    var container = new MockVehicleContainer() {
    //        VehicleSpeed = 10.SI<MeterPerSecond>(),
    //        DriverBehavior = DrivingBehavior.Driving,
    //        DrivingAction = DrivingAction.Accelerate,
    //        Altitude = 0.SI<Meter>(),
    //        VehicleMass = 10000.SI<Kilogram>(),
    //        ReducedMassWheels = 100.SI<Kilogram>(),
    //        TotalMass = 19000.SI<Kilogram>(),
    //        EngineSpeed = n.SI<PerSecond>(),
    //        RunData = GetDummyRunData(gearboxData),
    //        CycleData = new CycleData() {
    //            LeftSample = cycleData.Entries.First(),
    //        },
    //        ElectricMotorPositions = new PowertrainPosition[] { },
    //        HasCombustionEngine = true
    //    };
    //    var cycle = new MockDrivingCycle(container, cycleData);

    //    var gearbox = new Gearbox(container, new AMTShiftStrategy(container));
    //    var port = new MockTnOutPort() { EngineN95hSpeed = 2000.RPMtoRad() };
    //    container.EngineInfo = port;
    //    gearbox.InPort().Connect(port);

    //    var ratios = new[] { 0.0, 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };
    //    // the first element 0.0 is just a placeholder for axlegear, not used in this test

    //    var absTime = 0.SI<Second>();
    //    var dt = 2.SI<Second>();

    //    var expectedN = n.RPMtoRad();
    //    var angularVelocity = expectedN / ratios[gear];
    //    gearbox.OutPort().Initialize(1.SI<NewtonMeter>(), angularVelocity);

    //    absTime += dt;

    //    var expectedT = tq.SI<NewtonMeter>();


    //    var torque = expectedT * ratios[gear];


    //    gearbox.Gear = new GearshiftPosition((uint)gear);
    //    container.AbsTime = absTime;
    //    var response = gearbox.OutPort().Request(absTime, dt, torque, angularVelocity, false);
    //    NUnit.Framework.Assert.IsTrue(response.GetType() == responseType);

    //    absTime += dt;
    //    response = (ResponseSuccess)gearbox.OutPort().Request(absTime, dt, torque, angularVelocity, false);
    //    NUnit.Framework.Assert.AreEqual((uint)newGear, gearbox.Gear.Gear);
    //}

    const string EngineFldHdr = "engine speed [1/min],full load torque [Nm],motoring torque [Nm],PT1 [s]";
    
    static readonly string[] EngineFldData = new[] {
		"560,1180,-149,0.6		   ",
		"600,1282,-148,0.6		   ",
		"799.9999999,1791,-149,0.6 ",
		"1000,2300,-160,0.6		   ",
		"1200,2300,-179,0.6		   ",
		"1400,2300,-203,0.6		   ",
		"1599.999999,2079,-235,0.49",
		"1800,1857,-264,0.25	   ",
		"2000.000001,1352,-301,0.25",
		"2100,1100,-320,0.25	   ",
	};
}