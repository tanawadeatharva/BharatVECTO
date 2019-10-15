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
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public interface IElectricalConsumerList
	{
		/// <summary>
		/// 	List of Electrical Consumers
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		List<IElectricalConsumer> Items { get; }

		/// <summary>
		/// 	Add New Electrical Consumer
		/// 	</summary>
		/// 	<param name="consumer"></param>
		/// 	<remarks></remarks>
		void AddConsumer(IElectricalConsumer consumer);

		/// <summary>
		/// 	Remove Electrical Consumer
		/// 	</summary>
		/// 	<param name="consumer"></param>
		/// 	<remarks></remarks>
		void RemoveConsumer(IElectricalConsumer consumer);

		/// <summary>
		/// 	Get Total Average Demand In Amps 
		/// 	</summary>
		/// 	<param name="excludeOnBase">Exclude those on base vehicle</param>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Ampere GetTotalAverageDemandAmps(bool excludeOnBase);

		/// <summary>
		/// 	Door Actuation Time Fraction ( Total Time Spent Operational during cycle )
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		double DoorDutyCycleFraction { get; set; }

		// Merge Info data from ElectricalConsumer in a Default set into live set
		// This is required because the info is stored in the AAUX file and we do not want to use a persistance stored version.
		void MergeInfoData();
	}
}
