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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestClass]
	public class ClutchTest
	{
		private const string CoachEngine = @"TestData\Components\24t Coach.veng";

		[TestMethod]
		public void TestClutch()
		{
			var container = new VehicleContainer(ExecutionMode.Engineering);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(CoachEngine);
			var gearbox = new MockGearbox(container);

			var clutch = new Clutch(container, engineData, null);

			var inPort = clutch.InPort();
			var outPort = new MockTnOutPort();

			inPort.Connect(outPort);

			var clutchOutPort = clutch.OutPort();

			var driver = new MockDriver(container);

			//Test - Clutch slipping
			clutchOutPort.Request(0.SI<Second>(), 0.SI<Second>(), 100.SI<NewtonMeter>(), 0.SI<PerSecond>());

			Assert.AreEqual(0, outPort.Torque.Value(), 0.001);
			Assert.AreEqual(62.119969, outPort.AngularVelocity.Value(), 0.001);

			clutchOutPort.Request(0.SI<Second>(), 0.SI<Second>(), 100.SI<NewtonMeter>(), 30.SI<PerSecond>());

			Assert.AreEqual(48.293649, outPort.Torque.Value(), 0.001);
			Assert.AreEqual(62.119969, outPort.AngularVelocity.Value(), 0.001);

			//Test - Clutch opened
			driver.VehicleStopped = true;
			clutchOutPort.Request(0.SI<Second>(), 0.SI<Second>(), 100.SI<NewtonMeter>(), 30.SI<PerSecond>());

			Assert.AreEqual(0, outPort.Torque.Value(), 0.001);
			Assert.AreEqual(engineData.IdleSpeed.Value(), outPort.AngularVelocity.Value(), 0.001);

			//Test - Clutch closed
			driver.VehicleStopped = false;
			clutchOutPort.Request(0.SI<Second>(), 0.SI<Second>(), 100.SI<NewtonMeter>(), 80.SI<PerSecond>());

			Assert.AreEqual(100.0, outPort.Torque.Value(), 0.001);
			Assert.AreEqual(80.0, outPort.AngularVelocity.Value(), 0.001);
		}
	}
}