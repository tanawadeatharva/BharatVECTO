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
using Newtonsoft.Json;
using TUGraz.VectoCommon.BusAuxiliaries;

namespace TUGraz.VectoCore.Models.BusAuxiliaries {
	[Serializable()]
	public class AuxiliaryConfig : IAuxiliaryConfig
	{
		
		// Electrical
		public IElectricsUserInputsConfig ElectricalUserInputsConfig { get; internal set; }

		// Pneumatics
		public IPneumaticUserInputsConfig PneumaticUserInputsConfig { get; internal set; }

		public IPneumaticsAuxilliariesConfig PneumaticAuxillariesConfig { get; internal set; }

		public ISSMInputs SSMInputs { get; internal set; }

		public IActuationsMap ActuationsMap { get; internal set; }

		public string Cycle { get; internal set; }

		public IVehicleData VehicleData { get; internal set; }
	
		public IFuelConsumptionMap FuelMap { get; internal set; }

		// Vecto Signals
		public ISignals Signals { get; internal set; }

		// Constructors
		
		
		private bool CompareElectricalConfiguration(IAuxiliaryConfig other)
		{
			// AlternatorGearEfficiency
			if (ElectricalUserInputsConfig.AlternatorGearEfficiency != other.ElectricalUserInputsConfig.AlternatorGearEfficiency) {
				return false;
			}

			// AlternatorMap
			if (!ElectricalUserInputsConfig.AlternatorMap.Equals(other.ElectricalUserInputsConfig.AlternatorMap)) {
				return false;
			}

			// DoorActuationTimeSecond
			if (ElectricalUserInputsConfig.DoorActuationTimeSecond != other.ElectricalUserInputsConfig.DoorActuationTimeSecond) {
				return false;
			}

			// Consumer list
			if (ElectricalUserInputsConfig.ElectricalConsumers.Items.Count !=
				other.ElectricalUserInputsConfig.ElectricalConsumers.Items.Count) {
				return false;
			}

			int i;
			for (i = 0; i < ElectricalUserInputsConfig.ElectricalConsumers.Items.Count; i++) {
				IElectricalConsumer thisConsumer, otherConsumer;
				thisConsumer = ElectricalUserInputsConfig.ElectricalConsumers.Items[i];
				otherConsumer = other.ElectricalUserInputsConfig.ElectricalConsumers.Items[i];

				if (
					//thisConsumer.AvgConsumptionAmps != otherConsumer.AvgConsumptionAmps ||
					thisConsumer.BaseVehicle != otherConsumer.BaseVehicle || thisConsumer.Category != otherConsumer.Category ||
					thisConsumer.ConsumerName != otherConsumer.ConsumerName ||
					thisConsumer.NominalConsumptionAmps != otherConsumer.NominalConsumptionAmps ||
					thisConsumer.NumberInActualVehicle != otherConsumer.NumberInActualVehicle ||
					thisConsumer.PhaseIdle_TractionOn != otherConsumer.PhaseIdle_TractionOn) 
				{
					return false;
				}
			}

			// PowerNetVoltage
			if (ElectricalUserInputsConfig.PowerNetVoltage != other.ElectricalUserInputsConfig.PowerNetVoltage) {
				return false;
			}

			// ResultCardIdle
			if (!ElectricalUserInputsConfig.ResultCardIdle.Equals(other.ElectricalUserInputsConfig.ResultCardIdle)) {
				return false;
			}

			// ResultCardOverrun
			if (!ElectricalUserInputsConfig.ResultCardOverrun.Equals(other.ElectricalUserInputsConfig.ResultCardOverrun)) {
				return false;
			}

			// ResultCardTraction
			if (!ElectricalUserInputsConfig.ResultCardTraction.Equals(other.ElectricalUserInputsConfig.ResultCardTraction)) {
				return false;
			}

			// SmartElectrical
			if (ElectricalUserInputsConfig.SmartElectrical != other.ElectricalUserInputsConfig.SmartElectrical) {
				return false;
			}

			return true;
		}

		private bool ComparePneumaticAuxiliariesConfig(IAuxiliaryConfig other)
		{
			if (PneumaticAuxillariesConfig.AdBlueNIperMinute != other.PneumaticAuxillariesConfig.AdBlueNIperMinute) {
				return false;
			}
			if (PneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute !=
				other.PneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute) {
				return false;
			}
			if (PneumaticAuxillariesConfig.BrakingNoRetarderNIperKG != other.PneumaticAuxillariesConfig.BrakingNoRetarderNIperKG) {
				return false;
			}
			if (PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG !=
				other.PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG) {
				return false;
			}
			if (PneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM !=
				other.PneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM) {
				return false;
			}
			if (PneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour !=
				other.PneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour) {
				return false;
			}
			if (PneumaticAuxillariesConfig.DeadVolumeLitres != other.PneumaticAuxillariesConfig.DeadVolumeLitres) {
				return false;
			}
			if (PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand !=
				other.PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand) {
				return false;
			}
			if (PneumaticAuxillariesConfig.PerDoorOpeningNI != other.PneumaticAuxillariesConfig.PerDoorOpeningNI) {
				return false;
			}
			if (PneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG !=
				other.PneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG) {
				return false;
			}
			if (PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand !=
				other.PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand) {
				return false;
			}
			if (PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction !=
				other.PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction) {
				return false;
			}

			return true;
		}

		private bool ComparePneumaticUserConfig(IAuxiliaryConfig other)
		{
			//if (PneumaticUserInputsConfig.ActuationsMap != other.PneumaticUserInputsConfig.ActuationsMap)
			//	return false;
			if (PneumaticUserInputsConfig.AdBlueDosing != other.PneumaticUserInputsConfig.AdBlueDosing)
				return false;
			if (PneumaticUserInputsConfig.AirSuspensionControl != other.PneumaticUserInputsConfig.AirSuspensionControl)
				return false;
			if (PneumaticUserInputsConfig.CompressorGearEfficiency != other.PneumaticUserInputsConfig.CompressorGearEfficiency)
				return false;
			if (PneumaticUserInputsConfig.CompressorGearRatio != other.PneumaticUserInputsConfig.CompressorGearRatio)
				return false;
			if (PneumaticUserInputsConfig.CompressorMap != other.PneumaticUserInputsConfig.CompressorMap)
				return false;
			if (PneumaticUserInputsConfig.Doors != other.PneumaticUserInputsConfig.Doors)
				return false;
			if (PneumaticUserInputsConfig.KneelingHeightMillimeters != other.PneumaticUserInputsConfig.KneelingHeightMillimeters)
				return false;
			if (PneumaticUserInputsConfig.RetarderBrake != other.PneumaticUserInputsConfig.RetarderBrake)
				return false;
			if (PneumaticUserInputsConfig.SmartAirCompression != other.PneumaticUserInputsConfig.SmartAirCompression)
				return false;
			if (PneumaticUserInputsConfig.SmartRegeneration != other.PneumaticUserInputsConfig.SmartRegeneration)
				return false;

			return true;
		}


		public bool ConfigValuesAreTheSameAs(IAuxiliaryConfig other)
		{
			if (!CompareElectricalConfiguration(other))
				return false;
			if (!ComparePneumaticAuxiliariesConfig(other))
				return false;
			if (!ComparePneumaticUserConfig(other))
				return false;
			//if (!CompareHVACConfig(other))
			//	return false;

			return true;
		}


		public override bool Equals(object other)
		{
			var myOhter = other as AuxiliaryConfig;
			if (myOhter == null) {
				return false;
			}

			return ConfigValuesAreTheSameAs(myOhter);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

	}
}
