/*
* Copyright 2015 European Union
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
using TUGraz.VectoCore.FileIO.Reader;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestClass]
	public class DrivingCycleTests
	{
		[TestMethod]
		public void TestEngineOnly()
		{
			var dataWriter = new MockModalDataWriter();
			var container = new VehicleContainer(dataWriter);

			var cycleData = DrivingCycleDataReader.ReadFromFileEngineOnly(@"TestData\Cycles\Coach Engine Only.vdri");
			var cycle = new EngineOnlyDrivingCycle(container, cycleData);

			var outPort = new MockTnOutPort();
			var inPort = cycle.InPort();
			var cycleOut = cycle.OutPort();

			inPort.Connect(outPort);

			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();

			var response = cycleOut.Request(absTime, dt);
			Assert.IsInstanceOfType(response, typeof(ResponseSuccess));

			var time = absTime + dt / 2;
			var simulationInterval = dt;
			container.CommitSimulationStep(time, simulationInterval);

			Assert.AreEqual(absTime, outPort.AbsTime);
			Assert.AreEqual(dt, outPort.Dt);
			Assert.AreEqual(600.RPMtoRad(), outPort.AngularVelocity);
			Assert.AreEqual(0.SI<NewtonMeter>(), outPort.Torque);
		}

		[TestMethod, Ignore]
		public void TestEngineOnlyWithTimestamps()
		{
			var container = new VehicleContainer();

			var cycleData = DrivingCycleDataReader.ReadFromFileEngineOnly(@"TestData\Cycles\Coach Engine Only Paux_var-dt.vdri");
			var cycle = new EngineOnlyDrivingCycle(container, cycleData);

			var outPort = new MockTnOutPort();
			var inPort = cycle.InPort();

			inPort.Connect(outPort);

			var absTime = 10.SI<Second>();
			var dt = 1.SI<Second>();

			var response = cycle.OutPort().Request(absTime, dt);
			Assert.IsInstanceOfType(response, typeof(ResponseFailTimeInterval));

			dt = 0.25.SI<Second>();
			response = cycle.OutPort().Request(absTime, dt);
			Assert.IsInstanceOfType(response, typeof(ResponseSuccess));

			var dataWriter = new MockModalDataWriter();
			container.CommitSimulationStep(absTime, dt);

			Assert.AreEqual(absTime, outPort.AbsTime);
			Assert.AreEqual(dt, outPort.Dt);
			Assert.AreEqual(743.2361.RPMtoRad(), outPort.AngularVelocity);
			Assert.AreEqual(2779.576.SI<Watt>() / 743.2361.RPMtoRad(), outPort.Torque);

			// ========================

			dt = 1.SI<Second>();
			absTime = 500.SI<Second>();
			response = cycle.OutPort().Request(absTime, dt);
			Assert.IsInstanceOfType(response, typeof(ResponseFailTimeInterval));

			dt = 0.25.SI<Second>();

			for (int i = 0; i < 2; i++) {
				response = cycle.OutPort().Request(absTime, dt);
				Assert.IsInstanceOfType(response, typeof(ResponseSuccess));

				dataWriter = new MockModalDataWriter();
				container.CommitSimulationStep(absTime, dt);

				Assert.AreEqual(absTime, outPort.AbsTime);
				Assert.AreEqual(dt, outPort.Dt);
				Assert.AreEqual(1584.731.RPMtoRad(), outPort.AngularVelocity);
				Assert.AreEqual(3380.548.SI<Watt>() / 1584.731.RPMtoRad(), outPort.Torque);

				absTime += dt;
			}


			// todo: test going backward in time, end of cycle
		}

		[TestMethod]
		public void Test_TimeBased_FirstCycle()
		{
			var container = new VehicleContainer();

			var cycleData = DrivingCycleDataReader.ReadFromFileTimeBased(@"TestData\Cycles\Coach First Cycle only.vdri");
			var cycle = new TimeBasedDrivingCycle(container, cycleData);

			var outPort = new MockDrivingCycleOutPort();

			var inPort = cycle.InPort();
			var cycleOut = cycle.OutPort();

			inPort.Connect(outPort);

			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();

			var response = cycleOut.Request(absTime, dt);
			Assert.IsInstanceOfType(response, typeof(ResponseSuccess));

			Assert.AreEqual(absTime, outPort.AbsTime);
			Assert.AreEqual(0.SI<MeterPerSecond>(), outPort.Velocity);
			AssertHelper.AreRelativeEqual(-0.000202379727237.SI<Radian>(), outPort.Gradient);
		}

		[TestMethod]
		public void Test_TimeBased_TimeFieldMissing()
		{
			var container = new VehicleContainer(new MockModalDataWriter());

			var cycleData = DrivingCycleDataReader.ReadFromFileTimeBased(@"TestData\Cycles\Cycle time field missing.vdri");
			var cycle = new TimeBasedDrivingCycle(container, cycleData);

			var outPort = new MockDrivingCycleOutPort();

			var inPort = cycle.InPort();
			var cycleOut = cycle.OutPort();

			inPort.Connect(outPort);

			var dataWriter = new MockModalDataWriter();
			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();

			while (cycleOut.Request(absTime, dt) is ResponseSuccess) {
				Assert.AreEqual(absTime, outPort.AbsTime);
				Assert.AreEqual(dt, outPort.Dt);

				var time = absTime + dt / 2;
				var simulationInterval = dt;
				container.CommitSimulationStep(time, simulationInterval);

				absTime += dt;
			}
		}
	}
}