/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System.Collections.Generic;
using System.Data;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

// ReSharper disable UnusedVariable
// ReSharper disable NotAccessedVariable
// ReSharper disable CollectionNeverQueried.Local
// ReSharper disable RedundantAssignment

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestFixture]
	public class CombustionEngineTest
	{
		protected double Tolerance = 1E-3;

		private const string CoachEngine = @"TestData\Components\24t Coach.veng";

		private const string TruckEngine = @"TestData\Components\40t_Long_Haul_Truck.veng";

		[TestCase]
		public void TestEngineHasOutPort()
		{
			var vehicle = new VehicleContainer(ExecutionMode.EngineOnly);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(CoachEngine);
			var engine = new CombustionEngine(vehicle, engineData);

			var port = engine.OutPort();
			Assert.IsNotNull(port);
		}

		[TestCase]
		public void TestOutPortRequestNotFailing()
		{
			var vehicle = new VehicleContainer(ExecutionMode.EngineOnly);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(CoachEngine);
			var engine = new CombustionEngine(vehicle, engineData);

			var port = engine.OutPort();

			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();
			var torque = 400.SI<NewtonMeter>();
			var engineSpeed = 1500.RPMtoRad();

			port.Initialize(torque, engineSpeed);
			port.Request(absTime, dt, torque, engineSpeed);
		}

		[TestCase]
		public void TestSimpleModalData()
		{
			var vehicle = new VehicleContainer(ExecutionMode.Engineering);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(CoachEngine);
			var engine = new CombustionEngine(vehicle, engineData);
			var port = engine.OutPort();

			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();

			var torque = 0.SI<NewtonMeter>();
			var engineSpeed = 600.RPMtoRad();
			var dataWriter = new MockModalDataContainer();

			port.Initialize(torque, engineSpeed);
			for (var i = 0; i < 21; i++) {
				port.Request(absTime, dt, torque, engineSpeed);
				engine.CommitSimulationStep(dataWriter);
				if (i > 0) {
					dataWriter.CommitSimulationStep(absTime, dt);
				}
				absTime += dt;
			}

			engineSpeed = 644.4445.RPMtoRad();
			port.Request(absTime, dt, Formulas.PowerToTorque(2329.973.SI<Watt>(), engineSpeed), engineSpeed);
			engine.CommitSimulationStep(dataWriter);

			AssertHelper.AreRelativeEqual(1152.40304, ((SI)dataWriter[ModalResultField.P_eng_inertia]).Value());

			dataWriter.CommitSimulationStep(absTime, dt);
			absTime += dt;

			var power = new[] { 569.3641, 4264.177 };
			for (var i = 0; i < 2; i++) {
				port.Request(absTime, dt, Formulas.PowerToTorque(power[i].SI<Watt>(), engineSpeed), engineSpeed);
				engine.CommitSimulationStep(dataWriter);
				dataWriter.CommitSimulationStep(absTime, dt);
				absTime += dt;
			}

			engineSpeed = 869.7512.RPMtoRad();
			port.Request(absTime, dt, Formulas.PowerToTorque(7984.56.SI<Watt>(), engineSpeed), engineSpeed);
			engine.CommitSimulationStep(dataWriter);

			Assert.AreEqual(7108.32, ((SI)dataWriter[ModalResultField.P_eng_inertia]).Value(), 0.001);
			dataWriter.CommitSimulationStep(absTime, dt);
			absTime += dt;

			engineSpeed = 644.4445.RPMtoRad();
			port.Request(absTime, dt, Formulas.PowerToTorque(1351.656.SI<Watt>(), engineSpeed), engineSpeed);
			engine.CommitSimulationStep(dataWriter);

			Assert.AreEqual(-7108.32, ((SI)dataWriter[ModalResultField.P_eng_inertia]).Value(), 0.001);
			dataWriter.CommitSimulationStep(absTime, dt);

			dataWriter.Data.WriteToFile(@"test1.csv");
		}

		[TestCase("Test1Hz", @"TestData\Components\24t Coach.veng", 1000, 50, 50,
			@"TestData\Results\EngineFullLoadJumps\EngineFLJ_1000rpm_1Hz.csv")]
		[TestCase("Test10Hz", @"TestData\Components\24t Coach.veng", 1000, 50, 50,
			@"TestData\Results\EngineFullLoadJumps\EngineFLJ_1000rpm_10Hz.csv")]
		[TestCase("TestvarHz", @"TestData\Components\24t Coach.veng", 1000, 50, 50,
			@"TestData\Results\EngineFullLoadJumps\EngineFLJ_1000rpm_varHz.csv")]
		[TestCase("Test10Hz", @"TestData\Components\24t Coach_IncPT1.veng", 1000, 50, 50,
			@"TestData\Results\EngineFullLoadJumps\EngineFLJ_1000rpm_10Hz_IncPT1.csv")]
		public void TestEngineOnlyEngineFullLoadJump(string testName, string engineFile, double rpm, double initialIdleLoad,
			double finalIdleLoad, string resultFile)
		{
			var vehicleContainer = new VehicleContainer(ExecutionMode.Engineering);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(engineFile);
			var engine = new EngineOnlyCombustionEngine(vehicleContainer, engineData);

			var expectedResults = VectoCSVFile.Read(resultFile);

			var requestPort = engine.OutPort();

			//var modalData = new ModalDataWriter(string.Format("load_jump_{0}.csv", TestContext.DataRow["TestName"].ToString()));
			var modalData = new MockModalDataContainer();

			var idlePower = initialIdleLoad.SI<Watt>();

			var angularSpeed = rpm.RPMtoRad();

			var t = 0.SI<Second>();
			var dt = 0.1.SI<Second>();
			requestPort.Initialize(Formulas.PowerToTorque(idlePower, angularSpeed), angularSpeed);
			for (; t < 2; t += dt) {
				requestPort.Request(t, dt, Formulas.PowerToTorque(idlePower, angularSpeed), angularSpeed);
				engine.CommitSimulationStep(modalData);
			}

			var i = 0;
			// dt = TimeSpan.FromSeconds(double.Parse(TestContext.DataRow["dt"].ToString(), CultureInfo.InvariantCulture));
			// dt = TimeSpan.FromSeconds(expectedResults.Rows[i].ParseDouble(0)) - t;
			var engineLoadPower = engineData.FullLoadCurve.FullLoadStationaryPower(angularSpeed);
			idlePower = finalIdleLoad.SI<Watt>();
			for (; t < 25; t += dt, i++) {
				dt = (expectedResults.Rows[i + 1].ParseDouble(0) - expectedResults.Rows[i].ParseDouble(0)).SI<Second>();
				if (t >= 10.SI<Second>()) {
					engineLoadPower = idlePower;
				}
				requestPort.Request(t, dt, Formulas.PowerToTorque(engineLoadPower, angularSpeed), angularSpeed);
				modalData[ModalResultField.time] = t;
				modalData[ModalResultField.simulationInterval] = dt;
				engine.CommitSimulationStep(modalData);
				Assert.AreEqual(expectedResults.Rows[i].ParseDouble(0), t.Value(), 0.001, "Time");
				Assert.AreEqual(expectedResults.Rows[i].ParseDouble(1), ((SI)modalData[ModalResultField.P_eng_full]).Value(), 0.1,
					string.Format("Load in timestep {0}", t));
				modalData.CommitSimulationStep();
			}
			modalData.Finish(VectoRun.Status.Success);
		}

		[TestCase("Test1Hz", @"TestData\Components\24t Coach.veng", 1000, 50, 50,
			@"TestData\Results\EngineFullLoadJumps\EngineFLJ_1000rpm_1Hz.csv")]
		[TestCase("Test10Hz", @"TestData\Components\24t Coach.veng", 1000, 50, 50,
			@"TestData\Results\EngineFullLoadJumps\EngineFLJ_1000rpm_10Hz.csv")]
		[TestCase("TestvarHz", @"TestData\Components\24t Coach.veng", 1000, 50, 50,
			@"TestData\Results\EngineFullLoadJumps\EngineFLJ_1000rpm_varHz.csv")]
		[TestCase("Test10Hz", @"TestData\Components\24t Coach_IncPT1.veng", 1000, 50, 50,
			@"TestData\Results\EngineFullLoadJumps\EngineFLJ_1000rpm_10Hz_IncPT1.csv")]
		public void TestEngineFullLoadJump(string testName, string engineFile, double rpm, double initialIdleLoad,
			double finalIdleLoad, string resultFile)
		{
			var vehicleContainer = new VehicleContainer(ExecutionMode.Engineering);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(engineFile);
			var engine = new CombustionEngine(vehicleContainer, engineData);

			var expectedResults = VectoCSVFile.Read(resultFile);

			var requestPort = engine.OutPort();

			//var modalData = new ModalDataWriter(string.Format("load_jump_{0}.csv", TestName));
			var modalData = new MockModalDataContainer();

			var idlePower = initialIdleLoad.SI<Watt>();

			var angularSpeed = rpm.RPMtoRad();

			var t = 0.SI<Second>();
			var dt = 0.1.SI<Second>();
			requestPort.Initialize(Formulas.PowerToTorque(idlePower, angularSpeed), angularSpeed);
			for (; t < 2; t += dt) {
				requestPort.Request(t, dt, Formulas.PowerToTorque(idlePower, angularSpeed), angularSpeed);
				engine.CommitSimulationStep(modalData);
			}

			var i = 0;
			// dt = TimeSpan.FromSeconds(double.Parse(TestContext.DataRow["dt"].ToString(), CultureInfo.InvariantCulture));
			// dt = TimeSpan.FromSeconds(expectedResults.Rows[i].ParseDouble(0)) - t;
			var engineLoadPower = engineData.FullLoadCurve.FullLoadStationaryPower(angularSpeed);
			idlePower = finalIdleLoad.SI<Watt>();
			for (; t < 25; t += dt, i++) {
				dt = (expectedResults.Rows[i + 1].ParseDouble(0) - expectedResults.Rows[i].ParseDouble(0)).SI<Second>();
				if (t >= 10.SI<Second>()) {
					engineLoadPower = idlePower;
				}
				requestPort.Request(t, dt, Formulas.PowerToTorque(engineLoadPower, angularSpeed), angularSpeed);
				modalData[ModalResultField.time] = t;
				modalData[ModalResultField.simulationInterval] = dt;
				engine.CommitSimulationStep(modalData);
				Assert.AreEqual(expectedResults.Rows[i].ParseDouble(0), t.Value(), 0.001, "Time");
				Assert.AreEqual(expectedResults.Rows[i].ParseDouble(1), ((SI)modalData[ModalResultField.P_eng_full]).Value(), 0.1,
					string.Format("Load in timestep {0}", t));
				modalData.CommitSimulationStep();
			}
			modalData.Finish(VectoRun.Status.Success);
		}

		[TestCase]
		public void EngineIdleJump()
		{
			var container = new VehicleContainer(ExecutionMode.Engineering);
			var gearbox = new MockGearbox(container);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(CoachEngine);

			var engine = new CombustionEngine(container, engineData);
			var clutch = new Clutch(container, engineData, engine.IdleController);

			var d = new MockDriver(container);

			var aux = new EngineAuxiliary(container);
			aux.AddConstant("", 5000.SI<Watt>());

			gearbox.Gear = 1;

			//gearbox.InPort().Connect(engine.OutPort());
			gearbox.InPort().Connect(clutch.OutPort());
			clutch.InPort().Connect(engine.OutPort());
			engine.Connect(aux.Port());
			engine.IdleController.RequestPort = clutch.IdleControlPort;

			//			var expectedResults = VectoCSVFile.Read(TestContext.DataRow["ResultFile"].ToString());

			var requestPort = gearbox.OutPort();

			//vehicleContainer.DataWriter = new ModalDataWriter("engine_idle_test.csv");
			var dataWriter = new MockModalDataContainer();
			container.ModData = dataWriter;

			var torque = 1200.SI<NewtonMeter>();
			var angularVelocity = 800.RPMtoRad();

			// initialize engine...

			gearbox.Initialize(torque, angularVelocity);

			var absTime = 0.SI<Second>();
			var dt = Constants.SimulationSettings.TargetTimeInterval;

			var response = (ResponseSuccess)requestPort.Request(absTime, dt, torque, angularVelocity);

			container.CommitSimulationStep(absTime, dt);
			var row = dataWriter.Data.Rows.Cast<DataRow>().Last();
			Assert.AreEqual(100530.96491487339.SI<Watt>().Value(), ((SI)row[ModalResultField.P_eng_out.GetName()]).Value());
			Assert.AreEqual(105530.96491487339.SI<Watt>().Value(), ((SI)row[ModalResultField.P_eng_fcmap.GetName()]).Value());
			Assert.AreEqual(5000.SI<Watt>(), row[ModalResultField.P_aux.GetName()]);
			Assert.AreEqual(800.RPMtoRad(), row[ModalResultField.n_eng_avg.GetName()]);

			absTime += dt;

			// actual test...

			gearbox.Gear = 0;
			torque = 0.SI<NewtonMeter>();

			response = (ResponseSuccess)gearbox.Request(absTime, dt, torque, angularVelocity);

			container.CommitSimulationStep(absTime, dt);
			row = dataWriter.Data.Rows.Cast<DataRow>().Last();

			Assert.AreEqual(0.SI<Watt>(), row[ModalResultField.P_eng_out.GetName()]);
			Assert.AreEqual(5000.SI<Watt>(), row[ModalResultField.P_aux.GetName()]);
			Assert.AreEqual(800.RPMtoRad(), row[ModalResultField.n_eng_avg.GetName()]);
		}

		[TestCase]
		public void EngineIdleControllerTestCoach()
		{
			VehicleContainer container;
			CombustionEngine engine;
			ITnOutPort requestPort;
			VehicleContainer(CoachEngine, out container, out engine, out requestPort);

			var absTime = 0.SI<Second>();
			var dt = Constants.SimulationSettings.TargetTimeInterval;

			var angularVelocity = 800.RPMtoRad();
			var torque = 100000.SI<Watt>() / angularVelocity;

			var response = (ResponseSuccess)requestPort.Initialize(torque, angularVelocity);

			response = (ResponseSuccess)requestPort.Request(absTime, dt, torque, angularVelocity);
			Assert.AreEqual(105000, response.EnginePowerRequest.Value(), Tolerance);
			container.CommitSimulationStep(absTime, dt);
			absTime += dt;

			var engineSpeed = new[] { 800.RPMtoRad(), 800.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad() };
			var enginePower = new[] { 5000.SI<Watt>(), 5000.SI<Watt>(), -8601.6308.SI<Watt>(), 5000.SI<Watt>() };

			for (var i = 0; i < engineSpeed.Length; i++) {
				torque = 0.SI<NewtonMeter>();

				response = (ResponseSuccess)requestPort.Request(absTime, dt, torque, null);
				container.CommitSimulationStep(absTime, dt);
				absTime += dt;

				Assert.AreEqual(engineSpeed[i].Value(), engine.PreviousState.EngineSpeed.Value(), Tolerance, "i: {0}", i);
				Assert.AreEqual(enginePower[i].Value(), engine.PreviousState.EnginePower.Value(), Tolerance, "i: {0}", i);
			}
		}

		/*
		 * VECTO 2.2
		| time [s] | P_eng_out [kW] | n_eng_avg [1/min] | T_eng_fcmap [Nm] | Gear [-] |
		| 59.5     | 349.981     | 1679.281  | 1990.181    | 8        |
		| 60.5     | 5           | 1679.281  | 28.43269    | 0        |
		| 61.5     | -19.47213   | 1397.271  | -133.0774   | 0        |
		| 62.5     | -18.11888   | 1064.296  | -162.5699   | 0        |
		| 63.5     | -11.11163   | 714.1923  | -148.571    | 0        |
		| 64.5     | -0.5416708  | 560       | -9.236741   | 0        |
		| 65.5     | 5           | 560       | 85.26157    | 0        |
		| 66.5     | 5           | 560       | 85.26157    | 0        |
		| 67.5     | 5           | 560       | 85.26157    | 0        |
		| 68.5     | 5           | 560       | 85.26157    | 0        |
		| 69.5     | 5           | 560       | 85.26157    | 0        |
		| 70.5     | 308.729     | 1284.139  | 2295.815    | 9        |
				*/

		[TestCase]
		public void EngineIdleControllerTestTruck()
		{
			VehicleContainer container;
			CombustionEngine engine;
			ITnOutPort requestPort;
			VehicleContainer(TruckEngine, out container, out engine, out requestPort);

			//var dataWriter = new ModalDataWriter("EngienIdle.vmod");
			//container.DataWriter = dataWriter;

			var absTime = 0.SI<Second>();
			var dt = Constants.SimulationSettings.TargetTimeInterval;

			var angularVelocity = 1680.RPMtoRad();
			var torque = 345000.SI<Watt>() / angularVelocity;

			var response = (ResponseSuccess)requestPort.Initialize(torque, angularVelocity);

			response = (ResponseSuccess)requestPort.Request(absTime, dt, torque, angularVelocity);
			Assert.AreEqual(350000, response.EnginePowerRequest.Value(), Tolerance);
			container.CommitSimulationStep(absTime, dt);
			absTime += dt;

			var engineSpeed = new[] {
				1680.RPMtoRad(), 1680.RPMtoRad(), 1424.880146.RPMtoRad(), 1201.792344.RPMtoRad(), 998.69122.RPMtoRad(),
				805.9864149.RPMtoRad(), 612.5100267.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(),
				560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(),
				560.RPMtoRad()
			};

			var enginePower = new[] {
				5000.SI<Watt>(), 5000.SI<Watt>(), -36967.1249.SI<Watt>(), -26488.7680.SI<Watt>(), -19531.9403.SI<Watt>(),
				-14611.2765.SI<Watt>(), -10490.8989.SI<Watt>(), 1524.8368.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>(),
				5000.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>(),
				5000.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>()
			};

			var engSpeedResults = new List<dynamic>();
			for (var i = 0; i < engineSpeed.Length; i++) {
				torque = 0.SI<NewtonMeter>();

				response = (ResponseSuccess)requestPort.Request(absTime, dt, torque, null);

				container.CommitSimulationStep(absTime, dt);

				engSpeedResults.Add(new { absTime, engine.PreviousState.EngineSpeed, engine.PreviousState.EnginePower });
				Assert.AreEqual(engineSpeed[i].Value(), engine.PreviousState.EngineSpeed.Value(), Tolerance);
				Assert.AreEqual(enginePower[i].Value(), engine.PreviousState.EnginePower.Value(), Tolerance);

				absTime += dt;
			}
			//dataWriter.Finish();
		}

		[TestCase]
		public void EngineIdleControllerTest2Truck()
		{
			VehicleContainer container;
			CombustionEngine engine;
			ITnOutPort requestPort;
			VehicleContainer(TruckEngine, out container, out engine, out requestPort);

			//var dataWriter = new ModalDataWriter("EngienIdle.vmod");
			//container.DataWriter = dataWriter;

			var absTime = 0.SI<Second>();
			var dt = Constants.SimulationSettings.TargetTimeInterval;

			var angularVelocity = 95.5596.SI<PerSecond>();

			var torque = (engine.ModelData.FullLoadCurve.DragLoadStationaryPower(angularVelocity) - 5000.SI<Watt>()) /
						angularVelocity;

			var response = (ResponseSuccess)requestPort.Initialize(torque, angularVelocity);

			response = (ResponseSuccess)requestPort.Request(absTime, dt, torque, angularVelocity);
			Assert.AreEqual(-14829.79713, response.EnginePowerRequest.Value(), Tolerance);
			container.CommitSimulationStep(absTime, dt);
			absTime += dt;

			var engineSpeed = new[] {
				1680.RPMtoRad(), 1680.RPMtoRad(), 1467.014.RPMtoRad(), 1272.8658.RPMtoRad(), 1090.989.RPMtoRad(),
				915.3533.RPMtoRad(), 738.599.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(),
				560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(),
				560.RPMtoRad()
			};

			//var enginePower = new[] {
			//	5000.SI<Watt>(), 5000.SI<Watt>(), -32832.8834.SI<Watt>(), -25025.1308.SI<Watt>(), -19267.0360.SI<Watt>(),
			//	-14890.1962.SI<Watt>(), -11500.7991.SI<Watt>(), -8091.0577.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>(),
			//	5000.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>(),
			//	5000.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>()
			//};

			var engSpeedResults = new List<dynamic>();
			torque = 0.SI<NewtonMeter>();
			for (var i = 0; i < engineSpeed.Length; i++) {
				response = (ResponseSuccess)requestPort.Request(absTime, dt, torque, null);

				container.CommitSimulationStep(absTime, dt);

				engSpeedResults.Add(new { absTime, engine.PreviousState.EngineSpeed, engine.PreviousState.EnginePower });
				//Assert.AreEqual(engineSpeed[i].Value(), engine.PreviousState.EngineSpeed.Value(), Tolerance);
				//Assert.AreEqual(enginePower[i].Value(), engine.PreviousState.EnginePower.Value(), Tolerance);

				absTime += dt;
			}
			//dataWriter.Finish();
		}

		[TestCase]
		public void Test_EngineData()
		{
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(CoachEngine);
			var motorway = engineData.WHTCMotorway;
			Assert.AreEqual(motorway, 1);

			var rural = engineData.WHTCRural;
			Assert.AreEqual(rural, 1);

			var urban = engineData.WHTCUrban;
			Assert.AreEqual(urban, 1);

			var displace = engineData.Displacement;
			Assert.AreEqual(0.01273, displace.Value());
			Assert.IsTrue(displace.HasEqualUnit(new SI().Cubic.Meter));

			var inert = engineData.Inertia;
			Assert.AreEqual(3.8, inert.Value(), 0.00001);
			Assert.IsTrue(inert.HasEqualUnit(new SI().Kilo.Gramm.Square.Meter));

			var idle = engineData.IdleSpeed;
			Assert.AreEqual(58.6430628670095, idle.Value(), 0.000001);
			Assert.IsTrue(idle.HasEqualUnit(0.SI<PerSecond>()));
		}

		private static void VehicleContainer(string engineFile, out VehicleContainer container, out CombustionEngine engine,
			out ITnOutPort requestPort)
		{
			container = new VehicleContainer(ExecutionMode.Engineering);
			var gearbox = new MockGearbox(container);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(engineFile);

			engine = new CombustionEngine(container, engineData);
			var clutch = new Clutch(container, engineData, engine.IdleController);

			var d = new MockDriver(container);

			var aux = new EngineAuxiliary(container);
			aux.AddConstant("", 5000.SI<Watt>());

			gearbox.Gear = 1;

			//gearbox.InPort().Connect(engine.OutPort());
			gearbox.InPort().Connect(clutch.OutPort());
			clutch.InPort().Connect(engine.OutPort());
			engine.Connect(aux.Port());

			// has to be done after connecting components!
			engine.IdleController.RequestPort = clutch.IdleControlPort;

			requestPort = gearbox.OutPort();

			//vehicleContainer.DataWriter = new ModalDataWriter("engine_idle_test.csv");
			var dataWriter = new MockModalDataContainer();
			container.ModData = dataWriter;
		}
	}
}