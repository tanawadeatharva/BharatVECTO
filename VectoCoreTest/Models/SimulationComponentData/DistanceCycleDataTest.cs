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
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Integration;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData
{
	[TestClass]
	public class DistanceCycleDataTest
	{
//		public readonly string CycleFile = @"TestData\Cycles\";
		private const string ResourceNamespace = "TUGraz.VectoCore.Resources.Declaration.";


		[TestMethod]
		public void FilterRedundantEntries()
		{
			var data = new[] {
				" 0, 20, 0, 0",
				" 1, 20, 0, 0",
				" 2, 20, 0, 0",
				" 3, 20, 0, 0",
				" 4, 20, 0, 0",
				" 5, 20, 1, 0",
				" 6, 20, 1, 0",
				" 6, 20, 1, 0",
				" 8, 20, 1, 0",
				" 9, 20, 1, 0",
			};

			var cycleData = SimpleDrivingCycles.CreateCycleData(data);
			Assert.AreEqual(3, cycleData.Entries.Count);

			Assert.AreEqual(0, cycleData.Entries[0].Distance.Value());
			Assert.AreEqual(5, cycleData.Entries[1].Distance.Value());
			Assert.AreEqual(9, cycleData.Entries[2].Distance.Value());
		}

		[TestMethod]
		public void HandleStopTimes()
		{
			var data = new[] {
				" 0, 20, 0, 0",
				" 1, 20, 0, 0",
				" 2, 20, 0, 0",
				" 3, 20, 0, 0",
				" 4, 20, 0, 0",
				" 5,  0, 0, 5",
				" 6, 20, 0, 0",
				" 6, 20, 0, 0",
				" 8, 20, 0, 0",
				" 9, 20, 0, 0",
			};

			var cycleData = SimpleDrivingCycles.CreateCycleData(data);
			Assert.AreEqual(4, cycleData.Entries.Count);

			Assert.AreEqual(0, cycleData.Entries[0].Distance.Value());
			Assert.AreEqual(5, cycleData.Entries[1].Distance.Value());
			Assert.AreEqual(5, cycleData.Entries[2].Distance.Value());
			Assert.AreEqual(9, cycleData.Entries[3].Distance.Value());

			Assert.AreEqual(5, cycleData.Entries[1].StoppingTime.Value());
		}

		[TestMethod]
		public void StopTimeWhenVehicleSpeedIsNotZero()
		{
			var data = new[] {
				" 0, 20, 0, 0",
				"20, 20, 0, 5",
				"90, 20, 0, 0"
			};

			AssertHelper.Exception<VectoException>(() => SimpleDrivingCycles.CreateCycleData(data));
		}

		[TestMethod]
		public void DistanceNotStrictlyIncreasing()
		{
			var data = new[] {
				" 0, 20, 0, 0",
				"90, 20, 0, 0",
				"80, 20, 0, 0"
			};

			AssertHelper.Exception<VectoException>(() => SimpleDrivingCycles.CreateCycleData(data));
		}

		[TestMethod, Ignore]
		public void CycleAltitudeTest()
		{
			var missionType = "LongHaul";
			var stream = RessourceHelper.ReadStream(ResourceNamespace + "MissionCycles." + missionType + ".vdri");

			var cycleData = DrivingCycleDataReader.ReadFromStream(stream, CycleType.DistanceBased);
			foreach (var entry in cycleData.Entries) {
				var tmp = entry.Altitude;
			}
		}
	}
}