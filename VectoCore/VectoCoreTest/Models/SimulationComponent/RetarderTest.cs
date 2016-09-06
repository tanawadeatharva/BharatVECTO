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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestClass]
	public class RetarderTest
	{
		private const string RetarderLossMapFile = @"TestData\Components\Retarder.vrlm";
		private const double Delta = 0.0001;

		[TestMethod]
		public void RetarderBasicTest()
		{
			var vehicle = new VehicleContainer(ExecutionMode.Declaration);
			var retarderData = RetarderLossMapReader.ReadFromFile(RetarderLossMapFile);
			var retarder = new Retarder(vehicle, retarderData, 1.0);

			var nextRequest = new MockTnOutPort();

			retarder.InPort().Connect(nextRequest);
			var outPort = retarder.OutPort();

			var absTime = 0.SI<Second>();
			var dt = 0.SI<Second>();

			// --------
			outPort.Initialize(0.SI<NewtonMeter>(), 10.RPMtoRad());
			outPort.Request(absTime, dt, 0.SI<NewtonMeter>(), 10.RPMtoRad());

			Assert.AreEqual(10.RPMtoRad().Value(), nextRequest.AngularVelocity.Value(), Delta);
			Assert.AreEqual(10.002, nextRequest.Torque.Value(), Delta);

			// --------
			outPort.Initialize(100.SI<NewtonMeter>(), 1000.RPMtoRad());
			outPort.Request(absTime, dt, 100.SI<NewtonMeter>(), 1000.RPMtoRad());

			Assert.AreEqual(1000.RPMtoRad().Value(), nextRequest.AngularVelocity.Value(), Delta);
			Assert.AreEqual(112, nextRequest.Torque.Value(), Delta);

			// --------
			outPort.Initialize(50.SI<NewtonMeter>(), 650.RPMtoRad());
			outPort.Request(absTime, dt, 50.SI<NewtonMeter>(), 1550.RPMtoRad());
			retarder.CommitSimulationStep(new MockModalDataContainer());
			Assert.AreEqual(1550.RPMtoRad().Value(), nextRequest.AngularVelocity.Value(), Delta);

			// (650+1550)/2 = 1100 => 12.42Nm
			Assert.AreEqual(50 + 12.42, nextRequest.Torque.Value(), Delta);

			//VECTO-307: added an additional request after a commit
			outPort.Request(absTime, dt, 50.SI<NewtonMeter>(), 450.RPMtoRad());
			Assert.AreEqual(450.RPMtoRad().Value(), nextRequest.AngularVelocity.Value(), Delta);
			// avg: (1550+450)/2 = 1000 rpm => 12Nm
			Assert.AreEqual(50 + 12, nextRequest.Torque.Value(), Delta);
		}

		[TestMethod]
		public void RetarderRatioTest()
		{
			var vehicle = new VehicleContainer(ExecutionMode.Engineering);
			var retarderData = RetarderLossMapReader.ReadFromFile(RetarderLossMapFile);
			var retarder = new Retarder(vehicle, retarderData, 2.0);

			var nextRequest = new MockTnOutPort();

			retarder.InPort().Connect(nextRequest);
			var outPort = retarder.OutPort();

			var absTime = 0.SI<Second>();
			var dt = 0.SI<Second>();

			// --------
			outPort.Initialize(0.SI<NewtonMeter>(), 10.RPMtoRad());
			outPort.Request(absTime, dt, 0.SI<NewtonMeter>(), 10.RPMtoRad());

			Assert.AreEqual(10.RPMtoRad().Value(), nextRequest.AngularVelocity.Value(), Delta);
			Assert.AreEqual(5.002, nextRequest.Torque.Value(), Delta);

			// --------
			outPort.Initialize(100.SI<NewtonMeter>(), 1000.RPMtoRad());
			outPort.Request(absTime, dt, 100.SI<NewtonMeter>(), 1000.RPMtoRad());

			Assert.AreEqual(1000.RPMtoRad().Value(), nextRequest.AngularVelocity.Value(), Delta);
			Assert.AreEqual(109, nextRequest.Torque.Value(), Delta);

			// --------
			outPort.Initialize(50.SI<NewtonMeter>(), 1550.RPMtoRad());
			outPort.Request(absTime, dt, 50.SI<NewtonMeter>(), 1550.RPMtoRad());

			Assert.AreEqual(1550.RPMtoRad().Value(), nextRequest.AngularVelocity.Value(), Delta);
			Assert.AreEqual(50 + 13.89, nextRequest.Torque.Value(), Delta); // extrapolated
		}

		[TestMethod]
		public void RetarderDeclarationTest()
		{
			var retarderData = RetarderLossMapReader.ReadFromFile(RetarderLossMapFile);
			var declVehicle = new VehicleContainer(ExecutionMode.Declaration, null, null);
			var retarder = new Retarder(declVehicle, retarderData, 2.0);
			var nextRequest = new MockTnOutPort();

			retarder.InPort().Connect(nextRequest);
			var outPort = retarder.OutPort();

			outPort.Initialize(50.SI<NewtonMeter>(), 2550.RPMtoRad());
			outPort.Request(0.SI<Second>(), 0.SI<Second>(), 50.SI<NewtonMeter>(), 2550.RPMtoRad());
			AssertHelper.Exception<VectoException>(() => retarder.CommitSimulationStep(new MockModalDataContainer()),
				"Retarder LossMap data was extrapolated in Declaration mode: range for loss map is not sufficient: n:2550 (min:0, max:2300), ratio:2");
		}

		[TestMethod]
		public void RetarderDataSorting()
		{
			var retarderEntries = new[] {
				"100,10.02",
				"0,10",
				"200,10.08",
				"500,10.5",
				"300,10.18",
				"400,10.32",
			};
			var retarderTbl =
				VectoCSVFile.ReadStream(InputDataHelper.InputDataAsStream("Retarder Speed [rpm],Loss Torque [Nm]",
					retarderEntries));
			var vehicle = new VehicleContainer(ExecutionMode.Engineering, null, null);
			var retarderData = RetarderLossMapReader.Create(retarderTbl);
			var retarder = new Retarder(vehicle, retarderData, 2.0);

			var nextRequest = new MockTnOutPort();

			retarder.InPort().Connect(nextRequest);
			var outPort = retarder.OutPort();

			var absTime = 0.SI<Second>();
			var dt = 0.SI<Second>();

			// --------
			outPort.Initialize(100.SI<NewtonMeter>(), 125.RPMtoRad());
			outPort.Request(absTime, dt, 100.SI<NewtonMeter>(), 125.RPMtoRad());

			Assert.AreEqual(125.RPMtoRad().Value(), nextRequest.AngularVelocity.Value(), Delta);
			Assert.AreEqual(100 + 5.065, nextRequest.Torque.Value(), Delta);
		}
	}
}