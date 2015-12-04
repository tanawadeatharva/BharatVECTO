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

using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	/// <summary>
	/// Defines a method to access shared data of the driving cycle.
	/// </summary>
	/// 
	public interface IDrivingCycleInfo
	{
		/// <summary>
		/// Returns the data samples for the current position in the cycle.
		/// </summary>
		CycleData CycleData();
	}
}