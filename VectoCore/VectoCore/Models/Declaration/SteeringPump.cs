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
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public class SteeringPump : LookupData<MissionType, VehicleClass, string, Watt>
	{
		private readonly SteeringPumpTechnologies _technologies = new SteeringPumpTechnologies();

		private readonly Dictionary<Tuple<MissionType, VehicleClass>, Watt[]> _data =
			new Dictionary<Tuple<MissionType, VehicleClass>, Watt[]>();

		private const string ResourceId = "TUGraz.VectoCore.Resources.Declaration.VAUX.SP-Table.csv";

		public SteeringPump()
		{
			ParseData(ReadCsvResource(ResourceId));
		}

		public override Watt Lookup(MissionType mission, VehicleClass hdvClass, string technology)
		{
			try {
				var shares = _data[Tuple.Create(mission, hdvClass)];
				var factors = _technologies.Lookup(technology);

				var sum = 0.SI<Watt>();
				for (var i = 0; i < factors.Length; i++) {
					sum += shares[i] * factors[i];
				}
				return sum;
			} catch (KeyNotFoundException) {
				throw new VectoException(
					"Auxiliary Lookup Error: No value found for Steering Pump with mission '{0}', HDVClass '{1}' and technology '{3}'",
					mission, hdvClass, technology);
			}
		}

		protected override void ParseData(DataTable table)
		{
			_data.Clear();
			NormalizeTable(table);

			foreach (DataRow row in table.Rows) {
				var hdvClass = VehicleClassHelper.Parse(row.Field<string>("hdvclass/powerdemandpershare"));
				foreach (var mission in EnumHelper.GetValues<MissionType>()) {
					var values = row.Field<string>(mission.ToString().ToLower()).Split('/').ToDouble();
					values = values.Concat(Enumerable.Repeat(0.0, 3));

					_data[Tuple.Create(mission, hdvClass)] = values.Take(4).SI<Watt>().ToArray();
				}
			}
		}

		private class SteeringPumpTechnologies : LookupData<string, double[]>
		{
			private const string ResourceId = "TUGraz.VectoCore.Resources.Declaration.VAUX.SP-Tech.csv";

			public SteeringPumpTechnologies()
			{
				ParseData(ReadCsvResource(ResourceId));
			}

			protected override void ParseData(DataTable table)
			{
				Data = table.Rows.Cast<DataRow>().ToDictionary(
					key => key.Field<string>("Scaling Factors"),
					value => new[] { value.ParseDouble("U"), value.ParseDouble("F"), value.ParseDouble("B"), value.ParseDouble("S") });
			}

			public override double[] Lookup(string tech)
			{
				try {
					return Data[tech];
				} catch (KeyNotFoundException) {
					throw new VectoException("Auxiliary Lookup Error: No value found for SteeringPump Technology with key '{0}'", tech);
				}
			}
		}
	}
}