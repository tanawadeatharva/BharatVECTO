using System;
using System.ComponentModel;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

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

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	/// <summary>
	/// 	''' Described a consumer of Alternator electrical power
	/// 	''' </summary>
	/// 	''' <remarks></remarks>
	public class ElectricalConsumer : IElectricalConsumer
	{
		// Fields
		private bool _BaseVehicle;

		private string _Category;
		private string _ConsumerName;
		private Ampere _NominalConsumptionAmps;
		private int _NumberInActualVehicle;
		private double _PhaseIdle_TractionOn;
		private Volt _PowerNetVoltage;
		private string _Info;

		// Constructor
		public ElectricalConsumer(
			bool baseVehicle, string category, string consumerName, Ampere nominalConsumptionAmps, double phaseIdleTractionOn,
			Volt powerNetVoltage, int numberInVehicle, string info)
		{
			// Illegal Value Check.
			if (category.Trim().Length == 0)
				throw new ArgumentException("Category Name cannot be empty");
			if (consumerName.Trim().Length == 0)
				throw new ArgumentException("ConsumerName Name cannot be empty");
			if (phaseIdleTractionOn < ElectricConstants.PhaseIdleTractionOnMin |
				phaseIdleTractionOn > ElectricConstants.PhaseIdleTractionMax)
				throw new ArgumentException("PhaseIdle_TractionOn must have a value between 0 and 1");
			if (nominalConsumptionAmps < ElectricConstants.NonminalConsumerConsumptionAmpsMin |
				nominalConsumptionAmps > ElectricConstants.NominalConsumptionAmpsMax)
				throw new ArgumentException("NominalConsumptionAmps must have a value between 0 and 100");
			if (powerNetVoltage < ElectricConstants.PowenetVoltageMin | powerNetVoltage > ElectricConstants.PowenetVoltageMax)
				throw new ArgumentException("PowerNetVoltage must have a value between 6 and 48");
			if (numberInVehicle < 0)
				throw new ArgumentException("Cannot have less than 0 consumers in the vehicle");

			// Good, now assign.
			BaseVehicle = baseVehicle;
			Category = category;
			ConsumerName = consumerName;
			NominalConsumptionAmps = nominalConsumptionAmps;
			PhaseIdle_TractionOn = phaseIdleTractionOn;
			PowerNetVoltage = powerNetVoltage;
			NumberInActualVehicle = numberInVehicle;
			Info = info;
			
		}

		
		// Properties
		public bool BaseVehicle
		{
			get { return _BaseVehicle; }
			set {
				_BaseVehicle = value;
				NotifyPropertyChanged("BaseVehicle");
			}
		}

		public string Category
		{
			get { return _Category; }
			set {
				_Category = value;
				NotifyPropertyChanged("Category");
			}
		}

		public string ConsumerName
		{
			get { return _ConsumerName; }
			set {
				_ConsumerName = value;
				NotifyPropertyChanged("ConsumerName");
			}
		}

		public Ampere NominalConsumptionAmps
		{
			get { return _NominalConsumptionAmps; }
			set {
				_NominalConsumptionAmps = value;
				NotifyPropertyChanged("NominalConsumptionAmps");
			}
		}

		public int NumberInActualVehicle
		{
			get { return _NumberInActualVehicle; }
			set {
				_NumberInActualVehicle = value;
				NotifyPropertyChanged("NumberInActualVehicle");
			}
		}

		public double PhaseIdle_TractionOn
		{
			get { return _PhaseIdle_TractionOn; }
			set {
				_PhaseIdle_TractionOn = value;
				NotifyPropertyChanged("PhaseIdle_TractionOn");
			}
		}

		public Volt PowerNetVoltage
		{
			get { return _PowerNetVoltage; }
			set {
				_PowerNetVoltage = value;
				NotifyPropertyChanged("PowerNetVoltage");
			}
		}

		public string Info
		{
			get { return _Info; }
			set {
				_Info = value;
				NotifyPropertyChanged("Info");
			}
		}


		// Public class outputs
		public Ampere TotalAvgConumptionAmps
		{
			get { return NominalConsumptionAmps * (NumberInActualVehicle * PhaseIdle_TractionOn); }
		}

		

		// Comparison Overrides
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType()) {
				return false;
			}

			var other = (IElectricalConsumer)obj;

			return ConsumerName == other.ConsumerName;
		}

		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		public override int GetHashCode()
		{
			return 0;
		}


		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(string p)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
		}
	}
}
