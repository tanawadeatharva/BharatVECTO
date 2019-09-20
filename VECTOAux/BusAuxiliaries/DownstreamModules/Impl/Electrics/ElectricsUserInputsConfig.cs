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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using DownstreamModules.Electrics;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

namespace Electrics
{
	public class ElectricsUserInputsConfig : IElectricsUserInputsConfig
	{
		public double PowerNetVoltage { get; set; }
		public string AlternatorMap { get; set; }
		public double AlternatorGearEfficiency { get; set; }

		public IElectricalConsumerList ElectricalConsumers { get; set; }

		public int DoorActuationTimeSecond { get; set; }
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
			PowerNetVoltage = vectoInputs.PowerNetVoltage.Value();
			ResultCardIdle = new ResultCard(new List<SmartResult>());
			ResultCardOverrun = new ResultCard(new List<SmartResult>());
			ResultCardTraction = new ResultCard(new List<SmartResult>());
			SmartElectrical = false;
			AlternatorMap = string.Empty;
		}
	}
}
