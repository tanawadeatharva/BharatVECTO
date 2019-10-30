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
using System.Deployment.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;

namespace TUGraz.VectoCore.Models.BusAuxiliaries {
	[Serializable()]
	public class AuxiliaryConfig : IAuxiliaryConfig
	{
		
		// Electrical
		public IElectricsUserInputsConfig ElectricalUserInputsConfig { get; internal set; }

		// Pneumatics
		public IPneumaticUserInputsConfig PneumaticUserInputsConfig { get; internal set; }

		public IPneumaticsAuxilliariesConfig PneumaticAuxillariesConfig { get; internal set; }

		// Hvac
		public IHVACUserInputsConfig HvacUserInputsConfig { get; internal set; }

		public ISSMInputs SSMInputs { get; internal set; }

		public IPneumaticActuationsMap ActuationsMap { get; internal set; }

		public string Cycle { get; internal set; }

		public IVehicleData VehicleData { get; internal set; }
	
		public IFuelConsumptionMap FuelMap { get; internal set; }

		// Vecto Signals
		[JsonIgnore]
		public ISignals Signals { get; internal set; }

		// Constructors
		//public AuxiliaryConfig() /*: this("EMPTY")*/ { }

		
		
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
					thisConsumer.PhaseIdle_TractionOn != otherConsumer.PhaseIdle_TractionOn) // ||
					//thisConsumer.TotalAvgConumptionAmps() != otherConsumer.TotalAvgConumptionAmps())
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
			//if (ElectricalUserInputsConfig.ResultCardIdle.Results.Count !=
			//	other.ElectricalUserInputsConfig.ResultCardIdle.Results.Count) {
			//	return false;
			//}

			//for (i = 0; i <= ElectricalUserInputsConfig.ResultCardIdle.Results.Count - 1; i++) {
			//	if (ElectricalUserInputsConfig.ResultCardIdle.Results[i].Amps !=
			//		other.ElectricalUserInputsConfig.ResultCardIdle.Results[i].Amps ||
			//		ElectricalUserInputsConfig.ResultCardIdle.Results[i].SmartAmps !=
			//		other.ElectricalUserInputsConfig.ResultCardIdle.Results[i].SmartAmps) {
			//		return false;
			//	}
			//}

			// ResultCardOverrun
			if (!ElectricalUserInputsConfig.ResultCardOverrun.Equals(other.ElectricalUserInputsConfig.ResultCardOverrun)) {
				return false;
			}
			//if (ElectricalUserInputsConfig.ResultCardOverrun.Results.Count !=
			//	other.ElectricalUserInputsConfig.ResultCardOverrun.Results.Count) {
			//	return false;
			//}

			//for (i = 0; i <= ElectricalUserInputsConfig.ResultCardOverrun.Results.Count - 1; i++) {
			//	if (ElectricalUserInputsConfig.ResultCardOverrun.Results[i].Amps !=
			//		other.ElectricalUserInputsConfig.ResultCardOverrun.Results[i].Amps ||
			//		ElectricalUserInputsConfig.ResultCardOverrun.Results[i].SmartAmps !=
			//		other.ElectricalUserInputsConfig.ResultCardOverrun.Results[i].SmartAmps) {
			//		return false;
			//	}
			//}

			// ResultCardTraction
			if (!ElectricalUserInputsConfig.ResultCardTraction.Equals(other.ElectricalUserInputsConfig.ResultCardTraction)) {
				return false;
			}
			//if (ElectricalUserInputsConfig.ResultCardTraction.Results.Count !=
			//	other.ElectricalUserInputsConfig.ResultCardTraction.Results.Count)
			//	return false;

			//for (i = 0; i <= ElectricalUserInputsConfig.ResultCardTraction.Results.Count - 1; i++) {
			//	if (ElectricalUserInputsConfig.ResultCardTraction.Results[i].Amps !=
			//		other.ElectricalUserInputsConfig.ResultCardTraction.Results[i].Amps ||
			//		ElectricalUserInputsConfig.ResultCardTraction.Results[i].SmartAmps !=
			//		other.ElectricalUserInputsConfig.ResultCardTraction.Results[i].SmartAmps) {
			//		return false;
			//	}
			//}

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

		//private bool CompareHVACConfig(IAuxiliaryConfig other)
		//{
		//	if (HvacUserInputsConfig.SSMFilePath != other.HvacUserInputsConfig.SSMFilePath)
		//		return false;
		//	if (HvacUserInputsConfig.BusDatabasePath != other.HvacUserInputsConfig.BusDatabasePath)
		//		return false;
		//	if (HvacUserInputsConfig.SSMDisabled != other.HvacUserInputsConfig.SSMDisabled)
		//		return false;

		//	return true;
		//}

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

		// Persistance Functions




		// Persistance Helpers
		//public void AssumeValuesOfOther(AuxiliaryConfig other)
		//{
		//	CloneElectricaConfiguration(other);
		//	ClonePneumaticsAuxiliariesConfig(other);
		//	ClonePneumaticsUserInputsConfig(other);
		//	CloneHVAC(other);
		//}

		//private void CloneElectricaConfiguration(AuxiliaryConfig other)
		//{
		//	// AlternatorGearEfficiency
		//	ElectricalUserInputsConfig.AlternatorGearEfficiency = other.ElectricalUserInputsConfig.AlternatorGearEfficiency;

		//	// AlternatorMap
		//	ElectricalUserInputsConfig.AlternatorMap = other.ElectricalUserInputsConfig.AlternatorMap;

		//	// DoorActuationTimeSecond
		//	ElectricalUserInputsConfig.DoorActuationTimeSecond = other.ElectricalUserInputsConfig.DoorActuationTimeSecond;

		//	// Electrical Consumer list
		//	ElectricalUserInputsConfig.ElectricalConsumers.Items.Clear();
		//	foreach (var otherConsumer in other.ElectricalUserInputsConfig.ElectricalConsumers.Items) {
		//		var newConsumer = new ElectricalConsumer(
		//			otherConsumer.BaseVehicle, otherConsumer.Category, otherConsumer.ConsumerName, otherConsumer.NominalConsumptionAmps,
		//			otherConsumer.PhaseIdle_TractionOn, otherConsumer.PowerNetVoltage, otherConsumer.NumberInActualVehicle,
		//			otherConsumer.Info);

		//		ElectricalUserInputsConfig.ElectricalConsumers.Items.Add(newConsumer);
		//	}

		//	// PowerNetVoltage
		//	ElectricalUserInputsConfig.PowerNetVoltage = other.ElectricalUserInputsConfig.PowerNetVoltage;

		//	// ResultCardIdle
		//	ElectricalUserInputsConfig.ResultCardIdle.Results.Clear();
		//	foreach (var result in other.ElectricalUserInputsConfig.ResultCardIdle.Results)
		//		ElectricalUserInputsConfig.ResultCardIdle.Results.Add(new SmartResult(result.Amps, result.SmartAmps));

		//	// ResultCardOverrun
		//	ElectricalUserInputsConfig.ResultCardOverrun.Results.Clear();
		//	foreach (var result in other.ElectricalUserInputsConfig.ResultCardOverrun.Results)
		//		ElectricalUserInputsConfig.ResultCardOverrun.Results.Add(new SmartResult(result.Amps, result.SmartAmps));

		//	// ResultCardTraction
		//	ElectricalUserInputsConfig.ResultCardTraction.Results.Clear();
		//	foreach (var result in other.ElectricalUserInputsConfig.ResultCardTraction.Results)
		//		ElectricalUserInputsConfig.ResultCardTraction.Results.Add(new SmartResult(result.Amps, result.SmartAmps));

		//	// SmartElectrical
		//	ElectricalUserInputsConfig.SmartElectrical = other.ElectricalUserInputsConfig.SmartElectrical;
		//}

		//private void ClonePneumaticsAuxiliariesConfig(AuxiliaryConfig other)
		//{
		//	PneumaticAuxillariesConfig.AdBlueNIperMinute = other.PneumaticAuxillariesConfig.AdBlueNIperMinute;
		//	PneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute =
		//		other.PneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute;
		//	PneumaticAuxillariesConfig.BrakingNoRetarderNIperKG = other.PneumaticAuxillariesConfig.BrakingNoRetarderNIperKG;
		//	PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG = other.PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG;
		//	PneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM =
		//		other.PneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM;
		//	PneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour =
		//		other.PneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour;
		//	PneumaticAuxillariesConfig.DeadVolumeLitres = other.PneumaticAuxillariesConfig.DeadVolumeLitres;
		//	PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand =
		//		other.PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand;
		//	PneumaticAuxillariesConfig.PerDoorOpeningNI = other.PneumaticAuxillariesConfig.PerDoorOpeningNI;
		//	PneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG =
		//		other.PneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG;
		//	PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand =
		//		other.PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand;
		//	PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction =
		//		other.PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction;
		//}

		//private void ClonePneumaticsUserInputsConfig(AuxiliaryConfig other)
		//{
		//	PneumaticUserInputsConfig.ActuationsMap = other.PneumaticUserInputsConfig.ActuationsMap;
		//	PneumaticUserInputsConfig.AdBlueDosing = other.PneumaticUserInputsConfig.AdBlueDosing;
		//	PneumaticUserInputsConfig.AirSuspensionControl = other.PneumaticUserInputsConfig.AirSuspensionControl;
		//	PneumaticUserInputsConfig.CompressorGearEfficiency = other.PneumaticUserInputsConfig.CompressorGearEfficiency;
		//	PneumaticUserInputsConfig.CompressorGearRatio = other.PneumaticUserInputsConfig.CompressorGearRatio;
		//	PneumaticUserInputsConfig.CompressorMap = other.PneumaticUserInputsConfig.CompressorMap;
		//	PneumaticUserInputsConfig.Doors = other.PneumaticUserInputsConfig.Doors;
		//	PneumaticUserInputsConfig.KneelingHeightMillimeters = other.PneumaticUserInputsConfig.KneelingHeightMillimeters;
		//	PneumaticUserInputsConfig.RetarderBrake = other.PneumaticUserInputsConfig.RetarderBrake;
		//	PneumaticUserInputsConfig.SmartAirCompression = other.PneumaticUserInputsConfig.SmartAirCompression;
		//	PneumaticUserInputsConfig.SmartRegeneration = other.PneumaticUserInputsConfig.SmartRegeneration;
		//}

		//private void CloneHVAC(AuxiliaryConfig other)
		//{
		//	//HvacUserInputsConfig.HVACConstants = other.HvacUserInputsConfig.HVACConstants;

		//}
	}
}
