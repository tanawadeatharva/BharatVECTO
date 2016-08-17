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
using System.Data;
using System.Linq;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONVehicleDataV7 : JSONFile, IVehicleEngineeringInputData, IRetarderInputData, IAngularGearInputData,
		IPTOTransmissionInputData
	{
		public JSONVehicleDataV7(JObject data, string fileName) : base(data, fileName) {}

		#region IVehicleInputData

		public VehicleCategory VehicleCategory
		{
			get
			{
				return
					(VehicleCategory)Enum.Parse(typeof(VehicleCategory), Body[JsonKeys.Vehicle_VehicleCategory].Value<string>(), true);
			}
		}

		public virtual Kilogram CurbWeightChassis
		{
			get { return Body.GetEx<double>(JsonKeys.Vehicle_CurbWeight).SI<Kilogram>(); }
		}

		public virtual Kilogram CurbWeightExtra
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

		public virtual SquareMeter AirDragArea
		{
			get { return Body.GetEx<double>(JsonKeys.Vehicle_DragCoefficient).SI<SquareMeter>(); }
		}

		public virtual SquareMeter AirDragAreaRigidTruck
		{
			get { return Body.GetEx<double>(JsonKeys.Vehicle_DragCoefficientRigidTruck).SI<SquareMeter>(); }
		}

		public virtual CrossWindCorrectionMode CrossWindCorrectionMode
		{
			get { return CrossWindCorrectionModeHelper.Parse(Body.GetEx<string>("CdCorrMode")); }
		}

		public virtual string Rim
		{
			get { return Body.GetEx<string>(JsonKeys.Vehicle_Rim); }
		}

		public virtual AxleConfiguration AxleConfiguration
		{
			get
			{
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
						Inertia = axle.GetEx<double>(JsonKeys.Vehicle_Axles_Inertia).SI<KilogramSquareMeter>(),
						Wheels = axle.GetEx<string>(JsonKeys.Vehicle_Axles_Wheels),
						TwinTyres = axle.GetEx<bool>(JsonKeys.Vehicle_Axles_TwinTyres),
						RollResistanceCoefficient = axle.GetEx<double>(JsonKeys.Vehicle_Axles_RollResistanceCoefficient),
						TyreTestLoad = axle.GetEx<double>(JsonKeys.Vehicle_Axles_TyreTestLoad).SI<Newton>(),
						AxleWeightShare = axle.GetEx<double>("AxleWeightShare")
					});
		}

		public virtual DataTable CrosswindCorrectionMap
		{
			get { return ReadTableData(Body.GetEx<string>("CdCorrFile"), "CrosswindCorrection File"); }
		}

		#endregion

		#region IRetarderInputData

		RetarderType IRetarderInputData.Type
		{
			get
			{
				var retarderType = Body.GetEx(JsonKeys.Vehicle_Retarder).GetEx<string>(JsonKeys.Vehicle_Retarder_Type);
				switch (retarderType.ToLowerInvariant()) {
					case "primary":
						return RetarderType.TransmissionInputRetarder;
					case "secondary":
						return RetarderType.TransmissionOutputRetarder;
					default:
						return retarderType.ParseEnum<RetarderType>();
				}
			}
		}

		double IRetarderInputData.Ratio
		{
			get { return Body.GetEx(JsonKeys.Vehicle_Retarder).GetEx<double>(JsonKeys.Vehicle_Retarder_Ratio); }
		}

		DataTable IRetarderInputData.LossMap
		{
			get
			{
				return
					ReadTableData(
						Body.GetEx(JsonKeys.Vehicle_Retarder)
							.GetEx<string>(JsonKeys.Vehicle_Retarder_LossMapFile),
						"LossMap");
			}
		}

		#endregion

		#region IAngularGearInputData

		AngularGearType IAngularGearInputData.Type
		{
			get
			{
					return Body.GetEx(JsonKeys.Vehicle_AngularGear)
						.GetEx<string>(JsonKeys.Vehicle_AngularGear_Type)
						.ParseEnum<AngularGearType>();
			}
		}

		double IAngularGearInputData.Ratio
		{
			get { return Body.GetEx(JsonKeys.Vehicle_AngularGear).GetEx<double>(JsonKeys.Vehicle_AngularGear_Ratio); }
		}

		DataTable IAngularGearInputData.LossMap
		{
			get
			{
				return ReadTableData(
					Body.GetEx(JsonKeys.Vehicle_AngularGear)
						.GetEx<string>(JsonKeys.Vehicle_AngularGear_LossMapFile),
						"LossMap");
			}
		}

		double IAngularGearInputData.Efficiency
		{
			get { return Body.GetEx(JsonKeys.Vehicle_AngularGear).GetEx<double>(JsonKeys.Vehicle_AngularGear_Efficiency); }
		}

		#endregion

		#region IPTOTransmissionInputData

		string IPTOTransmissionInputData.PTOTransmissionType
		{
			get
			{
				try {
					return Body.GetEx(JsonKeys.Vehicle_PTO).GetEx<string>(JsonKeys.Vehicle_PTO_Type);
				} catch (Exception) {
					return "None";
				}
			}
		}

		DataTable IPTOTransmissionInputData.PTOLossMap
		{
			get
			{
				return ReadTableData(Body.GetEx(JsonKeys.Vehicle_PTO).GetEx<string>(JsonKeys.Vehicle_PTO_LossMapFile), "LossMap");
			}
		}

		#endregion

		public string Vendor
		{
			get { return "N/A"; }
		}

		public string ModelName
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

		public string TypeId
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