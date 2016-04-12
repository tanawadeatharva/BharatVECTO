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
using System.Collections.Generic;
using System.Data;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public class Fan : LookupData<MissionType, string, Watt>
	{
		private readonly Dictionary<Tuple<MissionType, string>, Watt> _data =
			new Dictionary<Tuple<MissionType, string>, Watt>();

		private const string DefaultTechnology = "Crankshaft mounted - Electronically controlled visco clutch (Default)";

		protected const string ResourceId = "TUGraz.VectoCore.Resources.Declaration.VAUX.Fan-Tech.csv";

		public Fan()
		{
			ParseData(ReadCsvResource(ResourceId));
		}


		protected override void ParseData(DataTable table)
		{
			NormalizeTable(table);

			_data.Clear();
			foreach (DataRow row in table.Rows) {
				foreach (var mission in EnumHelper.GetValues<MissionType>()) {
					_data[Tuple.Create(mission, row.Field<string>("Technology"))] =
						row.ParseDouble(mission.ToString().ToLower()).SI<Watt>();
				}
			}
		}

		public override Watt Lookup(MissionType mission, string technology)
		{
			if (string.IsNullOrWhiteSpace(technology)) {
				technology = DefaultTechnology;
			}

			try {
				return _data[Tuple.Create(mission, technology)];
			} catch (KeyNotFoundException) {
				throw new VectoException("Auxiliary Lookup Error: No value found for Fan with key '{0}' in mission '{1}'",
					technology, mission);
			}
		}
	}
}