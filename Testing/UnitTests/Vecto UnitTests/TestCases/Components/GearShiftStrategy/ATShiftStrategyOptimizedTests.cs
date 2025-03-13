using Moq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.GearShiftStrategy;

public class ATShiftStrategyOptimizedTests
{
	[Test,
	TestCase(0, 100, 1),
	TestCase(0, 200, 1),
	TestCase(5, 100, 1),
	TestCase(5, 300, 1),
	TestCase(5, 600, 1),
	TestCase(15, 100, 3),
	TestCase(15, 300, 3),
	TestCase(15, 600, 3),
	TestCase(40, 100, 6),
	TestCase(40, 300, 6),
	TestCase(40, 600, 6),
	TestCase(70, 100, 6),
	TestCase(70, 300, 6),
	TestCase(70, 600, 6),
	]
	public void TestATGearInitialize(double vehicleSpeed, double torque, int expectedGear)
	{
        var inputData = GetMockInputData();
		var gbxTypes = new[] {
			GearboxType.ATSerial
		};
		var runData = GetDummyVectoRunData(inputData.Components.GearboxInputData.Gears.Count);
		var shiftPolygonCalc = GetMockShiftPolygonCalc();
		
		// create gearbox data
		var tcDataAdapter = new TorqueConverterDataAdapter();
		var gearboxData = new GearboxDataAdapter(tcDataAdapter).CreateGearboxData(inputData, runData, shiftPolygonCalc, gbxTypes); 
		runData.GearboxData = gearboxData;

        var vehicleContainer = GetMockVehicleContainer(runData, out var vehicleInfo);
		vehicleInfo.Setup(v => v.VehicleSpeed).Returns(vehicleSpeed.KMPHtoMeterPerSecond());
		var shiftStrategy = new ATShiftStrategyOptimized(vehicleContainer);
        
		var gearbox = new APTGearbox(vehicleContainer, shiftStrategy);

		var mockPort = new Mock<ITnOutPort>();
		NewtonMeter tqRequest = null;
		PerSecond rpmRequest = null;
		mockPort.Setup(p => p.Initialize(It.IsAny<NewtonMeter>(),
			It.IsAny<PerSecond>())).Returns((NewtonMeter tq, PerSecond rpm) => {
				tqRequest = tq;
				rpmRequest = rpm;
				return new ResponseSuccess(this) {
					Engine = {
						EngineSpeed = rpm,
						PowerRequest = tq * rpm,
					}
				};
			});
		gearbox.Connect(mockPort.Object);

		// r_dyn = 0.465m, i_axle = 6.2
		var angularVelocity = vehicleSpeed.KMPHtoMeterPerSecond() / runData.VehicleData.DynamicTyreRadius * 6.2;

		var response = gearbox.Initialize(torque.SI<NewtonMeter>(), angularVelocity);

		Assert.IsInstanceOf(typeof(ResponseSuccess), response);
		Assert.AreEqual(expectedGear, gearbox.Gear.Gear);
		Assert.AreEqual(vehicleSpeed.IsEqual(0), gearbox.Disengaged);
	}

	private static VectoRunData GetDummyVectoRunData(int inputData)
	{
		var fldData = InputDataHelper.InputDataAsTableData(EngineFldHeader, EngineFldData);
		var fld = FullLoadCurveReader.Create(fldData);
		var engineIdlingSpeed = 600.RPMtoRad();
		var runData = new VectoRunData() {
			Cycle = new DrivingCycleData() {
				CycleType = CycleType.DistanceBased,
			},
			VehicleData = new VehicleData() {
				DynamicTyreRadius = 0.465.SI<Meter>(),
			},
			EngineData = new CombustionEngineData() {
				Inertia = 0.SI<KilogramSquareMeter>(),
				FullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>()
			},
			GearshiftParameters = new ShiftStrategyParameters() {
				LoadStageThresoldsUp = DeclarationData.GearboxTCU.LoadStageThresholdsUp,
				LoadStageThresoldsDown = DeclarationData.GearboxTCU.LoadStageThresoldsDown,
				ShiftSpeedsTCToLocked = engineIdlingSpeed == null ? null : DeclarationData.GearboxTCU.ShiftSpeedsTCToLocked
					.Select(x => x.Select(y => y + engineIdlingSpeed.AsRPM).ToArray()).ToArray(),
            }
		};
		for(uint i = 0; i <= inputData; i++)
			runData.EngineData.FullLoadCurves[i] = fld;
		return runData;
	}

	private static IShiftPolygonCalculator GetMockShiftPolygonCalc()
	{
		var shiftPolygonCalc = new Mock<IShiftPolygonCalculator>();
		var downshift = new List<ShiftPolygon.ShiftPolygonEntry>() { };
		var upshift = new List<ShiftPolygon.ShiftPolygonEntry>() { };
		var shiftpolygon = new ShiftPolygon(downshift, upshift);
		shiftPolygonCalc.Setup(s => s.ComputeDeclarationShiftPolygon(It.IsIn(GearboxType.ATSerial), It.IsAny<int>(),
				It.IsAny<EngineFullLoadCurve>(), It.IsAny<IList<ITransmissionInputData>>(),
				It.IsAny<CombustionEngineData>(), It.IsAny<double>(), It.IsAny<Meter>(), It.IsAny<ElectricMotorData>()))
			.Returns(shiftpolygon);
		return shiftPolygonCalc.Object;
	}

	private static IVehicleContainer GetMockVehicleContainer(VectoRunData runData, out Mock<IVehicleInfo> vehicleInfo)
	{
		var vehicleContainer = new Mock<IVehicleContainer>();
		vehicleInfo = new Mock<IVehicleInfo>();
		var engineInfo = new Mock<IEngineInfo>();
		var ptBuilder = new Mock<ISimplePowertrainBuilder>();
		var testPt = new Mock<ITestPowertrain>();
		var testContainer = new Mock<ISimpleVehicleContainer>();
		var vehiclePort = new Mock<ITestPowertrainVehicle>();
		var testGbx = new Mock<ITestPowertrainTransmission>();

		vehicleContainer.Setup(c => c.RunData).Returns(runData);
		vehicleContainer.Setup(c => c.VehicleInfo).Returns(vehicleInfo.Object);
		vehicleContainer.Setup(c => c.EngineInfo).Returns(engineInfo.Object);
		vehicleContainer.Setup(c => c.SimplePowertrainBuilder).Returns(ptBuilder.Object);
		
		engineInfo.Setup(e => e.EngineIdleSpeed).Returns(600.RPMtoRad());
		engineInfo.Setup(e => e.EngineRatedSpeed).Returns(2000.RPMtoRad());

		ptBuilder.Setup(b => b.CreateTestPowertrain(It.IsAny<IVehicleContainer>(), It.IsAny<bool>())).Returns(testPt.Object);

		testPt.Setup(t => t.Gearbox).Returns(testGbx.Object);
		testPt.Setup(t => t.Container).Returns(testContainer.Object);
		testPt.Setup(t => t.Vehicle).Returns(vehiclePort.Object);

        testContainer.Setup(c => c.RunData).Returns(runData);
		testContainer.Setup(c => c.GearboxCtl).Returns(new APTGearbox(testContainer.Object, null));
		
		return vehicleContainer.Object;
	}

	private IVehicleDeclarationInputData GetMockInputData()
	{
		var input = new Mock<IVehicleDeclarationInputData>();
		var components = new Mock<IVehicleComponentsDeclaration>();
		input.Setup(i => i.Components).Returns(components.Object);
		var gbx = new Mock<IGearboxDeclarationInputData>();
		var tc = new Mock<ITorqueConverterDeclarationInputData>();

		components.Setup(c => c.GearboxInputData).Returns(gbx.Object);
		components.Setup(c => c.TorqueConverterInputData).Returns(tc.Object);
		var gearRatios = new double[] {
			3.4, 1.9, 1.42, 1.0, 0.7, 0.62
		};
		var header = "Input Speed [rpm],Input Torque [Nm],Torque Loss [Nm]";
		var efficiency = 0.98;
		var data = new List<string>();
		foreach (var speed in new[] {0, 10000}) {
			foreach (var tq in new[] {1e5, -1e5, 0}) {
				data.Add($"{speed:f2}, {tq:f2}, {(1 - efficiency) * Math.Abs(tq)}");
			}
		}
        var lossmap = InputDataHelper.InputDataAsTableData(header, data.ToArray());
		var gears = gearRatios.Select((x, idx) => {
			var gear = new Mock<ITransmissionInputData>();
			gear.Setup(g => g.Ratio).Returns(x);
			gear.Setup(g => g.Gear).Returns(idx + 1);
			//gear.Setup(g => g.Efficiency).Returns(0.98);
			gear.Setup(g => g.LossMap).Returns(lossmap);
			return gear.Object;
		}).ToList();
		gbx.Setup(g => g.Type).Returns(GearboxType.ATSerial);
		gbx.Setup(g => g.Gears).Returns(gears);

		var tcData = InputDataHelper.InputDataAsTableData(TcHeader, TcData);
		tc.Setup(t => t.TCData).Returns(tcData);
		return input.Object;
	}

	public const string TcHeader = "Speed Ratio, Torque Ratio,MP1000";

	public static readonly string[] TcData = new[] {
		"0.0,1.80,377.80",
		"0.1,1.71,365.21",
		"0.2,1.61,352.62",
		"0.3,1.52,340.02",
		"0.4,1.42,327.43",
		"0.5,1.33,314.84",
		"0.6,1.23,302.24",
		"0.7,1.14,264.46",
		"0.8,1.04,226.68",
		"0.9,1.02,188.90",
		"1.0,1.0,0.00",
		"1.100,0.999,-40.34",
		"1.222,0.998,-80.34",
		"1.375,0.997,-136.11",
		"1.571,0.996,-216.52",
		"1.833,0.995,-335.19",
		"2.200,0.994,-528.77",
		"2.750,0.993,-883.40",
		"4.400,0.992,-2462.17",
		"11.000,0.991,-16540.98",
	};

	public const string EngineFldHeader = "engine speed [1/min],full load torque [Nm],motoring torque [Nm],PT1 [s]";

	public static readonly string[] EngineFldData = new[] {
		"560,1180,-149,0.6",
		"600,1282,-148,0.6",
		"799.9999999,1791,-149,0.6",
		"1000,2300,-160,0.6",
		"1200,2300,-179,0.6",
		"1400,2300,-203,0.6",
		"1599.999999,2079,-235,0.49",
		"1800,1857,-264,0.25",
		"2000.000001,1352,-301,0.25",
		"2100,1100,-320,0.25",
	};
}