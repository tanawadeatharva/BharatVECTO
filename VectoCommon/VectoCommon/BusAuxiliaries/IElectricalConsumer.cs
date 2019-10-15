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

using System.ComponentModel;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public interface IElectricalConsumer : INotifyPropertyChanged
	{
		string Category { get; set; }
		string ConsumerName { get; set; }
		bool BaseVehicle { get; set; }
		Ampere NominalConsumptionAmps { get; set; }
		double PhaseIdle_TractionOn { get; set; }
		int NumberInActualVehicle { get; set; }
		Volt PowerNetVoltage { get; set; }
		Ampere AvgConsumptionAmps { get; set; }
		string Info { get; set; }
		Ampere TotalAvgConumptionAmps(double PhaseIdle_TractionOnBasedOnCycle = 0.0);
		Watt TotalAvgConsumptionInWatts(double PhaseIdle_TractionOnBasedOnCycle = 0.0);
	}
}
