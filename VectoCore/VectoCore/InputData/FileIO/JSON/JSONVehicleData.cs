/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
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
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONVehicleDataV7 : JSONFile, IVehicleEngineeringInputData, IRetarderInputData, IAngledriveInputData,
		IPTOTransmissionInputData, IAirdragEngineeringInputData
	{
		public JSONVehicleDataV7(JObject data, string fileName, bool tolerateMissing = false)
			: base(data, fileName, tolerateMissing) {}

		#region IVehicleInputData

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
			get { return Body.GetEx<double>(JsonKeys.Vehicle_GrossVehicleMassRating).SI().Ton.Cast<Kilogram>(); }
		}

		public virtual Kilogram Loading
		{
			get { return Body.GetEx<double>(JsonKeys.Vehicle_Loading).SI<Kilogram>(); }
		}

		public virtual Meter DynamicTyreRadius
		{
			get { return Body.GetEx<double>(JsonKeys.Vehicle_DynamicTyreRadius).SI().Milli.Meter.Cast<Meter>(); }
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

		IList<IAxleDeclarationInputData> IVehicleDeclarationInputData.Axles
		{
			get { return AxleWheels().Cast<IAxleDeclarationInputData>().ToList(); }
		}

		private IEnumerable<AxleInputData> AxleWheels()
		{
			return
				Body.GetEx(JsonKeys.Vehicle_AxleConfiguration).GetEx(JsonKeys.Vehicle_AxleConfiguration_Axles).Select(
					axle => new AxleInputData {
						SourceType = DataSourceType.JSONFile,
						Source = Source,
						Inertia = axle.GetEx<double>(JsonKeys.Vehicle_Axles_Inertia).SI<KilogramSquareMeter>(),
						Wheels = axle.GetEx<string>(JsonKeys.Vehicle_Axles_Wheels),
						TwinTyres = axle.GetEx<bool>(JsonKeys.Vehicle_Axles_TwinTyres),
						RollResistanceCoefficient = axle.GetEx<double>(JsonKeys.Vehicle_Axles_RollResistanceCoefficient),
						TyreTestLoad = axle.GetEx<double>(JsonKeys.Vehicle_Axles_TyreTestLoad).SI<Newton>(),
						AxleWeightShare = axle.GetEx<double>("AxleWeightShare")
					});
		}

		#endregion

		#region Airdrag

		public virtual SquareMeter AirDragArea
		{
			get { return Body.GetEx<double>(JsonKeys.Vehicle_DragCoefficient).SI<SquareMeter>(); }
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
			get { return "N/A"; }
		}

		public string Creator
		{
			get { return "N/A"; }
		}

		public string Date
		{
			get { return "N/A"; }
		}

		public string TechnicalReportId
		{
			get { return "N/A"; }
		}

		public string CertificationNumber
		{
			get { return "N/A"; }
		}

		public string DigestValue
		{
			get { return ""; }
		}

		public IntegrityStatus IntegrityStatus
		{
			get { return IntegrityStatus.Unknown; }
		}
	}
}