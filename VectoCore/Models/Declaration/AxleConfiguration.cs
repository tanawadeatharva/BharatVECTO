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

using System;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.Declaration
{
	public enum AxleConfiguration
	{
		AxleConfig_4x2,
		AxleConfig_4x4,
		AxleConfig_6x2,
		AxleConfig_6x4,
		AxleConfig_6x6,
		AxleConfig_8x2,
		AxleConfig_8x4,
		AxleConfig_8x6,
		AxleConfig_8x8,
	}

	public static class AxleConfigurationHelper
	{
		private const string Prefix = "AxleConfig_";

		public static string GetName(this AxleConfiguration self)
		{
			return self.ToString().Replace(Prefix, "");
		}

		public static AxleConfiguration Parse(string typeString)
		{
			return (Prefix + typeString).Parse<AxleConfiguration>();
		}
	}
}