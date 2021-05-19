using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public abstract class JSONSubComponent
	{
		protected JSONVehicleDataV7 Base;

		protected JSONSubComponent(JSONVehicleDataV7 jsonFile)
		{
			Base = jsonFile;
		}

		protected JObject Body
		{
			get { return Base.Body; }
		}

		protected TableData ReadTableData(string filename, string tableType, bool required = true)
		{
			return Base.ReadTableData(filename, tableType, required);
		}

		protected string BasePath
		{
			get { return Base.BasePath; }
		}
		protected bool TolerateMissing
		{
			get { return Base.TolerateMissing; }
		}

		public DataSource DataSource
		{
			get { return Base.DataSource; }
		}
		public bool SavedInDeclarationMode
		{
			get { return Base.SavedInDeclarationMode; }
		}
		public string Manufacturer
		{
			get { return Base.Manufacturer; }
		}
		public string Model
		{
			get { return Base.Model; }
		}
		public DateTime Date
		{
			get { return Base.Date; }
		}
		public string AppVersion
		{
			get { return Base.AppVersion; }
		}
		public CertificationMethod CertificationMethod
		{
			get { return Base.CertificationMethod; }
		}
		public string CertificationNumber
		{
			get { return Base.CertificationNumber; }
		}
		public DigestData DigestValue
		{
			get { return Base.DigestValue; }
		}

		public virtual XmlNode XMLSource { get { return Base.XMLSource; } }
	}

	// ###################################################################
	// ###################################################################

	internal class JSONRetarderInputData : JSONSubComponent, IRetarderInputData
	{
		public JSONRetarderInputData(JSONVehicleDataV7 jsonFile) : base(jsonFile)
		{ }

		#region IRetarderInputData

		public virtual RetarderType Type
		{
			get
			{
				var retarderType = Body.GetEx(JsonKeys.Vehicle_Retarder).GetEx<string>(JsonKeys.Vehicle_Retarder_Type);
				return RetarderTypeHelper.Parse(retarderType);
			}
		}

		public virtual double Ratio
		{
			get { return Body.GetEx(JsonKeys.Vehicle_Retarder).GetEx<double>(JsonKeys.Vehicle_Retarder_Ratio); }
		}

		public virtual TableData LossMap
		{
			get
			{
				if (Body[JsonKeys.Vehicle_Retarder] != null &&
					Body.GetEx(JsonKeys.Vehicle_Retarder)[JsonKeys.Vehicle_Retarder_LossMapFile] != null)
				{
					var lossmapFile = Body.GetEx(JsonKeys.Vehicle_Retarder)[JsonKeys.Vehicle_Retarder_LossMapFile];
					if (string.IsNullOrWhiteSpace(lossmapFile.Value<string>()))
					{
						return null;
					}

					try
					{
						return ReadTableData(lossmapFile.Value<string>(), "LossMap");
					}
					catch (Exception)
					{
						if (!TolerateMissing)
						{
							throw;
						}

						return new TableData(
							Path.Combine(BasePath, lossmapFile.Value<string>()) + JSONFile.MissingFileSuffix,
							DataSourceType.Missing);
					}
				}

				return null;
			}
		}

		#endregion


	}

	internal class JSONRetarderInputDataBEV : JSONRetarderInputData
	{
		public JSONRetarderInputDataBEV(JSONVehicleDataV7 vehicle) : base(vehicle){}

		public override RetarderType Type
		{
			get
			{
				if (Base.VehicleType == VectoSimulationJobType.BatteryElectricVehicle)
				{
					return RetarderType.None;
				}

				return base.Type;
			}
		}

		public override double Ratio
		{
			get
			{
				if (Base.VehicleType == VectoSimulationJobType.BatteryElectricVehicle)
				{
					return 0.0;
				}
				return base.Ratio;
			}
		}
	}

	// ###################################################################
	// ###################################################################

	internal class JSONAngledriveInputData : JSONSubComponent, IAngledriveInputData
	{
		public JSONAngledriveInputData(JSONVehicleDataV7 jsonFile) : base(jsonFile)
		{}

		#region IAngledriveInputData

		public virtual AngledriveType Type
		{
			get
			{
				var angleDrive = Body[JsonKeys.Vehicle_Angledrive];
				if (angleDrive == null)
				{
					return AngledriveType.None;
				}

				return angleDrive.GetEx<string>(JsonKeys.Vehicle_Angledrive_Type).ParseEnum<AngledriveType>();
			}
		}

		public virtual double Ratio
		{
			get
			{
				var angleDrive = Body[JsonKeys.Vehicle_Angledrive];
				if (angleDrive == null)
				{
					return double.NaN;
				}

				return Body.GetEx(JsonKeys.Vehicle_Angledrive).GetEx<double>(JsonKeys.Vehicle_Angledrive_Ratio);
			}
		}

		public virtual TableData LossMap
		{
			get
			{
				var angleDrive = Body[JsonKeys.Vehicle_Angledrive];
				if (angleDrive == null || angleDrive[JsonKeys.Vehicle_Angledrive_LossMapFile] == null)
				{
					return null;
				}

				var lossmapFile = angleDrive[JsonKeys.Vehicle_Angledrive_LossMapFile];
				if (string.IsNullOrWhiteSpace(lossmapFile.Value<string>()))
				{
					return null;
				}

				try
				{
					return ReadTableData(lossmapFile.Value<string>(), "LossMap");
				}
				catch (Exception)
				{
					if (!TolerateMissing)
					{
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, lossmapFile.Value<string>()) + JSONFile.MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		public virtual double Efficiency
		{
			get { return Body.GetEx(JsonKeys.Vehicle_Angledrive).GetEx<double>(JsonKeys.Vehicle_Angledrive_Efficiency); }
		}

		#endregion
	}

	// ###################################################################
	// ###################################################################

	internal class JSONAirdragInputData : JSONSubComponent, IAirdragEngineeringInputData
	{
		public JSONAirdragInputData(JSONVehicleDataV7 vehicle) : base(vehicle) {}

		#region Airdrag

		public virtual SquareMeter AirDragArea
		{
			get
			{
				return Body[JsonKeys.Vehicle_DragCoefficient] == null
					? null
					: Body.GetEx<double>(JsonKeys.Vehicle_DragCoefficient).SI<SquareMeter>();
			}
		}

		public SquareMeter TransferredAirDragArea
		{
			get
			{
				return AirDragArea;
			}
		}

		public SquareMeter AirDragArea_0
		{
			get
			{
				return AirDragArea;
			}
		}

		public virtual CrossWindCorrectionMode CrossWindCorrectionMode
		{
			get { return CrossWindCorrectionModeHelper.Parse(Body.GetEx<string>("CdCorrMode")); }
		}

		public virtual TableData CrosswindCorrectionMap
		{
			get
			{
				try
				{
					return ReadTableData(Body.GetEx<string>("CdCorrFile"), "CrosswindCorrection File");
				}
				catch (Exception)
				{
					if (!TolerateMissing)
					{
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, Body["CdCorrFile"].ToString()) + JSONFile.MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		#endregion
	}

	// ###################################################################
	// ###################################################################

	internal class JSONPTOTransmissioninputData : JSONSubComponent, IPTOTransmissionInputData
	{
		public JSONPTOTransmissioninputData(JSONVehicleDataV7 vehicle) : base(vehicle) { }

		#region IPTOTransmissionInputData

		public virtual string PTOTransmissionType
		{
			get
			{
				var pto = Body[JsonKeys.Vehicle_PTO];
				if (pto == null)
				{
					return "None";
				}

				return pto[JsonKeys.Vehicle_PTO_Type].Value<string>();
			}
		}

		public virtual TableData PTOLossMap
		{
			get
			{
				var pto = Body[JsonKeys.Vehicle_PTO];
				if (pto == null || pto[JsonKeys.Vehicle_PTO_LossMapFile] == null)
				{
					return null;
				}

				var lossmapFile = pto[JsonKeys.Vehicle_PTO_LossMapFile];
				if (string.IsNullOrWhiteSpace(lossmapFile.Value<string>()))
				{
					return null;
				}

				try
				{
					return ReadTableData(Body.GetEx(JsonKeys.Vehicle_PTO).GetEx<string>(JsonKeys.Vehicle_PTO_LossMapFile), "LossMap");
				}
				catch (Exception)
				{
					if (!TolerateMissing)
					{
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, lossmapFile.Value<string>()) + JSONFile.MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		public virtual TableData PTOCycle
		{
			get
			{
				var pto = Body[JsonKeys.Vehicle_PTO];
				if (pto == null || pto[JsonKeys.Vehicle_PTO_Cycle] == null)
				{
					return null;
				}

				var cycle = pto[JsonKeys.Vehicle_PTO_Cycle];
				if (string.IsNullOrWhiteSpace(cycle.Value<string>()))
				{
					return null;
				}

				try
				{
					return ReadTableData(Body.GetEx(JsonKeys.Vehicle_PTO).GetEx<string>(JsonKeys.Vehicle_PTO_Cycle), "Cycle");
				}
				catch (Exception)
				{
					if (!TolerateMissing)
					{
						throw;
					}

					return new TableData(Path.Combine(BasePath, cycle.Value<string>()) + JSONFile.MissingFileSuffix, DataSourceType.Missing);
				}
			}
		}

		public virtual TableData PTOCycleDuringStop {
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
					return ReadTableData(Body.GetEx(JsonKeys.Vehicle_PTO).GetEx<string>(JsonKeys.Vehicle_PTO_Cycle), "PTO Cycle Standstill");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}
					return new TableData(Path.Combine(BasePath, cycle.Value<string>()) + JSONFile.MissingFileSuffix, DataSourceType.Missing);
				}
			}
		}

		public virtual TableData PTOCycleWhileDriving {
			get {
				var pto = Body[JsonKeys.Vehicle_PTO];
				if (pto == null || pto[JsonKeys.Vehicle_PTO_CycleDriving] == null) {
					return null;
				}
				var cycle = pto[JsonKeys.Vehicle_PTO_CycleDriving];
				if (string.IsNullOrWhiteSpace(cycle.Value<string>())) {
					return null;
				}
				try {
					return ReadTableData(Body.GetEx(JsonKeys.Vehicle_PTO).GetEx<string>(JsonKeys.Vehicle_PTO_CycleDriving), "PTO Cycle Driving");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}
					return new TableData(Path.Combine(BasePath, cycle.Value<string>()) + JSONFile.MissingFileSuffix, DataSourceType.Missing);
				}
			}
		}
		#endregion

	}

	// ###################################################################
	// ###################################################################

	internal class JSONADASInputDataV7 : JSONSubComponent, IAdvancedDriverAssistantSystemsEngineering
	{
		public JSONADASInputDataV7(JSONVehicleDataV7 vehicle) : base(vehicle) {}

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

		public virtual bool? ATEcoRollReleaseLockupClutch
		{
			get { return null; }
		}

		#endregion

	}

	// -------------------------------------------------------------------

	internal class JSONADASInputDataV8 : JSONADASInputDataV7
	{
		public JSONADASInputDataV8(JSONVehicleDataV7 vehicle) : base(vehicle) {}

		#region Overrides of JSONADASInputDataV7

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

		#endregion
	}

	// -------------------------------------------------------------------

	internal class JSONADASInputDataV9 : JSONADASInputDataV8
	{
		public JSONADASInputDataV9(JSONVehicleDataV7 vehicle) : base(vehicle) { }

		public override bool? ATEcoRollReleaseLockupClutch
		{
			get { return Body["ATEcoRollReleaseLockupClutch"]?.Value<bool>(); }
		}
	}

	// -------------------------------------------------------------------

	internal class JSONADASInputDataV10BEV : JSONADASInputDataV8
	{
		public JSONADASInputDataV10BEV(JSONVehicleDataV7 vehicle) : base(vehicle) { }

		public override bool EngineStopStart
		{
			get { return false; }
		}

		public override EcoRollType EcoRoll
		{
			get { return EcoRollType.None; }
		}
	}

	// ###################################################################
	// ###################################################################

	internal class JSONBusAuxiliariesData : JSONSubComponent, IBusAuxiliariesDeclarationData, IElectricSupplyDeclarationData,
		IElectricConsumersDeclarationData, IPneumaticSupplyDeclarationData, IPneumaticConsumersDeclarationData,
		IHVACBusAuxiliariesDeclarationData
	{
		public JSONBusAuxiliariesData(JSONVehicleDataV7 vehicle) : base(vehicle) {}

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
			get
			{
				return Body["Aux"]?["ElectricSupply"]?["Alternators"]
							.Select(x => new AlternatorInputData(x.GetEx<string>("Technology")))
							.Cast<IAlternatorDeclarationInputData>().ToList() ?? new List<IAlternatorDeclarationInputData>();
			}
		}

		#endregion

		#region Implementation of IElectricConsumersDeclarationData

        public virtual bool? InteriorLightsLED
		{
			get { return false; }
		}

        public virtual bool? DayrunninglightsLED
		{
			get { return false; }
		}

        public virtual bool? PositionlightsLED
		{
			get { return false; }
		}

        public virtual bool? HeadlightsLED
		{
			get { return false; }
		}

        public virtual bool? BrakelightsLED
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

		public CompressorDrive CompressorDrive { get; }
		public string Clutch { get; }
		public virtual double Ratio { get { return Body["Aux"]?["PneumaticSupply"]?.GetEx<double>("Ratio") ?? 0.0; } }
		public virtual string CompressorSize
		{
			get
			{
				return Body["Aux"]?["PneumaticSupply"]?.GetEx<string>("CompressorSize");
			}
		}

		public bool SmartAirCompression { get; }
		public bool SmartRegeneration { get; }

		#endregion

		#region Implementation of IPneumaticConsumersDeclarationData

		public virtual ConsumerTechnology AirsuspensionControl
		{
			get
			{
				return Body["Aux"]?["PneumaticConsumers"]?.GetEx<string>("AirsuspensionControl").ParseEnum<ConsumerTechnology>() ??
						ConsumerTechnology.Unknown;
			}
		}
		public virtual ConsumerTechnology AdBlueDosing
		{
			get
			{
				return Body["Aux"]?["PneumaticConsumers"]?.GetEx<string>("AdBlueDosing").ParseEnum<ConsumerTechnology>() ??
						ConsumerTechnology.Unknown;
			}
		}

		#endregion

		#region Implementation of IHVACBusAuxiliariesDeclarationData

        public virtual BusHVACSystemConfiguration? SystemConfiguration { get; set; }
		public virtual HeatPumpType? HeatPumpTypeDriverCompartment { get { return null; } }
		public virtual HeatPumpMode? HeatPumpModeDriverCompartment { get { return null; } }
		public virtual HeatPumpType? HeatPumpTypePassengerCompartment { get{ return null; } }
		public virtual HeatPumpMode? HeatPumpModePassengerCompartment { get { return null; } }
		public virtual Watt AuxHeaterPower { get { return null; } }
        public virtual bool? DoubleGlazing { get { return false; } }
		public virtual bool HeatPump { get { return false; } }
		public bool? OtherHeatingTechnology { get; }
		public virtual bool? AdjustableCoolantThermostat { get { return Body["Aux"]?["HVAC"]?.GetEx<bool>("AdjustableCoolantThermostat") ?? false; } }
        public virtual bool? AdjustableAuxiliaryHeater { get { return false; } }
		public virtual bool EngineWasteGasHeatExchanger { get { return Body["Aux"]?["HVAC"]?.GetEx<bool>("EngineWasteGasHeatExchanger") ?? false; } }
        public virtual bool? SeparateAirDistributionDucts { get { return false; } }
		public virtual bool? WaterElectricHeater { get; }
		public virtual bool? AirElectricHeater { get; }

		#endregion

	}

	// ###################################################################
	// ###################################################################

	public class JSONElectricMotors : IElectricMachinesEngineeringInputData
	{
		private readonly IList<ElectricMachineEntry<IElectricMotorEngineeringInputData>> _entries;

		public JSONElectricMotors(IList<ElectricMachineEntry<IElectricMotorEngineeringInputData>> entries)
		{
			_entries = entries;
		}

		IList<ElectricMachineEntry<IElectricMotorDeclarationInputData>> IElectricMachinesDeclarationInputData.Entries
		{
			get { return _entries.Cast<ElectricMachineEntry<IElectricMotorDeclarationInputData>>().ToList(); }
		}

		public virtual IList<ElectricMachineEntry<IElectricMotorEngineeringInputData>> Entries
		{
			get { return _entries; }
		}
	}

	// ###################################################################
	// ###################################################################

	public class JSONElectricStorageEngineeringInputData : IElectricStorageEngineeringInputData
	{
		public IREESSPackInputData REESSPack { get; set; }

		
		public int Count { get; internal set; }
	}

}