using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Newtonsoft.Json.Linq;
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

		public Kilogram CurbWeight
		{
			get { return Body[JsonKeys.Vehicle_CurbWeight].Value<double>().SI<Kilogram>(); }
		}

		public Kilogram CurbWeightExtra
		{
			get { return Body[JsonKeys.Vehicle_CurbWeightExtra].Value<double>().SI<Kilogram>(); }
		}

		public Kilogram GrossVehicleMassRating
		{
			get { return Body[JsonKeys.Vehicle_GrossVehicleMassRating].Value<double>().SI<Ton>().Cast<Kilogram>(); }
		}

		public Kilogram Loading
		{
			get { return Body[JsonKeys.Vehicle_Loading].Value<double>().SI<Kilogram>(); }
		}

		public Meter DynamicTyreRadius
		{
			get { return Body[JsonKeys.Vehicle_DynamicTyreRadius].Value<double>().SI().Milli.Meter.Cast<Meter>(); }
		}

		public SquareMeter DragCoefficient
		{
			get { return Body[JsonKeys.Vehicle_DragCoefficient].Value<double>().SI<SquareMeter>(); }
		}

		public SquareMeter DragCoefficientRigidTruck
		{
			get { return Body[JsonKeys.Vehicle_DragCoefficientRigidTruck].Value<double>().SI<SquareMeter>(); }
		}

		public CrossWindCorrectionMode CrossWindCorrectionMode
		{
			get { throw new NotImplementedException(); }
		}

		public string Rim
		{
			get { return Body[JsonKeys.Vehicle_Rim].Value<string>(); }
		}

		public AxleConfiguration AxleConfiguration
		{
			get
			{
				return
					AxleConfigurationHelper.Parse(
						Body[JsonKeys.Vehicle_AxleConfiguration][JsonKeys.Vehicle_AxleConfiguration_Type].Value<string>());
			}
		}

		public IList<IAxleInputData> Axles
		{
			get
			{
				return
					Body[JsonKeys.Vehicle_AxleConfiguration][JsonKeys.Vehicle_AxleConfiguration_Axles].Select(
						axle => new JSONAxleInputData() {
							Inertia = axle[JsonKeys.Vehicle_Axles_Inertia].Value<double>().SI<KilogramSquareMeter>(),
							Wheels = axle[JsonKeys.Vehicle_Axles_Wheels].Value<string>(),
							TwinTyres = axle[JsonKeys.Vehicle_Axles_TwinTyres].Value<bool>(),
							RollResistanceCoefficient = axle[JsonKeys.Vehicle_Axles_RollResistanceCoefficient].Value<double>(),
							TyreTestLoad = axle[JsonKeys.Vehicle_Axles_TyreTestLoad].Value<double>().SI<Newton>()
						}).Cast<IAxleInputData>().ToList();
			}
		}

		#endregion

		#region IRetarderInputData

		public RetarderData.RetarderType Type
		{
			get
			{
				return
					(RetarderData.RetarderType)
						Enum.Parse(typeof(RetarderData.RetarderType),
							Body[JsonKeys.Vehicle_Retarder][JsonKeys.Vehicle_Retarder_Type].Value<string>(), true);
			}
		}

		public double Ratio
		{
			get { return Body[JsonKeys.Vehicle_Retarder][JsonKeys.Vehicle_Retarder_Ratio].Value<double>(); }
		}

		public DataTable LossMap
		{
			get
			{
				return ReadTableData(Body[JsonKeys.Vehicle_Retarder][JsonKeys.Vehicle_Retarder_LossMapFile].Value<string>(),
					"LossMap");
			}
		}

		#endregion
	}


	public class JSONAxleInputData : IAxleInputData
	{
		public string Wheels { get; internal set; }

		public bool TwinTyres { get; internal set; }

		public double RollResistanceCoefficient { get; internal set; }

		public Newton TyreTestLoad { get; internal set; }

		public double AxleWeightShare { get; internal set; }

		public KilogramSquareMeter Inertia { get; internal set; }
	}
}