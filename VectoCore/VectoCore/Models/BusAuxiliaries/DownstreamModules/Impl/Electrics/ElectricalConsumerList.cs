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
using System.Collections.ObjectModel;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class ElectricalConsumerList : IElectricalConsumerList
	{
		private readonly List<IElectricalConsumer> _items;
		//private Volt _powernetVoltage;
		
		// Constructor
		public ElectricalConsumerList(List<IElectricalConsumer> consumer)
		{
			_items = consumer;
		}

		//public ElectricalConsumerList(Volt powernetVoltage, double doorDutyCycle_ZeroToOne)
		//{
		//	//_powernetVoltage = powernetVoltage;

		//	//if (createDefaultList)
		//	//	_items = GetDefaultConsumerList();


		//	DoorDutyCycleFraction = doorDutyCycle_ZeroToOne;
		//}

		// Transfers the Info comments from a default set of consumables to a live set.
		// This way makes the comments not dependent on saved data.
		//public void MergeInfoData()
		//{
		//	if (_items.Count != GetDefaultConsumerList().Count)
		//		return;

		//	var dflt = GetDefaultConsumerList();

		//	for (var idx = 0; idx <= _items.Count - 1; idx++)

		//		_items[idx].Info = dflt[idx].Info;
		//}

		// Initialise default set of consumers
		//public List<IElectricalConsumer> GetDefaultConsumerList()
		//{

		//	// This populates the default settings as per engineering spreadsheet.
		//	// Vehicle Basic Equipment' category can be added or remove by customers.
		//	// At some time in the future, this may be removed and replace with file based consumer lists.

		//	var items = new List<IElectricalConsumer> {
		//		new ElectricalConsumer(false, "Doors", "Doors per vehicle", 3.0.SI<Ampere>(), 0.096339, _powernetVoltage, 3, ""),
		//		new ElectricalConsumer(
		//			true, "Veh Electronics &Engine", "Controllers,Valves etc", 25.0.SI<Ampere>(), 1.0, _powernetVoltage, 1, ""),
		//		new ElectricalConsumer(
		//			false, "Vehicle basic equipment", "Radio City", 2.0.SI<Ampere>(), 0.8, _powernetVoltage, 1, ""),
		//		new ElectricalConsumer(
		//			false, "Vehicle basic equipment", "Radio Intercity", 5.0.SI<Ampere>(), 0.8, _powernetVoltage, 0, ""),
		//		new ElectricalConsumer(
		//			false, "Vehicle basic equipment", "Radio/Audio Tourism", 9.0.SI<Ampere>(), 0.8, _powernetVoltage, 0, ""),
		//		new ElectricalConsumer(false, "Vehicle basic equipment", "Fridge", 4.0.SI<Ampere>(), 0.5, _powernetVoltage, 0, ""),
		//		new ElectricalConsumer(
		//			false, "Vehicle basic equipment", "Kitchen Standard", 67.0.SI<Ampere>(), 0.05, _powernetVoltage, 0, ""),
		//		new ElectricalConsumer(
		//			false, "Vehicle basic equipment", "Interior lights City/ Intercity + Doorlights [Should be 1/m]", 1.0.SI<Ampere>(),
		//			0.7, _powernetVoltage, 12, "1 Per metre length of bus"),
		//		new ElectricalConsumer(
		//			false, "Vehicle basic equipment", "LED Interior lights ceiling city/Intercity + door [Should be 1/m]",
		//			0.6.SI<Ampere>(), 0.7, _powernetVoltage, 0, "1 Per metre length of bus"),
		//		new ElectricalConsumer(
		//			false, "Vehicle basic equipment", "Interior lights Tourism + reading [1/m]", 1.1.SI<Ampere>(), 0.7,
		//			_powernetVoltage, 0, "1 Per metre length of bus"),
		//		new ElectricalConsumer(
		//			false, "Vehicle basic equipment", "LED Interior lights ceiling Tourism + LED reading [Should be 1/m]",
		//			0.66.SI<Ampere>(), 0.7, _powernetVoltage, 0, "1 Per metre length of bus"),
		//		new ElectricalConsumer(
		//			false, "Customer Specific Equipment", "External Displays Font/Side/Rear", 2.65017667844523.SI<Ampere>(), 1.0,
		//			_powernetVoltage, 4, ""),
		//		new ElectricalConsumer(
		//			false, "Customer Specific Equipment", "Internal display per unit ( front side rear)",
		//			1.06007067137809.SI<Ampere>(), 1.0, _powernetVoltage, 1, ""),
		//		new ElectricalConsumer(
		//			false, "Customer Specific Equipment", "CityBus Ref EBSF Table4 Devices ITS No Displays", 9.3.SI<Ampere>(), 1.0,
		//			_powernetVoltage, 1, ""),
		//		new ElectricalConsumer(false, "Lights", "Exterior Lights BULB", 7.4.SI<Ampere>(), 1.0, _powernetVoltage, 1, ""),
		//		new ElectricalConsumer(
		//			false, "Lights", "Day running lights LED bonus", -0.723.SI<Ampere>(), 1.0, _powernetVoltage, 1, ""),
		//		new ElectricalConsumer(
		//			false, "Lights", "Antifog rear lights LED bonus", -0.17.SI<Ampere>(), 1.0, _powernetVoltage, 1, ""),
		//		new ElectricalConsumer(
		//			false, "Lights", "Position lights LED bonus", -1.2.SI<Ampere>(), 1.0, _powernetVoltage, 1, ""),
		//		new ElectricalConsumer(
		//			false, "Lights", "Direction lights LED bonus", -0.3.SI<Ampere>(), 1.0, _powernetVoltage, 1, ""),
		//		new ElectricalConsumer(false, "Lights", "Brake Lights LED bonus", -1.2.SI<Ampere>(), 1.0, _powernetVoltage, 1, "")
		//	};


		//	return items;
		//}


		// Interface implementation
		
		public IReadOnlyList<IElectricalConsumer> Items
		{
			get {
				return _items;
			}
		}

		//public void AddConsumer(IElectricalConsumer consumer)
		//{
		//	if (!_items.Contains(consumer))
		//		_items.Add(consumer);
		//	else
		//		throw new ArgumentException("Consumer Already Present in the list");
		//}

		//public void RemoveConsumer(IElectricalConsumer consumer)
		//{
		//	if (_items.Contains(consumer))
		//		_items.Remove(consumer);
		//	else
		//		throw new ArgumentException("Consumer Not In List");
		//}


		//public Ampere GetTotalAverageDemandAmps(bool excludeOnBase)
		//{
		//	return excludeOnBase
		//		? Items.Where(x => x.BaseVehicle == false)
		//				.Sum(consumer => consumer.TotalAvgConumptionAmps(DoorDutyCycleFraction))
		//		: Items.Sum(x => x.TotalAvgConumptionAmps(DoorDutyCycleFraction));

		//	//Aggregate item In Items Into Sum(item.TotalAvgConumptionAmps(DoorDutyCycleFraction))
		//}
	}
}
