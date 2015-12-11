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
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData
{
	[TestClass]
	public class VehicleDataTest
	{
		private const string VehicleDataFile = @"TestData\Components\24t Coach.vveh";

		[TestMethod]
		public void ReadVehicleFileTest()
		{
			//IDataFileReader reader = new EngineeringModeSimulationDataReader();
			//var vehicleData = reader.ReadVehicleDataFile(VehicleDataFile);
			//VehicleData.ReadFromFile(VehicleDataFile);

			//Assert.AreEqual(VehicleCategory.Coach, vehicleData.VehicleCategory);
		}
	}
}