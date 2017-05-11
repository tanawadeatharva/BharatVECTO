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

using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Org.BouncyCastle.Asn1.Esf;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestFixture]
	public class ClutchTest
	{
		private const string CoachEngine = @"TestData\Components\24t Coach.veng";

		[Test,
		// clutch slipping
		TestCase(DrivingBehavior.Driving, 100, 0, 0, 65.6889),
		// clutch opened - would cause neg. clutch losses (which is not possible), torque is adapted
		TestCase(DrivingBehavior.Halted, 100, 30, 51.1569, 58.643062),
		// clutch closed
		TestCase(DrivingBehavior.Driving, 100, 80, 100, 80),
		TestCase(DrivingBehavior.Braking, 100, 80, 100, 80),
		TestCase(DrivingBehavior.Driving, 100, 30, 100, 30),
			// clutch opened due to braking
			//TestCase(DrivingBehavior.Braking, 0, 55, null, null),
		]
		public void TestClutch(DrivingBehavior drivingBehavior, double torque, double angularSpeed, double expectedTorque,
			double expectedEngineSpeed)
		{
			var container = new VehicleContainer(ExecutionMode.Engineering);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(CoachEngine, 1);
			var gearbox = new MockGearbox(container);
			gearbox.Gear = 0;
			var clutch = new Clutch(container, engineData) { IdleController = new MockIdleController() };
			var vehicle = new MockVehicle(container);
			vehicle.MyVehicleSpeed = 50.KMPHtoMeterPerSecond();

			var inPort = clutch.InPort();
			var outPort = new MockTnOutPort();

			inPort.Connect(outPort);

			var clutchOutPort = clutch.OutPort();

			var driver = new MockDriver(container);

			driver.DriverBehavior = drivingBehavior;

			clutchOutPort.Request(0.SI<Second>(), 0.SI<Second>(), torque.SI<NewtonMeter>(), angularSpeed.SI<PerSecond>());

			Assert.AreEqual(expectedTorque, outPort.Torque.Value(), 0.001);
			Assert.AreEqual(expectedEngineSpeed, outPort.AngularVelocity.Value(), 0.001);
		}

		//[Test] // this test is just to make sure the clutch characteristic has no unsteadiness
		public void ClutchContinuityTest()
		{
			var container = new VehicleContainer(ExecutionMode.Engineering);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(CoachEngine, 1);
			var gearbox = new MockGearbox(container);
			gearbox.Gear = 1;
			var engine = new MockEngine(container);
			var clutch = new Clutch(container, engineData) { IdleController = new MockIdleController() };

			var inPort = clutch.InPort();
			var outPort = new MockTnOutPort();

			inPort.Connect(outPort);

			var clutchOutPort = clutch.OutPort();

			var driver = new MockDriver(container);

			var mass = 15E3.SI<Kilogram>();
			var rDyn = 0.5.SI<Meter>();
			var ds = 1.SI<Meter>();
			var ratio = 15.0;

			var data = new DataTable();
			data.Columns.Add("a", typeof(double));
			data.Columns.Add("n_out", typeof(double));
			data.Columns.Add("tq_out", typeof(double));
			data.Columns.Add("n_in", typeof(double));
			data.Columns.Add("tq_in", typeof(double));
			data.Columns.Add("P_out", typeof(double));
			data.Columns.Add("P_in", typeof(double));

			var step = 0.001;
			engine.EngineSpeed = 595.RPMtoRad();

			var dt = 1.SI<Second>(); // VectoMath.Sqrt<Second>(2 * ds / accel);

			for (var a = -3.0; a < 3; a += step) {
				var accel = a.SI<MeterPerSquareSecond>();

				var tq = mass * accel * rDyn / ratio;
				var angularVelocity = (accel * dt / rDyn * ratio).Cast<PerSecond>();

				clutchOutPort.Request(0.SI<Second>(), 0.SI<Second>(), tq, angularVelocity);

				var row = data.NewRow();
				row["a"] = a;
				row["n_out"] = angularVelocity.Value();
				row["tq_out"] = tq.Value();
				row["n_in"] = outPort.AngularVelocity.Value();
				row["tq_in"] = outPort.Torque.Value();
				row["P_out"] = (angularVelocity * tq).Value();
				row["P_in"] = (outPort.AngularVelocity * outPort.Torque).Value();
				data.Rows.Add(row);
			}

			VectoCSVFile.Write("clutch.csv", data);
		}
	}

	public class MockEngine : VectoSimulationComponent, IEngineInfo
	{
		public MockEngine(IVehicleContainer container) : base(container) {}

		public PerSecond EngineSpeed { get; set; }
		public NewtonMeter EngineTorque { get; set; }

		public Watt EngineStationaryFullPower(PerSecond angularSpeed)
		{
			throw new System.NotImplementedException();
		}

		public Watt EngineDragPower(PerSecond angularSpeed)
		{
			throw new System.NotImplementedException();
		}

		public PerSecond EngineIdleSpeed { get; set; }
		public PerSecond EngineRatedSpeed { get; set; }
		public PerSecond EngineN95hSpeed { get; set; }
		public PerSecond EngineN80hSpeed { get; set; }

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			container[ModalResultField.P_eng_fcmap] = 0.SI<Watt>();
			container[ModalResultField.P_eng_out] = 0.SI<Watt>();
			container[ModalResultField.P_eng_inertia] = 0.SI<Watt>();

			container[ModalResultField.n_eng_avg] = 0.SI<PerSecond>();
			container[ModalResultField.T_eng_fcmap] = 0.SI<NewtonMeter>();

			container[ModalResultField.P_eng_full] = 0.SI<Watt>();
			container[ModalResultField.P_eng_drag] = 0.SI<Watt>();
			container[ModalResultField.Tq_full] = 0.SI<NewtonMeter>();
			container[ModalResultField.Tq_drag] = 0.SI<NewtonMeter>();

			container[ModalResultField.FCMap] = 0.SI<KilogramPerSecond>();
			container[ModalResultField.FCAUXc] = 0.SI<KilogramPerSecond>();
			container[ModalResultField.FCWHTCc] = 0.SI<KilogramPerSecond>();
			container[ModalResultField.FCAAUX] = 0.SI<KilogramPerSecond>();
			container[ModalResultField.FCFinal] = 0.SI<KilogramPerSecond>();
		}

		protected override void DoCommitSimulationStep() {}
	}
}