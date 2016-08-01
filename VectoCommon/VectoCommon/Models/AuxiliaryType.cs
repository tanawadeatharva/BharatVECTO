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

using System;
using TUGraz.VectoCommon.Configuration;

namespace TUGraz.VectoCommon.Models
{
	public enum AuxiliaryType
	{
		Fan,
		SteeringPump,
		HVAC,
		PneumaticSystem,
		ElectricSystem
	}

	public static class AuxiliaryTypeHelper
	{
		public static AuxiliaryType Parse(string s)
		{
			switch (s) {
				case Constants.Auxiliaries.Names.Fan:
					return AuxiliaryType.Fan;
				case Constants.Auxiliaries.Names.SteeringPump:
					return AuxiliaryType.SteeringPump;
				case Constants.Auxiliaries.Names.HeatingVentilationAirCondition:
					return AuxiliaryType.HVAC;
				case Constants.Auxiliaries.Names.ElectricSystem:
					return AuxiliaryType.ElectricSystem;
				case Constants.Auxiliaries.Names.PneumaticSystem:
					return AuxiliaryType.PneumaticSystem;
				default:
					throw new ArgumentOutOfRangeException("s", s, "Could not parse auxiliary type string.");
			}
		}

		public static string ToString(AuxiliaryType t)
		{
			switch (t) {
				case AuxiliaryType.Fan:
					return Constants.Auxiliaries.Names.Fan;
				case AuxiliaryType.SteeringPump:
					return Constants.Auxiliaries.Names.SteeringPump;
				case AuxiliaryType.HVAC:
					return Constants.Auxiliaries.Names.HeatingVentilationAirCondition;
				case AuxiliaryType.PneumaticSystem:
					return Constants.Auxiliaries.Names.PneumaticSystem;
				case AuxiliaryType.ElectricSystem:
					return Constants.Auxiliaries.Names.ElectricSystem;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}