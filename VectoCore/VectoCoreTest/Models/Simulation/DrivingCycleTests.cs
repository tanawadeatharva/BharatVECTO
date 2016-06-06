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

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader;
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
			var dataWriter = new MockModalDataContainer();
			var container = new VehicleContainer(ExecutionMode.EngineOnly, dataWriter);

			var cycleData = DrivingCycleDataReader.ReadFromFile(@"TestData\Cycles\Coach Engine Only.vdri", CycleType.EngineOnly,
				false);
			var cycle = new PowertrainDrivingCycle(container, cycleData);

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

		[TestMethod]
		public void TestEngineOnlyWithTimestamps()
		{
			var container = new VehicleContainer(ExecutionMode.EngineOnly);

			var cycleData = DrivingCycleDataReader.ReadFromFile(@"TestData\Cycles\Coach Engine Only Paux_var-dt.vdri",
				CycleType.EngineOnly, false);
			var cycle = new PowertrainDrivingCycle(container, cycleData);

			var outPort = new MockTnOutPort();
			var inPort = cycle.InPort();

			inPort.Connect(outPort);

			var absTime = 0.SI<Second>();
			var dt = 5.SI<Second>();

			var response = cycle.OutPort().Request(absTime, dt);
			var timeFail = (response as ResponseFailTimeInterval);
			Assert.IsInstanceOfType(response, typeof(ResponseFailTimeInterval));
			// ReSharper disable once PossibleNullReferenceException
			Assert.AreEqual(0.25.SI<Second>(), timeFail.DeltaT);

			dt = timeFail.DeltaT;

			response = cycle.OutPort().Request(absTime, dt);
			Assert.IsInstanceOfType(response, typeof(ResponseSuccess));

			container.CommitSimulationStep(absTime, dt);

			Assert.AreEqual(absTime, outPort.AbsTime);
			Assert.AreEqual(dt, outPort.Dt);
			Assert.AreEqual(600.RPMtoRad(), outPort.AngularVelocity);
			Assert.AreEqual(0.SI<NewtonMeter>(), outPort.Torque);

			// ========================
			absTime += dt;
			dt = 1.SI<Second>();

			response = cycle.OutPort().Request(absTime, dt);
			Assert.IsInstanceOfType(response, typeof(ResponseFailTimeInterval));

			dt = ((ResponseFailTimeInterval)response).DeltaT;
			Assert.AreEqual(0.5.SI<Second>(), dt);

			for (var i = 0; i < 100; i++) {
				response = cycle.OutPort().Request(absTime, dt);
				response.Switch()
					.Case<ResponseFailTimeInterval>(r => dt = r.DeltaT)
					.Case<ResponseSuccess>(r => {
						container.CommitSimulationStep(absTime, dt);
						Assert.AreEqual(absTime, outPort.AbsTime);
						Assert.AreEqual(dt, outPort.Dt);

						if (absTime < 5) {
							Assert.AreEqual(600.RPMtoRad(), outPort.AngularVelocity);
							AssertHelper.AreRelativeEqual(0.SI<NewtonMeter>(), outPort.Torque, toleranceFactor: 1e-3);
						} else if (absTime.IsBetween(12.75, 13.25) || absTime.IsBetween(14, 15)) {
							Assert.IsTrue(outPort.AngularVelocity > 600.RPMtoRad());
							Assert.IsTrue(outPort.Torque < 0);
						} else {
							Assert.IsTrue(outPort.AngularVelocity > 600.RPMtoRad());
							Assert.IsTrue(outPort.Torque > 0);
						}

						absTime += dt;
						dt = 1.SI<Second>();
					})
					.Default(r => { throw new UnexpectedResponseException("Got an unexpected response", r); });
			}
		}

		[TestMethod]
		public void DrivingCycle_AutoDetect()
		{
			// declaration mode - distance based
			TestCycleDetect("<s>,<v>,<grad>,<stop>", CycleType.DistanceBased);
			TestCycleDetect("<s>,<<v>,>grad>,<stop>", CycleType.DistanceBased);

			// engineering mode - distance based
			TestCycleDetect("<s>,<v>,<grad>,<stop>,<Padd>,<vair_res>,<vair_beta>,<Aux_ELE>,<Aux_SP>", CycleType.DistanceBased);
			TestCycleDetect("<s>,<v>,<stop>,<Padd>,<vair_res>,<vair_beta>,<Aux_ELE>,<Aux_SP>", CycleType.DistanceBased);
			TestCycleDetect("<s>,<v>,<grad>,<stop>,<Padd>,<Aux_ELE>,<Aux_SP>", CycleType.DistanceBased);
			TestCycleDetect("<s>,<v>,<grad>,<stop>,<Padd>,<vair_res>,<vair_beta>", CycleType.DistanceBased);
			TestCycleDetect("<s>,<v>,<stop>,<Padd>", CycleType.DistanceBased);
			TestCycleDetect("s,v,stop,Padd", CycleType.DistanceBased);
			TestCycleDetect("s,v,stop", CycleType.DistanceBased);

			// engineering mode - time based
			// mk 2016-03-01: plain time based cycle does not exist anymore. replaced by measuredspeed, measuredspeed gear, engineonly and pwheel

			// engine only
			TestCycleDetect("<t>,<n>,<Me>,<Padd>", CycleType.EngineOnly);
			TestCycleDetect("<t>,<n>,<Me>", CycleType.EngineOnly);
			TestCycleDetect("<t>,<n>,<Me>,<Pe>,<Padd>", CycleType.EngineOnly);
			TestCycleDetect("<t>,<n>,<Pe>,<Padd>", CycleType.EngineOnly);
			TestCycleDetect("<t>,<n>,<Pe>", CycleType.EngineOnly);
			TestCycleDetect("<Me>,<n>,<Padd>,<t>", CycleType.EngineOnly);
			TestCycleDetect("t,n,Me,Padd", CycleType.EngineOnly);

			// p_wheel
			TestCycleDetect("<t>,<Pwheel>,<gear>,<n>,<Padd>", CycleType.PWheel);
			TestCycleDetect("<gear>,<t>,<n>,<Padd>,<Pwheel>", CycleType.PWheel);
			TestCycleDetect("<t>,<Pwheel>,<gear>,<n>", CycleType.PWheel);
			TestCycleDetect("t,Pwheel,gear,n,Padd", CycleType.PWheel);
			TestCycleDetect("Pwheel,t,gear,n,Padd", CycleType.PWheel);

			// measured speed
			TestCycleDetect("<t>,<v>,<grad>,<Padd>,<vair_res>,<vair_beta>,<Aux_ELE>,<Aux_SP>", CycleType.MeasuredSpeed);
			TestCycleDetect("<t>,<v>,<grad>,<Padd>,<vair_res>,<vair_beta>", CycleType.MeasuredSpeed);
			TestCycleDetect("<t>,<v>,<grad>,<Padd>", CycleType.MeasuredSpeed);
			TestCycleDetect("<t>,<v>,<grad>,<Padd>,<Aux_ALT>,<Aux_ES>", CycleType.MeasuredSpeed);
			TestCycleDetect("<t>,<v>,<grad>", CycleType.MeasuredSpeed);
			TestCycleDetect("<t>,<Padd>,<grad>,<v>", CycleType.MeasuredSpeed);
			TestCycleDetect("t,v,grad,Padd", CycleType.MeasuredSpeed);
			TestCycleDetect("t,v,grad", CycleType.MeasuredSpeed);

			// measured speed with gear
			TestCycleDetect("<t>,<v>,<grad>,<Padd>,<n>,<gear>,<vair_res>,<vair_beta>,<Aux_HVAC>,<Aux_HP>",
				CycleType.MeasuredSpeedGear);
			TestCycleDetect("<t>,<v>,<grad>,<Padd>,<n>,<gear>,<vair_res>,<vair_beta>", CycleType.MeasuredSpeedGear);
			TestCycleDetect("<t>,<v>,<grad>,<Padd>,<n>,<gear>,<Aux_HVAC>,<Aux_HP>", CycleType.MeasuredSpeedGear);
			TestCycleDetect("<t>,<v>,<grad>,<Padd>,<n>,<gear>", CycleType.MeasuredSpeedGear);
			TestCycleDetect("<t>,<v>,<grad>,<n>,<gear>", CycleType.MeasuredSpeedGear);
			TestCycleDetect("<n>,<Padd>,<gear>,<v>,<grad>,<t>", CycleType.MeasuredSpeedGear);
			TestCycleDetect("t,v,grad,Padd,n,gear", CycleType.MeasuredSpeedGear);

			// wrong cycles
			AssertHelper.Exception<VectoException>(() => TestCycleDetect("v,grad,Padd,n,gear", CycleType.MeasuredSpeedGear));
			AssertHelper.Exception<VectoException>(() => TestCycleDetect("<t>,<grad>", CycleType.MeasuredSpeed));
			AssertHelper.Exception<VectoException>(() => TestCycleDetect("<t>,<Pwheel>,<n>,<Padd>", CycleType.PWheel));
			AssertHelper.Exception<VectoException>(() => TestCycleDetect("<t>,<Pwheel>,<Pwheel>,<n>,<Padd>", CycleType.PWheel));
			AssertHelper.Exception<VectoException>(() => TestCycleDetect("<t>,<n>,<torque>,<>,<Padd>", CycleType.EngineOnly));
			AssertHelper.Exception<VectoException>(() => TestCycleDetect("x,y,z", CycleType.EngineOnly));
			AssertHelper.Exception<VectoException>(() => TestCycleDetect("x", CycleType.EngineOnly));
			AssertHelper.Exception<VectoException>(() => TestCycleDetect("", CycleType.MeasuredSpeed));
			AssertHelper.Exception<VectoException>(
				() =>
					TestCycleDetect("<t>,<v>,<gear>,<Pwheel>,<s>,<grad>,<Padd>,<n>,<gear>,<vair_res>,<vair_beta>,<Aux_HVAC>,<Aux_HP>",
						CycleType.MeasuredSpeedGear));
		}

		[TestMethod]
		public void DrivingCycle_Read()
		{
			// declaration mode - distance based
			TestCycleRead("<s>,<v>,<grad>,<stop>\n1,1,1,0", CycleType.DistanceBased, 2);
			TestCycleRead("<s>,<v>,<grad>,<stop>\n1,0,1,1", CycleType.DistanceBased, 3);
			AssertHelper.Exception<VectoException>(() => TestCycleRead("<s>,<v>,<grad>,<stop>\n1,1,1,1", CycleType.DistanceBased));
			TestCycleRead("<s>,<<v>,>grad>,<stop>\n1,1,1,0", CycleType.DistanceBased, 2);

			// engineering mode - distance based
			TestCycleRead("<s>,<v>,<grad>,<stop>,<Padd>,<vair_res>,<vair_beta>,<Aux_ELE>,<Aux_SP>\n1,1,1,0,1,1,1,1,1",
				CycleType.DistanceBased, 2);
			TestCycleRead("<s>,<v>,<stop>,<Padd>,<vair_res>,<vair_beta>,<Aux_ELE>,<Aux_SP>\n1,1,0,1,1,1,1,1",
				CycleType.DistanceBased, 2);
			TestCycleRead("<s>,<v>,<grad>,<stop>,<Padd>,<Aux_ELE>,<Aux_SP>\n1,1,1,0,1,1,1", CycleType.DistanceBased, 2);
			TestCycleRead("<s>,<v>,<grad>,<stop>,<Padd>,<vair_res>,<vair_beta>\n1,1,1,0,1,1,1", CycleType.DistanceBased, 2);
			TestCycleRead("<s>,<v>,<stop>,<Padd>\n1,1,0,1", CycleType.DistanceBased, 2);
			TestCycleRead("s,v,stop,Padd\n1,1,0,1", CycleType.DistanceBased, 2);
			TestCycleRead("s,v,stop\n1,1,0", CycleType.DistanceBased, 2);

			// engineering mode - time based
			// mk 2016-03-01: plain time based cycle does not exist anymore. replaced by measuredspeed, measuredspeed gear, engineonly and pwheel

			// engine only
			TestCycleRead("<t>,<n>,<Me>,<Padd>\n1,1,1,1", CycleType.EngineOnly);
			TestCycleRead("<t>,<n>,<Me>\n1,1,1", CycleType.EngineOnly);
			TestCycleRead("<t>,<n>,<Me>,<Pe>,<Padd>\n1,1,1,1,1", CycleType.EngineOnly);
			TestCycleRead("<t>,<n>,<Pe>,<Padd>\n1,1,1,1", CycleType.EngineOnly);
			TestCycleRead("<t>,<n>,<Pe>\n1,1,1", CycleType.EngineOnly);
			TestCycleRead("<Me>,<n>,<Padd>,<t>\n1,1,1,1", CycleType.EngineOnly);
			TestCycleRead("t,n,Me,Padd\n1,1,1,1", CycleType.EngineOnly);

			// p_wheel
			TestCycleRead("<t>,<Pwheel>,<gear>,<n>,<Padd>\n1,1,1,1,1", CycleType.PWheel);
			TestCycleRead("<gear>,<t>,<n>,<Padd>,<Pwheel>\n1,1,1,1,1", CycleType.PWheel);
			TestCycleRead("<t>,<Pwheel>,<gear>,<n>\n1,1,1,1", CycleType.PWheel);
			TestCycleRead("t,Pwheel,gear,n,Padd\n1,1,1,1,1", CycleType.PWheel);
			TestCycleRead("Pwheel,t,gear,n,Padd\n1,1,1,1,1", CycleType.PWheel);

			// measured speed
			TestCycleRead("<t>,<v>,<grad>,<Padd>,<vair_res>,<vair_beta>,<Aux_ELE>,<Aux_SP>\n1,1,1,1,1,1,1,1",
				CycleType.MeasuredSpeed);
			TestCycleRead("<t>,<v>,<grad>,<Padd>,<vair_res>,<vair_beta>,<Aux_ALT>,<Aux_ES>\n1,1,1,1,1,1,1,1",
				CycleType.MeasuredSpeed);
			TestCycleRead("<t>,<v>,<grad>,<Padd>,<vair_res>,<vair_beta>\n1,1,1,1,1,1", CycleType.MeasuredSpeed);
			TestCycleRead("<t>,<v>,<grad>,<Padd>\n1,1,1,1", CycleType.MeasuredSpeed);
			TestCycleRead("<t>,<v>,<grad>,<Padd>,<Aux_ALT>,<Aux_ES>\n1,1,1,1,1,1", CycleType.MeasuredSpeed);
			TestCycleRead("<t>,<v>,<grad>\n1,1,1", CycleType.MeasuredSpeed);
			TestCycleRead("<t>,<Padd>,<grad>,<v>\n1,1,1,1", CycleType.MeasuredSpeed);
			TestCycleRead("t,v,grad,Padd\n1,1,1,1", CycleType.MeasuredSpeed);
			TestCycleRead("t,v,grad\n1,1,1", CycleType.MeasuredSpeed);

			// measured speed with gear
			TestCycleRead("<t>,<v>,<grad>,<Padd>,<n>,<gear>,<vair_res>,<vair_beta>,<Aux_HVAC>,<Aux_HP>\n1,1,1,1,1,1,1,1,1,1",
				CycleType.MeasuredSpeedGear);
			TestCycleRead("<t>,<v>,<grad>,<Padd>,<n>,<gear>,<vair_res>,<vair_beta>\n1,1,1,1,1,1,1,1",
				CycleType.MeasuredSpeedGear);
			TestCycleRead("<t>,<v>,<grad>,<Padd>,<n>,<gear>,<Aux_HVAC>,<Aux_HP>\n1,1,1,1,1,1,1,1", CycleType.MeasuredSpeedGear);
			TestCycleRead("<t>,<v>,<grad>,<Padd>,<n>,<gear>\n1,1,1,1,1,1", CycleType.MeasuredSpeedGear);
			TestCycleRead("<t>,<v>,<grad>,<n>,<gear>\n1,1,1,1,1", CycleType.MeasuredSpeedGear);
			TestCycleRead("<n>,<Padd>,<gear>,<v>,<grad>,<t>\n1,1,1,1,1,1", CycleType.MeasuredSpeedGear);
			TestCycleRead("t,v,grad,Padd,n,gear\n1,1,1,1,1,1", CycleType.MeasuredSpeedGear);

			// wrong cycles
			AssertHelper.Exception<VectoException>(
				() => TestCycleRead("v,grad,Padd,n,gear\n1,1,1,1,1", CycleType.MeasuredSpeedGear));
			AssertHelper.Exception<VectoException>(() => TestCycleRead("<t>,<grad>\n1,1,1,1,1,1,1,1,1", CycleType.MeasuredSpeed));
			AssertHelper.Exception<VectoException>(
				() => TestCycleRead("<t>,<Pwheel>,<n>,<Padd>\n1,1,1,1,1,1,1,1,1", CycleType.PWheel));
			AssertHelper.Exception<VectoException>(
				() => TestCycleRead("<t>,<Pwheel>,<Pwheel>,<n>,<Padd>\n1,1,1,1,1,1,1,1,1", CycleType.PWheel));
			AssertHelper.Exception<VectoException>(
				() => TestCycleRead("<t>,<n>,<torque>,<>,<Padd>\n1,1,1,1,1,1,1,1,1", CycleType.EngineOnly));
			AssertHelper.Exception<VectoException>(() => TestCycleRead("x,y,z\n1,1,1,1,1,1,1,1,1", CycleType.EngineOnly));
			AssertHelper.Exception<VectoException>(() => TestCycleRead("x\n1,1,1,1,1,1,1,1,1", CycleType.EngineOnly));
			AssertHelper.Exception<VectoException>(() => TestCycleRead("\n1,1,1,1,1,1,1,1,1", CycleType.MeasuredSpeed));
			AssertHelper.Exception<VectoException>(() => TestCycleRead(
				"<t>,<v>,<gear>,<Pwheel>,<s>,<grad>,<Padd>,<n>,<gear>,<vair_res>,<vair_beta>,<Aux_HVAC>,<Aux_HP>\n1,1,1,1,1,1,1,1,1",
				CycleType.MeasuredSpeedGear));
		}

		[TestMethod]
		public void DrivingCycle_Read_File()
		{
			TestCycleDetect(File.ReadAllText(@"TestData\Cycles\Coach.vdri"), CycleType.DistanceBased);
			TestCycleRead(File.ReadAllText(@"TestData\Cycles\Coach.vdri"), CycleType.DistanceBased, 6116);

			TestCycleDetect(File.ReadAllText(@"TestData\Cycles\Engine Only1.vdri"), CycleType.EngineOnly);
			TestCycleRead(File.ReadAllText(@"TestData\Cycles\Engine Only1.vdri"), CycleType.EngineOnly, 696);

			TestCycleDetect(File.ReadAllText(@"TestData\Pwheel\RD_#1_Pwheel_AuxStd.vdri"), CycleType.PWheel);
			TestCycleRead(File.ReadAllText(@"TestData\Pwheel\RD_#1_Pwheel_AuxStd.vdri"), CycleType.PWheel, 3917);

			TestCycleDetect(File.ReadAllText(@"TestData\MeasuredSpeed\MeasuredSpeedVairAux.vdri"), CycleType.MeasuredSpeed);
			TestCycleRead(File.ReadAllText(@"TestData\MeasuredSpeed\MeasuredSpeedVairAux.vdri"), CycleType.MeasuredSpeed, 1300);

			TestCycleDetect(File.ReadAllText(@"TestData\MeasuredSpeed\MeasuredSpeed_Gear_Rural_VairAux.vdri"),
				CycleType.MeasuredSpeedGear);
			TestCycleRead(File.ReadAllText(@"TestData\MeasuredSpeed\MeasuredSpeed_Gear_Rural_VairAux.vdri"),
				CycleType.MeasuredSpeedGear, 1300);
		}

		private static void TestCycleDetect(string inputData, CycleType cycleType)
		{
			var cycleTypeCalc = DrivingCycleDataReader.DetectCycleType(VectoCSVFile.ReadStream(inputData.GetStream()));
			Assert.AreEqual(cycleType, cycleTypeCalc);
		}

		private static void TestCycleRead(string inputData, CycleType cycleType, int entryCount = 1)
		{
			var drivingCycle = DrivingCycleDataReader.ReadFromStream(inputData.GetStream(), cycleType, "", false);
			Assert.AreEqual(cycleType, drivingCycle.CycleType);
			Assert.AreEqual(entryCount, drivingCycle.Entries.Count, "Driving Cycle Entry count.");
		}
	}
}