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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestClass]
	public class PowerTrainBuilderTest
	{
		public const string JobFile = @"TestData\Jobs\24t Coach.vecto";

		[TestMethod]
		public void BuildFullPowerTrainTest()
		{
			var dataProvider = JSONInputDataFactory.ReadJsonJob(JobFile);
			var reader = new EngineeringModeVectoRunDataFactory(dataProvider);
			var runData = reader.NextRun().First();

			var writer = new MockModalDataContainer();
			var builder = new PowertrainBuilder(writer, false);

			var powerTrain = builder.Build(runData);

			Assert.IsInstanceOfType(powerTrain, typeof(IVehicleContainer));
			Assert.AreEqual(11, powerTrain.Components.Count);

			Assert.IsInstanceOfType(powerTrain.Engine, typeof(CombustionEngine));
			Assert.IsInstanceOfType(powerTrain.Gearbox, typeof(Gearbox));
			Assert.IsInstanceOfType(powerTrain.Cycle, typeof(ISimulationOutPort));
			Assert.IsInstanceOfType(powerTrain.Vehicle, typeof(Vehicle));
		}
	}
}