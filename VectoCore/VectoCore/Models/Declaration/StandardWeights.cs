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

using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	internal sealed class StandardWeight
	{
		public Kilogram CurbWeight;
		public Kilogram GrossVehicleWeight;

		public Kilogram MaxPayLoad
		{
			get { return GrossVehicleWeight - CurbWeight; }
		}

		public SquareMeter DeltaCrossWindArea;

		public static StandardWeight operator +(StandardWeight first, StandardWeight second)
		{
			return new StandardWeight {
				CurbWeight = first.CurbWeight + second.CurbWeight,
				DeltaCrossWindArea = first.DeltaCrossWindArea + second.DeltaCrossWindArea,
				GrossVehicleWeight = first.GrossVehicleWeight + second.GrossVehicleWeight
			};
		}
	}

	/// <summary>
	/// Lookup Class for Standard Weights of Bodies, Trailers and Semitrailers.
	/// Standard Weights include 
	///		CurbWeight (=Empty Weight), 
	///		Gross Vehicle Weight (=Maximum Allowed Weight), and 
	///		MaxPayload.
	/// </summary>
	internal sealed class StandardWeights : LookupData<string, StandardWeight>
	{
		private const string ResourceId = "TUGraz.VectoCore.Resources.Declaration.Body_Trailers_Weights.csv";

		public StandardWeights()
		{
			ParseData(ReadCsvResource(ResourceId));
		}

		public StandardWeight Empty = new StandardWeight {
			CurbWeight = 0.SI<Kilogram>(),
			GrossVehicleWeight = 0.SI<Kilogram>(),
			DeltaCrossWindArea = 0.SI<SquareMeter>()
		};

		public override StandardWeight Lookup(string id)
		{
			if (string.IsNullOrWhiteSpace(id)) {
				return Empty;
			}

			try {
				return Data[id];
			} catch (KeyNotFoundException) {
				throw new VectoException("StandardWeigths Lookup Error: No value found for ID '{0}'", id);
			}
		}

		protected override void ParseData(DataTable table)
		{
			NormalizeTable(table);

			Data = table.Rows.Cast<DataRow>()
				.ToDictionary(
					kv => kv.Field<string>("name"),
					kv => new StandardWeight {
						CurbWeight = kv.ParseDoubleOrGetDefault("curbmass").SI<Kilogram>(),
						GrossVehicleWeight = kv.ParseDoubleOrGetDefault("maxgrossmass").SI<Kilogram>(),
						DeltaCrossWindArea = kv.ParseDoubleOrGetDefault("deltacdxafortraileroperationinlonghaul").SI<SquareMeter>()
					});
		}
	}
}