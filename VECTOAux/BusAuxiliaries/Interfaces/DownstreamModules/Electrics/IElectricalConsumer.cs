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
using System.ComponentModel;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.Electrics
{
	public interface IElectricalConsumer : INotifyPropertyChanged
	{
		string Category { get; set; }
		string ConsumerName { get; set; }
		bool BaseVehicle { get; set; }
		double NominalConsumptionAmps { get; set; }
		double PhaseIdle_TractionOn { get; set; }
		int NumberInActualVehicle { get; set; }
		double PowerNetVoltage { get; set; }
		double AvgConsumptionAmps { get; set; }
		string Info { get; set; }
		Ampere TotalAvgConumptionAmps(double PhaseIdle_TractionOnBasedOnCycle = default(Double));
		Watt TotalAvgConsumptionInWatts(double PhaseIdle_TractionOnBasedOnCycle = 0.0);
	}
}
