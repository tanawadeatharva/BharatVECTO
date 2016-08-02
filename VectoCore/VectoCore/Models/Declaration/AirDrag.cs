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
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class AirDrag : LookupData<string, AirDrag.Entry>
	{
		protected override string ResourceId
		{
			get { return "TUGraz.VectoCore.Resources.Declaration.VCDV.parameters.csv"; }
		}

		protected override string ErrorMessage
		{
			get { throw new NotImplementedException(); }
		}

		protected override void ParseData(DataTable table)
		{
			Data = table.Rows.Cast<DataRow>().ToDictionary(
				row => row.Field<string>("Parameters"),
				row => new Entry(row.ParseDouble("a1"), row.ParseDouble("a2"), row.ParseDouble("a3")));
		}

		public Entry Lookup(VehicleCategory category)
		{
			switch (category) {
				case VehicleCategory.CityBus:
				case VehicleCategory.InterurbanBus:
				case VehicleCategory.Coach:
					return Lookup("CoachBus");
				case VehicleCategory.Tractor:
					return Lookup("TractorSemitrailer");
				case VehicleCategory.RigidTruck:
					return Lookup("RigidSolo");
				default:
					throw new ArgumentOutOfRangeException("category", category, null);
			}
		}

		public class Entry
		{
			public double A1;
			public double A2;
			public double A3;

			public Entry(double a1, double a2, double a3)
			{
				A1 = a1;
				A2 = a2;
				A3 = a3;
			}
		}
	}
}