using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestFixture]
	public class ElectricMotorTest
	{
		public const string MotorFile = @"TestData\Hybrids\ElectricMotor\GenericEMotor.vem";
		public const string BatFile = @"TestData\Hybrids\Battery\GenericBattery.vbat";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase(100, 100, -1479.601019),
		 TestCase(100, 30, -494.148831),
		 TestCase(100, 300, -5033.132712),
		 TestCase(600, 100, -7290.510011),
		 TestCase(600, 300, -21431.717255),
		 TestCase(800, -100, 7178.770573),
		 TestCase(800, -300, 22444.155535)]
		public void ElectricMotorOnlyRequestTest(double speed, double torque, double expectedBatteryPower)
		{
			var container = new MockVehicleContainer();

			var inputData = JSONInputDataFactory.ReadElectricMotorData(MotorFile, false);
			var dao = new EngineeringDataAdapter();
			var electricMachine = new MockElectricMachinesInputData() {
				Entries = new List<ElectricMachineEntry<IElectricMotorEngineeringInputData>>() {
					new ElectricMachineEntry<IElectricMotorEngineeringInputData>() {
						ElectricMachine = inputData,
						Count = 1,
						Ratio = 1,
						MechanicalEfficiency = 1
					}
				}
			};
			var data = dao.CreateElectricMachines(electricMachine);
			var strategy = new MockHybridControl();

			var battery = new MockBattery();
			var motor = new ElectricMotor(container, data.First().Item2, strategy, PowertrainPosition.HybridP2);
			var es = new ElectricSystem(container);
			es.Connect(battery);
			motor.Connect(es);

			strategy.ElectricShare = -torque.SI<NewtonMeter>();
			motor.Initialize(0.SI<NewtonMeter>(), speed.RPMtoRad());

			var response = motor.Request(0.SI<Second>(), 0.5.SI<Second>(), torque.SI<NewtonMeter>(), speed.RPMtoRad());

			Assert.IsInstanceOf<ResponseSuccess>(response);
			var enginePower = speed.RPMtoRad() * strategy.ElectricShare;
			Assert.AreEqual(0, response.Engine.PowerRequest.Value(), 1e-6);
			Assert.AreEqual(enginePower.Value(), response.ElectricMotor.ElectricMotorPowerMech.Value(), 1e-6);
			Assert.AreEqual(expectedBatteryPower, response.ElectricSystem.ConsumerPower.Value(), 1e-6);
			Assert.AreEqual(expectedBatteryPower, response.ElectricSystem.BatteryResponse.BatteryPower.Value(), 1e-6);
			Assert.IsTrue(response.ElectricSystem.ConsumerPower.Value() < enginePower.Value());
		}


		[TestCase(100, 100, -30, -494.148831),
		TestCase(100, 300, -150, -2265.054223),
		TestCase(600, 100, 100, 5368.366615),
		TestCase(600, 300, -50, -3926.835416),
		TestCase(800, -100, 200, 14945.984737),
		TestCase(800, -300, 200, 14945.984737),]
		public void ElectricMotorAssistingRequestTest(double speed, double torque, double electricTorque, double expectedBatteryPower)
		{
			var container = new MockVehicleContainer();

			var inputData = JSONInputDataFactory.ReadElectricMotorData(MotorFile, false);
			var dao = new EngineeringDataAdapter();
			var electricMachine = new MockElectricMachinesInputData()
			{
				Entries = new List<ElectricMachineEntry<IElectricMotorEngineeringInputData>>() {
					new ElectricMachineEntry<IElectricMotorEngineeringInputData>() {
						ElectricMachine = inputData,
						Count = 1,
						Ratio = 1,
						MechanicalEfficiency = 1
					}
				}
			};
			var data = dao.CreateElectricMachines(electricMachine);
			var strategy = new MockHybridControl();

			var battery = new MockBattery();
			var motor = new ElectricMotor(container, data.First().Item2, strategy, PowertrainPosition.HybridP2);
			var es = new ElectricSystem(container);
			es.Connect(battery);
			motor.Connect(es);
			var tnPort = new MockTnOutPort();
			motor.Connect(tnPort);

			strategy.ElectricShare = electricTorque.SI<NewtonMeter>();
			motor.Initialize(0.SI<NewtonMeter>(), speed.RPMtoRad());

			var response = motor.Request(0.SI<Second>(), 0.5.SI<Second>(), torque.SI<NewtonMeter>(), speed.RPMtoRad());

			Assert.IsInstanceOf<ResponseSuccess>(response);
			var enginePower = speed.RPMtoRad() * (torque + electricTorque).SI<NewtonMeter>();
			var motorMechPower = speed.RPMtoRad() * electricTorque.SI<NewtonMeter>();
			Assert.AreEqual(enginePower.Value(), response.Engine.PowerRequest.Value(), 1e-6);
			Assert.AreEqual(motorMechPower, response.ElectricMotor.ElectricMotorPowerMech);
			Assert.AreEqual(expectedBatteryPower, response.ElectricSystem.ConsumerPower.Value(), 1e-6);
			Assert.AreEqual(expectedBatteryPower, response.ElectricSystem.BatteryResponse.BatteryPower.Value(), 1e-6);
			Assert.IsTrue(response.ElectricSystem.ConsumerPower.Value() < response.ElectricMotor.ElectricMotorPowerMech.Value());
		}

		[TestCase(800, 300)]
		public void ElectricMotorWithBatteryIdlingRequestTest(double speed, double torque)
		{
			var container = new MockVehicleContainer();

			var inputData = JSONInputDataFactory.ReadElectricMotorData(MotorFile, false);
			var dao = new EngineeringDataAdapter();
			var electricMachine = new MockElectricMachinesInputData()
			{
				Entries = new List<ElectricMachineEntry<IElectricMotorEngineeringInputData>>() {
					new ElectricMachineEntry<IElectricMotorEngineeringInputData>() {
						ElectricMachine = inputData,
						Count = 1,
						Ratio = 1,
						MechanicalEfficiency = 1
					}
				}
			};
			var data = dao.CreateElectricMachines(electricMachine);
			var strategy = new MockHybridControl();

			var batInput = JSONInputDataFactory.ReadBatteryData(BatFile, false);
			var tmp = new MockBatteryInputData() {
				BatteryPack = batInput,
				Count = 1
			};
			var batteryData = dao.CreateBatteryData(tmp, 0.8);
			var battery = new Battery(container, batteryData);
			var es = new ElectricSystem(container);
			es.Connect(battery);

			battery.Initialize(0.5);
			var motor = new ElectricMotor(container, data.First().Item2, strategy, PowertrainPosition.HybridP2);
			motor.Connect(es);
			var tnPort = new MockTnOutPort();
			motor.Connect(tnPort);

			strategy.ElectricShare = null; //0.SI<NewtonMeter>();
			motor.Initialize(0.SI<NewtonMeter>(), speed.RPMtoRad());

			var absTime = 0.SI<Second>();
			var dt = 0.5.SI<Second>();
			var response = motor.Request(absTime, dt, torque.SI<NewtonMeter>(), speed.RPMtoRad());

			Assert.IsInstanceOf<ResponseSuccess>(response);
			var dragTorque = data.First().Item2.DragCurve.Lookup(speed.RPMtoRad());
			var enginePower = speed.RPMtoRad() * (torque.SI<NewtonMeter>() + dragTorque);
			var motorMechPower = dragTorque * speed.RPMtoRad();
			Assert.AreEqual(enginePower.Value(), response.Engine.PowerRequest.Value(), 1e-6);
			Assert.AreEqual(motorMechPower.Value(), response.ElectricMotor.ElectricMotorPowerMech.Value(), 1e-6);
			Assert.AreEqual(0, response.ElectricSystem.ConsumerPower.Value(), 1e-6);
			Assert.AreEqual(0, response.ElectricSystem.BatteryResponse.BatteryPower.Value(), 1e-6);
			var modData = new MockModalDataContainer();
			battery.CommitSimulationStep(absTime, dt, modData);
		}

		[TestCase(0.5, 100, 100, -1479.601019, 2.674905),
		TestCase(0.5, 100, 300, -5033.132712, 31.224759),
		TestCase(0.5, 600, 100, -7290.510011, 65.884061),
		TestCase(0.5, 600, 300, -21431.717255, 590.431866),
		TestCase(0.5, 800, -100, 7178.770573, 61.667578),
		TestCase(0.5, 800, -300, 22444.155535, 581.889779)
		]
		public void ElectricMotorOnlyWithBatteryRequestTest(double initialSoc, double speed, double torque, double expectedBatteryPower, double expectedBatteryLoss)
		{
			var container = new MockVehicleContainer();

			var inputData = JSONInputDataFactory.ReadElectricMotorData(MotorFile, false);
			var batInput = JSONInputDataFactory.ReadBatteryData(BatFile, false);
			var dao = new EngineeringDataAdapter();
			var electricMachine = new MockElectricMachinesInputData()
			{
				Entries = new List<ElectricMachineEntry<IElectricMotorEngineeringInputData>>() {
					new ElectricMachineEntry<IElectricMotorEngineeringInputData>() {
						ElectricMachine = inputData,
						Count = 1,
						Ratio = 1,
						MechanicalEfficiency = 1
					}
				}
			};
			var data = dao.CreateElectricMachines(electricMachine);
			var strategy = new MockHybridControl();

			var tmp = new MockBatteryInputData()
			{
				BatteryPack = batInput,
				Count = 1
			};
			var batteryData = dao.CreateBatteryData(tmp, 0.8);
			var battery = new Battery(container, batteryData);
			var es = new ElectricSystem(container);
			es.Connect(battery);
			battery.Initialize(initialSoc);
			var motor = new ElectricMotor(container, data.First().Item2, strategy, PowertrainPosition.HybridP2);
			motor.Connect(es);

			strategy.ElectricShare = -torque.SI<NewtonMeter>();
			motor.Initialize(0.SI<NewtonMeter>(), speed.RPMtoRad());

			var response = motor.Request(0.SI<Second>(), 0.5.SI<Second>(), torque.SI<NewtonMeter>(), speed.RPMtoRad());

			Assert.IsInstanceOf<ResponseSuccess>(response);
			var enginePower = speed.RPMtoRad() * strategy.ElectricShare;
			Assert.AreEqual(0, response.Engine.PowerRequest.Value(), 1e-6);
			Assert.AreEqual(enginePower.Value(), response.ElectricMotor.ElectricMotorPowerMech.Value(), 1e-6);
			Assert.AreEqual(expectedBatteryPower, response.ElectricSystem.ConsumerPower.Value(), 1e-6);
			Assert.AreEqual(expectedBatteryPower, response.ElectricSystem.BatteryResponse.BatteryPower.Value(), 1e-6);
			Assert.IsTrue(response.ElectricSystem.ConsumerPower.Value() < enginePower.Value());
			Assert.AreEqual(expectedBatteryLoss, response.ElectricSystem.BatteryResponse.BatteryLoss.Value(), 1e-4);
		}
	}

}