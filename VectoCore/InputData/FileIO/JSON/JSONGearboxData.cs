using System.Collections.Generic;
using System.Data;
using System.Linq;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONGearboxDataV5 : JSONFile, IGearboxInputData, IAxleGearInputData, ITorqueConverterInputData
	{
		public JSONGearboxDataV5(JObject data, string filename) : base(data, filename) {}

		#region IAxleGearInputData

		public double Ratio
		{
			get { return Body[JsonKeys.Gearbox_Gears][0][JsonKeys.Gearbox_Gear_Ratio].Value<double>(); }
		}

		public DataTable LossMap
		{
			get
			{
				return ReadTableData(Body[JsonKeys.Gearbox_Gears][0][JsonKeys.Gearbox_Gear_LossMapFile].Value<string>(), "AxleGear");
			}
		}

		#endregion

		#region IGearboxInputData

		public string ModelName
		{
			get { return Body[JsonKeys.Gearbox_ModelName].Value<string>(); }
		}

		public GearboxType Type
		{
			get { return Body[JsonKeys.Gearbox_GearboxType].Value<string>().Parse<GearboxType>(); }
		}


		public KilogramSquareMeter Inertia
		{
			get { return Body[JsonKeys.Gearbox_Inertia].Value<double>().SI<KilogramSquareMeter>(); }
		}

		public Second TractionInterruption
		{
			get { return Body[JsonKeys.Gearbox_TractionInterruption].Value<double>().SI<Second>(); }
		}

		public IList<ITransmissionInputData> Gears
		{
			get
			{
				var i = 0;
				return (from gear in Body[JsonKeys.Gearbox_Gears]
					where i++ != 0
					let lossMap =
						ReadTableData(gear[JsonKeys.Gearbox_Gear_LossMapFile].Value<string>(), string.Format("Gear {0} LossMap", i))
					let fullLoadCurve =
						ReadTableData(gear[JsonKeys.Gearbox_Gear_FullLoadCurveFile].Value<string>(),
							string.Format("Gear {0} FLD", i), false)
					let shiftPolygon =
						ReadTableData(gear[JsonKeys.Gearbox_Gear_ShiftPolygonFile].Value<string>(),
							string.Format("Gear {0} shiftPolygon", i), false)
					select new JSONTransmissionInputData() {
						Gear = i,
						Ratio = gear[JsonKeys.Gearbox_Gear_Ratio].Value<double>(),
						FullLoadCurve = fullLoadCurve,
						LossMap = lossMap,
						ShiftPolygon = shiftPolygon,
						TorqueConverterActive = gear[JsonKeys.Gearbox_Gear_TCactive].Value<bool>()
					}).Cast<ITransmissionInputData>().ToList();
			}
		}

		public bool SkipGears
		{
			get { return Body[JsonKeys.Gearbox_SkipGears].Value<bool>(); }
		}

		public Second ShiftTime
		{
			get { return Body[JsonKeys.Gearbox_ShiftTime].Value<double>().SI<Second>(); }
		}

		public bool EarlyShiftUp
		{
			get { return Body[JsonKeys.Gearbox_EarlyShiftUp].Value<bool>(); }
		}

		public double TorqueReserve
		{
			get { return Body[JsonKeys.Gearbox_TorqueReserve].Value<double>() / 100.0; }
		}

		public MeterPerSecond StartSpeed
		{
			get { return Body[JsonKeys.Gearbox_StartSpeed].Value<double>().SI<MeterPerSecond>(); }
		}

		public MeterPerSquareSecond StartAcceleration
		{
			get { return Body[JsonKeys.Gearbox_StartAcceleration].Value<double>().SI<MeterPerSquareSecond>(); }
		}

		public double StartTorqueReserve
		{
			get { return Body[JsonKeys.Gearbox_StartTorqueReserve].Value<double>() / 100.0; }
		}

		public ITorqueConverterInputData TorqueConverter
		{
			get { return this; }
		}

		#endregion

		#region ITorqueConverterInputData

		public RoundsPerMinute ReferenceRPM
		{
			get
			{
				return
					Body[JsonKeys.Gearbox_TorqueConverter][JsonKeys.Gearbox_TorqueConverter_ReferenceRPM].Value<double>()
						.SI<RoundsPerMinute>();
			}
		}

		public DataTable TCData
		{
			get
			{
				return ReadTableData(
					Body[JsonKeys.Gearbox_TorqueConverter][JsonKeys.Gearbox_TorqueConverter_TCMap].Value<string>(),
					"TorqueConverter Data");
			}
		}

		KilogramSquareMeter ITorqueConverterInputData.Inertia
		{
			get
			{
				return
					Body[JsonKeys.Gearbox_TorqueConverter][JsonKeys.Gearbox_TorqueConverter_Inertia].Value<double>()
						.SI<KilogramSquareMeter>();
			}
		}

		#endregion
	}

	public class JSONTransmissionInputData : ITransmissionInputData
	{
		public int Gear { get; internal set; }

		public double Ratio { get; internal set; }

		public DataTable LossMap { get; internal set; }

		public DataTable FullLoadCurve { get; internal set; }

		public DataTable ShiftPolygon { get; internal set; }

		public bool TorqueConverterActive { get; internal set; }
	}
}