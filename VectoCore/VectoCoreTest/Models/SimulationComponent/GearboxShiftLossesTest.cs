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

using System.Globalization;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Tests.Integration;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	public class GearboxShiftLossesTest
	{
		private static AxleGearData CreateAxleGearData(GearboxType gbxType)
		{
			var ratio = gbxType == GearboxType.ATSerial ? 6.2 : 5.8;
			return new AxleGearData {
				AxleGear = new GearData {
					Ratio = ratio,
					LossMap = TransmissionLossMapReader.Create(0.95, ratio, "Axlegear"),
				}
			};
		}

		[Test,
		TestCase(200, 2u, 562, 620, 19.5023),
		TestCase(400, 2u, 562, 620, 39.0046),
		TestCase(600, 2u, 562, 620, 58.5069),
		TestCase(800, 2u, 562, 620, 78.0092),
		TestCase(200, 2u, 562, 600, 19.7908),
		TestCase(400, 2u, 562, 600, 39.5816),
		TestCase(600, 2u, 562, 600, 59.3723),
		TestCase(800, 2u, 562, 600, 79.1632),
		TestCase(400, 3u, 500, 490, 30.7900),
		TestCase(400, 3u, 550, 490, 32.4643),
		TestCase(600, 3u, 550, 490, 48.6965),
		]
		public void TestShiftLossComputation(double torqueDemand, uint gear, double preShiftRpm,
			double postShiftRpm, double expectedShiftLoss)
		{
			var engineInertia = 5.SI<KilogramSquareMeter>();

			var gearboxData = ATPowerTrain.CreateGearboxData(GearboxType.ATSerial);

			var container = new VehicleContainer(ExecutionMode.Engineering);
			gearboxData.PowershiftShiftTime = 0.8.SI<Second>();

			var cycleDataStr = "0, 0, 0, 2\n100, 20, 0, 0\n1000, 50, 0, 0";
			var cycleData = SimpleDrivingCycles.CreateCycleData(cycleDataStr);
			var cycle = new MockDrivingCycle(container, cycleData);

			var axleGear = new AxleGear(container, CreateAxleGearData(GearboxType.ATSerial));
			axleGear.Connect(new MockComponent());

			var wheels = new Wheels(container, 0.5.SI<Meter>(), 9.5.SI<KilogramSquareMeter>());

			var vehicle = new MockVehicle(container);
			container.AbsTime = 100.SI<Second>();
			var driver = new MockDriver(container);
			vehicle.MyVehicleSpeed = 10.KMPHtoMeterPerSecond();
			driver.DriverBehavior = DrivingBehavior.Driving;
			var engine = new CombustionEngine(container,
				MockSimulationDataFactory.CreateEngineDataFromFile(ATPowerTrain.EngineFile, gearboxData.Gears.Count));
			container.Engine = engine;
			var runData = new VectoRunData() {
				GearboxData = gearboxData,
				EngineData = new CombustionEngineData() { Inertia = 5.SI<KilogramSquareMeter>() }
			};
			var gbx = new ATGearbox(container, new ATShiftStrategy(gearboxData, container), runData);
			gbx.Connect(engine);
			gbx.IdleController = new MockIdleController();

			var init = gbx.Initialize(0.SI<NewtonMeter>(), preShiftRpm.RPMtoRad());
			axleGear.Initialize(0.SI<NewtonMeter>(), preShiftRpm.RPMtoRad());

			gbx.Gear = gear;

			var absTime = 20.SI<Second>();
			var dt = 0.5.SI<Second>();
			var response = gbx.Request(absTime, dt, 0.SI<NewtonMeter>(), preShiftRpm.RPMtoRad());
			axleGear.Request(absTime, dt, 0.SI<NewtonMeter>(), preShiftRpm.RPMtoRad());


			Assert.IsInstanceOf<ResponseSuccess>(response);
			container.CommitSimulationStep(absTime, dt);
			absTime += dt;

			response = gbx.Request(absTime, dt, torqueDemand.SI<NewtonMeter>(), postShiftRpm.RPMtoRad());
			Assert.IsInstanceOf<ResponseFailTimeInterval>(response);

			dt = ((ResponseFailTimeInterval)response).DeltaT;
			response = gbx.Request(absTime, dt, torqueDemand.SI<NewtonMeter>(), postShiftRpm.RPMtoRad());

			Assert.IsInstanceOf<ResponseSuccess>(response);
			Assert.AreEqual(expectedShiftLoss, gbx.CurrentState.PowershiftLoss.Value(), 1e-3);
			Assert.AreEqual(gear + (postShiftRpm > preShiftRpm ? 1 : -1), gbx.Gear);
		}
	}
}