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

namespace TUGraz.VectoCommon.Models
{
	public enum VehicleCategory
	{
		RigidTruck,
		Tractor,
		CityBus,
		InterurbanBus,
		Coach
	}

	public static class VehicleCategoryHelper
	{
		public static string GetLabel(this VehicleCategory category)
		{
			switch (category)
			{
				case VehicleCategory.RigidTruck:
					return "Rigid Truck";
				case VehicleCategory.Tractor:
					return "Tractor";
				case VehicleCategory.CityBus:
					return "City Bus";
				case VehicleCategory.InterurbanBus:
					return "Interurban Bus";
				case VehicleCategory.Coach:
					return "Coach";
				default:
					return category.ToString();
			}
		}
		public static string GetCategoryName(this VehicleCategory category)
		{
			switch (category) {
				case VehicleCategory.RigidTruck:
					return "Rigid Truck";
				case VehicleCategory.Tractor:
					return "Semitrailer Truck";
				case VehicleCategory.CityBus:
					return "Citybus";
				case VehicleCategory.InterurbanBus:
					return "Interurban Bus";
				case VehicleCategory.Coach:
					return "Coach";
				default:
					return category.ToString();
			}
		}

		public static string ToXMLFormat(this VehicleCategory vehicleCategory)
		{
			switch (vehicleCategory) {
				case VehicleCategory.Coach:
				case VehicleCategory.Tractor:
					return vehicleCategory.ToString();
				case VehicleCategory.CityBus:
					return "City Bus";
				case VehicleCategory.InterurbanBus:
					return "Interurban Bus";
				case VehicleCategory.RigidTruck:
					return "Rigid Truck";
				default:
					throw new ArgumentOutOfRangeException("vehicleCategory", vehicleCategory, null);
			}
		}
	}
}