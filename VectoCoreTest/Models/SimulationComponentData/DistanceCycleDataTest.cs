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
using TUGraz.VectoCore.FileIO.Reader;
using TUGraz.VectoCore.FileIO.Reader.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData
{
	[TestClass]
	public class DistanceCycleDataTest
	{
//		public readonly string CycleFile = @"TestData\Cycles\";
		private const string ResourceNamespace = "TUGraz.VectoCore.Resources.Declaration.";


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