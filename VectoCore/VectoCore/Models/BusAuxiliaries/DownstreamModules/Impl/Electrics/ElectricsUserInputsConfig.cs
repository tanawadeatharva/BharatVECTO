// Copyright 2017 European Union.
// Licensed under the EUPL (the 'Licence');
// 
// * You may not use this work except in compliance with the Licence.
// * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
// * Unless required by applicable law or agreed to in writing,
// software distributed under the Licence is distributed on an "AS IS" basis,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 
// See the LICENSE.txt for the specific language governing permissions and limitations.

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class ElectricsUserInputsConfig : IElectricsUserInputsConfig
	{
		public Volt PowerNetVoltage { get; set; }
		public IAlternatorMap AlternatorMap { get; set; }
		public double AlternatorGearEfficiency { get; set; }

		public Second DoorActuationTimeSecond { get; set; }
		public double StoredEnergyEfficiency { get; set; }

		public IResultCard ResultCardIdle { get; set; }
		public IResultCard ResultCardTraction { get; set; }
		public IResultCard ResultCardOverrun { get; set; }

		public bool SmartElectrical { get; set; }

		[JsonIgnore]
		public Dictionary<string, Tuple<bool, Ampere>> ElectricalConsumers { get; set; }

		public string[] ElectricalConsumersSerialized
		{
			get { return ElectricalConsumers.Select(x => $"{x.Key}: {x.Value.Item2}  ({x.Value.Item1})").ToArray(); }
		}

		public Ampere AverageCurrentDemandInclBaseLoad { get { return ElectricalConsumers.Select(x => x.Value.Item2).Sum().Cast<Ampere>(); }  }

		public Ampere AverageCurrentDemandWithoutBaseLoad { get {
			return ElectricalConsumers.Where(x => !x.Value.Item1).Select(x => x.Value.Item2).Sum().Cast<Ampere>();
		}  }

		public Watt MaxAlternatorPower { get; set; }
		public WattSecond ElectricStorageCapacity { get; set; }
	}

}
