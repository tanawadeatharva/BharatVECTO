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
			get { return Body.GetEx(JsonKeys.Vehicle_CurbWeight).Value<double>().SI<Kilogram>(); }
		}

		public virtual Kilogram CurbWeightExtra
		{
			get { return Body.GetEx(JsonKeys.Vehicle_CurbWeightExtra).Value<double>().SI<Kilogram>(); }
		}

		public virtual Kilogram GrossVehicleMassRating
		{
			get { return Body.GetEx(JsonKeys.Vehicle_GrossVehicleMassRating).Value<double>().SI<Ton>().Cast<Kilogram>(); }
		}

		public virtual Kilogram Loading
		{
			get { return Body.GetEx(JsonKeys.Vehicle_Loading).Value<double>().SI<Kilogram>(); }
		}

		public virtual Meter DynamicTyreRadius
		{
			get { return Body.GetEx(JsonKeys.Vehicle_DynamicTyreRadius).Value<double>().SI().Milli.Meter.Cast<Meter>(); }
		}

		public virtual SquareMeter AirDragArea
		{
			get { return Body.GetEx(JsonKeys.Vehicle_DragCoefficient).Value<double>().SI<SquareMeter>(); }
		}

		public virtual SquareMeter AirDragAreaRigidTruck
		{
			get { return Body.GetEx(JsonKeys.Vehicle_DragCoefficientRigidTruck).Value<double>().SI<SquareMeter>(); }
		}

		public virtual CrossWindCorrectionMode CrossWindCorrectionMode
		{
			get { return CrossWindCorrectionModeHelper.Parse(Body.GetEx("CdCorrMode").Value<string>()); }
		}

		public virtual string Rim
		{
			get { return Body.GetEx(JsonKeys.Vehicle_Rim).Value<string>(); }
		}

		public virtual AxleConfiguration AxleConfiguration
		{
			get
			{
				return
					AxleConfigurationHelper.Parse(
						Body.GetEx(JsonKeys.Vehicle_AxleConfiguration).GetEx(JsonKeys.Vehicle_AxleConfiguration_Type).Value<string>());
			}
		}

		public virtual IList<IAxleInputData> Axles
		{
			get
			{
				return
					Body.GetEx(JsonKeys.Vehicle_AxleConfiguration).GetEx(JsonKeys.Vehicle_AxleConfiguration_Axles).Select(
						axle => new AxleInputData() {
							Inertia = axle.GetEx(JsonKeys.Vehicle_Axles_Inertia).Value<double>().SI<KilogramSquareMeter>(),
							Wheels = axle.GetEx(JsonKeys.Vehicle_Axles_Wheels).Value<string>(),
							TwinTyres = axle.GetEx(JsonKeys.Vehicle_Axles_TwinTyres).Value<bool>(),
							RollResistanceCoefficient = axle.GetEx(JsonKeys.Vehicle_Axles_RollResistanceCoefficient).Value<double>(),
							TyreTestLoad = axle.GetEx(JsonKeys.Vehicle_Axles_TyreTestLoad).Value<double>().SI<Newton>(),
							AxleWeightShare = axle.GetEx("AxleWeightShare").Value<double>()
						}).Cast<IAxleInputData>().ToList();
			}
		}

		public virtual DataTable CrosswindCorrectionMap
		{
			get { return ReadTableData(Body.GetEx("CdCorrFile").Value<string>(), "CrosswindCorrection File"); }
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
							Body.GetEx(JsonKeys.Vehicle_Retarder).GetEx(JsonKeys.Vehicle_Retarder_Type).Value<string>(), true);
			}
		}

		public virtual double Ratio
		{
			get { return Body.GetEx(JsonKeys.Vehicle_Retarder).GetEx(JsonKeys.Vehicle_Retarder_Ratio).Value<double>(); }
		}

		public virtual DataTable LossMap
		{
			get
			{
				return
					ReadTableData(Body.GetEx(JsonKeys.Vehicle_Retarder).GetEx(JsonKeys.Vehicle_Retarder_LossMapFile).Value<string>(),
						"LossMap");
			}
		}

		#endregion
	}
}