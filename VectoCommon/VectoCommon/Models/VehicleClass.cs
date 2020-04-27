/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
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

using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.Models
{
	public enum VehicleClass
	{
		Unknown,
		ClassML2r,
		ClassML2van,
		ClassML3r,
		ClassML3van,
		ClassML4r,
		ClassML4van,
		Class1s,
		Class0,
		Class1,
		Class2,
		Class3,
		Class4,
		Class5,
		Class6,
		Class7,
		Class8,
		Class9,
		Class10,
		Class11,
		Class12,
		Class13,
		Class14,
		Class15,
		Class16,
		Class17,


		// primary bus super groups
		ClassP31_32,
		ClassP33_34,
		ClassP35_36,
		ClassP37_38,
		ClassP39_40,

		// primary bus specific groups
		ClassP31SD,
		ClassP31DD,
		ClassP32SD,
		ClassP32DD,
		ClassP33SD,
		ClassP33DD,
		ClassP34SD,
		ClassP34DD,
		ClassP35SD,
		ClassP35DD,
		ClassP36SD,
		ClassP36DD,
		ClassP37SD,
		ClassP37DD,
		ClassP38SD,
		ClassP38DD,
		ClassP39SD,
		ClassP39DD,
		ClassP40SD,
		ClassP40DD,
		
		// completed bus groups
		Class31a,
		Class31b,
		Class31c,
		Class31d,
		Class31e,
		Class32a,
		Class32b,
		Class32c,
		Class32d,
		Class32e,
		Class32f,

		Class33a,
		Class33b,
		Class33c,
		Class33d,
		Class33e,
		Class34a,
		Class34b,
		Class34c,
		Class34d,
		Class34e,
		Class34f,

		Class35a,
		Class35b,
		Class35c,
		Class36a,
		Class36b,
		Class36c,
		Class36d,
		Class36e,
		Class36f,

		Class37a,
		Class37b,
		Class37c,
		Class37d,
		Class37e,
		Class38a,
		Class38b,
		Class38c,
		Class38d,
		Class38e,
		Class38f,

		Class39a,
		Class39b,
		Class39c,
		Class40a,
		Class40b,
		Class40c,
		Class40d,
		Class40e,
		Class40f,
	}

	public static class VehicleClassHelper
	{
		private const string Prefix = "Class";

		public static VehicleClass Parse(string text)
		{
			return (Prefix + text).ParseEnum<VehicleClass>();
		}

		public static string GetClassNumber(this VehicleClass hdvClass)
		{
			return hdvClass == VehicleClass.Unknown ? "-" : hdvClass.ToString().Substring(Prefix.Length);
		}

		public static bool IsMediumLorry(this VehicleClass vehicleClass)
		{
			switch (vehicleClass) {
				case VehicleClass.ClassML2r:
				case VehicleClass.ClassML2van:
				case VehicleClass.ClassML3r:
				case VehicleClass.ClassML3van:
				case VehicleClass.ClassML4r:
				case VehicleClass.ClassML4van:
					return true;
				default:
					return false;
			}
		}

		public static bool IsPrimaryBus(this VehicleClass vehicleClass)
		{
			switch (vehicleClass) {
				case VehicleClass.ClassP31SD:
				case VehicleClass.ClassP31DD:
				case VehicleClass.ClassP32SD:
				case VehicleClass.ClassP32DD:
				case VehicleClass.ClassP33SD:
				case VehicleClass.ClassP33DD:
				case VehicleClass.ClassP34SD:
				case VehicleClass.ClassP34DD:
				case VehicleClass.ClassP35SD:
				case VehicleClass.ClassP35DD:
				case VehicleClass.ClassP36SD:
				case VehicleClass.ClassP36DD:
				case VehicleClass.ClassP37SD:
				case VehicleClass.ClassP37DD:
				case VehicleClass.ClassP38SD:
				case VehicleClass.ClassP38DD:
				case VehicleClass.ClassP39SD:
				case VehicleClass.ClassP39DD:
				case VehicleClass.ClassP40SD:
				case VehicleClass.ClassP40DD:
				case VehicleClass.ClassP31_32:
				case VehicleClass.ClassP33_34:
				case VehicleClass.ClassP35_36:
				case VehicleClass.ClassP37_38:
				case VehicleClass.ClassP39_40: return true;
				default: return false;

			}
		}

		public static bool IsCompletedBus(this VehicleClass vehicleClass)
		{
			switch (vehicleClass) {
					case VehicleClass.Class31a:
					case VehicleClass.Class31b:
					case VehicleClass.Class31c:
					case VehicleClass.Class31d:
					case VehicleClass.Class31e:
					case VehicleClass.Class32a:
					case VehicleClass.Class32b:
					case VehicleClass.Class32c:
					case VehicleClass.Class32d:
					case VehicleClass.Class32e:
					case VehicleClass.Class32f:
					case VehicleClass.Class33a:
					case VehicleClass.Class33b:
					case VehicleClass.Class33c:
					case VehicleClass.Class33d:
					case VehicleClass.Class33e:
					case VehicleClass.Class34a:
					case VehicleClass.Class34b:
					case VehicleClass.Class34c:
					case VehicleClass.Class34d:
					case VehicleClass.Class34e:
					case VehicleClass.Class34f:
					case VehicleClass.Class35a:
					case VehicleClass.Class35b:
					case VehicleClass.Class35c:
					case VehicleClass.Class36a:
					case VehicleClass.Class36b:
					case VehicleClass.Class36c:
					case VehicleClass.Class36d:
					case VehicleClass.Class36e:
					case VehicleClass.Class36f:
					case VehicleClass.Class37a:
					case VehicleClass.Class37b:
					case VehicleClass.Class37c:
					case VehicleClass.Class37d:
					case VehicleClass.Class37e:
					case VehicleClass.Class38a:
					case VehicleClass.Class38b:
					case VehicleClass.Class38c:
					case VehicleClass.Class38d:
					case VehicleClass.Class38e:
					case VehicleClass.Class38f:
					case VehicleClass.Class39a:
					case VehicleClass.Class39b:
					case VehicleClass.Class39c:
					case VehicleClass.Class40a:
					case VehicleClass.Class40b:
					case VehicleClass.Class40c:
					case VehicleClass.Class40d:
					case VehicleClass.Class40e:
				case VehicleClass.Class40f: return true;
				default: return false;
			}
		}
	}
}