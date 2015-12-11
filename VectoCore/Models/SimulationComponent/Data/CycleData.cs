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

using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class CycleData
	{
		/// <summary>
		///     The current absolute distance in the driving cycle.
		/// </summary>
		public Meter AbsDistance;

		/// <summary>
		///     The current absolute time in the driving cycle.
		/// </summary>
		public Second AbsTime;

		/// <summary>
		///     The left data sample of the current driving cycle position. (current start point)
		/// </summary>
		public DrivingCycleData.DrivingCycleEntry LeftSample;

		/// <summary>
		///     The right data sample of the current driving cycle position. (current end point)
		/// </summary>
		public DrivingCycleData.DrivingCycleEntry RightSample;
	}
}