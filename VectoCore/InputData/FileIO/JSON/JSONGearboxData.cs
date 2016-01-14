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
			get { return Body.GetEx(JsonKeys.Gearbox_Gears)[0].GetEx<double>(JsonKeys.Gearbox_Gear_Ratio); }
		}

		public DataTable LossMap
		{
			get
			{
				return ReadTableData(
					Body.GetEx(JsonKeys.Gearbox_Gears)[0].GetEx<string>(JsonKeys.Gearbox_Gear_LossMapFile), "AxleGear");
			}
		}

		#endregion

		#region IGearboxInputData

		public virtual string ModelName
		{
			get { return Body.GetEx<string>(JsonKeys.Gearbox_ModelName); }
		}

		public virtual GearboxType Type
		{
			get { return Body.GetEx<string>(JsonKeys.Gearbox_GearboxType).Parse<GearboxType>(); }
		}


		public virtual KilogramSquareMeter Inertia
		{
			get { return Body.GetEx<double>(JsonKeys.Gearbox_Inertia).SI<KilogramSquareMeter>(); }
		}

		public Second TractionInterruption
		{
			get { return Body.GetEx<double>(JsonKeys.Gearbox_TractionInterruption).SI<Second>(); }
		}

		public virtual IList<ITransmissionInputData> Gears
		{
			get
			{
				var i = 0;
				return (from gear in Body.GetEx(JsonKeys.Gearbox_Gears)
					where i++ != 0
					let lossMap =
						ReadTableData(gear.GetEx<string>(JsonKeys.Gearbox_Gear_LossMapFile), string.Format("Gear {0} LossMap", i))
					let fullLoadCurve =
						ReadTableData(gear.GetEx<string>(JsonKeys.Gearbox_Gear_FullLoadCurveFile),
							string.Format("Gear {0} FLD", i), false)
					let shiftPolygon =
						ReadTableData(gear.GetEx<string>(JsonKeys.Gearbox_Gear_ShiftPolygonFile),
							string.Format("Gear {0} shiftPolygon", i), false)
					select new TransmissionInputData() {
						Gear = i,
						Ratio = gear.GetEx<double>(JsonKeys.Gearbox_Gear_Ratio),
						FullLoadCurve = fullLoadCurve,
						LossMap = lossMap,
						ShiftPolygon = shiftPolygon,
						TorqueConverterActive = gear.GetEx<bool>(JsonKeys.Gearbox_Gear_TCactive)
					}).Cast<ITransmissionInputData>().ToList();
			}
		}

		public virtual bool SkipGears
		{
			get { return Body.GetEx<bool>(JsonKeys.Gearbox_SkipGears); }
		}

		public virtual Second ShiftTime
		{
			get { return Body.GetEx<double>(JsonKeys.Gearbox_ShiftTime).SI<Second>(); }
		}

		public virtual bool EarlyShiftUp
		{
			get { return Body.GetEx<bool>(JsonKeys.Gearbox_EarlyShiftUp); }
		}

		public virtual double TorqueReserve
		{
			get { return Body.GetEx<double>(JsonKeys.Gearbox_TorqueReserve) / 100.0; }
		}

		public virtual MeterPerSecond StartSpeed
		{
			get { return Body.GetEx<double>(JsonKeys.Gearbox_StartSpeed).SI<MeterPerSecond>(); }
		}

		public virtual MeterPerSquareSecond StartAcceleration
		{
			get { return Body.GetEx<double>(JsonKeys.Gearbox_StartAcceleration).SI<MeterPerSquareSecond>(); }
		}

		public virtual double StartTorqueReserve
		{
			get { return Body.GetEx<double>(JsonKeys.Gearbox_StartTorqueReserve) / 100.0; }
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
						.GetEx<double>(JsonKeys.Gearbox_TorqueConverter_ReferenceRPM)
						.RPMtoRad();
			}
		}

		public virtual DataTable TCData
		{
			get
			{
				return
					ReadTableData(Body.GetEx(JsonKeys.Gearbox_TorqueConverter).GetEx<string>(JsonKeys.Gearbox_TorqueConverter_TCMap),
						"TorqueConverter Data");
			}
		}

		KilogramSquareMeter ITorqueConverterInputData.Inertia
		{
			get
			{
				return
					Body.GetEx(JsonKeys.Gearbox_TorqueConverter)
						.GetEx<double>(JsonKeys.Gearbox_TorqueConverter_Inertia)
						.SI<KilogramSquareMeter>();
			}
		}

		#endregion
	}
}