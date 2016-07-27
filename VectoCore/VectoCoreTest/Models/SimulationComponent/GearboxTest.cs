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

using System;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

// ReSharper disable RedundantAssignment
// ReSharper disable UnusedVariable
// ReSharper disable InconsistentNaming

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestFixture]
	public class GearboxTest
	{
		public const string GearboxDataFile = @"TestData\Components\24t Coach.vgbx";
		public const string EngineDataFile = @"TestData\Components\24t Coach.veng";

		public const string CycleFile = @"TestData\Integration\FullPowerTrain\1-Gear-Test-dist.vdri";
		public const string CoachCycleFile = @"TestData\Integration\FullPowerTrain\Coach.vdri";
		public const string EngineFile = @"TestData\Components\24t Coach.veng";

		public const string AccelerationFile = @"TestData\Components\Coach.vacc";

		public const string IndirectLossMap = @"TestData\Components\Indirect Gear.vtlm";
		public const string DirectLossMap = @"TestData\Components\Direct Gear.vtlm";
		public const string GearboxShiftPolygonFile = @"TestData\Components\ShiftPolygons.vgbs";
		//public const string GearboxFullLoadCurveFile = @"TestData\Components\Gearbox.vfld";

		public const string AxleGearValidRangeDataFile = @"TestData\Components\AxleGearValidRange.vgbx";
		public const string AxleGearInvalidRangeDataFile = @"TestData\Components\AxleGearInvalidRange.vgbx";

		public const string AngularGearLossMap = @"TestData\Components\AngleGear.vtlm";

		private static GearboxData CreateGearboxData()
		{
			var ratios = new[] { 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };

			return new GearboxData {
				Gears = ratios.Select((ratio, i) =>
					Tuple.Create((uint)i,
						new GearData {
							MaxTorque = 2300.SI<NewtonMeter>(),
							LossMap = TransmissionLossMapReader.ReadFromFile(i != 6 ? IndirectLossMap : DirectLossMap, ratio,
								string.Format("Gear {0}", i)),
							Ratio = ratio,
							ShiftPolygon = ShiftPolygonReader.ReadFromFile(GearboxShiftPolygonFile)
						}))
					.ToDictionary(k => k.Item1 + 1, v => v.Item2),
				ShiftTime = 2.SI<Second>(),
				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 1.SI<Second>(),
				DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
				UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
				UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration
			};
		}

		protected PerSecond SpeedToAngularSpeed(double v, double r)
		{
			return ((60 * v) / (2 * r * Math.PI / 1000)).RPMtoRad();
		}

		[TestCase(520, 20.320, 279698.4, 9401.44062)]
		public void AxleGearTest(double rdyn, double speed, double power, double expectedLoss)
		{
			var vehicle = new VehicleContainer(ExecutionMode.Engineering);
			var axleGearData = MockSimulationDataFactory.CreateAxleGearDataFromFile(GearboxDataFile);
			var axleGear = new AxleGear(vehicle, axleGearData);

			var mockPort = new MockTnOutPort();
			axleGear.InPort().Connect(mockPort);

			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();

			var angSpeed = SpeedToAngularSpeed(speed, rdyn);
			var PvD = power.SI<Watt>();
			var torqueToWheels = Formulas.PowerToTorque(PvD, angSpeed);

			axleGear.Request(absTime, dt, torqueToWheels, angSpeed);

			var loss = expectedLoss.SI<Watt>();

			Assert.AreEqual(Formulas.PowerToTorque(PvD + loss, angSpeed * axleGearData.AxleGear.Ratio).Value(),
				mockPort.Torque.Value(), 0.01, "Torque Engine Side");
			Assert.AreEqual((angSpeed * axleGearData.AxleGear.Ratio).Value(), mockPort.AngularVelocity.Value(), 0.01,
				"AngularVelocity Engine Side");
		}

		[TestCase]
		public void AxleGearValidRangeTest()
		{
			var vehicle = new VehicleContainer(ExecutionMode.Engineering);
			var axleGearData = MockSimulationDataFactory.CreateAxleGearDataFromFile(AxleGearValidRangeDataFile);
			var axleGear = new AxleGear(vehicle, axleGearData);
			Assert.AreEqual(0, axleGear.Validate().Count);
		}

		[TestCase]
		public void AxleGearInvalidRangeTest()
		{
			var vehicle = new VehicleContainer(ExecutionMode.Engineering);
			var axleGearData = MockSimulationDataFactory.CreateAxleGearDataFromFile(AxleGearInvalidRangeDataFile);
			var axleGear = new AxleGear(vehicle, axleGearData);
			var errors = axleGear.Validate();
			Assert.AreEqual(1, errors.Count);
		}

		[TestCase(520, 20.320, 279698.4, 9401.44062, 3.240355)]
		public void AngularGear_Losses(double rdyn, double speed, double power, double expectedLoss, double ratio)
		{
			// convert to SI
			var angSpeed = SpeedToAngularSpeed(speed, rdyn);
			var PvD = power.SI<Watt>();
			var torqueToWheels = PvD / angSpeed;
			var loss = expectedLoss.SI<Watt>();

			// setup components
			var vehicle = new VehicleContainer(ExecutionMode.Engineering);
			var angularGearData = new AngularGearData {
				AngularGear = new TransmissionData {
					LossMap = TransmissionLossMapReader.Create(VectoCSVFile.Read(AngularGearLossMap), ratio, "AngularGear"),
					Ratio = ratio
				}
			};
			var mockPort = new MockTnOutPort();
			var angularGear = new AngularGear(vehicle, angularGearData);
			angularGear.InPort().Connect(mockPort);

			// issue request
			angularGear.Request(0.SI<Second>(), 1.SI<Second>(), torqueToWheels, angSpeed);

			// test
			AssertHelper.AreRelativeEqual(angSpeed * angularGearData.AngularGear.Ratio, mockPort.AngularVelocity,
				"AngularVelocity Engine Side");

			AssertHelper.AreRelativeEqual((PvD + loss) / (angSpeed * angularGearData.AngularGear.Ratio), mockPort.Torque,
				"Torque Engine Side");
		}

		[TestCase(@"TestData\Components\24t Coach LessThanTwoGears.vgbx")]
		public void Gearbox_LessThanTwoGearsException(string wrongFile)
		{
			AssertHelper.Exception<VectoSimulationException>(
				() => MockSimulationDataFactory.CreateGearboxDataFromFile(wrongFile, EngineDataFile),
				"At least one Gear-Entry must be defined in Gearbox!");
		}

		[TestCase(GearboxDataFile, EngineDataFile, 6.38, 2300, 1600, 2356.2326),
		TestCase(GearboxDataFile, EngineDataFile, 6.38, -1300, 1000, -1267.0686)]
		public void Gearbox_LossMapInterpolation(string gbxFile, string engineFile, double ratio, double torque,
			double inAngularSpeed, double expectedTorque)
		{
			var gearboxData = MockSimulationDataFactory.CreateGearboxDataFromFile(gbxFile, engineFile);
			var container = new VehicleContainer(ExecutionMode.Engineering);
			var gearbox = new Gearbox(container, gearboxData, new AMTShiftStrategy(gearboxData, container));
			var driver = new MockDriver(container);
			var vehicle = new MockVehicle(container) { MyVehicleSpeed = 0.KMPHtoMeterPerSecond() };

			var port = new MockTnOutPort();
			gearbox.InPort().Connect(port);
			container.Engine = port;

			gearbox.Initialize(0.SI<NewtonMeter>(), 0.RPMtoRad());

			var absTime = 0.SI<Second>();
			var dt = 2.SI<Second>();
			var tq = torque.SI<NewtonMeter>();
			var n = inAngularSpeed.RPMtoRad();
			var response = (ResponseSuccess)gearbox.OutPort().Request(absTime, dt, tq * ratio, n / ratio);

			AssertHelper.AreRelativeEqual(absTime, port.AbsTime);
			AssertHelper.AreRelativeEqual(dt, port.Dt);
			AssertHelper.AreRelativeEqual(inAngularSpeed, port.AngularVelocity.Value() / Constants.RPMToRad);
			AssertHelper.AreRelativeEqual(expectedTorque, port.Torque.Value());

			Assert.IsFalse(gearbox.CurrentState.TorqueLossResult.Extrapolated);
			var modData = new MockModalDataContainer();
			gearbox.CommitSimulationStep(modData);
		}

		[TestCase(GearboxDataFile, EngineDataFile, 6.38, 2600, 1600, 2658.10267),
		TestCase(GearboxDataFile, EngineDataFile, 6.38, -2600, 1000, -2543.4076)]
		public void Gearbox_LossMapExtrapolation_Declaration(string gbxFile, string engineFile, double ratio, double torque,
			double inAngularSpeed, double expectedTorque)
		{
			var gearboxData = MockSimulationDataFactory.CreateGearboxDataFromFile(gbxFile, engineFile);
			var container = new VehicleContainer(ExecutionMode.Declaration);
			var gearbox = new Gearbox(container, gearboxData, new AMTShiftStrategy(gearboxData, container));
			var driver = new MockDriver(container);
			var vehicle = new MockVehicle(container) { MyVehicleSpeed = 0.KMPHtoMeterPerSecond() };

			var port = new MockTnOutPort();
			gearbox.InPort().Connect(port);
			container.Engine = port;

			gearbox.Initialize(0.SI<NewtonMeter>(), 0.RPMtoRad());

			var absTime = 0.SI<Second>();
			var dt = 2.SI<Second>();
			var tq = torque.SI<NewtonMeter>();
			var n = inAngularSpeed.RPMtoRad();
			var response = (ResponseSuccess)gearbox.OutPort().Request(absTime, dt, tq * ratio, n / ratio);

			Assert.AreEqual(absTime, port.AbsTime);
			Assert.AreEqual(dt, port.Dt);
			Assert.AreEqual(inAngularSpeed, port.AngularVelocity.Value() / Constants.RPMToRad, 1e-3);
			AssertHelper.AreRelativeEqual(expectedTorque.SI<NewtonMeter>(), port.Torque);

			var modData = new MockModalDataContainer();
			Assert.IsTrue(gearbox.CurrentState.TorqueLossResult.Extrapolated);
			AssertHelper.Exception<VectoException>(() => { gearbox.CommitSimulationStep(modData); });
		}

		[TestCase(GearboxDataFile, EngineDataFile, 6.38, 2600, 1600, 2658.10267),
		TestCase(GearboxDataFile, EngineDataFile, 6.38, -2600, 1000, -2543.4076)]
		public void Gearbox_LossMapExtrapolation_Engineering(string gbxFile, string engineFile, double ratio, double torque,
			double inAngularSpeed, double expectedTorque)
		{
			var gearboxData = MockSimulationDataFactory.CreateGearboxDataFromFile(GearboxDataFile, EngineDataFile);
			var container = new VehicleContainer(executionMode: ExecutionMode.Engineering);
			var gearbox = new Gearbox(container, gearboxData, new AMTShiftStrategy(gearboxData, container));
			var driver = new MockDriver(container);
			var vehicle = new MockVehicle(container) { MyVehicleSpeed = 0.KMPHtoMeterPerSecond() };

			var port = new MockTnOutPort();
			gearbox.InPort().Connect(port);
			container.Engine = port;

			gearbox.Initialize(0.SI<NewtonMeter>(), 0.RPMtoRad());

			var absTime = 0.SI<Second>();
			var dt = 2.SI<Second>();
			var t = torque.SI<NewtonMeter>();
			var n = inAngularSpeed.RPMtoRad();
			var response = (ResponseSuccess)gearbox.OutPort().Request(absTime, dt, t * ratio, n / ratio);

			Assert.AreEqual(absTime, port.AbsTime);
			Assert.AreEqual(dt, port.Dt);
			Assert.AreEqual(n, port.AngularVelocity);
			AssertHelper.AreRelativeEqual(expectedTorque.SI<NewtonMeter>(), port.Torque);

			var modData = new MockModalDataContainer();
			Assert.IsTrue(gearbox.CurrentState.TorqueLossResult.Extrapolated);
			gearbox.CommitSimulationStep(modData);
		}

		[TestCase(GearboxDataFile, EngineDataFile, 6.38, 2600, 1600, true, 2658.10267),
		TestCase(GearboxDataFile, EngineDataFile, 6.38, -2500, 1000, false, -2443.5392),
		TestCase(GearboxDataFile, EngineDataFile, 6.38, -1000, 1000, false, -972.95098)]
		public void Gearbox_LossMapExtrapolation_DryRun(string gbxFile, string engineFile, double ratio, double torque,
			double inAngularSpeed, bool extrapolated, double expectedTorque)
		{
			var gearboxData = MockSimulationDataFactory.CreateGearboxDataFromFile(GearboxDataFile, EngineDataFile);
			var container = new VehicleContainer(ExecutionMode.Engineering);
			var gearbox = new Gearbox(container, gearboxData, new AMTShiftStrategy(gearboxData, container));
			var driver = new MockDriver(container);
			var vehicle = new MockVehicle(container) { MyVehicleSpeed = 0.KMPHtoMeterPerSecond() };

			var port = new MockTnOutPort();
			gearbox.InPort().Connect(port);
			container.Engine = port;

			gearbox.Initialize(0.SI<NewtonMeter>(), 0.RPMtoRad());

			var absTime = 0.SI<Second>();
			var dt = 2.SI<Second>();
			var t = torque.SI<NewtonMeter>();
			var n = inAngularSpeed.RPMtoRad();
			var response = (ResponseSuccess)gearbox.OutPort().Request(absTime, dt, t * ratio, n / ratio);

			Assert.AreEqual(absTime, port.AbsTime);
			Assert.AreEqual(dt, port.Dt);
			Assert.AreEqual(n, port.AngularVelocity);
			AssertHelper.AreRelativeEqual(expectedTorque.SI<NewtonMeter>(), port.Torque);
			Assert.AreEqual(extrapolated, gearbox.CurrentState.TorqueLossResult.Extrapolated);

			var modData = new MockModalDataContainer();
			gearbox.CommitSimulationStep(modData);
		}

		[Test]
		public void Gearbox_IntersectFullLoadCurves()
		{
			var container = new VehicleContainer(ExecutionMode.Engineering);
			var gearboxData = MockSimulationDataFactory.CreateGearboxDataFromFile(GearboxDataFile, EngineDataFile);
			var gearbox = new Gearbox(container, gearboxData, new AMTShiftStrategy(gearboxData, container));
			var vehicle = new MockVehicle(container) { MyVehicleSpeed = 0.KMPHtoMeterPerSecond() };
			var driver = new MockDriver(container);

			var port = new MockTnOutPort();
			gearbox.InPort().Connect(port);
			container.Engine = port;

			gearbox.Initialize(0.SI<NewtonMeter>(), 0.RPMtoRad());

			const double ratio = 6.38;

			var expected = new[] {
				new { t = 2500, n = 900 },
				new { t = 2500, n = 1700 },
				new { t = 2129, n = 1600 }, // to high for gearbox, but ok for engine
			};

			foreach (var exp in expected) {
				var torque = exp.t.SI<NewtonMeter>() * ratio;
				var angularVelocity = exp.n.RPMtoRad() / ratio;
				var response = (ResponseSuccess)gearbox.OutPort().Request(0.SI<Second>(), 1.SI<Second>(), torque, angularVelocity);
			}

			var expectedCorrect = new[] {
				new { t = 500, n = 700 },
				new { t = 1500, n = 1100 },
				new { t = 2000, n = 1500 },
				new { t = 2240, n = 1200 } // ok for gearbox but would be to high for engine
			};

			foreach (var exp in expectedCorrect) {
				var torque = exp.t.SI<NewtonMeter>() * ratio;
				var angularVelocity = exp.n.RPMtoRad() / ratio;

				var response = (ResponseSuccess)gearbox.OutPort().Request(0.SI<Second>(), 1.SI<Second>(), torque, angularVelocity);
			}
		}

		[TestCase(1, -1000, 600, 28.096, typeof(ResponseSuccess)),
		TestCase(2, -1000, 600, 28.096, typeof(ResponseSuccess)),
		TestCase(7, -1000, 600, 13.096, typeof(ResponseSuccess)),
		TestCase(7, 850, 600, 12.346, typeof(ResponseSuccess)),
		TestCase(7, 2050, 1200, 21.382, typeof(ResponseSuccess)),
		TestCase(1, 50, 600, 9.096, typeof(ResponseSuccess)),
		TestCase(1, 2450, 800, 58.11, typeof(ResponseSuccess)),
		TestCase(1, 850, 800, 26.11, typeof(ResponseSuccess)),
		TestCase(1, 850, 200, 23.07, typeof(ResponseSuccess)),
		TestCase(2, 50, 600, 9.096, typeof(ResponseSuccess)),
		TestCase(2, 2050, 1200, 52.132, typeof(ResponseSuccess)),
		TestCase(2, 850, 800, 26.11, typeof(ResponseSuccess)),
		TestCase(2, 850, 600, 25.096, typeof(ResponseSuccess)),
		TestCase(2, 850, 0, 22.06, typeof(ResponseSuccess)),
		TestCase(1, 850, 0, 22.06, typeof(ResponseSuccess)),
		TestCase(7, -1000, 0, 10.06, typeof(ResponseSuccess)),
		TestCase(7, 850, 0, 9.31, typeof(ResponseSuccess)),
		TestCase(7, 2450, 0, 17.31, typeof(ResponseSuccess))
		]
		public void Gearbox_Request_engaged(int gear, double t, double n, double loss, Type responseType)
		{
			var container = new VehicleContainer(ExecutionMode.Engineering);
			var gearboxData = CreateGearboxData();
			var gearbox = new Gearbox(container, gearboxData, new AMTShiftStrategy(gearboxData, container));

			var driver = new MockDriver(container);
			var vehicle = new MockVehicle(container) { MyVehicleSpeed = 0.KMPHtoMeterPerSecond() };

			var port = new MockTnOutPort();
			gearbox.InPort().Connect(port);
			container.Engine = port;

			var ratios = new[] { 0.0, 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };
			// the first element 0.0 is just a placeholder for axlegear, not used in this test

			var absTime = 0.SI<Second>();
			var dt = 2.SI<Second>();

			var expectedT = t.SI<NewtonMeter>();
			var expectedN = n.RPMtoRad();
			var expectedLoss = loss.SI<NewtonMeter>();

			var torque = (expectedT - expectedLoss) * ratios[gear];
			var angularVelocity = expectedN / ratios[gear];

			gearbox.OutPort().Initialize(torque, angularVelocity);
			gearbox.Gear = (uint)gear;
			var response = gearbox.OutPort().Request(absTime, dt, torque, angularVelocity);
			Assert.IsTrue(response.GetType() == responseType);

			if (responseType == typeof(ResponseSuccess)) {
				AssertHelper.AreRelativeEqual(absTime, port.AbsTime);
				AssertHelper.AreRelativeEqual(dt, port.Dt);
				AssertHelper.AreRelativeEqual(expectedN, port.AngularVelocity);
				AssertHelper.AreRelativeEqual(expectedT, port.Torque, toleranceFactor: 1e-5);
			}
		}

		[TestCase(8, 7, 1500, 750, typeof(ResponseGearShift)),
		TestCase(7, 6, 1500, 750, typeof(ResponseGearShift)),
		TestCase(6, 5, 1500, 750, typeof(ResponseGearShift)),
		TestCase(5, 4, 1500, 750, typeof(ResponseGearShift)),
		TestCase(4, 3, 1500, 750, typeof(ResponseGearShift)),
		TestCase(3, 2, 1500, 750, typeof(ResponseGearShift)),
		TestCase(2, 1, 1500, 750, typeof(ResponseGearShift)),
		TestCase(1, 1, 1200, 700, typeof(ResponseSuccess)),
		TestCase(8, 1, 15000, 50, typeof(ResponseGearShift)),]
		public void Gearbox_ShiftDown(int gear, int newGear, double t, double n, Type responseType)
		{
			var container = new VehicleContainer(ExecutionMode.Engineering);
			var gearboxData = MockSimulationDataFactory.CreateGearboxDataFromFile(GearboxDataFile, EngineDataFile);
			var gearbox = new Gearbox(container, gearboxData, new AMTShiftStrategy(gearboxData, container));

			var driver = new MockDriver(container);
			var port = new MockTnOutPort() { EngineN95hSpeed = 2000.RPMtoRad() };
			gearbox.InPort().Connect(port);
			var vehicle = new MockVehicle(container) { MyVehicleSpeed = 10.SI<MeterPerSecond>() };
			container.Engine = port;

			var ratios = new[] { 0.0, 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };
			// the first element 0.0 is just a placeholder for axlegear, not used in this test

			var absTime = 0.SI<Second>();
			var dt = 2.SI<Second>();

			gearbox.OutPort().Initialize(1.SI<NewtonMeter>(), 1.SI<PerSecond>());

			var expectedT = t.SI<NewtonMeter>();
			var expectedN = n.RPMtoRad();

			var torque = expectedT * ratios[gear];
			var angularVelocity = expectedN / ratios[gear];

			gearbox.Gear = (uint)gear;
			var gearShiftResponse = gearbox.OutPort().Request(absTime, dt, torque, angularVelocity);
			Assert.IsTrue(gearShiftResponse.GetType() == responseType);

			absTime += dt;
			var successResponse = (ResponseSuccess)gearbox.OutPort().Request(absTime, dt, torque, angularVelocity);
			Assert.AreEqual((uint)newGear, container.Gear);
		}

		[TestCase(7, 8, 1000, 1400, typeof(ResponseGearShift)),
		TestCase(6, 7, 1000, 1400, typeof(ResponseGearShift)),
		TestCase(5, 6, 1000, 1400, typeof(ResponseGearShift)),
		TestCase(4, 5, 1000, 1400, typeof(ResponseGearShift)),
		TestCase(3, 4, 1000, 1400, typeof(ResponseGearShift)),
		TestCase(2, 3, 1000, 1400, typeof(ResponseGearShift)),
		TestCase(1, 2, 1000, 1400, typeof(ResponseGearShift)),
		TestCase(8, 8, 1000, 1400, typeof(ResponseSuccess)),
		TestCase(1, 6, 200, 9000, typeof(ResponseGearShift)),]
		public void Gearbox_ShiftUp(int gear, int newGear, double tq, double n, Type responseType)
		{
			var container = new MockVehicleContainer() {
				VehicleSpeed = 10.SI<MeterPerSecond>(),
				DriverBehavior = DrivingBehavior.Driving,
				Altitude = 0.SI<Meter>(),
				VehicleMass = 10000.SI<Kilogram>(),
				ReducedMassWheels = 100.SI<Kilogram>(),
				TotalMass = 19000.SI<Kilogram>()
			};
			var gearboxData = MockSimulationDataFactory.CreateGearboxDataFromFile(GearboxDataFile, EngineDataFile);
			var gearbox = new Gearbox(container, gearboxData, new AMTShiftStrategy(gearboxData, container));
			var port = new MockTnOutPort() { EngineN95hSpeed = 2000.RPMtoRad() };
			container.Engine = port;
			gearbox.InPort().Connect(port);

			var ratios = new[] { 0.0, 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };
			// the first element 0.0 is just a placeholder for axlegear, not used in this test

			var absTime = 0.SI<Second>();
			var dt = 2.SI<Second>();

			gearbox.OutPort().Initialize(1.SI<NewtonMeter>(), 1.SI<PerSecond>());

			absTime += dt;

			var expectedT = tq.SI<NewtonMeter>();
			var expectedN = n.RPMtoRad();

			var torque = expectedT * ratios[gear];
			var angularVelocity = expectedN / ratios[gear];

			gearbox.Gear = (uint)gear;
			var response = gearbox.OutPort().Request(absTime, dt, torque, angularVelocity);
			Assert.IsTrue(response.GetType() == responseType);

			absTime += dt;
			response = (ResponseSuccess)gearbox.OutPort().Request(absTime, dt, torque, angularVelocity);
			Assert.AreEqual((uint)newGear, gearbox.Gear);
		}
	}
}