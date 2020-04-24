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
		ClassPB41,
		ClassPB42,
		ClassPB43,
		ClassPB44,
		ClassPB45,

		ClassCB31a,
		ClassCB31b,
		ClassCB31c,
		ClassCB31d,
		ClassCB31e,
		ClassCB32a,
		ClassCB32b,
		ClassCB32c,
		ClassCB32d,
		ClassCB32e,
		ClassCB32f,

		ClassCB33a,
		ClassCB33b,
		ClassCB33c,
		ClassCB33d,
		ClassCB33e,
		ClassCB34a,
		ClassCB34b,
		ClassCB34c,
		ClassCB34d,
		ClassCB34e,
		ClassCB34f,

		ClassCB35a,
		ClassCB35b,
		ClassCB35c,
		ClassCB36a,
		ClassCB36b,
		ClassCB36c,
		ClassCB36d,
		ClassCB36e,
		ClassCB36f,

		ClassCB37a,
		ClassCB37b,
		ClassCB37c,
		ClassCB37d,
		ClassCB37e,
		ClassCB38a,
		ClassCB38b,
		ClassCB38c,
		ClassCB38d,
		ClassCB38e,
		ClassCB38f,

		ClassCB39a,
		ClassCB39b,
		ClassCB39c,
		ClassCB40a,
		ClassCB40b,
		ClassCB40c,
		ClassCB40d,
		ClassCB40e,
		ClassCB40f,
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
				case VehicleClass.ClassPB41:
				case VehicleClass.ClassPB42:
				case VehicleClass.ClassPB43:
				case VehicleClass.ClassPB44:
				case VehicleClass.ClassPB45: return true;
				default: return false;

			}
		}

		public static bool IsCompletedBus(this VehicleClass vehicleClass)
		{
			switch (vehicleClass) {
					case VehicleClass.ClassCB31a:
					case VehicleClass.ClassCB31b:
					case VehicleClass.ClassCB31c:
					case VehicleClass.ClassCB31d:
					case VehicleClass.ClassCB31e:
					case VehicleClass.ClassCB32a:
					case VehicleClass.ClassCB32b:
					case VehicleClass.ClassCB32c:
					case VehicleClass.ClassCB32d:
					case VehicleClass.ClassCB32e:
					case VehicleClass.ClassCB32f:
					case VehicleClass.ClassCB33a:
					case VehicleClass.ClassCB33b:
					case VehicleClass.ClassCB33c:
					case VehicleClass.ClassCB33d:
					case VehicleClass.ClassCB33e:
					case VehicleClass.ClassCB34a:
					case VehicleClass.ClassCB34b:
					case VehicleClass.ClassCB34c:
					case VehicleClass.ClassCB34d:
					case VehicleClass.ClassCB34e:
					case VehicleClass.ClassCB34f:
					case VehicleClass.ClassCB35a:
					case VehicleClass.ClassCB35b:
					case VehicleClass.ClassCB35c:
					case VehicleClass.ClassCB36a:
					case VehicleClass.ClassCB36b:
					case VehicleClass.ClassCB36c:
					case VehicleClass.ClassCB36d:
					case VehicleClass.ClassCB36e:
					case VehicleClass.ClassCB36f:
					case VehicleClass.ClassCB37a:
					case VehicleClass.ClassCB37b:
					case VehicleClass.ClassCB37c:
					case VehicleClass.ClassCB37d:
					case VehicleClass.ClassCB37e:
					case VehicleClass.ClassCB38a:
					case VehicleClass.ClassCB38b:
					case VehicleClass.ClassCB38c:
					case VehicleClass.ClassCB38d:
					case VehicleClass.ClassCB38e:
					case VehicleClass.ClassCB38f:
					case VehicleClass.ClassCB39a:
					case VehicleClass.ClassCB39b:
					case VehicleClass.ClassCB39c:
					case VehicleClass.ClassCB40a:
					case VehicleClass.ClassCB40b:
					case VehicleClass.ClassCB40c:
					case VehicleClass.ClassCB40d:
					case VehicleClass.ClassCB40e:
				case VehicleClass.ClassCB40f: return true;
				default: return false;
			}
		}
	}
}