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
using System.ComponentModel;
using DownstreamModules.Electrics;
using TUGraz.VectoCommon.Utils;

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

namespace Electrics
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
		private double _NominalConsumptionAmps;
		private int _NumberInActualVehicle;
		private double _PhaseIdle_TractionOn;
		private double _PowerNetVoltage;
		private string _Info;

		// Calculated
		public double AvgConsumptionAmps { get; set; }

		// Properties
		public bool BaseVehicle
		{
			get {
				return _BaseVehicle;
			}
			set {
				_BaseVehicle = value;
				NotifyPropertyChanged("BaseVehicle");
			}
		}

		public string Category
		{
			get {
				return _Category;
			}
			set {
				_Category = value;
				NotifyPropertyChanged("Category");
			}
		}

		public string ConsumerName
		{
			get {
				return _ConsumerName;
			}
			set {
				_ConsumerName = value;
				NotifyPropertyChanged("ConsumerName");
			}
		}

		public double NominalConsumptionAmps
		{
			get {
				return _NominalConsumptionAmps;
			}
			set {
				_NominalConsumptionAmps = value;
				NotifyPropertyChanged("NominalConsumptionAmps");
			}
		}

		public int NumberInActualVehicle
		{
			get {
				return _NumberInActualVehicle;
			}
			set {
				_NumberInActualVehicle = value;
				NotifyPropertyChanged("NumberInActualVehicle");
			}
		}

		public double PhaseIdle_TractionOn
		{
			get {
				return _PhaseIdle_TractionOn;
			}
			set {
				_PhaseIdle_TractionOn = value;
				NotifyPropertyChanged("PhaseIdle_TractionOn");
			}
		}

		public double PowerNetVoltage
		{
			get {
				return _PowerNetVoltage;
			}
			set {
				_PowerNetVoltage = value;
				NotifyPropertyChanged("PowerNetVoltage");
			}
		}

		public string Info
		{
			get {
				return _Info;
			}
			set {
				_Info = value;
				NotifyPropertyChanged("Info");
			}
		}


		// Public class outputs
		public Ampere TotalAvgConumptionAmps(double PhaseIdle_TractionOnBasedOnCycle = default(Double))
		{
			if (ConsumerName == "Doors per Door")
				return NominalConsumptionAmps.SI<Ampere>() * (NumberInActualVehicle * PhaseIdle_TractionOnBasedOnCycle);
			else
				return NominalConsumptionAmps.SI<Ampere>() * (NumberInActualVehicle * PhaseIdle_TractionOn);
		}

		public Watt TotalAvgConsumptionInWatts(double PhaseIdle_TractionOnBasedOnCycle = 0.0)
		{
			return TotalAvgConumptionAmps(PhaseIdle_TractionOnBasedOnCycle) * PowerNetVoltage.SI<Volt>();
		}

		// Constructor
		public ElectricalConsumer(bool BaseVehicle, string Category, string ConsumerName, double NominalConsumptionAmps, double PhaseIdle_TractionOn, double PowerNetVoltage, int numberInVehicle, string info)
		{

			// Illegal Value Check.
			if (Category.Trim().Length == 0)
				throw new ArgumentException("Category Name cannot be empty");
			if (ConsumerName.Trim().Length == 0)
				throw new ArgumentException("ConsumerName Name cannot be empty");
			if (PhaseIdle_TractionOn < ElectricConstants.PhaseIdleTractionOnMin | PhaseIdle_TractionOn > ElectricConstants.PhaseIdleTractionMax)
				throw new ArgumentException("PhaseIdle_TractionOn must have a value between 0 and 1");
			if (NominalConsumptionAmps < ElectricConstants.NonminalConsumerConsumptionAmpsMin | NominalConsumptionAmps > ElectricConstants.NominalConsumptionAmpsMax)
				throw new ArgumentException("NominalConsumptionAmps must have a value between 0 and 100");
			if (PowerNetVoltage < ElectricConstants.PowenetVoltageMin | PowerNetVoltage > ElectricConstants.PowenetVoltageMax)
				throw new ArgumentException("PowerNetVoltage must have a value between 6 and 48");
			if (numberInVehicle < 0)
				throw new ArgumentException("Cannot have less than 0 consumers in the vehicle");

			// Good, now assign.
			this.BaseVehicle = BaseVehicle;
			this.Category = Category;
			this.ConsumerName = ConsumerName;
			this.NominalConsumptionAmps = NominalConsumptionAmps;
			this.PhaseIdle_TractionOn = PhaseIdle_TractionOn;
			this.PowerNetVoltage = PowerNetVoltage;
			this.NumberInActualVehicle = numberInVehicle;
			this.Info = info;
		}

		// Comparison Overrides
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;

			IElectricalConsumer other = (IElectricalConsumer)obj;


			return this.ConsumerName == other.ConsumerName;
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
