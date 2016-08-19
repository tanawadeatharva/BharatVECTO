/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System.Diagnostics.CodeAnalysis;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.Models
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public enum GearboxType
	{
		MT, // Manual Transmission
		AMT, // Automated Manual Transmission
		ATSerial, // Automatic Transmission
		ATPowerSplit,
		//Custom,
		DrivingCycle
	}

	public static class GearBoxTypeExtension
	{
		public static bool AutomaticTransmission(this GearboxType type)
		{
			return type == GearboxType.ATPowerSplit || type == GearboxType.ATSerial;
		}

		public static bool ManualTransmission(this GearboxType type)
		{
			return type == GearboxType.MT || type == GearboxType.AMT;
		}

		public static bool EarlyShiftGears(this GearboxType type)
		{
			switch (type) {
				case GearboxType.MT:
					return false;
				case GearboxType.AMT:
					return true;
				case GearboxType.ATSerial:
				case GearboxType.ATPowerSplit:
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
				case GearboxType.ATSerial:
				case GearboxType.ATPowerSplit:
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
				case GearboxType.ATSerial:
				case GearboxType.ATPowerSplit:
					return 0.8.SI<Second>();
			}
			return 0.SI<Second>();
		}
	}
}