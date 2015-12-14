using System.Collections.Generic;
using System.Data;
using System.Linq;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	/// <summary>
	///		Represents the Data containing all parameters of the gearbox
	/// </summary>
	/// {
	///  "Header": {
	///    "CreatedBy": "Raphael Luz IVT TU-Graz (85407225-fc3f-48a8-acda-c84a05df6837)",
	///    "Date": "29.07.2014 16:59:17",
	///    "AppVersion": "2.0.4-beta",
	///    "FileVersion": 4
	///  },
	///  "Body": {
	///    "SavedInDeclMode": false,
	///    "ModelName": "Generic 24t Coach",
	///		"GearboxType": "AMT",
	///    "Gears": [
	///      {
	///        "Ratio": 3.240355,
	///        "LossMap": "Axle.vtlm"
	///      },
	///      {
	///        "Ratio": 6.38,
	///        "LossMap": "Indirect GearData.vtlm",
	///      },
	///		...
	///		]
	/// }
	public class JSONGearboxDataV5 : JSONFile, IGearboxInputData, IAxleGearInputData, ITorqueConverterInputData
	{
		public JSONGearboxDataV5(JObject data, string filename) : base(data, filename) {}

		#region IAxleGearInputData

		public virtual double Ratio
		{
			get { return Body.GetEx(JsonKeys.Gearbox_Gears)[0].GetEx(JsonKeys.Gearbox_Gear_Ratio).Value<double>(); }
		}

		public DataTable LossMap
		{
			get
			{
				return ReadTableData(
					Body.GetEx(JsonKeys.Gearbox_Gears)[0].GetEx(JsonKeys.Gearbox_Gear_LossMapFile).Value<string>(), "AxleGear");
			}
		}

		#endregion

		#region IGearboxInputData

		public virtual string ModelName
		{
			get { return Body.GetEx(JsonKeys.Gearbox_ModelName).Value<string>(); }
		}

		public virtual GearboxType Type
		{
			get { return Body.GetEx(JsonKeys.Gearbox_GearboxType).Value<string>().Parse<GearboxType>(); }
		}


		public virtual KilogramSquareMeter Inertia
		{
			get { return Body.GetEx(JsonKeys.Gearbox_Inertia).Value<double>().SI<KilogramSquareMeter>(); }
		}

		public Second TractionInterruption
		{
			get { return Body.GetEx(JsonKeys.Gearbox_TractionInterruption).Value<double>().SI<Second>(); }
		}

		public virtual IList<ITransmissionInputData> Gears
		{
			get
			{
				var i = 0;
				return (from gear in Body.GetEx(JsonKeys.Gearbox_Gears)
					where i++ != 0
					let lossMap =
						ReadTableData(gear.GetEx(JsonKeys.Gearbox_Gear_LossMapFile).Value<string>(), string.Format("Gear {0} LossMap", i))
					let fullLoadCurve =
						ReadTableData(gear.GetEx(JsonKeys.Gearbox_Gear_FullLoadCurveFile).Value<string>(),
							string.Format("Gear {0} FLD", i), false)
					let shiftPolygon =
						ReadTableData(gear.GetEx(JsonKeys.Gearbox_Gear_ShiftPolygonFile).Value<string>(),
							string.Format("Gear {0} shiftPolygon", i), false)
					select new TransmissionInputData() {
						Gear = i,
						Ratio = gear.GetEx(JsonKeys.Gearbox_Gear_Ratio).Value<double>(),
						FullLoadCurve = fullLoadCurve,
						LossMap = lossMap,
						ShiftPolygon = shiftPolygon,
						TorqueConverterActive = gear.GetEx(JsonKeys.Gearbox_Gear_TCactive).Value<bool>()
					}).Cast<ITransmissionInputData>().ToList();
			}
		}

		public virtual bool SkipGears
		{
			get { return Body.GetEx(JsonKeys.Gearbox_SkipGears).Value<bool>(); }
		}

		public virtual Second ShiftTime
		{
			get { return Body.GetEx(JsonKeys.Gearbox_ShiftTime).Value<double>().SI<Second>(); }
		}

		public virtual bool EarlyShiftUp
		{
			get { return Body.GetEx(JsonKeys.Gearbox_EarlyShiftUp).Value<bool>(); }
		}

		public virtual double TorqueReserve
		{
			get { return Body.GetEx(JsonKeys.Gearbox_TorqueReserve).Value<double>() / 100.0; }
		}

		public virtual MeterPerSecond StartSpeed
		{
			get { return Body.GetEx(JsonKeys.Gearbox_StartSpeed).Value<double>().SI<MeterPerSecond>(); }
		}

		public virtual MeterPerSquareSecond StartAcceleration
		{
			get { return Body.GetEx(JsonKeys.Gearbox_StartAcceleration).Value<double>().SI<MeterPerSquareSecond>(); }
		}

		public virtual double StartTorqueReserve
		{
			get { return Body.GetEx(JsonKeys.Gearbox_StartTorqueReserve).Value<double>() / 100.0; }
		}

		public virtual ITorqueConverterInputData TorqueConverter
		{
			get { return this; }
		}

		#endregion

		#region ITorqueConverterInputData

		public virtual bool Enabled
		{
			get
			{
				return false; // TODO @@@
			}
		}

		public virtual PerSecond ReferenceRPM
		{
			get
			{
				return
					Body.GetEx(JsonKeys.Gearbox_TorqueConverter)
						.GetEx(JsonKeys.Gearbox_TorqueConverter_ReferenceRPM)
						.Value<double>()
						.RPMtoRad();
			}
		}

		public virtual DataTable TCData
		{
			get
			{
				return ReadTableData(
					Body.GetEx(JsonKeys.Gearbox_TorqueConverter).GetEx(JsonKeys.Gearbox_TorqueConverter_TCMap).Value<string>(),
					"TorqueConverter Data");
			}
		}

		KilogramSquareMeter ITorqueConverterInputData.Inertia
		{
			get
			{
				return
					Body.GetEx(JsonKeys.Gearbox_TorqueConverter).GetEx(JsonKeys.Gearbox_TorqueConverter_Inertia).Value<double>()
						.SI<KilogramSquareMeter>();
			}
		}

		#endregion
	}
}