using System.Data;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONEngineDataV3 : JSONFile, IEngineInputData
	{
		public JSONEngineDataV3(JObject data, string fileName) : base(data, fileName) {}

		public string ModelName
		{
			get { return Body[JsonKeys.Engine_ModelName].Value<string>(); }
		}

		public CubicMeter Displacement
		{
			get { return Body[JsonKeys.Engine_Displacement].Value<double>().SI().Cubic.Centi.Meter.Cast<CubicMeter>(); }
			// convert vom ccm to m^3}
		}

		public RoundsPerMinute IdleSpeed
		{
			get { return Body[JsonKeys.Engine_IdleSpeed].Value<double>().SI<RoundsPerMinute>(); }
		}

		public DataTable FuelConsumptionMap
		{
			get { return ReadTableData(Body[JsonKeys.Engine_FuelConsumptionMap].Value<string>(), "FuelConsumptionMap"); }
		}

		public DataTable FullLoadCurve
		{
			get { return ReadTableData(Body[JsonKeys.Engine_FullLoadCurveFile].Value<string>(), "FullLoadCurve"); }
		}

		public KilogramSquareMeter Inertia
		{
			get { return Body[JsonKeys.Engine_Inertia].Value<double>().SI<KilogramSquareMeter>(); }
		}

		public KilogramPerWattSecond WHTCMotorway
		{
			get { return Body[JsonKeys.Engine_WHTC_Motorway].Value<double>().SI<KilogramPerWattSecond>(); }
		}

		public KilogramPerWattSecond WHTCRural
		{
			get { return Body[JsonKeys.Engine_WHTC_Rural].Value<double>().SI<KilogramPerWattSecond>(); }
		}

		public KilogramPerWattSecond WHTCUrban
		{
			get { return Body[JsonKeys.Engine_WHTC_Urban].Value<double>().SI<KilogramPerWattSecond>(); }
		}
	}
}