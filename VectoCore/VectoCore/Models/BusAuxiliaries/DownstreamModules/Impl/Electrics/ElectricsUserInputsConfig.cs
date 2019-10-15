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

using System.Collections.Generic;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class ElectricsUserInputsConfig : IElectricsUserInputsConfig
	{
		public Volt PowerNetVoltage { get; set; }
		public string AlternatorMap { get; set; }
		public double AlternatorGearEfficiency { get; set; }

		public IElectricalConsumerList ElectricalConsumers { get; set; }

		public double DoorActuationTimeSecond { get; set; }
		public double StoredEnergyEfficiency { get; set; }

		public IResultCard ResultCardIdle { get; set; }
		public IResultCard ResultCardTraction { get; set; }
		public IResultCard ResultCardOverrun { get; set; }

		public bool SmartElectrical { get; set; }

		public ElectricsUserInputsConfig(bool setToDefaults = false, VectoInputs vectoInputs = null/* TODO Change to default(_) if this is not a reference type */)
		{
			if (setToDefaults)
				SetPropertiesToDefaults(vectoInputs);
		}

		public void SetPropertiesToDefaults(VectoInputs vectoInputs)
		{
			DoorActuationTimeSecond = 4;
			StoredEnergyEfficiency = 0.935;
			AlternatorGearEfficiency = 0.92;
			PowerNetVoltage = vectoInputs.PowerNetVoltage;
			ResultCardIdle = new ResultCard(new List<SmartResult>());
			ResultCardOverrun = new ResultCard(new List<SmartResult>());
			ResultCardTraction = new ResultCard(new List<SmartResult>());
			SmartElectrical = false;
			AlternatorMap = string.Empty;
		}
	}
}
