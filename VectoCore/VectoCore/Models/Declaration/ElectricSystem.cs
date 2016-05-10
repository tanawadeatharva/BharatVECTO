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
	public sealed class ElectricSystem : LookupData<MissionType, string[], Watt>
	{
		private readonly Alternator _alternator = new Alternator();

		private const string ResourceId = "TUGraz.VectoCore.Resources.Declaration.VAUX.ES-Tech.csv";
		private const string BaseLine = "Baseline electric power consumption";

		private readonly Dictionary<Tuple<MissionType, string>, Watt> _data =
			new Dictionary<Tuple<MissionType, string>, Watt>();
		
		public ElectricSystem()
		{
			ParseData(ReadCsvResource(ResourceId));
		}

		protected override void ParseData(DataTable table)
		{
			NormalizeTable(table);

			foreach (DataRow row in table.Rows) {
				var name = row.Field<string>("Technology");
				foreach (MissionType mission in Enum.GetValues(typeof(MissionType))) {
					_data[Tuple.Create(mission, name)] = row.ParseDouble(mission.ToString().ToLower()).SI<Watt>();
				}
			}
		}

		public override Watt Lookup(MissionType missionType, string[] technologies)
		{
			var sum = _data[Tuple.Create(missionType, BaseLine)];

			if (technologies != null) {
				foreach (var technology in technologies) {
					try {
						sum += _data[Tuple.Create(missionType, technology)];
					} catch (KeyNotFoundException) {
						throw new VectoException(
							"Auxiliary Lookup Error: No value found for Electric System with mission '{0}' and technology '{1}'",
							missionType, technology);
					}
				}
			}

			return sum / _alternator.Lookup(missionType, null);
		}

		private sealed class Alternator : LookupData<MissionType, string, double>
		{
			private const string ResourceId = "TUGraz.VectoCore.Resources.Declaration.VAUX.ALT-Tech.csv";
			private const string Default = "Standard alternator";

			private readonly Dictionary<Tuple<MissionType, string>, double> _data =
				new Dictionary<Tuple<MissionType, string>, double>();
			
			public Alternator()
			{
				ParseData(ReadCsvResource(ResourceId));
			}

			protected override void ParseData(DataTable table)
			{
				NormalizeTable(table);

				foreach (DataRow row in table.Rows) {
					var name = row.Field<string>("Technology");
					foreach (MissionType mission in Enum.GetValues(typeof(MissionType))) {
						_data[Tuple.Create(mission, name)] = row.ParseDouble(mission.ToString().ToLower());
					}
				}
			}

			public override double Lookup(MissionType missionType, string technology)
			{
				if (string.IsNullOrWhiteSpace(technology)) {
					technology = Default;
				}

				try {
					return _data[Tuple.Create(missionType, technology)];
				} catch (KeyNotFoundException) {
					throw new VectoException(
						"Auxiliary Lookup Error: No value found for Alternator with mission '{0}' and technology '{1}'",
						missionType, technology);
				}
			}
		}
	}
}