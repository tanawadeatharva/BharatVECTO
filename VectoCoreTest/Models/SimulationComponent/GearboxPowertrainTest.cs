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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Tests.Integration;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestClass]
	public class GearboxPowertrainTest
	{
		[TestMethod]
		public void Gearbox_Initialize_Empty()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(new[] {
				// <s>, <v>, <grad>,           <stop>
				"  0,    0,  2.95016969027809, 1",
				"1000,  60,  2.95016969027809, 0",
			});
			var container = Truck40tPowerTrain.CreatePowerTrain(cycle, "Gearbox_Initialize.vmod", 7500.0.SI<Kilogram>(),
				0.SI<Kilogram>());
			var retVal = container.Cycle.Initialize();
			Assert.AreEqual(4u, container.Gear);
			Assert.IsInstanceOfType(retVal, typeof(ResponseSuccess));

			AssertHelper.AreRelativeEqual(560.RPMtoRad(), container.EngineSpeed);

			var absTime = 0.SI<Second>();
			var ds = 1.SI<Meter>();

			retVal = container.Cycle.Request(absTime, ds);
			container.CommitSimulationStep(absTime, retVal.SimulationInterval);
			absTime += retVal.SimulationInterval;

			AssertHelper.AreRelativeEqual(560.RPMtoRad(), container.EngineSpeed);

			container.Cycle.Request(absTime, ds);
			container.CommitSimulationStep(absTime, retVal.SimulationInterval);

			AssertHelper.AreRelativeEqual(593.202.RPMtoRad(), container.EngineSpeed);
		}

		[TestMethod]
		public void Gearbox_Initialize_RefLoad()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(new[] {
				// <s>, <v>,           <grad>, <stop>
				"    0,   0, 2.95016969027809,      1",
				" 1000,  60, 2.95016969027809,      0",
			});
			var container = Truck40tPowerTrain.CreatePowerTrain(cycle, "Gearbox_Initialize.vmod", 7500.0.SI<Kilogram>(),
				19300.SI<Kilogram>());
			var retVal = container.Cycle.Initialize();
			Assert.AreEqual(4u, container.Gear);
			Assert.IsInstanceOfType(retVal, typeof(ResponseSuccess));

			AssertHelper.AreRelativeEqual(560.RPMtoRad(), container.EngineSpeed);

			var absTime = 0.SI<Second>();
			var ds = 1.SI<Meter>();

			retVal = container.Cycle.Request(absTime, ds);
			container.CommitSimulationStep(absTime, retVal.SimulationInterval);
			absTime += retVal.SimulationInterval;

			AssertHelper.AreRelativeEqual(560.RPMtoRad(), container.EngineSpeed);

			container.Cycle.Request(absTime, ds);
			container.CommitSimulationStep(absTime, retVal.SimulationInterval);

			AssertHelper.AreRelativeEqual(593.202.RPMtoRad(), container.EngineSpeed);
		}

		[TestMethod]
		public void Gearbox_Initialize_85_RefLoad()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(new[] {
				// <s>,<v>,<grad>,<stop>
				"  0,  85, 2.95016969027809,     0",
				" 100, 85, 2.95016969027809,     0",
			});
			var container = Truck40tPowerTrain.CreatePowerTrain(cycle, "Gearbox_Initialize.vmod", 7500.0.SI<Kilogram>(),
				19300.SI<Kilogram>());
			var retVal = container.Cycle.Initialize();
			Assert.AreEqual(11u, container.Gear);
			Assert.IsInstanceOfType(retVal, typeof(ResponseSuccess));

			AssertHelper.AreRelativeEqual(1530.263.RPMtoRad(), container.EngineSpeed, 0.001);

			var absTime = 0.SI<Second>();
			var ds = 1.SI<Meter>();

			retVal = container.Cycle.Request(absTime, ds);
			container.CommitSimulationStep(absTime, retVal.SimulationInterval);
			absTime += retVal.SimulationInterval;
		}
	}
}