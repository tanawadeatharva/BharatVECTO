/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONVehicleDataV10 : JSONVehicleDataV9
	{
		private JSONElectricStorageEngineeringInputData _batteries;
		private JSONElectricMotors _electricMotors;

		public JSONVehicleDataV10(JObject data, string fileName, IJSONVehicleComponents job, bool tolerateMissing = false) :
			base(data, fileName, job, tolerateMissing) { }

		#region Overrides of JSONVehicleDataV7

		protected override IElectricMachinesEngineeringInputData GetElectricMachines()
		{
			return _electricMotors ?? (_electricMotors = ReadMotors());
		}

		protected override IElectricStorageEngineeringInputData GetElectricStorage()
		{
			return _batteries ?? (_batteries = ReadBatteries());
		}

		protected virtual JSONElectricMotors ReadMotors()
		{
			var retVal = new List<ElectricMachineEntry<IElectricMotorEngineeringInputData>>();
			foreach (var entry in Body["ElectricMotors"])
			{
				var tmp = new ElectricMachineEntry<IElectricMotorEngineeringInputData>()
				{
					Position = PowertrainPositionHelper.Parse(entry.GetEx<string>("Position")),
					Count = entry.GetEx<int>("Count"),
					ElectricMachine = JSONInputDataFactory.ReadElectricMotorData(Path.Combine(BasePath, entry.GetEx<string>("MotorFile")), false)
				};
				retVal.Add(tmp);
			}

			return new JSONElectricMotors(retVal);
		}


		protected virtual JSONElectricStorageEngineeringInputData ReadBatteries()
		{
			return new JSONElectricStorageEngineeringInputData() {
				Count = Body["Battery"].GetEx<int>("NumPacks"),
				BatteryPack = JSONInputDataFactory.ReadBatteryData(Path.Combine(BasePath, Body["Battery"].GetEx<string>("BatteryFile")), false)
			};
		}

		#endregion
	}

	public class JSONElectricMotors : IElectricMachinesEngineeringInputData {
		private readonly IList<ElectricMachineEntry<IElectricMotorEngineeringInputData>> _entries;

		public JSONElectricMotors(List<ElectricMachineEntry<IElectricMotorEngineeringInputData>> entries)
		{
			_entries = entries;
		}

		IList<ElectricMachineEntry<IElectricMotorDeclarationInputData>> IElectricMachinesDeclarationInputData.Entries
		{
			get { return _entries.Cast<ElectricMachineEntry<IElectricMotorDeclarationInputData>>().ToList(); }
			//get { return null; }
		}

		public virtual IList<ElectricMachineEntry<IElectricMotorEngineeringInputData>> Entries
		{
			get { return _entries; }
		}
	}

	public class JSONElectricStorageEngineeringInputData : IElectricStorageEngineeringInputData {
		IBatteryPackDeclarationInputData IElectricStorageDeclarationInputData.BatteryPack
		{
			get { return BatteryPack; }
		}

		public IBatteryPackEngineeringInputData BatteryPack { get; internal set; }
		public int Count { get; internal set; }
	}

	// ###################################################################
	// ###################################################################

	public class JSONVehicleDataV9 : JSONVehicleDataV7, IBusAuxiliariesDeclarationData, IElectricSupplyDeclarationData,
		IElectricConsumersDeclarationData, IPneumaticSupplyDeclarationData, IPneumaticConsumersDeclarationData,
		IHVACBusAuxiliariesDeclarationData
	{
		public JSONVehicleDataV9(JObject data, string fileName, IJSONVehicleComponents job, bool tolerateMissing = false) :
			base(data, fileName, job, tolerateMissing) { }

		#region Overrides of JSONVehicleDataV7

		public override IBusAuxiliariesDeclarationData BusAuxiliaries
		{
			get { return this; }
		}

		#region Overrides of JSONVehicleDataV7

		public override bool Articulated
		{
			get { return Body.GetEx<bool>("Articulated"); }
		}

		public override bool? ATEcoRollReleaseLockupClutch
		{
			get { return Body["ATEcoRollReleaseLockupClutch"]?.Value<bool>(); }
		}

		#endregion

		#endregion

		#region Implementation of IBusAuxiliariesDeclarationData

		public virtual string FanTechnology
		{
			get { return Body["Aux"]?.Value<string>("FanTechnology"); }
		}

		public virtual IList<string> SteeringPumpTechnology
		{
			get { return Body["Aux"]?["SteeringPumpTechnology"].Select(x => x.Value<string>()).ToList(); }
		}

		public virtual IElectricSupplyDeclarationData ElectricSupply
		{
			get { return this; }
		}

		public virtual IElectricConsumersDeclarationData ElectricConsumers
		{
			get { return this; }
		}

		public virtual IPneumaticSupplyDeclarationData PneumaticSupply
		{
			get { return this; }
		}

		public virtual IPneumaticConsumersDeclarationData PneumaticConsumers
		{
			get { return this; }
		}

		public virtual IHVACBusAuxiliariesDeclarationData HVACAux
		{
			get { return this; }
		}

		#endregion

		#region Implementation of IElectricSupplyDeclarationData

		public virtual IList<IAlternatorDeclarationInputData> Alternators
		{
			get {
				return Body["Aux"]?["ElectricSupply"]?["Alternators"]
							.Select(x => new AlternatorInputData(x.GetEx<string>("Technology")))
							.Cast<IAlternatorDeclarationInputData>().ToList() ?? new List<IAlternatorDeclarationInputData>();
			}
		}

		#endregion

		#region Implementation of IElectricConsumersDeclarationData

		public virtual bool InteriorLightsLED
		{
			get { return false; }
		}

		public virtual bool DayrunninglightsLED
		{
			get { return false; }
		}

		public virtual bool PositionlightsLED
		{
			get { return false; }
		}

		public virtual bool HeadlightsLED
		{
			get { return false; }
		}

		public virtual bool BrakelightsLED
		{
			get { return false; }
		}

		public virtual bool SmartElectrics
		{
			get { return Body["Aux"]?["ElectricSupply"]?.GetEx<bool>("SmartElectrics") ?? false; }
		}

		public Watt MaxAlternatorPower
		{
			get { return Body["Aux"]?["ElectricSupply"]?.GetEx<double>("MaxAlternatorPower").SI<Watt>() ?? null; }
		}

		public WattSecond ElectricStorageCapacity
		{
			get { return Body["Aux"]?["ElectricSupply"]?.GetEx<double>("ElectricStorageCapacity").SI(Unit.SI.Watt.Hour).Cast<WattSecond>() ?? null; }
		}

		#endregion

		#region Implementation of IPneumaticSupplyDeclarationData

		public string Clutch { get; }
		public virtual double Ratio { get { return Body["Aux"]?["PneumaticSupply"]?.GetEx<double>("Ratio") ?? 0.0; } }
		public virtual string CompressorSize { get {
			return Body["Aux"]?["PneumaticSupply"]?.GetEx<string>("CompressorSize");
		} }

		public bool SmartAirCompression { get; }
		public bool SmartRegeneration { get; }

		#endregion

		#region Implementation of IPneumaticConsumersDeclarationData

		public virtual ConsumerTechnology AirsuspensionControl { get {
			return Body["Aux"]?["PneumaticConsumers"]?.GetEx<string>("AirsuspensionControl").ParseEnum<ConsumerTechnology>() ??
					ConsumerTechnology.Unknown;
		} }
		public virtual ConsumerTechnology AdBlueDosing { get {
				return Body["Aux"]?["PneumaticConsumers"]?.GetEx<string>("AdBlueDosing").ParseEnum<ConsumerTechnology>() ??
						ConsumerTechnology.Unknown;
			}
		}
		public virtual ConsumerTechnology DoorDriveTechnology { get { return ConsumerTechnology.Unknown; } }

		
		#endregion

		#region Implementation of IHVACBusAuxiliariesDeclarationData

		public virtual BusHVACSystemConfiguration SystemConfiguration { get; set; }
		public virtual ACCompressorType CompressorTypeDriver { get { return ACCompressorType.Unknown; } }
		public virtual ACCompressorType CompressorTypePassenger { get { return ACCompressorType.Unknown; } }
		public virtual Watt AuxHeaterPower { get { return null; } }
		public virtual bool DoubleGlasing { get { return false; } }
		public virtual bool HeatPump { get { return false; } }
		public virtual bool AdjustableCoolantThermostat { get { return Body["Aux"]?["HVAC"]?.GetEx<bool>("AdjustableCoolantThermostat") ?? false; } }
		public virtual bool AdjustableAuxiliaryHeater { get { return false; } }
		public virtual bool EngineWasteGasHeatExchanger { get { return Body["Aux"]?["HVAC"]?.GetEx<bool>("EngineWasteGasHeatExchanger") ?? false; } }
		public virtual bool SeparateAirDistributionDucts { get { return false; } }

		#endregion

	}

	// ###################################################################
	// ###################################################################

	public class JSONVehicleDataV8 : JSONVehicleDataV7
	{
		public JSONVehicleDataV8(JObject data, string fileName, IJSONVehicleComponents job, bool tolerateMissing = false) :
			base(data, fileName, job, tolerateMissing) { }

		#region Overrides of JSONVehicleDataV7

		public override bool EngineStopStart
		{
			get { return Body.GetEx<bool>("EngineStopStart"); }
		}

		public override EcoRollType EcoRoll
		{
			get { return EcorollTypeHelper.Parse(Body.GetEx<string>("EcoRoll")); }
		}

		public override PredictiveCruiseControlType PredictiveCruiseControl
		{
			get { return Body.GetEx<string>("PredictiveCruiseControl").ParseEnum<PredictiveCruiseControlType>(); }
		}


		public override TankSystem? TankSystem
		{
			get { return Body["TankSystem"]?.ToString().ParseEnum<TankSystem>(); }
		}

		#endregion
	}

	// ###################################################################
	// ###################################################################


	public class JSONVehicleDataV7 : JSONFile, IVehicleEngineeringInputData, IRetarderInputData, IAngledriveInputData,
		IPTOTransmissionInputData, IAirdragEngineeringInputData, IAdvancedDriverAssistantSystemDeclarationInputData,
		IVehicleComponentsDeclaration, IVehicleComponentsEngineering, IAxlesEngineeringInputData, IAxlesDeclarationInputData,
		IAdvancedDriverAssistantSystemsEngineering
	{
		public JSONVehicleDataV7(JObject data, string fileName, IJSONVehicleComponents job, bool tolerateMissing = false)
			: base(data, fileName, tolerateMissing)
		{
			Job = job;
		}

		private IJSONVehicleComponents Job;

		#region IVehicleInputData

		public virtual string Identifier
		{
			get { return Path.GetFileNameWithoutExtension(_sourceFile); }
		}

		public virtual bool ExemptedVehicle
		{
			get { return false; }
		}

		public virtual string VIN
		{
			get { return Constants.NOT_AVailABLE; }
		}

		public virtual LegislativeClass LegislativeClass
		{
			get {
				return Body["LegislativeClass"] != null
					? Body["LegislativeClass"].Value<string>().ParseEnum<LegislativeClass>()
					: LegislativeClass.Unknown;
			}
		}

		public virtual VehicleCategory VehicleCategory
		{
			get {
				return
					(VehicleCategory)Enum.Parse(typeof(VehicleCategory), Body[JsonKeys.Vehicle_VehicleCategory].Value<string>(), true);
			}
		}

		public virtual Kilogram CurbMassChassis
		{
			get { return Body.GetEx<double>(JsonKeys.Vehicle_CurbWeight).SI<Kilogram>(); }
		}

		public virtual Kilogram CurbMassExtra
		{
			get { return Body.GetEx<double>(JsonKeys.Vehicle_CurbWeightExtra).SI<Kilogram>(); }
		}

		public virtual Kilogram GrossVehicleMassRating
		{
			get { return Body.GetEx<double>(JsonKeys.Vehicle_GrossVehicleMassRating).SI(Unit.SI.Ton).Cast<Kilogram>(); }
		}

		public virtual IList<ITorqueLimitInputData> TorqueLimits
		{
			get {
				var retVal = new List<ITorqueLimitInputData>();
				if (Body["TorqueLimits"] == null) {
					return retVal;
				}

				foreach (var entry in (JObject)Body["TorqueLimits"]) {
					retVal.Add(
						new TorqueLimitInputData() {
							Gear = entry.Key.ToInt(),
							MaxTorque = entry.Value.ToString().ToDouble(0).SI<NewtonMeter>()
						});
				}

				return retVal;
			}
		}

		public virtual Kilogram Loading
		{
			get { return Body.GetEx<double>(JsonKeys.Vehicle_Loading).SI<Kilogram>(); }
		}

		public virtual Meter DynamicTyreRadius
		{
			get { return Body.GetEx<double>(JsonKeys.Vehicle_DynamicTyreRadius).SI(Unit.SI.Milli.Meter).Cast<Meter>(); }
		}

		public virtual bool Articulated { get { return false; } }

		public virtual Meter Height
		{
			get { return Body["VehicleHeight"] == null ? null : Body.GetEx<double>("VehicleHeight").SI<Meter>(); }
		}

		public virtual bool? ATEcoRollReleaseLockupClutch
		{
			get { return null; }
		}

		public virtual XmlNode XMLSource { get { return null; } }

		public virtual Meter Length
		{
			get { return null; }
		}

		public virtual Meter Width
		{
			get { return null; }
		}

		public virtual Meter EntranceHeight { get { return null; } }

		IVehicleComponentsEngineering IVehicleEngineeringInputData.Components
		{
			get { return this; }
		}

		XmlNode IVehicleDeclarationInputData.XMLSource
		{
			get { return null; }
		}

		IAdvancedDriverAssistantSystemsEngineering IVehicleEngineeringInputData.ADAS
		{
			get { return this; }
		}

		public virtual AxleConfiguration AxleConfiguration
		{
			get {
				return
					AxleConfigurationHelper.Parse(
						Body.GetEx(JsonKeys.Vehicle_AxleConfiguration).GetEx<string>(JsonKeys.Vehicle_AxleConfiguration_Type));
			}
		}

		public virtual IList<IAxleEngineeringInputData> AxlesEngineering
		{
			get { return AxleWheels().Cast<IAxleEngineeringInputData>().ToList(); }
		}

		public virtual string ManufacturerAddress
		{
			get { return Constants.NOT_AVailABLE; }
		}

		public virtual PerSecond EngineIdleSpeed
		{
			get { return Body["IdlingSpeed"] != null ? Body.GetEx<double>("IdlingSpeed").RPMtoRad() : null; }
		}

		IList<IAxleDeclarationInputData> IAxlesDeclarationInputData.AxlesDeclaration
		{
			get { return AxleWheels().Cast<IAxleDeclarationInputData>().ToList(); }
		}

		private IEnumerable<AxleInputData> AxleWheels()
		{
			return
				Body.GetEx(JsonKeys.Vehicle_AxleConfiguration).GetEx(JsonKeys.Vehicle_AxleConfiguration_Axles).Select(
					(axle, idx) => new AxleInputData {
						TwinTyres = axle.GetEx<bool>(JsonKeys.Vehicle_Axles_TwinTyres),
						AxleWeightShare = axle.GetEx<double>("AxleWeightShare"),
						AxleType = axle["Type"] != null
							? axle.GetEx<string>("Type").ParseEnum<AxleType>()
							: (idx == 1 ? AxleType.VehicleDriven : AxleType.VehicleNonDriven),
						Tyre = new TyreInputData() {
							DataSource = new DataSource {
								SourceType = DataSourceType.JSONFile,
								SourceFile = Source,
								SourceVersion = Version,
							},
							Source = Source,
							AppVersion = AppVersion,
							Inertia = axle.GetEx<double>(JsonKeys.Vehicle_Axles_Inertia).SI<KilogramSquareMeter>(),
							Dimension = axle.GetEx<string>(JsonKeys.Vehicle_Axles_Wheels),
							RollResistanceCoefficient = axle.GetEx<double>(JsonKeys.Vehicle_Axles_RollResistanceCoefficient),
							TyreTestLoad = axle.GetEx<double>(JsonKeys.Vehicle_Axles_TyreTestLoad).SI<Newton>(),
						}
					});
		}

		#endregion

		#region "VehicleComponents"

		IAirdragDeclarationInputData IVehicleComponentsDeclaration.AirdragInputData
		{
			get { return this; }
		}

		IAirdragEngineeringInputData IVehicleComponentsEngineering.AirdragInputData
		{
			get { return this; }
		}

		IGearboxDeclarationInputData IVehicleComponentsDeclaration.GearboxInputData
		{
			get { return Job.Gearbox; }
		}

		public virtual ITorqueConverterDeclarationInputData TorqueConverter
		{
			get { return Job.TorqueConverter; }
		}

		IGearboxEngineeringInputData IVehicleComponentsEngineering.GearboxInputData
		{
			get { return Job.Gearbox; }
		}

		ITorqueConverterDeclarationInputData IVehicleComponentsDeclaration.TorqueConverterInputData
		{
			get { return Job.TorqueConverter; }
		}

		ITorqueConverterEngineeringInputData IVehicleComponentsEngineering.TorqueConverterInputData
		{
			get { return Job.TorqueConverter; }
		}

		IAxleGearInputData IVehicleComponentsEngineering.AxleGearInputData
		{
			get { return Job.AxleGear; }
		}

		IAngledriveInputData IVehicleComponentsEngineering.AngledriveInputData
		{
			get { return this; }
		}

		public virtual IEngineEngineeringInputData EngineInputData
		{
			get { return Job.Engine; }
		}

		IAxleGearInputData IVehicleComponentsDeclaration.AxleGearInputData
		{
			get { return Job.AxleGear; }
		}

		IAngledriveInputData IVehicleComponentsDeclaration.AngledriveInputData
		{
			get { return this; }
		}

		IEngineDeclarationInputData IVehicleComponentsDeclaration.EngineInputData
		{
			get { return Job.Engine; }
		}

		IAuxiliariesDeclarationInputData IVehicleComponentsDeclaration.AuxiliaryInputData
		{
			get { return Job.DeclarationAuxiliaries; }
		}

		IRetarderInputData IVehicleComponentsEngineering.RetarderInputData
		{
			get { return this; }
		}

		IPTOTransmissionInputData IVehicleComponentsEngineering.PTOTransmissionInputData
		{
			get { return this; }
		}

		IAxlesEngineeringInputData IVehicleComponentsEngineering.AxleWheels
		{
			get { return this; }
		}

		IElectricStorageEngineeringInputData IVehicleComponentsEngineering.ElectricStorage
		{
			get { return GetElectricStorage(); }
		}

		protected virtual IElectricStorageEngineeringInputData GetElectricStorage()
		{
			return null;
		}

		IElectricMachinesEngineeringInputData IVehicleComponentsEngineering.ElectricMachines
		{
			get { return GetElectricMachines(); }
		}

		protected virtual IElectricMachinesEngineeringInputData GetElectricMachines()
		{
			return null;
		}

		public virtual IBusAuxiliariesDeclarationData BusAuxiliaries
		{
			get { return null; }
		}

		IElectricStorageDeclarationInputData IVehicleComponentsDeclaration.ElectricStorage
		{
			get { return GetElectricStorage(); }
		}

		IElectricMachinesDeclarationInputData IVehicleComponentsDeclaration.ElectricMachines
		{
			get { return GetElectricMachines(); }
		}

		IAxlesDeclarationInputData IVehicleComponentsDeclaration.AxleWheels
		{
			get { return this; }
		}

		public virtual bool VocationalVehicle
		{
			get { return DeclarationData.Vehicle.VocationalVehicleDefault; }
		}

		public virtual bool SleeperCab
		{
			get { return DeclarationData.Vehicle.SleeperCabDefault; }
		}

		public virtual TankSystem? TankSystem
		{
			get { return DeclarationData.Vehicle.TankSystemDefault; }
		}

		public virtual IAdvancedDriverAssistantSystemDeclarationInputData ADAS
		{
			get { return this; }
		}

		public virtual bool ZeroEmissionVehicle
		{
			get { return DeclarationData.Vehicle.ZeroEmissionVehicleDefault; }
		}

		public virtual bool HybridElectricHDV
		{
			get { return DeclarationData.Vehicle.HybridElectricHDVDefault; }
		}

		public virtual bool DualFuelVehicle
		{
			get { return DeclarationData.Vehicle.DualFuelVehicleDefault; }
		}

		public virtual Watt MaxNetPower1
		{
			get { return null; }
		}

		public virtual Watt MaxNetPower2
		{
			get { return null; }
		}

		public virtual string RegisteredClass
		{
			get { return string.Empty; }
		}

		public virtual int NuberOfPassengersUpperDeck
		{
			get { return 0; }
		}

		public virtual int NumberOfPassengersLowerDeck
		{
			get { return 0; }
		}

		public virtual VehicleCode VehicleCode
		{
			get { return VehicleCode.NOT_APPLICABLE; }
		}

		public virtual FloorType FloorType { get { return FloorType.Unknown; } }

		IVehicleComponentsDeclaration IVehicleDeclarationInputData.Components
		{
			get { return this; }
		}

		IAuxiliariesEngineeringInputData IVehicleComponentsEngineering.AuxiliaryInputData
		{
			get { return Job.EngineeringAuxiliaries; }
		}

		IRetarderInputData IVehicleComponentsDeclaration.RetarderInputData
		{
			get { return this; }
		}


		IPTOTransmissionInputData IVehicleComponentsDeclaration.PTOTransmissionInputData
		{
			get { return this; }
		}

		#endregion

		#region Airdrag

		public virtual SquareMeter AirDragArea
		{
			get {
				return Body[JsonKeys.Vehicle_DragCoefficient] == null
					? null
					: Body.GetEx<double>(JsonKeys.Vehicle_DragCoefficient).SI<SquareMeter>();
			}
		}

		public virtual CrossWindCorrectionMode CrossWindCorrectionMode
		{
			get { return CrossWindCorrectionModeHelper.Parse(Body.GetEx<string>("CdCorrMode")); }
		}

		public virtual TableData CrosswindCorrectionMap
		{
			get {
				try {
					return ReadTableData(Body.GetEx<string>("CdCorrFile"), "CrosswindCorrection File");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, Body["CdCorrFile"].ToString()) + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		#endregion

		#region IRetarderInputData

		RetarderType IRetarderInputData.Type
		{
			get {
				var retarderType = Body.GetEx(JsonKeys.Vehicle_Retarder).GetEx<string>(JsonKeys.Vehicle_Retarder_Type);
				return RetarderTypeHelper.Parse(retarderType);
			}
		}

		double IRetarderInputData.Ratio
		{
			get { return Body.GetEx(JsonKeys.Vehicle_Retarder).GetEx<double>(JsonKeys.Vehicle_Retarder_Ratio); }
		}

		TableData IRetarderInputData.LossMap
		{
			get {
				if (Body[JsonKeys.Vehicle_Retarder] != null &&
					Body.GetEx(JsonKeys.Vehicle_Retarder)[JsonKeys.Vehicle_Retarder_LossMapFile] != null) {
					var lossmapFile = Body.GetEx(JsonKeys.Vehicle_Retarder)[JsonKeys.Vehicle_Retarder_LossMapFile];
					if (string.IsNullOrWhiteSpace(lossmapFile.Value<string>())) {
						return null;
					}

					try {
						return ReadTableData(lossmapFile.Value<string>(), "LossMap");
					} catch (Exception) {
						if (!TolerateMissing) {
							throw;
						}

						return new TableData(
							Path.Combine(BasePath, lossmapFile.Value<string>()) + MissingFileSuffix,
							DataSourceType.Missing);
					}
				}

				return null;
			}
		}

		#endregion

		#region IAngledriveInputData

		AngledriveType IAngledriveInputData.Type
		{
			get {
				var angleDrive = Body[JsonKeys.Vehicle_Angledrive];
				if (angleDrive == null) {
					return AngledriveType.None;
				}

				return angleDrive.GetEx<string>(JsonKeys.Vehicle_Angledrive_Type).ParseEnum<AngledriveType>();
			}
		}

		double IAngledriveInputData.Ratio
		{
			get {
				var angleDrive = Body[JsonKeys.Vehicle_Angledrive];
				if (angleDrive == null) {
					return double.NaN;
				}

				return Body.GetEx(JsonKeys.Vehicle_Angledrive).GetEx<double>(JsonKeys.Vehicle_Angledrive_Ratio);
			}
		}

		TableData IAngledriveInputData.LossMap
		{
			get {
				var angleDrive = Body[JsonKeys.Vehicle_Angledrive];
				if (angleDrive == null || angleDrive[JsonKeys.Vehicle_Angledrive_LossMapFile] == null) {
					return null;
				}

				var lossmapFile = angleDrive[JsonKeys.Vehicle_Angledrive_LossMapFile];
				if (string.IsNullOrWhiteSpace(lossmapFile.Value<string>())) {
					return null;
				}

				try {
					return ReadTableData(lossmapFile.Value<string>(), "LossMap");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, lossmapFile.Value<string>()) + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		double IAngledriveInputData.Efficiency
		{
			get { return Body.GetEx(JsonKeys.Vehicle_Angledrive).GetEx<double>(JsonKeys.Vehicle_Angledrive_Efficiency); }
		}

		#endregion

		#region IPTOTransmissionInputData

		string IPTOTransmissionInputData.PTOTransmissionType
		{
			get {
				var pto = Body[JsonKeys.Vehicle_PTO];
				if (pto == null) {
					return "None";
				}

				return pto[JsonKeys.Vehicle_PTO_Type].Value<string>();
			}
		}

		TableData IPTOTransmissionInputData.PTOLossMap
		{
			get {
				var pto = Body[JsonKeys.Vehicle_PTO];
				if (pto == null || pto[JsonKeys.Vehicle_PTO_LossMapFile] == null) {
					return null;
				}

				var lossmapFile = pto[JsonKeys.Vehicle_PTO_LossMapFile];
				if (string.IsNullOrWhiteSpace(lossmapFile.Value<string>())) {
					return null;
				}

				try {
					return ReadTableData(Body.GetEx(JsonKeys.Vehicle_PTO).GetEx<string>(JsonKeys.Vehicle_PTO_LossMapFile), "LossMap");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, lossmapFile.Value<string>()) + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		public virtual TableData PTOCycle
		{
			get {
				var pto = Body[JsonKeys.Vehicle_PTO];
				if (pto == null || pto[JsonKeys.Vehicle_PTO_Cycle] == null) {
					return null;
				}

				var cycle = pto[JsonKeys.Vehicle_PTO_Cycle];
				if (string.IsNullOrWhiteSpace(cycle.Value<string>())) {
					return null;
				}

				try {
					return ReadTableData(Body.GetEx(JsonKeys.Vehicle_PTO).GetEx<string>(JsonKeys.Vehicle_PTO_Cycle), "Cycle");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(Path.Combine(BasePath, cycle.Value<string>()) + MissingFileSuffix, DataSourceType.Missing);
				}
			}
		}

		#endregion

		public virtual string Manufacturer
		{
			get { return Constants.NOT_AVailABLE; }
		}

		public virtual string Model
		{
			get { return Constants.NOT_AVailABLE; }
		}

		public virtual DateTime Date
		{
			get { return DateTime.MinValue; }
		}

		public CertificationMethod CertificationMethod
		{
			get { return CertificationMethod.NotCertified; }
		}

		public virtual string CertificationNumber
		{
			get { return Constants.NOT_AVailABLE; }
		}

		public virtual DigestData DigestValue
		{
			get { return null; }
		}

		#region Implementation of IAdvancedDriverAssistantSystemDeclarationInputData

		public virtual bool EngineStopStart
		{
			get { return DeclarationData.Vehicle.ADAS.EngineStopStartDefault; }
		}

		public virtual EcoRollType EcoRoll
		{
			get { return DeclarationData.Vehicle.ADAS.EcoRoll; }
		}

		public virtual PredictiveCruiseControlType PredictiveCruiseControl
		{
			get { return DeclarationData.Vehicle.ADAS.PredictiveCruiseControlDefault; }
		}

		#endregion
	}
}
