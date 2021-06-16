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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONVehicleDataV10_HEV_BEV : JSONVehicleDataV9
	{
		private JSONElectricStorageEngineeringInputData _batteries;
		private JSONElectricMotors _electricMotors;

		public JSONVehicleDataV10_HEV_BEV(JObject data, string fileName, IJSONVehicleComponents job, bool tolerateMissing = false) :
			base(data, fileName, job, tolerateMissing) { }

		#region Overrides of JSONVehicleDataV7

		public override double InitialSOC
		{
			get { return Body.GetEx<double>("InitialSoC") / 100.0; }
		}

		protected override IRetarderInputData GetRetarder
		{
			get
			{
				return _retarderInputData ?? (_retarderInputData = new JSONRetarderInputDataBEV(this));
			}
		}

		protected override IElectricMachinesEngineeringInputData GetElectricMachines()
		{
			return _electricMotors ?? (_electricMotors = ReadMotors());
		}

		protected override IElectricStorageEngineeringInputData GetElectricStorage()
		{
			return _batteries ?? (_batteries = ReadBatteries());
		}

		public override VectoSimulationJobType VehicleType
		{
			get
			{
				switch (Body.GetEx<String>("PowertrainConfiguration")) {
					case "ParallelHybrid": return VectoSimulationJobType.ParallelHybridVehicle;
					case "BatteryElectric": return VectoSimulationJobType.BatteryElectricVehicle;
					default: throw new VectoException("Invalid parameter value {0}", Body.GetEx<String>("PowertrainConfiguration"));
				}
			}
		}

		protected virtual JSONElectricMotors ReadMotors()
		{
			var retVal = new List<ElectricMachineEntry<IElectricMotorEngineeringInputData>>();
			foreach (var entry in Body["ElectricMotors"]) {
				var tmp = new ElectricMachineEntry<IElectricMotorEngineeringInputData>() {
					Position = PowertrainPositionHelper.Parse(entry.GetEx<string>("Position")),
					RatioADC = entry.GetEx<double>("Ratio"),
					RatioPerGear = entry["RatioPerGear"] != null
						? entry["RatioPerGear"].Select(x => x.Value<double>()).ToArray()
						: new double[] { },
					MechanicalTransmissionEfficiency = entry["MechanicalEfficiency"] != null
						? entry.GetEx<double>("MechanicalEfficiency")
						: double.NaN,
					MechanicalTransmissionLossMap = entry["MechanicalTransmissionLossMap"] != null
						? ReadTableData(Path.Combine(BasePath, entry.GetEx<string>("MechanicalTransmissionLossMap")),
							"EM ADC LossMap")
						: null,
					Count = entry.GetEx<int>("Count"),
					ElectricMachine =
						JSONInputDataFactory.ReadElectricMotorData(
							Path.Combine(BasePath, entry.GetEx<string>("MotorFile")), false)
				};
				retVal.Add(tmp);
			}

			return new JSONElectricMotors(retVal);
		}

		protected override IAdvancedDriverAssistantSystemsEngineering GetADS()
		{
			return _adasInputData ?? (_adasInputData = (VehicleType == VectoSimulationJobType.BatteryElectricVehicle
				? new JSONADASInputDataV10BEV(this)
				: base.GetADS()));
		}
		

		protected virtual JSONElectricStorageEngineeringInputData ReadBatteries()
		{
			return new JSONElectricStorageEngineeringInputData() {
				Count = Body["Battery"].GetEx<int>("NumPacks"),
				REESSPack = JSONInputDataFactory.ReadREESSData(Path.Combine(BasePath, Body["Battery"].GetEx<string>("BatteryFile")), false)
			};
		}

		public override TableData ElectricMotorTorqueLimits
		{
			get
			{
				return Body["EMTorqueLimits"] == null
					? null
					: ReadTableData(Path.Combine(BasePath, Body.GetEx<string>("EMTorqueLimits")),
						"ElectricMotorTorqueLimits");
			}
		}

		public override TableData MaxPropulsionTorque
		{
			get
			{
				return Body["MaxPropulsionTorque"] == null
					? null
					: ReadTableData(Path.Combine(BasePath, Body.GetEx<string>("MaxPropulsionTorque")),
						"MaxPropulsionTorque");
			}
		}

		#endregion
	}


	// ###################################################################
	// ###################################################################

	public class JSONVehicleDataV9 : JSONVehicleDataV7
	{
		protected IBusAuxiliariesDeclarationData _busAuxiliariesData;

		public JSONVehicleDataV9(JObject data, string fileName, IJSONVehicleComponents job, bool tolerateMissing = false) :
			base(data, fileName, job, tolerateMissing) { }

		#region Overrides of JSONVehicleDataV7

		public override IBusAuxiliariesDeclarationData BusAuxiliaries
		{
			get { return _busAuxiliariesData ?? (_busAuxiliariesData = new JSONBusAuxiliariesData(this)); }
		}

		#region Overrides of JSONVehicleDataV7

		public override bool Articulated
		{
			get { return Body.GetEx<bool>("Articulated"); }
		}

		protected override IAdvancedDriverAssistantSystemsEngineering GetADS()
		{
			return _adasInputData ?? (_adasInputData = new JSONADASInputDataV9(this));
		}

		#endregion

		#endregion
	}

	// ###################################################################
	// ###################################################################

	public class JSONVehicleDataV8 : JSONVehicleDataV7
	{
		public JSONVehicleDataV8(JObject data, string fileName, IJSONVehicleComponents job, bool tolerateMissing = false) :
			base(data, fileName, job, tolerateMissing) { }

		

		public override TankSystem? TankSystem
		{
			get { return Body["TankSystem"]?.ToString().ParseEnum<TankSystem>(); }
		}

		protected override IAdvancedDriverAssistantSystemsEngineering GetADS()
		{
			return _adasInputData ?? (_adasInputData = new JSONADASInputDataV8(this));
		}
	}

	// ###################################################################
	// ###################################################################


	public class JSONVehicleDataV7 : JSONFile, IVehicleEngineeringInputData, 
		IVehicleComponentsDeclaration, IVehicleComponentsEngineering, IAxlesEngineeringInputData, IAxlesDeclarationInputData
		//IAdvancedDriverAssistantSystemsEngineering, IAdvancedDriverAssistantSystemDeclarationInputData

	{
		public JSONVehicleDataV7(JObject data, string fileName, IJSONVehicleComponents job, bool tolerateMissing = false)
			: base(data, fileName, tolerateMissing)
		{
			Job = job;
		}

		private IJSONVehicleComponents Job;
		protected IRetarderInputData _retarderInputData;
		protected IAngledriveInputData _angledriveData;
		protected IAirdragEngineeringInputData _airdragInputData;
		protected IPTOTransmissionInputData _ptoInputData;
		protected IAdvancedDriverAssistantSystemsEngineering _adasInputData;

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

		public virtual LegislativeClass? LegislativeClass
		{
			get {
				return Body["LegislativeClass"] != null
					? Body["LegislativeClass"].Value<string>().ParseEnum<LegislativeClass>()
					: VectoCommon.Models.LegislativeClass.Unknown;
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

		public virtual TableData ElectricMotorTorqueLimits
		{
			get { return null; }
		}

		public virtual TableData MaxPropulsionTorque
		{
			get { return null; }
		}


		public virtual Meter Length
		{
			get { return null; }
		}

		public virtual Meter Width
		{
			get { return null; }
		}

		public virtual Meter EntranceHeight { get { return null; } }
		public virtual ConsumerTechnology? DoorDriveTechnology { get { return ConsumerTechnology.Unknown; } }
		public virtual VehicleDeclarationType VehicleDeclarationType { get; }


		IVehicleComponentsEngineering IVehicleEngineeringInputData.Components
		{
			get { return this; }
		}

		XmlNode IVehicleDeclarationInputData.XMLSource
		{
			get { return null; }
		}

		public GearshiftPosition PTO_DriveGear { get {
			return Body["GearDuringPTODrive"] != null ? new GearshiftPosition(Body["GearDuringPTODrive"].Value<uint>()) : null;
		} }

		public PerSecond PTO_DriveEngineSpeed { get {
			return Body["EngineSpeedDuringPTODrive"] != null ? Body.GetEx<double>("EngineSpeedDuringPTODrive").RPMtoRad() : null;
		} }

		IAdvancedDriverAssistantSystemsEngineering IVehicleEngineeringInputData.ADAS
		{
			get { return GetADS(); }
		}

		public virtual IAdvancedDriverAssistantSystemDeclarationInputData ADAS
		{
			get { return GetADS(); }
		}

		protected virtual IAdvancedDriverAssistantSystemsEngineering GetADS()
		{
			return _adasInputData ?? (_adasInputData = new JSONADASInputDataV7(this));
		}

		public virtual double InitialSOC
		{
			get { return double.NaN; }
		}

		public virtual VectoSimulationJobType VehicleType
		{
			get { return VectoSimulationJobType.ConventionalVehicle; }
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
			get { return _airdragInputData ?? (_airdragInputData = new JSONAirdragInputData(this)); }
		}

		IAirdragEngineeringInputData IVehicleComponentsEngineering.AirdragInputData
		{
			get { return _airdragInputData ?? (_airdragInputData = new JSONAirdragInputData(this));  }
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
			get { return _angledriveData ?? (_angledriveData = new JSONAngledriveInputData(this)); }
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
			get { return _angledriveData ?? (_angledriveData = new JSONAngledriveInputData(this)); }
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
			get { return GetRetarder; }
		}

		IRetarderInputData IVehicleComponentsDeclaration.RetarderInputData
		{
			get { return GetRetarder; }
		}

		protected virtual IRetarderInputData GetRetarder
		{
			get {
				return _retarderInputData ?? (_retarderInputData = new JSONRetarderInputData(this));
			}
		}

		IPTOTransmissionInputData IVehicleComponentsEngineering.PTOTransmissionInputData
		{
			get { return _ptoInputData ?? (_ptoInputData = new JSONPTOTransmissioninputData(this)); }
		}

		IPTOTransmissionInputData IVehicleComponentsDeclaration.PTOTransmissionInputData
		{
			get { return _ptoInputData ?? (_ptoInputData = new JSONPTOTransmissioninputData(this)); }
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

		public virtual bool? AirdragModifiedMultistage { get; }

		public virtual TankSystem? TankSystem
		{
			get { return DeclarationData.Vehicle.TankSystemDefault; }
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

		public string ExemptedTechnology
		{
			get { return null; }
		}

		public virtual RegistrationClass? RegisteredClass
		{
			get { return RegistrationClass.unknown; }
		}

		public virtual int? NumberPassengerSeatsUpperDeck
		{
			get { return 0; }
		}

		public virtual int? NumberPassengerSeatsLowerDeck
		{
			get { return 0; }
		}

		public int? NumberPassengersStandingLowerDeck
		{
			get { return 0; }
		}
		public int? NumberPassengersStandingUpperDeck
		{
			get { return 0; }
		}

		public virtual CubicMeter CargoVolume {
			get { return 0.SI<CubicMeter>(); }
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
					return new TableData(Path.Combine(BasePath, cycle.Value<string>()) + MissingFileSuffix, DataSourceType.Missing);
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
					return new TableData(Path.Combine(BasePath, cycle.Value<string>()) + MissingFileSuffix, DataSourceType.Missing);
				}
			}
		}
		public virtual VehicleCode? VehicleCode
		{
			get { return  VectoCommon.Models.VehicleCode.NOT_APPLICABLE; }
		}

		public virtual bool? LowEntry { get { return false; } }

		IVehicleComponentsDeclaration IVehicleDeclarationInputData.Components
		{
			get { return this; }
		}

		IAuxiliariesEngineeringInputData IVehicleComponentsEngineering.AuxiliaryInputData
		{
			get { return Job.EngineeringAuxiliaries; }
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

		public virtual XmlNode XMLSource { get { return null; } }
	}



}
