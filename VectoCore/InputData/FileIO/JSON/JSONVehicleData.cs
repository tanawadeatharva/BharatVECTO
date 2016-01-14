using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONVehicleDataV7 : JSONFile, IVehicleInputData, IRetarderInputData
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

		public virtual Kilogram CurbWeight
		{
			get { return Body.GetEx<double>(JsonKeys.Vehicle_CurbWeight).SI<Kilogram>(); }
		}

		public virtual Kilogram CurbWeightExtra
		{
			get { return Body.GetEx<double>(JsonKeys.Vehicle_CurbWeightExtra).SI<Kilogram>(); }
		}

		public virtual Kilogram GrossVehicleMassRating
		{
			get { return Body.GetEx<double>(JsonKeys.Vehicle_GrossVehicleMassRating).SI<Ton>().Cast<Kilogram>(); }
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

		public virtual IList<IAxleInputData> Axles
		{
			get
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
						}).Cast<IAxleInputData>().ToList();
			}
		}

		public virtual DataTable CrosswindCorrectionMap
		{
			get { return ReadTableData(Body.GetEx<string>("CdCorrFile"), "CrosswindCorrection File"); }
		}

		#endregion

		#region IRetarderInputData

		public virtual RetarderData.RetarderType Type
		{
			get
			{
				return
					(RetarderData.RetarderType)
						Enum.Parse(typeof(RetarderData.RetarderType),
							Body.GetEx(JsonKeys.Vehicle_Retarder).GetEx<string>(JsonKeys.Vehicle_Retarder_Type), true);
			}
		}

		public virtual double Ratio
		{
			get { return Body.GetEx(JsonKeys.Vehicle_Retarder).GetEx<double>(JsonKeys.Vehicle_Retarder_Ratio); }
		}

		public virtual DataTable LossMap
		{
			get
			{
				return
					ReadTableData(Body.GetEx(JsonKeys.Vehicle_Retarder).GetEx<string>(JsonKeys.Vehicle_Retarder_LossMapFile),
						"LossMap");
			}
		}

		#endregion
	}
}