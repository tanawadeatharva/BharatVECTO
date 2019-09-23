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


namespace TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.Electrics
{
	public interface IElectricsUserInputsConfig
	{
		/// <summary>
		/// 	Power Net Voltage - The supply voltage used on the vehilce.
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		double PowerNetVoltage { get; set; }

		/// <summary>
		/// 	The Path for the Alternator map
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		string AlternatorMap { get; set; }

		/// <summary>
		/// 	Alternator Gear Efficiency
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		double AlternatorGearEfficiency { get; set; }

		/// <summary>
		/// 	List of Electrical Consumers
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		IElectricalConsumerList ElectricalConsumers { get; set; }

		/// <summary>
		/// 	Door Actuation Time In Seconds ( Time Taken to Open/Close the door )
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		int DoorActuationTimeSecond { get; set; }

		/// <summary>
		/// 	Result Card Taken During Idle.
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		IResultCard ResultCardIdle { get; set; }

		/// <summary>
		/// 	Result Card Taken During Traction
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		IResultCard ResultCardTraction { get; set; }

		/// <summary>
		/// 	Result Card Taken During Overrun
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		IResultCard ResultCardOverrun { get; set; }

		/// <summary>
		/// 	Smart Electrical System
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns>True For Smart Electrical Systems/ False For non Smart.</returns>
		/// 	<remarks></remarks>
		bool SmartElectrical { get; set; }

		/// <summary>
		/// 	Stored Energy Efficiency
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns>Stored Energy Efficiency</returns>
		/// 	<remarks></remarks>
		double StoredEnergyEfficiency { get; set; }
	}
}
