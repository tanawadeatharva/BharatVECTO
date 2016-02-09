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

using System.IO;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public class Segment
	{
		public VehicleCategory VehicleCategory { get; set; }


		public AxleConfiguration AxleConfiguration { get; set; }

		public Kilogram GrossVehicleWeightMin { get; set; }

		public Kilogram GrossVehicleWeightMax { get; set; }

		public Kilogram GrossVehicleMassRating { get; set; }

		public VehicleClass VehicleClass { get; internal set; }

		public Stream AccelerationFile { get; internal set; }

		public Mission[] Missions { get; internal set; }
	}
}