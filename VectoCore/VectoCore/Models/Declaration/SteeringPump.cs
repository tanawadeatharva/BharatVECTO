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
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class SteeringPump
	{
		private readonly SteeringPumpBaseLine _baseline = new SteeringPumpBaseLine();
		private readonly SteeringPumpAxles _axles = new SteeringPumpAxles();
		private readonly SteeringPumpTechnologies _technologies = new SteeringPumpTechnologies();

		public Watt Lookup(MissionType mission, VehicleClass hdvClass, IEnumerable<string> technologies)
		{
			var powerShares = _baseline.Lookup(mission, hdvClass);
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

		private sealed class SteeringPumpBaseLine : LookupData<MissionType, VehicleClass, SteeringPumpValues<Watt>>
		{
			protected override string ResourceId
			{
				get { return "TUGraz.VectoCore.Resources.Declaration.VAUX.SP-Table.csv"; }
			}

			protected override string ErrorMessage
			{
				get { return "Auxiliary Lookup Error: No value found for Steering Pump. Mission: '{0}', HDVClass: '{1}'"; }
			}

			protected override void ParseData(DataTable table)
			{
				NormalizeTable(table);
				Data.Clear();

				foreach (DataRow row in table.Rows) {
					var hdvClass = VehicleClassHelper.Parse(row.Field<string>("hdvclass/powerdemandpershare"));
					foreach (var mission in EnumHelper.GetValues<MissionType>()) {
						var values = row.Field<string>(mission.ToString().ToLower())
							.Split('/').Select(v => v.ToDouble() / 100.0).Concat(0.0.Repeat(3)).SI<Watt>().ToList();
						Data[Tuple.Create(mission, hdvClass)] = new SteeringPumpValues<Watt>(values[0], values[1], values[2]);
					}
				}
			}
		}

		private sealed class SteeringPumpTechnologies : LookupData<string, MissionType, SteeringPumpValues<double>>
		{
			private readonly ElectricSystem.Alternator _alternator = new ElectricSystem.Alternator();

			protected override string ResourceId
			{
				get { return "TUGraz.VectoCore.Resources.Declaration.VAUX.SP-Tech.csv"; }
			}

			protected override string ErrorMessage
			{
				get { return "Auxiliary Lookup Error: No value found for SteeringPump Technology. Key: '{0}'"; }
			}

			protected override void ParseData(DataTable table)
			{
				NormalizeTable(table);
				Data.Clear();

				Data = table.Rows.Cast<DataRow>().ToDictionary(
					key => Tuple.Create(key.Field<string>("Technology"), MissionType.LongHaul),
					value => new SteeringPumpValues<double>(value.ParseDouble("UF"), value.ParseDouble("B"), value.ParseDouble("S")));
			}

			public override SteeringPumpValues<double> Lookup(string tech, MissionType mission)
			{
				var values = base.Lookup(tech, MissionType.LongHaul);
				if (tech == "Electric") {
					values.Banking /= _alternator.Lookup(mission);
					values.Steering /= _alternator.Lookup(mission);
				}
				return values;
			}
		}

		private sealed class SteeringPumpAxles : LookupData<MissionType, int, SteeringPumpValues<double>>
		{
			protected override string ResourceId
			{
				get { return "TUGraz.VectoCore.Resources.Declaration.VAUX.SP-Axles.csv"; }
			}

			protected override string ErrorMessage
			{
				get { return "Auxiliary Lookup Error: No value found for SteeringPump Axle. Mission: '{0}', Axle Count: '{1}'"; }
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

		private class SteeringPumpValues<T>
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
}