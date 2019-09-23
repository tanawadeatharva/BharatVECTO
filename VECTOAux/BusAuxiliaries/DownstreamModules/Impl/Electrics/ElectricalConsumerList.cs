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
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

namespace TUGraz.VectoCore.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class ElectricalConsumerList : IElectricalConsumerList
	{
		private List<IElectricalConsumer> _items = new List<IElectricalConsumer>();
		private double _powernetVoltage;
		private double _doorDutyCycleZeroToOne;


		// Constructor
		public ElectricalConsumerList(double powernetVoltage, double doorDutyCycle_ZeroToOne, bool createDefaultList = false)
		{
			_powernetVoltage = powernetVoltage;

			if (createDefaultList)
				_items = GetDefaultConsumerList();


			_doorDutyCycleZeroToOne = doorDutyCycle_ZeroToOne;
		}

		// Transfers the Info comments from a default set of consumables to a live set.
		// This way makes the comments not dependent on saved data.
		public void MergeInfoData()
		{
			if (_items.Count != GetDefaultConsumerList().Count)
				return;

			var dflt = GetDefaultConsumerList();

			for (var idx = 0; idx <= _items.Count - 1; idx++)

				_items[idx].Info = dflt[idx].Info;
		}

		// Initialise default set of consumers
		public List<IElectricalConsumer> GetDefaultConsumerList()
		{

			// This populates the default settings as per engineering spreadsheet.
			// Vehicle Basic Equipment' category can be added or remove by customers.
			// At some time in the future, this may be removed and replace with file based consumer lists.

			var items = new List<IElectricalConsumer>();

			IElectricalConsumer c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15, c16, c17, c18, c19, c20;

			c1 = (IElectricalConsumer)new ElectricalConsumer(false, "Doors", "Doors per Door", 3.0, 0.096339, _powernetVoltage, 3, "");
			c2 = (IElectricalConsumer)new ElectricalConsumer(true, "Veh Electronics &Engine", "Controllers,Valves etc", 25.0, 1.0, _powernetVoltage, 1, "");
			c3 = (IElectricalConsumer)new ElectricalConsumer(false, "Vehicle basic equipment", "Radio City", 2.0, 0.8, _powernetVoltage, 1, "");
			c4 = (IElectricalConsumer)new ElectricalConsumer(false, "Vehicle basic equipment", "Radio Intercity", 5.0, 0.8, _powernetVoltage, 0, "");
			c5 = (IElectricalConsumer)new ElectricalConsumer(false, "Vehicle basic equipment", "Radio/Audio Tourism", 9.0, 0.8, _powernetVoltage, 0, "");
			c6 = (IElectricalConsumer)new ElectricalConsumer(false, "Vehicle basic equipment", "Fridge", 4.0, 0.5, _powernetVoltage, 0, "");
			c7 = (IElectricalConsumer)new ElectricalConsumer(false, "Vehicle basic equipment", "Kitchen Standard", 67.0, 0.05, _powernetVoltage, 0, "");
			c8 = (IElectricalConsumer)new ElectricalConsumer(false, "Vehicle basic equipment", "Interior lights City/ Intercity + Doorlights [Should be 1/m]", 1.0, 0.7, _powernetVoltage, 12, "1 Per metre length of bus");
			c9 = (IElectricalConsumer)new ElectricalConsumer(false, "Vehicle basic equipment", "LED Interior lights ceiling city/Intercity + door [Should be 1/m]", 0.6, 0.7, _powernetVoltage, 0, "1 Per metre length of bus");
			c10 = (IElectricalConsumer)new ElectricalConsumer(false, "Vehicle basic equipment", "Interior lights Tourism + reading [1/m]", 1.1, 0.7, _powernetVoltage, 0, "1 Per metre length of bus");
			c11 = (IElectricalConsumer)new ElectricalConsumer(false, "Vehicle basic equipment", "LED Interior lights ceiling Tourism + LED reading [Should be 1/m]", 0.66, 0.7, _powernetVoltage, 0, "1 Per metre length of bus");
			c12 = (IElectricalConsumer)new ElectricalConsumer(false, "Customer Specific Equipment", "External Displays Font/Side/Rear", 2.65017667844523, 1.0, _powernetVoltage, 4, "");
			c13 = (IElectricalConsumer)new ElectricalConsumer(false, "Customer Specific Equipment", "Internal display per unit ( front side rear)", 1.06007067137809, 1.0, _powernetVoltage, 1, "");
			c14 = (IElectricalConsumer)new ElectricalConsumer(false, "Customer Specific Equipment", "CityBus Ref EBSF Table4 Devices ITS No Displays", 9.3, 1.0, _powernetVoltage, 1, "");
			c15 = (IElectricalConsumer)new ElectricalConsumer(false, "Lights", "Exterior Lights BULB", 7.4, 1.0, _powernetVoltage, 1, "");
			c16 = (IElectricalConsumer)new ElectricalConsumer(false, "Lights", "Day running lights LED bonus", -0.723, 1.0, _powernetVoltage, 1, "");
			c17 = (IElectricalConsumer)new ElectricalConsumer(false, "Lights", "Antifog rear lights LED bonus", -0.17, 1.0, _powernetVoltage, 1, "");
			c18 = (IElectricalConsumer)new ElectricalConsumer(false, "Lights", "Position lights LED bonus", -1.2, 1.0, _powernetVoltage, 1, "");
			c19 = (IElectricalConsumer)new ElectricalConsumer(false, "Lights", "Direction lights LED bonus", -0.3, 1.0, _powernetVoltage, 1, "");
			c20 = (IElectricalConsumer)new ElectricalConsumer(false, "Lights", "Brake Lights LED bonus", -1.2, 1.0, _powernetVoltage, 1, "");

			items.Add(c1);
			items.Add(c2);
			items.Add(c3);
			items.Add(c4);
			items.Add(c5);
			items.Add(c6);
			items.Add(c7);
			items.Add(c8);
			items.Add(c9);
			items.Add(c10);
			items.Add(c11);
			items.Add(c12);
			items.Add(c13);
			items.Add(c14);
			items.Add(c15);
			items.Add(c16);
			items.Add(c17);
			items.Add(c18);
			items.Add(c19);
			items.Add(c20);

			return items;
		}


		// Interface implementation
		public double DoorDutyCycleFraction
		{
			get {
				return _doorDutyCycleZeroToOne;
			}
			set {
				_doorDutyCycleZeroToOne = value;
			}
		}

		public List<IElectricalConsumer> Items
		{
			get {
				return _items;
			}
		}

		public void AddConsumer(IElectricalConsumer consumer)
		{
			if (!_items.Contains(consumer))
				_items.Add(consumer);
			else
				throw new ArgumentException("Consumer Already Present in the list");
		}

		public void RemoveConsumer(IElectricalConsumer consumer)
		{
			if (_items.Contains(consumer))
				_items.Remove(consumer);
			else
				throw new ArgumentException("Consumer Not In List");
		}


		public Ampere GetTotalAverageDemandAmps(bool excludeOnBase)
		{
			Ampere Amps;

			if (excludeOnBase)

				Amps = Items.Where(x => x.BaseVehicle == false)
							.Sum(consumer => consumer.TotalAvgConumptionAmps(DoorDutyCycleFraction));

			// Aggregate item In Items Where item.BaseVehicle = False Into Sum(item.TotalAvgConumptionAmps(DoorDutyCycleFraction))


			else

				Amps = Items.Sum(x => x.TotalAvgConumptionAmps(DoorDutyCycleFraction));
					//Aggregate item In Items Into Sum(item.TotalAvgConumptionAmps(DoorDutyCycleFraction))

 
			return Amps;
		}
	}
}
