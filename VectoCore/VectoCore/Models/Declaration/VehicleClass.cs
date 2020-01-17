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

namespace TUGraz.VectoCore.Models.Declaration
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
		ClassPB41,
		ClassPB42,
		ClassPB43,
		ClassPB44,
		ClassPB45
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

		public static bool IsMediumLorry(VehicleClass vehicleClass)
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
	}
}