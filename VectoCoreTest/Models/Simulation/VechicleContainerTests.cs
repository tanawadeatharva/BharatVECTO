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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestClass]
	public class VechicleContainerTests
	{
		private const string EngineFile = @"TestData\Components\24t Coach.veng";

		[TestMethod]
		public void VechicleContainerHasEngine()
		{
			var vehicle = new VehicleContainer();
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile);
			var engine = new CombustionEngine(vehicle, engineData);

			Assert.IsNotNull(vehicle.EngineSpeed);
		}
	}
}