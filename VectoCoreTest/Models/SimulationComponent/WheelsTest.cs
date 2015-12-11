/*
* Copyright 2015 Graz University of Technology
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl5
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.FileIO.Reader.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestClass]
	public class WheelsTest
	{
		private const string VehicleDataFile = @"TestData\Components\24t Coach.vveh";

		[TestMethod]
		public void WheelsRequestTest()
		{
			var container = new VehicleContainer();
			//var reader = new EngineeringModeSimulationDataReader();
			var vehicleData = EngineeringModeSimulationDataReader.CreateVehicleDataFromFile(VehicleDataFile);

			IWheels wheels = new Wheels(container, vehicleData.DynamicTyreRadius);
			var mockPort = new MockTnOutPort();

			wheels.InPort().Connect(mockPort);

			var requestPort = wheels.OutPort();

			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();

			var force = 5000.SI<Newton>();
			var velocity = 20.SI<MeterPerSecond>();

			var retVal = requestPort.Request(absTime, dt, force, velocity);

			Assert.AreEqual(2600.0, mockPort.Torque.Value(), 0.0001);
			Assert.AreEqual(38.4615384615, mockPort.AngularVelocity.Value(), 0.0001);
		}
	}
}