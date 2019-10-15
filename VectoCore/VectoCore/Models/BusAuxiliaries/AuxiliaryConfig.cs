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
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.PneumaticSystem;
using TUGraz.VectoCore.Models.BusAuxiliaries.Legacy;

namespace TUGraz.VectoCore.Models.BusAuxiliaries {
	[Serializable()]
	public class AuxiliaryConfig : IAuxiliaryConfig
	{
		// Vecto
		public IVectoInputs VectoInputs { get; set; }

		// Electrical
		public IElectricsUserInputsConfig ElectricalUserInputsConfig { get; set; }

		// Pneumatics
		public IPneumaticUserInputsConfig PneumaticUserInputsConfig { get; set; }

		public IPneumaticsAuxilliariesConfig PneumaticAuxillariesConfig { get; set; }

		// Hvac
		public IHVACUserInputsConfig HvacUserInputsConfig { get; set; }

		// Vecto Signals
		[JsonIgnore]
		public ISignals Signals { get; set; }

		// Constructors
		public AuxiliaryConfig() : this("EMPTY") { }

		public AuxiliaryConfig(string auxConfigFile)
		{
			// Special Condition
			if (auxConfigFile == "EMPTY") {
				ElectricalUserInputsConfig = new ElectricsUserInputsConfig() { PowerNetVoltage = 28.3.SI<Volt>() };
				ElectricalUserInputsConfig.ElectricalConsumers = new ElectricalConsumerList(28.3.SI<Volt>(), 0.096, false);
				ElectricalUserInputsConfig.ResultCardIdle = new ResultCard(new List<SmartResult>());
				ElectricalUserInputsConfig.ResultCardOverrun = new ResultCard(new List<SmartResult>());
				ElectricalUserInputsConfig.ResultCardTraction = new ResultCard(new List<SmartResult>());
				PneumaticAuxillariesConfig = new PneumaticsAuxilliariesConfig(false);
				PneumaticUserInputsConfig = new PneumaticUserInputsConfig(false);
				HvacUserInputsConfig = new HVACUserInputsConfig(string.Empty, string.Empty, false);
				return;
			}

			if (auxConfigFile == null || auxConfigFile.Trim().Length == 0 || !File.Exists(auxConfigFile))
				setDefaults();
			else {
				setDefaults();

				if (!Load(auxConfigFile))
					MessageBox.Show(string.Format("Unable to load file  {0}", auxConfigFile));
			}
		}

		// Set Default Values
		private void setDefaults()
		{
			var tmp = new VectoInputs
				() {
					Cycle = "Urban",
					VehicleWeightKG = 16500.SI<Kilogram>(),
					PowerNetVoltage = 28.3.SI<Volt>()
				};
			VectoInputs = tmp;
			Signals = new Signals() { EngineSpeed = 2000.RPMtoRad(), TotalCycleTimeSeconds = 3114, ClutchEngaged = false };

			// Pneumatics set deault values
			PneumaticUserInputsConfig = new PneumaticUserInputsConfig(true);
			PneumaticAuxillariesConfig = new PneumaticsAuxilliariesConfig(true);

			// Electrical set deault values
			ElectricalUserInputsConfig = new ElectricsUserInputsConfig(true, tmp);
			ElectricalUserInputsConfig.ElectricalConsumers = new ElectricalConsumerList(28.3.SI<Volt>(), 0.096, true);

			// HVAC set deault values
			HvacUserInputsConfig = new HVACUserInputsConfig(string.Empty, string.Empty, false);
		}

		private double GetDoorActuationTimeFraction()
		{
			var actuationsMap = new PneumaticActuationsMap(PneumaticUserInputsConfig.ActuationsMap);
			var actuationsKey = new ActuationsKey("Park brake + 2 doors", VectoInputs.Cycle);

			var numActuations = actuationsMap.GetNumActuations(actuationsKey);
			var secondsPerActuation = ElectricalUserInputsConfig.DoorActuationTimeSecond;

			var doorDutyCycleFraction = (numActuations * secondsPerActuation) / (double)Signals.TotalCycleTimeSeconds;

			return doorDutyCycleFraction;
		}


		private bool CompareElectricalConfiguration(IAuxiliaryConfig other)
		{
			// AlternatorGearEfficiency
			if (ElectricalUserInputsConfig.AlternatorGearEfficiency != other.ElectricalUserInputsConfig.AlternatorGearEfficiency)
				return false;

			// AlternatorMap
			if (ElectricalUserInputsConfig.AlternatorMap != other.ElectricalUserInputsConfig.AlternatorMap)
				return false;

			// DoorActuationTimeSecond
			if (ElectricalUserInputsConfig.DoorActuationTimeSecond != other.ElectricalUserInputsConfig.DoorActuationTimeSecond)
				return false;

			// Consumer list
			if (ElectricalUserInputsConfig.ElectricalConsumers.Items.Count !=
				other.ElectricalUserInputsConfig.ElectricalConsumers.Items.Count)
				return false;

			int i;
			for (i = 0; i <= ElectricalUserInputsConfig.ElectricalConsumers.Items.Count - 1; i++) {
				IElectricalConsumer thisConsumer, otherConsumer;
				thisConsumer = ElectricalUserInputsConfig.ElectricalConsumers.Items[i];
				otherConsumer = other.ElectricalUserInputsConfig.ElectricalConsumers.Items[i];

				if (thisConsumer.AvgConsumptionAmps != otherConsumer.AvgConsumptionAmps ||
					thisConsumer.BaseVehicle != otherConsumer.BaseVehicle || thisConsumer.Category != otherConsumer.Category ||
					thisConsumer.ConsumerName != otherConsumer.ConsumerName ||
					thisConsumer.NominalConsumptionAmps != otherConsumer.NominalConsumptionAmps ||
					thisConsumer.NumberInActualVehicle != otherConsumer.NumberInActualVehicle ||
					thisConsumer.PhaseIdle_TractionOn != otherConsumer.PhaseIdle_TractionOn ||
					thisConsumer.TotalAvgConsumptionInWatts() != otherConsumer.TotalAvgConsumptionInWatts() ||
					thisConsumer.TotalAvgConumptionAmps() != otherConsumer.TotalAvgConumptionAmps())
					return false;
			}

			// PowerNetVoltage
			if (ElectricalUserInputsConfig.PowerNetVoltage != other.ElectricalUserInputsConfig.PowerNetVoltage)
				return false;

			// ResultCardIdle
			if (ElectricalUserInputsConfig.ResultCardIdle.Results.Count !=
				other.ElectricalUserInputsConfig.ResultCardIdle.Results.Count)
				return false;

			for (i = 0; i <= ElectricalUserInputsConfig.ResultCardIdle.Results.Count - 1; i++) {
				if (ElectricalUserInputsConfig.ResultCardIdle.Results[i].Amps !=
					other.ElectricalUserInputsConfig.ResultCardIdle.Results[i].Amps ||
					ElectricalUserInputsConfig.ResultCardIdle.Results[i].SmartAmps !=
					other.ElectricalUserInputsConfig.ResultCardIdle.Results[i].SmartAmps)
					return false;
			}

			// ResultCardOverrun
			if (ElectricalUserInputsConfig.ResultCardOverrun.Results.Count !=
				other.ElectricalUserInputsConfig.ResultCardOverrun.Results.Count)
				return false;

			for (i = 0; i <= ElectricalUserInputsConfig.ResultCardOverrun.Results.Count - 1; i++) {
				if (ElectricalUserInputsConfig.ResultCardOverrun.Results[i].Amps !=
					other.ElectricalUserInputsConfig.ResultCardOverrun.Results[i].Amps ||
					ElectricalUserInputsConfig.ResultCardOverrun.Results[i].SmartAmps !=
					other.ElectricalUserInputsConfig.ResultCardOverrun.Results[i].SmartAmps)
					return false;
			}

			// ResultCardTraction
			if (ElectricalUserInputsConfig.ResultCardTraction.Results.Count !=
				other.ElectricalUserInputsConfig.ResultCardTraction.Results.Count)
				return false;

			for (i = 0; i <= ElectricalUserInputsConfig.ResultCardTraction.Results.Count - 1; i++) {
				if (ElectricalUserInputsConfig.ResultCardTraction.Results[i].Amps !=
					other.ElectricalUserInputsConfig.ResultCardTraction.Results[i].Amps ||
					ElectricalUserInputsConfig.ResultCardTraction.Results[i].SmartAmps !=
					other.ElectricalUserInputsConfig.ResultCardTraction.Results[i].SmartAmps)
					return false;
			}

			// SmartElectrical
			if (ElectricalUserInputsConfig.SmartElectrical != other.ElectricalUserInputsConfig.SmartElectrical)
				return false;

			return true;
		}

		private bool ComparePneumaticAuxiliariesConfig(IAuxiliaryConfig other)
		{
			if (PneumaticAuxillariesConfig.AdBlueNIperMinute != other.PneumaticAuxillariesConfig.AdBlueNIperMinute)
				return false;
			if (PneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute !=
				other.PneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute)
				return false;
			if (PneumaticAuxillariesConfig.BrakingNoRetarderNIperKG != other.PneumaticAuxillariesConfig.BrakingNoRetarderNIperKG)
				return false;
			if (PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG !=
				other.PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG)
				return false;
			if (PneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM !=
				other.PneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM)
				return false;
			if (PneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour !=
				other.PneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour)
				return false;
			if (PneumaticAuxillariesConfig.DeadVolumeLitres != other.PneumaticAuxillariesConfig.DeadVolumeLitres)
				return false;
			if (PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand !=
				other.PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand)
				return false;
			if (PneumaticAuxillariesConfig.PerDoorOpeningNI != other.PneumaticAuxillariesConfig.PerDoorOpeningNI)
				return false;
			if (PneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG !=
				other.PneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG)
				return false;
			if (PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand !=
				other.PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand)
				return false;
			if (PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction !=
				other.PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction)
				return false;

			return true;
		}

		private bool ComparePneumaticUserConfig(IAuxiliaryConfig other)
		{
			if (PneumaticUserInputsConfig.ActuationsMap != other.PneumaticUserInputsConfig.ActuationsMap)
				return false;
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

		private bool CompareHVACConfig(IAuxiliaryConfig other)
		{
			if (HvacUserInputsConfig.SSMFilePath != other.HvacUserInputsConfig.SSMFilePath)
				return false;
			if (HvacUserInputsConfig.BusDatabasePath != other.HvacUserInputsConfig.BusDatabasePath)
				return false;
			if (HvacUserInputsConfig.SSMDisabled != other.HvacUserInputsConfig.SSMDisabled)
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
			if (!CompareHVACConfig(other))
				return false;

			return true;
		}


		// Persistance Functions
		public bool Save(string auxFile)
		{
			var returnValue = true;

			// JSON METHOD
			try {
				var body = new Dictionary<string, object>();

				body["ElectricalUserInputsConfig"] = SaveElectricalConfig();
				body["PneumaticAuxillariesConfig"] = SavePneumaticAuxconfig();
				body["PneumaticUserInputsConfig"] = SavePneumaticUserConfig();
				body["HvacUserInputsConfig"] = SaveHVACUserConfig();
				
				JSONInputDataFactory.WriteFile(JToken.FromObject(new Dictionary<string, object>(){ { "Header","AAUX"}, {"Body", body }}), auxFile);
			} catch (Exception ) {
				returnValue = false;
			}

			return returnValue;
		}

		protected Dictionary<string, object> SaveElectricalConfig()
		{
			var elData = new Dictionary<string, object>();
			// AlternatorGearEfficiency
			elData["AlternatorGearEfficiency"] = ElectricalUserInputsConfig.AlternatorGearEfficiency;

			// AlternatorMap
			elData["AlternatorMap"] = ElectricalUserInputsConfig.AlternatorMap;

			// DoorActuationTimeSecond
			elData["DoorActuationTimeSecond"] = ElectricalUserInputsConfig.DoorActuationTimeSecond;

			// Electrical Consumer list

			var elConsumers = new List<object>();
			foreach (var entry in ElectricalUserInputsConfig.ElectricalConsumers.Items) {
				var newConsumer = new Dictionary<string, object>();

				newConsumer["BaseVehicle"] = entry.BaseVehicle;
				newConsumer["Category"] = entry.Category;
				newConsumer["ConsumerName"] = entry.ConsumerName;
				newConsumer["NominalConsumptionAmps"] = entry.NominalConsumptionAmps.Value();
				newConsumer["PhaseIdle_TractionOn"] = entry.PhaseIdle_TractionOn;
				newConsumer["PowerNetVoltage"] = entry.PowerNetVoltage.Value();
				newConsumer["NumberInActualVehicle"] = entry.NumberInActualVehicle;
				newConsumer["Info"] = entry.Info;

				elConsumers.Add(newConsumer);
			}

			elData["ElectricalConsumers"] = elConsumers;

			// PowerNetVoltage
			elData["PowerNetVoltage"] = ElectricalUserInputsConfig.PowerNetVoltage.Value();

			// ResultCardIdle
			var resultCard = new List<object>();
			foreach (var result in ElectricalUserInputsConfig.ResultCardIdle.Results) {
				resultCard.Add(new Dictionary<string, object>() {
					{ "Amps",  result.Amps.Value()},
					{"SmartAmps",result.SmartAmps.Value() }
				});
			}

			elData["ResultCardIdle"] = resultCard;


			// ResultCardOverrun
			resultCard.Clear();
			foreach (var result in ElectricalUserInputsConfig.ResultCardOverrun.Results) {
				resultCard.Add(new Dictionary<string, object>() {
					{ "Amps",  result.Amps.Value()},
					{"SmartAmps",result.SmartAmps.Value() }
				});
			}

			elData["ResultCardOverrun"] = resultCard;

			// ResultCardTraction
			resultCard.Clear();
			foreach (var result in ElectricalUserInputsConfig.ResultCardTraction.Results) {
				resultCard.Add(new Dictionary<string, object>() {
					{ "Amps",  result.Amps.Value()},
					{"SmartAmps",result.SmartAmps.Value() }
				});
			}

			elData["ResultCardTraction"] = resultCard;

			// SmartElectrical
			elData["SmartElectrical"] = ElectricalUserInputsConfig.SmartElectrical;

			return elData;
		}


		private Dictionary<string,object > SaveHVACUserConfig()
		{
			var hvac = new Dictionary<string, object>();

			hvac["SSMFilePath"] = HvacUserInputsConfig.SSMFilePath;
			hvac["BusDatabasePath"] = HvacUserInputsConfig.BusDatabasePath;
			hvac["SSMDisabled"] = HvacUserInputsConfig.SSMDisabled;

			return hvac;
		}

		private Dictionary<string, object> SavePneumaticUserConfig()
		{
			var puData = new Dictionary<string, object>();

			puData["ActuationsMap"] = PneumaticUserInputsConfig.ActuationsMap;
			puData["AdBlueDosing"] = PneumaticUserInputsConfig.AdBlueDosing;
			puData["AirSuspensionControl"] = PneumaticUserInputsConfig.AirSuspensionControl;
			puData["CompressorGearEfficiency"] = PneumaticUserInputsConfig.CompressorGearEfficiency;
			puData["CompressorGearRatio"] = PneumaticUserInputsConfig.CompressorGearRatio;
			puData["CompressorMap"] = PneumaticUserInputsConfig.CompressorMap;
			puData["Doors"] = PneumaticUserInputsConfig.Doors;
			puData["KneelingHeightMillimeters"] = PneumaticUserInputsConfig.KneelingHeightMillimeters;
			puData["RetarderBrake"] = PneumaticUserInputsConfig.RetarderBrake;
			puData["SmartAirCompression"] = PneumaticUserInputsConfig.SmartAirCompression;
			puData["SmartRegeneration"] = PneumaticUserInputsConfig.SmartRegeneration;

			return puData;
		}

		protected Dictionary<string, object> SavePneumaticAuxconfig()
		{
			var paData = new Dictionary<string, object>();

			paData["AdBlueNIperMinute"] = PneumaticAuxillariesConfig.AdBlueNIperMinute;
			paData["AirControlledSuspensionNIperMinute"] = PneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute;
			paData["BrakingNoRetarderNIperKG"] = PneumaticAuxillariesConfig.BrakingNoRetarderNIperKG;
			paData["BrakingWithRetarderNIperKG"] = PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG;
			paData["BreakingPerKneelingNIperKGinMM"] = PneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM ;
			paData["DeadVolBlowOutsPerLitresperHour"] = PneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour;
			paData["DeadVolumeLitres"] = PneumaticAuxillariesConfig.DeadVolumeLitres;
			paData["NonSmartRegenFractionTotalAirDemand"] = PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand;
			paData["PerDoorOpeningNI"] = PneumaticAuxillariesConfig.PerDoorOpeningNI;
			paData["PerStopBrakeActuationNIperKG"] = PneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG;
			paData["SmartRegenFractionTotalAirDemand"] = PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand;
			paData["OverrunUtilisationForCompressionFraction"] =
				PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction;

			return paData;
		}

		public bool Load(string auxFile)
		{
			var returnValue = true;
			
			try {
				var json = JSONInputDataFactory.ReadFile(auxFile);
				LoadValues((JObject)json["Body"]);

			} catch (Exception ) {
				returnValue = false;
			}

			return returnValue;
		}

		protected void LoadValues(JObject data)
		{
			LoadElectricalConfig((JObject)data["ElectricalUserInputsConfig"]);
			LoadPneumaticsAuxConfig((JObject)data["PneumaticAuxillariesConfig"]);
			LoadPneumaticUserConfig((JObject)data["PneumaticUserInputsConfig"]);
			LoadHVAC((JObject)data["HvacUserInputsConfig"]);
		}

		protected void LoadElectricalConfig(JObject elData)
		{
			// AlternatorGearEfficiency
			ElectricalUserInputsConfig.AlternatorGearEfficiency = elData.GetEx<double>("AlternatorGearEfficiency"); 

			// AlternatorMap
			ElectricalUserInputsConfig.AlternatorMap = elData.GetEx("AlternatorMap").Value<string>();

			// DoorActuationTimeSecond
			ElectricalUserInputsConfig.DoorActuationTimeSecond = elData.GetEx<double>("DoorActuationTimeSecond"); 

			// Electrical Consumer list
			ElectricalUserInputsConfig.ElectricalConsumers.Items.Clear();
			foreach (var consumer in elData["ElectricalConsumers"]) {
				var newConsumer = new ElectricalConsumer(
					consumer.GetEx<bool>("BaseVehicle"), consumer.GetEx<string>("Category"), consumer.GetEx<string>("ConsumerName"), consumer.GetEx<double>("NominalConsumptionAmps").SI<Ampere>(),
					consumer.GetEx<double>("PhaseIdle_TractionOn"), consumer.GetEx<double>("PowerNetVoltage").SI<Volt>(), consumer.GetEx<int>("NumberInActualVehicle"),
					consumer.GetEx<string>("Info"));

				ElectricalUserInputsConfig.ElectricalConsumers.Items.Add(newConsumer);
			}

			// PowerNetVoltage
			ElectricalUserInputsConfig.PowerNetVoltage = elData.GetEx<double>("PowerNetVoltage").SI<Volt>();

			// ResultCardIdle
			ElectricalUserInputsConfig.ResultCardIdle.Results.Clear();
			foreach (var result in elData["ResultCardIdle"])
				ElectricalUserInputsConfig.ResultCardIdle.Results.Add(new SmartResult(result.GetEx<double>("Amps").SI<Ampere>(), result.GetEx<double>("SmartAmps").SI<Ampere>()));

			// ResultCardOverrun
			ElectricalUserInputsConfig.ResultCardOverrun.Results.Clear();
			foreach (var result in elData["ResultCardOverrun"])
				ElectricalUserInputsConfig.ResultCardOverrun.Results.Add(new SmartResult(result.GetEx<double>("Amps").SI<Ampere>(), result.GetEx<double>("SmartAmps").SI<Ampere>()));

			// ResultCardTraction
			ElectricalUserInputsConfig.ResultCardTraction.Results.Clear();
			foreach (var result in elData["ResultCardTraction"])
				ElectricalUserInputsConfig.ResultCardTraction.Results.Add(new SmartResult(result.GetEx<double>("Amps").SI<Ampere>(), result.GetEx<double>("SmartAmps").SI<Ampere>()));

			// SmartElectrical
			ElectricalUserInputsConfig.SmartElectrical = elData.GetEx<bool>("SmartElectrical"); 
		}

		protected void LoadPneumaticsAuxConfig(JObject paData)
		{
			PneumaticAuxillariesConfig.AdBlueNIperMinute = paData.GetEx<double>("AdBlueNIperMinute"); 
			PneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute = paData.GetEx<double>("AirControlledSuspensionNIperMinute"); 
			PneumaticAuxillariesConfig.BrakingNoRetarderNIperKG = paData.GetEx<double>("BrakingNoRetarderNIperKG"); 
			PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG = paData.GetEx<double>("BrakingWithRetarderNIperKG"); 
			PneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM = paData.GetEx<double>("BreakingPerKneelingNIperKGinMM"); 
			PneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour = paData.GetEx<double>("DeadVolBlowOutsPerLitresperHour"); 
			PneumaticAuxillariesConfig.DeadVolumeLitres = paData.GetEx<double>("DeadVolumeLitres"); 
			PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand = paData.GetEx<double>("NonSmartRegenFractionTotalAirDemand"); 
			PneumaticAuxillariesConfig.PerDoorOpeningNI = paData.GetEx<double>("PerDoorOpeningNI"); 
			PneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG = paData.GetEx<double>("PerStopBrakeActuationNIperKG"); 
			PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand = paData.GetEx<double>("SmartRegenFractionTotalAirDemand"); 
			PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction = paData.GetEx<double>("OverrunUtilisationForCompressionFraction"); 
		}

		protected void LoadPneumaticUserConfig(JObject puData)
		{
			PneumaticUserInputsConfig.ActuationsMap = puData.GetEx<string>("ActuationsMap"); 
			PneumaticUserInputsConfig.AdBlueDosing = puData.GetEx<string>("AdBlueDosing"); 
			PneumaticUserInputsConfig.AirSuspensionControl = puData.GetEx<string>("AirSuspensionControl"); 
			PneumaticUserInputsConfig.CompressorGearEfficiency = puData.GetEx<double>("CompressorGearEfficiency"); 
			PneumaticUserInputsConfig.CompressorGearRatio = puData.GetEx<double>("CompressorGearRatio"); 
			PneumaticUserInputsConfig.CompressorMap = puData.GetEx<string>("CompressorMap"); 
			PneumaticUserInputsConfig.Doors = puData.GetEx<string>("Doors"); 
			PneumaticUserInputsConfig.KneelingHeightMillimeters = puData.GetEx<double>("KneelingHeightMillimeters"); 
			PneumaticUserInputsConfig.RetarderBrake = puData.GetEx<bool>("RetarderBrake"); 
			PneumaticUserInputsConfig.SmartAirCompression = puData.GetEx<bool>("SmartAirCompression"); 
			PneumaticUserInputsConfig.SmartRegeneration = puData.GetEx<bool>("SmartRegeneration"); 
		}

		protected void LoadHVAC(JObject hvac)
		{
			HvacUserInputsConfig.SSMFilePath = hvac.GetEx<string>("SSMFilePath"); 
			HvacUserInputsConfig.BusDatabasePath = hvac.GetEx<string>("BusDatabasePath"); 
			HvacUserInputsConfig.SSMDisabled = hvac.GetEx<bool>("SSMDisabled"); 
		}


		// Persistance Helpers
		public void AssumeValuesOfOther(AuxiliaryConfig other)
		{
			CloneElectricaConfiguration(other);
			ClonePneumaticsAuxiliariesConfig(other);
			ClonePneumaticsUserInputsConfig(other);
			CloneHVAC(other);
		}

		private void CloneElectricaConfiguration(AuxiliaryConfig other)
		{
			// AlternatorGearEfficiency
			ElectricalUserInputsConfig.AlternatorGearEfficiency = other.ElectricalUserInputsConfig.AlternatorGearEfficiency;

			// AlternatorMap
			ElectricalUserInputsConfig.AlternatorMap = other.ElectricalUserInputsConfig.AlternatorMap;

			// DoorActuationTimeSecond
			ElectricalUserInputsConfig.DoorActuationTimeSecond = other.ElectricalUserInputsConfig.DoorActuationTimeSecond;

			// Electrical Consumer list
			ElectricalUserInputsConfig.ElectricalConsumers.Items.Clear();
			foreach (var otherConsumer in other.ElectricalUserInputsConfig.ElectricalConsumers.Items) {
				var newConsumer = new ElectricalConsumer(
					otherConsumer.BaseVehicle, otherConsumer.Category, otherConsumer.ConsumerName, otherConsumer.NominalConsumptionAmps,
					otherConsumer.PhaseIdle_TractionOn, otherConsumer.PowerNetVoltage, otherConsumer.NumberInActualVehicle,
					otherConsumer.Info);

				ElectricalUserInputsConfig.ElectricalConsumers.Items.Add(newConsumer);
			}

			// PowerNetVoltage
			ElectricalUserInputsConfig.PowerNetVoltage = other.ElectricalUserInputsConfig.PowerNetVoltage;

			// ResultCardIdle
			ElectricalUserInputsConfig.ResultCardIdle.Results.Clear();
			foreach (var result in other.ElectricalUserInputsConfig.ResultCardIdle.Results)
				ElectricalUserInputsConfig.ResultCardIdle.Results.Add(new SmartResult(result.Amps, result.SmartAmps));

			// ResultCardOverrun
			ElectricalUserInputsConfig.ResultCardOverrun.Results.Clear();
			foreach (var result in other.ElectricalUserInputsConfig.ResultCardOverrun.Results)
				ElectricalUserInputsConfig.ResultCardOverrun.Results.Add(new SmartResult(result.Amps, result.SmartAmps));

			// ResultCardTraction
			ElectricalUserInputsConfig.ResultCardTraction.Results.Clear();
			foreach (var result in other.ElectricalUserInputsConfig.ResultCardTraction.Results)
				ElectricalUserInputsConfig.ResultCardTraction.Results.Add(new SmartResult(result.Amps, result.SmartAmps));

			// SmartElectrical
			ElectricalUserInputsConfig.SmartElectrical = other.ElectricalUserInputsConfig.SmartElectrical;
		}

		private void ClonePneumaticsAuxiliariesConfig(AuxiliaryConfig other)
		{
			PneumaticAuxillariesConfig.AdBlueNIperMinute = other.PneumaticAuxillariesConfig.AdBlueNIperMinute;
			PneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute =
				other.PneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute;
			PneumaticAuxillariesConfig.BrakingNoRetarderNIperKG = other.PneumaticAuxillariesConfig.BrakingNoRetarderNIperKG;
			PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG = other.PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG;
			PneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM =
				other.PneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM;
			PneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour =
				other.PneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour;
			PneumaticAuxillariesConfig.DeadVolumeLitres = other.PneumaticAuxillariesConfig.DeadVolumeLitres;
			PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand =
				other.PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand;
			PneumaticAuxillariesConfig.PerDoorOpeningNI = other.PneumaticAuxillariesConfig.PerDoorOpeningNI;
			PneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG =
				other.PneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG;
			PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand =
				other.PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand;
			PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction =
				other.PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction;
		}

		private void ClonePneumaticsUserInputsConfig(AuxiliaryConfig other)
		{
			PneumaticUserInputsConfig.ActuationsMap = other.PneumaticUserInputsConfig.ActuationsMap;
			PneumaticUserInputsConfig.AdBlueDosing = other.PneumaticUserInputsConfig.AdBlueDosing;
			PneumaticUserInputsConfig.AirSuspensionControl = other.PneumaticUserInputsConfig.AirSuspensionControl;
			PneumaticUserInputsConfig.CompressorGearEfficiency = other.PneumaticUserInputsConfig.CompressorGearEfficiency;
			PneumaticUserInputsConfig.CompressorGearRatio = other.PneumaticUserInputsConfig.CompressorGearRatio;
			PneumaticUserInputsConfig.CompressorMap = other.PneumaticUserInputsConfig.CompressorMap;
			PneumaticUserInputsConfig.Doors = other.PneumaticUserInputsConfig.Doors;
			PneumaticUserInputsConfig.KneelingHeightMillimeters = other.PneumaticUserInputsConfig.KneelingHeightMillimeters;
			PneumaticUserInputsConfig.RetarderBrake = other.PneumaticUserInputsConfig.RetarderBrake;
			PneumaticUserInputsConfig.SmartAirCompression = other.PneumaticUserInputsConfig.SmartAirCompression;
			PneumaticUserInputsConfig.SmartRegeneration = other.PneumaticUserInputsConfig.SmartRegeneration;
		}

		private void CloneHVAC(AuxiliaryConfig other)
		{
			HvacUserInputsConfig.SSMFilePath = other.HvacUserInputsConfig.SSMFilePath;
			HvacUserInputsConfig.BusDatabasePath = other.HvacUserInputsConfig.BusDatabasePath;
			HvacUserInputsConfig.SSMDisabled = other.HvacUserInputsConfig.SSMDisabled;
		}
	}
}
