/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
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
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class FuelData : LookupData<FuelType, FuelData.Entry>
	{
		private static FuelData _instance;

		public static FuelData Instance()
		{
			return _instance ?? (_instance = new FuelData());
		}

		private FuelData() {}

		protected override string ResourceId
		{
			get { return DeclarationData.DeclarationDataResourcePrefix + ".FuelTypes.csv"; }
		}

		protected override string ErrorMessage
		{
			get { throw new InvalidOperationException("ErrorMessage not applicable."); }
		}

		public static Entry Diesel
		{
			get { return Instance().Lookup(FuelType.DieselCI); }
		}

		protected override void ParseData(DataTable table)
		{
			Data = table.Rows.Cast<DataRow>()
				.Select(r => {
					var density = r.Field<string>("fueldensity");
					return new Entry(
						r.Field<string>(0).ParseEnum<FuelType>(),
						string.IsNullOrWhiteSpace(density) ? null : density.ToDouble(0).SI<KilogramPerCubicMeter>(),
						r.ParseDouble("co2perfuelweight"),
                        //r.ParseDouble("lowerheatingvalue").SI().Kilo.Joule.Per.Kilo.Gramm.Cast<JoulePerKilogramm>()
						r.ParseDouble("lowerheatingvalue").SI(Unit.SI.Kilo.Joule.Per.Kilo.Gramm).Cast<JoulePerKilogramm>()
                        );
				})
				.ToDictionary(e => e.FuelType);
		}

		public struct Entry
		{
			public Entry(FuelType type, KilogramPerCubicMeter density, double weight, JoulePerKilogramm heatingValue) : this()
			{
				FuelType = type;
				FuelDensity = density;
				CO2PerFuelWeight = weight;
				LowerHeatingValue = heatingValue;
			}

			public FuelType FuelType { get; private set; }

			public KilogramPerCubicMeter FuelDensity { get; private set; }

			public double CO2PerFuelWeight { get; private set; }

			public JoulePerKilogramm LowerHeatingValue { get; private set; }
		}
	}
}