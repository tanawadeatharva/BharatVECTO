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

using System.IO;
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
			var data = DrivingCycleDataReader.ReadFromFile(TestContext.DataRow["CycleFile"].ToString(), CycleType.EngineOnly);
			var container = new VehicleContainer();
			var cycle = new MockDrivingCycle(container, data);
			var vehicle = new VehicleContainer();
			var engineData =
				MockSimulationDataFactory.CreateEngineDataFromFile(TestContext.DataRow["EngineFile"].ToString());

			var aux = new Auxiliary(vehicle);
			aux.AddDirect(cycle);

			var engine = new EngineOnlyCombustionEngine(vehicle, engineData);

			aux.InPort().Connect(engine.OutPort());
			var port = aux.OutPort();

			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();

			var modFile = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()); // + ".vmod";
			var fileWriter = new FileOutputWriter(modFile, "");
			var modData = new ModalDataContainer(modFile, fileWriter, ExecutionMode.EngineOnly);

			foreach (var cycleEntry in data.Entries) {
				var response = port.Request(absTime, dt, cycleEntry.EngineTorque, cycleEntry.EngineSpeed);
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

			engine.OutPort().Request(absTime, dt, power / angularVelocity, angularVelocity);

			foreach (var sc in vehicleContainer.SimulationComponents()) {
				sc.CommitSimulationStep(dataWriter);
			}

			Assert.IsNotNull(dataWriter.CurrentRow);
		}
	}
}