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
	public sealed class SteeringPump : LookupData<MissionType, VehicleClass, IEnumerable<string>, Watt>
	{
		private const string ResourceId = "TUGraz.VectoCore.Resources.Declaration.VAUX.SP-Table.csv";
		private readonly SteeringPumpTechnologies _technologies = new SteeringPumpTechnologies();
		private readonly SteeringPumpAxles _axles = new SteeringPumpAxles();

		private readonly Dictionary<Tuple<MissionType, VehicleClass>, SteeringPumpValues<Watt>> _data =
			new Dictionary<Tuple<MissionType, VehicleClass>, SteeringPumpValues<Watt>>();

		public SteeringPump()
		{
			ParseData(ReadCsvResource(ResourceId));
		}

		public override Watt Lookup(MissionType mission, VehicleClass hdvClass, IEnumerable<string> technologies)
		{
			SteeringPumpValues<Watt> powerShares;
			try {
				powerShares = _data[Tuple.Create(mission, hdvClass)];
			} catch (KeyNotFoundException) {
				throw new VectoException(
					"Auxiliary Lookup Error: No value found for Steering Pump. Mission: '{0}', HDVClass: '{1}'", mission, hdvClass);
			}

			var sum = 0.SI<Watt>();
			var i = 1;
			foreach (var technology in technologies) {
				var factors = _technologies.Lookup(technology, mission);
				var axles = _axles.Lookup(mission, i);
				sum += powerShares.UnloadedFriction * axles.UnloadedFriction * factors.UnloadedFriction
						+ powerShares.Banking * axles.Banking * factors.Banking
						+ powerShares.Steering * axles.Banking * factors.Steering;
				i++;
			}
			return sum;
		}

		protected override void ParseData(DataTable table)
		{
			NormalizeTable(table);
			_data.Clear();

			foreach (DataRow row in table.Rows) {
				var hdvClass = VehicleClassHelper.Parse(row.Field<string>("hdvclass/powerdemandpershare"));
				foreach (var mission in EnumHelper.GetValues<MissionType>()) {
					var values =
						row.Field<string>(mission.ToString().ToLower()).Split('/')
							.Select(v => v.ToDouble() / 100.0).Concat(0.0.Repeat(3)).SI<Watt>().ToList();
					_data[Tuple.Create(mission, hdvClass)] = new SteeringPumpValues<Watt>(values[0], values[1], values[2]);
				}
			}
		}

		private sealed class SteeringPumpTechnologies : LookupData<string, SteeringPumpValues<double>>
		{
			private readonly ElectricSystem.Alternator _alternator = new ElectricSystem.Alternator();

			private const string ResourceId = "TUGraz.VectoCore.Resources.Declaration.VAUX.SP-Tech.csv";

			public SteeringPumpTechnologies()
			{
				ParseData(ReadCsvResource(ResourceId));
			}

			protected override void ParseData(DataTable table)
			{
				NormalizeTable(table);
				Data.Clear();

				Data = table.Rows.Cast<DataRow>().ToDictionary(
					key => key.Field<string>("Technology"),
					value => new SteeringPumpValues<double>(value.ParseDouble("UF"), value.ParseDouble("B"), value.ParseDouble("S")));
			}

			[Obsolete("Use Lookup(string, MissionType) instead!", true)]
			public new SteeringPumpValues<double> Lookup(string tech)
			{
				throw new NotImplementedException("Use Lookup(string, MissionType) instead!");
			}

			public SteeringPumpValues<double> Lookup(string tech, MissionType mission)
			{
				try {
					var values = Data[tech];
					if (tech == "Electric") {
						values.Banking /= _alternator.Lookup(mission, "");
						values.Steering /= _alternator.Lookup(mission, "");
					}
					return values;
				} catch (KeyNotFoundException) {
					throw new VectoException("Auxiliary Lookup Error: No value found for SteeringPump Technology with key '{0}'", tech);
				}
			}
		}

		private sealed class SteeringPumpAxles : LookupData<MissionType, int, SteeringPumpValues<double>>
		{
			private const string ResourceId = "TUGraz.VectoCore.Resources.Declaration.VAUX.SP-Axles.csv";

			public SteeringPumpAxles()
			{
				ParseData(ReadCsvResource(ResourceId));
			}

			protected override void ParseData(DataTable table)
			{
				NormalizeTable(table);
				Data.Clear();

				var i = 1;
				foreach (DataRow row in table.Rows) {
					foreach (MissionType mission in Enum.GetValues(typeof(MissionType))) {
						var values =
							row.Field<string>(mission.ToString().ToLowerInvariant()).Split('/').ToDouble(0).Concat(0.0.Repeat(3)).ToList();
						Data[Tuple.Create(mission, i)] = new SteeringPumpValues<double>(values[0], values[1], values[2]);
					}
					i++;
				}
			}
		}
	}

	internal struct SteeringPumpValues<T>
	{
		public T UnloadedFriction;
		public T Banking;
		public T Steering;

		public SteeringPumpValues(T unloadedFriction, T banking, T steering)
		{
			UnloadedFriction = unloadedFriction;
			Banking = banking;
			Steering = steering;
		}
	}
}