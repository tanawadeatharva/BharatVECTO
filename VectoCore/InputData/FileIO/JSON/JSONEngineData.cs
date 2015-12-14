using System.Data;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	/// <summary>
	///     Represents the CombustionEngineData. Fileformat: .veng
	/// </summary>
	/// <code>
	/// {
	///  "Header": {
	///    "CreatedBy": " ()",
	///    "Date": "3/4/2015 12:26:24 PM",
	///    "AppVersion": "2.0.4-beta3",
	///    "FileVersion": 2
	///  },
	///  "Body": {
	///    "SavedInDeclMode": false,
	///    "ModelName": "Generic 24t Coach",
	///    "Displacement": 12730.0,
	///    "IdlingSpeed": 560.0,
	///    "Inertia": 3.8,
	///    "FullLoadCurves": [
	///      {
	///        "Path": "24t Coach.vfld",
	///        "Gears": "0 - 99"
	///      }
	///    ],
	///    "FuelMap": "24t Coach.vmap",
	///    "WHTC-Urban": 0.0,
	///    "WHTC-Rural": 0.0,
	///    "WHTC-Motorway": 0.0
	///  }
	/// }
	/// </code>
	public class JSONEngineDataV3 : JSONFile, IEngineInputData
	{
		public JSONEngineDataV3(JObject data, string fileName) : base(data, fileName) {}

		public virtual string ModelName
		{
			get { return Body.GetEx(JsonKeys.Engine_ModelName).Value<string>(); }
		}

		public virtual CubicMeter Displacement
		{
			get { return Body.GetEx(JsonKeys.Engine_Displacement).Value<double>().SI().Cubic.Centi.Meter.Cast<CubicMeter>(); }
			// convert vom ccm to m^3}
		}

		public virtual PerSecond IdleSpeed
		{
			get { return Body.GetEx(JsonKeys.Engine_IdleSpeed).Value<double>().RPMtoRad(); }
		}

		public virtual DataTable FuelConsumptionMap
		{
			get { return ReadTableData(Body.GetEx(JsonKeys.Engine_FuelConsumptionMap).Value<string>(), "FuelConsumptionMap"); }
		}

		public virtual DataTable FullLoadCurve
		{
			get { return ReadTableData(Body.GetEx(JsonKeys.Engine_FullLoadCurveFile).Value<string>(), "FullLoadCurve"); }
		}

		public virtual KilogramSquareMeter Inertia
		{
			get { return Body.GetEx(JsonKeys.Engine_Inertia).Value<double>().SI<KilogramSquareMeter>(); }
		}

		public virtual KilogramPerWattSecond WHTCMotorway
		{
			get { return Body.GetEx(JsonKeys.Engine_WHTC_Motorway).Value<double>().SI<KilogramPerWattSecond>(); }
		}

		public virtual KilogramPerWattSecond WHTCRural
		{
			get { return Body.GetEx(JsonKeys.Engine_WHTC_Rural).Value<double>().SI<KilogramPerWattSecond>(); }
		}

		public virtual KilogramPerWattSecond WHTCUrban
		{
			get { return Body.GetEx(JsonKeys.Engine_WHTC_Urban).Value<double>().SI<KilogramPerWattSecond>(); }
		}
	}
}