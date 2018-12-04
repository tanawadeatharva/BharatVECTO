/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
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
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONVehicleDataV7 : JSONFile, IVehicleEngineeringInputData, IRetarderInputData, IAngledriveInputData,
		IPTOTransmissionInputData, IAirdragEngineeringInputData, IAdvancedDriverAssistantSystemDeclarationInputData
	{
		public JSONVehicleDataV7(JObject data, string fileName, IJSONVehicleComponents job, bool tolerateMissing = false)
			: base(data, fileName, tolerateMissing)
		{
			Job = job;
		}

		private IJSONVehicleComponents Job;

		#region IVehicleInputData

		public bool ExemptedVehicle { get { return false; } }

		public string VIN
		{
			get { return "N.A."; }
		}

		public LegislativeClass LegislativeClass
		{
			get {
				return Body["LegislativeClass"] != null
					? Body["LegislativeClass"].Value<string>().ParseEnum<LegislativeClass>()
					: LegislativeClass.Unknown;
			}
		}

		public VehicleCategory VehicleCategory
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

		public IList<ITorqueLimitInputData> TorqueLimits
		{
			get {
				var retVal = new List<ITorqueLimitInputData>();
				if (Body["TorqueLimits"] == null) {
					return retVal;
				}
				foreach (var entry in (JObject)Body["TorqueLimits"]) {
					retVal.Add(new TorqueLimitInputData() {
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

		public Meter Height
		{
			get { return Body["VehicleHeight"] == null ? null : Body.GetEx<double>("VehicleHeight").SI<Meter>(); }
		}

		public virtual AxleConfiguration AxleConfiguration
		{
			get {
				return
					AxleConfigurationHelper.Parse(
						Body.GetEx(JsonKeys.Vehicle_AxleConfiguration).GetEx<string>(JsonKeys.Vehicle_AxleConfiguration_Type));
			}
		}

		public virtual IList<IAxleEngineeringInputData> Axles
		{
			get { return AxleWheels().Cast<IAxleEngineeringInputData>().ToList(); }
		}

		public string ManufacturerAddress
		{
			get { return "N.A."; }
		}

		public PerSecond EngineIdleSpeed
		{
			get { return Body["IdlingSpeed"] != null ? Body.GetEx<double>("IdlingSpeed").RPMtoRad() : null; }
		}

		IList<IAxleDeclarationInputData> IVehicleDeclarationInputData.Axles
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
							SourceType = DataSourceType.JSONFile,
							Source = Source,
							Inertia = axle.GetEx<double>(JsonKeys.Vehicle_Axles_Inertia).SI<KilogramSquareMeter>(),
							Dimension = axle.GetEx<string>(JsonKeys.Vehicle_Axles_Wheels),
							RollResistanceCoefficient = axle.GetEx<double>(JsonKeys.Vehicle_Axles_RollResistanceCoefficient),
							TyreTestLoad = axle.GetEx<double>(JsonKeys.Vehicle_Axles_TyreTestLoad).SI<Newton>(),
						}
					});
		}

		#endregion

		#region "VehicleComponents"

		IAirdragDeclarationInputData IVehicleDeclarationInputData.AirdragInputData
		{
			get { return this; }
		}

		IAirdragEngineeringInputData IVehicleEngineeringInputData.AirdragInputData
		{
			get { return this; }
		}

		IGearboxDeclarationInputData IVehicleDeclarationInputData.GearboxInputData
		{
			get { return Job.Gearbox; }
		}

		IGearboxEngineeringInputData IVehicleEngineeringInputData.GearboxInputData
		{
			get { return Job.Gearbox; }
		}

		ITorqueConverterDeclarationInputData IVehicleDeclarationInputData.TorqueConverterInputData
		{
			get { return Job.TorqueConverter; }
		}

		ITorqueConverterEngineeringInputData IVehicleEngineeringInputData.TorqueConverterInputData
		{
			get { return Job.TorqueConverter; }
		}

		IAxleGearInputData IVehicleEngineeringInputData.AxleGearInputData
		{
			get { return Job.AxleGear; }
		}

		IAngledriveInputData IVehicleEngineeringInputData.AngledriveInputData
		{
			get { return this; }
		}

		public IEngineEngineeringInputData EngineInputData
		{
			get { return Job.Engine; }
		}

		IAxleGearInputData IVehicleDeclarationInputData.AxleGearInputData
		{
			get { return Job.AxleGear; }
		}

		IAngledriveInputData IVehicleDeclarationInputData.AngledriveInputData
		{
			get { return this; }
		}

		IEngineDeclarationInputData IVehicleDeclarationInputData.EngineInputData
		{
			get { return Job.Engine; }
		}

		IAuxiliariesDeclarationInputData IVehicleDeclarationInputData.AuxiliaryInputData()
		{
			return Job.DeclarationAuxiliaries;
		}

		IRetarderInputData IVehicleEngineeringInputData.RetarderInputData
		{
			get { return this; }
		}

		IPTOTransmissionInputData IVehicleEngineeringInputData.PTOTransmissionInputData
		{
			get { return this; }
		}

		public bool VocationalVehicle { get { return DeclarationData.Vehicle.VocationalVehicleDefault; } }

		public bool SleeperCab { get { return DeclarationData.Vehicle.SleeperCabDefault; } }

		public TankSystem TankSystem { get { return DeclarationData.Vehicle.TankSystemDefault; } }
		public IAdvancedDriverAssistantSystemDeclarationInputData ADAS { get { return this; } }

		public bool ZeroEmissionVehicle { get { return DeclarationData.Vehicle.ZeroEmissionVehicleDefault; } }

		public bool HybridElectricHDV { get { return DeclarationData.Vehicle.HybridElectricHDVDefault; } }

		public bool DualFuelVehicle { get { return DeclarationData.Vehicle.DualFuelVehicleDefault; } }

		public Watt MaxNetPower1 { get { return null; } }

		public Watt MaxNetPower2 { get { return null; } }

		IAuxiliariesEngineeringInputData IVehicleEngineeringInputData.AuxiliaryInputData()
		{
			return Job.EngineeringAuxiliaries;
		}

		IRetarderInputData IVehicleDeclarationInputData.RetarderInputData
		{
			get { return this; }
		}


		IPTOTransmissionInputData IVehicleDeclarationInputData.PTOTransmissionInputData
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
					return new TableData(Path.Combine(BasePath, Body["CdCorrFile"].ToString()) + MissingFileSuffix,
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
						return new TableData(Path.Combine(BasePath, lossmapFile.Value<string>()) + MissingFileSuffix,
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
					return new TableData(Path.Combine(BasePath, lossmapFile.Value<string>()) + MissingFileSuffix,
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
					return new TableData(Path.Combine(BasePath, lossmapFile.Value<string>()) + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		public TableData PTOCycle
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

		public string Manufacturer
		{
			get { return "N/A"; }
		}

		public string Model
		{
			get { return "N.A."; }
		}

		public string Date
		{
			get { return "N/A"; }
		}

		public CertificationMethod CertificationMethod
		{
			get { return CertificationMethod.NotCertified; }
		}

		public string CertificationNumber
		{
			get { return "N.A."; }
		}

		public DigestData DigestValue
		{
			get { return null; }
		}

		#region Implementation of IAdvancedDriverAssistantSystemDeclarationInputData

		public bool EngineStopStart { get { return DeclarationData.Vehicle.ADAS.EngineStopStartDefault; } }
		public bool EcoRollWitoutEngineStop { get { return DeclarationData.Vehicle.ADAS.EcoRollWitoutEngineStop; } }
		public bool EcoRollWithEngineStop { get { return DeclarationData.Vehicle.ADAS.EcoRollWithEngineStop; } }
		public PredictiveCruiseControlType PredictiveCruiseControl { get {
			return DeclarationData.Vehicle.ADAS.PredictiveCruiseControlDefault;
		} }

		#endregion
	}
}
