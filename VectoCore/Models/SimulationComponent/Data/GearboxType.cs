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

using System.Diagnostics.CodeAnalysis;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public enum GearboxType
	{
		MT, // Manual Transmission
		AMT, // Automated Manual Transmission
		AT, // Automatic Transmission
		Custom
	}

	public static class GearBoxTypeExtension
	{
		public static bool EarlyShiftGears(this GearboxType type)
		{
			switch (type) {
				case GearboxType.MT:
					return false;
				case GearboxType.AMT:
					return true;
				case GearboxType.AT:
					return false;
			}
			return false;
		}

		public static bool SkipGears(this GearboxType type)
		{
			switch (type) {
				case GearboxType.MT:
					return true;
				case GearboxType.AMT:
					return true;
				case GearboxType.AT:
					return false;
			}
			return false;
		}

		public static Second TractionInterruption(this GearboxType type)
		{
			switch (type) {
				case GearboxType.MT:
					return 2.SI<Second>();
				case GearboxType.AMT:
					return 1.SI<Second>();
				case GearboxType.AT:
					return 0.8.SI<Second>();
			}
			return 0.SI<Second>();
		}
	}
}