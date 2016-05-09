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
	public sealed class AirDrag : LookupData<string, AirDrag.AirDragEntry>
	{
		private const string ResourceId = "TUGraz.VectoCore.Resources.Declaration.VCDV.parameters.csv";

		public AirDrag()
		{
			ParseData(ReadCsvResource(ResourceId));
		}

		protected override void ParseData(DataTable table)
		{
			Data = table.Rows.Cast<DataRow>().ToDictionary(row => row.Field<string>("Parameters"), row => new AirDragEntry {
				A1 = row.ParseDouble("a1"),
				A2 = row.ParseDouble("a2"),
				A3 = row.ParseDouble("a3")
			});
		}

		public AirDragEntry Lookup(VehicleCategory category)
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

		public class AirDragEntry
		{
			public double A1 { get; set; }
			public double A2 { get; set; }
			public double A3 { get; set; }

			protected bool Equals(AirDragEntry other)
			{
				return A1.Equals(other.A1) && A2.Equals(other.A2) && A3.Equals(other.A3);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) {
					return false;
				}
				if (ReferenceEquals(this, obj)) {
					return true;
				}
				if (obj.GetType() != GetType()) {
					return false;
				}
				return Equals((AirDragEntry)obj);
			}

			public override int GetHashCode()
			{
				unchecked {
					var hashCode = A1.GetHashCode();
					hashCode = (hashCode * 397) ^ A2.GetHashCode();
					hashCode = (hashCode * 397) ^ A3.GetHashCode();
					return hashCode;
				}
			}
		}
	}
}