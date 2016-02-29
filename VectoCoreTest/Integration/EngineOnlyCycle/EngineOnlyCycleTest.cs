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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration.EngineOnlyCycle
{
	[TestClass]
	public class EngineOnlyCycleTest
	{
		private const string EngineFile = @"TestData\Components\24t Coach.veng";
		public TestContext TestContext { get; set; }

		[DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\TestData\\EngineTests.csv",
			"EngineTests#csv", DataAccessMethod.Sequential)]
		[TestMethod]
		public void TestEngineOnlyDrivingCycle()
		{
			var data = DrivingCycleDataReader.ReadFromFile(TestContext.DataRow["CycleFile"].ToString(), CycleType.EngineOnly, false);
			var vehicle = new VehicleContainer();
			var cycle = new MockDrivingCycle(vehicle, data);
			var engineData =
				MockSimulationDataFactory.CreateEngineDataFromFile(TestContext.DataRow["EngineFile"].ToString());

			var aux = new EngineAuxiliary(vehicle);
			aux.AddDirect();

			var engine = new EngineOnlyCombustionEngine(vehicle, engineData);
			engine.Connect(aux);

			//aux.InPort().Connect(engine.OutPort());
			var port = engine.OutPort();

			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();

			var modFile = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()); // + ".vmod";
			var fileWriter = new FileOutputWriter(modFile, "");
			var modData = new ModalDataContainer(modFile, fileWriter, ExecutionMode.EngineOnly);

			port.Initialize(data.Entries.First().Torque, data.Entries.First().AngularVelocity);
			foreach (var cycleEntry in data.Entries) {
				var response = port.Request(absTime, dt, cycleEntry.Torque, cycleEntry.AngularVelocity);
				Assert.IsInstanceOfType(response, typeof(ResponseSuccess));
				foreach (var sc in vehicle.SimulationComponents()) {
					modData[ModalResultField.time] = absTime + dt / 2;
					sc.CommitSimulationStep(modData);
				}

				modData.CommitSimulationStep();
				absTime += dt;
			}
			modData.Finish(VectoRun.Status.Success);

			ResultFileHelper.TestModFile(TestContext.DataRow["ModalResultFile"].ToString(),
				modFile + Constants.FileExtensions.ModDataFile);
		}

		[TestMethod]
		public void AssembleEngineOnlyPowerTrain()
		{
			var dataWriter = new MockModalDataContainer();

			var vehicleContainer = new VehicleContainer();

			var engine = new CombustionEngine(vehicleContainer, MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile));

			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();

			var angularVelocity = 644.4445.RPMtoRad();
			var power = 2329.973.SI<Watt>();

			engine.OutPort().Initialize(power / angularVelocity, angularVelocity);
			engine.OutPort().Request(absTime, dt, power / angularVelocity, angularVelocity);

			foreach (var sc in vehicleContainer.SimulationComponents()) {
				sc.CommitSimulationStep(dataWriter);
			}

			Assert.IsNotNull(dataWriter.CurrentRow);
		}
	}
}