/*
* Copyright 2015, 2016 Graz University of Technology,
* Institute of Internal Combustion Engines and Thermodynamics,
* Institute of Technical Informatics
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData
{
	[TestClass]
	public class FuelConsumptionMapTest
	{
		private const double Tolerance = 0.0001;

		[TestMethod]
		public void TestFuelConsumption_FixedPoints()
		{
			var map = FuelConsumptionMap.ReadFromFile(@"TestData\Components\24t Coach.vmap");
			var lines = File.ReadAllLines(@"TestData\Components\24t Coach.vmap").Skip(1).ToArray();
			AssertMapValuesEqual(lines, map);
		}

		[TestMethod]
		public void TestFuelConsumption_InterpolatedPoints()
		{
			var map = FuelConsumptionMap.ReadFromFile(@"TestData\Components\24t Coach.vmap");
			var lines = File.ReadAllLines(@"TestData\Components\24t CoachInterpolated.vmap").Skip(1).ToArray();
			AssertMapValuesEqual(lines, map);
		}

		private static void AssertMapValuesEqual(string[] lines, FuelConsumptionMap map)
		{
			for (var i = 1; i < lines.Count(); i++) {
				var entry = lines[i].Split(',').Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToArray();
				try {
					Assert.AreEqual(entry[2].SI().Gramm.Per.Hour.ConvertTo().Kilo.Gramm.Per.Second.Value(),
						map.GetFuelConsumption(entry[1].SI<NewtonMeter>(), entry[0].RPMtoRad()).Value(),
						Tolerance,
						string.Format("Line: {0}, n={1}, T={2}", (i + 2), entry[0].SI().Rounds.Per.Minute, entry[1]));
				} catch (VectoException ex) {
					throw new VectoException(string.Format("Row {0}: Error in ConsumptionMap n={1}, T={2}: {3}",
						i + 2, entry[0], entry[1], ex.Message));
				}
			}
		}
	}
}